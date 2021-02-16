using BepInEx;
using HarmonyLib;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

public static class Patcher
{
    public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp-firstpass.dll" };

    public static void Patch(AssemblyDefinition target)
    {
        var path = Path.Combine(Paths.GameRootPath, @"BepInEx\patchers\UWEXRActivation.dll");

        if (!File.Exists(path))
        {
            Trace.TraceWarning($"Cannot apply UWEXR patch because UWEXRActivation dll could not be found. Expect plugin crashes!");
            return;
        }

        var source = AssemblyDefinition.ReadAssembly(path);

        var xrSettings_source = source.MainModule.GetType("UWEXR.XRSettings");
        Trace.TraceInformation($"XRSettings srouce type: {xrSettings_source.Name}");
        var xrSettings_target = target.MainModule.GetType("UWEXR.XRSettings");

        //var xrNode = source.MainModule.GetType("UWEXR.XRNode");
        //var xrNodeState = source.MainModule.GetType("UWEXR.XRNodeState");
        //var xrSettings = source.MainModule.GetType("UWEXR.XRSettings");
        //var xrDevice = source.MainModule.GetType("UWEXR.XRDevice");

        //foreach (var type in source.MainModule.GetTypes())
        //{
        //    Trace.TraceInformation($"Found type: {type.FullName}");
        //}

        // Cross-reference the two assemblies
        var sourceNameReference = new AssemblyNameReference(source.Name.Name, source.Name.Version);
        var targetNameReference = new AssemblyNameReference(target.Name.Name, target.Name.Version);
        source.MainModule.AssemblyReferences.Add(targetNameReference);
        target.MainModule.AssemblyReferences.Add(sourceNameReference);

        // Create a link in main game assembly to the missing studio types
        var forwardAttrConstructor = target.MainModule.ImportReference(typeof(TypeForwardedToAttribute).GetConstructor(new[] { typeof(Type) }));
        var forwardAttribute = new CustomAttribute(forwardAttrConstructor);
        var t = target.MainModule.ImportReference(xrSettings_source);
        forwardAttribute.ConstructorArguments.Add(new CustomAttributeArgument(target.MainModule.ImportReference(typeof(Type)), t));
        target.CustomAttributes.Add(forwardAttribute);
        target.MainModule.ExportedTypes.Add(new ExportedType(t.Namespace, t.Name, t.Module, t.Scope));

        //// Preform voodoo to remove duplicate types in studio ass and instead use the types from the main assembly
        //// Big thanks to Horse for sharing the voodoo knowledge
        //source.MainModule.Types.Remove(xrSettings_source);

        //xrSettings_source.Scope = sourceNameReference;
        //xrSettings_source.GetType().GetField("module", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(xrSettings_source, target.MainModule);
        //xrSettings_source.MetadataToken = new MetadataToken(TokenType.TypeRef, 0);

        //foreach (var typeDefinitionMethod in xrSettings_source.Methods.ToList())
        //    typeDefinitionMethod.MetadataToken = new MetadataToken(TokenType.MemberRef, 0);

        //foreach (var typeDefinitionField in xrSettings_source.Fields.ToList())
        //    typeDefinitionField.MetadataToken = new MetadataToken(TokenType.MemberRef, 0);

        // Preform further voodoo with the harmony patches to make the above voodoo work
        //var h = Harmony.CreateAndPatchAll(typeof(Patcher));
        //byte[] outputAss = null;
        //using (var ms = new MemoryStream())
        //{
        //    source.Write(ms, new WriterParameters() { WriteSymbols = false });
        //    outputAss = ms.ToArray();
        //}
        //h.UnpatchAll(h.Id);

        //Assembly.Load(outputAss);

        //source.Dispose();
    }
}
