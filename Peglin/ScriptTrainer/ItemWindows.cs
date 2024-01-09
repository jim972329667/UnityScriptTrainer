using BepInEx;
using Currency;
using HarmonyLib;
using I2.Loc;
using PeglinUI;
using PeglinUI.OrbDisplay;
using Relics;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
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
        public static ItemWindow Instance = null;

        #region[数据分页相关]
        private static GameObject ItemPanel;
        private static List<GameObject> ItemButtons = new List<GameObject>();
        private static int page = 1;
        private static int maxPage = 1;
        private static int conunt = 15;
        private static string searchText = "";
        private static GameObject uiText;
        private static GameObject m_tooltipcompanionSetInLocation;
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
            Instance = this;
            Initialize();
        }
        public void Initialize()
        {
            m_tooltipcompanionSetInLocation = Resources.Load<GameObject>("UI/TooltipText");
            //创建搜索框
            SearchBar(Panel);
            elementX += 280;
            //elementY += 30;

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

            //PageBar(Panel);
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

            Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            Sprite scrollbarSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));
            Sprite dropDownSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            Sprite checkmarkSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));
            Sprite customMaskSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#E65100FF"));
            var ItemPanel = UIControls.createUIScrollView(Panel, bgSprite2, customMaskSprite, scrollbarSprite, new Vector2(620, 300));
            //ItemPanel = UIControls.createUIPanel(Panel, "300", "600");
            //ItemPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            ItemPanel.scrollView.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, 0);
            ItemPanel.scrollView.GetComponent<ScrollRect>().scrollSensitivity = 50;
            var gridgroup = ItemPanel.content.AddComponent<VerticalLayoutGroup>();
            //gridgroup.cellSize = new Vector2(190, 50);
            //gridgroup.spacing = new Vector2(10, 5);
            gridgroup.spacing = 5;


            var gridgroup2 = ItemPanel.content.AddComponent<ContentSizeFitter>();
            gridgroup2.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            gridgroup2.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            int num = 0;
            GameObject line = new GameObject("ItemLine" + 1);

            foreach (Relic item in GetItemData())
            {
                if (num % 3 == 0)
                {
                    //if(num != 0)
                    //    hr();
                    line = new GameObject("ItemLine" + num % 3 + 1);
                    var linegroup = line.AddComponent<GridLayoutGroup>();
                    linegroup.cellSize = new Vector2(190, 50);
                    linegroup.spacing = new Vector2(10, 5);
                    line.transform.SetParent(ItemPanel.content.transform, false);
                    ItemButtons.Add(line);
                }

                var btn = CreateItemButton("获得", item, line, () =>
                {
                    SpawnEquipmentInputDialog("获得", GetItemDescription(item), () =>
                    {
                        SpawnItem(item, 1);
                        
                    });
                    
                });

                //ItemButtons.Add(btn);
                num++;

                
            }
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
            int size = 5 * 20 + 20 + 40;

            // 创建面板
            GameObject uiPanel = UIControls.createUIPanel(canvas, size.ToString(), "300", null);
            uiPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#37474FFF");
            //创建物品介绍
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            string DefaultColor = "#FFFFFFFF";

            GameObject uiText = UIControls.createUITextMeshProUGUI(uiPanel, txtBgSprite, DefaultColor);
            uiText.GetComponent<RectTransform>().sizeDelta = new Vector2(280, size);
            uiText.GetComponent<TextMeshProUGUI>().fontSizeMin = 14;
            uiText.GetComponent<TextMeshProUGUI>().text = Description;
            uiText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f, 0.5f);
            
            size = (int)(uiText.GetComponent<TextMeshProUGUI>().preferredHeight + 20 + 40);
            uiText.GetComponent<RectTransform>().localPosition = new Vector2(0, size / 2 - 90);
            uiPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(300, size);

            if (File.Exists(Paths.PluginPath + "/7000.unitypackage"))
            {
                var nfont = Resources.Load<TMP_FontAsset>(Paths.PluginPath + "/7000.unitypackage");
                if(nfont != null)
                    uiText.GetComponent<TextMeshProUGUI>().font = nfont;
            }

            Debug.Log($"ZG :{uiText.GetComponent<TextMeshProUGUI>().preferredHeight}");
            //uiText.GetComponent<TextMeshProUGUI>().enableWordWrapping = true;

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
            }, new Vector3(100, -size / 2 + 30, 0));

            //创建关闭按钮
            GameObject closeButton = UIControls.createUIButton(uiPanel, "#B71C1CFF", "X", () =>
            {
                Object.Destroy(canvas);
            }, new Vector3(350 / 2 - 10, size / 2 - 10, 0));
            //设置closeButton宽高
            closeButton.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            //字体颜色为白色
            closeButton.GetComponentInChildren<Text>().color = UIControls.HTMLString2Color("#FFFFFFFF");
        }
        private static GameObject CreateItemButton(string ButtonText, Relic item, GameObject panel, UnityAction action)
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

            //创建文字
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(background, txtBgSprite, ColorUtility.ToHtmlStringRGBA(Color.white));
            uiText.GetComponent<Text>().text = GetItemName(item);
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(0, 5, 0);

            var tip = background.AddComponent<TooltipGUI>();
            tip.Initialize(GetItemDescription(item));

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

        public static void SpawnItem(Relic item, int amount)
        {
            ScriptPatch.ZGRelicManager.AddRelic(item);
        }



        #region[获取数据相关函数]
        private static bool TryGetData()
        {
            if (ScriptPatch.ZGRelicManager == null)
            {
                return false;
            }
            return true;
        }
        private static List<Relic> GetItemData()
        {
            //var x = Traverse.Create(RelicManager.CreateInstance<RelicManager>()).Field("_globalRelics").GetValue<RelicSet>();
            var x = Traverse.Create(ScriptPatch.ZGRelicManager).Field("_globalRelics").GetValue<RelicSet>();
            if (x == null)
            {
                return null;
            }
            List<Relic> ItemData = x.relics;

            Debug.Log($"ZG:全物品数量:{ItemData.Count}");
            if (searchText != "")
            {
                ItemData = FilterItemData(ItemData);
            }
            //Curse诅咒 Magazine
            //对 DataList 进行分页
            List<Relic> list = new List<Relic>();
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
            return ItemData;
            //return list;
        }
        //搜索过滤
        private static List<Relic> FilterItemData(List<Relic> dataList)
        {
            if (searchText == "")
            {
                return dataList;
            }
            List<Relic> list = new List<Relic>();

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
        private static string GetItemName(Relic item)
        {
            return LocalizationManager.GetTranslation(item.nameKey, true, 0, true, false, null, null, true);
        }
        private static string GetItemDescription(Relic item)
        {
            Tooltip x = Traverse.Create(TooltipManager.Instance).Field("_tooltipOrb").GetValue<Tooltip>() ;
            Localize localize = x.descriptionText.GetComponent<Localize>();
            localize.Term = item.descKey;

            return Localize.MainTranslation;
        }
        private static Sprite GetItemIcon(Relic item)
        {
            return item.sprite;
        }
        #endregion
    }

}
