using MediatR;
using FruitShop.Api.Commands;
using FruitShop.Api.Queries;
using FruitShop.Api.DTOs;
using FruitShop.Api.Models;

namespace FruitShop.Api.Extensions;

/// <summary>
/// Extension methods for configuring WebApplication endpoints.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Maps all API endpoints for the application.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    public static void MapApiEndpoints(this WebApplication app)
    {
        // Fruit endpoints
        app.MapGet("/fruits", async (IMediator mediator) =>
        {
            var query = new GetAllFruitsQuery();
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetAllFruits")
        .WithTags("Fruits")
        .Produces<List<FruitDto>>(StatusCodes.Status200OK);

        app.MapGet("/fruits/{id}", async (IMediator mediator, int id) =>
        {
            var query = new GetFruitQuery { Id = id };
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetFruit")
        .WithTags("Fruits")
        .Produces<FruitDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/fruits", async (IMediator mediator, CreateFruitRequest request) =>
        {
            var command = new CreateFruitCommand
            {
                Name = request.Name,
                BasePrice = request.BasePrice,
                PricingStrategyType = request.PricingStrategyType,
                DiscountThreshold = request.DiscountThreshold,
                DiscountPercentage = request.DiscountPercentage
            };
            var result = await mediator.Send(command);
            return Results.Created($"/fruits/{result.Id}", result);
        })
        .WithName("CreateFruit")
        .WithTags("Fruits")
        .WithSummary("Create a new fruit")
        .WithDescription("Creates a new fruit with the specified pricing strategy. " +
                         "Valid PricingStrategyType values: 'PerKg' (price per kilogram), 'PerItem' (price per item), " +
                         "or 'Discounted' (requires DiscountThreshold and DiscountPercentage).")
        .Produces<FruitDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/fruits/{id}", async (IMediator mediator, int id, UpdateFruitRequest request) =>
        {
            var command = new UpdateFruitCommand
            {
                Id = id,
                Name = request.Name,
                BasePrice = request.BasePrice,
                PricingStrategyType = request.PricingStrategyType,
                DiscountThreshold = request.DiscountThreshold,
                DiscountPercentage = request.DiscountPercentage
            };
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateFruit")
        .WithTags("Fruits")
        .WithSummary("Update an existing fruit")
        .WithDescription("Updates an existing fruit with the specified pricing strategy. " +
                         "Valid PricingStrategyType values: 'PerKg' (price per kilogram), 'PerItem' (price per item), " +
                         "or 'Discounted' (requires DiscountThreshold and DiscountPercentage).")
        .Produces<FruitDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        app.MapDelete("/fruits/{id}", async (IMediator mediator, int id) =>
        {
            var command = new DeleteFruitCommand { Id = id };
            await mediator.Send(command);
            return Results.NoContent();
        })
        .WithName("DeleteFruit")
        .WithTags("Fruits")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        // Order endpoints
        app.MapPost("/orders/calculate", async (IMediator mediator, Order order) =>
        {
            var query = new CalculateOrderTotalQuery { Order = order };
            var result = await mediator.Send(query);
            return Results.Ok(new { Total = result });
        })
        .WithName("CalculateOrderTotal")
        .WithTags("Orders")
        .WithSummary("Calculate the total price of an order")
        .WithDescription("Calculates the total price for an order containing multiple fruit items. " +
                         "Each order item must specify a valid FruitId and an Amount. " +
                         "The Amount field interpretation depends on the fruit's pricing strategy: " +
                         "- For fruits with 'PerKg' or 'Discounted' pricing: Amount represents weight in kilograms (e.g., 2.5 for 2.5 kg). " +
                         "- For fruits with 'PerItem' pricing: Amount represents quantity as number of items (e.g., 10 for 10 items). " +
                         "Use GET /fruits to see available fruits and their pricing strategies. " +
                         "Example request body: { \"items\": [{ \"fruitId\": 1, \"amount\": 2.5 }, { \"fruitId\": 2, \"amount\": 10 }] }")
        .Produces<object>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
