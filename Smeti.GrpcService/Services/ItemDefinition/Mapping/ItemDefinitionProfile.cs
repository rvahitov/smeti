using AutoMapper;
using JetBrains.Annotations;
using Smeti.Domain.Models.ItemDefinitionModel;
using Smeti.Services.ItemDefinition.Proto;
using FieldDefinition = Smeti.Domain.Models.ItemDefinitionModel.FieldDefinition;
using FieldValueType = Smeti.Domain.Models.ItemDefinitionModel.FieldValueType;
using ProtoFieldDefinition = Smeti.Services.ItemDefinition.Proto.FieldDefinition;
using ProtoValueType = Smeti.Services.ItemDefinition.Proto.FieldValueType;
using ProtoItemDefinition = Smeti.Services.ItemDefinition.Proto.ItemDefinition;
using ModelItemDefinition = Smeti.Domain.Models.ItemDefinitionModel.ItemDefinition;

namespace Smeti.Services.ItemDefinition.Mapping;

[UsedImplicitly]
public sealed class ItemDefinitionProfile : Profile
{
    public ItemDefinitionProfile()
    {
        CreateMap<string, ItemDefinitionId>().ConvertUsing(value => new ItemDefinitionId(value));
        CreateMap<ItemDefinitionId, string>().ConvertUsing(id => id.Value);
        CreateMap<string, ItemDefinitionName>().ConvertUsing(value => new ItemDefinitionName(value));
        CreateMap<ItemDefinitionName, string>().ConvertUsing(name => name.Value);
        CreateMap<ProtoValueType, FieldValueType>().ConvertUsing<FieldValueTypeConverter>();
        CreateMap<FieldValueType, ProtoValueType>().ConvertUsing<FieldValueTypeConverter>();
        CreateMap<ProtoFieldDefinition, FieldDefinition>();
        CreateMap<FieldDefinition, ProtoFieldDefinition>();
        CreateMap<ProtoItemDefinition, ModelItemDefinition>();
        CreateMap<ModelItemDefinition, ProtoItemDefinition>()
           .ConvertUsing(( model,  _,  context) =>
            {
                var result = new ProtoItemDefinition
                {
                    Id = model.Id.Value,
                    Name = model.Name.Value,
                };
                result.FieldDefinitions.AddRange(
                    model.FieldDefinitions.Select(fd => context.Mapper.Map<ProtoFieldDefinition>(fd))
                );
                return result;
            });
        CreateMap<CreateItemDefinitionRequest, CreateItemDefinitionCommand>();
        CreateMap<AddFieldDefinitionRequest, AddFieldDefinitionCommand>();
        CreateMap<RemoveFieldDefinitionRequest, RemoveFieldDefinitionCommand>();
        CreateMap<GetItemDefinitionRequest, GetItemDefinitionCommand>();
    }

    [UsedImplicitly]
    private sealed class FieldValueTypeConverter
        : ITypeConverter<ProtoValueType, FieldValueType>,
          ITypeConverter<FieldValueType, ProtoValueType>
    {
        public FieldValueType Convert(ProtoValueType source, FieldValueType destination, ResolutionContext context)
            => source switch
            {
                ProtoValueType.Boolean   => FieldValueType.Boolean,
                ProtoValueType.Integer   => FieldValueType.Integer,
                ProtoValueType.Double    => FieldValueType.Double,
                ProtoValueType.Decimal   => FieldValueType.Decimal,
                ProtoValueType.DateTime  => FieldValueType.DateTime,
                ProtoValueType.TimeSpan  => FieldValueType.TimeSpan,
                ProtoValueType.String    => FieldValueType.String,
                ProtoValueType.Reference => FieldValueType.Reference,
                _                        => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };

        public ProtoValueType Convert(FieldValueType source, ProtoValueType destination, ResolutionContext context)
            => source switch
            {
                FieldValueType.Boolean   => ProtoValueType.Boolean,
                FieldValueType.Integer   => ProtoValueType.Integer,
                FieldValueType.Double    => ProtoValueType.Double,
                FieldValueType.Decimal   => ProtoValueType.Decimal,
                FieldValueType.DateTime  => ProtoValueType.DateTime,
                FieldValueType.TimeSpan  => ProtoValueType.TimeSpan,
                FieldValueType.String    => ProtoValueType.String,
                FieldValueType.Reference => ProtoValueType.Reference,
                _                        => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
    }
}