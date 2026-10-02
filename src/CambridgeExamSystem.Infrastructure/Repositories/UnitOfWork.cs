using CambridgeExamSystem.Application.Interfaces.Repositories;
using CambridgeExamSystem.Infrastructure.Data;

namespace CambridgeExamSystem.Infrastructure.Repositories;

public sealed class UnitOfWork(CambridgeDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);
}
