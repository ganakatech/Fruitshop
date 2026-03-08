using MediatR;
using FruitShop.Api.DTOs;

namespace FruitShop.Api.Queries;

/// <summary>
/// Query for getting all fruits.
/// </summary>
public class GetAllFruitsQuery : IRequest<List<FruitDto>>
{
}
