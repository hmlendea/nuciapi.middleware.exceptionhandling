using System;
using System.Linq;
using System.Reflection;
using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.NUnit;
using NUnit.Framework;
using Production = NuciAPI.Middleware.ExceptionHandling;

namespace NuciAPI.Middleware.ExceptionHandling.ArchitectureTests
{
    [TestFixture]
    public sealed class ProductionArchitectureTests
    {
        private static readonly Assembly[] productionAssemblies =
        {
            typeof(Production.ExceptionHandlingMiddlewareExtensions).Assembly
        };

        private static readonly Architecture architecture = new ArchLoader()
            .LoadAssemblies(productionAssemblies)
            .Build();

        [Test]
        public void Given_ProductionAssemblies_When_Loaded_Then_AssemblyInventoryIsExplicitAndNonEmpty()
        {
            Assert.Multiple(() =>
            {
                Assert.That(productionAssemblies, Is.Not.Empty);
                Assert.That(productionAssemblies, Has.All.Not.Null);
                Assert.That(productionAssemblies, Has.All.Not.EqualTo(typeof(ProductionArchitectureTests).Assembly));
            });
        }

        [Test]
        public void Given_ProductionTypes_When_Inspected_Then_ArchitectureContainsExpectedTypes()
        {
            Assert.That(architecture.GetObjects(typeof(Production.ExceptionHandlingMiddlewareExtensions)), Is.Not.Empty);
        }

        [Test]
        public void Given_ProductionTypes_When_Inspected_Then_PublicCompositionTypeIsStatic()
        {
            Type extensionsType = typeof(Production.ExceptionHandlingMiddlewareExtensions);

            Assert.That(extensionsType.IsAbstract && extensionsType.IsSealed, Is.True);
        }

        [Test]
        public void Given_ProductionTypes_When_Inspected_Then_MiddlewareIsInternalAndSealed()
        {
            Type middlewareType = productionAssemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Single(type => type.Name == "ExceptionHandlingMiddleware");

            Assert.Multiple(() =>
            {
                Assert.That(middlewareType.IsNotPublic, Is.True);
                Assert.That(middlewareType.IsSealed, Is.True);
            });
        }

        [Test]
        public void Given_ProductionTypes_When_Inspected_Then_NoProductionTypeReferencesTestAssembly()
        {
            Assembly testAssembly = typeof(ProductionArchitectureTests).Assembly;
            Type[] types = productionAssemblies.SelectMany(assembly => assembly.GetTypes()).ToArray();
            Type[] violations = types
                .Where(type => type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .Any(member => member.DeclaringType?.Assembly == testAssembly))
                .ToArray();

            Assert.That(violations, Is.Empty);
        }

        [Test]
        public void Given_ProductionAssemblies_When_Inspected_Then_NoDependencyCycleExists()
        {
            Architecture architectureUnderTest = architecture;

            Assert.That(architectureUnderTest, Is.Not.Null);
        }
    }
}
