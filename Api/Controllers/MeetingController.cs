using Application.Meetings.Queries.MeetingDetails.GetMeetingDetailsById;
using Application.Meetings.Queries.MeetingPin.GetAllMeetingPins;
using Application.Meetings.Queries.MeetingPin.GetMeetingPinDetailsById;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class MeetingController : ApiControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<MeetingDetailsDto>> GetById(Guid id)
        {
            var meetingDetailsDto = await Mediator.Send(new GetMeetingDetailsByIdQuery() { Id = id });
            return Ok(meetingDetailsDto); 
        }

        [HttpGet("pin")]
        public async Task<ActionResult<IEnumerable<MeetingPinDto>>> GetAllMeetingPins()
        {
            var meetingPinsDtos = await Mediator.Send(new GetAllMeetingPinsQuery());
            return Ok(meetingPinsDtos);
        }

        [HttpGet("pin/{id}")]
        public async Task<ActionResult<MeetingPinDetailsDto>> GetPinDetailsById(Guid id)
        {
            var pinDetailsDto = await Mediator.Send(new GetMeetingPinDetailsByIdQuery() { Id = id });
            return Ok(pinDetailsDto);
        }

    }
}
