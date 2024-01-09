using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnlockablePermanentsConfiguration;
using UnityEngine;
using Zenject.Extensions;
using ZGScriptTrainer.UI.Panels;

namespace ZGScriptTrainer
{
    public class ScriptPatch
    {
        #region[全局参数]
        public static IPrefabFactory factory = null;
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
                if (ZGScriptTrainer.IsDiscount.Value)
                    __result /= 2;
            }

        }

        [HarmonyPatch(typeof(Market), "GetPrices")]
        public class MarketOverridePatch_GetPrices
        {
            [HarmonyPostfix]
            public static void Postfix(ref int __result)
            {
                if (ZGScriptTrainer.IsDiscount.Value)
                    __result /= 2;
            }

        }
        [HarmonyPatch(typeof(Carpentry), "GetPrice")]
        public class CarpentryOverridePatch_GetPrice
        {
            [HarmonyPostfix]
            public static void Postfix(ref int __result)
            {
                if (ZGScriptTrainer.IsDiscount.Value)
                    __result /= 2;
            }

        }

        //[HarmonyPatch(typeof(UnlockablePermanentConfiguration), "GetPrice")]
        //public class UnlockablePermanentConfigurationOverridePatch_GetPrice
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(ref int? __result)
        //    {
        //        if (IsDiscount)
        //            if (__result.HasValue)
        //                __result = new Nullable<int>(__result.Value / 2);
        //    }

        //}

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

        #region 获取游戏信息
        [HarmonyPatch(typeof(GameState), "OnNetworkSpawn")]
        public class GameStateOverridePatch_OnNetworkSpawn
        {
            [HarmonyPostfix]
            public static void Postfix(GameState __instance)
            {
                factory = __instance.prefabFactory;
                Debug.Log($"ZG:获取物品添加");
                pool = __instance.pool;
                Debug.Log($"ZG:获取物品池:{pool}");
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

    }
}
