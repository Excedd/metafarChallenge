using Entities;

namespace Domain.Repositories
{
    public interface IOperationRepository
    {
        Task AddOperationsAsync(Operation Operation);

        Task<List<Operation>> GetOperationsByCardIdAsync(int CardId, int page, int pageSize);

        Task<int> GetTotalOperationsByCardIdAsync(int CardId);
    }
}