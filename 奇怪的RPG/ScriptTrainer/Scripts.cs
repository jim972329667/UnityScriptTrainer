using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGameUI;
using static UnityEngine.UI.CanvasScaler;

namespace ScriptTrainer
{
    public class Scripts : MonoBehaviour
    {

        public Scripts()
        {
            
        }
        // 添加现金
        public static void AddMoney()
        {
            UIWindows.SpawnInputDialog("您需要添加多少钱？", "添加", "1000000", (string money) => {
                Singleton<BagManager>.Instance.AddItem(GameConst.MoneyItemId, money.ConvertToIntDef(100000), true);
            });
        }
        public static void RemoveNeedMaterials(bool state)
        {
            ScriptPatch.IsRemoveNeedMaterials = state;
        }
        public static void AddAllItem()
        {
            foreach(int item in Config_Item.data.Keys)
            {
                if (!Singleton<BagManager>.Instance.IsExist(item))
                {
                    Singleton<BagManager>.Instance.AddItem(item);
                }
            }
            
        }
        public static void AddDeadNum()
        {
            UIWindows.SpawnInputDialog("您需要添加多少死亡次数？", "添加", "100", (string dead) => {
                for(int i = 0;i< dead.ConvertToIntDef(100); i++)
                {
                    Singleton<AchivementManager>.Instance.AddUnitDeadNum(GameConst.PlayerUnitId);
                }
            });
        }
    }
}
