namespace AndrealClient.Core;

internal class CreateTableSqlAttribute : Attribute
{
    internal CreateTableSqlAttribute(string sql) { Sql = sql; }
    internal string Sql { get; }
}
