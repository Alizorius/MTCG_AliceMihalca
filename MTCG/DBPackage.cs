using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
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

            packageID = (int)cmd.Parameters[1].Value + 1;

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
                cardCmd.Parameters[2].NpgsqlDbType = NpgsqlDbType.Varchar;

                cardCmd.Parameters.AddWithValue("p4", card.ElementType);
                cardCmd.Parameters[3].NpgsqlDbType = NpgsqlDbType.Varchar;

                cardCmd.Parameters.AddWithValue("p6", card.PackageId);
                cardCmd.Parameters[5].NpgsqlDbType = NpgsqlDbType.Integer;

                cardCmd.Parameters.AddWithValue("p7", card.Username);
                cardCmd.Parameters[6].NpgsqlDbType = NpgsqlDbType.Integer;

                cardCmd.Parameters.AddWithValue("p8", false);
                cardCmd.Parameters[7].NpgsqlDbType = NpgsqlDbType.Boolean;

                if(card is Monster)
                {
                    Monster m = card as Monster;
                    cardCmd.Parameters.AddWithValue("p5", m.MonsterType);
                    cardCmd.Parameters[4].NpgsqlDbType = NpgsqlDbType.Varchar;
                }

                cardCmd.ExecuteNonQuery();
            }


            return true;
        }
    }
}
