using HarmonyLib;
using PrefabEntities;
using System;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using Zenject;
using static UnityEngine.UI.Image;
using static UnlockablePermanentsConfiguration;
using static Zenject.SignalSubscription;
using Random = UnityEngine.Random;
using Type = System.Type;

namespace ScriptTrainer
{
    public class ScriptPatch
    {
        #region[全局参数]
        public static bool IsDiscount = false;
        public static bool IsChangeStormDelay = false;
        public static PrefabFactory factory = null;
        public static PrefabEntities.Pool pool = null;
        public static Transform player = null;
        #endregion

        #region 价格显示，以及价格
        [HarmonyPatch(typeof(Shop), "GetPrice")]
        public class ShopOverridePatch_GetPrice
        {
            [HarmonyPostfix]
            public static void Postfix(ref int __result)
            {
                if (IsDiscount)
                    __result /= 2;
            }

        }
        [HarmonyPatch(typeof(Market), "GetPrices")]
        public class MarketOverridePatch_GetPrices
        {
            [HarmonyPostfix]
            public static void Postfix(ref int __result)
            {
                if (IsDiscount)
                    __result /= 2;
            }

        }
        [HarmonyPatch(typeof(Carpentry), "GetPrice")]
        public class CarpentryOverridePatch_GetPrice
        {
            [HarmonyPostfix]
            public static void Postfix(ref int __result)
            {
                if (IsDiscount)
                    __result /= 2;
            }

        }
        [HarmonyPatch(typeof(UnlockablePermanentConfiguration), "GetPrice")]
        public class UnlockablePermanentConfigurationOverridePatch_GetPrice
        {
            [HarmonyPostfix]
            public static void Postfix(ref Nullable<int> __result)
            {
                if (IsDiscount)
                    if (__result != null)
                        __result /= 2;
            }

        }

        [HarmonyPatch(typeof(PriceTag), "Initialize")]
        class PriceTagOverridePatch_Initialize
        {
            [HarmonyPrefix]
            public static void Prefix(ref int price)
            {
                price /= 2;
            }
        }
        #endregion

        #region 地图修改
        [HarmonyPatch(typeof(HexMapManager), "GenerateNewMap", new Type[] {typeof(Sector) })]
        public class HexMapManagerOverridePatch_GenerateNewMap
        {
            [HarmonyPrefix]
            public static void Prefix(HexMapManager __instance, Sector sector)
            {
                if(IsChangeStormDelay)
                    Traverse.Create(__instance).Field("initialStormDelay").SetValue(30);
                //sector.startingDifficultyLevel = Random.Range(10, 19);

                #region 数量
                //Debug.Log($"ZG:introRegionConfiguration.numberOfIslands;{sector.introRegionConfiguration.numberOfIslands}");
                //Debug.Log($"ZG:introRegionConfiguration.numberOfHighWinds;{sector.introRegionConfiguration.numberOfHighWinds}");
                //Debug.Log($"ZG:introRegionConfiguration.numberOfBasicRewardEncounters;{sector.introRegionConfiguration.numberOfBasicRewardEncounters}");
                //Debug.Log($"ZG:introRegionConfiguration.numberOfRareRewardEncounters;{sector.introRegionConfiguration.numberOfRareRewardEncounters}");
                //Debug.Log($"ZG:introRegionConfiguration.numberOfSecrets;{sector.introRegionConfiguration.numberOfSecrets}");

                //Debug.Log($"ZG:earlyRegionConfiguration.numberOfIslands;{sector.earlyRegionConfiguration.numberOfIslands}");
                //Debug.Log($"ZG:earlyRegionConfiguration.numberOfHighWinds;{sector.earlyRegionConfiguration.numberOfHighWinds}");
                //Debug.Log($"ZG:earlyRegionConfiguration.numberOfBasicRewardEncounters;{sector.earlyRegionConfiguration.numberOfBasicRewardEncounters}");
                //Debug.Log($"ZG:earlyRegionConfiguration.numberOfRareRewardEncounters;{sector.earlyRegionConfiguration.numberOfRareRewardEncounters}");
                //Debug.Log($"ZG:earlyRegionConfiguration.numberOfSecrets;{sector.earlyRegionConfiguration.numberOfSecrets}");

                //Debug.Log($"ZG:midRegionConfiguration.numberOfIslands;{sector.midRegionConfiguration.numberOfIslands}");
                //Debug.Log($"ZG:midRegionConfiguration.numberOfHighWinds;{sector.midRegionConfiguration.numberOfHighWinds}");
                //Debug.Log($"ZG:midRegionConfiguration.numberOfBasicRewardEncounters;{sector.midRegionConfiguration.numberOfBasicRewardEncounters}");
                //Debug.Log($"ZG:midRegionConfiguration.numberOfRareRewardEncounters;{sector.midRegionConfiguration.numberOfRareRewardEncounters}");
                //Debug.Log($"ZG:midRegionConfiguration.numberOfSecrets;{sector.midRegionConfiguration.numberOfSecrets}");

                //Debug.Log($"ZG:lateRegionConfiguration.numberOfIslands;{sector.lateRegionConfiguration.numberOfIslands}");
                //Debug.Log($"ZG:lateRegionConfiguration.numberOfHighWinds;{sector.lateRegionConfiguration.numberOfHighWinds}");
                //Debug.Log($"ZG:lateRegionConfiguration.numberOfBasicRewardEncounters;{sector.lateRegionConfiguration.numberOfBasicRewardEncounters}");
                //Debug.Log($"ZG:lateRegionConfiguration.numberOfRareRewardEncounters;{sector.lateRegionConfiguration.numberOfRareRewardEncounters}");
                //Debug.Log($"ZG:lateRegionConfiguration.numberOfSecrets;{sector.lateRegionConfiguration.numberOfSecrets}");

                //Debug.Log($"ZG:lastRegionConfiguration.numberOfIslands;{sector.lastRegionConfiguration.numberOfIslands}");
                //Debug.Log($"ZG:lastRegionConfiguration.numberOfHighWinds;{sector.lastRegionConfiguration.numberOfHighWinds}");
                //Debug.Log($"ZG:lastRegionConfiguration.numberOfBasicRewardEncounters;{sector.lastRegionConfiguration.numberOfBasicRewardEncounters}");
                //Debug.Log($"ZG:lastRegionConfiguration.numberOfRareRewardEncounters;{sector.lastRegionConfiguration.numberOfRareRewardEncounters}");
                //Debug.Log($"ZG:lastRegionConfiguration.numberOfSecrets;{sector.lastRegionConfiguration.numberOfSecrets}");
                #endregion

                //sector.introRegionConfiguration.numberOfRareRewardEncounters *= 5;
                //sector.earlyRegionConfiguration.numberOfHighWinds *= 3;
                //sector.midRegionConfiguration.numberOfHighWinds *= 3;
                //sector.lateRegionConfiguration.numberOfHighWinds *= 3;
                //sector.lastRegionConfiguration.numberOfRareRewardEncounters *= 5;
            }
        }
        #endregion

        #region 刷新池
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
        //[HarmonyPatch(typeof(RecyclingWells), "SpawnRecycledItem")]
        //public class RecyclingWellsOverridePatch_SpawnRecycledItem
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(RecyclingWells __instance)
        //    {
        //        FoolUnlockManager manager = Traverse.Create(__instance).Field("foolUnlockManager").GetValue<FoolUnlockManager>();
        //        Transform transform = Traverse.Create(__instance).Field("foolUnlockSpawnPoint").GetValue<Transform>();

        //        if (Traverse.Create(__instance).Field("shouldUnlockFool").GetValue<bool>() && manager.UnlockFoolFromRecyclingPlank(transform.position))
        //        {
        //            Traverse.Create(__instance).Field("shouldUnlockFool").SetValue(false);
        //        }
        //        else
        //        {
        //            PrefabFactory factory = Traverse.Create(__instance).Field("prefabFactory").GetValue<PrefabFactory>();
        //            PrefabEntity prefab = Traverse.Create(__instance).Field("recycledEntity").GetValue<PrefabEntity>();
        //            Pool pool = Traverse.Create(__instance).Field("pool").GetValue<Pool>();
        //            PrefabEntities.Type[] refusedTypes = Traverse.Create(__instance).Field("refusedTypes").GetValue<PrefabEntities.Type[]>();

        //            if (prefab.Types.Contains(PrefabEntities.Type.Trinket))
        //            {
        //                var recycledEntity = pool.GetRandomBy(null, prefab.Types, refusedTypes, new PrefabID[]
        //                {
        //                    prefab.prefab
        //                }, Pool.IncludeRemoved.No, false, true, true, null);
        //                if (!recycledEntity)
        //                {
        //                    recycledEntity = prefab;
        //                }

        //                RecyclingWells.Identity origin = Traverse.Create(__instance).Field("origin").GetValue<RecyclingWells.Identity>();
        //                Transform rightItemFallPoint = Traverse.Create(__instance).Field("rightItemFallPoint").GetValue<Transform>();
        //                Transform leftItemFallPoint = Traverse.Create(__instance).Field("leftItemFallPoint").GetValue<Transform>();
        //                AnimationOverDistance itemSpawnAnimation = Traverse.Create(__instance).Field("itemSpawnAnimation").GetValue<AnimationOverDistance>();
        //                Transform rightItemTransform = Traverse.Create(__instance).Field("rightItemTransform").GetValue<Transform>();
        //                Transform leftItemTransform = Traverse.Create(__instance).Field("leftItemTransform").GetValue<Transform>();

        //                PrefabID prefabID = factory.Create<PrefabID>(recycledEntity.prefab, new GameObjectCreationParameters
        //                {
        //                    Position = new Vector3?((origin != RecyclingWells.Identity.Left) ? rightItemFallPoint.position : leftItemFallPoint.position)
        //                }, null, null);
        //                itemSpawnAnimation.AnimateObject(prefabID.GetComponent<Holdable>(), (origin != RecyclingWells.Identity.Left) ? rightItemTransform.position : leftItemTransform.position);
        //            }

        //        }
        //    }
        //}
        #endregion

        #region 获取游戏信息
        [HarmonyPatch(typeof(GameState), "OnNetworkSpawn")]
        public class GameStateOverridePatch_OnNetworkSpawn
        {
            [HarmonyPostfix]
            public static void Postfix(GameState __instance)
            {
                factory = Traverse.Create(__instance).Field("prefabFactory").GetValue<PrefabFactory>();
                Debug.Log($"ZG:获取物品添加");
                pool = Traverse.Create(__instance).Field("pool").GetValue<PrefabEntities.Pool>();
                Debug.Log($"ZG:获取物品池");
            }
        }
        [HarmonyPatch(typeof(PlayersCenterPoint), "Update")]
        public class PlayersCenterPointOverridePatch_Update
        {
            [HarmonyPostfix]
            public static void Postfix(PlayersCenterPoint __instance)
            {
                player = __instance.transform;
            }
        }
        #endregion

        #region Boss数据
        

        #endregion

        //[HarmonyPatch(typeof(GameState), "IncrementEnemyKillCount")]
        //public class GameStateOverridePatch_IncrementEnemyKillCount
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(GameState __instance, ref int __result)
        //    {
        //        int playerEnemyKillCount = (int)Traverse.Create(__instance).Field("playerEnemyKillCount").GetValue();

        //        Debug.Log($"ZG:修改前;{playerEnemyKillCount}");
        //        Traverse.Create(__instance).Field("playerEnemyKillCount").SetValue(playerEnemyKillCount + 9);
        //        __result = playerEnemyKillCount + 9;

        //        Debug.Log($"ZG:修改后;{Traverse.Create(__instance).Field("playerEnemyKillCount").GetValue()}");
        //    }

        //}

        //[HarmonyPatch(typeof(GameState), "Pay")]
        //public class GameStateOverridePatch_GetPrice
        //{
        //    [HarmonyPrefix]
        //    public static void Prefix(ref int price, Currency currency)
        //    {
        //        price /= 2;
        //    }
        //}
    }
}
