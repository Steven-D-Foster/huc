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
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using EmbedIO;
using HavokMultimedia.Utilities.Console.External;

namespace HavokMultimedia.Utilities.Console.Commands
{
    public class WebServerUtility : WebServerBase
    {
        protected override void CreateHelp(CommandHelpBuilder help)
        {
            base.CreateHelp(help);

            help.AddSummary("Creates a web server that provides various utilities");
            help.AddParameter("randomDisable", "rd", "Disables the /random utility");
            help.AddExample("");
        }

        private bool disableRandom;
        private bool disableRandomFile;

        protected override void Execute()
        {
            base.Execute();
            var config = GetConfig();


            disableRandom = GetArgParameterOrConfigBool("randomDisable", "rd", false);
            if (!disableRandom) config.AddPathHandler("/random", HttpVerbs.Get, HandleRandom);

            disableRandomFile = GetArgParameterOrConfigBool("randomFileDisable", "rfd", false);
            if (!disableRandomFile) config.AddPathHandler("/randomFile", HttpVerbs.Get, HandleRandomFile);

            config.AddPathHandler("/", HttpVerbs.Get, HandleIndex);


            using (var server = GetWebServer(config))
            {
                foreach (var ipa in config.UrlPrefixes) log.Info("  " + ipa);
                log.Info("WebServer running, press ESC or Q to quit");
                while (true)
                {
                    Thread.Sleep(50);
                    var cki = System.Console.ReadKey(true);

                    if (cki.Key.In(ConsoleKey.Escape, ConsoleKey.Q)) break;
                }
            }

            log.Info("WebServer shutdown");
        }

        private object HandleIndex(IHttpContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<br />");
            if (!disableRandom) sb.AppendLine($"<p><a href=\"/random\">Random</a></p>");
            if (!disableRandomFile) sb.AppendLine($"<p><a href=\"/randomFile\">RandomFile</a></p>");
            return External.WebServer.HtmlMessage("Utilities", sb.ToString());
        }

        private object HandleRandom(IHttpContext context)
        {
            var sb = new StringBuilder();
            using (var srandom = RandomNumberGenerator.Create())
            {
                var length = context.GetParameterInt("length", 100);
                var chars = context.GetParameterString("chars", Constant.CHARS_A_Z_LOWER + Constant.CHARS_0_9);

                for (int i = 0; i < length; i++)
                {
                    var c = chars[srandom.Next(chars.Length)];
                    sb.Append(c);
                }

            }
            return sb.ToString();
        }

        private object HandleRandomFile(IHttpContext context)
        {
            var randomString = (string)HandleRandom(context);
            context.AddHeader("Content-Disposition", "attachment", "filename=\"random.txt\"");
            var bytes = Constant.ENCODING_UTF8_WITHOUT_BOM.GetBytes(randomString);
            using (var stream = context.OpenResponseStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            return "Generated Random File";
        }
    }
}
