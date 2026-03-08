using MediatR;
using AutoMapper;
using FruitShop.Api.DTOs;
using FruitShop.Api.Services;
using FruitShop.Api.Factories;

namespace FruitShop.Api.Commands;

/// <summary>
/// Handler for CreateFruitCommand.
/// </summary>
public class CreateFruitCommandHandler : IRequestHandler<CreateFruitCommand, FruitDto>
{
    private readonly IFruitService _fruitService;
    private readonly IMapper _mapper;

    public CreateFruitCommandHandler(IFruitService fruitService, IMapper mapper)
    {
        _fruitService = fruitService;
        _mapper = mapper;
    }

    public async Task<FruitDto> Handle(CreateFruitCommand request, CancellationToken cancellationToken)
    {
        var fruit = FruitFactory.Create(
            request.Name,
            request.BasePrice,
            request.PricingStrategyType,
            request.DiscountThreshold,
            request.DiscountPercentage);

        var createdFruit = await _fruitService.CreateAsync(fruit);
        return _mapper.Map<FruitDto>(createdFruit);
    }
}
