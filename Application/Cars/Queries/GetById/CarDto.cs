using Application.Common.Mappings;
using Domain.Entities;
using Domain.Enums;

namespace Application.Cars.Queries.GetById;

public class CarDto : IMappable<Car>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int MaxSpeed { get; set; }
    public Color Color { get; set; }
}