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
using System.IO;
using System.Linq;
using System.Text;

namespace HavokMultimedia.Utilities.Console
{
    public interface ICommand
    {
        string Name { get; }
        void Execute(string[] args);
        string HelpSummary { get; }
        string HelpDetails { get; }
        bool IsHidden { get; }
    }

    public sealed class HideCommandAttribute : Attribute { }

    public abstract class Command : ICommand
    {
        private Args args;
        private ConfigFile config;
        protected readonly ILogger log;

        public bool IsHidden => GetType().GetCustomAttributes(true).Where(o => o is HideCommandAttribute).Any();
        public CommandHelpBuilder Help { get; }
        public string Name => Help.Name;
        public string HelpSummary => Name.PadRight(Program.CommandObjects.Select(o => o.Name).MaxLength() + 2) + Help.Summary;
        public string HelpDetails
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine(Name);
                sb.AppendLine(Help.Summary);
                foreach (var s in Help.Details) sb.AppendLine(s);

                if (!Help.Parameters.IsEmpty()) { sb.AppendLine(); sb.AppendLine("Parameters:"); }
                var padWidth = 0;
                foreach (var s in Help.Parameters)
                {
                    var ss = "-" + s.p1;
                    if (s.p2 != null) ss += ", -" + s.p2;
                    var len = ss.Length;
                    if (len > padWidth) padWidth = len;
                }
                padWidth += 3;

                foreach (var s in Help.Parameters)
                {
                    var ss = "-" + s.p1;
                    if (s.p2 != null) ss += ", -" + s.p2;
                    ss = ss.PadRight(padWidth);
                    ss += s.description;
                    sb.AppendLine("  " + ss);
                }

                if (!Help.Values.IsEmpty()) { sb.AppendLine(); sb.AppendLine("Arguments:"); }
                foreach (var s in Help.Values) sb.AppendLine("  " + s);

                if (!Help.Examples.IsEmpty()) { sb.AppendLine(); sb.AppendLine("Examples:"); }
                foreach (var example in Help.Examples) sb.AppendLine("  huc " + Name + " " + example);

                return sb.ToString();
            }
        }

        protected Command()
        {
            log = Program.LogFactory.GetLogger(GetType());
            Help = new CommandHelpBuilder(GetType().Name);
            CreateHelp(Help);
        }

        public void Execute(string[] args)
        {
            using (var diag = Util.Diagnostic(log.Debug))
            {
                this.args = new Args(args);
                this.config = new ConfigFile();
                Execute();
            }
        }

        protected abstract void Execute();
        protected abstract void CreateHelp(CommandHelpBuilder help);

        #region File

        protected void DeleteExistingFile(string file)
        {
            if (File.Exists(file))
            {
                log.Info("Deleting existing file " + file);
                File.Delete(file);
            }
        }

        protected void CheckFileExists(string file)
        {
            if (!File.Exists(file)) throw new FileNotFoundException("File " + file + " does not exist", file);
        }

        protected string ReadFile(string path, Encoding encoding = null)
        {
            string data;
            path = Path.GetFullPath(path);
            log.Debug($"Reading text file {path}");
            using (Util.Diagnostic(log.Trace))
            {
                CheckFileExists(path);
                data = Util.FileRead(path, encoding ?? Constant.ENCODING_UTF8_WITHOUT_BOM);
            }
            log.Debug($"Read text file {path}   {data.Length} characters");
            return data;
        }

        protected byte[] ReadFileBinary(string path)
        {
            byte[] data;
            path = Path.GetFullPath(path);
            log.Debug($"Reading binary file {path}");
            using (Util.Diagnostic(log.Trace))
            {
                CheckFileExists(path);
                data = Util.FileRead(path);
            }
            log.Debug($"Read binary file {path}   {data.Length} bytes");
            return data;
        }

        protected void WriteFile(string path, string data, Encoding encoding = null)
        {
            path = Path.GetFullPath(path);
            log.Debug($"Writing text file {path}   {data.Length} characters");
            using (Util.Diagnostic(log.Trace))
            {
                DeleteExistingFile(path);
                Util.FileWrite(path, data, encoding ?? Constant.ENCODING_UTF8_WITHOUT_BOM);
            }
            log.Debug($"Wrote text file {path}   {data.Length} characters");
        }

        protected void WriteFileBinary(string path, byte[] data)
        {
            path = Path.GetFullPath(path);
            log.Debug($"Writing binary file {path}   {data.Length} bytes");
            using (Util.Diagnostic(log.Trace))
            {
                DeleteExistingFile(path);
                Util.FileWrite(path, data);
            }
            log.Debug($"Wrote binary file {path}   {data.Length} bytes");
        }

        protected Utilities.Table ReadTableTab(string path, Encoding encoding = null, bool headerRow = true)
        {
            var data = ReadFile(path, encoding);
            log.Debug($"Read {data.Length} characters from file {path}");

            var lines = data.SplitOnNewline();
            if (lines.Length > 0 && lines[lines.Length - 1] != null && lines[lines.Length - 1].Length == 0) lines = lines.RemoveTail(); // Ignore if last line is just line feed
            log.Debug($"Found {lines.Length} lines in file {path}");

            var t = Utilities.Table.Create(lines.Select(l => l.Split('\t')), headerRow);
            log.Debug($"Created table with {t.Columns.Count} columns and {t.Count} rows");

            return t;
        }

        protected void WriteTableTab(string fileName, Utilities.Table table, string suffix = null)
        {
            fileName = Path.GetFullPath(fileName);
            table.CheckNotNull(nameof(table));

            log.Debug("Writing TAB delimited Table to file " + fileName);
            using (var stream = Util.FileOpenWrite(fileName))
            using (var streamWriter = new StreamWriter(stream, Utilities.Constant.ENCODING_UTF8_WITHOUT_BOM))
            {
                table.ToDelimited(
                    o => streamWriter.Write(o),
                    headerDelimiter: "\t",
                    headerQuoting: null,
                    includeHeader: true,
                    dataDelimiter: "\t",
                    dataQuoting: null,
                    includeRows: true,
                    newLine: Utilities.Constant.NEWLINE_WINDOWS,
                    headerDelimiterReplacement: "        ",
                    dataDelimiterReplacement: "        "
                    );
                streamWriter.Flush();
                stream.Flush(true);
            }

            log.Info("Successfully wrote " + table.ToString() + " to file " + fileName + (suffix ?? string.Empty));
        }

        #endregion File

        #region Parameters

        public string GetArgParameter(string key1, string key2) => args.GetParameter(key1, key2);

        public string GetArgParameterOrConfig(string key1, string key2)
        {
            var v = GetArgParameter(key1, key2);
            if (v.TrimOrNull() == null) v = GetArgParameterConfig(key1);
            log.Debug($"{key1}: {v}");
            return v;
        }

        public string GetArgParameterOrConfig(string key1, string key2, string defaultValue)
        {
            var v = GetArgParameter(key1, key2);
            if (v.TrimOrNull() == null) v = GetArgParameterConfig(key1);
            if (v.TrimOrNull() == null) v = defaultValue;
            log.Debug($"{key1}: {v}");
            return v;
        }

        public string GetArgParameterOrConfigRequired(string key1, string key2)
        {
            var v = GetArgParameter(key1, key2);

            if (v.TrimOrNull() == null) v = GetArgParameterConfig(key1);
            if (v.TrimOrNull() != null)
            {
                log.Debug($"{key1}: {v}");
                return v;
            }

            var msg = $"No value provided for argument '{key1}' or properties file entry for '{Name}.{key1}'";
            throw new ArgsException(key1, msg);
        }

        public Encoding GetArgParameterOrConfigEncoding(string key1, string key2)
        {
            var v = GetArgParameter(key1, key2);
            if (v.TrimOrNull() == null) v = GetArgParameterConfig(key1);
            log.Debug($"{key1}String: {v}");
            Encoding encoding = Constant.ENCODING_UTF8_WITHOUT_BOM;
            if (v.TrimOrNull() != null) encoding = Util.ParseEncoding(v);
            log.Debug($"{key1}: {encoding}");
            return encoding;
        }

        public int GetArgParameterOrConfigInt(string key1, string key2, int defaultValue)
        {
            var v = GetArgParameter(key1, key2);
            if (v.TrimOrNull() == null) v = GetArgParameterConfig(key1);
            log.Debug($"{key1}String: {v}");
            var o = defaultValue;
            if (v.TrimOrNull() != null) o = v.ToInt();
            log.Debug($"{key1}: {o}");
            return o;
        }

        public bool GetArgParameterOrConfigBool(string key1, string key2, bool defaultValue)
        {
            var v = GetArgParameter(key1, key2);
            if (v.TrimOrNull() == null) v = GetArgParameterConfig(key1);
            log.Debug($"{key1}String: {v}");
            var o = defaultValue;
            if (v.TrimOrNull() != null) o = v.ToBool();
            log.Debug($"{key1}: {o}");
            return o;
        }

        public T GetArgParameterOrConfigEnum<T>(string key1, string key2, T defaultValue) where T : struct, IConvertible, IComparable, IFormattable
        {
            var v = GetArgParameter(key1, key2);
            if (v.TrimOrNull() == null) v = GetArgParameterConfig(key1);
            log.Debug($"{key1}String: {v}");
            var o = defaultValue;
            if (v.TrimOrNull() != null)
            {
                var onullable = Util.GetEnumItemNullable<T>(v);
                if (onullable == null) throw new ArgsException(key1, "Parameter " + key1 + " is not valid, values are [ " + Util.GetEnumItems<T>().ToStringDelimited(" | ") + " ]");
                o = onullable.Value;
            }
            log.Debug($"{key1}: {o}");
            return o;
        }

        public string GetArgParameterConfig(string key) => config[Name + "." + key];

        public IReadOnlyList<string> GetArgValues() => args.Values;

        public List<string> GetArgValuesTrimmed() => GetArgValues().TrimOrNull().WhereNotNull().ToList();

        public string GetArgValueTrimmed(int index) => GetArgValuesTrimmed().GetAtIndexOrDefault(index);

        public (string firstValue, List<string> otherValues) GetArgValuesTrimmed1N()
        {
            var list = GetArgValuesTrimmed();
            if (list.Count < 1) return (null, list);
            var firstItem = list.PopHead();
            return (firstItem, list);
        }
        #endregion Parameters
    }

    public class CommandHelpBuilder
    {
        public CommandHelpBuilder(string name) => Name = name;
        public string Name { get; }

        private readonly List<string> summary = new();
        public string Summary => summary.FirstOrDefault();

        private readonly List<(string p1, string p2, string description)> parameters = new();
        public IReadOnlyList<(string p1, string p2, string description)> Parameters => parameters;

        private readonly List<string> values = new();
        public IReadOnlyList<string> Values => values;

        private readonly List<string> details = new();
        public IReadOnlyList<string> Details => details;

        private readonly List<string> examples = new();
        public IReadOnlyList<string> Examples => examples;

        public void AddSummary(string msg) => summary.Add(msg);
        public void AddValue(string msg) => values.Add(msg);
        public void AddDetail(string msg) => details.Add(msg);
        public void AddParameter(string p1, string p2, string description) => parameters.Add((p1, p2, description));
        public void AddParameter(string p1, string description) => parameters.Add((p1, null, description));
        public void AddExample(string example) => examples.Add(example.Replace("`", "\""));
    }
}
