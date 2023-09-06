using Domain.Exceptions;
using Domain.Extensions;
using Domain.Repositories;
using Domain.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Domain.UseCases
{
    public interface ILogUser
    {
        public Task DoAsync(string numberCard, string pin);
    }

    public class LogUser : ILogUser
    {
        private readonly ICardRepository _cardRepository;
        private readonly IValidateCardService _validateCard;

        public LogUser(ICardRepository icardRepository, IValidateCardService validateCard)
        {
            _cardRepository = icardRepository;
            _validateCard = validateCard;
        }

        public async Task DoAsync(string numberCard, string pin)
        {
            numberCard.IsValidCardNumber();
            var card = await _cardRepository.GetCardByNumberAsync(numberCard) ?? throw new CardNotFoundException();
            await _validateCard.Validate(card, pin);
        }
    }
}