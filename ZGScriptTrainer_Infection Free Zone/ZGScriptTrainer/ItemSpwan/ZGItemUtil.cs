using Commands;
using Controllers;
using Core.States;
using Data.Resorces;
using Gameplay.GameResources;
using Gameplay.Units.Equipment;
using Gameplay.Vehicles;
using Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniverseLib.UI;
using ZGScriptTrainer.UI;
using ZGScriptTrainer.UI.Models;
using ZGScriptTrainer.UI.Panels;

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
            "全部",
            "资源",
            "武器",
            "板条箱",
            "汽车"
        };
        //判断是否可以获取所有物品
        public static bool CanGetItemData()
        {
            return ResourcesDataContainer.Instance != null;
        }
        //获取所有物品
        public static void GetBaseItemData()
        {
            BaseItems.Clear();

            for (int i = 0;i< ItemTypeDic.Count; i++)
            {
                BaseItems.Add(i, new List<ZGItem>());
            }

            foreach (var x in Enum.GetValues(typeof(ResourceID)))
            {
                ResourceID t = (ResourceID)x;
                if(t != ResourceID.None && t != ResourceID.res_unknown && t != ResourceID.eq_molotov)
                {
                    var tmp = ResourcesDataContainer.Instance.GetResourceDataByType(t);

                    if (tmp as Weapon != null)
                        BaseItems[2].Add(new ZGItem() { Type = ItemTypeDic[2], ID = t });
                    else if (tmp as Gear != null)
                        BaseItems[3].Add(new ZGItem() { Type = ItemTypeDic[3], ID = t });
                    else if (!t.ToString().StartsWith("eq_inf"))
                        BaseItems[1].Add(new ZGItem() { Type = ItemTypeDic[1], ID = t });
                    else
                        continue;
                }
            }
            
            foreach (var x in Enum.GetValues(typeof(VehicleType)))
            {
                VehicleType t = (VehicleType)x;
                if (t != VehicleType.None)
                {
                    BaseItems[4].Add(new ZGItem() { Type = ItemTypeDic[4], VehicleType = t });
                }
            }


            var list = new List<ZGItem>();
            for (int i = 1; i < BaseItems.Count; i++)
            {
                list.AddRange(BaseItems[i]);
            }
            BaseItems[0] = list;
        }
        //获取物品名称
        public static string GetItemName(this ZGItem item)
        {
            if(item.Type == ItemTypeDic[4])
            {
                string vehicleKey = "VehicleDefaultNames.Name." + item.VehicleType.ToString();
                string vehicleName = LEManager.Get(vehicleKey, true);
                if (vehicleName == vehicleKey)
                {
                    vehicleName = LEManager.Get("VehicleDefaultNames.Name.Default", false);
                }
                return vehicleName;
            }
            var tmp = ResourcesDataContainer.Instance.GetResourceDataByType(item.ID);
            return tmp != null ? tmp.GetName() : "";
        }
        //获取物品图标
        public static Sprite GetItemIcon(this ZGItem item)
        {
            if (item.Type == ItemTypeDic[4])
            {
                if(ScriptPatch.vehiclesStats != null)
                    return ScriptPatch.vehiclesStats.Get(item.VehicleType)?.GroupIcons.iconSprite;
            }
            var tmp = ResourcesDataContainer.Instance.GetResourceDataByType(item.ID);
            return tmp != null ? tmp.SpriteUI : null;
        }
        //获取物品解释
        public static string GetItemDescription(this ZGItem item)
        {
            if (item.Type == ItemTypeDic[4])
            {
                return "";
            }
            var tmp = ResourcesDataContainer.Instance.GetResourceDataByType(item.ID);
            return tmp != null ? tmp.GetDescription() : "";
        }
        //添加物品
        public static void SpwanItem(this ZGItem item)
        {
            if(GameConsoleCommandHandler.Instance != null)
            {
                if (item.Type == ItemTypeDic[4])
                {
                    UIManager.WorldToolTip.GetComponent<TooltipGUI>().SetText("鼠标左键选择地面添加汽车[鼠标右键退出]");
                    UIManager.WorldToolTip.GetComponent<TooltipGUI>().EnableTooltip = true;
                    GameConsoleCommandHandler.Instance.SpawnVehicle(item.VehicleType);
                    return ;
                }
                //UIManager.ToolTip.Initialize("鼠标左键选择建筑添加资源[鼠标右键退出]");
                //UIManager.ToolTip.EnableTooltip = true;
                BuildingsController.MainHeadquarter.ResourcesContainer.ForceAddResource(item.ID, item.Count);
            }
        }
        #endregion


        public static List<ZGItem> GetTypeItems(int ItemType = -1)
        {
            List<ZGItem> result = new List<ZGItem>();
            if (ItemType < 0)
            {
                if (BaseItems.Count > 0)
                    result.AddRange(BaseItems[0]);
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
