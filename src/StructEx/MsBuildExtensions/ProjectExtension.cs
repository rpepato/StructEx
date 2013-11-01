using MSBuild = Microsoft.Build.Evaluation;

namespace StructEx.MsBuildExtensions
{
    public static class ProjectExtension
    {
        public static bool GetPropertyAsBoolean(this MSBuild.Project project, string propertyName)
        {
            var propertyValue = project.GetPropertyValue(propertyName);
            bool boolCastResult;
            return bool.TryParse(propertyValue, out boolCastResult) ? true : false;
        } 
    }
}
