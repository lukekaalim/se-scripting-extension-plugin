# module.manifest.xml

|Schema Version|`0.2.0`|
|-|-|

This document contains a brief description of all the valid elements a `module.manifest.xml` can contain. This file should be placed inside a subdirectory inside `Data/ScriptModules` in your mod, such as `Data/ScriptModules/MyCoolModule/module.manifest.xml`

The name of the subdirectory is not significant, it just deliminates different modules.

The root element of this file should be a Module element.

## Module

|Attributes|Requirement|Description|
|-|-|-|
|id|Required|A unique identifier across all installed mods.|
|version|_Optional_|A Semver Number describing the version of the mod. Defaults to "0.0.0"|
|priority|_Optional_|Defaults to 1. Used to decide when multiple otherwise equally valid modules meet a dependency|

Module can contain the following as children:

 - Compatability
 - ModuleDependency
 - SteamWorkshopDependency
 - PluginDependency
 - InGameScriptReference

## Compatability

Compatability can contain the following as children:
  - Workshop

## Workshop
|Attributes|Requirement|Description|
|-|-|-|
|path|Required|Define the path this module represents if imported as a workshop dependency|

Multiple "Workshop" elements can be declared to satisfy multiple WorkshopDependencies with a single ScriptModule.

## ModuleDependency

|Attributes|Requirement|Description|
|-|-|-|
|id|Required|The ID declared in the `module.manifest.xml` of the script module you wish to add as a dependency|
|version|_Optional_|A valid semver range to match against|

## SteamWorkshopDependency

|Attributes|Requirement|Description|
|-|-|-|
|workshopId|Required|The ID declared in the mod.manifest.xml of the script module you wish to add as a dependency|
|path|Required|The path, relative to `Data/Scripts` in the mod's directory for which assembly we should load|

A Steam Workshop Dependency works a little differently than a ModuleDependency - a steam workshop dependency cannot have it's own subdependencies, but it can be depended upon if it has any public classes.

This will use the assemblies generated from the default space engineers compiler - which means
each subfolder (and the root) count as seperate projects. The path attribute identifies which subfolder to load assemblies from.

You can define multiple SteamWorkshopDependency elements with the same workshopId and different paths.

## PluginDependency

|Attributes|Requirement|Description|
|-|-|-|
|id|Required|The plugin id (typically a github id), such as `lukekaalim/scriptingextension|

## InGameScriptNamespace

|Attributes|Requirement|Description|
|-|-|-|
|scope|_Optional_||
|alias|_Optional_||

Adds a reference to automatic imports occuring inside a script.

One or more instances of this block adds this module as an assembly reference against all
programmable blocks.