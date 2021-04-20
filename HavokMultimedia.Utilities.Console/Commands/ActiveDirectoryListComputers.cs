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
using System.Linq;

namespace HavokMultimedia.Utilities.Console.Commands
{
    public class ActiveDirectoryListComputers : ActiveDirectoryBase
    {
        protected override void CreateHelp(CommandHelpBuilder help)
        {
            base.CreateHelp(help);
            help.AddSummary("Lists all group names in an ActiveDirectory");
            help.AddExample("-h=192.168.1.5 -u=administrator -p=testpass");
        }

        protected override void Execute()
        {
            base.Execute();

            using (var ad = GetActiveDirectory())
            {
                var objects = ad.GetAll().OrEmpty();
                foreach (var obj in objects.OrderBy(o => o.SAMAccountName, StringComparer.OrdinalIgnoreCase))
                {
                    if (!obj.IsComputer) continue;
                    log.Info(obj.SAMAccountName);
                }
            }
        }
    }
}