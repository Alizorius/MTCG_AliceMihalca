using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    class Deck
    {
        static Random rnd = new Random();
        public List<Card> deckList = new List<Card>();
        public User user;

        public Deck(List<Card> deckList, User user)
        {
            this.deckList = deckList;
            this.user = user;
        }
        public Deck(Card card1, Card card2, Card card3, Card card4)
        {
            deckList.Add(card1);
            deckList.Add(card2);
            deckList.Add(card3);
            deckList.Add(card4);
        }

        public int getSize()
        {
            return deckList.Count;
        }
        public Card getRndCard()
        {
            return deckList[rnd.Next(deckList.Count)];
        }
        public void addCard(Card c)
        {
            deckList.Add(c);
        }
        public void removeCard(Card c)
        {
            deckList.Remove(c);
        }
    }

}
