using System;
using System.Diagnostics;
using System.Reflection;
using HarmonyLib;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;

namespace ScriptTrainer.Runtime
{
    public class SinglePatch
    {
        public static Harmony Harmony { get; } = new Harmony("ScriptTrainer.Jim97.Runtime.Harmony");
        internal static bool Patch(Type type, string methodName, MethodType methodType, Type[] arguments = null,
            MethodInfo prefix = null, MethodInfo postfix = null, MethodInfo finalizer = null)
        {
            try
            {
                string namePrefix;
                if (methodType != MethodType.Getter)
                {
                    if (methodType != MethodType.Setter)
                    {
                        namePrefix = string.Empty;
                    }
                    else
                    {
                        namePrefix = "set_";
                    }
                }
                else
                {
                    namePrefix = "get_";
                }
                MethodInfo target;
                if (arguments != null)
                    target = type.GetMethod($"{namePrefix}{methodName}", AccessTools.all, null, arguments, null);
                else
                    target = type.GetMethod($"{namePrefix}{methodName}", AccessTools.all);

                if (target == null)
                {
                    // LogWarning($"\t Couldn't find any method on type {type.FullName} called {methodName}!");
                    return false;
                }

                // if this is an IL2CPP type, ensure method wasn't stripped.
                if (Il2CppType.From(type, false) != null
                    && UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(target) == null)
                {
                    return false;
                }


                PatchProcessor processor = Harmony.CreateProcessor(target);

                if (prefix != null)
                    processor.AddPrefix(new HarmonyMethod(prefix));
                if (postfix != null)
                    processor.AddPostfix(new HarmonyMethod(postfix));
                if (finalizer != null)
                    processor.AddFinalizer(new HarmonyMethod(finalizer));

                processor.Patch();

                // Log($"\t Successfully patched {type.FullName}.{methodName}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
    internal static class ReflectionPatches
    {
        internal static void Init()
        {
            SinglePatch.Patch(typeof(Assembly),
                nameof(Assembly.GetTypes),
                MethodType.Normal,
                new Type[0],
                finalizer: AccessTools.Method(typeof(ReflectionPatches), nameof(Finalizer_Assembly_GetTypes)));
        }

        public static Exception Finalizer_Assembly_GetTypes(Assembly __instance, Exception __exception, ref Type[] __result)
        {
            if (__exception != null)
            {
                if (__exception is ReflectionTypeLoadException rtle)
                {
                    __result = ReflectionUtility.TryExtractTypesFromException(rtle);
                }
                else // It was some other exception, try use GetExportedTypes
                {
                    try
                    {
                        __result = __instance.GetExportedTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        __result = ReflectionUtility.TryExtractTypesFromException(e);
                    }
                    catch
                    {
                        __result = new Type[0];
                    }
                }
            }

            return null;
        }
    }
}
