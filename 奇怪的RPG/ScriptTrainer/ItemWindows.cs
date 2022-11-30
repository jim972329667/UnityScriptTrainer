using BattleDrakeStudios.ModularCharacters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityGameUI;
using Object = UnityEngine.Object;

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
        private static bool RemoveBugItem = false;
        private static List<int> ShopItems = new List<int>();
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
            //Debug.Log(DataList.Count.ToString());
            // 创建搜索框
            searchBar(Panel);
            elementX += 270;
            AddToggle("移除背包已拥有装备", 150, Panel, (bool state) =>
            {
                RemoveBugItem = state;
                page = 1;
                container();
            });
            elementY += 10;
            hr();

            // 创建物品列表
            container();


            // 创建分页
            pageBar(Panel);
        }

        #region[创建详细]

        // 搜索框
        private void searchBar(GameObject panel)
        {
            elementY += 10;

            // label
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(panel, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<Text>().text = "搜索";
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);
            //uiText.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 30);
            uiText.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

            // 坐标偏移
            elementX += 10;

            // 输入框
            int w = 260;
            Sprite inputFieldSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject uiInputField = UIControls.createUIInputField(panel, inputFieldSprite, "#FFFFFFFF");
            uiInputField.GetComponent<InputField>().text = searchText;
            uiInputField.GetComponent<RectTransform>().localPosition = new Vector3(elementX + 100, elementY, 0);
            uiInputField.GetComponent<RectTransform>().sizeDelta = new Vector2(w, 30);

            
            // 文本框失去焦点时触发方法
            uiInputField.GetComponent<InputField>().onEndEdit.AddListener((string text) =>
            {
                //Debug.Log(text);
                page = 1;
                searchText = text;
                container();
                ItemWindow.uiText.GetComponent<Text>().text = uiText_text;
                //Destroy(ItemPanel);
            });
        }

        // 分页
        private void pageBar(GameObject panel)
        {
            // 背景
            GameObject pageObj = UIControls.createUIPanel(panel, "40", "500");
            pageObj.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            pageObj.GetComponent<RectTransform>().localPosition = new Vector3(0, elementY, 0);

            // 当前页数 / 总页数
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));

            if (uiText == null)
            {
                uiText = UIControls.createUIText(pageObj, txtBgSprite, "#ffFFFFFF");
                uiText.GetComponent<Text>().text = uiText_text;
                uiText.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                // 设置字体
                uiText.GetComponent<Text>().fontSize = 20;
                uiText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            }


            // 上一页
            string backgroundColor = "#8C9EFFFF";
            GameObject prevBtn = UIControls.createUIButton(pageObj, backgroundColor, "上一页", () =>
            {

                page--;
                if (page <= 0) page = 1;
                container();
                uiText.GetComponent<Text>().text = uiText_text;
            }, new Vector3());
            prevBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 20);
            prevBtn.GetComponent<RectTransform>().localPosition = new Vector3(-100, 0, 0);

            // 下一页            
            GameObject nextBtn = UIControls.createUIButton(pageObj, backgroundColor, "下一页", () =>
            {
                page++;
                if (page >= maxPage) page = maxPage;
                container();
                uiText.GetComponent<Text>().text = uiText_text;
            });
            nextBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 20);
            nextBtn.GetComponent<RectTransform>().localPosition = new Vector3(100, 0, 0);
        }

        private static void container()
        {
            //Debug.Log($"x:{elementX}, y:{elementY}");
            elementX = -200;
            elementY = 125;

            // 先清空旧的 ItemPanel
            foreach (var item in ItemButtons)
            {
                UnityEngine.Object.Destroy(item);
            }
            ItemButtons.Clear();


            ItemPanel = UIControls.createUIPanel(Panel, "300", "600");
            ItemPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            ItemPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, 0);

            int num = 0;
            foreach (Config_Item item in GetItemData())
            {
                if(item.itemType == ItemType.Equipment)
                {
                    var btn = createItemButton(item.itemName, ItemPanel, item.icon, item.quality, GetPrice(item), () =>
                    {
                        SpawnEquipmentInputDialog("购买", item.id, (string count) =>
                        {
                            BuyItem(item.id, count.ConvertToIntDef(1));
                        });
                    });
                    ItemButtons.Add(btn);
                }
                else
                {
                    var btn = createItemButton(item.itemName, ItemPanel, item.icon, item.quality, GetPrice(item), () =>
                    {
                        UIWindows.SpawnInputDialog($"您想购买多少个{item.itemName}？", "购买", "1", (string count) =>
                        {
                            Debug.Log($"已购买{count}个{item.itemName}到背包");
                            Singleton<GameManager>.Instance.BuyItem(item.id, count.ConvertToIntDef(1));
                        });
                    });
                    ItemButtons.Add(btn);
                }

                num++;
                if (num % 3 == 0)
                {
                    hr();
                }
            }
        }
        public static void SpawnEquipmentInputDialog(string title, int id, Action<string> onFinish)
        {
            GameObject canvas = UIControls.createUICanvas();    // 创建画布
            Object.DontDestroyOnLoad(canvas);
            // 设置置顶显示
            canvas.GetComponent<Canvas>().overrideSorting = true;
            canvas.GetComponent<Canvas>().sortingOrder = 100;

            string xx = StrTool.GetPlayerItemInfoStr(id).Replace("color", "").Replace("/", "").Replace("=", "");
            var xxx = xx.Split('<', '>');
            List<string> outline = new List<string>();
            for (int i = 0; i < xxx.Length; i++)
            {
                if (xxx[i].Contains('\n'))
                {
                    outline.Add("#FFFFFF");
                    string[] tmps = xxx[i].Split('\n');
                    foreach (string tmp in tmps)
                    {
                        string tmp_2 = tmp.Replace(" ", "").Trim('\r', '\n');
                        if (tmp_2.Length > 1)
                        {
                            outline.Add(tmp_2);
                        }
                    }
                }
                else
                {
                    string text = xxx[i].Replace(" ", "").Trim('\r', '\n');
                    if (text.Length > 1)
                    {
                        if(text.Length > 20)
                        {
                            var tmps = GetSeparateSubString(text, 20);
                            foreach(var tmp in tmps)
                            {
                                outline.Add(tmp.ToString());
                            }
                        }
                        else
                            outline.Add(text);
                    }
                }
            }
            for (int i = 0; i < outline.Count; i++)
            {
                if (outline[i].Contains("受到伤害增加"))
                {
                    if (i + 1 < outline.Count)
                    {
                        outline[i + 1] = "";
                    }
                    if(i + 2 < outline.Count)
                    {
                        outline[i] += outline[i + 2];
                        outline[i + 2] = "";
                    }
                }
            }
            int size = GetLineCount(outline) * 20 + 20 + 40;

            

            int index = 0;
            foreach(var txt in outline)
            {
                Debug.Log($"{index}:{txt}");
                index++;
            }

            GameObject uiPanel = UIControls.createUIPanel(canvas, size.ToString(), "300", null);  // 创建面板
            uiPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#37474FFF"); // 设置背景颜色

            // 创建标题
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));


            string DefaultColor = "#FFFFFFFF";
            

            
            int j = 0;
            foreach(var line in outline)
            {
                if (line.StartsWith("#"))
                {
                    DefaultColor = line + "FF";
                }
                else if (line == "green")
                {
                    DefaultColor = ColorUtility.ToHtmlStringRGBA(Color.green);
                }
                else if(line == "red")
                {
                    DefaultColor = ColorUtility.ToHtmlStringRGBA(Color.red);
                }
                else if(line != "")
                {
                    GameObject uiText = UIControls.createUIText(uiPanel, txtBgSprite, DefaultColor);
                    uiText.GetComponent<Text>().text = line;
                    uiText.GetComponent<RectTransform>().localPosition = new Vector2(0, size / 2 - 20 - 20 * j);
                    uiText.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 20);
                    uiText.GetComponent<Text>().fontSize = 14;
                    j++;
                }
            }

            // 创建确定按钮
            GameObject uiButton = UIControls.createUIButton(uiPanel, "#8C9EFFFF", title, () =>
            {
                onFinish("1");
                Object.Destroy(canvas);
            }, new Vector3(100, -size / 2 + 30, 0));

            // 创建关闭按钮
            GameObject closeButton = UIControls.createUIButton(uiPanel, "#B71C1CFF", "X", () =>
            {
                Object.Destroy(canvas);
            }, new Vector3(350 / 2 - 10, size / 2 - 10, 0));
            // 设置closeButton宽高
            closeButton.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            // 字体颜色为白色
            closeButton.GetComponentInChildren<Text>().color = UIControls.HTMLString2Color("#FFFFFFFF");
        }
        private static GameObject createItemButton(string text, GameObject panel, string itemIcon, QualityType quality,int money, UnityAction action)
        {
            // 按钮宽 200 高 50
            int buttonWidth = 190;
            int buttonHeight = 50;

            // 根据品质设置背景颜色
            string qualityColor = "#FFFFFFFF";
            qualityColor = StrTool.GetQuailityColorStr(quality);
            //qualityColor = GetQuailityColor(quality);

            // 创建一个背景
            GameObject background = UIControls.createUIPanel(panel, buttonHeight.ToString(), buttonWidth.ToString(), null);
            background.GetComponent<Image>().color = UIControls.HTMLString2Color("#455A64FF");
            background.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);


            GameObject background_icon = UIControls.createUIPanel(background, buttonHeight.ToString(), "50", null);
            background_icon.GetComponent<Image>().color = UIControls.HTMLString2Color(qualityColor);
            background_icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(70, 0);
            // 创建图标  60x60
            Sprite BgSprite = null;
            Singleton<ResManager>.Instance.Load<Sprite>(itemIcon, delegate (Sprite sprite)
            {
                if (BgSprite == null)
                {
                    BgSprite = sprite;
                }
            });
            //if (BgSprite == null)
            //{
            //    BgSprite = ResManager.inst.LoadSprite("Item Icon/1");
            //}
            GameObject icon = UIControls.createUIImage(background_icon, BgSprite);
            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
            icon.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);

            // 创建文字
            //Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#455A64FF"));
            //GameObject uiText = UIControls.createUIText(background, txtBgSprite, text);
            //uiText.GetComponent<Text>().fontSize = 20;
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(background, txtBgSprite, ColorUtility.ToHtmlStringRGBA(Color.white));
            uiText.GetComponent<Text>().text = text;
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(0, 5, 0);

            // 创建按钮
            string backgroundColor_btn = "#8C9EFFFF";
            GameObject button = UIControls.createUIButton(background, backgroundColor_btn, "购买", action, new Vector3());
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 20);
            button.GetComponent<RectTransform>().localPosition = new Vector3(-50, -10, 0);

            GameObject uiText2 = UIControls.createUIText(background, txtBgSprite, ColorUtility.ToHtmlStringRGBA(Color.white));
            uiText2.GetComponent<Text>().text = money.ToString();
            uiText2.GetComponent<RectTransform>().localPosition = new Vector3(70, -17, 0);
            elementX += 200;

            //return button;
            return background;
        }
        public static GameObject AddToggle(string Text, int width, GameObject panel, UnityAction<bool> action)
        {
            // 计算x轴偏移
            elementX += width / 2 - 30;

            Sprite toggleBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture(ColorUtility.ToHtmlStringRGBA(Color.white)));
            Sprite toggleSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#18FFFFFF"));
            GameObject uiToggle = UIControls.createUIToggle(panel, toggleBgSprite, toggleSprite);
            uiToggle.GetComponentInChildren<Text>().color = Color.white;
            uiToggle.GetComponentInChildren<Toggle>().isOn = false;
            uiToggle.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);

            uiToggle.GetComponentInChildren<Text>().text = Text;
            uiToggle.GetComponentInChildren<Toggle>().onValueChanged.AddListener(action);


            elementX += width / 2 + 10;

            return uiToggle;
        }
        private static void hr()
        {
            elementX = initialX;
            elementY -= 60;
        }

        #endregion

        public static string GetQuailityColor(QualityType quality)
        {
            if (quality == QualityType.Green)
                return ColorUtility.ToHtmlStringRGBA(Color.green);
            else if (quality == QualityType.Blue)
                return ColorUtility.ToHtmlStringRGBA(Color.blue);
            else if (quality == QualityType.Purple)
                return ColorUtility.ToHtmlStringRGBA(new Color(143f / 255f, 7f / 255f, 131f / 255f));
            else if (quality == QualityType.Orange)
                return ColorUtility.ToHtmlStringRGBA(new Color(1f, 165f / 255f, 0f));
            else if (quality == QualityType.Red)
                return ColorUtility.ToHtmlStringRGBA(Color.red);
            else
                return ColorUtility.ToHtmlStringRGBA(Color.white);
        }
        public static void BuyItem(int itemId, int buyNum = 1)
        {
            Config_Item itemInfo = Config_Item.GetItemInfo(itemId);
            if (Singleton<GameManager>.Instance.IsBuyItemExist(itemInfo))
            {
                string text = Singleton<LanguageManager>.Instance.GetStr("tips_buyFail");
                Singleton<EventManager>.Instance.Send<string>("ShowTip", Singleton<LanguageManager>.Instance.GetStr("tips_buyFail"));
                return;
            }
            int buyprice = GetPrice(itemInfo);
            Dictionary<int, int> newdic = new Dictionary<int, int>
            {
                { GameConst.MoneyItemId, buyprice }
            };

            bool flag = GameUtil.IsAllSuccess(newdic, buyNum);
            if (flag)
            {
                Singleton<BagManager>.Instance.RemoveItem(GameConst.MoneyItemId, buyprice * buyNum, true);
                Singleton<GameManager>.Instance.AddItem(itemInfo.id, buyNum, false);
            }
            else
            {
                Singleton<EventManager>.Instance.Send<string>("ShowTip", Singleton<LanguageManager>.Instance.GetStr("tips_buyFail2"));
            }
            
        }

        #region[获取数据相关函数]
        private static List<Config_Item> GetItemData()
        {
            var ItemData = ClearNoneItem(Config_Item.data.Values.ToList<Config_Item>());

            if (searchText != "")
            {
                ItemData = FilterItemData(ItemData);
            }
            if (RemoveBugItem)
            {
                ItemData = RemoveItemData(ItemData);
            }
            // 对 DataList 进行分页
            List<Config_Item> list = new List<Config_Item>();
            int start = (page - 1) * conunt;
            int end = start + conunt;
            for (int i = start; i < end; i++)
            {
                if (i < ItemData.Count)
                {
                    list.Add(ItemData[i]);
                }
            }
            maxPage = ItemData.Count / conunt + 1;

            return list;
        }

        // 搜索过滤
        private static List<Config_Item> FilterItemData(List<Config_Item> dataList)
        {
            if (searchText == "")
            {
                return dataList;
            }
            List<Config_Item> list = new List<Config_Item>();

            foreach (var item in dataList)
            {
                if (item.itemName.Contains(searchText.Replace(" ","")))
                {
                    list.Add(item);
                }
            }

            return list;
        }
        private static List<Config_Item> RemoveItemData(List<Config_Item> dataList)
        {
            List<Config_Item> list = new List<Config_Item>();

            foreach (var item in dataList)
            {
                if(item.itemType == ItemType.Skill || item.itemType == ItemType.Equipment)
                {
                    if (!Singleton<BagManager>.Instance.IsExist(item.id))
                    {
                        list.Add(item);
                    }
                }
                else
                {
                    list.Add(item);
                }
            }

            return list;
        }
        private static List<Config_Item> ClearNoneItem(List<Config_Item> dataList)
        {
            List<Config_Item> list = new List<Config_Item>();

            foreach (var item in dataList)
            {
                if (item.itemName != null && item.itemName != "" && item.itemName != string.Empty)
                {
                    if(item.itemType != ItemType.Special && item.id != GameConst.MoneyItemId)
                        list.Add(item);
                }
            }

            return list;
        }
        public static int BuyPrice(QualityType quality)
        {
            if (quality == QualityType.Green)
                return 100;
            else if (quality == QualityType.Blue)
                return 500;
            else if (quality == QualityType.Purple)
                return 5000;
            else if (quality == QualityType.Orange)
                return 20000;
            else if (quality == QualityType.Red)
                return 100000;
            else
                return 0;
        }
        public static int GetPrice(Config_Item item)
        {
            int buyprice = 0;
            if (item.itemType == ItemType.Material)
                return BuyPrice(item.quality);
            foreach (KeyValuePair<int, int> keyValuePair in item.needMaterialsDic)
            {
                int key = keyValuePair.Key;
                int value = keyValuePair.Value;
                if (key != GameConst.MoneyItemId)
                {
                    int num = BuyPrice(Config_Item.GetItemInfo(key).quality) * value;
                    buyprice += num;
                }
                else
                {
                    buyprice += value;
                }
            }
            if(buyprice == 0)
            {
                return item.sellPrice * 5;
            }
            else if (!ShopItemsIsExist(item))
            {
                return buyprice * 5;
            }
            return buyprice;
        }
        private static void GetShopItem()
        {
            foreach (Config_Shop shop in Config_Shop.data.Values)
            {
                foreach (int item in shop.defaultItem)
                {
                    if (!ShopItems.Contains(item))
                        ShopItems.Add(item);
                }
            }
        }
        private static int GetLineCount(List<string> strings)
        {
            int count = 0;
            foreach(var line in strings)
            {
                if (!line.StartsWith("#") && line != "green" && line != "red" && line.Length > 1)
                {
                    count++;
                }
            }
            return count;
        }
        private static ArrayList GetSeparateSubString(string txtString, int charNumber)
        {
            ArrayList arrlist = new ArrayList();
            string tempStr = txtString;
            for (int i = 0; i < tempStr.Length; i += charNumber)
            {
                if ((tempStr.Length - i) > charNumber)//如果是，就截取
                {
                    arrlist.Add(tempStr.Substring(i, charNumber));
                }
                else
                {
                    arrlist.Add(tempStr.Substring(i));//如果不是，就截取最后剩下的那部分
                }
            }
            return arrlist;
        }
        public static bool ShopItemsIsExist(Config_Item item)
        {
            if(ShopItems.Count == 0)
            {
                GetShopItem();
            }
            if(ShopItems.Contains(item.id))
                return true;
            else
                return false;
        }
        #endregion
    }

}
