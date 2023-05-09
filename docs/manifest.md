# module.manifest.xml

This document contains a brief description of all the valid elements a `module.manifest.xml` can contain. This file should be placed inside a subdirectory inside `Data/ScriptModules` in your mod, such as `Data/ScriptModules/MyCoolModule/module.manifest.xml`

The name of the subdirectory is not significant, it just deliminates different modules.

The root element of this file should be a Module element.

## Module

|Attributes|Requirement|Description|
|-|-|-|
|id|Required|A unique identifier across all installed mods.|
|version|_Optional_|A Semver Number describing the version of the mod.|
|language|_Optional_|One of "csharp" or "fsharp". Defaults to csharp.|

Module can contain the following as children:

 - ModuleDependency
 - SteamWorkshopDependency
 - PluginDependency
 - InGameScriptReference

## ModuleDependency

|Attributes|Requirement|Description|
|-|-|-|
|id|Required|The ID declared in the `module.manifest.xml` of the script module you wish to add as a dependency|
|version|_Optional_|A valid semver to match against|

## SteamWorkshopDependency

|Attributes|Requirement|Description|
|-|-|-|
|workshopId|Required|The ID declared in the mod.manifest.xml of the script module you wish to add as a dependency|
|path|_Optional_|The path, relative to `Data/Scripts` in the mod's directory for which assembly we should load|

A Steam Workshop Dependency works a little differently than a ModuleDependency - a steam workshop dependency cannot have it's own subdependencies, but it can be depended upon if it has any public classes.

This will use the assemblies generated from the default space engineers compiler - which means
each subfolder (and the root) count as seperate projects. The path variable can be defined to only
reference a specific folder, or it can be omitted to only import the root level.

You can define multiple SteamWorkshopDependency elements with the same workshopId and different paths.

## PluginDependency

|Attributes|Requirement|Description|
|-|-|-|
|id|Required|The plugin id (typically a github id), such as `lukekaalim/scriptingextensionplus`|

## InGameScriptReference

|Attributes|Requirement|Description|
|-|-|-|
|whitelist|_optional_|The plugin id (typically a github id), such as `lukekaalim/scriptingextensionplus`|

This element allows the assembly the be referenced from inside an ingame programming block.

## InGameScriptNamespace

|Attributes|Requirement|Description|
|-|-|-|
|scope|_Optional_||
|alias|_Optional_||

Adds a reference to automatic imports occuring inside a script.