﻿// Copyright (c) 2022 Max Run Software (dev@maxrunsoftware.com)
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// ReSharper disable RedundantAssignment

namespace MaxRunSoftware.Utilities.Tests;

public class BucketReadOnlyTests : TestBase
{
    public BucketReadOnlyTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

    [TestFact]
    public void Testing()
    {
        var cgf = new CacheGenFunc();

        var bro = new BucketCache<string, string>(cgf.GenVal);
        Assert.Equal(0, cgf.TimesCalled);
        Assert.Empty(bro.Keys);

        var val = bro["a"];
        Assert.Equal("Va", val);
        Assert.Equal(1, cgf.TimesCalled);
        Assert.Single(bro.Keys);

        val = bro["a"];
        Assert.Equal("Va", val);
        Assert.Equal(1, cgf.TimesCalled);
        Assert.Single(bro.Keys);

        val = bro["b"];
        Assert.Equal("Vb", val);
        Assert.Equal(2, cgf.TimesCalled);
        Assert.Equal(2, bro.Keys.Count());

        val = bro["a"];
        Assert.Equal("Va", val);
        Assert.Equal(2, cgf.TimesCalled);
        Assert.Equal(2, bro.Keys.Count());

        val = bro["n"];
        Assert.Null(val);
        Assert.Equal(3, cgf.TimesCalled);
        Assert.Equal(3, bro.Keys.Count());

        val = bro["n"];
        Assert.Null(val);
        Assert.Equal(3, cgf.TimesCalled);
        Assert.Equal(3, bro.Keys.Count());

        try
        {
            val = bro[null];
            Assert.True(false, "Expecting exception to be thrown");
        }
        catch (Exception) { Assert.True(true); }
    }

    private class CacheGenFunc
    {
        public int TimesCalled { get; private set; }

        public string GenVal(string key)
        {
            TimesCalled++;
            if (key == "n") return null;

            return "V" + key;
        }
    }
}
