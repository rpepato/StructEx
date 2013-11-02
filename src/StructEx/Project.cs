using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using StructEx.MsBuildExtensions;

namespace StructEx
{
    public class Project
    {
        private IList<string> _referencedAssemblies;

        public Project(Solution solution, string title, string fileName)
        {
            AssembliesResolved = false;
            ReferencedAssemblies = new List<string>();
            CompilerSettings = new CompilerSettings();
            ReferencedProjects = new List<string>();
            Files = new List<File>();
            Solution = solution;
            Title = title;
            FileName = Path.GetFullPath(fileName);

            ProjectCollection.GlobalProjectCollection.UnloadAllProjects();
            MsBuildProject = new Microsoft.Build.Evaluation.Project(fileName);

            AssemblyName = MsBuildProject.GetPropertyValue("AssemblyName");
            CompilerSettings.AllowUnsafeBlocks = MsBuildProject.GetPropertyAsBoolean("AllowUnsafeBlocks");
            CompilerSettings.CheckForOverflow = MsBuildProject.GetPropertyAsBoolean("CheckForOverflowUnderflow");
            var defineConstants = MsBuildProject.GetPropertyValue("DefineConstants");

            foreach (string symbol in defineConstants.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                CompilerSettings.ConditionalSymbols.Add(symbol.Trim());
            }

            foreach (var sourceCodeFile in MsBuildProject.GetItems("Compile"))
            {
                Files.Add(new File(this, Path.Combine(MsBuildProject.DirectoryPath, sourceCodeFile.EvaluatedInclude)));
            }

            foreach (var projectReference in MsBuildProject.GetItems("ProjectReference"))
            {
                string referencedFileName = Path.GetFullPath(Path.Combine(MsBuildProject.DirectoryPath, projectReference.EvaluatedInclude));
                ReferencedProjects.Add(referencedFileName);
            }

        }

        private bool AssembliesResolved { get; set; }

        protected Microsoft.Build.Evaluation.Project MsBuildProject { get; private set; }

        public string Title { get; private set; }

        public string FileName { get; private set; }

        public Solution Solution { get; private set; }

        public string AssemblyName { get; private set; }

        public CompilerSettings CompilerSettings { get; private set; }

        public IList<File> Files { get; private set; }

        public IList<string> ReferencedProjects { get; private set; }

        public IList<string> ReferencedAssemblies
        {
            get
            {
                // Lazy evalution for assembly resolution, so we can speed up our tests that don't use external assemblies
                if (!AssembliesResolved)
                {
                    foreach (var assembly in ResolveAssemblyReferences(MsBuildProject))
                    {
                        _referencedAssemblies.Add(assembly);
                    }
                    AssembliesResolved = true;
                }
                return _referencedAssemblies;
            }
            private set { _referencedAssemblies = value; }
        }

        private IEnumerable<string> ResolveAssemblyReferences(Microsoft.Build.Evaluation.Project project)
        {
            var projectInstance = project.CreateProjectInstance();
            projectInstance.SetProperty("BuildingProject", "false");
            project.SetProperty("DesignTimeBuild", "true");

            projectInstance.Build("ResolveAssemblyReferences", new[] { new ConsoleLogger(LoggerVerbosity.Minimal) });
            var items = projectInstance.GetItems("_ResolveAssemblyReferenceResolvedFiles");
            return items.Select(i => Path.Combine(Path.GetDirectoryName(this.FileName), i.GetMetadataValue("Identity")));
        }
    }
}
