using System.Reflection;
using BuildingBlocks.Domain.Primitives;
using FluentValidation;
using MediatR;

namespace BuildingBlocks.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly MethodInfo GenericFailureMethod = typeof(Result)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .Single(m => m.Name == nameof(Result.Failure) && m.IsGenericMethod);

    private readonly IValidator<TRequest>[] _validators = validators as IValidator<TRequest>[] ?? validators.ToArray();

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Length == 0)
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationTasks = _validators.Select(v => v.ValidateAsync(context, cancellationToken));
        var results = await Task.WhenAll(validationTasks);
        var failures = results.SelectMany(r => r.Errors).Where(f => f is not null).ToArray();

        if (failures.Length > 0)
        {
            var error = new Error("Validation.Failed", string.Join("; ", failures.Select(f => f.ErrorMessage)));
            return CreateFailureResponse(error);
        }

        return await next();
    }

    private static TResponse CreateFailureResponse(Error error)
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var argument = typeof(TResponse).GetGenericArguments()[0];
            var method = GenericFailureMethod.MakeGenericMethod(argument);
            return (TResponse)method.Invoke(null, new object[] { error })!;
        }

        throw new InvalidOperationException($"ValidationBehavior can only be used with Result or Result<T> responses. Actual: {typeof(TResponse).Name}");
    }
}
