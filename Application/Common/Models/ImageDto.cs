using Application.Common.Mappings;
using Domain.Entities;

namespace Application.Common.Models;

public class ImageDto : IMappable<Image>
{
    public string PublicId { get; set; } = "";
    public string Url { get; set; } = "";
}