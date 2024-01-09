using JTW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityGameUI;
using static JTW.CombatAction;
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
        private static GameObject m_tooltipcompanionSetInLocation;
        private static bool IsAllCard = false;
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
            m_tooltipcompanionSetInLocation = Resources.Load<GameObject>("UI/TooltipText");
            //创建搜索框
            SearchBar(Panel);
            elementX += 280;
            //elementY += 30;
            AddToggle("是否添加其他职业卡牌", 240, Panel, (bool state) =>
            {
                IsAllCard = state;
                if (state)
                {
                    container();
                }
            });

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
        //搜索框
        private void SearchBar(GameObject panel)
        {
            elementY += 10;
            elementX = -MainWindow.width / 2 + 120;
            //label
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(panel, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<Text>().text = "搜索";
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);
            uiText.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 30);
            uiText.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

            //坐标偏移
            elementX += 60;

            //输入框
            int w = 260;
            Sprite inputFieldSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject uiInputField = UIControls.createUIInputField(panel, inputFieldSprite, "#FFFFFFFF");
            uiInputField.GetComponent<InputField>().text = searchText;
            uiInputField.GetComponent<RectTransform>().localPosition = new Vector3(elementX + 100, elementY, 0);
            uiInputField.GetComponent<RectTransform>().sizeDelta = new Vector2(w, 30);


            //文本框失去焦点时触发方法
            uiInputField.GetComponent<InputField>().onEndEdit.AddListener((string text) =>
            {
                //Debug.Log(text);
                page = 1;
                searchText = text;
                if (TryGetData())
                    container();
                ItemWindow.uiText.GetComponent<Text>().text = uiText_text;
                //Destroy(ItemPanel);
            });
        }
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
            foreach (Card item in GetItemData())
            {
                var btn = CreateItemButton("获得", item, ItemPanel, () =>
                {
                    SpawnEquipmentInputDialog("获得", GetItemDescription(item), () =>
                    {
                        SpawnItem(item);
                    });
                });
                ItemButtons.Add(btn);
                num++;
                if (num % 3 == 0)
                {
                    hr();
                }
            }
        }
        public static void Popup(TooltipTextScript anchorObject, GameObject ui, Vector2 offset)
        {
            GameObject sceneDialogCanvas = ui;
            if (sceneDialogCanvas == null)
            {
                return;
            }

            RectTransform component = ui.GetComponent<RectTransform>();
            Vector3 vector;
            if (component)
            {
                Vector3 position = default(Vector3);
                position.x = (anchorObject.CustomAnchorPoint.x - component.pivot.x) * component.sizeDelta.x;
                position.y = (anchorObject.CustomAnchorPoint.y - component.pivot.y) * component.sizeDelta.y;
                position.z = 0f;
                vector = sceneDialogCanvas.transform.InverseTransformPoint(ui.transform.TransformPoint(position));
            }
            else
            {
                vector = sceneDialogCanvas.transform.InverseTransformPoint(ui.transform.position);
            }
            RectTransform component2 = anchorObject.GetComponent<RectTransform>();
            component2.anchorMin = new Vector2(0.5f, 0.5f);
            component2.anchorMax = new Vector2(0.5f, 0.5f);
            Vector2 vector2 = offset;
            
            component2.SetParent(sceneDialogCanvas.transform, false);
            component2.anchoredPosition = new Vector2(vector.x, vector.y) + vector2;

        }
        public static void SpawnEquipmentInputDialog(string ButtonText, string Description, UnityAction onFinish)
        {
            Debug.Log($"ZG:{Description}");
            //创建画布
            GameObject canvas = UIControls.createUICanvas();
            Object.DontDestroyOnLoad(canvas);
            //设置置顶显示
            canvas.GetComponent<Canvas>().overrideSorting = true;
            canvas.GetComponent<Canvas>().sortingOrder = 100;
            //分割物品介绍，设置界面大小
            Debug.Log(Description);
            List<string> outlines = Description.ParseDescription(16);
            int size = outlines.Count * 20 + 20 + 40;

            // 创建面板
            GameObject uiPanel = UIControls.createUIPanel(canvas, size.ToString(), "300", null);
            uiPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#37474FFF");
            //创建物品介绍
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));

            GameObject m_tooltip = Object.Instantiate<GameObject>(m_tooltipcompanionSetInLocation);

            TooltipTextScript component = m_tooltip.GetComponent<TooltipTextScript>();

            component.SetText(Description, new TooltipTextScript.PopupTextLayoutInfo());
            component.SetFontSize(18);
            //component.Popup(uiPanel, TooltipTextScript.Direction.CUSTOM, new Vector2(-50, 0), 0f, true);

            RectTransform rectTransform2 = m_tooltip.GetComponent<RectTransform>();

            Popup(component, uiPanel, new Vector2(-50, 0));
            
            component.ShowTooltip(true, 0f);





            //for (int i = 0; i < outlines.Count; i++)
            //{
            //    if (outlines[i].StartsWith("#"))
            //    {
            //        DefaultColor = outlines[i];
            //        continue;
            //    }


            //    GameObject uiText = UIControls.createUIText(uiPanel, txtBgSprite, DefaultColor);
            //    uiText.GetComponent<Text>().text = outlines[i];
            //    uiText.GetComponent<RectTransform>().localPosition = new Vector2(0, size / 2 - 20 - 20 * i);
            //    uiText.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 20);
            //    uiText.GetComponent<Text>().fontSize = 14;
            //}
            //创建确定按钮
            GameObject uiButton = UIControls.createUIButton(uiPanel, "#8C9EFFFF", ButtonText, () =>
            {
                onFinish();
                Object.Destroy(canvas);
                Object.Destroy(m_tooltip);
            }, new Vector3(100, -size / 2 + 30, 0));

            //创建关闭按钮
            GameObject closeButton = UIControls.createUIButton(uiPanel, "#B71C1CFF", "X", () =>
            {
                Object.Destroy(canvas);
                Object.Destroy(m_tooltip);
            }, new Vector3(350 / 2 - 10, size / 2 - 10, 0));
            //设置closeButton宽高
            closeButton.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            //字体颜色为白色
            closeButton.GetComponentInChildren<Text>().color = UIControls.HTMLString2Color("#FFFFFFFF");
        }
        private static GameObject CreateItemButton(string ButtonText, Card Item, GameObject panel, UnityAction action)
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

            var tip = background.AddComponent<TooltipGUI>();
            tip.Init(GetItemDescription(Item));

            GameObject background_icon = UIControls.createUIPanel(background, buttonHeight.ToString(), "50", null);
            background_icon.GetComponent<Image>().color = UIControls.HTMLString2Color(qualityColor);
            background_icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(70, 0);

            GameObject icon = UIControls.createUIImage(background_icon, GetItemIcon(Item));
            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
            icon.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);

            //创建文字
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(background, txtBgSprite, ColorUtility.ToHtmlStringRGBA(Color.white));
            uiText.GetComponent<Text>().text = GetItemName(Item);
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(0, 5, 0);

            //创建按钮
            string backgroundColor_btn = "#8C9EFFFF";
            GameObject button = UIControls.createUIButton(background, backgroundColor_btn, ButtonText, action, new Vector3());
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 20);
            button.GetComponent<RectTransform>().localPosition = new Vector3(-50, -10, 0);

            elementX += 200;

            return background;
        }
        public static GameObject AddToggle(string Text, int width, GameObject panel, UnityAction<bool> action)
        {
            //计算x轴偏移
            elementX += width / 2 - 30;

            Sprite toggleBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture(ColorUtility.ToHtmlStringRGBA(Color.white)));
            Sprite toggleSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#18FFFFFF"));
            GameObject uiToggle = UIControls.createUIToggle(panel, toggleBgSprite, toggleSprite);
            uiToggle.GetComponentInChildren<Text>().color = Color.white;
            uiToggle.GetComponentInChildren<Toggle>().isOn = false;
            uiToggle.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);

            uiToggle.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 20);

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

        public static void SpawnItem(Card item)
        {
            List<Card> list = new List<Card>();
            list.Add(item);
            Debug.Log(item.GetType());
            Game.Get().GetPlayer().AddCardsToDeck(list, null);
        }



        #region[获取数据相关函数]
        private static bool TryGetData()
        {
            if (Game.Get().GetPlayer() == null)
            {
                return false;
            }
            return true;
        }
        private static List<Card> GetItemData()
        {

            List<Card> ItemData = new List<Card>();

            if (IsAllCard)
            {
                IEnumerable<Type> enumerable = from t in Assembly.GetAssembly(typeof(Card)).GetTypes()
                                               where t.IsSubclassOf(typeof(Card))
                                               select t;
                new List<Card>();
                foreach (Type type in enumerable)
                {
                    if (!type.IsAbstract && !(type.GetConstructor(Type.EmptyTypes) == null))
                    {
                        Card card = (Card)Activator.CreateInstance(type);
                        if (card.Rarity != Rarity.NATIVE)
                        {
                            ItemData.Add(card);
                        }
                    }
                }
                //ItemData.AddRange(new CardSetHolyMonk().Cards);
                //ItemData.AddRange(new CardSetTheSwineKing().Cards);
                //ItemData.AddRange(new CardSetNeutral().Cards);
                //ItemData.AddRange(new CardSetSpecial().Cards);
                //ItemData.AddRange(new CardSetWhiteDragon().Cards);
                //ItemData.AddRange(new CardSetWukong().Cards);
            }
            else
            {
                Dictionary<string, Card> cards = Game.Get().GetPlayer().CardPool.GetCards();
                foreach (var card in cards)
                {
                    ItemData.Add(card.Value);
                }
                ItemData.AddRange(new CardSetNeutral().Cards);
            }


            Debug.Log($"ZG:全物品数量:{ItemData.Count}");
            if (searchText != "")
            {
                ItemData = FilterItemData(ItemData);
            }
            //Curse诅咒 Magazine
            //对 DataList 进行分页
            List<Card> list = new List<Card>();
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
        private static List<Card> FilterItemData(List<Card> dataList)
        {
            if (searchText == "")
            {
                return dataList;
            }
            List<Card> list = new List<Card>();

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
        private static string GetItemName(Card item)
        {
            return item.GetDisplayName();
        }
        private static string GetItemDescription(Card item)
        {
            return item.GetDescription();
        }
        private static Sprite GetItemIcon(Card item)
        {
            return CardAssets.GetImageSprite(item);
        }
        #endregion
    }

}
