using VRage.Plugins;
using VRage.Game;
using VRage.Utils;
using VRage.Scripting;
using Sandbox.Game.World;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Linq;

using HarmonyLib;
using Microsoft.CodeAnalysis;

namespace ScriptingExtension
{
  public class Main : IPlugin
  {
    public static Main Plugin;

    public void Dispose()
    {
      Plugin = null;
      if (manager != null)
      {
        manager.UnloadMods();
        manager = null;
      }
      MyLog.Default.WriteLine("Disposed Scripting Extension Plugin");
      harmony.UnpatchAll();
    }
    readonly Harmony harmony = new Harmony("lkaalim.ScriptManagerExtended");

    public void Init(object gameInstance)
    {
      Plugin = this;
      manager = new ScriptModules.ScriptManager()
      {
        registry = new ScriptModules.Registry(),
      };
      Patches.ScriptCompilerPatches.Patch(harmony);
      Patches.ScriptManagerPatches.Patch(harmony);
      MyLog.Default.WriteLine("Initialized Scripting Extension Plugin");

      MethodInfo target = typeof(MyScriptManager).GetMethod("LoadData", BindingFlags.Instance | BindingFlags.Public);
      MethodInfo patch = typeof(Main).GetMethod("LoadData", BindingFlags.Static | BindingFlags.Public);
      harmony.Patch(target, null, new HarmonyMethod(patch));
    }

    public void Update()
    {

    }


    static ScriptModules.ScriptManager manager;
    public static void LoadData()
    {
      MyLog.Default.WriteLine("Intercepting Load Data");
      FieldInfo metadataReferencesField = typeof(MyScriptCompiler)
          .GetField("m_metadataReferences", AccessTools.all);

      var staticReferences = metadataReferencesField
          .GetValue(MyScriptCompiler.Static) as List<MetadataReference>;

      var resolver = new ScriptModules.DependencyResolver() {
        compiler = new ScriptModules.Compiler() {
          StaticReferences = staticReferences
        },
        scriptManager = MyScriptManager.Static,
        session = MySession.Static,
      };
        
      MyLog.Default.WriteLine("Assigning static references:");
      MyLog.Default.IncreaseIndent();
      foreach (var reference in resolver.compiler.StaticReferences)
        MyLog.Default.WriteLine($"{reference.Display}");
      MyLog.Default.DecreaseIndent();

      var mods = MySession.Static.Mods
          .Select(m => m.GetModContext())
          .OfType<MyModContext>()
          .ToArray();

      MyLog.Default.WriteLine("Loading mods");
      MyLog.Default.IncreaseIndent();
      foreach (var mod in mods)
        MyLog.Default.WriteLine($"{mod.ModName}");
      MyLog.Default.DecreaseIndent();

      manager.LoadMods(resolver, mods);
    }

    public static void LoadScriptModules()
    {

    }
  }
}
