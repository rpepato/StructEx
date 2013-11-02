using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace StructEx
{
    public class Solution
    {
        private const string MSBUILDPROJECTPATTERN =
            "Project\\(\"(?<TypeGuid>.*)\"\\)\\s+=\\s+\"(?<Title>.*)\",\\s*\"(?<Location>.*)\",\\s*\"(?<Guid>.*)\"";
        private readonly Regex _msbuildProjectDefinitionPattern = new Regex(MSBUILDPROJECTPATTERN, RegexOptions.Compiled);
        private const string CSHARPPROJECTIDENTIFIER = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

        protected Solution()
        {
            
        }

        public Solution(string path)
        {
            Projects = new List<Project>();
            if (System.IO.File.Exists(path))
            {
                Path = path;
                foreach (var line in System.IO.File.ReadLines(Path))
                {
                    var match = _msbuildProjectDefinitionPattern.Match(line);
                    if (match.Success)
                    {
                        if (match.Groups["TypeGuid"].Value.ToUpperInvariant().Equals(CSHARPPROJECTIDENTIFIER))
                        {
                            Projects.Add(new Project(this,
                                                            match.Groups["Title"].Value,
                                                            System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path),
                                                                         match.Groups["Location"].Value)));
                        }
                    }
                }
            }
            else
            {
                throw new System.IO.FileNotFoundException("Solution file wasn't found on the specified path", Path);
            }
        }

        protected Regex MSBuildProjectDefinitionPattern { get { return _msbuildProjectDefinitionPattern; }}

        protected string CSharpProjectIdentifier { get { return CSHARPPROJECTIDENTIFIER; } }

        public string Path { get; protected set; }

        public IList<Project> Projects { get; protected set; }

        public string Name
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(Path); }
        }
    }
}
