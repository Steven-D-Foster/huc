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

namespace HavokMultimedia.Utilities
{
    public class SqlMySQL : Sql
    {
        public override IEnumerable<string> GetDatabases() => ExecuteQueryToList("SELECT schema_name FROM information_schema.schemata;");
        public override IEnumerable<string> GetTables(string database, string schema) => ExecuteQueryToList($"SELECT TABLE_NAME FROM information_schema.tables WHERE TABLE_SCHEMA='{database}';");
        public override void DropTable(string database, string schema, string table) => ExecuteNonQuery($"DROP TABLE {Escape(database)}.{Escape(table)};");
        public override IEnumerable<string> GetSchemas(string database) => new List<string>();
        public override IEnumerable<string> GetColumns(string database, string schema, string table) => ExecuteQueryToList($"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='{database}' AND TABLE_NAME = '{table}' ORDER BY ORDINAL_POSITION;");

        public static readonly Func<string, string> ESCAPE_MYSQL = (o =>
        {
            if (o == null) return o;
            if (!o.StartsWith("`")) o = "`" + o;
            if (!o.EndsWith("`")) o = o + "`";
            return o;
        });

        public SqlMySQL(Func<IDbConnection> connectionFactory) : base(connectionFactory)
        {
            EscapeObject = ESCAPE_MYSQL;
        }


    }
}
