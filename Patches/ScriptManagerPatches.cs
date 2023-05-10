using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using VRage;
using VRage.Game;
using VRage.FileSystem;

using Sandbox;
using Sandbox.Game.Screens;
using Sandbox.Engine.Utils;
using Sandbox.Game.World;
/*
namespace ScriptingExtension.Patches {
  public static  class ScriptManagerPatches
  {
    private static void LoadScripts(MyScriptManager __instance, string path, MyModContext mod = null)
    {
      if (!MyFakes.ENABLE_SCRIPTS)
      {
        return;
      }
      string text = Path.Combine(path, "Data", "Scripts");
      string[] array;
      try
      {
        array = MyFileSystem.GetFiles(text, "*.cs").ToArray();
      }
      catch (Exception)
      {
        MySandboxGame.Log.WriteLine("Failed to load scripts from: " + path);
        return;
      }
      if (array.Length == 0)
      {
        return;
      }
      if (!MyVRage.Platform.Scripting.IsRuntimeCompilationSupported)
      {
        throw new MyLoadingRuntimeCompilationNotSupportedException();
      }
      bool zipped = MyZipFileProvider.IsZipFile(path);
      string[] array2 = array.First().Split('\\');
      int num = text.Split('\\').Length;
      if (num >= array2.Length)
      {
        MySandboxGame.Log.WriteLine(string.Format("\nWARNING: Mod \"{0}\" contains misplaced .cs files ({2}). Scripts are supposed to be at {1}.\n", path, text, array.First()));
        return;
      }
      List<string> list = new List<string>();
      string text2 = array2[num];
      string[] array3 = array;
      foreach (string text3 in array3)
      {
        array2 = text3.Split('\\');
        if (!(array2[array2.Length - 1].Split('.').Last() != "cs"))
        {
          int num2 = Array.IndexOf(array2, "Scripts") + 1;
          if (array2[num2] == text2)
          {
            list.Add(text3);
            continue;
          }
          __instance.Compile(list, __instance.GetAssemblyName(mod, text2), zipped, mod);
          list.Clear();
          text2 = array2[num];
          list.Add(text3);
        }
      }
      __instance.Compile(list.ToArray(), Path.Combine(MyFileSystem.ModsPath, __instance.GetAssemblyName(mod, text2)), zipped, mod);
      list.Clear();
    }
  }
}
*/