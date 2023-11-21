using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.Models;

public class ChatMessageDto : IMappable<ChatMessage>
{
    public string Value { get; set; } = "";
    public DateTime SentAtUtc { get; set; }
    public string Username { get; set; } = "Unknown";
    public Guid UserId { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<ChatMessage, ChatMessageDto>()
            .ForMember(x => x.Username, o => o.MapFrom(s => s.User.Username));
    }
    
}