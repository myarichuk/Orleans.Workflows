using System;
using System.Linq;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;

namespace Orleans.Workflows
{
    public static class CompilationExtensions
    {       
        public static string ToSourceCode(this Type source)
        {
            if (source.IsNested)
                throw new NotSupportedException("Decompilation of nested types is not supported");

            if(!source.IsClass)
                throw new NotSupportedException("Decompilation of non-reference types is not supported");

            var decompiler = new CSharpDecompiler(source.Assembly.Location, new DecompilerSettings());
            return decompiler.DecompileTypeAsString(new FullTypeName(source.FullName));
        }

        public static Type CompileFromSourceCode(this string typeCSharpDefinition)
        {
            var typeAssembly = CSharpLanguage.CreateAssemblyFrom(typeCSharpDefinition);
            return typeAssembly.GetTypes().FirstOrDefault();
        }
    }
}
