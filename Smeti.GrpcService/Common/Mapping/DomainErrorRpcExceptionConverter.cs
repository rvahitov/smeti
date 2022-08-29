using AutoMapper;
using Grpc.Core;
using JetBrains.Annotations;
using LanguageExt;
using Smeti.Common.Errors;
using Smeti.Domain.Common.Errors;
using Smeti.Domain.Models.ItemDefinitionModel;
using Smeti.Domain.Models.ItemModel;

namespace Smeti.Common.Mapping;

[UsedImplicitly]
public sealed class DomainErrorRpcExceptionConverter : ITypeConverter<IDomainError, RpcException>
{
    public RpcException Convert(
        IDomainError source,
        RpcException destination,
        ResolutionContext context
    ) => source switch
    {
        UnknownCommandError error                           => Convert(error),
        ItemDefinitionNotExistsError error                  => Convert(error),
        ItemDefinitionAlreadyExistError error               => Convert(error),
        ItemDefinitionAlreadyHasFieldDefinitionError error  => Convert(error),
        ItemDefinitionDoesNotHaveFieldDefinitionError error => Convert(error),
        ItemNotExistError error                             => Convert(error),
        ItemAlreadyExistError error                         => Convert(error),
        ItemDeletedError error                              => Convert(error),
        ItemFieldDuplicateError error                       => Convert(error),
        ItemAlreadyHasFieldError error                      => Convert(error),
        ItemDoesNotHaveFieldError error                     => Convert(error),
        ItemFieldsVerificationError error                   => Convert(error),
        ExceptionalError error                              => Convert(error),
        ValidationError error                               => Convert(error),
        _                                                   => throw new NotImplementedException()
    };

    private static RpcException Convert(UnknownCommandError error)
    {
        var status = new Status(StatusCode.Internal, "Unknown command");
        var metadata = new Metadata
        {
            new("Command", error.CommandName)
        };
        return new RpcException(status, metadata);
    }

    private static RpcException Convert(ItemDefinitionNotExistsError error)
    {
        var status = new Status(StatusCode.NotFound, "Item definition not found");
        var metadata = new Metadata
        {
            new("ItemDefinitionId", error.ItemDefinitionId.Value)
        };
        return new RpcException(status, metadata);
    }

    private static RpcException Convert(ItemDefinitionAlreadyExistError error)
    {
        var status = new Status(StatusCode.AlreadyExists, "Item definition already exists");
        var metadata = new Metadata
        {
            new("ItemDefinitionId", error.ItemDefinitionId.Value)
        };
        return new RpcException(status, metadata);
    }

    private static RpcException Convert(ItemDefinitionAlreadyHasFieldDefinitionError error)
    {
        var status = new Status(StatusCode.AlreadyExists, "Item definition already has field definition");
        var (id, fieldName) = error;
        var metadata = new Metadata
        {
            new("ItemDefinitionId", id.Value),
            new("FieldName", fieldName.Value)
        };
        return new RpcException(status, metadata);
    }

    private static RpcException Convert(ItemDefinitionDoesNotHaveFieldDefinitionError error)
    {
        var status = new Status(StatusCode.NotFound, "Item definition does not have field definition");
        var (id, fieldName) = error;
        var metadata = new Metadata
        {
            new("ItemDefinitionId", id.Value),
            new("FieldDefinition", fieldName.Value)
        };
        return new RpcException(status, metadata);
    }

    private static RpcException Convert(ItemNotExistError error)
    {
        var status = new Status(StatusCode.NotFound, "Item not found");
        var metadata = new Metadata
        {
            new("ItemId", error.ItemId.Value)
        };
        return new RpcException(status, metadata);
    }

    private static RpcException Convert(ItemAlreadyExistError error)
    {
        var status = new Status(StatusCode.AlreadyExists, "Item already exists");
        var metadata = new Metadata
        {
            new("ItemId", error.ItemId.Value)
        };
        return new RpcException(status, metadata);
    }

    private static RpcException Convert(ItemDeletedError error)
    {
        var status = new Status(StatusCode.FailedPrecondition, "Item is deleted");
        var metadata = new Metadata
        {
            new("ItemId", error.ItemId.Value)
        };
        return new RpcException(status, metadata);
    }

    private static RpcException Convert(ItemFieldDuplicateError error)
    {
        var status = new Status(StatusCode.InvalidArgument, "Request has duplicated fields");
        var metadata = new Metadata
        {
            new("ItemId", error.ItemId.Value),
            new("Fields", error.FieldNames.Apply(names => string.Join(", ", names.Select( n => n.Value))))
        };
        return new RpcException(status, metadata);
    }

    private static RpcException Convert(ItemAlreadyHasFieldError error)
    {
        var status = new Status(StatusCode.AlreadyExists, "Item already has field");
        var metadata = new Metadata
        {
            new("ItemId", error.ItemId.Value),
            new("Field", error.FieldName.Value)
        };
        return new RpcException(status, metadata);
    }

    private static RpcException Convert(ItemDoesNotHaveFieldError error)
    {
        var status = new Status(StatusCode.NotFound, "Item does not have field");
        var metadata = new Metadata
        {
            new("ItemId", error.ItemId.Value),
            new("Field", error.FieldName.Value)
        };
        return new RpcException(status, metadata);
    }

    private  static RpcException Convert(ItemFieldsVerificationError error)
    {
        var status = new Status(StatusCode.InvalidArgument, "Request has invalid fields");
        var metadata = new Metadata
        {
            new("ItemId", error.ItemId.Value),
            new("ItemDefinitionId", error.ItemDefinitionId.Value)
        };
        error.InvalidFields.Iter(t => metadata.Add(t.Item1.Value, ReasonToString(t.Item2)));
        return new RpcException(status, metadata);
    }

    private static string ReasonToString(InvalidFieldReason reason) =>
        reason switch
        {
            InvalidFieldReason.NotRegistered    => "Field definition is not registered for field",
            InvalidFieldReason.InvalidValueType => "Invalid field value type",
            InvalidFieldReason.Duplicate        => "Field duplicate",
            _                                   => throw new ArgumentOutOfRangeException(nameof(reason), reason, null )
        };

    private static RpcException Convert(ExceptionalError source)
    {
        var status = new Status(StatusCode.Internal, source.Exception.Message, source.Exception);
        return new RpcException(status);
    }

    private static RpcException Convert(ValidationError source)
    {
        var status = new Status(StatusCode.InvalidArgument, "Request is invalid");
        var metadata = new Metadata();
        source.ValidationResult.Errors.Iter(e => metadata.Add(e.PropertyName, e.ErrorMessage));
        return new RpcException(status, metadata);
    }
}