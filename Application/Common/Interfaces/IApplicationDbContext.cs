using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Meeting> Meetings { get; }
    DbSet<Friendship> Friendships { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}