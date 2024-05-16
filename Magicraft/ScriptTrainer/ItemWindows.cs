using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityGameUI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using static UnityGameUI.UIControls;
using Image = UnityEngine.UI.Image;
using Object = UnityEngine.Object;
using Resources = UnityEngine.Resources;

namespace ScriptTrainer
{
    public class ZGItem
    {
        public string Type {  get; set; }
        public int id { get; set; }
        public int Count {  get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string icon { get; set; }
        public string maxMP { get; set; }
        public string shootInterval { get; set; }
        public string mpRecovery { get; set; }
        public string coolDown { get; set; }
        public int unitType {  get; set; }

        public object GetConfig()
        {
            if (Type.IsNullOrWhiteSpace() || ScriptTrainer.MainAssembly == null)
                return null;
            var method = GetZGMethod("GetConfig", "GetConfigCopy");
            if (method == null) return "";
            return method.Invoke(null, new object[] { id });
        }
        public string GetName()
        {
            var obj = GetConfig();
            if (obj == null) return "";
            var method = GetZGMethod("GetName");
            
            if (method == null) return "";
            ParameterInfo[] parameters = method.GetParameters();
            object[] defaultArgs = parameters.Select(p => p.HasDefaultValue ? p.DefaultValue : null).ToArray();
            return (string)method.Invoke(obj, defaultArgs) ?? "";
        }
        public object ZGInvoke(string[] methods, params object[] args)
        {
            var obj = GetConfig();
            if (obj == null) return null;
            var method = GetZGMethod(methods);
            if (method == null) return null;
            if(args == null)
            {
                ParameterInfo[] parameters = method.GetParameters();
                object[] defaultArgs = parameters.Select(p => p.HasDefaultValue ? p.DefaultValue : null).ToArray();
                return (string)method.Invoke(obj, defaultArgs) ?? "";
            }
            return method.Invoke(obj, args);
        }
        public MethodInfo GetZGMethod(params string[] targets)
        {
            var type = ScriptTrainer.MainAssembly.GetType(Type);
            if (type == null) return null;
            foreach (var me in type.GetMethods())
            {
                if (targets.Contains(me.Name))
                {
                    return me;
                }
            }
            return null;
        }
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

        public static Dictionary<int,List<ZGItem>> ItemDic = new Dictionary<int,List<ZGItem>>();
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
                try
                {
                    container();
                    initialized = true;
                }
                catch (Exception e)
                {
                    ScriptTrainer.Instance.Log(e, LogType.Error);
                }
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

            ItemPanel.scrollView.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, -30);
            ItemPanel.scrollView.GetComponent<ScrollRect>().scrollSensitivity = 40;
            var gridgroup = ItemPanel.content.AddComponent<VerticalLayoutGroup>();
            gridgroup.spacing = 5;


            var gridgroup2 = ItemPanel.content.AddComponent<ContentSizeFitter>();
            gridgroup2.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            gridgroup2.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            int num = 0;
            GameObject line = new GameObject("ItemLine" + 1);
            ScriptTrainer.Instance.Log("开始获取GetItemData");

            try
            {
                List<ZGItem> tmps = GetItemData(index);
            }
            catch(Exception e)
            {
                ScriptTrainer.Instance.Log(e, LogType.Error);
            }

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
            if (item.Type == "RelicConfig")
            {
                for (int i = 0; i < count; i++)
                {
                    ScriptTrainer.WriteGameCmd($"relic {item.id}");
                } 
            }
            else if (item.Type == "PotionConfig")
            {
                for (int i = 0; i < count; i++)
                {
                    ScriptTrainer.WriteGameCmd($"potion {item.id}");
                }
            }
            else if (item.Type == "SpellConfig")
            {
                ScriptTrainer.WriteGameCmd($"spell {item.id} {count}");
            }
            else if (item.Type == "WandConfig")
            {
                for (int i = 0; i < count; i++)
                {
                    ScriptTrainer.WriteGameCmd($"wand {item.id}");
                }
            }
            else if (item.Type == "CurseConfig")
            {
                for (int i = 0; i < count; i++)
                {
                    ScriptTrainer.WriteGameCmd($"curse {item.id}");
                }
            }
            else if (item.Type == "UnitConfig")
            {
                for (int i = 0; i < count; i++)
                {
                    ScriptTrainer.WriteGameCmd($"unit {item.id}");
                }
            }
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
            if (ItemDic.Count == 0 || ScriptTrainer.PlayerManagerInvoke(new string[] { "get_BattleDat" , "get_BaData" }, null) == null)
            {
                return false;
            }
            return true;
        }
        private List<ZGItem> GetItemData(int index = 0)
        {
            ScriptTrainer.Instance.Log("开始获取Items");
            ItemDic.TryGetValue(index , out List<ZGItem> ItemData);
            
            if(index == 5)
            {
                List<ZGItem> NewItemData = new List<ZGItem>();
                foreach(var item in ItemData) 
                { 
                    if(item.unitType > 2 && item.unitType < 7)
                    {
                        NewItemData.Add(item);
                    }
                }
                ItemData = NewItemData;
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
                return item.GetName();
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
                string ra = (string)item.ZGInvoke(new string[] { "GetStrRarity" }, null) ?? string.Empty;
                string info = (string)item.ZGInvoke(new string[] { "GetInfo" }, null) ?? string.Empty;

                switch (item.Type)
                {
                    case "PotionConfig":
                        return info;
                    case "RelicConfig":
                    case "CurseConfig":
                    case "SpellConfig":
                        return ra + "\n" + info;
                    case "UnitConfig":
                        return item.GetName();
                    case "WandConfig":
                        return $"魔法值：{item.maxMP}\n射击间隔：{item.shootInterval}\n回魔：{item.mpRecovery}/s\n冷却：{item.coolDown}\n{info}";
                    default:
                        return null;
                }
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
                switch (item.Type)
                {
                    case "RelicConfig":
                        return Resources.Load<Sprite>($"Textures/RelicIcons/{item.id}");
                    case "PotionConfig":
                        return Resources.Load<Sprite>($"Textures/PotionIcons/{item.id}");
                    case "CurseConfig":
                        return Resources.Load<Sprite>($"Textures/CurseIcons/{item.id}");
                    case "UnitConfig":
                        return Resources.Load<Sprite>($"Textures/UnitIcons/{item.id}");
                    case "SpellConfig":
                        return Resources.Load<Sprite>($"Textures/SpellIcons/{item.icon}");
                    case "WandConfig":
                        return Resources.Load<Sprite>($"Textures/WandIcons/{item.icon}");
                    default:
                        return null;
                }
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
