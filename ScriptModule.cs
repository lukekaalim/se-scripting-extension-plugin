using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using VRage.Scripting;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.Utils;
using System.IO;
using System.Text;
using System;


namespace lkaalim.ScriptManagerExtended
{

  public class ScriptModuleCompiler {
    public List<MetadataReference> StaticReferences;

    public ScriptModuleResult Compile(ScriptModule module, IEnumerable<CompiledScriptModule> dependencies) {
      var trees = module.files.Select(file => file.Tree);
      var references = dependencies
        .Select(dep => dep.reference)
        .Concat(StaticReferences);
      var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
      var compilation = CSharpCompilation.Create(module.id, trees, references, options);

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

  public class ScriptFile {
    public string Filename;
    public string Contents;

    public SyntaxTree Tree {
      get => CSharpSyntaxTree.ParseText(Contents, null, Filename, Encoding.UTF8);
    }
  }

  public class ScriptModule {

    public string id;
    public string[] dependencies;
    public ScriptFile[] files;
    public MyModContext context;

    public static ScriptModuleResult[] Build(IEnumerable<ScriptModule> modules, ScriptModuleCompiler compiler) {
      return Resolve(
        modules.Select(module => new UncompiledScriptModule() { module = module }),
        new CompiledScriptModule[0],
        compiler
      );
    }

    public static ScriptModuleResult[] Resolve(
      IEnumerable<UncompiledScriptModule> unresolvedModules,
      IEnumerable<CompiledScriptModule> resolvedModules,
      ScriptModuleCompiler compiler
    ) {
      // find all unresolved modules that have their dependencies met
      var modules = unresolvedModules
        .Select(uncompiled => {

          var dependencies = uncompiled.module.dependencies
            .Select(id => resolvedModules
              .FirstOrDefault(rm => rm.module.id == id));

          var unmetDependencies = uncompiled.module.dependencies
            .Where(d => dependencies.FirstOrDefault(de => de.module.id == d) == null);

          if (dependencies.Any(d => d == null))
            return uncompiled;

          return compiler.Compile(uncompiled.module, resolvedModules);
        });
      var freshlyCompiledModules = modules
        .Select(m => m as CompiledScriptModule)
        .Where(m => m != null);

      var remainingUncompiledModules = modules
        .Select(m => m as UncompiledScriptModule)
        .Where(m => m != null);

      if (remainingUncompiledModules.Count() == unresolvedModules.Count())
        return resolvedModules
          .Concat<ScriptModuleResult>(remainingUncompiledModules)
          .ToArray();

      return Resolve(
        remainingUncompiledModules,
        resolvedModules.Concat(freshlyCompiledModules),
        compiler
      );
    }
  }

  public class ScriptModuleResult {
    public ScriptModule module;
  }

  public class CompiledScriptModule : ScriptModuleResult {
    public Assembly assembly;
    public MetadataReference reference;
  }

  public class UncompiledScriptModule : ScriptModuleResult {
  }

  public class ErrorUncompiledScriptModule : UncompiledScriptModule {
    public Diagnostic[] diagnostics;
  }
}