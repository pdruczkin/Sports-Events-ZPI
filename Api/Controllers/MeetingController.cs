using Application.Meeting.Queries.MeetingDetails;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class MeetingController : ApiControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<MeetingDetailsDto>> GetById(Guid id)
        {
            return await Mediator.Send(new GetMeetingDetailsByIdQuery() { Id = id });
        }



    }
}
