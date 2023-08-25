using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

    public LoginUserCommandHandler(IApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
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

        var claimList = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, $"{user.Username}"),
            new(ClaimTypes.Role, $"{user.Role}"),
            new(ClaimTypes.Email, $"{user.Email}"),
            new(ClaimTypes.Gender, $"{user.Gender}"),
            new(ClaimTypes.DateOfBirth, $"{user.DateOfBirth.ToShortDateString()}"),
        };

        return "test";
    }
}
