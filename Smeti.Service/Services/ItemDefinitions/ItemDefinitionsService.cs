using Grpc.Core;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Smeti.Domain.Extensions;
using Smeti.Service.Extensions;
using Smeti.Service.Services.ItemDefinitions.Proto;

namespace Smeti.Service.Services.ItemDefinitions;

public sealed class ItemDefinitionsService
    : Proto.ItemDefinitionsService.ItemDefinitionsServiceBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ItemDefinitionsService> _logger;

    public ItemDefinitionsService(
        IMediator mediator,
        ILogger<ItemDefinitionsService> logger
    )
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override Task<ItemDefinitionOut> GetItemDefinition(
        GetItemDefinitionRequest request,
        ServerCallContext context
    ) => HandleRequest(request, context);

    public override Task<ItemDefinitionOut> CreateItemDefinition(
        CreateItemDefinitionRequest request,
        ServerCallContext context
    ) => HandleRequest(request, context);

    public override Task<ItemDefinitionOut> AddFieldDefinition(
        AddFieldDefinitionRequest request,
        ServerCallContext context
    ) => HandleRequest(request, context);

    public override Task<ItemDefinitionOut> UpdateFieldDefinition(
        UpdateFieldDefinitionRequest request,
        ServerCallContext context
    ) => HandleRequest(request, context);

    private async Task<ItemDefinitionOut> HandleRequest(
        IRequest<Either<Error, ItemDefinitionOut>> request,
        ServerCallContext context
    )
    {
        _logger.LogDebug("Request: {@RequestToItemDefinitionsService}", request);
        var response = await _mediator.TrySend(request, context.CancellationToken);
        return Prelude.match(
            response,
            dto =>
            {
                _logger.LogInformation("Request handled: {@RequestToItemDefinitionsService}", request);
                return dto;
            },
            error =>
            {
                _logger.LogError(error.ToException(), "Request failed: {@RequestToItemDefinitionsService}", request);
                throw new RpcException(error.ErrorToStatus());
            }
        );
    }
}