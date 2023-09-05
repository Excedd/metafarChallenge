using Entities;

namespace Domain.DTO
{
    public record GetOperationsPaginated(string CardNumber, int Page = 0, int PageSize = 10);

    public record OperationsPaginated(int Page, int TotalPages, IEnumerable<Operation> Operations);
}