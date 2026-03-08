using MediatR;

namespace FruitShop.Api.Commands;

/// <summary>
/// Command for deleting a fruit.
/// </summary>
public class DeleteFruitCommand : IRequest<bool>
{
    public int Id { get; set; }
}
