using Grpc.Core;
using LanguageExt;
using MediatR;

namespace Smeti.Services.ItemDefinition.Proto;

partial class RemoveFieldDefinitionRequest : IRequest<Either<RpcException, ItemDefinition>>
{
}