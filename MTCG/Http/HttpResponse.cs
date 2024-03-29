﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Http
{
    static class HttpResponse
    {
        public static void SendMessage(NetworkStream stream, string message)
        {
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(msg, 0, msg.Length);
        }

        public static string CardsResponseMsg(bool plainFormat, Deck deck)
        {
            return CardsResponseMsg(plainFormat, deck.DeckList);
        }

        public static string CardsResponseMsg(bool plainFormat, List<Card> cards)
        {
            string cardsStr = null;
            foreach (var card in cards)
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
            return cardsStr;
        }

        public static string UserResponseMsg(User user)
        {
            string userStr = null;

            userStr += "Username: " + user.Username + "\n\r";

            if (!(user.Displayname is null))
            {
                userStr += "Displayname: " + user.Displayname + "\n\r";
            }
            if (!(user.Bio is null))
            {
                userStr += "Bio: " + user.Bio + "\n\r";
            }
            if (!(user.Image is null))
            {
                userStr += "Image: " + user.Image + "\n\r";
            }

            userStr += "Coins: " + user.Coins.ToString();

            return userStr;
        }

        public static string StatsResponseMsg(Score score)
        {
            string statsStr = null;

            statsStr += "Username: " + score.Username + "\n\r";
            statsStr += "Elo: " + score.Elo.ToString() + "\n\r";
            statsStr += "Wins: " + score.Wins.ToString() + "\n\r";
            statsStr += "Losses: " + score.Losses.ToString() + "\n\r";
            statsStr += "Draws: " + score.Draws.ToString();

            return statsStr;
        }

        public static string ScoreboardResponseMsg(List<Score> scores)
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
            return scoreStr;
        }

        public static string DealsResponseMsg(List<Deal> deals)
        {
            string dealsStr = null;
            foreach (var deal in deals)
            {
                dealsStr += "Deal-ID: " + deal.Id + ", ";
                dealsStr += "Card to Trade: " + deal.CardToTrade + ", ";
                dealsStr += "Type-Requirement: " + deal.Type + ", ";
                dealsStr += "Minimum-Damage: " + deal.MinimumDamage + "\n\r";
            }
            return dealsStr;
        }
    }
}
