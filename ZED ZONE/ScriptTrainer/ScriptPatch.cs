using HarmonyLib;
using Il2CppSystem.Numerics;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using static Il2CppSystem.Globalization.CultureInfo;
using UnityEngine.UI;
using static UnityEngine.Random;

namespace ScriptTrainer
{
    public class ScriptPatch
    {
        #region[全局参数]
        public static bool UnChangeBackpackSize = false;
        public static bool UnChangeBackpackWeight = false;
        public static bool InfiniteDurability = false;
        public static bool InfiniteAmmo = false;
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


        [HarmonyPatch(typeof(CharacterData), "backpackSize", MethodType.Setter)]
        public class CharacterDataOverridePatch_backpackSize
        {
            [HarmonyPrefix]
            public static bool Prefix(ref Vector2Int value)
            {
                if(UnChangeBackpackSize)
                {
                    ScriptTrainer.Log.LogMessage("成功跳过背包");
                    ScriptTrainer.Log.LogMessage(value);
                    value = Scripts.GetBackpackSize();
                    GameController.instance.gameData.playerData.inventoryData.inventorySize = value;
                    return false;
                }
                return true;
            }
        }

        
        [HarmonyPatch(typeof(InventoryData), "RefreshTotalItemWeight")]
        public class InventoryDataOverridePatch_totalItemWeight
        {
            [HarmonyPostfix]
            public static void Postfix(InventoryData __instance)
            {
                if (UnChangeBackpackWeight && __instance.isPlayerInventory)
                {
                    __instance.totalItemWeight = 0;
                    GameData data = GameController.instance.gameData;
                    data.playerData.totalEquipmentWeight = 0;
                    ScriptTrainer.Log.LogMessage("刷新背包重量");
                }
            }
        }

        [HarmonyPatch(typeof(CharacterEquipmentData), "SetWeaponData")]
        public class CharacterEquipmentDataOverridePatch_SetWeaponData
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (UnChangeBackpackWeight)
                {
                    GameData data = GameController.instance.gameData;
                    data.playerData.totalEquipmentWeight = 0;
                    ScriptTrainer.Log.LogMessage("刷新装备重量");
                }
            }
        }

        #region[无限耐久]
        [HarmonyPatch(typeof(InventoryData), "CostItemDurability", new System.Type[] { typeof(ItemData),typeof(float) })]
        public class InventoryDataOverridePatch_CostItemDurability
        {
            [HarmonyPrefix]
            public static void Prefix(ref float durabilityCost)
            {
                if (InfiniteDurability)
                {
                    ScriptTrainer.WriteLog($"成功跳过耐久度消耗{durabilityCost}");
                    durabilityCost=0f;
                }
            }
        }

        // 近战武器
        [HarmonyPatch]
        public class BasicMeleeWeaponOverridePatch
        {
            public static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.Method(typeof(BasicMeleeWeapon), "OnMeleeAttack");
                yield return AccessTools.Method(typeof(BasicMeleeWeapon), "OnMeleeAttackHit");
                yield return AccessTools.Method(typeof(BasicMeleeWeapon), "OnMeleeAttackHitObject");
            }
            [HarmonyPrefix]
            public static void Prefix(ref BasicMeleeWeapon __instance, out float __state)
            {
                __state = -1;
                if (InfiniteDurability && __instance.character.isPlayer)
                {
                    __state = __instance.weaponData.durability;
                }
            }
            [HarmonyPostfix]
            public static void Postfix(ref BasicMeleeWeapon __instance, float __state)
            {
                if (__state != -1)
                {
                    ScriptTrainer.WriteLog($"近战武器耐久：{__instance.weaponData.durability}=>{__state}");
                    __instance.weaponData.durability = __state;
                }
            }
        }
        // 远程武器
        [HarmonyPatch]
        public class FiringPatch
        {
            public static float DurabilityCostFloat = 0f;
            public static float Durability = 0f;
            public static float Energy = 0f;
            public static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.Method(typeof(BasicRangedWeapon), "Fire");
                //yield return AccessTools.Method(typeof(BasicRangedWeapon), "Fire_Bow");
                //yield return AccessTools.Method(typeof(BasicRangedWeapon), "Fire_Cartridge");
                //yield return AccessTools.Method(typeof(BasicRangedWeapon), "Fire_Crossbow");
                //yield return AccessTools.Method(typeof(BasicRangedWeapon), "Fire_EnergyWeapon");
            }
            [HarmonyPrefix]
            public static void Prefix(ref BasicRangedWeapon __instance)
            {
                if (InfiniteDurability && __instance.character.isPlayer)
                {
                    DurabilityCostFloat = __instance.m_gunpartDurabilityCostFloat;
                    __instance.m_gunpartDurabilityCostFloat = 0;
                    Durability = __instance.weaponData.properties[0];
                }
                if (InfiniteAmmo && __instance.character.isPlayer)
                {
                    __instance.airgunPressure = 35;
                    if (__instance.weaponAttr.batteryEnergyPerSec != 0)
                        Energy = __instance.weaponData.properties[2];
                    __instance.weaponData.properties[3] = 99;
                }
            }
            [HarmonyPostfix]
            public static void Postfix(ref BasicRangedWeapon __instance)
            {
                if (InfiniteDurability && __instance.character.isPlayer)
                {
                    string x = "开火后：";
                    //for (int i = 0; i < 13; i++)
                    //{
                    //    x += $"{__instance.weaponData.properties[i]};";
                    //}
                    x += $"damageFloat = {__instance.damageFloat};damage={__instance.damage}";
                    ScriptTrainer.WriteLog(x);

                    __instance.m_gunpartDurabilityCostFloat = DurabilityCostFloat;
                    __instance.weaponData.properties[0] = Durability;
                }
                if (InfiniteAmmo && __instance.character.isPlayer)
                {
                    if (__instance.weaponAttr.batteryEnergyPerSec != 0)
                        __instance.weaponData.properties[2] = Energy;
                }
            }
        }

        //车辆
        [HarmonyPatch]
        public class BasicVehiclePatch
        {
            public static float DurabilityCostFloat = 0f;
            public static float Durability = 0f;
            public static float Energy = 0f;
            public static IEnumerable<MethodBase> TargetMethods()
            {
                //yield return AccessTools.Method(typeof(BasicVehicle), "OnPlayerEnterRange");
                yield return AccessTools.Method(typeof(BasicVehicle), "GetInVehicle");
                yield return AccessTools.Method(typeof(BasicVehicle), "OnPlayerGetOutVehicle");
                //yield return AccessTools.Method(typeof(BasicVehicle), "OnPlayerExitRange");
                //yield return AccessTools.Method(typeof(BasicVehicle), "Fire_EnergyWeapon");
            }
            [HarmonyPrefix]
            public static void Prefix(ref BasicVehicle __instance, MethodBase __originalMethod)
            {
                ScriptTrainer.WriteLog($"{__originalMethod.Name}");
                ScriptTrainer.WriteLog($"{__instance.vehicleData.worldCoordinate};{__instance.currentChunk.chunkData.coordinate};{GameController.instance.gameData.playerData.characterPosition}");
            }
            [HarmonyPostfix]
            public static void Postfix(ref BasicVehicle __instance)
            {
                
            }
        }
        #endregion

        #region[无限子弹]
        [HarmonyPatch(typeof(BasicRangedWeapon), "CanFire")]
        public class BasicRangedWeaponOverridePatch_CanFire
        {
            public static float DurabilityCostFloat = 0f;
            public static float Durability = 1200f;
            [HarmonyPostfix]
            public static void Postfix(ref BasicRangedWeapon __instance,ref bool __result)
            {
                if (InfiniteAmmo && __instance.character.isPlayer)
                {
                    ScriptTrainer.WriteLog($"{__instance.airgunPressure};{__instance.airgunPowerFloat}");
                    __result = true;
                }
                //if (InfiniteDurability && __instance.character.isPlayer)
                //{
                //    DurabilityCostFloat = __instance.m_gunpartDurabilityCostFloat;
                //    Durability = __instance.weaponData.properties[0];
                //    __instance.m_gunpartDurabilityCostFloat = 0;
                //}
            }
        }

        [HarmonyPatch(typeof(BasicRangedWeapon), "WeaponMalfunction")]
        public class BasicRangedWeaponOverridePatch_WeaponMalfunction
        {
            [HarmonyPrefix]
            public static bool Prefix(ref BasicRangedWeapon __instance)
            {
                if (InfiniteAmmo && __instance.character.isPlayer)
                {
                    return false;
                }
                return true;
            }
        }
        #endregion

    }
}
