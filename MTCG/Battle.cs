using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    class Battle
    {
        public void deckBattle(Deck deck1, Deck deck2)
        {
            for(int i = 0; i < 100; i++)
            {
                Card card1 = deck1.getRndCard();
                Card card2 = deck2.getRndCard();

                switch (cardBattle(card1, card2))
                {
                    //incomplete 
                    case 2:
                        Console.WriteLine("P2 won this round");
                        if (deck1.getSize().Equals(1))
                        {
                            Console.WriteLine("Player 2 wins");
                        }
                        deck2.addCard(card1);
                        deck1.removeCard(card1);
                        break;

                    case 1:
                        Console.WriteLine("P1 won this round");
                        if (deck2.getSize().Equals(1))
                        {
                            Console.WriteLine("Player 1 wins");
                        }
                        deck1.addCard(card2);
                        deck2.removeCard(card2);
                        break;

                    case 0:
                        Console.WriteLine("Its a draw this round");
                        break;
                    default:
                        break;
                }
            }
            Console.WriteLine("Its a draw");
        }

        public int cardBattle(Card card1, Card card2) //returns 0 if its a draw, 1 if the first card won and 2 if the second card won
        {
            if (card1.effectiveDmg(card2) < card2.effectiveDmg(card1)) return 2;

            else if (card1.effectiveDmg(card2) > card2.effectiveDmg(card1)) return 1;
            
            else return 0;
        }
    }
}
