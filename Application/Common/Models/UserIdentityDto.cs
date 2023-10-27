using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.Models;

public class UserIdentityDto : IMappable<User>
{
    public Guid Id { get; set; }
    public string Username { get; set; }
}
