using ScriptTrainer.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using Object = UnityEngine.Object;
using HarmonyLib.Tools;
using static Il2CppSystem.DateTimeParse;

namespace ScriptTrainer
{
    internal class ItemWindow : MonoBehaviour
    {
        public static ItemWindow Instance;
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
            Instance = this;
            Panel = panel;
            initialX = elementX = x + 50;
            initialY = elementY = y;
            Initialize();
        }
        public void Initialize()
        {
            //创建搜索框
            Vector2 vector = new Vector2(elementX - 80,Panel.GetComponent<RectTransform>().sizeDelta.y/2 - 30);

            //SearchBar(Panel);
            Panel.AddInputField(ref vector,"搜索：",260,"", (string text) =>
            {
                Debug.Log(text);
                page = 1;
                searchText = text;
                GameObject.Destroy(ItemPanel);
                if (TryGetData())
                    container();
                ItemWindow.uiText.GetComponent<Text>().text = uiText_text;
            });


            elementX += 300;
            elementY += 10;
            hr();

            //创建物品列表
            if (TryGetData())
            {
                container();
            }
            else
            {
                //elementX += 200;
                elementY = 125 - 60 * 5;
            }
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

            if (uiText == null)
            {
                uiText = UIControls.createUIText(pageObj, "#ffFFFFFF");
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
        private void container()
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

            int maxsort = MainWindow.canvas?.GetComponent<Canvas>().sortingOrder ?? Extensions.GetMaxSortingOrder();
            maxsort++;
            int i = 0;
            foreach (var item in GetItemData())
            {
                var btn = CreateItemButton("获得", maxsort, item, ItemPanel, () =>
                {
                    SpawnItem(item);
                });
                ItemButtons.Add(btn);
                i++;
                if (i % 3 == 0)
                {
                    hr();
                }
            }
        }

        private GameObject CreateItemButton(string ButtonText,int sortorder, ItemAttr item, GameObject panel, Action action)
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
            tip.Initialize(GetItemDescription(item), sortorder);
            //tip.WindowSizeFactor = ScriptTrainer.WindowSizeFactor.Value;

            //创建文字
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(background, "#FFFFFFFF");
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

        private void hr()
        {
            elementX = initialX;
            elementY -= 60;
        }

        #endregion
        public  ItemAttr_RangedWeapon GetRangedWeapon(ItemAttr item)
        {
            foreach(var x in ItemManager.instance.rangedWeaponList)
            {
                if(x.itemId == item.itemId)
                    return x;
            }
            return null;
        }
        public  void SpawnItem(ItemAttr item)
        {
            GameData data = GameController.instance.gameData;
            ItemData itemData = ItemManager.instance.GetRandomDataByAttr(item,true);
            itemData.durability = 100;
            itemData.itemNumberFloat = item.stackNumber;
            ScriptTrainer.WriteLog($"{itemData.deployedCamp};{itemData.deployedDirection};{itemData.dropOnTime};{itemData.durability};{itemData.eulerAngles};{itemData.inventoryCoorAry};{itemData.inventoryCoordinate};{itemData.itemNumberFloat};{itemData.rotationZ};{itemData.rangedWeaponAirPressure};{item.itemType}");

            if(item.itemType == ItemType.RangedWeapon)
            {
                ItemAttr_RangedWeapon weapon = GetRangedWeapon(item);
                if(weapon != null)
                {
                    if (itemData.properties[1] != -1)//弹夹ID
                    {
                        itemData.properties[2] = 100;//弹夹耐久
                        itemData.properties[3] = (float)weapon.magazineSize;//弹夹的子弹数量
                    }
                    else
                    {
                        if (!weapon.ammoPlusOne)
                        {
                            itemData.properties[1] = (float)weapon.defaultMagazineId;
                            itemData.properties[2] = 100;
                            itemData.properties[3] = (float)weapon.magazineSize;
                            itemData.properties[4] = 1;//不知道是什么，但有弹夹就有
                        }
                    }
                }
                itemData.properties[0] = 9000;//枪械耐久

                for (int i = 0; i < 13; i++)
                {
                    ScriptTrainer.WriteLog($"{itemData.properties[i]}");
                }
            }
            else if(item.itemType == ItemType.MeleeWeapon)
            {
                itemData.properties[0] = 100;
            }


            if(data.playerData.inventoryData.TryAddItem(itemData))
                UIWindows.MessageDialog($"已成功添加物品：{GetItemName(item)}");
        }



        #region[获取数据相关函数]
        private bool TryGetData()
        {
            if (ItemManager.instance.itemList == null)
            {
                return false;
            }
            return true;
        }
        private List<ItemAttr> GetItemData()
        {

            List<ItemAttr> ItemData = new List<ItemAttr>();
            
            foreach (ItemAttr ingredient in ItemManager.instance.itemList)
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
            List<ItemAttr> list = new List<ItemAttr>();
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
        private List<ItemAttr> FilterItemData(List<ItemAttr> dataList)
        {
            if (searchText == "")
            {
                return dataList;
            }
            List<ItemAttr> list = new List<ItemAttr>();

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
        private string GetItemName(ItemAttr item)
        {
            return item.ItemName;
        }
        private string GetItemDescription(ItemAttr item)
        {
            return item.GetDescription();
        }
        private Sprite GetItemIcon(ItemAttr item)
        {
            return ItemManager.instance.GetItemSprite(item.itemId);
        }
        #endregion
    }

}
