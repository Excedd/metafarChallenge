using Domain.Repositories;
using Domain.Services;
using Domain.UseCases;
using Entities;
using Moq;
using Xunit;

namespace Domain.Tests.UseCases
{
    public class LogUserTests
    {
        [Fact]
        public async void DoAsync_Valid_ShouldReturnJwtToken()
        {
            // Arrange
            var cardRepository = new Mock<ICardRepository>();
            var validateCardService = new Mock<IValidateCardService>();
            var sut = new LogUser(cardRepository.Object, validateCardService.Object);

            var cardId = 1;
            var pin = "12345";

            cardRepository.Setup(repo => repo.GetCardByNumberAsync(It.IsAny<string>()))
                .ReturnsAsync(new Card("Mcano", cardId, "123", "12345", 100, 0, false, DateTimeOffset.Now));

            validateCardService.Setup(service => service.Validate(It.IsAny<Card>(), It.IsAny<string>()))
                .Verifiable();

            // Act
            await sut.DoAsync("1234567890", pin);

            // Assert
            validateCardService.Verify();
            cardRepository.Verify();
        }
    }
}