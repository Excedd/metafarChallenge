using Domain.DTO;
using Domain.Exceptions;
using Domain.Repositories;
using Entities;

namespace Domain.UseCases
{
    public interface IWithdrawal
    {
        public Task<WithdrawalReturn> DoAsync(string cardNumber, int amount);
    }

    public class Withdrawal : IWithdrawal
    {
        private readonly ICardRepository _cardRepository;
        private readonly IOperationRepository _operationRepository;

        public Withdrawal(ICardRepository cardRepository,
            IOperationRepository operationRepository)
        {
            _cardRepository = cardRepository;
            _operationRepository = operationRepository;
        }

        public async Task<WithdrawalReturn> DoAsync(string cardNumber, int amount)
        {
            var card = await _cardRepository.GetCardByNumberAsync(cardNumber);
            if (amount > card.Balance)
                throw new BadAmount();
            var lastBalance = card.Balance;
            var currentBalance = card.SubtractBalance(amount);
            await _operationRepository.AddOperationsAsync(new WithdrawalOperation(amount, lastBalance, currentBalance, card.CardId));
            await _cardRepository.UpdateCardAsync(card);
            return new WithdrawalReturn(lastBalance, currentBalance);
        }
    }
}