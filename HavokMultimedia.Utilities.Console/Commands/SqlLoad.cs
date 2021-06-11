﻿/*
Copyright (c) 2021 Steven Foster (steven.d.foster@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace HavokMultimedia.Utilities.Console.Commands
{
    public static class SqlLoadExtensions
    {
        private static readonly IReadOnlyDictionary<DbType, SqlDbType> DBTYPE_SQLDBTYPE_MAP = new Dictionary<DbType, SqlDbType>
            {
                { DbType.AnsiString, SqlDbType.NVarChar },
                { DbType.Binary, SqlDbType.Binary },
                { DbType.Byte, SqlDbType.SmallInt },
                { DbType.Boolean, SqlDbType.Bit },
                { DbType.Currency, SqlDbType.Money },
                { DbType.Date, SqlDbType.Date },
                { DbType.DateTime, SqlDbType.DateTime },
                { DbType.Decimal, SqlDbType.Decimal },
                { DbType.Double, SqlDbType.Float },
                { DbType.Guid, SqlDbType.UniqueIdentifier },
                { DbType.Int16, SqlDbType.SmallInt },
                { DbType.Int32, SqlDbType.Int },
                { DbType.Int64, SqlDbType.BigInt },
                { DbType.Object, SqlDbType.Variant },
                { DbType.SByte, SqlDbType.TinyInt },
                { DbType.Single, SqlDbType.Real },
                { DbType.String, SqlDbType.NVarChar },
                { DbType.Time, SqlDbType.Time },
                { DbType.UInt16, SqlDbType.Int },
                { DbType.UInt32, SqlDbType.BigInt },
                { DbType.UInt64, SqlDbType.BigInt },
                { DbType.VarNumeric, SqlDbType.Decimal },
                { DbType.AnsiStringFixedLength, SqlDbType.Char },
                { DbType.StringFixedLength, SqlDbType.NChar },
                { DbType.Xml, SqlDbType.Xml },
                { DbType.DateTime2, SqlDbType.DateTime2 },
                { DbType.DateTimeOffset, SqlDbType.DateTimeOffset },
        }.AsReadOnly();

        private static readonly IReadOnlyDictionary<SqlDbType, DbType> SQLDBTYPE_DBTYPE_MAP = new Dictionary<SqlDbType, DbType>
            {
                { SqlDbType.BigInt, DbType.Int64 },
                { SqlDbType.Binary, DbType.Binary },
                { SqlDbType.Bit, DbType.Boolean },
                { SqlDbType.Char, DbType.StringFixedLength },
                { SqlDbType.DateTime, DbType.DateTime },
                { SqlDbType.Decimal, DbType.Decimal },
                { SqlDbType.Float, DbType.Double },
                { SqlDbType.Image, DbType.Binary },
                { SqlDbType.Int, DbType.Int32 },
                { SqlDbType.Money, DbType.Currency },
                { SqlDbType.NChar, DbType.StringFixedLength },
                { SqlDbType.NText, DbType.String },
                { SqlDbType.NVarChar, DbType.String },
                { SqlDbType.Real, DbType.Single },
                { SqlDbType.UniqueIdentifier, DbType.Guid },
                { SqlDbType.SmallDateTime, DbType.DateTime },
                { SqlDbType.SmallInt, DbType.Int16 },
                { SqlDbType.SmallMoney, DbType.Currency },
                { SqlDbType.Text, DbType.String },
                { SqlDbType.Timestamp, DbType.Binary },
                { SqlDbType.TinyInt, DbType.Byte },
                { SqlDbType.VarBinary, DbType.Binary },
                { SqlDbType.VarChar, DbType.String },
                { SqlDbType.Variant, DbType.Object },
                { SqlDbType.Xml, DbType.Xml },
                { SqlDbType.Udt, DbType.Object },
                { SqlDbType.Structured, DbType.Object },
                { SqlDbType.Date, DbType.Date },
                { SqlDbType.Time, DbType.Time },
                { SqlDbType.DateTime2, DbType.DateTime2 },
                { SqlDbType.DateTimeOffset, DbType.DateTimeOffset },
        }.AsReadOnly();

        private static readonly IReadOnlyDictionary<SqlDbType, Type> SQLDBTYPE_TYPE_MAP = new Dictionary<SqlDbType, Type> {
            { SqlDbType.BigInt, typeof(long) },
            { SqlDbType.Binary, typeof(byte[]) },
            { SqlDbType.Bit, typeof(bool) },
            { SqlDbType.Char, typeof(char[]) },
            { SqlDbType.DateTime, typeof(DateTime) },
            { SqlDbType.Decimal, typeof(decimal) },
            { SqlDbType.Float, typeof(double) },
            { SqlDbType.Image, typeof(byte[]) },
            { SqlDbType.Int, typeof(int) },
            { SqlDbType.Money, typeof(decimal) },
            { SqlDbType.NChar, typeof(char[]) },
            { SqlDbType.NText, typeof(string) },
            { SqlDbType.NVarChar, typeof(string) },
            { SqlDbType.Real, typeof(float) },
            { SqlDbType.UniqueIdentifier, typeof(Guid) },
            { SqlDbType.SmallDateTime, typeof(DateTime) },
            { SqlDbType.SmallInt, typeof(short) },
            { SqlDbType.SmallMoney, typeof(decimal) },
            { SqlDbType.Text, typeof(string) },
            { SqlDbType.Timestamp, typeof(byte[]) },
            { SqlDbType.TinyInt, typeof(byte) },
            { SqlDbType.VarBinary, typeof(byte[]) },
            { SqlDbType.VarChar, typeof(string) },
            { SqlDbType.Variant, typeof(object) },
            { SqlDbType.Xml, typeof(string) },
            { SqlDbType.Udt, typeof(object) },
            { SqlDbType.Structured, typeof(object) },
            { SqlDbType.Date, typeof(DateTime) },
            { SqlDbType.Time, typeof(DateTime) },
            { SqlDbType.DateTime2, typeof(DateTime) },
            { SqlDbType.DateTimeOffset, typeof(DateTimeOffset) },
        }.AsReadOnly();

        private static readonly IReadOnlyDictionary<Type, SqlDbType> TYPE_SQLDBTYPE_MAP = new Dictionary<Type, SqlDbType>
            {
                { typeof(bool), SqlDbType.Bit },
                { typeof(bool?), SqlDbType.Bit },

                { typeof(byte), SqlDbType.TinyInt },
                { typeof(byte?), SqlDbType.TinyInt },
                { typeof(sbyte), SqlDbType.SmallInt },
                { typeof(sbyte?), SqlDbType.SmallInt },

                { typeof(short), SqlDbType.SmallInt },
                { typeof(short?), SqlDbType.SmallInt },
                { typeof(ushort), SqlDbType.Int },
                { typeof(ushort?), SqlDbType.Int },

                { typeof(char), SqlDbType.NChar },
                { typeof(char?), SqlDbType.NChar },
                { typeof(char[]), SqlDbType.NChar },

                { typeof(int), SqlDbType.Int },
                { typeof(int?), SqlDbType.Int },
                { typeof(uint), SqlDbType.BigInt },
                { typeof(uint?), SqlDbType.BigInt },

                { typeof(long), SqlDbType.BigInt },
                { typeof(long?), SqlDbType.BigInt },
                { typeof(ulong), SqlDbType.BigInt }, // TODO: unsafe
                { typeof(ulong?), SqlDbType.BigInt }, // TODO: unsafe

                { typeof(float), SqlDbType.Real },
                { typeof(float?), SqlDbType.Real },
                { typeof(double), SqlDbType.Float },
                { typeof(double?), SqlDbType.Float },
                { typeof(decimal), SqlDbType.Decimal },
                { typeof(decimal?), SqlDbType.Decimal },

                { typeof(byte[]), SqlDbType.Binary },

                { typeof(Guid), SqlDbType.UniqueIdentifier },
                { typeof(Guid?), SqlDbType.UniqueIdentifier },

                { typeof(string), SqlDbType.NVarChar },

                { typeof(System.Net.IPAddress), SqlDbType.NVarChar },
                { typeof(Uri), SqlDbType.NVarChar },

                { typeof(System.Numerics.BigInteger), SqlDbType.NVarChar }, // TODO: is this correct
                { typeof(System.Numerics.BigInteger?), SqlDbType.NVarChar }, // TODO: is this correct

                { typeof(DateTime), SqlDbType.DateTime },
                { typeof(DateTime?), SqlDbType.DateTime },
                { typeof(DateTimeOffset), SqlDbType.DateTimeOffset },
                { typeof(DateTimeOffset?), SqlDbType.DateTimeOffset },
            }.AsReadOnly();


        public static DbType GetDbType(this SqlDbType sqlDbType) => SQLDBTYPE_DBTYPE_MAP.TryGetValue(sqlDbType, out var dbType) ? dbType : DbType.String;

        public static SqlDbType GetSqlDbType(this DbType dbType) => DBTYPE_SQLDBTYPE_MAP.TryGetValue(dbType, out var sqlDbType) ? sqlDbType : SqlDbType.NVarChar;

        public static Type GetDotNetType(this SqlDbType sqlDbType) => SQLDBTYPE_TYPE_MAP.TryGetValue(sqlDbType, out var type) ? type : typeof(string);

        public static SqlDbType GetSqlDbType(this Type type) => TYPE_SQLDBTYPE_MAP.TryGetValue(type, out var sqlDbType) ? sqlDbType : SqlDbType.NVarChar;
    }

    public class SqlLoad : SqlBase
    {
        protected override void CreateHelp(CommandHelpBuilder help)
        {
            base.CreateHelp(help);
            help.AddSummary("Loads a tab delimited data file into a SQL server table");
            help.AddDetail("It is expected that the table being imported is in Tab delimited format, and that there is a header row. If there is no header row set the -noHeader option");
            help.AddParameter(nameof(drop), "dp", "Drop existing table if it exists (false)");
            help.AddParameter(nameof(detectColumnTypes), "dct", "Attempts to detect column types and lengths (false)");
            help.AddParameter(nameof(errorOnNonexistentColumns), "ec", "When inserting to an existing table, whether to error if columns in the data file don't have cooresponding columns in the existing SQL table (false)");
            help.AddParameter(nameof(batchSize), "bs", "Number of insert commands to batch. Set to zero if having issues (1000)");
            help.AddParameter(nameof(rowNumberColumnName), "rncn", "If provided, an extra column is inserted with the line number, -1 because excluding header");
            help.AddParameter(nameof(currentUtcDateTimeColumnName), "ctcn", "If provided, an extra column containing the UTC datetime is inserted");
            help.AddParameter(nameof(noHeader), "nh", "Set if your importing tab delimited file does not have a header row (false)");
            help.AddParameter(nameof(database), "d", "Database name to load table");
            help.AddParameter(nameof(schema), "s", "Schema to load table");
            help.AddParameter(nameof(table), "t", "Table name");
            help.AddValue("<tab delimited file to load>");
            help.AddExample("-c=`Server=192.168.1.5;Database=NorthWind;User Id=testuser;Password=testpass;` -d=NorthWind -s=dbo -t=TempOrders Orders.txt");
            help.AddExample("-c=`Server=192.168.1.5;Database=NorthWind;User Id=testuser;Password=testpass;` -drop -rowNumberColumnName=RowNumber -currentUtcDateTimeColumnName=UploadTime -d=NorthWind -s=dbo -t=TempOrders Orders.txt");
        }

        private bool drop;
        private bool detectColumnTypes;
        private bool errorOnNonexistentColumns;
        private int batchSize;
        private string rowNumberColumnName;
        private string currentUtcDateTimeColumnName;
        private bool noHeader;
        private string database;
        private string schema;
        private string table;

        protected override void ExecuteInternal()
        {
            base.ExecuteInternal();

            drop = GetArgParameterOrConfigBool(nameof(drop), "dp", false);
            detectColumnTypes = GetArgParameterOrConfigBool(nameof(detectColumnTypes), "dct", false);
            errorOnNonexistentColumns = GetArgParameterOrConfigBool(nameof(errorOnNonexistentColumns), "ec", false);
            batchSize = GetArgParameterOrConfigInt(nameof(batchSize), "bs", 1000);
            rowNumberColumnName = GetArgParameterOrConfig(nameof(rowNumberColumnName), "rncn").TrimOrNull();
            currentUtcDateTimeColumnName = GetArgParameterOrConfig(nameof(currentUtcDateTimeColumnName), "ctcn").TrimOrNull();
            noHeader = GetArgParameterOrConfigBool(nameof(noHeader), "nh", false);
            database = GetArgParameterOrConfigRequired(nameof(database), "d").TrimOrNull();
            schema = GetArgParameterOrConfig(nameof(schema), "s").TrimOrNull();
            table = GetArgParameterOrConfigRequired(nameof(table), "t").TrimOrNull();

            var inputFile = GetArgValueTrimmed(0);
            inputFile.CheckValueNotNull(nameof(inputFile), log);
            inputFile = ParseInputFiles(inputFile.Yield()).FirstOrDefault();
            CheckFileExists(inputFile);

            var t = ReadTableTab(inputFile, headerRow: !noHeader);
            if (t.Columns.IsEmpty())
            {
                throw new Exception("No columns in import file");
            }

            var c = GetSqlHelper();
            var databaseSchemaTable = c.Escape(database);
            if (schema != null) databaseSchemaTable = databaseSchemaTable + "." + c.Escape(schema);
            databaseSchemaTable = databaseSchemaTable + "." + c.Escape(table);

            log.Debug(nameof(databaseSchemaTable), databaseSchemaTable);

            var tableExists = c.GetTableExists(database, schema, table);
            if (tableExists && drop)
            {
                log.Info($"Dropping existing table: {databaseSchemaTable}");
                c.DropTable(database, schema, table);
            }

            tableExists = c.GetTableExists(database, schema, table);
            if (!tableExists)
            {
                var sql = new StringBuilder();

                sql.Append($"CREATE TABLE {databaseSchemaTable} (");
                if (rowNumberColumnName != null) sql.Append(c.Escape(rowNumberColumnName) + " INTEGER NULL,");
                if (currentUtcDateTimeColumnName != null) sql.Append(c.Escape(currentUtcDateTimeColumnName) + " DATETIME NULL,");
                for (var i = 0; i < t.Columns.Count; i++)
                {
                    var column = t.Columns[i];
                    if (i > 0) sql.Append(",");
                    sql.Append(c.Escape(column.Name));
                    sql.Append(" ");
                    if (detectColumnTypes)
                    {
                        if (serverType.NotIn(SqlServerType.MSSQL)) throw new NotImplementedException($"detectColumnTypes for SqlServerType {serverType} has not been implemented yet");

                        // MSSQL
                        var sqlDbType = column.Type.GetSqlDbType();
                        log.Debug("Column[" + column.Index + ":" + column.Name + "] " + column.Type.NameFormatted() + "  -->  " + sqlDbType);
                        sql.Append(sqlDbType);
                        if (sqlDbType.In(SqlDbType.NVarChar, SqlDbType.VarChar)) sql.Append("(" + (column.MaxLength > 4000 ? "MAX" : column.MaxLength) + ")");
                        else if (sqlDbType.In(SqlDbType.NChar, SqlDbType.Char)) sql.Append("(" + column.MaxLength + ")");
                        else if (sqlDbType.In(SqlDbType.Decimal)) sql.Append("(" + (37 - column.NumberOfDecimalDigits) + "," + column.NumberOfDecimalDigits + ")");



                    }
                    else
                    {
                        if (serverType == SqlServerType.MSSQL) sql.Append("NVARCHAR(MAX)");
                        else if (serverType == SqlServerType.MySQL) sql.Append("LONGTEXT");
                        else throw new NotImplementedException($"SqlServerType {serverType} has not been implemented yet");
                    }
                    sql.Append(" ");
                }
                sql.Append(");");
                log.Debug("Executing Create Table...");
                log.Debug(sql.ToString());
                c.ExecuteNonQuery(sql.ToString());
            }

            var sqlColumns = c.GetColumns(database, schema, table)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (rowNumberColumnName != null)
            {
                int i = 1;
                t = t.AddColumn(rowNumberColumnName, row => (i++).ToString(), newColumnIndex: 0);
            }

            if (currentUtcDateTimeColumnName != null)
            {
                var now = DateTime.UtcNow;
                t = t.AddColumn(currentUtcDateTimeColumnName, row => now.ToString("yyyy-MM-dd HH:mm:ss.fff"), newColumnIndex: 1);
            }

            var columnsToInsert = new List<TableColumn>();

            foreach (var tablecolumn in t.Columns)
            {
                if (sqlColumns.Contains(tablecolumn.Name)) // Found column in table
                {
                    columnsToInsert.Add(tablecolumn);
                }
                else if (!errorOnNonexistentColumns) // Didn't find column, but that is OK we'll just not import it
                {
                    log.Info($"Ignoring column {tablecolumn.Name} because it does not exist as a column in table {table}");
                }
                else // Didn't find column, and it is required, so we should fail here
                {
                    throw new Exception($"DataFile contains column {tablecolumn.Name} but existing SQL table {table} does not contain this column.");
                }
            }

            /*
            var dataToInsert = new List<DatabaseCrudParameter[]>();
            foreach (var row in t)
            {
                foreach (var column in columnsToInsert)
                {
                    var dcp = new DatabaseCrudParameter(column.Name, row[column], System.Data.DbType.String);
                }
                dataToInsert.Add(columnsToInsert.Select(o => new DatabaseCrudParameter(o.Name, row[o], System.Data.DbType.String)).ToArray());
            }
            */

            var columnsToRemove = t.Columns.Select(o => o.Name).Except(columnsToInsert.Select(o => o.Name), StringComparer.OrdinalIgnoreCase);
            t = t.RemoveColumns(columnsToRemove.ToArray());

            log.Info($"Writing {t.Count} rows to database in columns " + string.Join(", ", columnsToInsert.Select(o => o.Name)));
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //foreach (var row in dataToInsert) c.Insert(databaseSchemaTable, row);
            c.Insert(database, schema, table, t);

            stopwatch.Stop();
            var stopwatchtime = stopwatch.Elapsed.TotalSeconds.ToString(MidpointRounding.AwayFromZero, 3);
            log.Info($"Completed writing {t.Count} rows to database in {stopwatchtime} seconds");


        }






    }
}
