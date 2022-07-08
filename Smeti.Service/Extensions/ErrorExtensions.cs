using Grpc.Core;
using LanguageExt.Common;
using Smeti.Domain.Models.ItemModel;

namespace Smeti.Service.Extensions;

public static class ErrorExtensions
{
    public static Status ErrorToStatus(this Error error) => error.Code switch
    {
        var code and >= (int) StatusCode.Cancelled and <= (int) StatusCode.DataLoss =>
            new Status((StatusCode) code, error.Message),
        ItemErrors.Codes.ItemNotExist =>
            new Status(StatusCode.NotFound, error.Message),
        ItemErrors.Codes.ItemAlreadyExists =>
            new Status(StatusCode.AlreadyExists, error.Message),
        ItemErrors.Codes.ItemFieldDuplicates =>
            new Status(StatusCode.InvalidArgument, error.Message),
        ItemErrors.Codes.ItemAlreadyHasField =>
            new Status(StatusCode.InvalidArgument, error.Message),
        ItemErrors.Codes.ItemNotHaveField =>
            new Status(StatusCode.InvalidArgument, error.Message),
        _ => new Status(StatusCode.Internal, error.Message, error.ToException())
    };
}