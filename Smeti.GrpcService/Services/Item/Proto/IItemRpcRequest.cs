using Grpc.Core;
using LanguageExt;
using MediatR;

namespace Smeti.Services.Item.Proto;

public interface IItemRpcRequest : IRequest<Either<RpcException, Item>>
{
}