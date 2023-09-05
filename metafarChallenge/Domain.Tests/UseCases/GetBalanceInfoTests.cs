using Domain.Repositories;
using Domain.UseCases;
using Entities;
using Moq;

namespace Domain.Tests.UseCases
{
    public class GetBalanceInfoTests
    {
        [Fact]
        public async Task Get_Should_Return_BalanceInfo()
        {
            // Arrange
            var cardRepository = new Mock<ICardRepository>();
            cardRepository.Setup(repo => repo.GetCardByNumberAsync(It.IsAny<string>()))
                .ReturnsAsync(new Card("mcano", 123, "123456", "123", 100, 0, false, DateTimeOffset.Now));

            var getBalanceInfo = new GetBalanceInfo(cardRepository.Object);

            // Act
            var result = await getBalanceInfo.Get("123456");

            // Assert
            Assert.Equal("mcano", result.UserName);
            Assert.Equal(123, result.CardId);
            Assert.Equal(100.0m, result.Balance);
        }
    }
}