using Domain.Exceptions;
using Domain.Repositories;
using Entities;

namespace Domain.Services
{
    public interface IValidateCardService
    {
        public Task Validate(Card card, string pin);
    }

    public class ValidateCardService : IValidateCardService
    {
        private readonly ICardRepository _cardRepository;

        public ValidateCardService(ICardRepository icardRepository)
        {
            _cardRepository = icardRepository;
        }

        public async Task Validate(Card card, string pin)
        {
            if (card.Pin != pin)
            {
                if (card.FailedTries >= 4)
                {
                    if (card.Blocked)
                        throw new CardBlocked();

                    card.Block();
                    await _cardRepository.UpdateCardAsync(card);
                    throw new BadPin();
                }
                card.AddTry();
                await _cardRepository.UpdateCardAsync(card);
            }
        }
    }
}