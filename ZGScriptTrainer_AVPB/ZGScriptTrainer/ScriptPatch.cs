using HarmonyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace ZGScriptTrainer
{
    public static class ScriptPatchUtil
    {
        public static int GetMaxNum(this int max,int value)
        {
            if (value < 0)
                return 0;
            if(value > max) return max;
            return value;
        }
    }
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

        [HarmonyPatch(typeof(HudManager), "StartGame")]
        public class HudManagerOverridePatch_StartGame
        {
            [HarmonyPrefix]
            public static void Prefix()
            {
                List<BaseClass> players = CollectionManager.Instance.GetPlayers();
                if (players.Count > 0)
                {
                    var farmer = players[0].GetComponent<Farmer>();
                    ZGScriptTrainer.WriteLog($"ZG StartGame farmer修改");
                    farmer.m_FarmerUpgrades.SetCapacity(farmer.m_FarmerUpgrades.m_Capacity + ZGScriptTrainer.FarmerExtraUpgrades.Value);
                }
            }
        }

        [HarmonyPatch(typeof(FarmerPlayer), "Restart")]
        public class FarmerPlayerOverridePatch_Restart
        {
            [HarmonyPostfix]
            public static void Postfix(FarmerPlayer __instance)
            {
                ZGScriptTrainer.WriteLog($"ZG FarmerPlayer Restart farmer修改");
                __instance.SetOverallSpeedScale(ZGScriptTrainer.FarmerMoveSpeedScale.Value);

                __instance.m_FarmerCarry.SetCapacity(__instance.m_FarmerCarry.m_MaxCarryCount + ZGScriptTrainer.FarmerExtraCarry.Value);
                __instance.m_FarmerInventory.SetCapacity(__instance.m_FarmerInventory.m_Capacity + ZGScriptTrainer.FarmerExtraInventory.Value);
            }
        }

        [HarmonyPatch(typeof(Worker), "Awake")]
        [HarmonyPatch(typeof(Worker), "Restart")]
        public class WorkerOverridePatch_Awake
        {
            [HarmonyPostfix]
            public static void Postfix(Worker __instance)
            {
                __instance.SetMoveSpeedScale(ZGScriptTrainer.WorkerMoveSpeedScale.Value);

                __instance.SetExtraMemory(__instance.m_ExtraMemorySize + ZGScriptTrainer.WorkerExtraMemory.Value);
                __instance.SetExtraEnergy(__instance.m_ExtraEnergy + ZGScriptTrainer.WorkerExtraEnergy.Value);
                __instance.SetExtraSearch(__instance.m_ExtraSearchRange + ZGScriptTrainer.WorkerExtraSearchRange.Value, __instance.m_ExtraSearchDelay + ZGScriptTrainer.WorkerExtraSearchDelay.Value);

                __instance.m_ExtraMovementDelay = ZGScriptTrainer.WorkerExtraMovementDelay.Value;
                __instance.m_ExtraMovementScale = ZGScriptTrainer.WorkerExtraMovementScale.Value;
                __instance.SetOverallSpeedScale(__instance.m_OverallSpeedScale + __instance.m_ExtraMovementScale);

                //__instance.SetExtraMovement(ZGScriptTrainer.WorkerExtraMovementDelay.Value, ZGScriptTrainer.WorkerExtraMovementScale.Value);

                //__instance.SetExtraSearch(__instance.m_ExtraSearchRange + ZGScriptTrainer.WorkerExtraSearchRange.Value, __instance.m_HeadInfo.m_FindNearestDelay.GetMaxNum(__instance.m_ExtraSearchDelay + ZGScriptTrainer.WorkerExtraSearchDelay.Value));
                //__instance.SetExtraMovement(__instance.m_DriveInfo.m_MoveInitialDelay.GetMaxNum(__instance.m_ExtraMovementDelay + ZGScriptTrainer.WorkerExtraMovementDelay.Value), __instance.m_ExtraMovementScale + ZGScriptTrainer.WorkerExtraMovementScale.Value);
            }
        }
        [HarmonyPatch(typeof(Worker), "UpdateFrame")]
        public class WorkerOverridePatch_UpdateFrame
        {
            [HarmonyPostfix]
            public static void Postfix(Worker __instance)
            {
                __instance.m_FarmerCarry.SetCapacity(__instance.m_FarmerCarry.m_MaxCarryCount + ZGScriptTrainer.WorkerExtraCarry.Value);
                __instance.m_FarmerInventory.SetCapacity(__instance.m_FarmerInventory.m_Capacity + ZGScriptTrainer.WorkerExtraInventory.Value);
                __instance.m_FarmerUpgrades.SetCapacity(__instance.m_FarmerUpgrades.m_Capacity + ZGScriptTrainer.WorkerExtraUpgrades.Value);
            }
        }

        [HarmonyPatch(typeof(FarmerUpgrades), "UpdateAppliedObjects")]
        public class FarmerUpgradesOverridePatch_UpdateAppliedObjects
        {
            [HarmonyPostfix]
            public static void Postfix(FarmerUpgrades __instance)
            {
                Worker component = __instance.GetComponent<Farmer>().GetComponent<Worker>();
                if (component)
                {
                    component.SetExtraMemory(component.m_ExtraMemorySize + ZGScriptTrainer.WorkerExtraMemory.Value);
                    component.SetExtraEnergy(component.m_ExtraEnergy + ZGScriptTrainer.WorkerExtraEnergy.Value);
                    component.SetExtraSearch(component.m_ExtraSearchRange + ZGScriptTrainer.WorkerExtraSearchRange.Value, component.m_HeadInfo.m_FindNearestDelay.GetMaxNum(component.m_ExtraSearchDelay + ZGScriptTrainer.WorkerExtraSearchDelay.Value));
                    ZGScriptTrainer.WriteLog($"ZG SetExtraMovement:{ZGScriptTrainer.WorkerExtraMovementDelay.Value},{ZGScriptTrainer.WorkerExtraMovementScale.Value}");
                    component.SetExtraMovement(component.m_DriveInfo.m_MoveInitialDelay.GetMaxNum(component.m_ExtraMovementDelay + ZGScriptTrainer.WorkerExtraMovementDelay.Value), component.m_ExtraMovementScale + ZGScriptTrainer.WorkerExtraMovementScale.Value);
                }
            }
        }


        [HarmonyPatch(typeof(StorageTypeManager), "Reset")]
        public class StorageTypeManagerOverridePatch_Reset
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if(ZGScriptTrainer.StorageMult.Value > 1)
                {
                    foreach (var x in StorageTypeManager.m_StorageGenericInformation)
                    {
                        x.Value.m_Capacity = x.Value.m_Capacity * ZGScriptTrainer.StorageMult.Value;
                    }
                    foreach (var x in StorageTypeManager.m_StoragePaletteInformation)
                    {
                        x.Value.m_Capacity = x.Value.m_Capacity * ZGScriptTrainer.StorageMult.Value;
                    }
                }
                else if(ZGScriptTrainer.StorageSize.Value != -1)
                {
                    foreach (var x in StorageTypeManager.m_StorageGenericInformation)
                    {
                        x.Value.m_Capacity = ZGScriptTrainer.StorageSize.Value;
                    }
                    foreach (var x in StorageTypeManager.m_StoragePaletteInformation)
                    {
                        x.Value.m_Capacity = ZGScriptTrainer.StorageSize.Value;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Holdable), "PostCreate")]
        public class HoldableOverridePatch_Create
        {
            [HarmonyPostfix]
            public static void Postfix(Holdable __instance)
            {
                if(__instance is MyTool MyTool)
                {
                    if (MyTool.m_MaxUsageCount == 0)
                        return;
                    if (ZGScriptTrainer.ToolMult.Value > 1)
                    {
                        MyTool.m_MaxUsageCount *= ZGScriptTrainer.ToolMult.Value;
                    }
                    else if (ZGScriptTrainer.ToolSize.Value != -1)
                    {
                        MyTool.m_MaxUsageCount = ZGScriptTrainer.ToolSize.Value;
                    }
                }
            }
        }
        [HarmonyPatch(typeof(HayBale), "Restart")]
        [HarmonyPatch(typeof(RepairKit), "Restart")]
        public class ToolManagerOverridePatch_Create
        {
            [HarmonyPostfix]
            public static void Postfix(Holdable __instance)
            {
                if (__instance.m_MaxUsageCount == 0)
                    return;
                if (ZGScriptTrainer.ToolMult.Value > 1)
                {
                    __instance.m_MaxUsageCount *= ZGScriptTrainer.ToolMult.Value;
                }
                else if(ZGScriptTrainer.ToolSize.Value != -1)
                {
                    __instance.m_MaxUsageCount = ZGScriptTrainer.ToolSize.Value;
                }
            }
        }
    }
}
