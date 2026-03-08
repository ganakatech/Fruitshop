using MediatR;
using FruitShop.Api.DTOs;

namespace FruitShop.Api.Queries;

/// <summary>
/// Query for getting a fruit by ID.
/// </summary>
public class GetFruitQuery : IRequest<FruitDto>
{
    public int Id { get; set; }
}
