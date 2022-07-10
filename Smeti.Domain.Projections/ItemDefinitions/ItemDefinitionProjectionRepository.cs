using Dapper;
using Npgsql;
using NpgsqlTypes;
using Smeti.Domain.Models.ItemDefinitionModel;
using Smeti.Domain.Projections.Extensions;
using Smeti.Domain.Projections.Services;

namespace Smeti.Domain.Projections.ItemDefinitions;

internal static class ItemDefinitionProjectionRepository
{
    public static void WriteEvents(
        IPostgresConnectionFactory connectionFactory,
        IEnumerable<IItemDefinitionEvent> events
    )
    {
        using var connection = connectionFactory.Create();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        DoPrepareForProjectionImport(connection);
        using var importer = BeginProjectionImport(connection);

        foreach(var @event in events)
        {
            importer.StartRow();
            WriteEvent(importer, @event);
        }

        importer.Complete();
        importer.Close();
        FinishProjectionImport(connection);
        transaction.Commit();
        connection.Close();
    }

    private static void WriteEvent(NpgsqlBinaryImporter importer, IItemDefinitionEvent @event)
    {
        switch (@event)
        {
            case ItemDefinitionCreatedEvent(var id, var title, var definitions):
                importer.Write(id.Value);                 //id
                importer.Write("insert_item_definition"); //op_type
                importer.WriteOption(title);              //title
                importer.WriteNull();                     //field_name
                importer.WriteNull();                     //field_value_type
                importer.WriteNull();                     //is_required
                importer.WriteNull();                     //min_integer_value
                importer.WriteNull();                     //max_integer_value
                importer.WriteNull();                     //max_decimal_value
                importer.WriteNull();                     //max_decimal_value
                importer.WriteNull();                     //min_date_time_value
                importer.WriteNull();                     //max_date_time_value
                importer.WriteNull();                     //min_date_value
                importer.WriteNull();                     //max_date_value
                importer.WriteNull();                     //min_time_value
                importer.WriteNull();                     //max_time_value
                importer.WriteNull();                     //min_time_span_value
                importer.WriteNull();                     //max_time_span_value
                importer.WriteNull();                     //min_length
                importer.WriteNull();                     //max_length

                foreach(var definition in definitions)
                {
                    importer.StartRow();
                    importer.Write(id.Value);
                    importer.Write("insert_field_definition");
                    importer.WriteNull();
                    WriteFieldDefinition(importer, definition);
                }

                break;
            case FieldDefinitionAddedEvent(var id, var fieldDefinition):
                importer.Write(id.Value);                  //id
                importer.Write("insert_field_definition"); //op_type
                importer.WriteNull();                      //title
                WriteFieldDefinition(importer, fieldDefinition);
                break;
            case FieldDefinitionUpdatedEvent(var id, var fieldDefinition):
                importer.Write(id.Value);                  //id
                importer.Write("update_field_definition"); //op_type
                importer.WriteNull();                      //title
                WriteFieldDefinition(importer, fieldDefinition);
                break;
        }
    }

    private static void WriteFieldDefinition(NpgsqlBinaryImporter importer, IFieldDefinition fieldDefinition)
    {
        switch(fieldDefinition)
        {
            case BooleanFieldDefinition(var name, var isRequired):
                importer.Write(name.Value); //field_name
                importer.Write("Boolean");  //field_value_type
                importer.Write(isRequired); //is_required
                importer.WriteNull();       //min_integer_value
                importer.WriteNull();       //max_integer_value
                importer.WriteNull();       //max_decimal_value
                importer.WriteNull();       //max_decimal_value
                importer.WriteNull();       //min_date_time_value
                importer.WriteNull();       //max_date_time_value
                importer.WriteNull();       //min_date_value
                importer.WriteNull();       //max_date_value
                importer.WriteNull();       //min_time_value
                importer.WriteNull();       //max_time_value
                importer.WriteNull();       //min_time_span_value
                importer.WriteNull();       //max_time_span_value
                importer.WriteNull();       //min_length
                importer.WriteNull();       //max_length
                break;
            case IntegerFieldDefinition(var name, var isRequired, var min, var max):
                importer.Write(name.Value); //field_name
                importer.Write("Integer");  //field_value_type
                importer.Write(isRequired); //is_required
                importer.WriteOption(min);  //min_integer_value
                importer.WriteOption(max);  //max_integer_value
                importer.WriteNull();       //max_decimal_value
                importer.WriteNull();       //max_decimal_value
                importer.WriteNull();       //min_date_time_value
                importer.WriteNull();       //max_date_time_value
                importer.WriteNull();       //min_date_value
                importer.WriteNull();       //max_date_value
                importer.WriteNull();       //min_time_value
                importer.WriteNull();       //max_time_value
                importer.WriteNull();       //min_time_span_value
                importer.WriteNull();       //max_time_span_value
                importer.WriteNull();       //min_length
                importer.WriteNull();       //max_length
                break;
            case DecimalFieldDefinition(var name, var isRequired, var min, var max):
                importer.Write(name.Value); //field_name
                importer.Write("Decimal");  //field_value_type
                importer.Write(isRequired); //is_required
                importer.WriteNull();       //min_integer_value
                importer.WriteNull();       //max_integer_value
                importer.WriteOption(min);  //min_decimal_value
                importer.WriteOption(max);  //max_decimal_value
                importer.WriteNull();       //min_date_time_value
                importer.WriteNull();       //max_date_time_value
                importer.WriteNull();       //min_date_value
                importer.WriteNull();       //max_date_value
                importer.WriteNull();       //min_time_value
                importer.WriteNull();       //max_time_value
                importer.WriteNull();       //min_time_span_value
                importer.WriteNull();       //max_time_span_value
                importer.WriteNull();       //min_length
                importer.WriteNull();       //max_length
                break;
            case DateTimeFieldDefinition(var name, var isRequired, var min, var max):
                importer.Write(name.Value); //field_name
                importer.Write("DateTime"); //field_value_type
                importer.Write(isRequired); //is_required
                importer.WriteNull();       //min_integer_value
                importer.WriteNull();       //max_integer_value
                importer.WriteNull();       //max_decimal_value
                importer.WriteNull();       //max_decimal_value
                importer.WriteOption(min);  //min_date_time_value
                importer.WriteOption(max);  //max_date_time_value
                importer.WriteNull();       //min_date_value
                importer.WriteNull();       //max_date_value
                importer.WriteNull();       //min_time_value
                importer.WriteNull();       //max_time_value
                importer.WriteNull();       //min_time_span_value
                importer.WriteNull();       //max_time_span_value
                importer.WriteNull();       //min_length
                importer.WriteNull();       //max_length
                break;
            case DateFieldDefinition(var name, var isRequired, var min, var max):
                importer.Write(name.Value);                   //field_name
                importer.Write("Date");                       //field_value_type
                importer.Write(isRequired);                   //is_required
                importer.WriteNull();                         //min_integer_value
                importer.WriteNull();                         //max_integer_value
                importer.WriteNull();                         //max_decimal_value
                importer.WriteNull();                         //max_decimal_value
                importer.WriteNull();                         //min_date_time_value
                importer.WriteNull();                         //max_date_time_value
                importer.WriteOption(min, NpgsqlDbType.Date); //min_date_value
                importer.WriteOption(max, NpgsqlDbType.Date); //max_date_value
                importer.WriteNull();                         //min_time_value
                importer.WriteNull();                         //max_time_value
                importer.WriteNull();                         //min_time_span_value
                importer.WriteNull();                         //max_time_span_value
                importer.WriteNull();                         //min_length
                importer.WriteNull();                         //max_length
                break;
            case TimeFieldDefinition(var name, var isRequired, var min, var max):
                importer.Write(name.Value);                   //field_name
                importer.Write("Time");                       //field_value_type
                importer.Write(isRequired);                   //is_required
                importer.WriteNull();                         //min_integer_value
                importer.WriteNull();                         //max_integer_value
                importer.WriteNull();                         //max_decimal_value
                importer.WriteNull();                         //max_decimal_value
                importer.WriteNull();                         //min_date_time_value
                importer.WriteNull();                         //max_date_time_value
                importer.WriteNull();                         //min_date_value
                importer.WriteNull();                         //max_date_value
                importer.WriteOption(min, NpgsqlDbType.Time); //min_time_value
                importer.WriteOption(max, NpgsqlDbType.Time); //max_time_value
                importer.WriteNull();                         //min_time_span_value
                importer.WriteNull();                         //max_time_span_value
                importer.WriteNull();                         //min_length
                importer.WriteNull();                         //max_length
                break;
            case TimeSpanFieldDefinition(var name, var isRequired, var min, var max):
                importer.Write(name.Value);                       //field_name
                importer.Write("TimeSpan");                       //field_value_type
                importer.Write(isRequired);                       //is_required
                importer.WriteNull();                             //min_integer_value
                importer.WriteNull();                             //max_integer_value
                importer.WriteNull();                             //max_decimal_value
                importer.WriteNull();                             //max_decimal_value
                importer.WriteNull();                             //min_date_time_value
                importer.WriteNull();                             //max_date_time_value
                importer.WriteNull();                             //min_date_value
                importer.WriteNull();                             //max_date_value
                importer.WriteNull();                             //min_time_value
                importer.WriteNull();                             //max_time_value
                importer.WriteOption(min, NpgsqlDbType.Interval); //min_time_span_value
                importer.WriteOption(max, NpgsqlDbType.Interval); //max_time_span_value
                importer.WriteNull();                             //min_length
                importer.WriteNull();                             //max_length
                break;
            case TextFieldDefinition(var name, var isRequired, var min, var max):
                importer.Write(name.Value); //field_name
                importer.Write("Text");     //field_value_type
                importer.Write(isRequired); //is_required
                importer.WriteNull();       //min_integer_value
                importer.WriteNull();       //max_integer_value
                importer.WriteNull();       //max_decimal_value
                importer.WriteNull();       //max_decimal_value
                importer.WriteNull();       //min_date_time_value
                importer.WriteNull();       //max_date_time_value
                importer.WriteNull();       //min_date_value
                importer.WriteNull();       //max_date_value
                importer.WriteNull();       //min_time_value
                importer.WriteNull();       //max_time_value
                importer.WriteNull();       //min_time_span_value
                importer.WriteNull();       //max_time_span_value
                importer.WriteOption(min);  //min_length
                importer.WriteOption(max);  //max_length
                break;
            case ReferenceFieldDefinition(var name, var isRequired):
                importer.Write(name.Value);  //field_name
                importer.Write("Reference"); //field_value_type
                importer.Write(isRequired);  //is_required
                importer.WriteNull();        //min_integer_value
                importer.WriteNull();        //max_integer_value
                importer.WriteNull();        //max_decimal_value
                importer.WriteNull();        //max_decimal_value
                importer.WriteNull();        //min_date_time_value
                importer.WriteNull();        //max_date_time_value
                importer.WriteNull();        //min_date_value
                importer.WriteNull();        //max_date_value
                importer.WriteNull();        //min_time_value
                importer.WriteNull();        //max_time_value
                importer.WriteNull();        //min_time_span_value
                importer.WriteNull();        //max_time_span_value
                importer.WriteNull();        //min_length
                importer.WriteNull();        //max_length
                break;
        }
    }

    private static NpgsqlBinaryImporter BeginProjectionImport(NpgsqlConnection connection) =>
        connection.BeginBinaryImport(@"
copy tmp_item_definition (
                          id,
                          op_type,
                          title,
                          field_name,
                          field_value_type,
                          is_required,
                          min_integer_value,
                          max_integer_value,
                          min_decimal_value,
                          max_decimal_value,
                          min_date_time_value,
                          max_date_time_value,
                          min_date_value,
                          max_date_value,
                          min_time_value,
                          max_time_value,
                          min_time_span_value,
                          max_time_span_value,
                          min_length,
                          max_length
) from stdin (format binary );
");

    private static void DoPrepareForProjectionImport(NpgsqlConnection connection)
    {
        const string prepareStatement = @"
create temp table tmp_item_definition
(
    id                  varchar(250)  not null,
    op_type             varchar(50)   not null,
    title               varchar(1024) null,
    field_name          varchar(250)  null,
    field_value_type    varchar(50)   null,
    is_required         bool          null,
    min_integer_value   bigint        null,
    max_integer_value   bigint        null,
    min_decimal_value   decimal       null,
    max_decimal_value   decimal       null,
    min_date_time_value timestamptz   null,
    max_date_time_value timestamptz   null,
    min_date_value      date          null,
    max_date_value      date          null,
    min_time_value      time          null,
    max_time_value      time          null,
    min_time_span_value interval      null,
    max_time_span_value interval      null,
    min_length          int           null,
    max_length          int           null
) on commit drop;
";
        connection.Execute(prepareStatement);
    }

    private static void FinishProjectionImport(NpgsqlConnection connection)
    {
        connection.Execute(@"
insert into item_definition(id, title)
select t.id, t.title
from tmp_item_definition t
where t.op_type = 'insert_item_definition';

insert into field_definition (item_definition_id,
                              field_name,
                              field_value_type,
                              is_required,
                              min_integer_value,
                              max_integer_value,
                              min_decimal_value,
                              max_decimal_value,
                              min_date_time_value,
                              max_date_time_value,
                              min_date_value,
                              max_date_value,
                              min_time_value,
                              max_time_value,
                              min_time_span_value,
                              max_time_span_value,
                              min_length,
                              max_length)
select t.id,
       t.field_name,
       t.field_value_type,
       t.is_required,
       t.min_integer_value,
       t.max_integer_value,
       t.min_decimal_value,
       t.max_decimal_value,
       t.min_date_time_value,
       t.max_date_time_value,
       t.min_date_value,
       t.max_date_value,
       t.min_time_value,
       t.max_time_value,
       t.min_time_span_value,
       t.max_time_span_value,
       t.min_length,
       t.max_length
from tmp_item_definition t
where t.op_type = 'insert_field_definition';

update field_definition fd
set field_value_type    = t.field_value_type,
    is_required         = t.is_required,
    min_integer_value   = t.min_integer_value,
    max_integer_value   = t.min_integer_value,
    min_decimal_value   = t.min_decimal_value,
    max_decimal_value   = t.max_decimal_value,
    min_date_time_value = t.min_date_value,
    max_date_time_value = t.max_date_time_value,
    min_date_value      = t.min_date_value,
    max_date_value      = t.max_date_value,
    min_time_value      = t.min_time_value,
    max_time_value      = t.max_time_value,
    min_time_span_value = t.min_time_span_value,
    max_time_span_value = t.max_time_span_value,
    min_length          = t.min_length,
    max_length          = t.max_length
from (select *
      from tmp_item_definition) t
where fd.item_definition_id = t.id
  and t.op_type = 'update_field_definition';
");
    }
}