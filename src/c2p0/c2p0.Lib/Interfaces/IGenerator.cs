using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System.CodeDom.Compiler;

namespace c2p0.Lib.Interfaces
{
    public interface IGenerator
    {

    }
    public class Generator : IGenerator
    {
        public void Compile(string filepath, string publicKey)
        {
            
        }
    }
}
