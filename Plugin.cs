using VRage.Plugins;
using VRage.Game;
using VRage;
using VRage.Utils;
using VRage.Scripting;
using Sandbox.Game.World;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Linq;

using HarmonyLib;
using Microsoft.CodeAnalysis;
//using Sandbox.Game.World;
//using Sandbox.Game.Gui;

namespace lkaalim.ScriptManagerExtended
{
    public class Main : IPlugin
    {
        public void Dispose()
        {

        }

        public void Init(object gameInstance)
        {
            MyLog.Default.WriteLine("++++ LOUD SCREAMING ++++");
            Harmony harmony = new Harmony("lkaalim.ScriptManagerExtended");
            MethodInfo target = typeof(MyScriptManager).GetMethod("LoadData", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo patch = typeof(Main).GetMethod("LoadData", BindingFlags.Static | BindingFlags.Public);
            harmony.Patch(target, new HarmonyMethod(patch));
            PatchAddAssembly(harmony);
            MyLog.Default.WriteLine("++++ FINISHING LOUD SCREAMING ++++");
        }

        void PatchAddAssembly(Harmony harmony) {
            MethodInfo target = typeof(MyScriptCompiler).GetMethod("Compile", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo patch = typeof(Main).GetMethod("PatchedCompile", BindingFlags.Static | BindingFlags.Public);

            harmony.Patch(target, new HarmonyMethod(patch));

        }
        public static void PatchedCompile(MyScriptCompiler __instance) {
            FieldInfo field = typeof(MyScriptCompiler)
                .GetField("m_metadataReferences", AccessTools.all);
            //List<MetadataReference>
            MyLog.Default.WriteLine($"field {field}");
            var references = field.GetValue(__instance) as List<MetadataReference>;
            MyLog.Default.WriteLine($"references {references}");

            MyLog.Default.WriteLine("Adding Assemblies");
            MyLog.Default.WriteLine(string.Join("\n: ", references.Select(r => r.Display)));
        }

        public void Update()
        {

        }

        public static void LoadData() {
            FieldInfo metadataReferencesField = typeof(MyScriptCompiler)
                .GetField("m_metadataReferences", AccessTools.all);
            var metadataReferences = metadataReferencesField.GetValue(MyScriptCompiler.Static) as List<MetadataReference>;
            
            MyLog.Default.WriteLine("LOADING COOL MOD DATA");
            var modules = MySession.Static.Mods.SelectMany(mod => {
                var context = mod.GetModContext() as MyModContext;
                var scriptProjectRoot = Path.Combine(context.ModPath, "ScriptProject");
                MyLog.Default.WriteLine(string.Join("\nLOADING: ", Directory.GetDirectories(scriptProjectRoot).ToArray()));

                return Directory.GetDirectories(scriptProjectRoot).Select(directory => {
                    MyLog.Default.WriteLine("Loading Script Project");
                    MyLog.Default.WriteLine(directory);
                    try {
                        var directoryInfo = new DirectoryInfo(directory);
                        var doc = new XmlDocument();
                        var manifestPath = Path.Combine(directory, "mod.manifest.xml");
                        doc.Load(manifestPath);
                        
                        var modElement = doc.ChildNodes.OfType<XmlElement>()
                            .First(node => node.Name == "Mod");
                        var id = modElement.GetAttribute("id");

                        var dependencies = modElement.ChildNodes.OfType<XmlElement>()
                            .Where(node => node.Name == "Dependency")
                            .Select(node => node.GetAttribute("name"))
                            .ToArray();

                        var files = Directory.EnumerateFiles(directory, "*.cs", SearchOption.TopDirectoryOnly)
                            .Select(file => new ScriptFile() {
                                Filename = file,
                                Contents = File.ReadAllText(file),
                            })
                            .ToArray();

                        var module = new ScriptModule() {
                            context = context,
                            files = files,
                            id = id,
                            dependencies = dependencies,
                        };
                        MyLog.Default.WriteLine($"LOADED Module {module.id}");
                        MyLog.Default.WriteLine($"Dependencies: {string.Join("\n", dependencies)}");
                        MyLog.Default.WriteLine($"Files: {string.Join("\n", files.Select(f => f.Filename))}");
                        return module;
                    } catch (Exception exception) {
                        MyLog.Default.WriteLine("FAILED TO LOAD XML");
                        MyLog.Default.WriteLine(exception.ToString());
                        return null;
                    }
                });
            })
            .Where(m => m != null);

            var compiler = new ScriptModuleCompiler() {
                StaticReferences = metadataReferences
            };
            var compiledMods = ScriptModule.Build(modules, compiler)
                .Select(mod => mod as CompiledScriptModule)
                .Where(mod => mod != null);

            foreach (var compiled in compiledMods) {
                MyLog.Default.WriteLine($"Compiled {compiled.module.id}");
                VRage.ObjectBuilders.MyObjectBuilderType.RegisterFromAssembly(compiled.assembly);
                VRage.Game.Components.MyComponentFactory.Static.RegisterFromAssembly(compiled.assembly);
                VRage.Game.Components.MyComponentTypeFactory.Static.RegisterFromAssembly(compiled.assembly);
                VRage.ObjectBuilders.MyObjectBuilderSerializer.RegisterFromAssembly(compiled.assembly);

                MyLog.Default.WriteLine($"Registered {compiled.module.id}");
                MySession.Static
                    .RegisterComponentsFromAssembly(compiled.assembly, modAssembly: true);
            }
            VRage.Game.Entity.UseObject.MyUseObjectFactory
                .RegisterAssemblyTypes(compiledMods.Select(c => c.assembly).ToArray());
        }

        public static void LoadScriptModules() {

        }
    }
}
