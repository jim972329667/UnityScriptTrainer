using HarmonyLib;
using Il2CppSystem.Numerics;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using static Il2CppSystem.Globalization.CultureInfo;
using UnityEngine.UI;
using static UnityEngine.Random;
using SteamHome;
using SteamMatchTogether;

namespace ScriptTrainer
{
    public class ScriptPatch
    {
        #region[全局参数]
        public static int test = -1;
        #endregion

        #region 前补丁
        /// <summary>
        /// Prefix 前补丁，在补丁的函数前执行
        /// 示例
        ///    [HarmonyPatch(typeof(RecyclingWells), "OnNetworkSpawn")] 
        ///    RecyclingWells为类型名称，OnNetworkSpawn为函数名称
        ///    public class RecyclingWellsOverridePatch_OnNetworkSpawn
        ///    {
        ///        [HarmonyPrefix] 前置补丁的补丁函数前的声明
        ///        public static void Prefix(RecyclingWells __instance)    
        ///        __instance为特殊名称，当前注入类型入口
        ///        {
        ///            Traverse.Create(__instance).Field("initialNumberOfUses").SetValue(99);
        ///        }
        ///}
        /// </summary>
        #endregion

        #region 后补丁
        /// <summary>
        ///Postfix后补丁，在函数执行后执行
        ///[HarmonyPatch(typeof(Test), "Updata")]    Test为类型名称，Updata为函数名称
        ///public class TestOverridePatch_Updata
        ///{
        ///    [HarmonyPostfix]                                         后置补丁的补丁函数前的声明
        ///    public static void Postfix(ref int __result)    __result为特殊名称，当前函数的返回值
        ///    {
        ///        __result \= 2;
        ///    }
        ///}
        /// </summary>
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

        #region[多函数用同个补丁]
        //[HarmonyPatch]
        //public class FiringPatch
        //{
        //    public static float DurabilityCostFloat = 0f;
        //    public static float Durability = 0f;
        //    public static IEnumerable<MethodBase> TargetMethods()
        //    {
        //        yield return AccessTools.Method(typeof(BasicRangedWeapon), "Fire");
        //        yield return AccessTools.Method(typeof(BasicRangedWeapon), "Fire_Bow");
        //        yield return AccessTools.Method(typeof(BasicRangedWeapon), "Fire_Cartridge");
        //        yield return AccessTools.Method(typeof(BasicRangedWeapon), "Fire_Crossbow");
        //        yield return AccessTools.Method(typeof(BasicRangedWeapon), "Fire_EnergyWeapon");
        //    }
        //    [HarmonyPrefix]
        //    public static void Prefix(ref BasicRangedWeapon __instance)
        //    {

        //        if (InfiniteDurability && __instance.character.isPlayer)
        //        {
        //            DurabilityCostFloat = __instance.m_gunpartDurabilityCostFloat;
        //            __instance.m_gunpartDurabilityCostFloat = 0;
        //            Durability = __instance.weaponData.properties[0];
        //            Debug.Log($"{DurabilityCostFloat};{Durability}");
        //        }
        //    }
        //    [HarmonyPostfix]
        //    public static void Postfix(ref BasicRangedWeapon __instance)
        //    {
        //        //if (InfiniteDurability && __instance.character.isPlayer)
        //        //{
        //        //    __instance.m_gunpartDurabilityCostFloat = DurabilityCostFloat;
        //        //    __instance.weaponData.properties[0] = Durability;
        //        //}
        //    }
        //}
        #endregion


        //[HarmonyPatch(typeof(SteamHomeTeamManager), "CheckHeroDLC")]
        //public class SteamHomeTeamManagerOverridePatch_CheckHeroDLC
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(ref bool __result, ulong _member, int hero)
        //    {
        //        __result = true;
        //        ScriptTrainer.WriteLog($"_member:{_member},hero:{hero}");
        //    }
        //}

        [HarmonyPatch(typeof(SteamMatchAuthManager), "DlcStatUpdated")]
        public class SteamMatchAuthManagerOverridePatch_DlcStatUpdated
        {
            [HarmonyPostfix]
            public static void Postfix(string reason)
            {
                ScriptTrainer.WriteLog($"reason:{reason}");
            }
        }
        [HarmonyPatch(typeof(NewPlayerManager), "AddPlayer")]
        public class DlcManagerOverridePatch_DlcOwnershipChanged
        {
            [HarmonyPostfix]
            public static void Postfix(int pid, NewPlayerObject pobj)
            {
                ScriptTrainer.WriteLog($"pid:{pid}");
                test = pid;
            }
        }
    }
}
