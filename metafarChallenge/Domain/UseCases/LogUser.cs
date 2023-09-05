using Domain.Repositories;
using Domain.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Domain.UseCases
{
    public interface ILogUser
    {
        public Task<string> DoAsync(string numberCard, string pin);
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

        public async Task<string> DoAsync(string numberCard, string pin)
        {
            var card = await _cardRepository.GetCardByNumberAsync(numberCard) ?? throw new NotImplementedException(nameof(numberCard));
            await _validateCard.Validate(card, pin);
            return string.Empty;
            //return GenerateJwtToken(card.CardId);
        }

        private static string GenerateJwtToken(int cardId)
        {
            var token = new JwtSecurityToken(
                claims: new[] { new Claim("cardId", cardId.ToString()) },
                expires: DateTime.UtcNow.AddHours(1));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}