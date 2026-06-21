using Application.Common.DTOs;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Users.Commands
{
    public sealed record SyncUserCommand(
        string KeycloakUserId,
        string Email,
        string FullName,
        string PhoneNumber = null) : IRequest<UserDto>;

    public sealed class SyncUserCommandHandler
        : IRequestHandler<SyncUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SyncUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserDto> Handle(
            SyncUserCommand request,
            CancellationToken cancellationToken)
        {
            // Idempotent: return existing user if already synced.
            var existing = await _userRepository.GetByKeycloakIdAsync(
                request.KeycloakUserId, cancellationToken);

            if (existing is not null)
                return existing.ToDto();

            var user = AppUser.Create(
                request.KeycloakUserId,
                request.FullName,
                request.Email,
                request.PhoneNumber);

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return user.ToDto();
        }
    }
}