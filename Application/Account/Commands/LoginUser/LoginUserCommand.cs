using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Account.Commands.LoginUser;

public class LoginUserCommand : IRequest<string>
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AuthenticationSettings _authSettings;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LoginUserCommandHandler(IApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher, AuthenticationSettings authSettings, IDateTimeProvider dateTimeProvider) 
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _authSettings = authSettings;
        _dateTimeProvider = dateTimeProvider;
    }
    
    public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext
            .Users
            .SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken: cancellationToken);

        if (user == null) throw new AppException("Email or Password is incorrect");

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed) 
            throw new AppException("Email or Password is incorrect");

        if (user.VerifiedAt is null)
        {
            throw new AppException("Account is not verified, check your email!");
        }
        
        
        var claimList = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, $"{user.Username}"),
            new(ClaimTypes.Role, $"{user.Role}"),
            new(ClaimTypes.Email, $"{user.Email}"),
            new(ClaimTypes.Gender, $"{user.Gender}"),
            new(ClaimTypes.DateOfBirth, $"{user.DateOfBirth.ToShortDateString()}"),
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.JwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = _dateTimeProvider.UtcNow.AddDays(_authSettings.JwtExpireDays);

        var token = new JwtSecurityToken(_authSettings.JwtIssuer, _authSettings.JwtIssuer, claimList, null, expires, credentials);

        var tokenHandler = new JwtSecurityTokenHandler();

        var generatedToken = tokenHandler.WriteToken(token);

        return generatedToken;
    }
}
