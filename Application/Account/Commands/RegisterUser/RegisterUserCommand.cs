﻿using System.Security.Cryptography;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Account.Commands.RegisterUser;

public class RegisterUserCommand : IRequest<string>, IMappable<User>
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }


    public void Mapping(Profile profile)
    {
        profile.CreateMap<RegisterUserCommand, User>()
            .ForMember(u => u.DateOfBirth, o => o.MapFrom(s => new DateTime(s.DateOfBirth.Year, s.DateOfBirth.Month, s.DateOfBirth.Day)));
    }
}



public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, string>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IEmailSender _emailSender;

    public RegisterUserCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, IPasswordHasher<User> passwordHasher, IEmailSender emailSender)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _emailSender = emailSender;
    }
    
    
    public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<User>(request);

        var passwordHash = _passwordHasher.HashPassword(user, request.Password);
        user.PasswordHash = passwordHash;

        var verificationToken = CreateRandomBase64Token();
        user.VerificationToken = verificationToken;
        
        _applicationDbContext.Users.Add(user);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        
        SendVerificationEmail(user.Email, user.Username, user.VerificationToken);
        
        return user.Id.ToString();
    }

    private string CreateRandomBase64Token()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
    }

    private void SendVerificationEmail(string userEmail, string username, string token)
    {
        var url = $"tutajURLdoFrontu?token={token}";

        var emailDto = new EmailDto()
        {
            To = userEmail,
            Subject = "Verify your Account",
            Body = $"Hello {username}, click on link below to complete account activation process \n {url}"
        };
        
        _emailSender.SendEmail(emailDto);
    }
}