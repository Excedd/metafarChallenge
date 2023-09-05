using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Card
    {
        public string UserName { get; private set; }
        public int CardId { get; private set; }
        public string CardNumber { get; private set; }
        public string Pin { get; private set; }
        public decimal Balance { get; private set; }
        public int FailedTries { get; private set; }
        public bool Blocked { get; private set; }
        public DateTimeOffset LastExtraction { get; private set; }

        public void AddTry()
            => FailedTries++;

        public void Block()
            => Blocked = true;

        public decimal SubtractBalance(int amount)
        {
            return Balance -= amount;
        }

        private Card()
        { }

        public Card(string userName, int cardId, string cardNumber, string pin, decimal balance, int failedTries, bool blocked, DateTimeOffset lastExtraction)
        {
            UserName = userName;
            CardId = cardId;
            CardNumber = cardNumber;
            Pin = pin;
            Balance = balance;
            FailedTries = failedTries;
            Blocked = blocked;
            LastExtraction = lastExtraction;
        }
    }
}