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

namespace HavokMultimedia.Utilities.Console.Commands
{
    public abstract class WebServerBase : Command
    {
        protected override void CreateHelp(CommandHelpBuilder help)
        {
            help.AddParameter("ipaddress", "ip", "IP address to bind to (localhost)");
            help.AddParameter("port", "o", "Port to bind to (8080)");
            help.AddParameter("username", "u", "Require a username to access this resource (user)");
            help.AddParameter("password", "p", "Require a password to access this resource");
            help.AddDetail("If you specify a password, the default username is 'user' but can be changed");
        }

        private string ipaddress;
        private ushort port = 0;
        private string username;
        private string password;

        protected override void Execute()
        {
            ipaddress = GetArgParameterOrConfig("ipaddress", "ip").TrimOrNull();
            port = GetArgParameterOrConfigUShort("port", "o", 8080);
            if (port == 0) throw new ArgsException(nameof(port), "Invalid port 0 specified");
            username = GetArgParameterOrConfig("username", "u", "user").TrimOrNull();
            password = GetArgParameterOrConfig("password", "p").TrimOrNull();
        }

        protected External.WebServerConfig GetConfig()
        {
            if (port == 0) throw new Exception("base.Execute() never called for class " + GetType().FullNameFormatted());

            var config = new External.WebServerConfig();
            if (ipaddress != null)
            {
                config.Hostnames.Clear();
                config.Hostnames.Add(ipaddress);
            }
            config.Port = port;
            if (username != null && password != null) config.Users.Add((username, password));
            return config;
        }

        protected External.WebServer GetWebServer(External.WebServerConfig config)
        {
            if (port == 0) throw new Exception("base.Execute() never called for class " + GetType().FullNameFormatted());

            var server = new External.WebServer();
            log.Debug("Starting web server");
            server.Start(config);
            log.Info("Webserver started");
            return server;
        }

    }
}
