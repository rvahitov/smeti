using Grpc.Core;
using LanguageExt.Common;
using Smeti.Domain.Models.ItemDefinitionModel;
using Smeti.Domain.Models.ItemModel;

namespace Smeti.Service.Extensions;

public static class ErrorExtensions
{
    public static Status ErrorToStatus(this Error error) => error.Code switch
    {
        var code and >= (int) StatusCode.Cancelled and <= (int) StatusCode.DataLoss =>
            new Status((StatusCode) code, error.Message),
        ItemError.Codes.ItemNotExist =>
            new Status(StatusCode.NotFound, error.Message),
        ItemError.Codes.ItemAlreadyExists =>
            new Status(StatusCode.AlreadyExists, error.Message),
        ItemError.Codes.ItemFieldDuplicates =>
            new Status(StatusCode.InvalidArgument, error.Message),
        ItemError.Codes.ItemAlreadyHasField =>
            new Status(StatusCode.InvalidArgument, error.Message),
        ItemError.Codes.ItemNotHaveField =>
            new Status(StatusCode.InvalidArgument, error.Message),
        ItemDefinitionError.Codes.ItemDefinitionNotExist =>
            new Status(StatusCode.NotFound, error.Message),
        ItemDefinitionError.Codes.ItemDefinitionAlreadyExists =>
            new Status(StatusCode.AlreadyExists, error.Message),
        ItemDefinitionError.Codes.ItemDefinitionFieldDefinitionDuplicates =>
            new Status(StatusCode.InvalidArgument, error.Message),
        ItemDefinitionError.Codes.ItemDefinitionAlreadyHasFieldDefinition =>
            new Status(StatusCode.InvalidArgument, error.Message),
        ItemDefinitionError.Codes.ItemDefinitionNotHaveFieldDefinition =>
            new Status(StatusCode.InvalidArgument, error.Message),
        ItemDefinitionError.Codes.ItemDefinitionInvalidFieldValue =>
            new Status(StatusCode.InvalidArgument, error.Message),
        _ => new Status(StatusCode.Internal, error.Message, error.ToException())
    };
}