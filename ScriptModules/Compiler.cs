using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VRage.Game;
using VRage.Utils;
using System.IO;
using System.Text;

namespace ScriptingExtension.ScriptModules {
  public class Compiler
  {
    public List<MetadataReference> StaticReferences;

    public ScriptModuleResult Compile(ScriptModule module, IEnumerable<MetadataReference> dependencies) {
      var trees = module.files.Select(file => file.Tree);
      var references = dependencies
        .Concat(StaticReferences)
        .ToArray();
      MyLog.Default.WriteLine($"Compiling {module.AssemblyName} with {string.Join(",", references.Select(r => r.Display))}");

      var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
      var compilation = CSharpCompilation.Create(module.AssemblyName, trees, references, options);

      using (MemoryStream documentationStream = new MemoryStream())
      using (MemoryStream assemblyStream = new MemoryStream()) {
        var emitResult = compilation.Emit(assemblyStream, documentationStream);

        if (!emitResult.Success)
          return new ErrorUncompiledScriptModule() {
            module = module,
            diagnostics = emitResult.Diagnostics.ToArray()
          };

        var assemblyBytes = assemblyStream.ToArray();
        var documentationBytes = documentationStream.ToArray();
        
        var assembly = Assembly.Load(assemblyBytes, documentationBytes);
        var reference = MetadataReference.CreateFromImage(assemblyBytes);

        return new CompiledScriptModule() {
          assembly = assembly,
          reference = reference,
          module = module,
        };
      }
    }
  }
}
