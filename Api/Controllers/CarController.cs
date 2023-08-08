using Application.Cars.Commands.CreateCar;
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

    [HttpPost]
    public async Task<ActionResult<string>> CreateCar([FromBody] CreateCarCommand command)
    {
        var newCarGuid = await Mediator.Send(command);
        return Created(newCarGuid, null);
    }

}