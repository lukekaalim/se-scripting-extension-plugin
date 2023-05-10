using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VRage.Game;
using System.IO;
using System.Text;
using Semver;

namespace ScriptingExtension.ScriptModules {
  public class DependencyResolver
  {
    public Compiler compiler;

    public ScriptModuleResult[] Resolve(IEnumerable<ScriptModule> modules) {
      return RecursiveResolveDependencies(
        modules.Select(module => new UncompiledScriptModule() { module = module }),
        new CompiledScriptModule[0]
      );
    }

    bool IsDependencySatisfied(Manifest.ModuleDependencyDeclaration declaration, Manifest.ModuleDeclaration module) {
      if (declaration.id != module.id)
        return false;
      return module.version.Satisfies(declaration.range);
    }

    ScriptModuleResult[] RecursiveResolveDependencies(
      IEnumerable<UncompiledScriptModule> unresolvedModules,
      IEnumerable<CompiledScriptModule> resolvedModules
    ) {
      // find all unresolved modules that have their dependencies met
      var modules = unresolvedModules
        .Select(moduleToCompile => {
          // dont bother if the reason for the uncompilation is an error
          if (moduleToCompile is ErrorUncompiledScriptModule)
            return moduleToCompile;

          var dependencies = moduleToCompile.module.manifest.moduleDependencies
            .Select(dep => resolvedModules
              .FirstOrDefault(rm => IsDependencySatisfied(dep, rm.module.manifest.module)));

          if (dependencies.Any(d => d == null))
            return moduleToCompile;

          return compiler.Compile(moduleToCompile.module, resolvedModules);
        });
      var freshlyCompiledModules = modules
        .OfType<CompiledScriptModule>();

      var remainingUncompiledModules = modules
        .OfType<UncompiledScriptModule>();

      if (remainingUncompiledModules.Count() == unresolvedModules.Count())
        return resolvedModules
          .Concat<ScriptModuleResult>(remainingUncompiledModules)
          .ToArray();

      return RecursiveResolveDependencies(
        remainingUncompiledModules,
        resolvedModules.Concat(freshlyCompiledModules)
      );
    }
  }
}
