using Grpc.Core;
using LanguageExt;
using MediatR;

namespace Smeti.Services.ItemDefinition.Proto;

partial class GetItemDefinitionRequest : IRequest<Either<RpcException, ItemDefinition>>
{
}