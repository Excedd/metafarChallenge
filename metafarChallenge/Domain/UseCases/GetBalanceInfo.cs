using Domain.DTO;
using Domain.Exceptions;
using Domain.Repositories;

namespace Domain.UseCases
{
    public interface IGetBalanceInfo
    {
        public Task<BalanceInfo> DoAsync(string cardId);
    }

    public class GetBalanceInfo : IGetBalanceInfo
    {
        private readonly ICardRepository _cardRepository;

        public GetBalanceInfo(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public async Task<BalanceInfo> DoAsync(string cardNumber)
        {
            var card = await _cardRepository.GetCardByNumberAsync(cardNumber) ?? throw new CardNotFoundException();
            return new BalanceInfo(card.UserName, card.CardId, card.Balance, card.LastExtraction.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}