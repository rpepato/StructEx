﻿//The MIT License (MIT)

//Copyright (c) 2013 Roberto Pepato

//Permission is hereby granted, free of charge, to any person obtaining a copy of
//this software and associated documentation files (the "Software"), to deal in
//the Software without restriction, including without limitation the rights to
//use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//the Software, and to permit persons to whom the Software is furnished to do so,
//subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SharpTestsEx;

namespace StructEx.Tests
{
    [TestFixture]
    public class ProjectTests
    {
        private string _fixturesBasePath;

        [SetUp]
        public void SetUp()
        {
            var uri = new Uri(Assembly.GetAssembly(typeof(SolutionTests)).CodeBase);
            DirectoryInfo directory = new DirectoryInfo(uri.AbsolutePath);
            _fixturesBasePath = Path.Combine(directory.Parent.Parent.Parent.FullName, "SolutionFixtures");
        }

        [Test]
        public void ShouldReturnNoneProjectsForAnEmptySolution()
        {
            var solution = CreateSolutionFor("EmptySolution", "EmptySolution.sln");
            solution.Projects.Count.Should().Be.EqualTo(0);
        }

        [Test]
        public void ShouldReturnProjectNameForAOneProjectSolution()
        {
            var solution = CreateSolutionFor("SimpleSolution", "SimpleSolution.sln");
            solution.Projects.Count.Should().Be.EqualTo(1);
            solution.Projects.First().Title.Should().Be.EqualTo("FirstProject");
        }

        [Test]
        public void ShouldReturnProjectNamesForAMoreThanOneProjectSolution()
        {
            var solution = CreateSolutionFor("CompositeSolution", "CompositeSolution.sln");
            solution.Projects.Count.Should().Be.EqualTo(4);
            string[] projectNamePrefixes = { "First", "Second", "Third", "Fourth" };
            foreach (var projectNamePrefix in projectNamePrefixes)
            {
                solution.Projects.Any(prj => prj.Title == string.Format("{0}Project", projectNamePrefix)).Should().Be(true);
            }
        }

        [Test]
        public void ShouldReturnAssemblyNameForAGivenProject()
        {
            var solution = CreateSolutionFor("SimpleSolution", "SimpleSolution.sln");
            solution.Projects.First().AssemblyName.Should().Be.EqualTo("FirstProject");
        }

        [Test]
        public void ShouldReturnAssemblyNameForASetOfCSharpProjects()
        {
            var solution = CreateSolutionFor("CompositeSolution", "CompositeSolution.sln");
            var assemblies = solution.Projects.Select(prj => prj.AssemblyName);
            string[] assemblyNames = { "FirstProject", "SecondProject", "ThirdProject", "FourthProject" };
            assemblies.Should().Have.SameValuesAs(assemblyNames.AsEnumerable());
        }

        [Test]
        public void ShouldInspectAndReturnTheCompilerFlagsForACSharpProject()
        {
            var solution = CreateSolutionFor("SimpleSolution", "SimpleSolution.sln");
            var project = solution.Projects.First();
            project.CompilerSettings.AllowUnsafeBlocks.Should().Be.True();
            project.CompilerSettings.CheckForOverflow.Should().Be.True();
            project.CompilerSettings.ConditionalSymbols.Should().Have.SameValuesAs(new string[] { "DEBUG", "TRACE", "SOME", "CONDITIONAL", "SYMBOL" });
        }

        [Test]
        public void ShouldListFilesInProjectForASolutionWithOnlyOneProject()
        {
            var solution = CreateSolutionFor("SimpleSolution", "SimpleSolution.sln");
            var project = solution.Projects.First();
            project.Files.Count.Should().Be(2);
            string[] files = { "Class1", "AssemblyInfo" };
            foreach (var file in files)
            {
                project.Files.Any(f => f.Name.Contains(file)).Should().Be.True();
            }
        }

        [Test]
        public void ShouldListFilesInProjectForASolutionWithMultipleProjects()
        {
            var solution = CreateSolutionFor("CompositeSolution", "CompositeSolution.sln");
            var solutionFiles = solution.Projects.SelectMany(p => p.Files);
            solutionFiles.Count().Should().Be(21);
            solutionFiles.Select(a => a.Name).Where(a => a.EndsWith(".cs")).Count().Should().Be(21);
            var project = solution.Projects.First();
            string[] files = { "Class1", "AssemblyInfo", "Program", "Form1", "Form1.Designer", "Resources.Designer", "Settings.Designer", "AccountController", "HomeController", "Global.asax", "AccountModels" };
            foreach (var file in files)
            {
                solutionFiles.Any(f => f.Name.Contains(file)).Should().Be.True();
            }
        }

        [Test]
        public void ShouldListAssemblyReferencesOfACSharpProject()
        {
            var solution = CreateSolutionFor("SimpleSolution", "SimpleSolution.sln");
            var project = solution.Projects.First();
            project.ReferencedAssemblies.Count.Should().Be(8);
            string[] assemblies = {
                                "mscorlib", "System.Core", "Microsoft.CSharp", "System", "System.Data",
                                "System.Data.DataSetExtensions", "System.Xml", "System.Xml.Linq"
                            };
            foreach (var assembly in assemblies)
            {
                project.ReferencedAssemblies.Any(a => a.Contains(assembly)).Should().Be.True();
            }
        }

        [Test]
        public void ShouldListProjectReferencesOfACSharpProject()
        {
            var solution = CreateSolutionFor("CompositeSolution", "CompositeSolution.sln");
            var referencee = solution.Projects.First(prj => prj.FileName.Contains("SecondProject"));
            referencee.ReferencedProjects.Any(prj => prj.Contains("FirstProject")).Should().Be.True();
        }

        private Solution CreateSolutionFor(string solutionFolderName, string solutionFileName)
        {
            var solutionPath = Path.Combine(Path.Combine(_fixturesBasePath, solutionFolderName), solutionFileName);
            return new Solution(solutionPath);
        }
    }
}
