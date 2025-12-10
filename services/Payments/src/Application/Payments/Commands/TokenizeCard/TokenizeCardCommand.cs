using FluentValidation;
using MediatR;
using PaymentsService.Application.Payments.Models;

namespace PaymentsService.Application.Payments.Commands.TokenizeCard;

public sealed record TokenizeCardCommand(string CardNumber, string Expiry, string Brand) : IRequest<TokenizedCard>;

public sealed class TokenizeCardCommandValidator : AbstractValidator<TokenizeCardCommand>
{
    public TokenizeCardCommandValidator()
    {
        RuleFor(x => x.CardNumber).CreditCard();
        RuleFor(x => x.Expiry).NotEmpty();
    }
}

public sealed class TokenizeCardCommandHandler : IRequestHandler<TokenizeCardCommand, TokenizedCard>
{
    public Task<TokenizedCard> Handle(TokenizeCardCommand request, CancellationToken cancellationToken)
    {
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        return Task.FromResult(new TokenizedCard(token, request.Brand, request.CardNumber[^4..]));
    }
}
