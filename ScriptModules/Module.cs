using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VRage.Game;
using System.IO;
using System.Text;

namespace ScriptingExtension.ScriptModules {

  public class ScriptModule {
    public Manifest manifest;
    public ScriptFile[] files;
    public MyModContext context;

    public string AssemblyName => $"ScriptManagerExtended.ScriptModule.{manifest.module.id}";
  }
}
