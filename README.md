StructEx
========

Have you ever needed to programmatically inspect the contents of a .net solution and look on its projects, its files and the references of each project for other projects and external assemblies? 

StructEx is a small .net library that exposes a DSL to inspect a .net solution and its contents.

## Why StructEx?

StructEx can be used or extended on scenarios that you need to deal with .net source code. Some of these scenareios are: (very) basic foundation for a static code analysis tool, inspection of project contents to automatically configure build tasks, configure Continous Integration environment dynamically by selecting which files to include or compile, etc.

## Usage

The usage of StructEx is straightforward. Just create a solution object by passing it the solution file path and inspect its contents:

```csharp

using StructEx;

//...

var solution = new Solution(@"C:\work\SomeSolution.sln")

// inspect the solutions projects
foreach(var project in solution.Projects)
{
  var projectName = project.Name;
  var projectPath = project.FileName;
  var assemblyName = project.AssemblyName;
  
  // inspect referenced assemblies
  foreach(var assembly in project.ReferencedAssemblies)
  {
    var assemblyPath = assembly;
  }
  
  // inspect another projects from the solution and referenced by this project
  foreach(var referencedProject in project.ReferencedProjects)
  {
    // ... do some stuff with referenced projects
  }
  
  // inspect the files on the current project
  foreach(var file in project.Files)
  {
    var fileName = file.Name;
    var fileContent = file.Content;
    
    // ...
  }
  
}

// ...

```

## Limitations

The current version only works with C# projects (including class libraries, console, windows forms, asp.net mvc, and other project types)

## Inspiration and References

This project is largely inspired on the [work](http://www.codeproject.com/Articles/408663/Using-NRefactory-for-analyzing-Csharp-code) and samples provided by [Daniel Grunwald](http://www.danielgrunwald.de/). Please, check the original article for more information.
