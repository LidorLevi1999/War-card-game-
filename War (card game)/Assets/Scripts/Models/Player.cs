using System.Collections.Generic;
using System.Linq;
using Managers;

namespace Models
{
    public class Player
    {
        public Queue<Card> CurrentHand;
        public List<Card> CardsDeck;
        public Stack<Card> Field;
        public Player()
        {
            this.CardsDeck = new List<Card>();
            this.CurrentHand = new Queue<Card>();
            this.Field = new Stack<Card>();
        }

        public void AddCardToQueue(Card CardToAdd)
        {
            CurrentHand.Enqueue(CardToAdd);
        }

        public void AddCardsToStack(List<Card> uCards, List<Card> cCards)
        {
            foreach (var card in uCards)
                Field.Push(card);
            foreach (var card in cCards)
                Field.Push(card);
        }

        public void AddCardsToList(Stack<Card> CardsStack)
        {
            while (CardsStack.Count > 0)
            {
                CardsDeck.Add(CardsStack.Pop());
            }
        }

        public void Reset()
        {
            CurrentHand.Clear();
            CardsDeck.Clear();
            Field.Clear();
        }
    }
}