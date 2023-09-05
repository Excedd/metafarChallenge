using Entities.Enums;

namespace Entities
{
    public class Operation
    {
        public int OperationId { get; private set; }
        public OperationType Type { get; private set; }
        public decimal LastBalace { get; private set; }
        public decimal CurrentBalance { get; private set; }
        public DateTimeOffset Date { get; private set; }
        public int CardId { get; private set; }

        private Operation()
        {
        }

        public Operation(OperationType type, decimal lastBalance,
            decimal currentBalance, int cardI)
        {
            Type = type;
            LastBalace = lastBalance;
            CurrentBalance = currentBalance;
            CardId = cardI;
            Date = DateTimeOffset.UtcNow;
        }
    }

    public class WithdrawalOperation : Operation
    {
        public decimal WithdrawalAmount { get; private set; }

        public WithdrawalOperation(decimal withdrawalAmount, decimal lastBalance,
            decimal currentBalance, int cardId)
            : base(OperationType.WithDrawal, lastBalance, currentBalance, cardId)
        {
            WithdrawalAmount = withdrawalAmount;
        }
    }
}