using AutoMapper;
using FluentValidation;
using LanguageExt;
using MediatR;
using Smeti.Common.Errors;
using Smeti.Domain.Common.Errors;

namespace Smeti.Common.Extensions;

using static Prelude;

public static class TryExtensions
{
    public static EitherAsync<IDomainError, TResult> TrySendAsync<TResult>(
        this IMediator mediator,
        IRequest<TResult> request,
        CancellationToken cancellationToken = default
    ) => TryAsync(() => mediator.Send(request, cancellationToken))
       .ToEither(e => (IDomainError) new ExceptionalError(e.ToException()));

    public static EitherAsync<IDomainError, T> TryValidateAsync<T>(
        this IValidator<T> validator,
        T target,
        CancellationToken cancellationToken = default
    ) => TryAsync(async () =>
         {
             var validationResult = await validator.ValidateAsync(target, cancellationToken);
             if(validationResult.IsValid) return target;
             return Left<IDomainError, T>(new ValidationError(validationResult));
         })
        .ToEither(e => (IDomainError) new ExceptionalError(e.ToException()))
        .Bind(e => e.ToAsync());

    public static EitherAsync<IDomainError, T> TryMapAsync<T>(this IMapper mapper, object source) =>
        TryAsync(() => mapper.Map<T>(source).AsTask())
           .ToEither(e => (IDomainError) new ExceptionalError(e.ToException()));
}