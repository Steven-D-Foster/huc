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
    public class GoogleSheetsLoad : GoogleSheetsBase
    {
        protected override void CreateHelp(CommandHelpBuilder help)
        {
            base.CreateHelp(help);
            help.AddSummary("Loads a tab delimited data file into a Google Sheet");
            help.AddParameter("sheetName", "s", "The spreadsheet sheet name/tab to upload to (default first sheet)");
            help.AddValue("<tab delimited data file>");
        }

        protected override void Execute()
        {
            base.Execute();
            var sheetName = GetArgParameterOrConfig("sheetName", "s");

            var values = GetArgValues().TrimOrNull().WhereNotNull().ToList();
            var dataFileName = values.GetAtIndexOrDefault(0);
            log.Debug(nameof(dataFileName) + ": " + dataFileName);
            if (dataFileName == null) throw new ArgsException("dataFileName", "No dataFile specified");

            var table = ReadTableTab(dataFileName);

            using (var c = CreateConnection())
            {
                log.Debug("Clearing sheet");
                c.ClearSheet(sheetName);
                log.Info("Cleared sheet");

                log.Debug("Setting data");
                c.SetData(sheetName, table);
                log.Info("Data loaded");
            }
        }
    }
}