using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces;

public interface IImageService
{
    Task<ImageUploadResult> AddImageAsync(IFormFile file);
    Task<DeletionResult> DeleteImageAsync(string publicId);
}