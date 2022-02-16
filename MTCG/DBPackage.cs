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
        public static bool AddPackage(List<Card> cards)
        {
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

        public static bool AcquirePackage(string username)
        {
            //check if player has enough money

            using var conn = DB.Connection();
            NpgsqlDataReader? dr = null;

            using var cmd = new NpgsqlCommand(
                "SELECT MAX(packageID) FROM cardTable",
                conn);

            object max = cmd.ExecuteScalar();

            if (max is System.DBNull)
            {
                //no more packages available
            }
            else
            {
                int packageId = (int)max;

                //using var cardQueryCmd = new NpgsqlCommand(
                //    "SELECT * from cardTable WHERE package = @p1",
                //    conn);
                //cardQueryCmd.Parameters.AddWithValue("p1", packageId);
                //cardQueryCmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Integer;

                //dr = cardQueryCmd.ExecuteReader();
                //var cards = new List<Card>();
                //while (dr.Read())
                //{
                //    if(dr.GetString(4))
                //    {
                //        cards.Add(new Monster(
                //            dr.GetString(0), dr.GetString(1), dr.GetDouble(2),
                //            dr.GetData(3), dr.GetData(4), dr.GetInt32(5), username, false
                //        ));
                //    }
                //    else
                //    {
                //        cards.Add(new Spell(
                //            dr.GetString(0), dr.GetString(1), dr.GetDouble(2),
                //            null, null, null, username, false
                //        ));
                //    }   
                //}
                //dr.Close();


                using var cardUpdateCmd = new NpgsqlCommand(
                "UPDATE cardTable " +
                "SET username = @p1 AND packageID = NULL WHERE packageID = @p2",
                conn);
                cardUpdateCmd.Parameters.AddWithValue("p1", username);
                cardUpdateCmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;
                cardUpdateCmd.Parameters.AddWithValue("p2", packageId);
                cardUpdateCmd.Parameters[1].NpgsqlDbType = NpgsqlDbType.Integer;
                
                cardUpdateCmd.ExecuteNonQuery();
                
            }


            //unfinished

            return true; //must also return false if failed?
        }
    }
}
