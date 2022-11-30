using HarmonyLib;
using PrefabEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnlockablePermanentsConfiguration;

namespace ScriptTrainer
{
    public class ScriptPatch
    {
        #region[全局参数]
        public static bool IsDiscount = false;
        #endregion

        #region 价格显示，以及价格
        [HarmonyPatch(typeof(Shop), "GetPrice")]
        public class ShopOverridePatch_GetPrice
        {
            [HarmonyPostfix]
            public static void Postfix(ref int __result)
            {
                if(IsDiscount)
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
        #endregion

        //[HarmonyPatch(typeof(GameState), "Pay")]
        //public class GameStateOverridePatch_GetPrice
        //{
        //    [HarmonyPrefix]
        //    public static void Prefix(ref int price, Currency currency)
        //    {
        //        price /= 2;
        //    }
        //}

        //[HarmonyPatch(typeof(Item), "Construct")]
        //public class ItemOverridePatch_Construct
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(Item __instance)
        //    {
        //        bool overridePlankPrice = (bool)Traverse.Create(__instance).Field("overridePlankPrice").GetValue();
        //        bool overrideSandDollarPrice = (bool)Traverse.Create(__instance).Field("overrideSandDollarPrice").GetValue();
        //        bool overrideShardPrice = (bool)Traverse.Create(__instance).Field("overrideShardPrice").GetValue();

        //        if (overridePlankPrice)
        //        {
        //            int plankPrice = (int)Traverse.Create(__instance).Field("plankPrice").GetValue();
        //            Traverse.Create(__instance).Field("plankPrice").SetValue(plankPrice / 2);
        //        }
        //        if (overrideSandDollarPrice)
        //        {
        //            int sandDollarPrice = (int)Traverse.Create(__instance).Field("sandDollarPrice").GetValue();
        //            Traverse.Create(__instance).Field("sandDollarPrice").SetValue(sandDollarPrice / 2);
        //        }
        //        if (overrideShardPrice)
        //        {
        //            int shardPrice = (int)Traverse.Create(__instance).Field("shardPrice").GetValue();
        //            Traverse.Create(__instance).Field("shardPrice").SetValue(shardPrice / 2);
        //        }
        //    }

        //}

        //[HarmonyPatch(typeof(Prices), "GetPrice")]
        //public class PricesOverridePatch_GetPrice
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(ref int __result)
        //    {
        //        __result /= 2;
        //    }

        //}

        //[HarmonyPatch(typeof(ShopShowcase), "Price", MethodType.Getter)]
        //class ShopShowcasePricePatch
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(ref int __result)
        //    {
        //        __result /= 2;       
        //    }
        //}

        //[HarmonyPatch(typeof(PriceTag), "Price", MethodType.Getter)]
        //class PriceTagPricePatch
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(ref int __result)
        //    {
        //        __result /= 2;
        //    }
        //}
    }
}
