namespace StructEx
{
    public class File
    {
        public Project Project { get; protected set; }
        public string Name { get; protected set; }
        public string Content { get; protected set; }

        public File(Project project, string name)
        {
            Project = project;
            Name = name;
            Content = System.IO.File.ReadAllText(Name);
        }
    }
}
