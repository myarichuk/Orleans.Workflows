using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orleans.Workflows.Exceptions
{
    public class CompilationFailedException : Exception
    {
        public List<Diagnostic> CompilationErrors { get; }

        public CompilationFailedException(IEnumerable<Diagnostic> compilationErrors) : base($"Failed to compile code, found {compilationErrors.Count()} errors")
        {
            CompilationErrors = compilationErrors.ToList();
        }
    }
}
