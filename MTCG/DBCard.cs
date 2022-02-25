using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    class DBCard
    {
        public static List<Card> GetAllUserCards(string username)
        {
            using var conn = DB.Connection();

            using var cardQueryCmd = new NpgsqlCommand(
                "SELECT * from cardTable WHERE username = @p1",
                conn);
            cardQueryCmd.Parameters.AddWithValue("p1", username);
            cardQueryCmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;

            return GetCards(cardQueryCmd, username);
        }

        public static Deck GetDeck(string username) 
        {
            using var conn = DB.Connection();

            using var cardQueryCmd = new NpgsqlCommand(
                "SELECT * from cardTable WHERE username = @p1, deck = true",
                conn);
            cardQueryCmd.Parameters.AddWithValue("p1", username);
            cardQueryCmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;

            return new Deck(GetCards(cardQueryCmd, username), username);
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
                        elementType, monsterType, packageId, username, false
                    ));
                }
                else
                {
                    cards.Add(new Spell(
                        reader.GetString(0), reader.GetString(1), reader.GetDouble(2),
                        elementType, packageId, username, false
                    ));
                }
            }
            reader.Close();

            if (cards == null)
            {
                //if no cards available under the given conditions
                return null;
                //error handling
            }

            return cards;
        }

        public static bool ConfigureDeck(string username, string[] cardIds)
        {
            using var conn = DB.Connection();

            //should fail if user doesnt have the cards 
            //or card number lower (or higher?) than 4

            foreach(var Id in cardIds)
            {
                using var cardUpdateCmd = new NpgsqlCommand(
                "UPDATE cardTable " +
                "SET deck = true WHERE username = @p1, id = @p2",
                conn);
                cardUpdateCmd.Parameters.AddWithValue("p1", username);
                cardUpdateCmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;
                cardUpdateCmd.Parameters.AddWithValue("p2", Id);
                cardUpdateCmd.Parameters[1].NpgsqlDbType = NpgsqlDbType.Varchar;

                cardUpdateCmd.ExecuteNonQuery();
            }

            return true;
        }
    }
}
