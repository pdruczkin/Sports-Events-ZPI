using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Car> Cars { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}