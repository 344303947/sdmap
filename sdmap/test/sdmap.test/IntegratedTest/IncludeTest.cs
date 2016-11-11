﻿using sdmap.Parser.Visitor;
using sdmap.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace sdmap.test.IntegratedTest
{
    public class IncludeTest
    {
        [Fact]
        public void Include()
        {
            var code = "sql v1{#include<v2>} sql v2{Life is good}";
            var rt = new SdmapRuntime();
            rt.AddSourceCode(code);
            var result = rt.Emit("v1", null);
            Assert.Equal("Life is good", result);
        }

        [Fact]
        public void MixIncludeWithPlain()
        {
            var code = 
                "sql v1{Hello World #include<v2>}" + 
                "sql v2{Life is good}";
            var rt = new SdmapRuntime();
            rt.AddSourceCode(code);
            var result = rt.Emit("v1", null);
            Assert.Equal("Hello World Life is good", result);
        }

        [Fact]
        public void Include2()
        {
            var code =
                "sql v1{#include<v2> #include<v3>}" +
                "sql v2{Life is good}" + 
                "sql v3{Nice!}";
            var rt = new SdmapRuntime();
            rt.AddSourceCode(code);
            var result = rt.Emit("v1", null);
            Assert.Equal("Life is good Nice!", result);
        }

        [Fact]
        public void IncludeNested()
        {
            var code =
                "sql v1{#include<v2>#include<v3>}" +
                "sql v2{2#include<v3>}" +
                "sql v3{3}";
            var rt = new SdmapRuntime();
            rt.AddSourceCode(code);
            var result = rt.Emit("v1", null);
            Assert.Equal("233", result);
        }

        [Fact]
        public void Include2SourceFile()
        {
            var code1 = "sql v1{1#include<v2>}";
            var code2 = "sql v2{#iif<A, 'A', 'B'>}";
            var rt = new SdmapRuntime();
            rt.AddSourceCode(code1);
            rt.AddSourceCode(code2);
            var result = rt.Emit("v1", new { A = true });
            Assert.Equal("1A", result);
        }

        [Fact]
        public void IncludeWithNs()
        {
            var code1 = "sql v1{1#include<ns.v2>}";
            var code2 = "namespace ns{sql v2{2}}";
            var rt = new SdmapRuntime();
            rt.AddSourceCode(code1);
            rt.AddSourceCode(code2);
            var result = rt.Emit("v1", new { A = true });
            Assert.Equal("12", result);
        }

        [Fact]
        public void CanReferenceOtherInOneNamespace()
        {
            var code = "namespace ns{sql v1{1#include<v2>} sql v2{2}}";
            var rt = new SdmapRuntime();
            rt.AddSourceCode(code);
            var result = rt.Emit("ns.v1", new { A = true });
            Assert.Equal("12", result);
        }
    }
}
