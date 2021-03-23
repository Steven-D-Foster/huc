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
using System.Linq;

namespace HavokMultimedia.Utilities.Console
{

    public class Program
    {
        private static readonly ILogger log = LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ILogFactory LogFactory { get { return Utilities.LogFactory.LogFactoryImpl; } }

        public static List<Type> CommandTypes => typeof(Program).Assembly
            .GetTypesOf<ICommand>(requireNoArgConstructor: true)
            .OrderBy(o => o.FullNameFormatted())
            .ToList();

        public static List<ICommand> CommandObjects => CommandTypes
            .Select(o => CreateCommand(o))
            .ToList();

        public static string Version
        {
            get
            {
                try
                {
                    return typeof(Program).Assembly.GetVersion();
                }
                catch (Exception) { }
                return null;
            }
        }

        private static ICommand CreateCommand(Type type) => (ICommand)Activator.CreateInstance(type);

        public static int Main(string[] args)
        {
            int returnValue;
            try
            {
                var p = new Program();
                returnValue = p.Run(args);
            }
            catch (Exception e)
            {
                returnValue = 1;
                log.Error("Error", e);
            }

            return returnValue;
        }

        private int Run(string[] args)
        {
            var a = new Args(args);
            Utilities.LogFactory.LogFactoryImpl.SetupConsole(a.IsDebug);
            log.Debug(a.ToString());

            var commandTypes = CommandTypes;

            try
            {
                ConfigFile.CreateDefaultPropertiesFile();
            }
            catch (Exception e)
            {
                log.Warn("Could not create default properties file", e);
            }

            if (a.Command == null)
            {
                log.Info(typeof(Program).Namespace + " " + Version);
                log.Info("No command specified");
                log.Info("Commands: ");
                foreach (var c in CommandObjects) log.Info("  " + c.HelpSummary);
                return 2;
            }
            var command = commandTypes.Where(o => o.Name.Equals(a.Command, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (command == null)
            {
                log.Info(typeof(Program).Namespace + " " + Version);
                log.Info($"Command '{a.Command}' does not exist");
                log.Info("Commands: ");
                foreach (var c in CommandObjects) log.Info("  " + c.HelpSummary);
                return 3;
            }
            if (a.Values.IsEmpty() && a.Parameters.IsEmpty())
            {
                log.Info(typeof(Program).Namespace + " " + Version + " : " + command.Name);
                log.Info(CreateCommand(command).HelpDetails);
                return 4;
            }


            log.Info(typeof(Program).Namespace + " " + Version + " : " + command.Name);
            var cmd = CreateCommand(command);
            cmd.Execute(args);
            return 0;
        }



    }

}