using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.Image;
using Random = UnityEngine.Random;
using Type = System.Type;

namespace ScriptTrainer
{
    public class ScriptPatch
    {
        #region[全局参数]

        public static bool InfiniteLoadoutPoints = true;
        public static bool LoadConfigs = false;
        #endregion

        #region 前补丁
        //Prefix 前补丁，在补丁的函数前执行//return false跳过源程序

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

        //[HarmonyPatch(typeof(JTW.ga), "ProgressCurrentResearch")]
        //public class GameManagerOverridePatch_ProgressCurrentResearch
        //{
        //    [HarmonyPrefix]
        //    public static void Prefix(GameManager __instance)
        //    {
        //        if (!ResearchRate)
        //            return;

        //        GraphicsManager GameGraphics = Traverse.Create(__instance).Field("GameGraphics").GetValue<GraphicsManager>();
        //        if (GameGraphics == null)
        //            return;

        //        CardData card = UniqueIDScriptable.GetFromID<CardData>(GameGraphics.BlueprintModelsPopup.CurrentResearch.UniqueID);
        //        Debug.Log(card.CardName);

        //        if (ChangedValue.Contains(GameGraphics.BlueprintModelsPopup.CurrentResearch))
        //            return;

        //        GameGraphics.BlueprintModelsPopup.CurrentResearch.BlueprintUnlockSunsCost = card.BlueprintUnlockSunsCost / 2;
        //        ChangedValue.Add(GameGraphics.BlueprintModelsPopup.CurrentResearch);
        //    }

        //}

        [HarmonyPatch(typeof(PartyManagementDirector), "_getLoadoutPoints")]
        public class PartyManagementDirectorOverridePatch__getLoadoutPoints
        {
            [HarmonyPrefix]
            public static void Prefix(Entity pEntity)
            {
                if (InfiniteLoadoutPoints)
                {
                    CharacterHelper.SetStat(pEntity, eCharacterStats.LOP.ToString(), 999);
                }
            }
        }
        [HarmonyPatch(typeof(ConfigsHelper), "LoadConfigs")]
        public class ConfigsHelperOverridePatch_LoadConfigs
        {
            [HarmonyPostfix]
            public static void Postfix(Configs __result)
            {
                if (!LoadConfigs)
                {
                    ItemWindow.Configs = __result;
                    ScriptTrainer.Instance.Log("加载Configs成功！");
                    LoadConfigs = true;
                }
            }
        }

        [HarmonyPatch(typeof(MemoryManagementHelper), "PreloadRequiredAssets")]
        public class MemoryManagementHelperOverridePatch_PreloadRequiredAssets
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (!ScriptTrainer.Instance.Initialized)
                {
                    ScriptTrainer.Instance.Init();
                }
            }
        }

        [HarmonyPatch(typeof(GameRunData), "SetEntities")]
        public class GameRunDataOverridePatch__SetEntities
        {
            [HarmonyPostfix]
            public static void Postfix(List<Entity> pEntities)
            {
                ItemWindow.Players.Clear();
                foreach (Entity entity in pEntities)
                {
                    if (entity.Components.ContainsKey("PlayerComponent"))
                    {
                        ItemWindow.Players.Add(entity);
                    }
                }
                ItemWindow.UpdatePlayer();
                ScriptTrainer.Instance.Log(ItemWindow.Players.Count);
            }
        }

        [HarmonyPatch(typeof(AdventureHelper), "InitializeData")]
        public class AdventureHelperOverridePatch__InitializeData
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                ItemWindow.Players.Clear();
                foreach (Entity entity in RouterHelper.Env.GameRun.Entities)
                {
                    if (entity.Components.ContainsKey("PlayerComponent"))
                    {
                        ItemWindow.Players.Add(entity);
                    }
                }
                ItemWindow.UpdatePlayer();
                ScriptTrainer.Instance.Log($"AdventureHelper : {ItemWindow.Players.Count}");
            }
        }
        [HarmonyPatch(typeof(CombatHelper), "CreateSummon")]
        public class CombatHelperOverridePatch__CreateSummon
        {
            [HarmonyPostfix]
            public static void Postfix(int pGroupIndex, eSummonTypes pSummonType, string pSummonTarget, int pLevel, GameRandom pRandom)
            {
                List<string> actorsByTags = CharacterHelper.GetActorsByTags(new List<string>
                {
                    pSummonTarget
                }, pLevel, RouterMono.GetExpansion());
                actorsByTags.RemoveAll((string e) => CharacterHelper.GetActorTags(e).Contains(eConfigTags.TWO_BY_TWO.ToString()));
                object[] args = CoreHelper.BuildActorWeightArray(actorsByTags, null);
                foreach(var tag in actorsByTags)
                {
                    ScriptTrainer.Instance.Log($"actorsByTags : {tag}");
                }
                foreach (var actor in args)
                {
                    ScriptTrainer.Instance.Log($"args : {actor}");
                }
            }
        }


    }
}
