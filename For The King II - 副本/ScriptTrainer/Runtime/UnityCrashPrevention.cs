using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ScriptTrainer.Runtime
{
    internal static class UnityCrashPrevention
    {
        internal static void Init()
        {
            UnityCrashPrevention.TryPatch<Canvas>("get_renderingDisplaySize", "Canvas_renderingDisplaySize_Prefix", null);
            IEnumerable<MethodBase> patchedMethods = UnityCrashPrevention.harmony.GetPatchedMethods();
            bool flag = patchedMethods.Any<MethodBase>();
            if (flag)
            {
                ScriptTrainer.Instance.Log("初始化Unity崩溃预防: " + string.Join(", ", (from it in patchedMethods
                                                                                               select it.DeclaringType.Name + "." + it.Name).ToArray<string>()));
            }
        }
        internal static void TryPatch<T>(string orig, string prefix, Type[] argTypes = null)
        {
            try
            {
                UnityCrashPrevention.harmony.Patch(AccessTools.Method(typeof(T), orig, argTypes, null), new HarmonyMethod(AccessTools.Method(typeof(UnityCrashPrevention), prefix, null, null)), null, null, null);
            }
            catch
            {
            }
        }
        internal static void Canvas_renderingDisplaySize_Prefix(Canvas __instance)
        {
            bool flag = __instance.renderMode == RenderMode.WorldSpace && !__instance.worldCamera;
            if (flag)
            {
                throw new InvalidOperationException("Canvas is set to RenderMode.WorldSpace but not worldCamera is set.");
            }
        }
        private static readonly Harmony harmony = new Harmony("ScriptTrainer.Jim97.crashprevention");
    }
}
