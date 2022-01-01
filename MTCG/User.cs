using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    class User
    {
        public string name { get; set; }
        public string password { get; set; }
        public int score { get; set; }
        public int playedGames { get; set; }
        public int coin { get; set; }
        public List<Card> stack { get; set; }
        public List<Card> deck { get; set; }
    }
}
