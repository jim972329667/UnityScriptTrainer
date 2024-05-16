using BepInEx;
using GridEditor;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
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
using static Ara.AraTrail;
using static UnityGameUI.UIControls;
using Image = UnityEngine.UI.Image;
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
        private static List<GameObject> ItemButtons = new List<GameObject>();
        private static int page = 1;
        private static int maxPage = 1;
        private static int conunt = 15;
        private static string searchText = "";
        public static Dropdown PlayerDropdown;
        public static Dropdown ItemTypeDropdown;
        public static Dropdown ItemRaritiesDropdown;
        public static MyScrollView ItemPanel;

        public static List<Entity> Players = new List<Entity>();
        public static Configs Configs = new Configs();
        public static Dictionary<string,Thing> TmpThings = new Dictionary<string,Thing>();
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
            SearchBar(Panel);
            elementX += 280;
            //elementY += 30;

            foreach (var item in Configs.Things)
            {
                TmpThings.Add(item.Key, InventoryHelper.CreateThing(item.Key));
            }
            hr();

            //创建物品列表
            if (TryGetData())
            {
                container();
            }
            //else
            //{
            //    //elementX += 200;
            //elementY = 125 - 60 * 5;
            //}
            //创建分页

            //PageBar(Panel);
        }

        #region[创建详细]
        //搜索框
        private void SearchBar(GameObject panel)
        {
            elementY += 10;
            //elementX = -MainWindow.width / 2 + 120;
            ////label
            //Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            //GameObject uiText = UIControls.createUIText(panel, txtBgSprite, "#FFFFFFFF");
            //uiText.GetComponent<Text>().text = "搜索";
            //uiText.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);
            //uiText.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 30);
            //uiText.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

            ////坐标偏移
            //elementX += 60;

            ////输入框
            //int w = 260;
            //Sprite inputFieldSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            //GameObject uiInputField = UIControls.createUIInputField(panel, inputFieldSprite, "#FFFFFFFF");
            //uiInputField.GetComponent<InputField>().text = searchText;
            //uiInputField.GetComponent<RectTransform>().localPosition = new Vector3(elementX + 100, elementY, 0);
            //uiInputField.GetComponent<RectTransform>().sizeDelta = new Vector2(w, 30);

            ////文本框失去焦点时触发方法
            //uiInputField.GetComponent<InputField>().onEndEdit.AddListener((string text) =>
            //{
            //    if (text != null)
            //        searchText = text;
            //    if (TryGetData())
            //    {
            //        if (ItemTypeDropdown != null)
            //            container(ItemTypeDropdown.value);
            //        else
            //            container();
            //    }
            //});
            GameObject gameObject = UIFactory.CreateDropdown(panel, "PlayerDropdown", out PlayerDropdown, "Mouse Inspect", 14, (int count) =>
            {
                ScriptTrainer.Instance.Log($"PlayerDropdown : {count} {PlayerDropdown.value}");
            }, null);
            PlayerDropdown.options.Add(new Dropdown.OptionData("无角色"));
            PlayerDropdown.options.Add(new Dropdown.OptionData("无角色"));
            PlayerDropdown.options.Add(new Dropdown.OptionData("无角色"));
            PlayerDropdown.options.Add(new Dropdown.OptionData("无角色"));
            
            gameObject.transform.SetSiblingIndex(0);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 30);
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3(-220, elementY, 0);

            GameObject gameObject2 = UIFactory.CreateDropdown(panel, "ItemTypeDropdown", out ItemTypeDropdown, "选择物品类型", 14, new Action<int>(ItemTypeDropdownValueChange), null);
            ItemTypeDropdown.options.Add(new Dropdown.OptionData("全部"));
            ItemTypeDropdown.options.Add(new Dropdown.OptionData("装备"));
            ItemTypeDropdown.options.Add(new Dropdown.OptionData("战斗消耗品"));
            ItemTypeDropdown.options.Add(new Dropdown.OptionData("日常消耗品"));
            ItemTypeDropdown.options.Add(new Dropdown.OptionData("事件物品"));
            ItemTypeDropdown.options.Add(new Dropdown.OptionData("属性"));
            ItemTypeDropdown.options.Add(new Dropdown.OptionData("其他物品"));
            gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 30);
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector3(-70, elementY, 0);

            GameObject gameObject3 = UIFactory.CreateDropdown(panel, "ItemRaritiesDropdown", out ItemRaritiesDropdown, "选择物品类型", 14, new Action<int>(ItemRaritiesDropdownValueChange), null);
            ItemRaritiesDropdown.options.Add(new Dropdown.OptionData("普通"));
            ItemRaritiesDropdown.options.Add(new Dropdown.OptionData("罕见"));
            ItemRaritiesDropdown.options.Add(new Dropdown.OptionData("稀有"));
            ItemRaritiesDropdown.options.Add(new Dropdown.OptionData("宝物"));
            ItemRaritiesDropdown.options.Add(new Dropdown.OptionData("其他"));
            gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 30);
            gameObject3.GetComponent<RectTransform>().localPosition = new Vector3(80, elementY, 0);
            gameObject3.SetActive(false);

        }
        public static void UpdatePlayer()
        {
            if (PlayerDropdown != null)
            {
                for(int i = 0;i < 4; i++)
                {
                    if(i < Players.Count)
                    {
                        CharacterComponent name = (CharacterComponent)Players[i].Components["CharacterComponent"];
                        PlayerDropdown.options[i] = new Dropdown.OptionData(name.DisplayName);
                    }
                    else
                    {
                        PlayerDropdown.options[i] = new Dropdown.OptionData("无角色");
                    }
                }

                PlayerDropdown.captionText.text = PlayerDropdown.options[PlayerDropdown.value].text;
            }
        }
        private void ItemTypeDropdownValueChange(int value)
        {
            if(value == 1)
                ItemRaritiesDropdown.gameObject.SetActive(true);
            else
                ItemRaritiesDropdown.gameObject.SetActive(false);
            if (TryGetData())
                container(value);
            ScriptTrainer.Instance.Log($"ItemTypeDropdown : {value}");
        }
        private void ItemRaritiesDropdownValueChange(int value)
        {
            if (TryGetData())
                container(ItemTypeDropdown.value);
            ScriptTrainer.Instance.Log($"ItemRaritiesDropdown : {value}");
        }
        //分页
        public static void container(int index = 0)
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

            foreach (String item in GetItemData(index))
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
                    UIWindows.SpawnInputDialog("获得", GetItemDescription(item), "1" , (string amount) =>
                    {
                        SpawnItem(item, amount.ConvertToIntDef(1));
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
        private static GameObject CreateItemButton(string ButtonText, String item, GameObject panel, UnityAction action)
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
            try
            {
                var x = ItemCardViewHelper.GetItemCardStyle(item, false);
                background_icon.GetComponent<Image>().color = x.backColor;
                
                //Sprite sprite = GetItemIcon(item);
                if (x.icon != null)
                {
                    GameObject icon = UIControls.createUIImage(background_icon, Sprite.Create(x.icon, new Rect(0, 0, x.icon.width, x.icon.height), Vector2.zero));
                    icon.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
                    icon.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
                    icon.GetComponent<Image>().color = x.iconColor;
                }
            }
            catch(Exception ex)
            {
                ScriptTrainer.Instance.Log(ex, LogType.Error);
                background_icon.GetComponent<Image>().color = Color.white;
            }
            background_icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(70, 0);

            //创建文字
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(background, txtBgSprite, ColorUtility.ToHtmlStringRGBA(Color.white));
            uiText.GetComponent<Text>().text = GetItemName(item);
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(0, 5, 0);

            //var tip = background.AddComponent<TooltipGUI>();
            //tip.thing = TmpThings[item];
            //Debug.Log(GetItemDescription(item));
            //tip.Initialize(GetItemDescription(item));
            //VisualElement element = new VisualElement();

            //ToolTipHelper.Bind(background, delegate ()
            //{
            //    ToolTipHelper.ShowItemCard(background, ToolTipHelper.ePosition.ABOVE, GetCurEntity(), InventoryHelper.CreateThing(item), true, eItemCardViewModes.LOOT);
            //}, delegate ()
            //{
            //    ToolTipHelper.HideItemCard();
            //});
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
        public static Entity GetEntity(string guid)
        {
            return RouterHelper.Env.GameRun?.Entities.Find((Entity e) => e.Guid == guid);
        }
        public static Entity GetEntity(Entity player)
        {
            return RouterHelper.Env.GameRun?.Entities.Find((Entity e) => e.Guid == player.Guid);
        }
        public static Entity GetSelectEntity()
        {
            if (Players.Count > 0)
            {
                if (PlayerDropdown.value < Players.Count)
                    return GetEntity(Players[PlayerDropdown.value]);
                else
                    return GetEntity(Players[0]);
            }
            else
                return null;
        }
        public static List<Entity> GetPartyEntity()
        {
            List < Entity > entities = new List<Entity>();
            foreach (var player in Players)
            {
                var tmp = RouterHelper.Env.GameRun?.Entities.Find((Entity e) => e.Guid == player.Guid);
                if(tmp!=null)
                    entities.Add(tmp);
            }
            return entities;
        }
        public static void SpawnItem(String item, int amount)
        {
            if (Players.Count > 0)
            {
                Entity player = GetSelectEntity();
                if (player == null) return;
                List<Thing> things = player.Get<CharacterComponent>().Things;
                if (!IsCharacterStat(item))
                {
                    if(Configs.Things.TryGetValue(item, out ThingConfig thingConfig))
                    {
                        try
                        {
                            if (thingConfig.Tags.Contains("ARMOR") || thingConfig.Tags.Contains("WEAPON"))
                            {
                                InventoryHelper.GiveByName(item, amount, things, null, (int)thingConfig.MinTier, (int)thingConfig.MaxTier, new GameRandom());
                            }
                            else if(item == "CHAOS_REDUCE_01")
                            {
                                AdventureHelper.RemoveNextChaosEvent(RouterMono.GetEnv().GameRun);
                            }
                            else if(item == "LIFE_POOL_UP_01")
                            {
                                RouterMono.GetEnv().GameRun.CurrentLifePool++;
                                GlobalHeaderViewHelper.ShowLifePool();
                            }
                            else
                                InventoryHelper.GiveByName(item, amount, things, null, 0, 0, null);
                            ScriptTrainer.Instance.Log($"成功添加{amount}个物品 : {item}");
                        }
                        catch (Exception e)
                        {
                            ScriptTrainer.Instance.Log(e.Message, LogType.Error);
                        }
                    }
                    else
                    {
                        ScriptTrainer.Instance.Log($"Configs.Things不存在Key : {item}");
                    }
                }
                else
                {
                    if (Enum.TryParse<eCharacterStats>(item.Trim(), out eCharacterStats result))
                    {
                        if (result == eCharacterStats.PARTY_XP)
                            ProgressionHelper.PlayersGainXP(GetPartyEntity(), amount, true, new List<ValueTuple<eAbilityResults, object>>());
                        else if (result == eCharacterStats.XP)
                            ProgressionHelper.PlayerGainXP(player, amount, true, new List<ValueTuple<eAbilityResults, object>>());
                        else
                            CharacterHelper.AppendStat(player, result.ToString(), amount);
                    }
                }
                RefreshUI();
            }
        }
        public static bool IsCharacterStat(string item)
        {
            foreach (eCharacterStats o in Enum.GetValues(typeof(eCharacterStats)))
            {
                if(o.ToString().ToUpper() == item.Trim().ToUpper())
                    return true;
            }
            return false;
        }

        public static void RefreshUI()
        {
            try
            {
                RouterMono _router = (RouterMono)typeof(RouterHelper).GetField("_router", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);
                if (_router == null)
                    return;
                AdventureDirector _adventureDirector = (AdventureDirector)typeof(RouterMono).GetField("_adventureDirector", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(_router);

                _adventureDirector.GetType().GetMethod("_doRefreshUIAll", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Invoke(_adventureDirector, null);
            }
            catch(Exception e)
            {
                ScriptTrainer.Instance.Log($"刷新UI错误 : {e.Message}");
            }
        }
        #region[获取数据相关函数]
        private static bool TryGetData()
        {
            if (Configs.Things == null)
            {
                return false;
            }
            return true;
        }
        private static List<String> GetItemData(int index = 0)
        {
            List<String> ItemData = new List<string>();
            switch (index)
            {
                case 0:
                    ItemData = Configs.Things.Keys.ToList();
                    break;
                case 1:
                    int index2 = ItemRaritiesDropdown.value;
                    switch (index2)
                    {
                        default:
                        case 0:
                            foreach (var item in Configs.Things)
                            {
                                if (InventoryHelper.GetThingType(item.Key) == eThingTypes.EQUIPMENT && item.Value.Rarity == eItemRarities.COMMON)
                                {
                                    ItemData.Add(item.Key);
                                }
                            }
                            break;
                        case 1:
                            foreach (var item in Configs.Things)
                            {
                                if (InventoryHelper.GetThingType(item.Key) == eThingTypes.EQUIPMENT && item.Value.Rarity == eItemRarities.UNCOMMON)
                                {
                                    ItemData.Add(item.Key);
                                }
                            }
                            break;
                        case 2:
                            foreach (var item in Configs.Things)
                            {
                                if (InventoryHelper.GetThingType(item.Key) == eThingTypes.EQUIPMENT && item.Value.Rarity == eItemRarities.RARE)
                                {
                                    ItemData.Add(item.Key);
                                }
                            }
                            break;
                        case 3:
                            foreach (var item in Configs.Things)
                            {
                                if (InventoryHelper.GetThingType(item.Key) == eThingTypes.EQUIPMENT && item.Value.Rarity == eItemRarities.ARTIFACT)
                                {
                                    ItemData.Add(item.Key);
                                }
                            }
                            break;
                        case 4:
                            foreach (var item in Configs.Things)
                            {
                                if (InventoryHelper.GetThingType(item.Key) == eThingTypes.EQUIPMENT && item.Value.Rarity != eItemRarities.RARE && item.Value.Rarity != eItemRarities.COMMON && item.Value.Rarity != eItemRarities.UNCOMMON && item.Value.Rarity != eItemRarities.ARTIFACT)
                                {
                                    ItemData.Add(item.Key);
                                }
                            }
                            break;
                    }
                    break;
                case 2:
                    foreach (var item in Configs.Things)
                    {
                        if (item.Value.ConsumableType == eConsumableTypes.COMBAT)
                        {
                            ItemData.Add(item.Key);
                        }
                    }
                    break;
                case 3:
                    foreach (var item in Configs.Things)
                    {
                        if (item.Value.ConsumableType == eConsumableTypes.OVERWORLD || item.Value.ConsumableType == eConsumableTypes.ANY)
                        {
                            ItemData.Add(item.Key);
                        }
                    }
                    break;
                case 4:
                    foreach (var item in Configs.Things)
                    {
                        if (item.Value.ConsumableType == eConsumableTypes.EITHER || item.Value.ConsumableType == eConsumableTypes.TRIGGER)
                        {
                            ItemData.Add(item.Key);
                        }
                    }
                    break;
                case 5:
                    foreach (var item in Configs.Things)
                    {
                        if (IsCharacterStat(item.Key))
                        {
                            ItemData.Add(item.Key);
                        }
                    }
                    break;
                case 6:
                    foreach (var item in Configs.Things)
                    {
                        if (item.Value.ConsumableType == eConsumableTypes.NONE && InventoryHelper.GetThingType(item.Key) != eThingTypes.EQUIPMENT && !IsCharacterStat(item.Key))
                        {
                            Debug.Log(item.Key);
                            ItemData.Add(item.Key);
                        }
                    }
                    break;
                default:
                    ItemData = Configs.Things.Keys.ToList();
                    break;
            }

            Debug.Log($"ZG:全物品数量:{ItemData.Count}");
            if (searchText != "")
            {
                ItemData = FilterItemData(ItemData);
            }
            //Curse诅咒 Magazine
            //对 DataList 进行分页
            List<String> list = new List<String>();
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
        private static List<String> FilterItemData(List<String> dataList)
        {
            if (searchText == "")
            {
                return dataList;
            }
            List<String> list = new List<String>();

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
        private static string GetItemName(String item)
        {
            return Lang.__t(item, null, null, null, false) ?? "";
        }
        private static string GetItemDescription(String item)
        {
            if (item.StartsWith("TRAIT_"))
            {
                return Lang.__t(item + "_DESCRIPTION", null, null, null, false) ?? "";
            }
            if (Enum.TryParse<eConfigTags>(item, out eConfigTags obj))
            {
                return Lang.__t("UI_LORESTORE_FAMILY_DESCRIPTION", Lang.__t(item, null, null, null, false), null, null, false) ?? "";
            }
            if (item.StartsWith("GRAND_SANCTUM_"))
            {
                return AdventureHelper.GetSanctumEffectString(item);
            }
            return Lang.__t(item + "_DESCRIPTION", null, null, null, false) ?? "";
        }
        private static Sprite GetItemIcon(String item)
        {
            try
            {
                var im = UIHelper.GetThingIconTexture(item, out Color pIconColor, true);
                if (im != null)
                    return Sprite.Create(im, new Rect(0, 0, im.width, im.height), Vector2.zero);
                else return null;
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
