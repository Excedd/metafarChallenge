using Domain.DTO;
using Domain.Exceptions;
using Domain.Extensions;
using Domain.Repositories;

namespace Domain.UseCases
{
    public interface IGetOperation
    {
        public Task<OperationsPaginated> DoAsync(GetOperationsPaginated getOperationsPaginated);
    }

    public class GetOperations : IGetOperation
    {
        public readonly IOperationRepository _operationRepository;
        public readonly ICardRepository _cardRepository;

        public GetOperations(IOperationRepository operationRepository,
            ICardRepository cardRepository)
        {
            _operationRepository = operationRepository;
            _cardRepository = cardRepository;
        }

        public async Task<OperationsPaginated> DoAsync(GetOperationsPaginated getOperationsPaginated)
        {
            getOperationsPaginated.CardNumber.IsValidCardNumber();
            if (getOperationsPaginated.Page == 0)
                throw new PageNotValid();
            var card = await _cardRepository.GetCardByNumberAsync(getOperationsPaginated.CardNumber) ?? throw new CardNotFoundException();
            var operations = await _operationRepository.GetOperationsByCardIdAsync(card.CardId,
                getOperationsPaginated.Page);
            var totalOperations = await _operationRepository.GetTotalOperationsByCardIdAsync(card.CardId);
            var totalPages = (int)Math.Ceiling((double)totalOperations / 10);
            return new OperationsPaginated(getOperationsPaginated.Page, totalPages, operations);
        }
    }
}