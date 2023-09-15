using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Meeting> Meetings { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}