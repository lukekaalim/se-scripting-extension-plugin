# Scripting Extension Plugin

This Plugin adds some new features for scripting in space engineers,
expanding InGameScripting and Mods.

## Features

- **Script Modules**. Reference other mods and their API's directly in your code.
  - IngameScript Namespaces. Add your mods's types for use with InGameScripts.

See more details in the [features document](docs/features.md)

## Usage
Place you scripts inside an alternative directory in your
Data directory - instead of "Data/Scripts" put it inside 
"Data/ScriptModules/MyCoolModule" (no name restriction on the subdirectory); 

In addition, you need to create a module.manifest.xml file
in that directory, conforming to the following structure:

```xml
<?xml version="1.0"?>
<Module id="lkaalim.mods.mymodname" version="1.0.0">
  <ModuleDependency id="lkaalim.mods.myothermod" version="2.0.0" />
  <ModuleDependency id="lkaalim.mods.anotherMod" />
  <SteamWorkshopDependency workshopId="542345" />
  <PluginDependency id="lkaalim/myplugin"/>

  <InGameScriptNamespace scope="lkaalim.mod.mymodname.ingame" />
  <Compatibility>
    <Workshop path="mymodname">
  </Compatibility>
</Mod>
```

See more details in the [manifest schema](docs/manifest.md)