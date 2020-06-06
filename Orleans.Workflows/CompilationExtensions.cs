using System;
using System.Reflection;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.TypeSystem;
using Microsoft.CodeAnalysis.CSharp;

namespace Orleans.Workflows
{
    public static class CompilationExtensions
    {
        public static string ToSourceCode(this Type source, Assembly assembly = null)
        {
            var decompiler = new CSharpDecompiler((assembly ?? source.Assembly).Location, new DecompilerSettings());
            return decompiler.DecompileTypeAsString(new FullTypeName(source.AssemblyQualifiedName));
        }

        public static Type FromSourceCode(string typeDefinition)
        {
            throw new NotImplementedException();
        }
    }
}
