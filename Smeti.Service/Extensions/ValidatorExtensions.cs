using FluentValidation;
using Grpc.Core;
using LanguageExt;
using LanguageExt.Common;

namespace Smeti.Service.Extensions;

public static class ValidatorExtensions
{
    public static Either<Error, T> DoValidate<T>(
        this IValidator<T> validator,
        T target
    )
    {
        var validationResult = validator.Validate(target);
        if(validationResult.IsValid)
            return target;

        var errorMessage =
            validationResult
               .Errors
               .Select(e => e.ErrorMessage)
               .Apply(messages => string.Join("\n", messages));
        return Error.New((int)StatusCode.InvalidArgument, errorMessage);
    }
}