using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Account.Commands.VerifyAccount;

public class VerifyAccountCommand : IRequest<Unit>
{
    public string Token { get; set; }
}

public class VerifyAccountCommandHandler : IRequestHandler<VerifyAccountCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public VerifyAccountCommandHandler(IApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }
    public async Task<Unit> Handle(VerifyAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.VerificationToken == request.Token,
            cancellationToken);

        if (user == null)
        {
            throw new AppException("Invalid token");
        }

        user.VerifiedAt = _dateTimeProvider.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await Task.FromResult(Unit.Value);
    }
}