using MediatR;
using FruitShop.Api.Services;

namespace FruitShop.Api.Commands;

/// <summary>
/// Handler for DeleteFruitCommand.
/// </summary>
public class DeleteFruitCommandHandler : IRequestHandler<DeleteFruitCommand, bool>
{
    private readonly IFruitService _fruitService;

    public DeleteFruitCommandHandler(IFruitService fruitService)
    {
        _fruitService = fruitService;
    }

    public async Task<bool> Handle(DeleteFruitCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _fruitService.DeleteAsync(request.Id);
        if (!deleted)
        {
            throw new KeyNotFoundException($"Fruit with ID {request.Id} not found.");
        }
        return deleted;
    }
}
