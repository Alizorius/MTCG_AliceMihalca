using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MTCG
{
    static class DBUser
    {
       
        public static bool AddUser(string request)
        {
            User user = Helper.ExtractUser(request);
  
            string pattern = "[a-zA-Z0-9]{5,11}";
            Regex defaultRegex = new Regex(pattern);

            if (defaultRegex.IsMatch(user.Username))
            {
                if (GetUser(user.Username) == null)
                {
                    using var conn = DB.Connection();

                    //error handling
                    using var userTableCmd = new NpgsqlCommand(
                            "INSERT INTO userTable (username, password, role) " +
                            "VALUES(@p1, @p2, @p3)",
                            conn);

                    userTableCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Varchar, user.Username);
                    userTableCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Varchar, user.Password);
                    userTableCmd.Parameters.AddWithValue("p3", NpgsqlDbType.Varchar, user.Role.ToString());

                    userTableCmd.ExecuteNonQuery();


                    return true;
                }
            }
            return false;
        }

        public static User GetUser(string username)
        {
            using var conn = DB.Connection();

            using var cmd = new NpgsqlCommand(
                    "SELECT * FROM userTable WHERE username=@p1",
                    conn);

            cmd.Parameters.AddWithValue("p1", username);
            cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;

            cmd.Parameters.Add(new NpgsqlParameter("username", NpgsqlDbType.Varchar)
            { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new NpgsqlParameter("password", NpgsqlDbType.Varchar)
            { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new NpgsqlParameter("role", NpgsqlDbType.Varchar)
            { Direction = ParameterDirection.Output });

            cmd.ExecuteNonQuery();

            if (cmd.Parameters[1].Value != null &&
                    cmd.Parameters[2].Value != null &&
                    cmd.Parameters[3].Value != null)
            {
                return new User(
                    (string)cmd.Parameters[1].Value!,
                    (string)cmd.Parameters[2].Value!,
                    (string)cmd.Parameters[3].Value!);
            }

            return null;
        }
    }
}
