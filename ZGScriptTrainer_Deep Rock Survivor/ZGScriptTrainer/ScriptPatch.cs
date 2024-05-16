using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZGScriptTrainer
{
    public class ScriptPatch
    {
        #region[全局参数]

        #endregion

        #region[教程]

        #region 前补丁
        //Prefix 前补丁，在补丁的函数前执行
        //RecyclingWells为类型名称，OnNetworkSpawn为函数名称
        //__instance，当前注入类型入口
        //__result，当前函数的返回值
        //__runOriginal,如果设置为 false，则不运行原始方法代码。
        //__<idx>,例如__0  函数的输入变量

        //[HarmonyPatch(typeof(RecyclingWells), "OnNetworkSpawn")]
        //public class RecyclingWellsOverridePatch_OnNetworkSpawn
        //{
        //    [HarmonyPrefix]
        //    public static void Prefix(RecyclingWells __instance)
        //    {
        //        Debug.Log($"ZG:修改前{Traverse.Create(__instance).Field("initialNumberOfUses").GetValue()}");
        //        Traverse.Create(__instance).Field("initialNumberOfUses").SetValue(99);
        //        Debug.Log($"ZG:修改后{99}");
        //    }
        //}
        #endregion

        #region 后补丁
        //Postfix后补丁，在函数执行后执行

        //[HarmonyPatch(typeof(Test), "Updata")]
        //public class TestOverridePatch_Updata
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(ref int __result)    
        //    {
        //        __result \= 2;
        //    }
        //}
        #endregion

        #region 多个同名函数补丁制作
        //[HarmonyPatch(typeof(HexMapManager), "GenerateNewMap", new Type[] { typeof(Sector) })]
        //在HexMapManager类里有多个名为GenerateNewMap的函数时，HarmonyPatch的第三个参数是函数输入变量的类型，第四个参数是函数out输出变量的类型
        #endregion

        #region 可读写属性补丁制作
        //[HarmonyPatch(typeof(MinigameChest), "Price", MethodType.Getter)]
        //[HarmonyPatch(typeof(MinigameChest), "Price", MethodType.Setter)]
        #endregion

        #region 成员修改
        //Traverse.Create(__instance).Field("initialNumberOfUses").GetValue<T>();
        //Traverse.Create(__instance).Field("initialNumberOfUses").SetValue(99);
        //initialNumberOfUses 是成员名称， T为该成员类型
        #endregion

        #region 多个函数用一个补丁
        //[HarmonyPatch]
        //public class BasicVehiclePatch
        //{
        //    public static float DurabilityCostFloat = 0f;
        //    public static float Durability = 0f;
        //    public static float Energy = 0f;
        //    public static IEnumerable<MethodBase> TargetMethods()
        //    {
        //        yield return AccessTools.Method(typeof(BasicVehicle), "OnPlayerEnterRange");
        //        yield return AccessTools.Method(typeof(BasicVehicle), "GetInVehicle");
        //        yield return AccessTools.Method(typeof(BasicVehicle), "OnPlayerGetOutVehicle");
        //        yield return AccessTools.Method(typeof(BasicVehicle), "OnPlayerExitRange");
        //        yield return AccessTools.Method(typeof(BasicVehicle), "Fire_EnergyWeapon");
        //    }
        //    [HarmonyPrefix]
        //    public static void Prefix(ref BasicVehicle __instance, MethodBase __originalMethod)
        //    {
        //        ZGScriptTrainer.WriteLog($"{__originalMethod.Name}");
        //        ZGScriptTrainer.WriteLog($"{__instance.vehicleData.worldCoordinate};{__instance.currentChunk.chunkData.coordinate};{GameController.instance.gameData.playerData.characterPosition}");
        //    }
        //    [HarmonyPostfix]
        //    public static void Postfix(ref BasicVehicle __instance)
        //    {

        //    }
        //}
        #endregion

        #region 单独注入补丁

        //CreateProcessor  待补丁的原始函数
        //AddPrefix 补丁函数
        //PatchProcessor patchProcessor = harmony.CreateProcessor(TargetMethod);
        //patchProcessor.AddPrefix(AccessTools.Method(typeof(ScriptPatch), "PrefixMethod"));
        //patchProcessor.Patch();
        #endregion

        #endregion

        //[HarmonyPatch(typeof(Player), "GainXp")]
        //public class PlayerOverridePatch_GainXp
        //{
        //    [HarmonyPrefix]
        //    public static void Prefix(float amount)
        //    {
        //        ZGScriptTrainer.WriteLog($"Gain Patch: {amount}");
        //    }
        //}
    }
}
