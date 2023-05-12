using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

using HarmonyLib;

using VRage.Scripting;

namespace ScriptingExtension.Patches
{
  public static class ScriptCompilerPatches
  {
    public static Dictionary<string, CSharpCompilation> compilations = new Dictionary<string, CSharpCompilation>();

    public static void Patch(Harmony harmony)
    {
      MethodInfo createCompilation = typeof(MyScriptCompiler).GetMethod("CreateCompilation", AccessTools.all);
      MethodInfo postCreateCompilation = typeof(ScriptCompilerPatches).GetMethod("PostCreateCompilation", AccessTools.all);
      harmony.Patch(createCompilation, null, new HarmonyMethod(postCreateCompilation));
    }

    public static CSharpCompilation GetCompilationForAssemblyName(this MyScriptCompiler _, string assemblyName) {
      return compilations.GetValueSafe(assemblyName);
    }

    public static void PostCreateCompilation(string assemblyFileName, ref CSharpCompilation __result)
    {
      compilations.Add(assemblyFileName, __result);
    }
  }
}
