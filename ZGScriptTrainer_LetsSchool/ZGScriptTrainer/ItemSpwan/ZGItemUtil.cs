using FrameworkNs;
using Lanka.GameItemSystem;
using Lanka.OrganizationSystem;
using ProjectSchoolNs;
using ProjectSchoolNs.GameItemNs;
using ProjectSchoolNs.SchoolNs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ZGScriptTrainer.UI;

namespace ZGScriptTrainer.ItemSpwan
{
    public static class ZGItemUtil
    {
        public const string DefaultSearchText = "请输入。。。";
        public static Dictionary<int, List<ZGItem>> BaseItems { get; private set; } = new Dictionary<int, List<ZGItem>>();

        #region[每个游戏都需要修改]
        //物品种类
        public static readonly List<string> ItemTypeDic = new List<string>()
        {
            "遗物",
            "药剂",
            "法术"
        };
        //判断是否可以获取所有物品
        public static bool CanGetItemData()
        {
            if (GameModeModule.instance != null && GameModeModule.instance.gameModeConfig != null && GameModeModule.instance.gameModeConfig.initialGameItem.Count > 0 && SchoolBagModule.instance != null && SchoolBagModule.instance.SchoolgameItem != null && SchoolBagModule.instance.SchoolgameItem.Count > 0 && GameItemModule.instance != null)
                return true;
            return false;
        }
        //获取所有物品
        public static void GetBaseItemData()
        {
            BaseItems.Clear();
            List<ZGItem> items = new();
            foreach (var x in GameModeModule.instance.gameModeConfig.initialGameItem)
            {
                foreach(var y in x.itemGroup)
                {
                    items.Add(new ZGItem() { BaseItem = y, ID = y.Id, Type = "物品" });
                }
            }
            ZGScriptTrainer.WriteLog($"GameModeModule Success : {items.Count}");
            ZGScriptTrainer.WriteLog($"GameStateModule : {Singleton<GameManagerV2>.Instance.State}");
            BaseItems[0] = items;

            List<ZGItem> items2 = new();
            var tmp = SchoolBagModule.instance;
            ZGScriptTrainer.WriteLog("SchoolModule Start");
            if (tmp != null)
            {
                if (tmp.schoolgameItem != null)
                {
                    foreach (var item in tmp.schoolgameItem)
                    {
                        items2.Add(new ZGItem() { BaseItem = item.template, ID = item.template.Id, Type = "物品" });
                    }
                }
            }
            ZGScriptTrainer.WriteLog($"SchoolModule Success : {items2.Count}");
            BaseItems[1] = items2;

            List<ZGItem> items3 = new();
            foreach (var x in typeof(GameItemModule).GetProperty("loadedNamrGameItems").GetValue(GameItemModule.instance, null) as Il2CppSystem.Collections.Generic.Dictionary<System.String,GameItem>)
            {
                items3.Add(new ZGItem() { BaseItem = x.Value, ID = x.Value.Id, Type = "物品" });
                //foreach (var y in x.Value)
                //{
                //    //foreach (var z in typeof(GameItemStack).GetProperty("items").GetValue(y, null) as Il2CppSystem.Collections.Generic.Dictionary<int, GameItemInstance>)
                //    //{
                //    //    items3.Add(new ZGItem() { BaseItem = z.Value.template, ID = z.Value.template.Id, Type = "物品" });
                //    //}
                //    //ZGScriptTrainer.WriteLog($"GameItemStack : {typeof(GameItemStack).GetProperty("items").GetValue(y, null)}");
                //}
            }
            BaseItems[2] = items3;
        }
        //获取物品名称
        public static string GetItemName(this ZGItem item)
        {
            return item.BaseItem.GetName();
        }
        //获取物品图标
        public static Sprite GetItemIcon(this ZGItem item)
        {
            return item.BaseItem.Icon ?? ZGUIUtility.createDefaultTexture("#FFFFFF").createSpriteFrmTexture();
        }
        //获取物品解释
        public static string GetItemDescription(this ZGItem item)
        {
            return item.BaseItem.GetComments();
        }
        //添加物品
        public static void SpwanItem(this ZGItem item)
        {
            if(GameItemModule.instance.curCharacter == null)
            SchoolBagModule.instance.AddItem(item.BaseItem, item.Count);
            else
            {
                for (int i = 0;i< item.Count; i++)
                {
                    GameItemModule.instance.curCharacter.AddBagItem(item.BaseItem);
                }
            }
        }
        #endregion


        public static List<ZGItem> GetTypeItems(int ItemType = -1)
        {
            List<ZGItem> result = new List<ZGItem>();
            if (ItemType < 0)
            {
                if (BaseItems.Count > 0)
                    foreach (var item in BaseItems)
                    {
                        result.AddRange(item.Value);
                    }
            }
            else
            {
                BaseItems.TryGetValue(ItemType, out result);
            }
            return result ?? new List<ZGItem>();
        }
        public static List<ZGItem> FilterItemData(this string text, int type = -1)
        {
            if (!string.IsNullOrEmpty(text) && text != DefaultSearchText)
            {
                List<ZGItem> list = new List<ZGItem>();

                foreach (var item in GetTypeItems(type))
                {
                    string tmp1 = GetItemName(item);
                    string tmp2 = GetItemDescription(item);
                    if (tmp1 != null && tmp1.Contains(text.Replace(" ", "")))
                    {
                        list.Add(item);
                    }
                    else if (tmp2 != null && tmp2.Contains(text.Replace(" ", "")))
                    {
                        list.Add(item);
                    }
                }

                return list;
            }
            else { return GetTypeItems(type); }
        }

    }
}
