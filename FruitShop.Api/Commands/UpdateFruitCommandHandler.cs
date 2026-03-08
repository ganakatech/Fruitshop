using MediatR;
using AutoMapper;
using FruitShop.Api.DTOs;
using FruitShop.Api.Services;
using FruitShop.Api.Factories;

namespace FruitShop.Api.Commands;

/// <summary>
/// Handler for UpdateFruitCommand.
/// </summary>
public class UpdateFruitCommandHandler : IRequestHandler<UpdateFruitCommand, FruitDto>
{
    private readonly IFruitService _fruitService;
    private readonly IMapper _mapper;

    public UpdateFruitCommandHandler(IFruitService fruitService, IMapper mapper)
    {
        _fruitService = fruitService;
        _mapper = mapper;
    }

    public async Task<FruitDto> Handle(UpdateFruitCommand request, CancellationToken cancellationToken)
    {
        var existingFruit = await _fruitService.GetByIdAsync(request.Id);
        if (existingFruit == null)
        {
            throw new ArgumentException($"Fruit with ID {request.Id} not found.");
        }

        var fruit = _mapper.Map<Models.Fruit>(request);
        fruit.Id = request.Id;

        var updatedFruit = await _fruitService.UpdateAsync(fruit);
        return _mapper.Map<FruitDto>(updatedFruit);
    }
}
