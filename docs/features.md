# Features

## Alternative Compiler

Inside a script module, a slightly different compiler
setup is used. This has some benefits such as:
  - **Folder support**. Code can now be placed inside folders without them being considered seperate assemblies.

## Script Module API
Read-only structures of this API are available for Mods to read
to learn how their own dependencies were resolved.

`SciprtingExtension.ScriptModules.ModAPI`

For other plugins, the more direct

`SciprtingExtension.ScriptModules`

provides classes and static fields to load, unload, and
inspect script modules.

## Dependency Resolution

When a script module is provided but has unmet dependencies,
it is not loaded. Since any single mod can provide multiple
script modules, you can define some core functionality with only
required dependencies and define additional modules for
each optional dependency.

In these additional modules, you can depend on your optional
dependency and your core module to extend your functionality,
and this module will automatically be disabled if the optional
dependency is not met.

> **Cylic Dependencies are not supported**.
>
> If a module depends on a module that depends on it
> (or any circular configuration),
> neither module can be resolved and they will always be
> disabled.


## Script Module Dependencies

### Ids

Script Module Ids are global across an install of Space Engineers:
when you declare a script module dependency, the script manager
searches across all installed mods to find the "first" declaration
of that script module that "meets" its requirements.

A script module ID will also be used to generate the assembly name, so there are some name restrictions.

Script module ids should look similar to namespaces according to convention. Ids are case sensitive. Here are some sample module ids:
 - `lukekaalim.mods.regiongeneration`
 - `common.serialcommunication`
 - `debug.drawingapi.modapi`

If no script module is present that contains that Id, the dependency
is considered unmet.

Multiple Script Modules can declare the same Id, even across different modules. Ensure you name your Id unique and personal to you to avoid accidental clashes.

### Version

Script Module Dependencies can be declared with a "version", which is a SemVer range identifiying a set of valid versions that would be acceptable. If a script module's version does not meet a dependencies version range, it is considered unmet.

You can define multiple Script Modules with the same Id, but with unique versions. This can mean you can continue to support older versions of your API will simulatously making newer versions available in the same mod. You can use this mechanism to retroactivley shim older methods into calling your newer API by declaring a dependency on your most-recent version, so you only need to maintain a single implementation.

### Priority

If multiple Script Modules meet a Dependencies's requirements, their Priority will define which module is loaded.

> **Intercepting Modules**
>
> If, for some reason, you want to force your own module to overrite
> another's with the same Id, increasing your module's priority
> above the other's will cause all references to resolve to your
> module. Note that you cannot access the "overwritten" module, for instance if you wanted to enhance it with additional features.
>
> This mechanism is meant to be available to temporarily replace
> broken modules, and is unsuited for other use cases.

### Aquisition

There is no provision for automatically download missing dependencies: you should ensure that there is an external mechanism that
can retrieve mods that your mod depends on (i.e. steam workshop, a readme, angry github issue, etc).

### Errors

Even if a module meets a depndencies requirements, it may fail to load
due to a compilation/syntax issue or some other runtime issue, causing
it to be considered unmet.

## Workshop Dependencies

## Plugin Dependencies

## IngameScript Namespaces

## Fallback

When at least one script module is detected in a mod,
the regular code inside `Data/Scripts` is ignored and not loaded.

That way, when a user downloads a mod that utitlizes
script modules, but doesnt have the Scripting Extension Plugin
installed, you can gracefully fallback to previous behaviour.

## Backwards Compatability

If you published a mod that is used as a Workshop Dependency
by other mods, and you with to add ScriptModule support, you can
add a `<Compatability workshopDefault="true" />` block to your Module
manifest.

Instead of resolving to the (now disabled, see "Fallback")
Scripts directory, WorkshopDependencies will resolve according to
workshopPath or workshopDefault properties of Compatability, with the
"default" referenced assembly being the root level code.

## Known Issues

- [ ] Mod script performance metrics are not working.
- [ ] Mod syntax blacklisting/type whitelisting is not working. (Security sandbox broken)
- [ ] Doesnt work for Mod.IO (zipped mods)
- [ ] Doesnt respect any flag that disabled mods or scripting
- [ ] Everything is delciously synchronous