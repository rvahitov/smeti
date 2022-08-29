using Grpc.Core;
using LanguageExt;
using MediatR;
using Smeti.Services.ItemDefinition.Proto;

namespace Smeti.Services.ItemDefinition;

public sealed class ItemDefinitionService : Proto.ItemDefinitionService.ItemDefinitionServiceBase
{
    private readonly IMediator _mediator;

    public ItemDefinitionService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override Task<Proto.ItemDefinition> CreateItemDefinition(
        CreateItemDefinitionRequest request,
        ServerCallContext context
    ) => ExecuteRequest(request, context);

    public override Task<Proto.ItemDefinition> AddFieldDefinition(
        AddFieldDefinitionRequest request,
        ServerCallContext context
    ) => ExecuteRequest(request, context);

    public override Task<Proto.ItemDefinition> RemoveFieldDefinition(
        RemoveFieldDefinitionRequest request,
        ServerCallContext context
    ) => ExecuteRequest(request, context);

    public override Task<Proto.ItemDefinition> GetItemDefinition(
        GetItemDefinitionRequest request,
        ServerCallContext context
    ) => ExecuteRequest(request, context);

    private async Task<T> ExecuteRequest<T>(IRequest<Either<RpcException, T>> request, ServerCallContext context)
    {
        var response = await _mediator.Send(request, context.CancellationToken).ConfigureAwait(false);
        return response.Case switch
        {
            T t            => t,
            RpcException e => throw e,
            _              => throw new NotSupportedException()
        };
    }
}