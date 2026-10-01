using Application.Services.Repositories;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Core.CrossCuttingConcerns.Exception.Types;
using Core.Security.Constants;
using MediatR;

namespace Application.Features.Users.Commands.ToggleStatus;

public class ToggleUserStatusCommand : IRequest<bool>, ISecuredRequest, ITransactionalRequest, ILoggableRequest
{
    public int UserId { get; set; }

    public string[] Roles => [GeneralOperationClaims.Admin, "Users.Write", "Users.Update"];

    public class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public ToggleUserStatusCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(u => u.Id == request.UserId, cancellationToken: cancellationToken);
            if (user == null)
                throw new BusinessException("Kullanıcı bulunamadı.");

            user.Status = !user.Status;
            await _userRepository.UpdateAsync(user, cancellationToken);
            return user.Status;
        }
    }
}
