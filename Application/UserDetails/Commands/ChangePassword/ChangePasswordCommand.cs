using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.UserDetails.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<Unit>
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public ChangePasswordCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
        _passwordHasher = passwordHasher;
    }
    
    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        
        if (user is null) throw new AppException("User is not found");
        
        
        var oldPasswordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.OldPassword);
        
        if (oldPasswordVerificationResult == PasswordVerificationResult.Failed) throw new AppException("OldPassword is incorrect");
        
        var newPasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);

        user.PasswordHash = newPasswordHash;
        
        return await Task.FromResult(Unit.Value);
    }
}