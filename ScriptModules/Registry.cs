using System;

using VRage.Plugins;
using VRage.Game;
using VRage.Utils;
using VRage.Scripting;
using VRage.Game.Entity;
using VRage.ObjectBuilders;
using VRage.Game.Components;
using Sandbox.Game.World;

using System.Linq;
using System.Reflection;

namespace ScriptingExtension.ScriptModules {

  public interface IRegistry {
    void RegisterAssemblies(Assembly[] assemblies);
    void DeregisterAssemblies(Assembly[] assemblies);
  }
  public class Registry: IRegistry
  {
    public void RegisterAssemblies(Assembly[] assemblies) {
      foreach (var assembly in assemblies) {
        try {
          MyObjectBuilderType.RegisterFromAssembly(assembly);
          MyComponentFactory.Static.RegisterFromAssembly(assembly);
          MyComponentTypeFactory.Static.RegisterFromAssembly(assembly);
          MyObjectBuilderSerializer.RegisterFromAssembly(assembly);
          MySession.Static.RegisterComponentsFromAssembly(assembly, modAssembly: true);
        } catch (Exception exception) {
          MyLog.Default.WriteLine($"Failed to register assembly {assembly.GetName()} due to {exception}");
        }
      }
      VRage.Game.Entity.UseObject.MyUseObjectFactory.RegisterAssemblyTypes(assemblies);
    }

    public void DeregisterAssemblies(Assembly[] assemblies) {
      foreach (var assembly in assemblies) {
        try {
          MyObjectBuilderType.UnregisterFromAssembly(assembly);
          MyComponentFactory.Static.UnregisterFromAssembly(assembly);
          MyComponentTypeFactory.Static.UnregisterFromAssembly(assembly);
          MyObjectBuilderSerializer.UnregisterFromAssembly(assembly);
          //MySession.Static.RegisterComponentsFromAssembly(assembly, modAssembly: true);
        }
        catch (Exception exception) {
          MyLog.Default.WriteLine($"Failed to unregister assembly {assembly.GetName()} due to {exception}");
        }
      }
      //VRage.Game.Entity.UseObject.MyUseObjectFactory.RegisterAssemblyTypes(assemblies);
    }
  }
}