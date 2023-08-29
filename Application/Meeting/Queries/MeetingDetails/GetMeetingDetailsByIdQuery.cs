using Application.Cars.Queries.GetById;
using Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Meeting.Queries.MeetingDetails
{
    public class GetMeetingDetailsByIdQuery : IRequest<MeetingDetailsDto>
    {
        public Guid Id { get; set; }
    }

    public class GetMeetingDetailsByIdQueryHandler : IRequestHandler<GetMeetingDetailsByIdQuery, MeetingDetailsDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetMeetingDetailsByIdQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<MeetingDetailsDto> Handle(GetMeetingDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            var meetingDetails = _dbContext.Meetings.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

            var meetingDetailsDto = _mapper.Map<MeetingDetailsDto>(meetingDetails);
            meetingDetailsDto.OrganizerUsername = _dbContext.Users.FirstOrDefaultAsync(x => x.Id == meetingDetails.Result.OrganizerId, cancellationToken: cancellationToken).Result.Username;

            return meetingDetailsDto;
        }
    }

}
