using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    class DBTrade
    {
        public static bool AddTradingDeal(string request)
        {
            Deal deal = Helper.ExtractTradingDeal(request);
            List<Card> userCards = DBCard.GetAllUserCards(request);
            Deck userDeck = DBCard.GetDeck(request);

            if (!userCards.Exists(c => c.Id == deal.CardToTrade))
            {
                return false;
            }
            if(userDeck.DeckList.Exists(c => c.Id == deal.CardToTrade))
            {
                return false;
            }

            using var conn = DB.Connection();

            using var tradingTableCmd = new NpgsqlCommand(
                "INSERT INTO tradingTable (id, cardId, type, minDamage) " +
                "VALUES(@p1, @p2, @p3, @p4)",
                conn);

            tradingTableCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Varchar, deal.Id);
            tradingTableCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Varchar, deal.CardToTrade);
            tradingTableCmd.Parameters.AddWithValue("p3", NpgsqlDbType.Varchar, deal.Type);
            tradingTableCmd.Parameters.AddWithValue("p4", NpgsqlDbType.Integer, deal.MinimumDamage);

            tradingTableCmd.ExecuteNonQuery();

            return true;
        }

        public static List<Deal> GetAllDeals()
        {
            using var conn = DB.Connection();

            using var dealQueryCmd = new NpgsqlCommand(
                "SELECT * from tradingTable",
                conn);

            NpgsqlDataReader? reader = null;

            reader = dealQueryCmd.ExecuteReader();
            var deals = new List<Deal>();

            while (reader.Read())
            {
                deals.Add(new Deal(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3)));
            }
            reader.Close();
            return deals;
        }

        public static Deal GetDealById(string dealId)
        {
            using var conn = DB.Connection();

            using var dealQueryCmd = new NpgsqlCommand(
                "SELECT * from tradingTable WHERE id = @p1",
                conn);
            dealQueryCmd.Parameters.AddWithValue("p1", dealId);
            dealQueryCmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;

            NpgsqlDataReader? reader = null;

            reader = dealQueryCmd.ExecuteReader();
            if (reader.Read())
            {
                Deal deal = new Deal(dealId, reader.GetString(1), reader.GetString(2), reader.GetInt32(3));
                reader.Close();
                return deal;
            }

            reader.Close();
            return null;
        }

        public static bool AcceptTradingDeal(string request)
        {
            string username = Helper.ExtractUsernameToken(request);
            Deal deal = GetDealById(Helper.ExtractDealId(request));
            Card tradingCard = DBCard.GetCardById(Helper.ExtractTradingCardId(request), username);
            string ownerOfDeal = DBUser.GetUsernamebyCardId(deal.CardToTrade);

            if(tradingCard is null)
            {
                return false;
            }
            if (username.Equals(ownerOfDeal))
            {
                return false;
            }
            if((tradingCard is Monster && deal.Type.Equals("monster") || tradingCard is Spell && deal.Type.Equals("spell"))
                && tradingCard.Damage >= deal.MinimumDamage)
            {
                using var conn = DB.Connection();
                using var cmd = new NpgsqlCommand(
                "UPDATE cardTable " +
                "SET username = @p1 WHERE id = @p2",
                conn);
                cmd.Parameters.AddWithValue("p1", ownerOfDeal);
                cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;
                cmd.Parameters.AddWithValue("p2", tradingCard.Id);
                cmd.Parameters[1].NpgsqlDbType = NpgsqlDbType.Varchar;
                cmd.ExecuteNonQuery();

                using var conn1 = DB.Connection();
                using var cmd1 = new NpgsqlCommand(
                "UPDATE cardTable " +
                "SET username = @p1 WHERE id = @p2",
                conn1);
                cmd1.Parameters.AddWithValue("p1", username);
                cmd1.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;
                cmd1.Parameters.AddWithValue("p2", deal.CardToTrade);
                cmd1.Parameters[1].NpgsqlDbType = NpgsqlDbType.Varchar;
                cmd1.ExecuteNonQuery();

                return DeleteTradingDeal(deal.Id);
            }
            return false;
        }

        public static bool DeleteTradingDeal(string dealId)
        {
            using var conn = DB.Connection();
            using var cmd = new NpgsqlCommand(
            "DELETE FROM tradingTable WHERE id = @p1",
            conn);
            cmd.Parameters.AddWithValue("p1", dealId);
            cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;
            return cmd.ExecuteNonQuery() != 0;
        }
        
    }
}
