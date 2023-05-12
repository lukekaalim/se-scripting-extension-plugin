using System.Xml;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using VRage.Game;

using Semver;

namespace ScriptingExtension.ScriptModules
{
  public static class MyExtensions
  {
    public static string Or(this string str, string fallback)
    {
      if (str == null || str == "")
        return fallback;
      return str;
    }
  }

  public class Manifest
  {
    public ModuleDeclaration module;
    public class ModuleDeclaration
    {
      public string id;
      public SemVersion version;

      public static ModuleDeclaration ReadFromXML(XmlElement xml)
      {
        var module = new ModuleDeclaration() {
          id =      xml.GetAttribute("id"),
          version = SemVersion.Parse(
            xml.GetAttribute("version").Or("0.0.0"),
            SemVersionStyles.Strict
          )
        };
        if (module.id == "")
          throw new System.Exception("Module is missing required attribute: \"id\"");
        return module;
      }
    }

    public ModuleDependencyDeclaration[] moduleDependencies;
    public class ModuleDependencyDeclaration
    {
      public string id;
      public SemVersionRange range;

      public static ModuleDependencyDeclaration ReadFromXML(XmlElement xml)
      {
        var declaration = new ModuleDependencyDeclaration() {
          id =      xml.GetAttribute("id"),
          range = SemVersionRange.Parse(xml.GetAttribute("version").Or("*"))
        };
        if (declaration.id == "")
          throw new System.Exception("Dependency is missing required attribute: \"id\"");
        return declaration;
      }
    }

    public SteamWorkshopDependencyDeclaration[] steamWorkshopDependencies;
    public class SteamWorkshopDependencyDeclaration
    {
      public WorkshopId workshopId;
      public string path;

      public static SteamWorkshopDependencyDeclaration ReadFromXML(XmlElement xml)
      {
        return new SteamWorkshopDependencyDeclaration() {
          workshopId = new WorkshopId(ulong.Parse(xml.GetAttribute("workshopId")), "steam"),
          path = xml.GetAttribute("path") ?? null,
        };
      }
      public override bool Equals(object obj)
      {
        if (!(obj is SteamWorkshopDependencyDeclaration dep))
          return false;
        return dep.path == path && dep.workshopId.Id == workshopId.Id;
      }
      
      public override int GetHashCode()
      {
        return (path + workshopId.Id.ToString()).GetHashCode();
      }
    }


    public static Manifest ReadFromXML(XmlDocument xml)
    {
      var modElement = xml.ChildNodes.OfType<XmlElement>()
          .FirstOrDefault(node => node.Name == "Module") ?? throw new System.Exception("Missing top level \"Module\" element");

      var module = ModuleDeclaration.ReadFromXML(modElement);

      var moduleDependencies = modElement.ChildNodes.OfType<XmlElement>()
          .Where(node => node.Name == "ModuleDependency")
          .Select(node => ModuleDependencyDeclaration.ReadFromXML(node))
          .ToArray();
      var steamWorkshopDependencies = modElement.ChildNodes.OfType<XmlElement>()
          .Where(node => node.Name == "SteamWorkshopDependency")
          .Select(node => SteamWorkshopDependencyDeclaration.ReadFromXML(node))
          .ToArray();

      return new Manifest() {
        module = module,
        moduleDependencies = moduleDependencies,
        steamWorkshopDependencies = steamWorkshopDependencies
      };
    }

    public static Manifest LoadFromFile(string filename)
    {
      var xml = new XmlDocument();
      xml.Load(filename);
      return ReadFromXML(xml);
    }
  }
}

