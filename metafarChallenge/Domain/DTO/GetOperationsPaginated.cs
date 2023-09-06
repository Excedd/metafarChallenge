using Entities;

namespace Domain.DTO
{
    public record GetOperationsPaginated(string CardNumber, int Page = 1);

    public record OperationsPaginated(int Page, int TotalPages, IEnumerable<Operation> Operations);
}