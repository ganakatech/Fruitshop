using MediatR;
using AutoMapper;
using FruitShop.Api.DTOs;
using FruitShop.Api.Services;

namespace FruitShop.Api.Queries;

/// <summary>
/// Handler for GetAllFruitsQuery.
/// </summary>
public class GetAllFruitsQueryHandler : IRequestHandler<GetAllFruitsQuery, List<FruitDto>>
{
    private readonly IFruitService _fruitService;
    private readonly IMapper _mapper;

    public GetAllFruitsQueryHandler(IFruitService fruitService, IMapper mapper)
    {
        _fruitService = fruitService;
        _mapper = mapper;
    }

    public async Task<List<FruitDto>> Handle(GetAllFruitsQuery request, CancellationToken cancellationToken)
    {
        var fruits = await _fruitService.GetAllAsync();
        return _mapper.Map<List<FruitDto>>(fruits);
    }
}
