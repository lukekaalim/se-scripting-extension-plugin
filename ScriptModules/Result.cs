using System.Reflection;
using Microsoft.CodeAnalysis;

using VRage.Game;

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

  public class CompiledWorkshopDependency {
    public WorkshopId workshopId;
    public string path;
    
    public MetadataReference reference;
    public Assembly assembly;
  }
}