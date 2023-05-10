using System.IO;
using System.Linq;
using System;

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text;

namespace ScriptingExtension.ScriptModules {
  public class Source
  {
    public string[] excludedDirectories;

    public ScriptFile[] FindSourceFiles(string directory) {
      return Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories)
        .Select(file => new FileInfo(file))
        .Where(file => !new Uri(directory)
            .MakeRelativeUri(new Uri(file.FullName))
            .ToString()
            .Split(Path.PathSeparator)
            .Any(segment => excludedDirectories.Any(dir => dir == segment)))
        .Select(f => new ScriptFile() {
          Filename = f.FullName,
          Contents = File.ReadAllText(f.FullName)
        })
        .ToArray();
    }
  }

  public class ScriptFile {
    public string Filename;
    public string Contents;

    public SyntaxTree Tree {
      get => CSharpSyntaxTree.ParseText(Contents, null, Filename, Encoding.UTF8);
    }
  }
}