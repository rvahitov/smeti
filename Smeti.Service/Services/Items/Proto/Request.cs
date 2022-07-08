using LanguageExt;
using LanguageExt.Common;
using MediatR;

namespace Smeti.Service.Services.Items.Proto;

public partial class CreateItemRequest : IRequest<Either<Error, ItemOut>>
{
}

public partial class AddFieldRequest : IRequest<Either<Error, ItemOut>>
{
}

public partial class UpdateFieldRequest : IRequest<Either<Error, ItemOut>>
{
}

public partial class GetItemRequest : IRequest<Either<Error, ItemOut>>
{
}