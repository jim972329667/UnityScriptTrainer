using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace ScriptTrainer
{
    public class ScriptPatch
    {
        #region[全局参数]
        public static bool IsRemoveNeedMaterials = false;
        public static bool IsDropMoney = false;
        public static float DropMoneyRate = 1;
        #endregion

        [HarmonyPatch(typeof(GameManager), "BuyItem")]
        public class GameManagerOverridePatch_BuyItem
        {
            [HarmonyPrefix]
            public static void Prefix(int itemId, int buyNum = 1)
            {
                Config_Item itemInfo = Config_Item.GetItemInfo(itemId);
                if (IsRemoveNeedMaterials)
                {
                    foreach (KeyValuePair<int, int> keyValuePair in itemInfo.needMaterialsDic)
                    {
                        
                        int key = keyValuePair.Key;
                        int num = keyValuePair.Value * buyNum;
                        if (key != GameConst.MoneyItemId)
                            Singleton<BagManager>.Instance.AddItem(key, num, true);
                    }
                }
                if(itemInfo.itemType == ItemType.Material)
                {
                    Singleton<BagManager>.Instance.RemoveItem(GameConst.MoneyItemId, ItemWindow.BuyPrice(itemInfo.quality) * buyNum, true);
                }
            }

        }
        [HarmonyPatch(typeof(DropManager), "DropMoney")]
        public class DropManagerOverridePatch_DropMoney
        {
            [HarmonyPrefix]
            public static void Prefix(ref int moneyNum)
            {
                if (IsDropMoney)
                {
                    moneyNum = (int)(DropMoneyRate * moneyNum);
                }
            }
        }
    }
}
