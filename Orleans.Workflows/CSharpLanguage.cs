using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using ICSharpCode.Decompiler.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host;
using Microsoft.IO;
using Orleans.Workflows.Exceptions;
using LanguageVersion = Microsoft.CodeAnalysis.CSharp.LanguageVersion;

namespace Orleans.Workflows
{
    //credit: adapted from https://josephwoodward.co.uk/2016/12/in-memory-c-sharp-compilation-using-roslyn
    public class CSharpLanguage : ILanguageService
    {
        private static readonly LanguageVersion MaxLanguageVersion = Enum
            .GetValues(typeof(LanguageVersion))
            .Cast<LanguageVersion>()
            .Max();

        private static readonly Lazy<CSharpLanguage> SourceLanguage = new Lazy<CSharpLanguage>(() => new CSharpLanguage());
        private static readonly RecyclableMemoryStreamManager RecyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        private static readonly List<PortableExecutableReference> SystemReferences =
            ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator)
                .Select(p => MetadataReference.CreateFromFile(p))
                .ToList();

        private static readonly CSharpParseOptions DefaultOtions = new CSharpParseOptions(kind: SourceCodeKind.Regular, languageVersion: MaxLanguageVersion);

        internal readonly IReadOnlyCollection<MetadataReference> References = new[] 
        {
          MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location),
          MetadataReference.CreateFromFile(typeof(ExpandoObject).GetTypeInfo().Assembly.Location),
          MetadataReference.CreateFromFile(typeof(WorkflowActivity).GetTypeInfo().Assembly.Location),
          MetadataReference.CreateFromFile(typeof(List<>).GetTypeInfo().Assembly.Location),
          MetadataReference.CreateFromFile(typeof(IQueryable<>).GetTypeInfo().Assembly.Location),
          MetadataReference.CreateFromFile(typeof(ValueTuple<>).GetTypeInfo().Assembly.Location)
        };


        internal SyntaxTree ParseText(string sourceCode, CSharpParseOptions options = null) =>
            CSharpSyntaxTree.ParseText(sourceCode, options ?? DefaultOtions);

        internal Compilation CreateLibraryCompilation(string assemblyName)
        {
#if RELEASE
            var enableOptimisations = true;
#else
            var enableOptimisations = false;
#endif

            //TODO: make this static to avoid unnecessary allocations
            var options = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: enableOptimisations ? OptimizationLevel.Release : OptimizationLevel.Debug,
                allowUnsafe: true);

            return CSharpCompilation.Create(assemblyName, options: options, references: References);
        }

        public static Assembly CompileAssemblyFrom(string code)
        {
            var syntaxTree = SourceLanguage.Value.ParseText(code);
            
            var compilation = SourceLanguage.Value
                .CreateLibraryCompilation(assemblyName: $"InMemoryAssembly_{Guid.NewGuid()}")
                .AddReferences(SourceLanguage
                    .Value
                    .References)
                .AddReferences(SystemReferences)
                .AddSyntaxTrees(syntaxTree);

            using (var stream = RecyclableMemoryStreamManager.GetStream())
            {
                var emitResult = compilation.Emit(stream);
                if (emitResult.Success)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    return AssemblyLoadContext.Default.LoadFromStream(stream);
                }
                else
                {
                    var compilationErrors = emitResult.Diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error);
                    if(compilationErrors.Any())
                        throw new CompilationFailedException(compilationErrors);
                }
            }

            return null;
        }
    }
}
