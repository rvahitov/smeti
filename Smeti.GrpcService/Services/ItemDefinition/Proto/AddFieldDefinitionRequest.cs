using Grpc.Core;
using LanguageExt;
using MediatR;

namespace Smeti.Services.ItemDefinition.Proto;

partial class AddFieldDefinitionRequest : IRequest<Either<RpcException, ItemDefinition>>
{
}