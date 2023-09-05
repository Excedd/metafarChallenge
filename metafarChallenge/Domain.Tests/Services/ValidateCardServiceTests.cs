using Domain.Repositories;
using Domain.Services;
using Entities;
using Moq;

namespace Domain.Tests.Services
{
    public class ValidateCardServiceTests
    {
        [Fact]
        public void BadPin_Should_AddTryAndUpdateCard()
        {
            //arrange
            var cardRepository = new Mock<ICardRepository>();
            var card = new Card("Mcano", 123, "123", "12345", 100, 1, false, DateTimeOffset.Now);
            cardRepository.Setup(service => service.UpdateCardAsync(card))
                .Verifiable();
            var sut = new ValidateCardService(cardRepository.Object);
            //act
            sut.Validate(card, "11");
            //assert
            Assert.Equal(card.FailedTries, 2);
            cardRepository.Verify();

        }

        [Fact]
        public async void BadPin_Should_BlockCardUpdateAndThrowEx()
        {
            //arrange
            var cardRepository = new Mock<ICardRepository>();
            var card = new Card("Mcano", 123, "123", "12345", 100, 4, false, DateTimeOffset.Now);
            cardRepository.Setup(service => service.UpdateCardAsync(card))
                .Verifiable();
            var sut = new ValidateCardService(cardRepository.Object);

            //act
            await Assert.ThrowsAsync<NotImplementedException>(async () => await sut.Validate(card, "11"));

            //assert
            Assert.Equal(card.Blocked, true);
            cardRepository.Verify();

        }
    }
}