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
            if (user.Username.Equals("admin"))
            {
                user.Role = Role.Admin;
            }

            string pattern = "[a-zA-Z0-9]{5,11}";
            Regex defaultRegex = new Regex(pattern);

            if (defaultRegex.IsMatch(user.Username))
            {
                if (GetUserByUsername(user.Username) == null)
                {
                    using var conn = DB.Connection();

                    using var userTableCmd = new NpgsqlCommand(
                            "INSERT INTO userTable (username, password, role, displayname, bio, image, coins, isLoggedIn) " +
                            "VALUES(@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8)",
                            conn);

                    userTableCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Varchar, user.Username);
                    userTableCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Varchar, user.Password);
                    userTableCmd.Parameters.AddWithValue("p3", NpgsqlDbType.Varchar, user.Role.ToString());
                    userTableCmd.Parameters.AddWithValue("p4", NpgsqlDbType.Varchar, DBNull.Value);
                    userTableCmd.Parameters.AddWithValue("p5", NpgsqlDbType.Varchar, DBNull.Value);
                    userTableCmd.Parameters.AddWithValue("p6", NpgsqlDbType.Varchar, DBNull.Value);
                    userTableCmd.Parameters.AddWithValue("p7", NpgsqlDbType.Integer, user.Coins);
                    userTableCmd.Parameters.AddWithValue("p8", NpgsqlDbType.Boolean, false);

                    userTableCmd.ExecuteNonQuery();

                    return true;
                }
            }
            return false;
        }

        public static bool LoginUser(string username)
        {
            User user = GetUserByUsername(username);
            if (user != null)
            {
                using var conn = DB.Connection();

                using var userUpdateCmd = new NpgsqlCommand(
                "UPDATE userTable " +
                "SET isLoggedIn = true WHERE username = @p1 AND password = @p2",
                conn);
                userUpdateCmd.Parameters.AddWithValue("p1", username);
                userUpdateCmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;
                userUpdateCmd.Parameters.AddWithValue("p2", user.Password);
                userUpdateCmd.Parameters[1].NpgsqlDbType = NpgsqlDbType.Varchar;
                userUpdateCmd.ExecuteNonQuery();
                return true;
            }
            return false;
        }

        public static bool LoggedInCheck(string username)
        {
            using var conn = DB.Connection();

            using var cmd = new NpgsqlCommand(
                    "SELECT isLoggedIn FROM userTable WHERE username = @p1",
                    conn);
            cmd.Parameters.AddWithValue("p1", username);
            cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;

            NpgsqlDataReader? reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                bool checkLogin = reader.GetBoolean(0);
                reader.Close();
                return checkLogin;
            }
            reader.Close();
            return false;
        }

        public static string GetUsernamebyCardId(string cardId)
        {
            using var conn = DB.Connection();

            using var cmd = new NpgsqlCommand(
                    "SELECT username FROM cardTable WHERE id = @p1",
                    conn);
            cmd.Parameters.AddWithValue("p1", cardId);
            cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;

            NpgsqlDataReader? reader = cmd.ExecuteReader();

            if(reader.Read())
            {
                string username = reader.GetString(0);
                reader.Close();
                return username;
            }
            reader.Close();
            return null;
        }

        public static User GetUserByUsername(string username)
        {
            using var conn = DB.Connection();

            using var cmd = new NpgsqlCommand(
                    "SELECT * FROM userTable WHERE username = @p1",
                    conn);
            cmd.Parameters.AddWithValue("p1", username);
            cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;

            NpgsqlDataReader? reader = null;
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                User user = new User(reader.GetString(0), reader.GetString(1), reader.GetString(2));

                if (!reader.IsDBNull(3))
                {
                    user.Displayname = reader.GetString(3);
                }
                if (!reader.IsDBNull(4))
                {
                    user.Bio = reader.GetString(4);
                }
                if (!reader.IsDBNull(5))
                {
                    user.Image = reader.GetString(5);
                }
                if (!reader.IsDBNull(6))
                {
                    user.Coins = reader.GetInt32(6);
                }
                reader.Close();

                return user;
            }
            reader.Close();

            return null;
        }

        public static bool UpdateUserData(string request)
        {
            string username = Helper.ExtractUsernameToken(request);
            Dictionary<string, string> data = Helper.ExtractUserData(request);

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
