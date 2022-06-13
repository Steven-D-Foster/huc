﻿/*
Copyright (c) 2022 Max Run Software (dev@maxrunsoftware.com)

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

using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MaxRunSoftware.Utilities.External;

public class VMwareFolder : VMwareObject
{
    public string Name { get; }
    public string Folder { get; }
    public string Type { get; }

    // ReSharper disable once UnusedParameter.Local
    public VMwareFolder(VMwareClient vmware, JToken obj)
    {
        Name = obj.ToString("name");
        Folder = obj.ToString("folder");
        Type = obj.ToString("type");
    }

    public static IEnumerable<VMwareFolder> Query(VMwareClient vmware)
    {
        foreach (var obj in vmware.GetValueArray("/rest/vcenter/folder"))
        {
            yield return new VMwareFolder(vmware, obj);
        }
    }
}