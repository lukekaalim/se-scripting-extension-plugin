# Script Modules

This Plugin enabled an alternative way of loading scripts from mods,
called "Script Modules".

## Usages
Place you scripts inside an alternative directory in your
Data directory - instead of "Data/Scripts" put it inside 
"Data/ScriptModules/$MyModuleName" (no name restriction on MyModuleName); 

In addition, you need to create a mod.manifest.xml file
in that directory, conforming to the following structure:

```xml
<?xml version="1.0"?>
<Mod id="lkaalim.mods.mymodname" version="1.0.0">
  <ModDependency id="lkaalim.mods.myothermod" version="2.0.0" />
  <SteamWorkshopDependency workshopId="542345" />
  <PluginDependency id="lkaalim.mods.myplugin"/>

  <InGameScriptNamespace scope="lkaalim.mod.mymodname.ingame" static alias="mymodname" />
</Mod>
```

```xml
<InGameScript id="lkaalim.ingamescript.myscript" version="1.0.0">
  <Title>In Game Script</Title>
  <ModDependency id="lkaalim.mods.myothermod" version="2.0.0" />
</InGameScript>
```