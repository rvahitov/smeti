using AutoMapper;
using Grpc.Core;
using JetBrains.Annotations;
using Smeti.Domain.Common;
using Smeti.Domain.Common.Errors;

namespace Smeti.Common.Mapping;

[UsedImplicitly]
public sealed class CommonProfile : Profile
{
    public CommonProfile()
    {
        CreateMap<string, FieldName>().ConvertUsing(s => new FieldName(s));
        CreateMap<FieldName, string>().ConvertUsing(n => n.Value);
        CreateMap<IDomainError, RpcException>().ConvertUsing<DomainErrorRpcExceptionConverter>();
    }
}