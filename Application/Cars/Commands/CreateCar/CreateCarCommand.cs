using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using Application.Cars.Queries.GetById;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using Application.Common.Mappings;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Application.Cars.Commands.CreateCar
{
    public class CreateCarCommand : IRequest<string>, IMappable<Car>
    {
        public string Name { get; set; }
        public int MaxSpeed { get; set; }
        public Color Color { get; set; }
    }


    public class CreateCarCommandHandler : IRequestHandler<CreateCarCommand, string>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public CreateCarCommandHandler(IMapper mapper, IApplicationDbContext applicationDbContext)
        {
            _mapper = mapper;
            _applicationDbContext = applicationDbContext;
        }
        
        public async Task<string> Handle(CreateCarCommand request, CancellationToken cancellationToken)
        {
            var car = _mapper.Map<Car>(request);
            _applicationDbContext.Cars.Add(car);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return car.Id.ToString();
        }
    }
}
