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
using System.Linq;

namespace HavokMultimedia.Utilities.Console.Commands
{
    public class DirectoryRemoveEmpty : Command
    {
        protected override void CreateHelp(CommandHelpBuilder help)
        {
            help.AddSummary("Scans all subfolders recursively in target directory and removes any folders that are empty");
            help.AddValue("<target directory>");
            help.AddExample("MyDirectory");
        }

        protected override void ExecuteInternal()
        {
            //var encoding = GetArgParameterOrConfigEncoding("encoding", "en");
            var targetDirectory = GetArgValueTrimmed(0);
            if (targetDirectory == null) throw new ArgsException(nameof(targetDirectory), $"No <{nameof(targetDirectory)}> specified");
            targetDirectory = Path.GetFullPath(targetDirectory);
            log.Debug($"{nameof(targetDirectory)}: {targetDirectory}");
            if (!Directory.Exists(targetDirectory)) throw new ArgsException(nameof(targetDirectory), $"<{nameof(targetDirectory)}> does not exist {targetDirectory}");

            var subdirs = Util.FileListDirectories(targetDirectory, recursive: true)
                .Select(o => Path.GetFullPath(o))
                .Where(o => o.Length > targetDirectory.Length)
                .Where(o => !o.EqualsCaseInsensitive(targetDirectory))
                .OrderBy(o => o.Length)
                .Reverse()
                .ToList();

            foreach (var subdir in subdirs)
            {
                var items = Util.FileList(subdir).Where(o => !o.Path.EqualsCaseSensitive(subdir)).ToList();
                log.Debug(subdir + " [files:" + items.Where(o => !o.IsDirectory).Count() + "] [directories:" + items.Where(o => o.IsDirectory).Count() + "]");
                if (items.IsEmpty())
                {
                    log.Info("Delete: " + subdir);
                    Directory.Delete(subdir);
                }
            }

        }
    }
}