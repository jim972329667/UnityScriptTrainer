using BepInEx;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityGameUI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.Utility;
using static UnityGameUI.UIControls;
using Image = UnityEngine.UI.Image;
using Object = UnityEngine.Object;
using Resources = UnityEngine.Resources;

namespace ScriptTrainer
{
    public class ZGItem
    {
        public string Type {  get; set; }
        public int ID { get; set; }
        public int Count {  get; set; }
    }
    internal class ItemWindow : MonoBehaviour
    {
        private static GameObject Panel;
        private static int initialX;
        private static int initialY;
        private static int elementX;
        private static int elementY;
        public static ItemWindow Instance = null;
        public static bool initialized = false;

        #region[数据分页相关]
        private static List<GameObject> ItemButtons = new List<GameObject>();
        public static Dropdown ItemTypeDropdown;
        public static string SearchText = "请输入。。。";
        public static InputFieldRef SearchInput;
        public static MyScrollView ItemPanel;
        //public static object SelectWand = null;

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
            SearchBar(Panel);
            elementX += 280;
            //elementY += 30;

            hr();

            //创建物品列表
            //InitializeItem();
        }
        public void InitializeItem()
        {
            if (TryGetData())
            {
                container();
                initialized = true;
            }
        }
        #region[创建详细]
        //搜索框
        public void SearchInput_OnEndEdit()
        {
            try
            {
                if (TryGetData())
                {
                    if (ItemTypeDropdown != null)
                        container(ItemTypeDropdown.value);
                    else
                        container();
                }
            }
            catch (Exception ex)
            {
                ScriptTrainer.Instance.Log(ex, LogType.Error);
            }
            
        }
        public void SearchBar(GameObject panel)
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

            SearchInput = UIFactory.CreateInputField(panel, "ItemSearchInput", SearchText);
            UIFactory.SetLayoutElement(SearchInput.GameObject, new int?(125), new int?(25), new int?(9999), null, null, null, null);
            SearchInput.GameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(w, 30);
            SearchInput.GameObject.GetComponent<RectTransform>().localPosition = new Vector3(elementX + 100, elementY, 0);
            //SearchInput.Component.onValueChanged.AddListener(delegate { SearchInput_OnEndEdit(); });

            GameObject gameObject2 = UIFactory.CreateDropdown(panel, "ItemTypeDropdown", out ItemTypeDropdown, "选择物品类型", 14, new Action<int>(ItemTypeDropdownValueChange), null);
            ItemTypeDropdown.options.Add(new Dropdown.OptionData("遗物"));
            ItemTypeDropdown.options.Add(new Dropdown.OptionData("药剂"));
            ItemTypeDropdown.options.Add(new Dropdown.OptionData("法术"));
            ItemTypeDropdown.options.Add(new Dropdown.OptionData("法杖"));
            ItemTypeDropdown.options.Add(new Dropdown.OptionData("诅咒"));
            ItemTypeDropdown.options.Add(new Dropdown.OptionData("单位"));
            //ItemTypeDropdown.options.Add(new Dropdown.OptionData("房间"));
            gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 30);
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector3(140, elementY, 0);
        }
        private void ItemTypeDropdownValueChange(int value)
        {
            if (TryGetData())
                container(value);
            ScriptTrainer.Instance.Log($"ItemTypeDropdown : {value}");
        }
        //分页
        public void container(int index = 0)
        {
            elementX = -200;
            elementY = 125;

            //先清空旧的 ItemPanel
            foreach (var item in ItemButtons)
            {
                UnityEngine.Object.Destroy(item);
            }
            ItemButtons.Clear();
            ItemPanel?.Destroy();
            //UpdatePlayer();
            Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            Sprite scrollbarSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));
            Sprite dropDownSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            Sprite checkmarkSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));
            Sprite customMaskSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#E65100FF"));
            ItemPanel = UIControls.createUIScrollView(Panel, bgSprite2, customMaskSprite, scrollbarSprite, new Vector2(620, 350));
            //ItemPanel = UIControls.createUIPanel(Panel, "300", "600");
            //ItemPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            ItemPanel.scrollView.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, -30);
            ItemPanel.scrollView.GetComponent<ScrollRect>().scrollSensitivity = 40;
            var gridgroup = ItemPanel.content.AddComponent<VerticalLayoutGroup>();
            //gridgroup.cellSize = new Vector2(190, 50);
            //gridgroup.spacing = new Vector2(10, 5);
            gridgroup.spacing = 5;


            var gridgroup2 = ItemPanel.content.AddComponent<ContentSizeFitter>();
            gridgroup2.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            gridgroup2.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            int num = 0;
            GameObject line = new GameObject("ItemLine" + 1);

            foreach (ZGItem item in GetItemData(index))
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
                    if (item.Type != "room")
                    {
                        UIWindows.SpawnInputDialog("获得", GetItemDescription(item), "1", (string amount) =>
                        {
                            SpawnItem(item, amount.ConvertToIntDef(1));
                        });
                    }
                    else
                    {
                        SpawnItem(item);
                    }   

                });

                //ItemButtons.Add(btn);
                num++;


            }
        }
        public static void SpawnEquipmentInputDialog(string ButtonText, string Description, UnityAction onFinish)
        {
            //创建画布
            GameObject canvas = UIControls.createUICanvas();
            Object.DontDestroyOnLoad(canvas);
            //设置置顶显示
            canvas.GetComponent<Canvas>().overrideSorting = true;
            canvas.GetComponent<Canvas>().sortingOrder = 100;
            //分割物品介绍，设置界面大小
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
        private GameObject CreateItemButton(string ButtonText, ZGItem item, GameObject panel, UnityAction action)
        {
            //按钮宽 200 高 50
            int buttonWidth = 190;
            int buttonHeight = 50;

            //根据品质设置背景颜色
            //string qualityColor = "#FFFFFFFF";
            //创建一个背景
            GameObject background = UIControls.createUIPanel(panel, buttonHeight.ToString(), buttonWidth.ToString(), null);
            background.GetComponent<Image>().color = UIControls.HTMLString2Color("#455A64FF");
            background.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);


            GameObject background_icon = UIControls.createUIPanel(background, buttonHeight.ToString(), "50", null);
            background_icon.GetComponent<Image>().sprite = GetItemIcon(item);
            background_icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(70, 0);



            //创建文字
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(background, txtBgSprite, ColorUtility.ToHtmlStringRGBA(Color.white));
            uiText.GetComponent<Text>().text = GetItemName(item);
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(0, 5, 0);

            string dec = GetItemDescription(item);
            if(dec != null)
            {
                var tip = background.AddComponent<TooltipGUI>();
                tip.Initialize(dec, ItemPanel.scrollView);
                ItemButtons.Add(tip.canvas);
            }
            
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
            Sprite toggleSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#18FFFF"));
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
        public static void SpawnItem(ZGItem item, int count = 1)
        {
            if (item.Type == "relic")
            {
                for (int i = 0; i < count; i++)
                {
                    ScriptTrainer.WriteGameCmd($"relic {item.ID}");
                } 
            }
            else if (item.Type == "potion")
            {
                for (int i = 0; i < count; i++)
                {
                    ScriptTrainer.WriteGameCmd($"potion {item.ID}");
                }
            }
            else if (item.Type == "spell")
            {
                ScriptTrainer.WriteGameCmd($"spell {item.ID} {count}");
            }
            else if (item.Type == "wand")
            {
                for (int i = 0; i < count; i++)
                {
                    ScriptTrainer.WriteGameCmd($"wand {item.ID}");
                }
            }
            else if (item.Type == "curse")
            {
                for (int i = 0; i < count; i++)
                {
                    ScriptTrainer.WriteGameCmd($"curse {item.ID}");
                }
            }
            else if (item.Type == "unit")
            {
                for (int i = 0; i < count; i++)
                {
                    ScriptTrainer.WriteGameCmd($"unit {item.ID}");
                }
            }
            //else if (item.Type == "room")
            //{
            //    ScriptTrainer.WriteGameCmd($"stage {item.ID}");
            //}
            return;
        }

        public void Update()
        {
            if(SearchInput != null)
            {
                if(SearchInput.Text != SearchText)
                {
                    SearchText = SearchInput.Text;
                    SearchInput_OnEndEdit();
                }
            }
        }
        #region[获取数据相关函数]
        private static bool TryGetData()
        {
            if (WandConfig.dic.Count == 0 || UnitConfig.dic.Count == 0 || SpellConfig.dic.Count == 0 || RelicConfig.dic.Count == 0 || PotionConfig.dic.Count == 0 || CurseConfig.dic.Count == 0 || ScriptTrainer.PlayerManagerInvoke("get_BattleDat", null) == null)
            {
                return false;
            }
            return true;
        }
        private List<ZGItem> GetItemData(int index = 0)
        {
            //SelectWand = ScriptTrainer.PlayerManagerInvoke("get_SelectedWand", null);
            List<ZGItem> ItemData = new List<ZGItem>();
            switch (index)
            {
                default:
                case 0:
                    foreach(var x in RelicConfig.dic)
                    {
                        ItemData.Add(new ZGItem() { Count = 1,ID =x.Key, Type = "relic" });
                    }
                    break;
                case 1:
                    foreach (var x in PotionConfig.dic)
                    {
                        ItemData.Add(new ZGItem() { Count = 1, ID = x.Key, Type = "potion" });
                    }
                    break;
                case 2:
                    foreach (var x in SpellConfig.dic)
                    {
                        ItemData.Add(new ZGItem() { Count = 1, ID = x.Key, Type = "spell" });
                    }
                    break;
                case 3:
                    foreach (var x in WandConfig.dic)
                    {
                        ItemData.Add(new ZGItem() { Count = 1, ID = x.Key, Type = "wand" });
                    }
                    break;
                case 4:
                    foreach (var x in CurseConfig.dic)
                    {
                        ItemData.Add(new ZGItem() { Count = 1, ID = x.Key, Type = "curse" });
                    }
                    break;
                case 5:
                    foreach (var x in UnitConfig.dic)
                    {
                        if((int)x.Value.unitType == 3 || (int)x.Value.unitType == 4 || (int)x.Value.unitType == 5 || (int)x.Value.unitType == 6)
                            ItemData.Add(new ZGItem() { Count = 1, ID = x.Key, Type = "unit" });
                    }
                    break;
                //case 6:
                //    foreach (var x in RoomConfig.dic)
                //    {
                //        ItemData.Add(new ZGItem() { Count = 1, ID = x.Key, Type = "room" });
                //    }
                //    break;
            }

            ScriptTrainer.Instance.Log($"ZG:全物品数量:{ItemData.Count}");

            ItemData = FilterItemData(ItemData);

            ScriptTrainer.Instance.Log($"ZG:过滤后数量:{ItemData.Count}");

            return ItemData;
        }
        //搜索过滤
        private List<ZGItem> FilterItemData(List<ZGItem> dataList)
        {
            if (SearchInput != null && SearchInput.Text != "请输入。。。")
            {
                List<ZGItem> list = new List<ZGItem>();

                foreach (var item in dataList)
                {
                    string text = GetItemName(item);
                    string text2 = GetItemDescription(item);
                    if (text != null && text.Contains(SearchInput.Text.Replace(" ", "")))
                    {
                        list.Add(item);
                    }
                    else if(text2 != null && text2.Contains(SearchInput.Text.Replace(" ", "")))
                    {
                        list.Add(item);
                    }
                }

                return list;
            }
            else
                return dataList;
        }
        private string GetItemName(ZGItem item)
        {
            try
            {
                if (item.Type == "relic")
                {
                    return RelicConfig.GetConfig(item.ID).GetName(true) ?? "";
                }
                else if (item.Type == "potion")
                {
                    return PotionConfig.GetConfig(item.ID).GetName() ?? "";
                }
                else if (item.Type == "spell")
                {
                    var x = (SpellConfig)ScriptTrainer.GetSpellConfig?.Invoke(null, new object[] { item.ID });
                    if (x != null)
                        return x.GetName(true) ?? "";
                }
                else if (item.Type == "wand")
                {
                    return WandConfig.GetConfig(item.ID).GetName() ?? "";
                }
                else if (item.Type == "curse")
                {
                    return CurseConfig.GetConfig(item.ID).GetName() ?? "";
                }
                else if (item.Type == "unit")
                {
                    return UnitConfig.GetConfig(item.ID).GetName() ?? "";
                }
                else if (item.Type == "room")
                {
                    return RoomConfig.GetConfig(item.ID).name ?? "";
                }
                return "";
            }
            catch (Exception ex)
            {
                ScriptTrainer.Instance.Log($"获取物品名字错误：{ex}", LogType.Error);
                return "";
            }
        }
        private string GetItemDescription(ZGItem item)
        {
            try
            {
                if (item.Type == "relic")
                {
                    var tmp = RelicConfig.GetConfig(item.ID);
                    string ra = tmp.GetStrRarity() ?? string.Empty;
                    string info = tmp.GetInfo(true) ?? "";
                    return ra + "\n" + info;
                }
                else if (item.Type == "potion")
                {
                    return PotionConfig.GetConfig(item.ID).GetInfo() ?? "";
                }
                else if (item.Type == "spell")
                {
                    var tmp  = (SpellConfig)ScriptTrainer.GetSpellConfig?.Invoke(null, new object[] { item.ID });

                    string ra = tmp?.GetStrRarity() ?? string.Empty;
                    //MethodInfo methodInfo = tmp.GetType().GetMethod("GetInfo");
                    //var methodInfo2 = SelectWand.GetType().GetMethod("get_WandCfg");
                    //WandConfig wand = (WandConfig)methodInfo2?.Invoke(SelectWand, null);
                    //string info = string.Empty;
                    //if (wand != null)
                    //{
                    //    WandConfig tmp2 = WandConfig.GetConfig(wand.id);
                    //    if (tmp2.normalSlots.Length > 0)
                    //    {
                    //        //info = (string)methodInfo.Invoke(tmp, new object[] { SelectWand, tmp2.normalSlots[0] });
                    //        info = tmp.GetInfo((GMPOEJNJJCF)SelectWand, tmp2.normalSlots[0]);
                    //    }
                    //    else
                    //    {
                    //        info = tmp.GetInfo() ?? "";
                    //    }
                    //}
                    //else
                    //    info = tmp.GetInfo() ?? "";

                    string info = tmp.GetInfo() ?? "";
                    return ra + "\n" + info;
                }
                else if (item.Type == "wand")
                {
                    WandConfig tmp = WandConfig.GetConfig(item.ID);
                    string info = tmp.GetInfo() ?? "";
                    return $"魔法值：{tmp.maxMP}\n射击间隔：{tmp.shootInterval}\n回魔：{tmp.mpRecovery}/s\n冷却：{tmp.coolDown}\n{info}";
                }
                else if (item.Type == "curse")
                {
                    CurseConfig tmp = CurseConfig.GetConfig(item.ID);
                    string ra = tmp.GetStrRarity() ?? string.Empty;
                    string info = tmp.GetInfo() ?? "";
                    return ra + "\n" + info;
                }
                else if (item.Type == "unit")
                {
                    return UnitConfig.GetConfig(item.ID).GetName() ?? "";
                }
                else if (item.Type == "room")
                {
                    return $"阶段:{RoomConfig.GetConfig(item.ID).belongStage}";
                }
                return "";
            }
            catch(Exception ex)
            {
                ScriptTrainer.Instance.Log($"获取物品解释错误：{ex}", LogType.Error);
                return "";
            }
            
        }
        private Sprite GetItemIcon(ZGItem item)
        {
            try
            {
                if (item.Type == "relic")
                {
                    return Resources.Load<Sprite>($"Textures/RelicIcons/{item.ID}");
                }
                else if (item.Type == "potion")
                {
                    return Resources.Load<Sprite>($"Textures/PotionIcons/{item.ID}");
                }
                else if (item.Type == "spell")
                {
                    var x = (SpellConfig)ScriptTrainer.GetSpellConfig?.Invoke(null, new object[] { item.ID });
                    if(x != null)
                        return Resources.Load<Sprite>($"Textures/SpellIcons/{x.icon}");
                }
                else if (item.Type == "wand")
                {
                    return Resources.Load<Sprite>($"Textures/WandIcons/{WandConfig.GetConfig(item.ID).icon}");
                }
                else if (item.Type == "curse")
                {
                    return Resources.Load<Sprite>($"Textures/CurseIcons/{item.ID}");
                }
                else if (item.Type == "unit")
                {
                    return Resources.Load<Sprite>($"Textures/UnitIcons/{item.ID}");
                }

                return null;
            }
         
            catch(Exception ex)
            {
                ScriptTrainer.Instance.Log(ex, LogType.Error);
                return null;
            }
        }
        #endregion
    }

}
