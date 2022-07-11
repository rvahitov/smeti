using Dapper;
using Npgsql;
using NpgsqlTypes;
using Smeti.Domain.Models.ItemModel;
using Smeti.Domain.Projections.Extensions;
using Smeti.Domain.Projections.Services;

namespace Smeti.Domain.Projections.Items;

public static class ItemsProjectionRepository
{
    public static void WriteEvents(IPostgresConnectionFactory connectionFactory, IEnumerable<IItemEvent> events)
    {
        using var connection = connectionFactory.Create();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        DoPrepareForProjectionImport(connection);
        using var importer = BeginProjectionImport(connection);
        foreach(var @event in events)
        {
            WriteEvent(importer, @event);
        }

        importer.Complete();
        importer.Close();
        FinishProjectionImport(connection);
        transaction.Commit();
        connection.Close();
    }

    private static void WriteEvent(NpgsqlBinaryImporter importer, IItemEvent @event)
    {
        importer.StartRow();
        switch(@event)
        {
            case ItemCreatedEvent(var id, var definitionId, var fields, _):
                importer.Write(id.Value);           //id
                importer.Write("insert_item");      //op_type
                importer.Write(definitionId.Value); //item_definition_id
                importer.WriteNull();               //field_name
                importer.WriteNull();               //value_type
                importer.WriteNull();               //boolean_value
                importer.WriteNull();               //integer_value
                importer.WriteNull();               //decimal_value
                importer.WriteNull();               //date_time_value
                importer.WriteNull();               //time_span_value
                importer.WriteNull();               //text_value
                importer.WriteNull();               //reference_value
                foreach(var field in fields)
                {
                    importer.StartRow();
                    importer.Write(id.Value);            //id
                    importer.Write("insert_item_field"); //op_type
                    importer.WriteNull();                //item_definition_id
                    WriteField(importer, field);
                }

                break;
            case FieldAddedEvent(var id, var field, _):
                importer.Write(id.Value);            //id
                importer.Write("insert_item_field"); //op_type
                importer.WriteNull();                //item_definition_id
                WriteField(importer, field);
                break;
            case FieldUpdatedEvent(var id, var field, _):
                importer.Write(id.Value);            //id
                importer.Write("update_item_field"); //op_type
                importer.WriteNull();                //item_definition_id
                WriteField(importer, field);
                break;
        }
    }

    private static void WriteField(NpgsqlBinaryImporter importer, IItemField field)
    {
        switch(field)
        {
            case BooleanField(var name, var value):
                importer.Write(name.Value);  //field_name
                importer.Write("Boolean");   //value_type
                importer.WriteOption(value); //boolean_value
                importer.WriteNull();        //integer_value
                importer.WriteNull();        //decimal_value
                importer.WriteNull();        //date_time_value
                importer.WriteNull();        //time_span_value
                importer.WriteNull();        //text_value
                importer.WriteNull();        //reference_value
                break;
            case IntegerField(var name, var value):
                importer.Write(name.Value);  //field_name
                importer.Write("Integer");   //value_type
                importer.WriteNull();        //boolean_value
                importer.WriteOption(value); //integer_value
                importer.WriteNull();        //decimal_value
                importer.WriteNull();        //date_time_value
                importer.WriteNull();        //time_span_value
                importer.WriteNull();        //text_value
                importer.WriteNull();        //reference_value
                break;
            case DecimalField(var name, var value):
                importer.Write(name.Value);  //field_name
                importer.Write("Decimal");   //value_type
                importer.WriteNull();        //boolean_value
                importer.WriteNull();        //integer_value
                importer.WriteOption(value); //decimal_value
                importer.WriteNull();        //date_time_value
                importer.WriteNull();        //time_span_value
                importer.WriteNull();        //text_value
                importer.WriteNull();        //reference_value
                break;
            case DateTimeField(var name, var value):
                importer.Write(name.Value);                            //field_name
                importer.Write("DateTime");                            //value_type
                importer.WriteNull();                                  //boolean_value
                importer.WriteNull();                                  //integer_value
                importer.WriteNull();                                  //decimal_value
                importer.WriteOption(value, NpgsqlDbType.TimestampTz); //date_time_value
                importer.WriteNull();                                  //time_span_value
                importer.WriteNull();                                  //text_value
                importer.WriteNull();                                  //reference_value
                break;
            case TimeSpanField(var name, var value):
                importer.Write(name.Value);                         //field_name
                importer.Write("TimeSpan");                         //value_type
                importer.WriteNull();                               //boolean_value
                importer.WriteNull();                               //integer_value
                importer.WriteNull();                               //decimal_value
                importer.WriteNull();                               //date_time_value
                importer.WriteOption(value, NpgsqlDbType.Interval); //time_span_value
                importer.WriteNull();                               //text_value
                importer.WriteNull();                               //reference_value
                break;
            case TextField(var name, var value):
                importer.Write(name.Value);  //field_name
                importer.Write("Text");      //value_type
                importer.WriteNull();        //boolean_value
                importer.WriteNull();        //integer_value
                importer.WriteNull();        //decimal_value
                importer.WriteNull();        //date_time_value
                importer.WriteNull();        //time_span_value
                importer.WriteOption(value); //text_value
                importer.WriteNull();        //reference_value
                break;
            case ReferenceField(var name, var value):
                importer.Write(name.Value);  //field_name
                importer.Write("Reference"); //value_type
                importer.WriteNull();        //boolean_value
                importer.WriteNull();        //integer_value
                importer.WriteNull();        //decimal_value
                importer.WriteNull();        //date_time_value
                importer.WriteNull();        //time_span_value
                importer.WriteNull();        //text_value
                importer.WriteOption(value); //reference_value
                break;
        }
    }

    private static NpgsqlBinaryImporter BeginProjectionImport(NpgsqlConnection connection) =>
        connection.BeginBinaryImport(@"
copy tmp_item (
               id,
               op_type,
               item_definition_id,
               field_name,
               value_type,
               boolean_value,
               integer_value,
               decimal_value,
               date_time_value,
               time_span_value,
               text_value,
               reference_value
    ) from stdin (format binary );
");

    private static void DoPrepareForProjectionImport(NpgsqlConnection connection)
    {
        const string prepareStatement = @"
create temp table tmp_item
(
    id                 varchar(250) not null,
    op_type            varchar(50)  not null,
    item_definition_id varchar(250) null,
    field_name         varchar(250) null,
    value_type         varchar(50)  null,
    boolean_value      bool         null,
    integer_value      bigint       null,
    decimal_value      decimal      null,
    date_time_value    timestamptz  null,
    time_span_value    interval     null,
    text_value         text         null,
    reference_value    varchar(250) null
) on commit drop;
";
        connection.Execute(prepareStatement);
    }

    private static void FinishProjectionImport(NpgsqlConnection connection)
    {
        const string finishStatement = @"
insert into item(id, item_definition_id)
select t.id, t.item_definition_id
from tmp_item t
where t.op_type = 'insert_item';

insert into item_field(item_id,
                       field_name,
                       value_type,
                       boolean_value,
                       integer_value,
                       decimal_value,
                       date_time_value,
                       time_span_value,
                       text_value,
                       reference_value)
select t.id,
       t.field_name,
       t.value_type,
       t.boolean_value,
       t.integer_value,
       t.decimal_value,
       t.date_time_value,
       t.time_span_value,
       t.text_value,
       t.reference_value
from tmp_item t
where op_type = 'insert_item_field';

update item_field i
set value_type      = t.value_type,
    boolean_value   = t.boolean_value,
    integer_value   = t.integer_value,
    decimal_value   = t.decimal_value,
    date_time_value = t.date_time_value,
    time_span_value = t.time_span_value,
    text_value      = t.text_value,
    reference_value = t.reference_value
from (select * from tmp_item) t
where i.item_id = t.id
  and i.field_name = t.field_name
  and t.op_type = 'update_item_field';
";
        connection.Execute(finishStatement);
    }
}