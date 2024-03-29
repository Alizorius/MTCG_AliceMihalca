﻿using MTCG.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClientTuple = System.Tuple<System.Net.Sockets.NetworkStream, string>;

namespace MTCG
{
    public class Battle
    {
        public string battleLog = "";

        public void DeckBattle(ClientTuple client1, ClientTuple client2)
        {
            Deck deck1 = DBCard.GetDeckFromUsername(client1.Item2);
            Deck deck2 = DBCard.GetDeckFromUsername(client2.Item2);

            Score scoreUser1 = DBScore.GetStatsFromUsername(deck1.Username);
            Score scoreUser2 = DBScore.GetStatsFromUsername(deck2.Username);

            for (int i = 0; i < 100; i++)
            {
                Card card1 = deck1.GetRndCard();
                Card card2 = deck2.GetRndCard();

                battleLog += card1.Name + " from " + deck1.Username + " is fighting " + card2.Name + " from " + deck2.Username + ".\n\r\n\r";

                switch (CardBattle(card1, card2))
                {
                    case 2:
                        battleLog += deck2.Username + " won this Round. (You gain " + card1.Name + " from your oponents deck)\n\r\n\r";
                        if (deck1.GetSize().Equals(1))
                        {
                            battleLog += deck2.Username + " wins this Game!\n\r\n\r";
                            if(i < 20)
                            {
                                battleLog += deck2.Username + " You managed to win this game under 20 rounds, you get 1 coin as a reward!\n\r\n\r";
                                DBUser.GetUserByUsername(deck2.Username).Coins += 1;
                            }
                            
                            scoreUser1.Elo -= 5;
                            scoreUser1.Losses += 1;

                            scoreUser2.Elo += 3;
                            scoreUser2.Wins += 1;

                            DBScore.SetStats(scoreUser1);
                            DBScore.SetStats(scoreUser2);

                            HttpResponse.SendMessage(client1.Item1, battleLog);
                            HttpResponse.SendMessage(client2.Item1, battleLog);

                            return;
                        }
                        deck2.AddCard(card1);
                        deck1.RemoveCard(card1);
                        break;

                    case 1:
                        battleLog += deck1.Username + " won this Round. (You gain " + card2.Name + " from your oponents deck)\n\r\n\r";
                        if (deck2.GetSize().Equals(1))
                        {
                            battleLog += deck1.Username + " wins this Game!\n\r\n\r";
                            if (i < 20)
                            {
                                battleLog += deck1.Username + " You managed to win this game under 20 rounds, you get 1 coin as a reward!\n\r\n\r";
                                DBUser.GetUserByUsername(deck1.Username).Coins += 1;
                            }

                            scoreUser1.Elo += 3;
                            scoreUser1.Wins += 1;

                            scoreUser2.Elo -= 5;
                            scoreUser2.Losses += 1;

                            DBScore.SetStats(scoreUser1);
                            DBScore.SetStats(scoreUser2);

                            HttpResponse.SendMessage(client1.Item1, battleLog);
                            HttpResponse.SendMessage(client2.Item1, battleLog);

                            return;
                        }
                        deck1.AddCard(card2);
                        deck2.RemoveCard(card2);
                        break;

                    case 0:
                        battleLog += "Its a draw this Round.\n\r\n\r";
                        break;
                    default:
                        break;
                }
            }

            battleLog += "The Game ends in a draw.\n\r";

            scoreUser1.Draws += 1;
            scoreUser2.Draws += 1;

            DBScore.SetStats(scoreUser1);
            DBScore.SetStats(scoreUser2);

            HttpResponse.SendMessage(client1.Item1, battleLog);
            HttpResponse.SendMessage(client2.Item1, battleLog);

        }

        public int CardBattle(Card card1, Card card2) //returns 0 if its a draw, 1 if the first card won and 2 if the second card won
        {
            if (card1.EffectiveDmg(card2) < card2.EffectiveDmg(card1)) return 2;

            else if (card1.EffectiveDmg(card2) > card2.EffectiveDmg(card1)) return 1;
            
            else return 0;
        }
    }
}
