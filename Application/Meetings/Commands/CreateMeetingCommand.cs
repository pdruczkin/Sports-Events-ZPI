using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Meetings.Commands;

public class CreateMeetingCommand : IRequest<string>, IMappable<Meeting>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public double Latitude { get; set; } // range: from -90 (South) to 90 (North)
    public double Longitude { get; set; } // range: from -180 (West) to 180 (East)
    public DateTime StartDateTimeUtc { get; set; }
    public DateTime EndDateTimeUtc { get; set; }
    public MeetingVisibility Visibility { get; set; }
    public SportsDiscipline SportsDiscipline { get; set; }
    public Difficulty Difficulty { get; set; }
}

public class CreateMeetingCommandHandler : IRequestHandler<CreateMeetingCommand, string>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public CreateMeetingCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    public async Task<string> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
    {
        return "test";

    }
}
