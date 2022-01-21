using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    static class DBUser
    {
       
        public static bool AddUser(string request)
        {
            User user = Helper.ExtractUser(request);
            DB.CreateDBIfNotPresent();

            using var conn = DB.Connection(DB.connString);

            //error handling
            using var userTabelCmd = new NpgsqlCommand(
                    "INSERT INTO userTabel (username, password, role) " +
                    "VALUES(@p1, @p2, @p3)",
                    conn);

            userTabelCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Varchar, user.username);
            userTabelCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Varchar, user.password);
            userTabelCmd.Parameters.AddWithValue("p3", NpgsqlDbType.Varchar, user.role.ToString());

            userTabelCmd.ExecuteNonQuery();


            return true;
        }
    }
}
