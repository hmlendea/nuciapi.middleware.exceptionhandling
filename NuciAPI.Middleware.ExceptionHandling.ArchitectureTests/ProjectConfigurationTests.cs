using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace NuciAPI.Middleware.ExceptionHandling.ArchitectureTests
{
    [TestFixture]
    public sealed class ProjectConfigurationTests
    {
        [Test]
        public void Given_ProductionProject_When_Inspected_Then_RequiredCompilationSettingsAreDeclared()
        {
            XElement project = LoadProductionProject();
            XElement propertyGroup = project.Elements("PropertyGroup").Single(group => group.Element("TargetFramework") is not null);

            Assert.Multiple(() =>
            {
                Assert.That(propertyGroup.Element("TargetFramework")?.Value, Is.EqualTo("net10.0"));
                Assert.That(propertyGroup.Element("ImplicitUsings")?.Value, Is.EqualTo("disable"));
                Assert.That(propertyGroup.Element("Nullable")?.Value, Is.EqualTo("enable"));
                Assert.That(propertyGroup.Element("IsPackable")?.Value, Is.Not.EqualTo("false"));
            });
        }

        [Test]
        public void Given_ProductionProject_When_Inspected_Then_PackageReferencesAreAlphabeticallyOrdered()
        {
            XElement project = LoadProductionProject();
            string[] packageNames = project.Descendants("PackageReference")
                .Select(reference => (string?)reference.Attribute("Include"))
                .Where(name => name is not null)
                .Cast<string>()
                .ToArray();

            Assert.That(packageNames, Is.EqualTo(packageNames.OrderBy(name => name, StringComparer.Ordinal).ToArray()));
        }

        [Test]
        public void Given_ArchitectureProject_When_Inspected_Then_TestMetadataIsExplicit()
        {
            XElement project = LoadArchitectureProject();

            Assert.Multiple(() =>
            {
                Assert.That(project.Descendants("IsTestProject").Single().Value, Is.EqualTo("true"));
                Assert.That(project.Descendants("ExcludeFromCodeCoverage").Single().Value, Is.EqualTo("true"));
                Assert.That(project.Descendants("ImplicitUsings").Single().Value, Is.EqualTo("disable"));
                Assert.That(project.Descendants("Nullable").Single().Value, Is.EqualTo("enable"));
            });
        }

        private static XElement LoadArchitectureProject()
            => XElement.Load(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "NuciAPI.Middleware.ExceptionHandling.ArchitectureTests", "NuciAPI.Middleware.ExceptionHandling.ArchitectureTests.csproj"));

        private static XElement LoadProductionProject()
            => XElement.Load(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "NuciAPI.Middleware.ExceptionHandling", "NuciAPI.Middleware.ExceptionHandling.csproj"));
    }
}
