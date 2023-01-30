using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static UnityEngine.UI.Image;
using Random = UnityEngine.Random;
using Type = System.Type;

namespace ScriptTrainer
{
    public class ScriptPatch
    {
        #region[全局参数]
        public static bool ResearchRate = false;
        public static List<CardData> ChangedValue { get; set; } = new List<CardData>();
        #endregion

        #region 前补丁
        //Prefix 前补丁，在补丁的函数前执行

        //示例
        //[HarmonyPatch(typeof(RecyclingWells), "OnNetworkSpawn")]    RecyclingWells为类型名称，OnNetworkSpawn为函数名称
        //public class RecyclingWellsOverridePatch_OnNetworkSpawn
        //{
        //    [HarmonyPrefix]                                         前置补丁的补丁函数前的声明
        //    public static void Prefix(RecyclingWells __instance)    __instance为特殊名称，当前注入类型入口
        //    {
        //        Debug.Log($"ZG:修改前{Traverse.Create(__instance).Field("initialNumberOfUses").GetValue()}");
        //        Traverse.Create(__instance).Field("initialNumberOfUses").SetValue(99);
        //        Debug.Log($"ZG:修改后{99}");
        //    }
        //}


        #endregion

        #region 后补丁
        //Postfix后补丁，在函数执行后执行
        //[HarmonyPatch(typeof(Test), "Updata")]    Test为类型名称，Updata为函数名称
        //public class TestOverridePatch_Updata
        //{
        //    [HarmonyPostfix]                                         后置补丁的补丁函数前的声明
        //    public static void Postfix(ref int __result)    __result为特殊名称，当前函数的返回值
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

        [HarmonyPatch(typeof(GameManager), "ProgressCurrentResearch")]
        public class GameManagerOverridePatch_ProgressCurrentResearch
        {
            [HarmonyPrefix]
            public static void Prefix(GameManager __instance)
            {
                if (!ResearchRate)
                    return;

                GraphicsManager GameGraphics = Traverse.Create(__instance).Field("GameGraphics").GetValue<GraphicsManager>();
                if(GameGraphics == null)
                    return;

                CardData card = UniqueIDScriptable.GetFromID<CardData>(GameGraphics.BlueprintModelsPopup.CurrentResearch.UniqueID);
                Debug.Log(card.CardName);

                if (ChangedValue.Contains(GameGraphics.BlueprintModelsPopup.CurrentResearch))
                    return;

                GameGraphics.BlueprintModelsPopup.CurrentResearch.BlueprintUnlockSunsCost = card.BlueprintUnlockSunsCost / 2;
                ChangedValue.Add(GameGraphics.BlueprintModelsPopup.CurrentResearch);
            }
            
        }
        //[HarmonyPatch(typeof(GameController), "OnDestroy")]
        //public class GameControllerOverridePatch_OnDestroy
        //{
        //    [HarmonyPostfix]
        //    public static void OnDestroy()
        //    {
        //        Debug.Log("GameController.OnDestroy");
        //        AddEffects.Clear();
        //        Debug.Log("移除添加过的效果信息");
        //    }
        //}
    }
}
