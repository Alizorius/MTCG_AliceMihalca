using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ClientTuple = System.Tuple<System.Net.Sockets.NetworkStream, string>;

namespace MTCG
{
    static class BattleRequests
    {
        static List<ClientTuple> playerPool = new List<ClientTuple>();
        static private readonly object lockObj = new object();

        public static void AddRequestToPool(NetworkStream stream, string request)
        {
            string username = Helper.ExtractUsernameToken(request);
            lock (lockObj)
            {
                ClientTuple tuple = new ClientTuple(stream, username);
                playerPool.Add(tuple);
            }
        }

        public static void startMatch(string request)
        {
            string username = Helper.ExtractUsernameToken(request);
            while (true)
            {
                lock (lockObj)
                {
                    if (playerPool.Count >= 2)
                    {
                        ClientTuple c1 = playerPool.Find(p => p.Item2.Equals(username));
                        ClientTuple c2 = playerPool.Find(p => !p.Item2.Equals(username));
                        Battle battle = new Battle();

                        battle.DeckBattle(c1, c2);
                        playerPool.Remove(c1);
                        playerPool.Remove(c2);
                        return;
                    }
                }
                Thread.Sleep(1000);
            }
        }

    }
}
