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

using System.IO;
using System.Text;

namespace HavokMultimedia.Utilities.Console.Commands
{
    public abstract class ConvertBase : Command
    {
        protected override void CreateHelp(CommandHelpBuilder help)
        {
            help.AddSummary(Summary);
            help.AddParameter("bufferSizeMegabytes", "b", "SFTP buffer size in megabytes (10)");
            help.AddParameter("encoding", "en", "Encoding of the output file, ASCII/BIGENDIANUNICODE/DEFAULT/UNICODE/UTF32/UTF8/UTF8BOM (UTF8)");
            help.AddValue("<input file> <output file>");
        }

        protected abstract string Summary { get; }
        protected abstract void ProcessStreams(Stream inputStream, Stream outputStream);
        protected Encoding encoding;
        protected int bufferSize;
        protected override void Execute()
        {
            encoding = GetArgParameterOrConfigEncoding("encoding", "en");
            var bufferSizeMegabytes = GetArgParameterOrConfigInt("bufferSizeMegabytes", "b", 10).ToString().ToUInt();
            bufferSize = (int)bufferSizeMegabytes * (int)Constant.BYTES_MEGA;

            var inputFile = GetArgValues().GetAtIndexOrDefault(0).TrimOrNull();
            log.Debug($"{nameof(inputFile)}: {inputFile}");
            inputFile = Util.ParseInputFile(inputFile);
            if (!File.Exists(inputFile)) throw new FileNotFoundException($"{nameof(inputFile)} {inputFile} does not exist");
            log.Debug($"{nameof(inputFile)}: {inputFile}");

            var outputFile = GetArgValues().GetAtIndexOrDefault(1).TrimOrNull();
            log.Debug($"{nameof(outputFile)}: {outputFile}");
            if (outputFile == null) throw new ArgsException(nameof(outputFile), $"No {nameof(outputFile)} specified");
            outputFile = Path.GetFullPath(outputFile);
            if (inputFile.EqualsCaseInsensitive(outputFile)) throw new ArgsException(nameof(outputFile), $"{nameof(outputFile)} cannot be the same as {nameof(inputFile)}");
            log.Debug($"{nameof(outputFile)}: {outputFile}");
            DeleteExistingFile(outputFile);

            using (var iStream = Util.FileOpenRead(inputFile))
            {
                using (var oStream = Util.FileOpenWrite(outputFile))
                {
                    ProcessStreams(iStream, oStream);
                    oStream.FlushSafe();
                    oStream.CloseSafe();
                }
                iStream.Close();
            }
        }
    }

    public abstract class ConvertBinaryToText : ConvertBase
    {
        protected override void ProcessStreams(Stream inputStream, Stream outputStream)
        {
            using (var writer = new StreamWriter(outputStream, encoding, bufferSize))
            {
                inputStream.Read(o => writer.Write(Convert(o)));
                writer.FlushSafe();
            }
        }

        protected abstract string Convert(byte[] bytes);
    }

    public abstract class ConvertTextToBinary : ConvertBase
    {
        protected override void ProcessStreams(Stream inputStream, Stream outputStream)
        {
            using (var writer = new BinaryWriter(outputStream))
            {
                using (var reader = new StreamReader(inputStream, encoding, false, bufferSize))
                {
                    reader.Read(o => writer.Write(Convert(new string(o))));
                }
                writer.FlushSafe();
            }
        }

        protected abstract byte[] Convert(string str);
    }

    public class ConvertBinaryToBase16 : ConvertBinaryToText
    {
        protected override string Summary => "Converts binary file to base 16 file";
        protected override string Convert(byte[] bytes) => Util.Base16(bytes);
    }

    public class ConvertBinaryToBase64 : ConvertBinaryToText
    {
        protected override string Summary => "Converts binary file to base 64 file";
        protected override string Convert(byte[] bytes) => Util.Base64(bytes);
    }

    public class ConvertBase16ToBinary : ConvertTextToBinary
    {
        protected override string Summary => "Converts base 16 file to binary file";
        protected override byte[] Convert(string str) => Util.Base16(str);
    }

    public class ConvertBase16ToBase64 : ConvertTextToBinary
    {
        protected override string Summary => "Converts base 16 file to base 64 file";
        protected override byte[] Convert(string str) => Utilities.Constant.ENCODING_UTF8_WITHOUT_BOM.GetBytes(Util.Base64(Util.Base16(str)));
    }

    public class ConvertBase64ToBinary : ConvertTextToBinary
    {
        protected override string Summary => "Converts base 64 file to binary file";
        protected override byte[] Convert(string str) => Util.Base64(str);
    }

    public class ConvertBase64ToBase16 : ConvertTextToBinary
    {
        protected override string Summary => "Converts base 64 file to base 16 file";
        protected override byte[] Convert(string str) => Utilities.Constant.ENCODING_UTF8_WITHOUT_BOM.GetBytes(Util.Base16(Util.Base64(str)));
    }

}