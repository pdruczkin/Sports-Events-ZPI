using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Meetings.Commands.CreateMeeting;

public class CreateMeetingCommand : IRequest<string>, IMappable<Meeting>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public double Latitude { get; set; } // range: from -90 (South) to 90 (North)
    public double Longitude { get; set; } // range: from -180 (West) to 180 (East)
    public DateTime StartDateTimeUtc { get; set; }
    public DateTime EndDateTimeUtc { get; set; }
    public MeetingVisibility Visibility { get; set; } = MeetingVisibility.Private;
    public SportsDiscipline SportsDiscipline { get; set; } = SportsDiscipline.Other;
    public Difficulty Difficulty { get; set; } = Difficulty.Amateur;
}

public class CreateMeetingCommandHandler : IRequestHandler<CreateMeetingCommand, string>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;

    public CreateMeetingCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, IUserContextService userContextService)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
        _userContextService = userContextService;
    }

    public async Task<string> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        if(userId == null)
        {
            throw new UnauthorizedException("Only registered users are allowed to arrange new meetings.");
        }

        var newMeeting = _mapper.Map<Meeting>(request);
        newMeeting.OrganizerId = (Guid) userId;

        _applicationDbContext.Meetings.Add(newMeeting);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return newMeeting.Id.ToString();
    }
}
