using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    static class HttpResponse
    {
        public static void SendMessage(NetworkStream stream, string message)
        {
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(msg, 0, msg.Length);
        }

        public static void SendCards(bool plainFormat, NetworkStream stream, Deck deck)
        {
            SendCards(plainFormat, stream, deck.DeckList);
        }

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
            SendMessage(stream, cardsStr);
        }

        public static void SendStats(NetworkStream stream, Score score)
        {
            string statsStr = null;

            statsStr += "Username: " + score.Username + "\n\r";
            statsStr += "Elo: " + score.Elo.ToString() + "\n\r";
            statsStr += "Wins: " + score.Wins.ToString() + "\n\r";
            statsStr += "Losses: " + score.Losses.ToString() + "\n\r";
            statsStr += "Draws: " + score.Draws.ToString();

            SendMessage(stream, statsStr);
        }

        public static void SendScoreboard(NetworkStream stream, List<Score> scores)
        {
            string scoreStr = null;
            foreach (var score in scores)
            {
                scoreStr += "Elo: " + score.Elo.ToString() + ", ";
                scoreStr += "Username: " + score.Username;
                //scoreStr += "Wins: " + score.Wins.ToString() + ", ";
                //scoreStr += "Losses: " + score.Losses.ToString() + ", ";
                //scoreStr += "Draws: " + score.Draws.ToString();
                scoreStr += "\n\r";
            }
            SendMessage(stream, scoreStr);
        }
    }
}
