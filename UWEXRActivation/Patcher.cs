//using BepInEx;
//using Mono.Cecil;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Reflection;
//using System.Runtime.CompilerServices;

//public static class Patcher
//{
//    public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp-firstpass.dll" };
//    private static readonly string[] moddedClasses = { "UWEXR.XRSettings", "UWEXR.XRNodeState", "UWEXR.InputTracking", "UWEXR.XRDevice" };

//    public static void Patch(AssemblyDefinition target)
//    {
//        var path = Assembly.GetExecutingAssembly().Location;

//        if (!File.Exists(path))
//        {
//            Trace.TraceWarning($"Cannot apply UWEXR patch because UWEXRActivation dll could not be found. Expect plugin crashes!");
//            return;
//        }

//        var source = AssemblyDefinition.ReadAssembly(path);
//        AddModdedClasses(target, source);
//    }

//    private static void AddModdedClasses(AssemblyDefinition target, AssemblyDefinition source)
//    {
//        foreach (var moddedClass in moddedClasses)
//        {
//            var type = source.MainModule.GetType(moddedClass);
//            var forwardAttrConstructor = target.MainModule.ImportReference(typeof(TypeForwardedToAttribute).GetConstructor(new[] { typeof(Type) }));
//            var forwardAttribute = new CustomAttribute(forwardAttrConstructor);
//            var t = target.MainModule.ImportReference(type);
//            forwardAttribute.ConstructorArguments.Add(new CustomAttributeArgument(target.MainModule.ImportReference(typeof(Type)), t));
//            target.CustomAttributes.Add(forwardAttribute);
//            target.MainModule.ExportedTypes.Add(new ExportedType(t.Namespace, t.Name, t.Module, t.Scope));
//        }
//    }
//}

