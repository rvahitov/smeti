using Grpc.Core;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Smeti.Service.Extensions;
using Smeti.Service.Services.Items.Proto;

namespace Smeti.Service.Services.Items;

public sealed class ItemsService : Proto.ItemsService.ItemsServiceBase
{
    private readonly ILogger<ItemsService> _logger;
    private readonly IMediator _mediator;

    public ItemsService(
        IMediator mediator,
        ILogger<ItemsService> logger
    )
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override Task<ItemOut> GetItem(GetItemRequest request, ServerCallContext context) =>
        HandleRequest(request, context);

    public override Task<ItemOut> CreateItem(CreateItemRequest request, ServerCallContext context) =>
        HandleRequest(request, context);

    public override Task<ItemOut> AddField(AddFieldRequest request, ServerCallContext context) =>
        HandleRequest(request, context);

    public override Task<ItemOut> UpdateField(UpdateFieldRequest request, ServerCallContext context) =>
        HandleRequest(request, context);

    private async Task<ItemOut> HandleRequest(IRequest<Either<Error, ItemOut>> request, ServerCallContext context)
    {
        _logger.LogDebug("Request: {@RequestToItemsService}", request);
        var response = await _mediator.TrySend(request, context.CancellationToken);
        return Prelude.match(
            response,
            dto =>
            {
                _logger.LogInformation("Request handled: {@RequestToItemsService}", request);
                return dto;
            },
            error =>
            {
                _logger.LogError(error.ToException(), "Request failed: {@RequestToItemsService}", request);
                throw new RpcException(error.ErrorToStatus());
            }
        );
    }
}