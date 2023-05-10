using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using VRage.Game;
using VRage.Utils;

namespace ScriptingExtension.ScriptModules {
  public class ScriptManager
  {
    public DependencyResolver resolver;
    public Registry registry;

    public CompiledScriptModule[] compiledModules = new CompiledScriptModule[] {};

    public void LoadMods(MyModContext[] mods) {
      var modules = mods.SelectMany(LoadMod);
      var results = resolver.Resolve(modules);

      MyLog.Default.WriteLine($"Found {modules.Count()} modules");

      compiledModules = results
        .OfType<CompiledScriptModule>()
        .ToArray();

      MyLog.Default.WriteLine($"Compiled {compiledModules.Count()} modules");
      var assemblies = compiledModules.Select(c => c.assembly).ToArray();
      registry.RegisterAssemblies(assemblies);
    }

    public ScriptModule[] LoadMod(MyModContext context) {
      var modulesRoot = Path.Combine(context.ModPath, "Data/ScriptModules");
      return Directory.GetDirectories(modulesRoot)
        .Select(moduleDirectory => LoadModule(context, moduleDirectory))
        .OfType<ScriptModule>()
        .ToArray();
    }

    public ScriptModule LoadModule(MyModContext context, string directory) {
      try
      {
        var manifestPath = Path.Combine(directory, "module.manifest.xml");

        var manifest = Manifest.LoadFromFile(manifestPath);
        var source = new Source() {
            excludedDirectories = new string[] { "bin", "obj" }
        };
        var files = source.FindSourceFiles(directory);

        return new ScriptModule() {
            context = context,
            files = files,
            manifest = manifest,
        };
      } catch (Exception exception)
      {
        MyLog.Default.WriteLine($"Discarding Module in {directory} because {exception}");
        return null;
      }
    }

    public void UnloadMods() {
      var assemblies = compiledModules.Select(c => c.assembly).ToArray();
      registry.DeregisterAssemblies(assemblies);
      compiledModules = new CompiledScriptModule[] {};
    }
  }
}
