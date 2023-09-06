using Domain.Repositories;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repositories
{
    public class CardRepository : ICardRepository
    {
        public readonly AppDbContext _appdbContext;

        public CardRepository(AppDbContext AppDbContext)
        {
            _appdbContext = AppDbContext;
        }

        public async Task<Card> GetCardByNumberAsync(string cardNumber)
         => await _appdbContext.Card.Where(c => c.CardNumber == cardNumber).FirstOrDefaultAsync() ;

        public async Task UpdateCardAsync(Card card)
        {
            _appdbContext.Card.Update(card);
            await _appdbContext.SaveChangesAsync();
        }
    }
}