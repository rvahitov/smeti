using LanguageExt;
using LanguageExt.Common;
using MediatR;

namespace Smeti.Service.Services.ItemDefinitions.Proto;

public partial class GetItemDefinitionRequest : IRequest<Either<Error, ItemDefinitionOut>>
{
}

public partial class CreateItemDefinitionRequest : IRequest<Either<Error, ItemDefinitionOut>>
{
}

public partial class AddFieldDefinitionRequest : IRequest<Either<Error, ItemDefinitionOut>>
{
}

public partial class UpdateFieldDefinitionRequest : IRequest<Either<Error, ItemDefinitionOut>>
{
}