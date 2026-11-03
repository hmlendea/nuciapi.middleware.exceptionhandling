using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace NuciAPI.Middleware.ExceptionHandling.ArchitectureTests
{
    [TestFixture]
    public sealed class ArchitectureProjectSourceTests
    {
        [Test]
        public void Given_ArchitectureSources_When_Inspected_Then_AllTestFixturesUseNUnit()
        {
            string projectDirectory = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "NuciAPI.Middleware.ExceptionHandling.ArchitectureTests");
            string[] sourceFiles = Directory.GetFiles(projectDirectory, "*.cs", SearchOption.AllDirectories)
                .Where(path => !path.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                .Where(path => !path.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                .Where(path => !path.EndsWith("AssemblyInfo.cs", StringComparison.Ordinal))
                .ToArray();

            string[] violations = sourceFiles
                .Where(path => !CSharpSyntaxTree.ParseText(File.ReadAllText(path)).GetCompilationUnitRoot()
                    .DescendantNodes().OfType<AttributeSyntax>()
                    .Any(attribute => attribute.Name.ToString().Contains("TestFixture", StringComparison.Ordinal)))
                .ToArray();

            Assert.That(violations, Is.Empty);
        }
    }
}
