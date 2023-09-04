using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Account.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest<Unit>
{
    public string Token { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPasswordHasher<User> _passwordHasher;

    public ResetPasswordCommandHandler(IApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
        _passwordHasher = passwordHasher;
    }
    
    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.PasswordResetToken == request.Token, cancellationToken);
        
        if (user == null)
        {
            throw new AppException("User not found");
        }

        if (user.VerifiedAt is null)
        {
            throw new AppException("Account is not verified, check your email!");
        }

        if (user.ResetTokenExpires is null || user.ResetTokenExpires.Value < _dateTimeProvider.UtcNow)
        {
            throw new AppException("The Password reset token has expired");
        }
        
        var passwordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        user.PasswordHash = passwordHash;
        user.PasswordResetToken = null;
        user.ResetTokenExpires = null;

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return await Task.FromResult(Unit.Value);
    }
}