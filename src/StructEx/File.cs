namespace StructEx
{
    public class File
    {
        public Project Project { get; private set; }
        public string Name { get; private set; }
        public string Content { get; private set; }

        public File(Project project, string name)
        {
            Project = project;
            Name = name;
            Content = System.IO.File.ReadAllText(Name);
        }
    }
}
