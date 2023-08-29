using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Car> Cars { get; }
    DbSet<User> Users { get; }
    DbSet<Domain.Entities.Meeting> Meetings { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}