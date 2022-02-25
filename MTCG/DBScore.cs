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
        public static Score GetStats(string username)
        {
            using var conn = DB.Connection();

            using var cmd = new NpgsqlCommand(
                    "SELECT * FROM scoreTable WHERE username = @p1",
                    conn);

            cmd.Parameters.AddWithValue("p1", username);
            cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;

            cmd.Parameters.Add(new NpgsqlParameter("username", NpgsqlDbType.Varchar)
            { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new NpgsqlParameter("elo", NpgsqlDbType.Integer)
            { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new NpgsqlParameter("wins", NpgsqlDbType.Integer)
            { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new NpgsqlParameter("losses", NpgsqlDbType.Integer)
            { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new NpgsqlParameter("draws", NpgsqlDbType.Integer)
            { Direction = ParameterDirection.Output });

            cmd.ExecuteNonQuery();

            if (cmd.Parameters[1].Value != null &&
                    cmd.Parameters[2].Value != null &&
                    cmd.Parameters[3].Value != null &&
                    cmd.Parameters[4].Value != null &&
                    cmd.Parameters[5].Value != null)

            {
                return new Score(
                    (string)cmd.Parameters[1].Value!,
                    (int)cmd.Parameters[2].Value!,
                    (int)cmd.Parameters[3].Value!,
                    (int)cmd.Parameters[4].Value!,
                    (int)cmd.Parameters[5].Value!);
            }

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
    }
}
