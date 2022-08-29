using AutoMapper;
using JetBrains.Annotations;
using Smeti.Domain.Common;
using Smeti.Domain.Models.ItemModel;
using Smeti.Services.Item.Extensions;
using Smeti.Services.Item.Proto;

namespace Smeti.Services.Item.Mapping;

using Item = Smeti.Domain.Models.ItemModel.Item;
using ProtoItem = Smeti.Services.Item.Proto.Item;

[UsedImplicitly]
public sealed class ItemProfile : Profile
{
    public ItemProfile()
    {
        CreateMap<Field, IField>().ConvertUsing<FieldConverter>();
        CreateMap<IField, Field>().ConvertUsing<FieldConverter>();
        CreateMap<ProtoItem, Item>();
        CreateMap<Item, ProtoItem>().ConvertUsing<ItemConverter>();
    }

    [UsedImplicitly]
    private sealed class FieldConverter
        : ITypeConverter<Field, IField>,
          ITypeConverter<IField, Field>
    {
        public IField Convert(Field source, IField destination, ResolutionContext context)
        {
            var fieldName = new FieldName(source.FieldName);
            return source.ValueCase switch
            {
                Field.ValueOneofCase.Boolean   => new BooleanField(fieldName, source.GetBooleanValue()),
                Field.ValueOneofCase.Integer   => new IntegerField(fieldName, source.GetIntegerValue()),
                Field.ValueOneofCase.Double    => new DoubleField(fieldName, source.GetDoubleValue()),
                Field.ValueOneofCase.Decimal   => new DecimalField(fieldName, source.GetDecimalValue()),
                Field.ValueOneofCase.DateTime  => new DateTimeField(fieldName, source.GetDateTimeValue()),
                Field.ValueOneofCase.TimeSpan  => new TimeSpanField(fieldName, source.GetTimeSpanValue()),
                Field.ValueOneofCase.String    => new StringField(fieldName, source.GetStringValue()),
                Field.ValueOneofCase.Reference => new ReferenceField(fieldName, source.GetReferenceValue()),
                _                              => throw new NotImplementedException("Unknown field")
            };
        }

        public Field Convert(IField source, Field destination, ResolutionContext context) => source switch
        {
            BooleanField((var fieldName) _, var value)   => Field.Create(fieldName, value),
            IntegerField((var fieldName) _, var value)   => Field.Create(fieldName, value),
            DoubleField((var fieldName) _, var value)    => Field.Create(fieldName, value),
            DecimalField((var fieldName) _, var value)   => Field.Create(fieldName, value),
            DateTimeField((var fieldName) _, var value)  => Field.Create(fieldName, value),
            TimeSpanField((var fieldName) _, var value)  => Field.Create(fieldName, value),
            StringField((var fieldName) _, var value)    => Field.Create(fieldName, value),
            ReferenceField((var fieldName) _, var value) => Field.Create(fieldName, value),
            _                                            => throw new NotImplementedException("Unknown field")
        };
    }

    [UsedImplicitly]
    private sealed class ItemConverter : ITypeConverter<Item, ProtoItem>
    {
        public ProtoItem Convert(Item source, ProtoItem destination, ResolutionContext context)
        {
            var result = new ProtoItem
            {
                Id = context.Mapper.Map<string>(source.Id),
                ItemDefinitionId = context.Mapper.Map<string>(source.ItemDefinitionId)
            };
            result.Fields.AddRange(source.Fields.Select(f => context.Mapper.Map<Field>(f)));
            return result;
        }
    }
}