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
        public static bool GetAllUserCards(string username)
        {
            using var conn = DB.Connection();
            NpgsqlDataReader? dr = null;

            using var cardQueryCmd = new NpgsqlCommand(
                "SELECT * from cardTable WHERE username = @p1",
                conn);
            cardQueryCmd.Parameters.AddWithValue("p1", username);
            cardQueryCmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;

            dr = cardQueryCmd.ExecuteReader();
            var cards = new List<Card>();
            while (dr.Read())
            {
                ElementType elementType = (ElementType)Enum.Parse(typeof(ElementType), dr.GetString(3));
                
                if (!String.IsNullOrEmpty(dr.GetString(4)))
                {
                    MonsterType monsterType = (MonsterType)Enum.Parse(typeof(MonsterType), dr.GetString(4));
                    
                    cards.Add(new Monster(
                        dr.GetString(0), dr.GetString(1), dr.GetDouble(2),
                        elementType, monsterType, dr.GetInt32(5), username, false
                    ));
                }
               
                cards.Add(new Spell(
                    dr.GetString(0), dr.GetString(1), dr.GetDouble(2),
                    elementType, dr.GetInt32(5), username, false
                ));
            }
            dr.Close();

            return true;
        }
    }
}
