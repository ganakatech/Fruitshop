using MediatR;
using AutoMapper;
using FruitShop.Api.DTOs;
using FruitShop.Api.Services;

namespace FruitShop.Api.Queries;

/// <summary>
/// Handler for GetFruitQuery.
/// </summary>
public class GetFruitQueryHandler : IRequestHandler<GetFruitQuery, FruitDto>
{
    private readonly IFruitService _fruitService;
    private readonly IMapper _mapper;

    public GetFruitQueryHandler(IFruitService fruitService, IMapper mapper)
    {
        _fruitService = fruitService;
        _mapper = mapper;
    }

    public async Task<FruitDto> Handle(GetFruitQuery request, CancellationToken cancellationToken)
    {
        var fruit = await _fruitService.GetByIdAsync(request.Id);
        if (fruit == null)
        {
            throw new KeyNotFoundException($"Fruit with ID {request.Id} not found.");
        }

        return _mapper.Map<FruitDto>(fruit);
    }
}
