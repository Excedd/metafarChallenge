using Domain.Repositories;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repositories
{
    public class OperationRepository : IOperationRepository
    {
        public readonly AppDbContext _appdbContext;

        public OperationRepository(AppDbContext AppDbContext)
        {
            _appdbContext = AppDbContext;
        }

        public async Task AddOperationsAsync(Operation Operation)
        {
            await _appdbContext.Operation.AddAsync(Operation);
        }

        public async Task<List<Operation>> GetOperationsByCardIdAsync(int CardId, int page, int pageSize)
            => await _appdbContext.Operation
            .Where(o => o.CardId == CardId)
            .OrderByDescending(o => o.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        public async Task<int> GetTotalOperationsByCardIdAsync(int CardId)
            => await _appdbContext.Operation
            .Where(o => o.CardId == CardId)
            .CountAsync();
    }
}