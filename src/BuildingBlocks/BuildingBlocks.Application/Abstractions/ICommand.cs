using BuildingBlocks.Domain.Primitives;
using MediatR;

namespace BuildingBlocks.Application.Abstractions;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}

public interface ICommand : IRequest<Result>
{
}
