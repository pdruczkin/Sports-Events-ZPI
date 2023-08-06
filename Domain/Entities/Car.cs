using Domain.Enums;

namespace Domain.Entities;

public class Car
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int MaxSpeed { get; set; }
    public Color Color { get; set; }
}