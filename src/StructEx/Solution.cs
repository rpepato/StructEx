//The MIT License (MIT)

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
