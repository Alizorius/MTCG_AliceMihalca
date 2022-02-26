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
    class DBScore
    {
        public static void SetDefaultStats(string request)
        {
            string username = Helper.ExtractUser(request).Username;

            using var conn = DB.Connection();

            using var cmd = new NpgsqlCommand(
                    "INSERT INTO scoreTable (username, elo, wins, losses, draws) " +
                    "VALUES(@p1, @p2, @p3, @p4, @p5)",
                     conn);

            cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Varchar, username);
            cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Integer, 100);
            cmd.Parameters.AddWithValue("p3", NpgsqlDbType.Integer, 0);
            cmd.Parameters.AddWithValue("p4", NpgsqlDbType.Integer, 0);
            cmd.Parameters.AddWithValue("p5", NpgsqlDbType.Integer, 0);

            cmd.ExecuteNonQuery();
        }


        public static Score GetStats(string request)
        {
            string username = Helper.ExtractUsernameToken(request);
            using var conn = DB.Connection();

            using var cmd = new NpgsqlCommand(
                    "SELECT * FROM scoreTable WHERE username = @p1",
                    conn);

            cmd.Parameters.AddWithValue("p1", username);
            cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;

            NpgsqlDataReader? reader = null;
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                Score score = new Score(reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4));

                reader.Close();

                return score;
            }
            reader.Close();

            return null;
        }

        public static void SetStats(Score score)
        {
            using var conn = DB.Connection();

            using var scoreUpdateCmd = new NpgsqlCommand(
                "UPDATE scoreTable " +
                "SET elo = @p1, wins = @p2, losses = @p3, draws = @p4 WHERE username = @p5",
                conn);

            scoreUpdateCmd.Parameters.AddWithValue("p1", score.Elo);
            scoreUpdateCmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Integer;
            scoreUpdateCmd.Parameters.AddWithValue("p2", score.Wins);
            scoreUpdateCmd.Parameters[1].NpgsqlDbType = NpgsqlDbType.Integer;
            scoreUpdateCmd.Parameters.AddWithValue("p3", score.Losses);
            scoreUpdateCmd.Parameters[2].NpgsqlDbType = NpgsqlDbType.Integer;
            scoreUpdateCmd.Parameters.AddWithValue("p4", score.Draws);
            scoreUpdateCmd.Parameters[3].NpgsqlDbType = NpgsqlDbType.Integer;
            scoreUpdateCmd.Parameters.AddWithValue("p5", score.Username);
            scoreUpdateCmd.Parameters[4].NpgsqlDbType = NpgsqlDbType.Varchar;

            scoreUpdateCmd.ExecuteNonQuery();
        }

        public static List<Score> GetScoreBoard()
        {
            using var conn = DB.Connection();
            NpgsqlDataReader? reader = null;

            using var cmd = new NpgsqlCommand(
                    "SELECT * FROM scoreTable ORDER BY elo DESC LIMIT 10",
                    conn);

            reader = cmd.ExecuteReader();
            var scores = new List<Score>();
            while (reader.Read())
            {
                var username = reader.GetString(0);
                var elo = reader.GetInt32(1);
                var wins = reader.GetInt32(2);
                var looses = reader.GetInt32(3);
                var draws = reader.GetInt32(4);
                scores.Add(new Score(username, elo, wins, looses, draws));
            }
            reader.Close();

            return scores;
        }
    }
}
