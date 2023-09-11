using Application.Meetings.Commands.CreateMeeting;
using Application.Meetings.Queries.MeetingDetails.GetMeetingDetailsById;
using Application.Meetings.Queries.MeetingListItem.GetAllMeetingListItems;
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

        [HttpPost]
        public async Task<ActionResult<string>> Create([FromBody] CreateMeetingCommand command)
        {
            var newMeetingId = await Mediator.Send(command);
            return Created($"/api/Meeting/{newMeetingId}", null);
        }


        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<MeetingListItemDto>>> GetAllMeetingListItems()
        {
            var meetingListItemsDtos = await Mediator.Send(new GetAllMeetingListItemsQuery());
            return Ok(meetingListItemsDtos);
        }

        [HttpGet("pin")]
        public async Task<ActionResult<IEnumerable<MeetingListItemDto>>> GetAllMeetingPins([FromQuery] double swLat, [FromQuery] double swLng, [FromQuery] double neLat, [FromQuery] double neLng)
        {
            var meetingPinsDtos = await Mediator.Send(new GetAllMeetingListItemsQuery() { 
                SouthWestLatitude = swLat,
                SouthWestLongitude = swLng,
                NorthEastLatitude = neLat,
                NorthEastLongitude = neLng
            });
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
