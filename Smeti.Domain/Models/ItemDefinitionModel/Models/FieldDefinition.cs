using Smeti.Domain.Common;

namespace Smeti.Domain.Models.ItemDefinitionModel;

public sealed record FieldDefinition(FieldName FieldName, FieldValueType ValueType);