using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
            => _context = context;

        public async Task<AppUser?> GetByKeycloakIdAsync(
            string keycloakUserId,
            CancellationToken cancellationToken = default)
            => await _context.Users
                .FirstOrDefaultAsync(
                    u => u.KeycloakUserId == keycloakUserId && !u.IsDeleted,
                    cancellationToken);

        public async Task AddAsync(
            AppUser user,
            CancellationToken cancellationToken = default)
            => await _context.Users.AddAsync(user, cancellationToken);
    }
}
