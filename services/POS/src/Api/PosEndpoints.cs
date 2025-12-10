using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using PosService.Application.Invoices.Commands.CapturePayment;
using PosService.Application.Invoices.Commands.CloseDay;
using PosService.Application.Invoices.Commands.CreateInvoice;
using PosService.Application.Invoices.Commands.FinalizeInvoice;
using PosService.Application.Invoices.Commands.RefundInvoice;

namespace PosService.Api;

public static class PosEndpoints
{
    public static IEndpointRouteBuilder MapPosEndpoints(this IEndpointRouteBuilder app)
    {
        var invoices = app.MapGroup("/invoices");

        invoices.MapPost("", async (CreateInvoiceCommand command, IMediator mediator) =>
        {
            var invoice = await mediator.Send(command);
            return Results.Created($"/invoices/{invoice.Id}", invoice);
        });

        invoices.MapPost("/{id:guid}/finalize", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new FinalizeInvoiceCommand(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        invoices.MapPost("/{id:guid}/refund", async (Guid id, RefundInvoiceCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command with { InvoiceId = id });
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        var payments = app.MapGroup("/payments");
        payments.MapPost("", async (CapturePaymentCommand command, IMediator mediator) => Results.Ok(await mediator.Send(command)));

        app.MapPost("/closereads", async (CloseDayCommand command, IMediator mediator) => Results.Ok(await mediator.Send(command)));
        return app;
    }
}
