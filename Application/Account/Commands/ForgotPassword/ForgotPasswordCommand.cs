using Application.Common;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Application.Account.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest<Unit>
{
    public string Email { get; set; }
}


public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IEmailSender _emailSender;

    public ForgotPasswordCommandHandler(IApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider, IEmailSender emailSender)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
        _emailSender = emailSender;
    }
    
    public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
        
        if (user == null)
        {
            throw new AppException("User not found");
        }

        if (user.VerifiedAt is null)
        {
            throw new AppException("Account is not verified, check your email!");
        }

        user.PasswordResetToken = CreateRandomHexToken();
        user.ResetTokenExpires = _dateTimeProvider.UtcNow.AddHours(2);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var emailDto = Mails.GetForgotPasswordEmail(request.Email, user.Username, user.PasswordResetToken);
        await _emailSender.SendEmailAsync(emailDto);

        return await Task.FromResult(Unit.Value);
    }
    
    private string CreateRandomHexToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
    }
}