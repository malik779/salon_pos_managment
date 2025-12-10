using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using PaymentsService.Application.Payments.Commands.ChargePayment;
using PaymentsService.Application.Payments.Commands.CompletePayment;
using PaymentsService.Application.Payments.Commands.TokenizeCard;

namespace PaymentsService.Api;

public static class PaymentsEndpoints
{
    public static IEndpointRouteBuilder MapPaymentsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/payments/charge", async (ChargePaymentCommand command, IMediator mediator) => Results.Ok(await mediator.Send(command)));
        app.MapPost("/payments/tokenize", async (TokenizeCardCommand command, IMediator mediator) => Results.Ok(await mediator.Send(command)));

        app.MapPost("/payments/webhook", async (HttpRequest request, IMediator mediator) =>
        {
            var paymentIdHeader = request.Headers["X-Payment-Id"].FirstOrDefault();
            if (!Guid.TryParse(paymentIdHeader, out var paymentId))
            {
                return Results.BadRequest();
            }

            var result = await mediator.Send(new CompletePaymentCommand(paymentId));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        return app;
    }
}
