using Il2CppSystem.Linq;
using PrefabEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using Zenject.Extensions;
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
            return ScriptPatch.pool != null;
        }
        //获取所有物品
        public static void GetBaseItemData()
        {
            var ItemData = ScriptPatch.pool.GetAllBy(includeLocked: Pool.IncludeLocked.Yes,includeRemoved: Pool.IncludeRemoved.Yes).ToList();

            BaseItems.Clear();
            var items = new List<ZGItem>();
            foreach (var item in ItemData)
            {
                var ZGItem = new ZGItem
                {
                    Prefab = item
                };
                if (item.Types.Contains(PrefabEntities.Type.Resource))
                {
                    if (!item.Types.Contains(PrefabEntities.Type.Unlockable))
                    {
                        items.Add(ZGItem);
                    }
                }

                if (item.Types.Contains(PrefabEntities.Type.Item))
                {
                    if (!item.Types.Contains(PrefabEntities.Type.Unlockable) && !item.Types.Contains(PrefabEntities.Type.Curse))
                    {
                        items.Add(ZGItem);
                    }
                }
                if (item.Types.Contains(PrefabEntities.Type.Cannon))
                {
                    items.Add(ZGItem);
                }
            }
            BaseItems[0] = items;
            ZGScriptTrainer.WriteLog($"成功获取物品{items.Count}");
        }
        //获取物品名称
        public static string GetItemName(this ZGItem item)
        {
            try
            {
                Item item1 = item.Prefab.prefab.GetComponent<Item>();
                return item1?.itemDescription.itemName.GetLocalizedString() ?? item.Prefab.name;
            }
            catch
            {
                return item.Prefab.name;
            }
        }
        //获取物品图标
        public static Sprite GetItemIcon(this ZGItem item)
        {
            Sprite sprite = null;
            try
            {
                Item item1 = item.Prefab.prefab.GetComponent<Item>();
                if (item1 != null)
                {
                    sprite = item1.itemDescription.sprite;
                }
            }
            catch
            {

            }
            return sprite;
        }
        //获取物品解释
        public static string GetItemDescription(this ZGItem item)
        {
            try
            {
                Item item1 = item.Prefab.prefab.GetComponent<Item>();
                return item1?.itemDescription.description.GetLocalizedString() ?? GetItemName(item);
            }
            catch
            {
                return GetItemName(item);
            }
        }
        //添加物品
        public static void SpwanItem(this ZGItem item)
        {
            if (ScriptPatch.factory != null && ScriptPatch.player != null)
            {
                for(int i = 0; i < item.Count; i++)
                {
                    GameObject prefabID = ScriptPatch.factory.Create(item.Prefab.prefab.gameObject);
                    //prefabID.transform.position = ScriptPatch.player.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
                    prefabID.transform.localRotation = ScriptPatch.player.localRotation;
                    prefabID.transform.localPosition = ScriptPatch.player.localPosition + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));

                    ZGScriptTrainer.WriteLog($"ZG:添加物品{prefabID}");
                    foreach (var type in item.Prefab.Types)
                    {
                        ZGScriptTrainer.WriteLog($"ZG:添加物品类型{type}");
                    }
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
