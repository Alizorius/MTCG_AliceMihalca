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
                            "INSERT INTO userTable (username, password, role, displayname, bio, image, coins) " +
                            "VALUES(@p1, @p2, @p3, @p4, @p5, @p6, @p7)",
                            conn);

                    userTableCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Varchar, user.Username);
                    userTableCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Varchar, user.Password);
                    userTableCmd.Parameters.AddWithValue("p3", NpgsqlDbType.Varchar, user.Role.ToString());
                    userTableCmd.Parameters.AddWithValue("p4", NpgsqlDbType.Varchar, DBNull.Value);
                    userTableCmd.Parameters.AddWithValue("p5", NpgsqlDbType.Varchar, DBNull.Value);
                    userTableCmd.Parameters.AddWithValue("p6", NpgsqlDbType.Varchar, DBNull.Value);
                    userTableCmd.Parameters.AddWithValue("p7", NpgsqlDbType.Integer, user.Coins);

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
                    "SELECT * FROM userTable WHERE username = @p1",
                    conn);

            cmd.Parameters.AddWithValue("p1", username);
            cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;

            cmd.Parameters.Add(new NpgsqlParameter("username", NpgsqlDbType.Varchar)
            { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new NpgsqlParameter("password", NpgsqlDbType.Varchar)
            { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new NpgsqlParameter("role", NpgsqlDbType.Varchar)
            { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new NpgsqlParameter("displayname", NpgsqlDbType.Varchar)
            { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new NpgsqlParameter("bio", NpgsqlDbType.Varchar)
            { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new NpgsqlParameter("image", NpgsqlDbType.Varchar)
            { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new NpgsqlParameter("coins", NpgsqlDbType.Integer)
            { Direction = ParameterDirection.Output });


            cmd.ExecuteNonQuery();

            if (cmd.Parameters[4].Value is System.DBNull || cmd.Parameters[5].Value is System.DBNull || cmd.Parameters[6].Value is System.DBNull)
            {
                return new User(
                    (string)cmd.Parameters[1].Value!,
                    (string)cmd.Parameters[2].Value!,
                    (string)cmd.Parameters[3].Value!);
            }
            else if (cmd.Parameters[1].Value != null &&
                    cmd.Parameters[2].Value != null &&
                    cmd.Parameters[3].Value != null &&
                    cmd.Parameters[4].Value != null &&
                    cmd.Parameters[5].Value != null &&
                    cmd.Parameters[6].Value != null &&
                    cmd.Parameters[7].Value != null)

            {
                return new User(
                    (string)cmd.Parameters[1].Value!,
                    (string)cmd.Parameters[2].Value!,
                    (string)cmd.Parameters[3].Value!,
                    (string)cmd.Parameters[4].Value!,
                    (string)cmd.Parameters[5].Value!,
                    (int)cmd.Parameters[6].Value!,
                    (string)cmd.Parameters[7].Value!);
            }
           
            return null;
        }

        public static bool UpdateUserData(string username, Dictionary<string, string> data)
        {
            using var conn = DB.Connection();

            string displayname = data["Name"];
            string bio = data["Bio"];
            string image = data["Image"];

            using var userUpdateCmd = new NpgsqlCommand(
                "UPDATE userTable " +
                "SET displayname = @p1, bio = @p2, image = @p3 WHERE username = @p4",
                conn);
            userUpdateCmd.Parameters.AddWithValue("p1", displayname);
            userUpdateCmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;
            userUpdateCmd.Parameters.AddWithValue("p2", bio);
            userUpdateCmd.Parameters[1].NpgsqlDbType = NpgsqlDbType.Varchar;
            userUpdateCmd.Parameters.AddWithValue("p3", image);
            userUpdateCmd.Parameters[1].NpgsqlDbType = NpgsqlDbType.Varchar;
            userUpdateCmd.Parameters.AddWithValue("p4", username);
            userUpdateCmd.Parameters[1].NpgsqlDbType = NpgsqlDbType.Varchar;

            userUpdateCmd.ExecuteNonQuery();

            return true;
        }
    }
}
