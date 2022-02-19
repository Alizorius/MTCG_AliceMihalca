using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    class HttpResponse
    {
        public static void SendCards(bool plainFormat, NetworkStream stream, List<Card> cards)
        {
            string cardsStr = null;
            foreach(var card in cards)
            {
                if (plainFormat)
                {
                    cardsStr += card.PlainFormat();
                }
                else
                {
                    cardsStr += card.CardToString();
                }
            }
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(cardsStr);
            stream.Write(msg, 0, msg.Length);
        }
    }
}
