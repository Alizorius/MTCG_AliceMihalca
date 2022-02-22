using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    class Score
    {
        public string Username { get; set; }
        public int Elo { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }

        public Score(string username, int elo = 0, int wins = 0, int losses = 0, int draws = 0)
        {
            Username = username;
            Elo = elo;
            Wins = wins;
            Losses = losses;
            Draws = draws;
        }

    }
}
