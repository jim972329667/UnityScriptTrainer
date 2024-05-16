using HarmonyLib;
using JTW;
using ScriptTrainer.Cards;
using ScriptTrainer.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;
using Random = UnityEngine.Random;
using Type = System.Type;

namespace ScriptTrainer
{
    public class ScriptPatch
    {
        #region[全局参数]
        public static bool ZeroEnergyCost = false;
        public static bool InfinityCompass = false;
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

        [HarmonyPatch(typeof(Card), "GetEnergyCost")]
        public class CardOverridePatch_GetEnergyCost
        {
            [HarmonyPostfix]
            public static void Postfix(ref int __result)
            {
                if(ZeroEnergyCost)
                {
                    if(__result != -20000)
                    {
                        __result = -10000;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Map), "GetNextLocations")]
        public class MapOverridePatch_GetNextLocations
        {
            [HarmonyPostfix]
            public static void Postfix(ref HashSet<Location> __result, Map __instance)
            {
                if (InfinityCompass)
                {
                    if (__instance.CurrentLocation == __instance.Start)
                    {
                        return;
                    }
                    __result.Clear();
                    foreach (Route route in __instance.m_currentLocation.GetOutgoingRoutes())
                    {
                        __result.Add(route.To);
                    }
                    HashSet<Location> locationsOnDepth = __instance.GetLocationsOnDepth(__instance.CurrentLocation.Depth);
                    locationsOnDepth.Remove(__instance.CurrentLocation);
                    foreach (Location location in locationsOnDepth)
                    {
                        foreach (Route route in location.GetOutgoingRoutes())
                        {
                            if (!__result.Contains(route.To))
                            {
                                __result.Add(route.To);
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPatch]
        public class Player_LoadCards_Patch
        {
            public static IEnumerable<MethodBase> TargetMethods()
            {
                IEnumerable<Type> enumerable = from t in Assembly.GetAssembly(typeof(Player)).GetTypes()
                                               where t.IsSubclassOf(typeof(Player)) && !t.IsAbstract && !(t.GetConstructor(Type.EmptyTypes) == null)
                                               select t;
                return enumerable.SelectMany(t => t.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)).Where(m => m.Name == "LoadCards");
            }
            [HarmonyPostfix]
            public static void Postfix(ref Player __instance, ref CardPool cardPool)
            {
                if (DynamicCardCreator.Cards.TryGetValue(__instance.GetType().ToString(), out List<Card> value))
                {
                    foreach(var x in value)
                    {
                        IModCard modCard = x as IModCard;
                        if(!DynamicCardCreator.BannedList.Contains(modCard.GetModInfo().CardName))
                            cardPool.AddCard(Card.Clone(x));
                    }
                }
                Debug.Log($"ZGZGZG:{__instance.GetType()} : {cardPool.GetCards().Count}");

            }
        }
        [HarmonyPatch(typeof(CardSet), "GenerateCards")]
        public class CardSetOverridePatch_GenerateCards
        {
            [HarmonyPostfix]
            public static void Postfix(string cardAlignment, ref List<Card> __result)
            {
                if (cardAlignment == "Neutral")
                {
                    if (DynamicCardCreator.Cards.TryGetValue("Neutral", out List<Card> value))
                    {
                        foreach(var x in value)
                        {
                            IModCard modCard = x as IModCard;
                            if (!DynamicCardCreator.BannedList.Contains(modCard.GetModInfo().CardName))
                                __result.Add(Card.Clone(x));
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(EntranceSceneController), "SetupMoreModesButtons")]
        public class EntranceSceneControllerOverridePatch_SetupMoreModesButtons
        {
            [HarmonyPostfix]
            public static void Postfix(EntranceSceneController __instance)
            {
                GameObject canvasGO = Traverse.Create(__instance).Field("m_canvasGO").GetValue<GameObject>();
                GameObject moreModesButtonsGO = canvasGO.transform.Find("MoreModesButtons").gameObject;

                GameObject gameObject = GameObject.Instantiate(moreModesButtonsGO.transform.Find("StoryTestButton").gameObject);
                gameObject.transform.SetParent(moreModesButtonsGO.transform, false);
                var title = gameObject.GetComponent<TitleButtonNewComponent>();
                title.SetText("Mod卡牌管理");
                title.Clicked += delegate ()
                {
                    if(ScriptTrainer.CardUI != null)
                    {
                        GameObject cardUI = GameObject.Instantiate(ScriptTrainer.CardUI);
                        cardUI.AddComponent<ModCardUI>();
                        cardUI.transform.SetParent(canvasGO.transform, false);
                        cardUI.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        cardUI.transform.localPosition = Vector3.zero;
                    }
                };
                gameObject.SetActive(true);
                gameObject.transform.SetSiblingIndex(2);
            }
        }
    }
}
