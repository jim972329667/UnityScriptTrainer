using HarmonyLib;
using System.Collections.Generic;
using static UnityEngine.EventSystems.EventTrigger;

namespace ScriptTrainer
{
    public class ScriptPatch
    {
        #region[全局参数]
        public static bool IsRemoveNeedMaterials = false;
        #endregion

        [HarmonyPatch(typeof(GameManager), nameof(GameManager.BuyItem))]
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
            }

        }

    }
}
