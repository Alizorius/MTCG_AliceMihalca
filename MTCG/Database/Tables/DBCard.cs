﻿using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    public class DBCard
    {
        public static List<Card> GetAllUserCards(string request)
        {
            return GetAllUserCards(Helper.ExtractUsernameToken(request));
        }

        public static List<Card> GetAllUserCardsByUsername(string username)
        {
            using var conn = DB.Connection();

            using var cardQueryCmd = new NpgsqlCommand(
                "SELECT * from cardTable WHERE username = @p1",
                conn);
            cardQueryCmd.Parameters.AddWithValue("p1", username);
            cardQueryCmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;

            return GetCards(cardQueryCmd, username);
        }

        public static Deck GetDeckFromUsername(string username)
        {
            using var conn = DB.Connection();

            using var cardQueryCmd = new NpgsqlCommand(
                "SELECT * from cardTable WHERE username = @p1 AND deck = true",
                conn);
            cardQueryCmd.Parameters.AddWithValue("p1", username);
            cardQueryCmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;

            if (GetCards(cardQueryCmd, username).Count() == 0)
            {
                return null;
            }
            return new Deck(GetCards(cardQueryCmd, username), username);
        }

        public static Deck GetDeck(string request)
        {
            string username = Helper.ExtractUsernameToken(request);
            return GetDeckFromUsername(username);
        }

        public static Card GetCardById(string cardId, string username)
        {
            using var conn = DB.Connection();

            using var cardQueryCmd = new NpgsqlCommand(
                "SELECT * from cardTable WHERE id = @p1 AND username = @p2 LIMIT 1",
                conn);
            cardQueryCmd.Parameters.AddWithValue("p1", cardId);
            cardQueryCmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;
            cardQueryCmd.Parameters.AddWithValue("p2", username);
            cardQueryCmd.Parameters[1].NpgsqlDbType = NpgsqlDbType.Varchar;

            if(GetCards(cardQueryCmd, username).Count() == 0)
            {
                return null;
            }
            return GetCards(cardQueryCmd, username).ElementAt(0);
        }

        private static List<Card> GetCards(NpgsqlCommand cardQueryCmd, string username)
        {
            NpgsqlDataReader? reader = null;

            reader = cardQueryCmd.ExecuteReader();
            var cards = new List<Card>();
            while (reader.Read())
            {
                int packageId;
                ElementType elementType = (ElementType)Enum.Parse(typeof(ElementType), reader.GetString(3));

                if (reader.IsDBNull(5))
                {
                    packageId = 0;
                }
                else
                {
                    packageId = reader.GetInt32(5);
                }

                if (!reader.IsDBNull(4))
                {
                    MonsterType monsterType = (MonsterType)Enum.Parse(typeof(MonsterType), reader.GetString(4));

                    cards.Add(new Monster(
                        reader.GetString(0), reader.GetString(1), reader.GetDouble(2),
                        elementType, monsterType, packageId, username, reader.GetBoolean(7)
                    ));
                }
                else
                {
                    cards.Add(new Spell(
                        reader.GetString(0), reader.GetString(1), reader.GetDouble(2),
                        elementType, packageId, username, reader.GetBoolean(7)
                    ));
                }
            }
            reader.Close();
            return cards;
        }

        public static bool ConfigureDeck(string request)
        {
            string username = Helper.ExtractUsernameToken(request);
            string[] cardIds = Helper.ExtractCardIds(request);

            using var conn = DB.Connection();

            if (cardIds.Length < 4)
            {
                return false;
            }

            List<Card> cards = GetAllUserCards(request);
            foreach (var Id in cardIds)
            {
                if (!cards.Exists(c => c.Id == Id))
                {
                    return false;
                }
            }

            using var cmd = new NpgsqlCommand(
                "UPDATE cardTable " +
                "SET deck = false WHERE username = @p1",
                conn);
            cmd.Parameters.AddWithValue("p1", username);
            cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;
            cmd.ExecuteNonQuery();

            foreach (var Id in cardIds)
            {
                using var cardUpdateCmd = new NpgsqlCommand(
                "UPDATE cardTable " +
                "SET deck = true WHERE username = @p1 AND id = @p2",
                conn);
                cardUpdateCmd.Parameters.AddWithValue("p1", username);
                cardUpdateCmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;
                cardUpdateCmd.Parameters.AddWithValue("p2", Id);
                cardUpdateCmd.Parameters[1].NpgsqlDbType = NpgsqlDbType.Varchar;

                cardUpdateCmd.ExecuteNonQuery();
            }
            return true;
        }

        public static bool AddSingleSpellCard(Spell spell)
        {
            using var conn = DB.Connection();

            using var cardTableCmd = new NpgsqlCommand(
                "INSERT INTO cardTable (id, cardname, damage, elementType, packageId, username, deck) " +
                "VALUES(@p1, @p2, @p3, @p4, @p5, @p6, @p7)",
                conn);

            cardTableCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Varchar, spell.Id);
            cardTableCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Varchar, spell.Name);
            cardTableCmd.Parameters.AddWithValue("p3", NpgsqlDbType.Double, spell.Damage);
            cardTableCmd.Parameters.AddWithValue("p4", NpgsqlDbType.Varchar, spell.ElementType.ToString());
            cardTableCmd.Parameters.AddWithValue("p5", NpgsqlDbType.Integer, spell.PackageId);
            cardTableCmd.Parameters.AddWithValue("p6", NpgsqlDbType.Varchar, spell.Username);
            cardTableCmd.Parameters.AddWithValue("p7", NpgsqlDbType.Boolean, spell.InDeck);
            
            cardTableCmd.ExecuteNonQuery();

            return true;
        }

        public static void DeleteCardById(string cardId)
        {
            using var conn = DB.Connection();
            using var cmd = new NpgsqlCommand(
                "DELETE FROM cardTable WHERE id = @p1",
                conn);
            cmd.Parameters.AddWithValue("p1", cardId);
            cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;
            cmd.ExecuteNonQuery();
        }
        
    }
}
