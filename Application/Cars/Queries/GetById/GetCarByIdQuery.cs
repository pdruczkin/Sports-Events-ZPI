using Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cars.Queries.GetById;

public class GetCarByIdQuery : IRequest<CarDto>
{
    public Guid Id { get; set; }
}

public class GetCarByIdQueryHandler : IRequestHandler<GetCarByIdQuery, CarDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetCarByIdQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    public async Task<CarDto> Handle(GetCarByIdQuery request, CancellationToken cancellationToken)
    {
        var car = await _dbContext.Cars.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        var carDto = _mapper.Map<CarDto>(car);

        return carDto;
    }
}