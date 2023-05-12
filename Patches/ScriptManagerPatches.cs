using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using VRage;
using VRage.Utils;
using VRage.Game;
using VRage.FileSystem;
using VRage.Scripting;

using Sandbox;
using Sandbox.Game.Screens;
using Sandbox.Engine.Utils;
using Sandbox.Game.World;

using HarmonyLib;

namespace ScriptingExtension.Patches {
  public static class ScriptManagerPatches
  {
    public static void Patch(Harmony harmony) {
      MethodInfo loadScripts = typeof(MyScriptManager).GetMethod("LoadScripts", AccessTools.all);
      MethodInfo preLoadScripts = typeof(ScriptManagerPatches).GetMethod("PreLoadScripts", AccessTools.all);
      harmony.Patch(loadScripts, new HarmonyMethod(preLoadScripts));

      MethodInfo compile = typeof(MyScriptManager).GetMethod("Compile", AccessTools.all);
      MethodInfo preCompile = typeof(ScriptManagerPatches).GetMethod("PreCompile", AccessTools.all);
      harmony.Patch(compile, new HarmonyMethod(preCompile));

      MethodInfo addAssembly = typeof(MyScriptManager).GetMethod("AddAssembly", AccessTools.all);
      MethodInfo preAddAssembly = typeof(ScriptManagerPatches).GetMethod("PreAddAssembly", AccessTools.all);
      harmony.Patch(addAssembly, new HarmonyMethod(preAddAssembly));

      MyLog.Default.WriteLine($"Patching Script Manager");
    }

    public static string GetModScriptAssemblyName(this MyScriptManager _, MyModContext mod, string scriptPath) {
      if (scriptPath.EndsWith(".cs")) {
        // single script
        return GetAssemblyName(mod, scriptPath);
      } else {
        return Path.Combine(MyFileSystem.ModsPath, GetAssemblyName(mod, scriptPath));
      }
    }

    public static MetadataReference GetScriptReference(this MyScriptManager manager, MyModContext mod, string scriptPath) {
      var name = manager.GetModScriptAssemblyName(mod, scriptPath);
      var compilation = MyScriptCompiler.Static.GetCompilationForAssemblyName(name);
      return compilation.ToMetadataReference();
    }
    public static Assembly GetScriptAssembly(this MyScriptManager manager, MyModContext mod, string scriptPath) {
      var id = MyStringId.GetOrCompute(manager.GetModScriptAssemblyName(mod, scriptPath));
      return manager.Scripts.GetValueSafe(id);
    }

    public static void PreLoadScripts(MyScriptManager __instance, string path, MyModContext mod = null)
    {
      MyLog.Default.WriteLine($"Loading Script {mod?.ModId} {mod.ModServiceName} {mod.ModName} {mod?.ModPath} {path}");
    }

    public static void PreCompile(MyScriptManager __instance, string assemblyName, MyModContext context)
    {
      var id = MyStringId.GetOrCompute(assemblyName);
      MyLog.Default.WriteLine($"Compiling Assembly {context.ModId} {assemblyName}");
      MyLog.Default.WriteLine($"Guessing id {id}");
    }

    public static void PreAddAssembly(MyModContext context, MyStringId myStringId, Assembly assembly)
    {
      MyLog.Default.WriteLine($"Adding Assembly {myStringId} {assembly.FullName}");
    }

    static string GetAssemblyName(MyModContext mod, string scriptDir)
    {
      return mod?.ModId + "_" + scriptDir;
    }
  }
}
