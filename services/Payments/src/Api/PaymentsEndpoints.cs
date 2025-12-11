using MediatR;
using PaymentsService.Application.PaymentIntents;

namespace PaymentsService.Api;

public static class PaymentsEndpoints
{
    public static IEndpointRouteBuilder MapPaymentsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/payments");

        group.MapPost("/charge", async (CreatePaymentIntentRequest request, HttpContext context, ISender sender) =>
        {
            var actor = context.User.Identity?.Name ?? "system";
            var command = new CreatePaymentIntentCommand(request.InvoiceId, request.Amount, request.Provider, actor);
            var result = await sender.Send(command);
            return Results.Created($"/payments/{result.Id}", result);
        });

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetPaymentIntentByIdQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        return app;
    }
}

public sealed record CreatePaymentIntentRequest(Guid InvoiceId, decimal Amount, string Provider);
