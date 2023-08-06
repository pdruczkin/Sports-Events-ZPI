using AutoMapper;

namespace Application.Common.Mappings;

public interface IMappable<T>
{
    void Mapping(Profile profile)
    {
        profile.CreateMap(GetType(), typeof(T));
        profile.CreateMap(typeof(T), GetType());
    }
}