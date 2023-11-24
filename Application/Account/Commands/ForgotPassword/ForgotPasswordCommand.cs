using System.Security.Cryptography;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        
        await SendForgotPasswordEmail(request.Email, user.PasswordResetToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await Task.FromResult(Unit.Value);
    }
    
    private string CreateRandomHexToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
    }
    
    private Task SendForgotPasswordEmail(string userEmail, string token)
    {
        var url = $"tutajURLdoFrontu?token={token}";

        var emailDto = new EmailDto
        {
            To = userEmail,
            Subject = "ZPI Meetings reset password",
            Body = $"Click on the link below to reset your password \n {url}"
        };
        
        return _emailSender.SendEmailAsync(emailDto);
    }
}