using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using SharpTestsEx;

namespace StructEx.Tests
{
    [TestFixture]
    public class SolutionTests
    {
        private string _fixturesBasePath;

        [SetUp]
        public void SetUp()
        {
            var uri = new Uri(Assembly.GetAssembly(typeof(SolutionTests)).CodeBase);
            var directory = new DirectoryInfo(uri.AbsolutePath);
            _fixturesBasePath = Path.Combine(directory.Parent.Parent.Parent.FullName, "SolutionFixtures");
        }

        [Test]
        public void ShouldReturnSolutionNameForSimpleSolution()
        {
            var solutionPath = Path.Combine(Path.Combine(_fixturesBasePath, "SimpleSolution"), "SimpleSolution.sln");
            var solution = new Solution(solutionPath);
            solution.Name.Should().Be("SimpleSolution");
        }

        [Test]
        public void ShouldReturnSolutionNameACompositeSolution()
        {
            var solutionPath = Path.Combine(Path.Combine(_fixturesBasePath, "CompositeSolution"), "CompositeSolution.sln");
            var solution = new Solution(solutionPath);
            solution.Name.Should().Be("CompositeSolution");
        }
    }
}
