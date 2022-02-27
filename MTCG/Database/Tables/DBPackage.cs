using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    class DBPackage
    {
        public static bool AddPackage(string request)
        {
            List<Card> cards = Helper.ExtractCards(request);
            using var conn = DB.Connection();
            int packageID;

            using var cmd = new NpgsqlCommand(
                "SELECT MAX(packageID) FROM cardTable",
                conn);

            object max = cmd.ExecuteScalar();

            if (max is System.DBNull)
            {
                packageID = 1;
            }
            else
            {
                packageID = (int)max + 1;
            }

            foreach (var card in cards)
            {
                using var cardCmd = new NpgsqlCommand(
                    "INSERT INTO cardTable (id, cardname, damage, elementType, monsterType, packageID, username, deck) " +
                    "VALUES(@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8)"
                    , conn);
                cardCmd.Parameters.AddWithValue("p1", card.Id);
                cardCmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;

                cardCmd.Parameters.AddWithValue("p2", card.Name);
                cardCmd.Parameters[1].NpgsqlDbType = NpgsqlDbType.Varchar;

                cardCmd.Parameters.AddWithValue("p3", card.Damage);
                cardCmd.Parameters[2].NpgsqlDbType = NpgsqlDbType.Double;

                cardCmd.Parameters.AddWithValue("p4", card.ElementType.ToString());
                cardCmd.Parameters[3].NpgsqlDbType = NpgsqlDbType.Varchar;

                if (card is Monster)
                {
                    Monster m = card as Monster;
                    cardCmd.Parameters.AddWithValue("p5", m.MonsterType.ToString());
                }
                else
                {
                    cardCmd.Parameters.AddWithValue("p5", DBNull.Value);
                }
                cardCmd.Parameters[4].NpgsqlDbType = NpgsqlDbType.Varchar;

                cardCmd.Parameters.AddWithValue("p6", packageID);
                cardCmd.Parameters[5].NpgsqlDbType = NpgsqlDbType.Integer;

                cardCmd.Parameters.AddWithValue("p7", DBNull.Value);
                cardCmd.Parameters[6].NpgsqlDbType = NpgsqlDbType.Integer;

                cardCmd.Parameters.AddWithValue("p8", false);
                cardCmd.Parameters[7].NpgsqlDbType = NpgsqlDbType.Boolean;

                cardCmd.ExecuteNonQuery();
            }

            return true; //must also return false if failed?
        }

        public static bool AcquirePackage(string request)
        {
            User user = DBUser.GetUser(Helper.ExtractUsernameToken(request));

            if (user.Coins < 5)
            {
                return false;
            }

            using var conn = DB.Connection();

            using var cmd = new NpgsqlCommand(
                "SELECT MIN(packageID) FROM cardTable",
                conn);

            object min = cmd.ExecuteScalar();

            if (min is System.DBNull)
            {
                return false;
            }
            else
            {
                int packageId = (int)min;

                using var cardUpdateCmd = new NpgsqlCommand(
                "UPDATE cardTable " +
                "SET username = @p1, packageID = NULL WHERE packageID = @p2",
                conn);
                cardUpdateCmd.Parameters.AddWithValue("p1", user.Username);
                cardUpdateCmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;
                cardUpdateCmd.Parameters.AddWithValue("p2", packageId);
                cardUpdateCmd.Parameters[1].NpgsqlDbType = NpgsqlDbType.Integer;
                
                cardUpdateCmd.ExecuteNonQuery();

                using var userUpdateCmd = new NpgsqlCommand(
                "UPDATE userTable " +
                "SET coins = @p1 WHERE username = @p2",
                conn);
                
                userUpdateCmd.Parameters.AddWithValue("p1", user.Coins - 5);
                userUpdateCmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Integer;
                userUpdateCmd.Parameters.AddWithValue("p2", user.Username);
                userUpdateCmd.Parameters[1].NpgsqlDbType = NpgsqlDbType.Varchar;

                userUpdateCmd.ExecuteNonQuery();

                return true;
            }
        }
    }
}
