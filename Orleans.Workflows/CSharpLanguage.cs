using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
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

        internal readonly IReadOnlyCollection<MetadataReference> References = new[] 
        {
          MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location),
          MetadataReference.CreateFromFile(typeof(ExpandoObject).GetTypeInfo().Assembly.Location),
          MetadataReference.CreateFromFile(typeof(WorkflowActivity).GetTypeInfo().Assembly.Location),
          MetadataReference.CreateFromFile(typeof(List<>).GetTypeInfo().Assembly.Location),
          MetadataReference.CreateFromFile(typeof(IQueryable<>).GetTypeInfo().Assembly.Location),
          MetadataReference.CreateFromFile(typeof(ValueTuple<>).GetTypeInfo().Assembly.Location)
        };

        private static readonly Lazy<CSharpLanguage> SourceLanguage = new Lazy<CSharpLanguage>(() => new CSharpLanguage());
        private static readonly RecyclableMemoryStreamManager RecyclableMemoryStreamManager = new RecyclableMemoryStreamManager();

        public SyntaxTree ParseText(string sourceCode, SourceCodeKind kind = SourceCodeKind.Regular)
        {
            var options = new CSharpParseOptions(kind: kind, languageVersion: MaxLanguageVersion);

            return CSharpSyntaxTree.ParseText(sourceCode, options);
        }

        public Compilation CreateLibraryCompilation(string assemblyName, bool enableOptimisations)
        {
            var options = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: enableOptimisations ? OptimizationLevel.Release : OptimizationLevel.Debug,
                allowUnsafe: true);

            return CSharpCompilation.Create(assemblyName, options: options, references: References);
        }

        public static Assembly CreateAssemblyFrom(string code)
        {
            var syntaxTree = SourceLanguage.Value.ParseText(code);

            var trustedAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);

            var systemReferences = trustedAssembliesPaths
                .Select(p => MetadataReference.CreateFromFile(p))
                .ToList();

            var compilation = SourceLanguage.Value
                .CreateLibraryCompilation(assemblyName: "InMemoryAssembly", enableOptimisations: false)
                .AddReferences(SourceLanguage
                    .Value
                    .References)
                .AddReferences(systemReferences)
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
