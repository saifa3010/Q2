using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByKeycloakIdAsync(string keycloakUserId, CancellationToken cancellationToken = default);
        Task AddAsync(AppUser user, CancellationToken cancellationToken = default);
    }
}