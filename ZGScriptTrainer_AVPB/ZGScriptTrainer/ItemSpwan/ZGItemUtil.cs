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
            return true;
        }
        //获取所有物品
        public static void GetBaseItemData()
        {
            BaseItems.Clear();
            List<ZGItem> items = new List<ZGItem>
            {
                new ZGItem() { ID = 0 },
                new ZGItem() { ID = 1 },
                new ZGItem() { ID = 2 },
                new ZGItem() { ID = 3 },
                new ZGItem() { ID = 4 },
                new ZGItem() { ID = 5 },
                new ZGItem() { ID = 6 },
                new ZGItem() { ID = 7 },
                new ZGItem() { ID = 8 },
                new ZGItem() { ID = 9 },
                new ZGItem() { ID = 10 },
                new ZGItem() { ID = 11 },
                new ZGItem() { ID = int.MaxValue }
            };
            BaseItems[0] = items;
        }
        //获取物品名称
        public static string GetItemName(this ZGItem item)
        {
            return $"ID : {item.ID}:ZG Test Item";
        }
        //获取物品图标
        public static Sprite GetItemIcon(this ZGItem item)
        {
            return ZGUIUtility.createDefaultTexture("#FFFFFF").createSpriteFrmTexture();
        }
        //获取物品解释
        public static string GetItemDescription(this ZGItem item)
        {
            return item.GetItemName() + "\n" + item.GetItemName() + "\n" + item.GetItemName() + "\n" + item.GetItemName() + "\n" + item.GetItemName() + "\n" + item.GetItemName() + "\n" + item.GetItemName();
        }
        //添加物品
        public static void SpwanItem(this ZGItem item)
        {

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
