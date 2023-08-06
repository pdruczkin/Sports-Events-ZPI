using Application.Cars.Queries.GetById;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;


public class CarController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CarDto>> GetCarById(Guid id)
    {
        return await Mediator.Send(new GetCarByIdQuery {Id = id});
    }
    
    
}