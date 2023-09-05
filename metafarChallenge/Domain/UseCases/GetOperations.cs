﻿using Domain.DTO;
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
            var card = await _cardRepository.GetCardByNumberAsync(getOperationsPaginated.CardNumber);

            var operations = await _operationRepository.GetOperationsByCardIdAsync(card.CardId,
                getOperationsPaginated.Page,
                getOperationsPaginated.PageSize);

            var totalOperations = await _operationRepository.GetTotalOperationsByCardIdAsync(card.CardId);
            var totalPages = (int)Math.Ceiling((double)totalOperations / getOperationsPaginated.PageSize);
            return new OperationsPaginated(getOperationsPaginated.Page, totalPages, operations);
        }
    }
}