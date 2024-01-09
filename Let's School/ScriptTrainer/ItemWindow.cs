using ScriptTrainer.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using Object = UnityEngine.Object;
using HarmonyLib.Tools;
using ProjectSchoolNs.WorldNs;
using ProjectSchoolNs.SchoolNs;
using static ProjectSchoolNs.WorldNs.PlaceableBase;

namespace ScriptTrainer
{
    internal class ItemWindow : MonoBehaviour
    {
        private static GameObject Panel;
        private static int initialX;
        private static int initialY;
        private static int elementX;
        private static int elementY;

        #region[数据分页相关]
        private static GameObject ItemPanel;
        private static List<GameObject> ItemButtons = new List<GameObject>();
        private static int page = 1;
        private static int maxPage = 1;
        private static int conunt = 15;
        private static string searchText = "";
        private static GameObject uiText;
        private static string uiText_text
        {
            get
            {
                return $"{page} / {maxPage}";
            }
        }
        #endregion

        public ItemWindow(GameObject panel, int x, int y)
        {
            Panel = panel;
            initialX = elementX = x + 50;
            initialY = elementY = y;
            Initialize();
        }
        public void Initialize()
        {
            //创建搜索框
            Vector2 vector = new Vector2(elementX - 80, Panel.GetComponent<RectTransform>().sizeDelta.y / 2 - 30);

            //SearchBar(Panel);
            Panel.AddInputField(ref vector, "搜索：", 260, "", (string text) =>
            {
                Debug.Log(text);
                page = 1;
                searchText = text;
                Destroy(ItemPanel);
                if (TryGetData())
                    container();
                ItemWindow.uiText.GetComponent<Text>().text = uiText_text;
            });


            elementX += 300;
            elementY += 10;
            hr();

            //创建物品列表
            //if (TryGetData())
            //{
            //    container();
            //}
            //else
            //{
            //    elementX += 200;
            elementY = 125 - 60 * 5;
            //}
            //创建分页

            PageBar(Panel);
        }

        #region[创建详细]
        //分页
        private void PageBar(GameObject panel)
        {
            //背景
            GameObject pageObj = UIControls.createUIPanel(panel, "40", "500");
            pageObj.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            pageObj.GetComponent<RectTransform>().localPosition = new Vector3(0, elementY, 0);

            //当前页数 / 总页数
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));

            if (uiText == null)
            {
                uiText = UIControls.createUIText(pageObj, txtBgSprite, "#ffFFFFFF");
                uiText.GetComponent<Text>().text = uiText_text;
                uiText.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                //设置字体
                uiText.GetComponent<Text>().fontSize = 20;
                uiText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            }


            //上一页
            string backgroundColor = "#8C9EFFFF";
            GameObject prevBtn = UIControls.createUIButton(pageObj, backgroundColor, "上一页", () =>
            {

                page--;
                if (page <= 0) page = 1;
                if (TryGetData())
                    container();
                uiText.GetComponent<Text>().text = uiText_text;
            }, new Vector3());
            prevBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 20);
            prevBtn.GetComponent<RectTransform>().localPosition = new Vector3(-100, 0, 0);

            //下一页
            GameObject nextBtn = UIControls.createUIButton(pageObj, backgroundColor, "下一页", () =>
            {
                page++;
                if (page >= maxPage) page = maxPage;
                if (TryGetData())
                    container();
                uiText.GetComponent<Text>().text = uiText_text;
            });
            nextBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 20);
            nextBtn.GetComponent<RectTransform>().localPosition = new Vector3(100, 0, 0);
        }
        private static void container()
        {
            elementX = -200;
            elementY = 125;

            //先清空旧的 ItemPanel
            foreach (var item in ItemButtons)
            {
                UnityEngine.Object.Destroy(item);
            }
            ItemButtons.Clear();

            ItemPanel = UIControls.createUIPanel(Panel, "300", "600");
            ItemPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            ItemPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, 0);

            int num = 0;
            foreach (PlaceableBase item in GetItemData())
            {
                var btn = CreateItemButton("获得", item, ItemPanel, () =>
                {
                    SpawnItem(item);
                });
                ItemButtons.Add(btn);
                num++;
                if (num % 3 == 0)
                {
                    hr();
                }
            }
        }

        private static GameObject CreateItemButton(string ButtonText, PlaceableBase item, GameObject panel, Action action)
        {
            //按钮宽 200 高 50
            int buttonWidth = 190;
            int buttonHeight = 50;

            //根据品质设置背景颜色
            string qualityColor = "#FFFFFFFF";

            //创建一个背景
            GameObject background = UIControls.createUIPanel(panel, buttonHeight.ToString(), buttonWidth.ToString(), null);
            background.GetComponent<Image>().color = UIControls.HTMLString2Color("#455A64FF");
            background.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);

            GameObject background_icon = UIControls.createUIPanel(background, buttonHeight.ToString(), "50", null);
            background_icon.GetComponent<Image>().color = UIControls.HTMLString2Color(qualityColor);
            background_icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(70, 0);

            GameObject icon = UIControls.createUIImage(background_icon, GetItemIcon(item));
            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
            icon.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);

            var tip = background.AddComponent<TooltipGUI>();
            tip.Tooltip.Add(GetItemDescription(item).GetSeparateString(25));
            tip.WindowSizeFactor = ScriptTrainer.WindowSizeFactor.Value;

            //创建文字
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(background, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<Text>().text = GetItemName(item);
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(0, 5, 0);

            //创建按钮
            string backgroundColor_btn = "#8C9EFFFF";
            GameObject button = UIControls.createUIButton(background, backgroundColor_btn, ButtonText, action, new Vector3());
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 20);
            button.GetComponent<RectTransform>().localPosition = new Vector3(-50, -10, 0);

            elementX += 200;

            return background;
        }

        private static void hr()
        {
            elementX = initialX;
            elementY -= 60;
        }

        #endregion

        public static void SpawnItem(PlaceableBase item)
        {
            SchoolModule.instance.UnlockFurniture(item, false);
            //UIWindows.MessageDialog($"已成功添加物品：{GetItemName(item)}");
        }



        #region[获取数据相关函数]
        private static bool TryGetData()
        {
            if (MapModule.Instance.MapFurnitureSubModule.loadedFurnitureTemplates == null)
            {
                return false;
            }
            return true;
        }
        private static List<PlaceableBase> GetItemData()
        {

            List<PlaceableBase> ItemData = new List<PlaceableBase>();

            foreach (PlaceableBase ingredient in MapModule.Instance.MapFurnitureSubModule.loadedFurnitureTemplates.Values)
            {
                ItemData.Add(ingredient);
            }

            Debug.Log($"ZG:全物品数量:{ItemData.Count}");
            if (searchText != "")
            {
                ItemData = FilterItemData(ItemData);
            }
            //Curse诅咒 Magazine
            //对 DataList 进行分页
            List<PlaceableBase> list = new List<PlaceableBase>();
            int start = (page - 1) * conunt;
            int end = start + conunt;
            for (int i = start; i < end; i++)
            {
                if (i < ItemData.Count)
                {
                    list.Add(ItemData[i]);
                }
            }
            if (ItemData.Count % conunt != 0)
                maxPage = ItemData.Count / conunt + 1;
            else
                maxPage = ItemData.Count / conunt;

            return list;
        }
        //搜索过滤
        private static List<PlaceableBase> FilterItemData(List<PlaceableBase> dataList)
        {
            if (searchText == "")
            {
                return dataList;
            }
            List<PlaceableBase> list = new List<PlaceableBase>();

            foreach (var item in dataList)
            {
                string text = GetItemName(item);
                string text2 = GetItemDescription(item);
                if (text.Contains(searchText.Replace(" ", "")) || text2.Contains(searchText.Replace(" ", "")))
                {
                    list.Add(item);
                }
            }

            return list;
        }
        private static string GetUIPanelName(UIPanelType uIPanel)
        {
            switch (uIPanel)
            {
                case UIPanelType.common:
                    return "一般";
                case UIPanelType.especial:
                    return "特殊";
                case UIPanelType.decorate:
                    return "装饰";
                case UIPanelType.storage:
                    return "存储";
                case UIPanelType.hygiene:
                    return "卫生";
                case UIPanelType.entertainment:
                    return "环境";
                case UIPanelType.windowsAndDoors:
                    return "门窗";
                case UIPanelType.other:
                    return "其他";
                case UIPanelType.all:
                    return "全部";
                case UIPanelType.roof:
                    return "房顶";
                case UIPanelType.plants:
                    return "户外植物";
                case UIPanelType.light:
                    return "照明";
                case UIPanelType.safety:
                    return "安全";
                case UIPanelType.publicChair:
                    return "公共座椅";
                case UIPanelType.stairs:
                    return "楼梯";
                case UIPanelType.pillar:
                    return "柱子";
                case UIPanelType.hidden:
                    return "隐藏";
                case UIPanelType.temperature:
                    return "温度";
                case UIPanelType.diningAndDrinking:
                    return "饮食";
                default:return "未知";
            }
        }
        private static string GetItemName(PlaceableBase item)
        {
            return $"({GetUIPanelName(item.uIPanelType)}){item.FurnitureNameI18n.GetText()}";
        }
        private static string GetItemDescription(PlaceableBase item)
        {
            return item.CommentsI18n.GetText();
        }
        private static Sprite GetItemIcon(PlaceableBase item)
        {
            return item.icon;
        }
        #endregion
    }

}
