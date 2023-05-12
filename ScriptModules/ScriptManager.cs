using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using VRage.Game;
using VRage.Utils;

namespace ScriptingExtension.ScriptModules
{
  public class ScriptManager
  {
    public IRegistry registry;

    public CompiledScriptModule[] compiledModules = new CompiledScriptModule[] { };
    public UncompiledScriptModule[] uncompiledModules = new UncompiledScriptModule[] { };

    public void LoadMods(DependencyResolver resolver, MyModContext[] mods)
    {
      var modules = mods.SelectMany(LoadMod).ToArray();
      MyLog.Default.WriteLine($"Found {modules.Count()} modules");

      var results = resolver.Resolve(modules);
      MyLog.Default.WriteLine($"Completed Resolution");

      compiledModules = results
        .OfType<CompiledScriptModule>()
        .ToArray();
      uncompiledModules = results
        .OfType<UncompiledScriptModule>()
        .ToArray();

      MyLog.Default.WriteLine($"Compiled {compiledModules.Count()} modules");
      MyLog.Default.IncreaseIndent();
      MyLog.Default.WriteLine(string.Join("\n", compiledModules.Select(m => m.module.manifest.module.id)));
      MyLog.Default.DecreaseIndent();
      MyLog.Default.WriteLine($"Failed to compile {uncompiledModules.Count()} modules");
      MyLog.Default.IncreaseIndent();
      foreach (var uncompiled in uncompiledModules) {
        MyLog.Default.WriteLine($"{uncompiled.module.manifest.module.id}");
        if (uncompiled is ErrorUncompiledScriptModule error) {
          MyLog.Default.IncreaseIndent();
          foreach (var diognostic in error.diagnostics)
            MyLog.Default.WriteLine(diognostic.ToString());
          MyLog.Default.DecreaseIndent();
        }
      }
      MyLog.Default.DecreaseIndent();
      var assemblies = compiledModules.Select(c => c.assembly).ToArray();
      registry.RegisterAssemblies(assemblies);
    }

    public ScriptModule[] LoadMod(MyModContext context)
    {
      try {
        MyLog.Default.WriteLine($"Seaching mod ${context.ModName}");
        var modulesRoot = Path.Combine(context.ModPath, "Data/ScriptModules");
        return Directory.GetDirectories(modulesRoot)
          .Distinct()
          .Select(d => { MyLog.Default.WriteLine($"Seaching directory ${d}"); return d; })
          .Select(moduleDirectory => LoadModule(context, moduleDirectory))
          .OfType<ScriptModule>()
          .ToArray();
      } catch (Exception) {
        return new ScriptModule[] {};
      }
    }

    public ScriptModule LoadModule(MyModContext context, string directory)
    {
      try
      {
        MyLog.Default.WriteLine($"Loading modile {context.ModName} {directory}");
        var manifestPath = Path.Combine(directory, "module.manifest.xml");

        var manifest = Manifest.LoadFromFile(manifestPath);
        var source = new Source()
        {
          excludedDirectories = new string[] { "bin", "obj" }
        };
        var files = source.FindSourceFiles(directory);

        return new ScriptModule()
        {
          context = context,
          files = files,
          manifest = manifest,
        };
      }
      catch (Exception exception)
      {
        MyLog.Default.WriteLine($"Discarding Module in {directory} because {exception}");
        return null;
      }
    }

    public void UnloadMods()
    {
      var assemblies = compiledModules.Select(c => c.assembly).ToArray();
      registry.DeregisterAssemblies(assemblies);
      compiledModules = new CompiledScriptModule[] { };
    }
  }
}
