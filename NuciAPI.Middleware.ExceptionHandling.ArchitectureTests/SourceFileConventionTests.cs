using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace NuciAPI.Middleware.ExceptionHandling.ArchitectureTests
{
    [TestFixture]
    public sealed class SourceFileConventionTests
    {
        [Test]
        public void Given_ProductionSourceFiles_When_Inspected_Then_InventoryIsNonEmpty()
        {
            string[] sourceFiles = GetProductionSourceFiles();

            Assert.That(sourceFiles, Is.Not.Empty);
        }

        [Test]
        public void Given_ProductionSourceFiles_When_Inspected_Then_EachFileContainsExactlyOneDeclaredType()
        {
            List<string> violations = new();

            foreach (string sourceFile in GetProductionSourceFiles())
            {
                CompilationUnitSyntax syntax = ParseSourceFile(sourceFile);
                int declaredTypeCount = syntax.DescendantNodes().OfType<BaseTypeDeclarationSyntax>().Count()
                    + syntax.DescendantNodes().OfType<DelegateDeclarationSyntax>().Count();

                if (declaredTypeCount != 1)
                {
                    violations.Add($"{sourceFile}: expected one declared type, found {declaredTypeCount}.");
                }
            }

            Assert.That(violations, Is.Empty, string.Join(Environment.NewLine, violations));
        }

        [Test]
        public void Given_ProductionSourceFiles_When_Inspected_Then_FileNamesMatchDeclaredTypes()
        {
            List<string> violations = new();

            foreach (string sourceFile in GetProductionSourceFiles())
            {
                CompilationUnitSyntax syntax = ParseSourceFile(sourceFile);
                string? declaredTypeName = GetDeclaredTypeName(syntax);
                string fileName = Path.GetFileNameWithoutExtension(sourceFile);

                if (declaredTypeName is null || !string.Equals(fileName, declaredTypeName, StringComparison.Ordinal))
                {
                    violations.Add($"{sourceFile}: file stem must match declared type '{declaredTypeName ?? "<none>"}'.");
                }
            }

            Assert.That(violations, Is.Empty, string.Join(Environment.NewLine, violations));
        }

        [Test]
        public void Given_ProductionSourceFiles_When_Inspected_Then_NoGeneratedOrBuildFilesAreIncluded()
        {
            string[] sourceFiles = GetProductionSourceFiles();

            Assert.That(sourceFiles, Has.All.Matches<string>(path =>
                !path.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                && !path.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                && !Path.GetFileName(path).Contains(".g.", StringComparison.Ordinal)));
        }

        private static CompilationUnitSyntax ParseSourceFile(string sourceFile)
        {
            string source = File.ReadAllText(sourceFile);
            return CSharpSyntaxTree.ParseText(source).GetCompilationUnitRoot();
        }

        private static string? GetDeclaredTypeName(CompilationUnitSyntax syntax)
        {
            BaseTypeDeclarationSyntax? type = syntax.DescendantNodes().OfType<BaseTypeDeclarationSyntax>().FirstOrDefault();
            if (type is not null)
            {
                return type.Identifier.ValueText;
            }

            return syntax.DescendantNodes().OfType<DelegateDeclarationSyntax>().FirstOrDefault()?.Identifier.ValueText;
        }

        private static string[] GetProductionSourceFiles()
        {
            string projectDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "NuciAPI.Middleware.ExceptionHandling"));

            if (!Directory.Exists(projectDirectory))
            {
                Assert.Fail($"Production source directory is unavailable: {projectDirectory}.");
            }

            return Directory.GetFiles(projectDirectory, "*.cs", SearchOption.AllDirectories)
                .Where(path => !string.Equals(Path.GetFileName(path), "AssemblyInfo.cs", StringComparison.Ordinal))
                .Where(path => !path.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                .Where(path => !path.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                .Where(path => !Path.GetFileName(path).Contains(".g.", StringComparison.Ordinal))
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();
        }
    }
}
