using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VRage.Game;
using System.IO;
using System.Text;

namespace ScriptingExtension.ScriptModules {
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