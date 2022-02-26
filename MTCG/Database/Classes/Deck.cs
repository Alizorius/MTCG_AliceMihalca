using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    class Deck
    {
        static Random Rnd = new Random();
        public List<Card> DeckList = new List<Card>();
        public string Username;

        public Deck(List<Card> deckList, User user) : this(deckList, user.Username) { }

        public Deck(List<Card> deckList, string username)
        {
            DeckList = deckList;
            Username = username;
        }

        public Deck(Card card1, Card card2, Card card3, Card card4)
        {
            DeckList.Add(card1);
            DeckList.Add(card2);
            DeckList.Add(card3);
            DeckList.Add(card4);
        }

        public int GetSize()
        {
            return DeckList.Count;
        }
        public Card GetRndCard()
        {
            return DeckList[Rnd.Next(DeckList.Count)];
        }
        public void AddCard(Card c)
        {
            DeckList.Add(c);
        }
        public void RemoveCard(Card c)
        {
            DeckList.Remove(c);
        }
    }

}
