using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using ClientTuple = System.Tuple<System.Net.Sockets.NetworkStream, string>;

namespace MTCG
{
    static class BattleRequests
    {
        static List<ClientTuple> playerPool;
        static private readonly object lockObj = new object();

        public static void AddRequestToPool(NetworkStream stream, string username)
        {
            lock (lockObj)
            {
                ClientTuple tuple = new ClientTuple(stream, username);

                playerPool.Add(tuple);
                if(playerPool.Count > 2)
                {
                    Battle battle = new Battle();
                    battle.DeckBattle(playerPool[0], playerPool[1]);
                    playerPool.RemoveRange(0, 2);

                }
            }
        }

    }
}
