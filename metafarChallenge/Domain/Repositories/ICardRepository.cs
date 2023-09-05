using Entities;

namespace Domain.Repositories
{
    public interface ICardRepository
    {
        Task<Card> GetCardByNumberAsync(string cardNumber);

        Task UpdateCardAsync(Card card);
    }
}