using Grpc.Core;
using LanguageExt;
using MediatR;

namespace Smeti.Services.ItemDefinition.Proto;

partial class CreateItemDefinitionRequest : IRequest<Either<RpcException, ItemDefinition>>
{
}