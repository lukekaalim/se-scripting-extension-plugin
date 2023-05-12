using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using VRage.Game;
using VRage.Utils;
using VRage.FileSystem;
using Sandbox.Game.World;

using System.IO;
using System.Text;

using ScriptingExtension.Patches;

using Semver;

namespace ScriptingExtension.ScriptModules {
  public class DependencyResolver
  {
    public Compiler compiler;
    public MyScriptManager scriptManager;
    public MySession session;

    IEnumerable<CompiledWorkshopDependency> FindWorkshopDependencies(
      IEnumerable<ScriptModule> modules
    ) {
      return modules
        .SelectMany(module => module.manifest.steamWorkshopDependencies)
        .Distinct()
        .Select(workshopDependency => {
          var mod = session.GetMod(workshopDependency.workshopId);
          var context = mod?.GetModContext() as MyModContext;
          var reference = scriptManager.GetScriptReference(context, workshopDependency.path);
          var assembly = scriptManager.GetScriptAssembly(context, workshopDependency.path);

          if (reference == null || mod == null || context == null || assembly == null)
            return null;

          return new CompiledWorkshopDependency() {
            reference = reference,
            assembly = assembly,

            workshopId = workshopDependency.workshopId,
            path = workshopDependency.path,
          };
        })
        .OfType<CompiledWorkshopDependency>();
    }

    public ScriptModuleResult[] Resolve(IEnumerable<ScriptModule> modules) {

      MyLog.Default.WriteLine("All Scripts");
      MyLog.Default.IncreaseIndent();
      MyLog.Default.WriteLine(string.Join("\n", scriptManager.Scripts.Values.Select(a => a.FullName)));
      MyLog.Default.DecreaseIndent();

      var workshopReferences = FindWorkshopDependencies(modules);

      return RecursiveResolveDependencies(
        modules.Select(module => new UncompiledScriptModule() { module = module }),
        new CompiledScriptModule[0],
        workshopReferences
      );
    }

    bool IsDependencySatisfied(Manifest.ModuleDependencyDeclaration declaration, Manifest.ModuleDeclaration module) {
      if (declaration.id != module.id)
        return false;
      return module.version.Satisfies(declaration.range);
    }

    private string GetAssemblyName(MyModContext mod, string scriptDir)
    {
      return mod?.ModId + "_" + scriptDir;
    }

    ScriptModuleResult[] RecursiveResolveDependencies(
      IEnumerable<UncompiledScriptModule> unresolvedModules,
      IEnumerable<CompiledScriptModule> resolvedModules,
      IEnumerable<CompiledWorkshopDependency> allWorkshopDependencies
    ) {
      ScriptModuleResult ResolveScriptModule(UncompiledScriptModule moduleToCompile) {
          if (moduleToCompile is ErrorUncompiledScriptModule)
            return moduleToCompile;

          var moduleDependencies = moduleToCompile.module.manifest
            .moduleDependencies
            .Select(dep => resolvedModules
              .FirstOrDefault(rm => IsDependencySatisfied(dep, rm.module.manifest.module)));
          
          var steamDependencies = moduleToCompile.module.manifest
            .steamWorkshopDependencies
            .Select(dep => allWorkshopDependencies
              .FirstOrDefault(compiledDep =>
                compiledDep.workshopId.Id == dep.workshopId.Id && compiledDep.path == dep.path));
          
          if (moduleDependencies.Any(d => d == null) || steamDependencies.Any(d => d == null))
            return moduleToCompile;

          var references = moduleDependencies
            .Select(md => md.reference)
            .Concat(steamDependencies.Select(sd => sd.reference));

          return compiler.Compile(moduleToCompile.module, references);
      }

      var modules = unresolvedModules
        .Select(ResolveScriptModule);

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
        resolvedModules.Concat(freshlyCompiledModules),
        allWorkshopDependencies
      );
    }
  }
}
