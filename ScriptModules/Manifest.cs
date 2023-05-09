using System.Xml;
using System.Linq;
using Semver;

namespace ScriptManagerExtended.ScriptModules {
  public class Manifest
  {
    public ModuleDeclaration module;
    public class ModuleDeclaration {
      public string id;
      public SemVersion version;

      public static ModuleDeclaration ReadFromXML(XmlElement xml) {
        var id = xml.GetAttribute("id");
        if (id == "")
          throw new System.Exception("Module is missing required attribute: \"id\"");

        var versionAttribute = xml.GetAttribute("version");
        versionAttribute = versionAttribute == "" ? "0.0.0" : versionAttribute;

        var version = SemVersion.Parse(versionAttribute, SemVersionStyles.Strict);
        return new ModuleDeclaration() { id = id, version = version };
      }
    }
    
    public ModuleDependencyDeclaration[] moduleDependencies;
    public class ModuleDependencyDeclaration {
      public string id;
      public SemVersionRange range;

      public static ModuleDependencyDeclaration ReadFromXML(XmlElement xml) {
        var id = xml.GetAttribute("id");
        if (id == "")
          throw new System.Exception("Module is missing required attribute: \"id\"");

        var versionAttribute = xml.GetAttribute("version");
        versionAttribute = versionAttribute == "" ? "*" : versionAttribute;

        var range = SemVersionRange.Parse(versionAttribute);
        return new ModuleDependencyDeclaration() { id = id, range = range };
      }
    }

    public SteamWorkshopDependencyDeclaration[] steamWorkshopDependencies;
    public class SteamWorkshopDependencyDeclaration {

    }


    public static Manifest ReadFromXML(XmlDocument xml) {
      var modElement = xml.ChildNodes.OfType<XmlElement>()
          .FirstOrDefault(node => node.Name == "Module") ?? throw new System.Exception("Missing top level \"Module\" element");

      var module = ModuleDeclaration.ReadFromXML(modElement);
      
      var moduleDependencies = modElement.ChildNodes.OfType<XmlElement>()
          .Where(node => node.Name == "ModuleDependency")
          .Select(node => ModuleDependencyDeclaration.ReadFromXML(node))
          .ToArray();

      return new Manifest() { module = module, moduleDependencies = moduleDependencies };
    }

    public static Manifest LoadFromFile(string filename) {
      var xml = new XmlDocument();
      xml.Load(filename);
      return ReadFromXML(xml);
    }
  }
}

