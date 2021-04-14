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

using System.Linq;

namespace HavokMultimedia.Utilities.Console.Commands
{
    public class GoogleSheetsQuery : GoogleSheetsBase
    {
        protected override void CreateHelp(CommandHelpBuilder help)
        {
            base.CreateHelp(help);
            help.AddSummary("Query a Google Sheet for data and generate a tab delimited file of the data");
            help.AddParameter("sheetName", "s", "The spreadsheet sheet name/tab to query (default first sheet)");
            help.AddParameter("range", "r", "The range to query from (A1:ZZ)");
            help.AddValue("<tab delimited output file name>");
        }

        protected override void Execute()
        {
            base.Execute();
            var sheetName = GetArgParameterOrConfig("sheetName", "s");
            var range = GetArgParameterOrConfig("range", "r", "A1:ZZ");

            var values = GetArgValues().TrimOrNull().WhereNotNull().ToList();
            var outputFile = values.GetAtIndexOrDefault(0);
            log.Debug(nameof(outputFile) + ": " + outputFile);
            if (outputFile == null) throw new ArgsException(nameof(outputFile), $"No {nameof(outputFile)} specified");

            using (var c = CreateConnection())
            {
                log.Debug("Querying sheet");
                var items = c.Query(sheetName, range: range);
                var table = HavokMultimedia.Utilities.Table.Create(items, true);
                WriteTableTab(outputFile, table);
                log.Info("Sheet with " + items.Count + " rows written to " + outputFile);
            }
        }
    }
}