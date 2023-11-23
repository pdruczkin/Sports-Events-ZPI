using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Models;

public class ParticipantIdentityDto : IMappable<User>
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public InvitationStatus Status { get; set; }
}
