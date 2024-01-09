using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityGameUI;
using UniverseLib;
using static UnityEngine.Random;
using Extensions = UnityGameUI.Extensions;
using Navigation = UnityGameUI.Navigation;
using Object = UnityEngine.Object;

namespace ScriptTrainer
{
    public class MainWindow : MonoBehaviour
    {
        #region[声明]
        // Trainer companionSetInLocation
        public static GameObject obj = null;
        public static MainWindow instance;
        public static bool initialized = false;
        public static bool _optionToggle = false;
        private static TooltipGUI toolTipComp = null;
        public static DragAndDrog DragAndDrog=null;
        // UI
        public static AssetBundle testAssetBundle = null;
        public static GameObject canvas = null;
        private static GameObject uiPanel = null;
        public static readonly int width = Mathf.Min(Screen.width, 740);
        private static readonly int height = (Screen.height < 400) ? Screen.height : (450);

        public static GameObject PickInput = null;
        public static GameObject StackInput = null;
        public static GameObject SpawnSettingsInput = null;
        public static GameObject ToolUseInput = null;
        // 按钮位置
        private static int elementX = initialX;
        private static int elementY = initialY;
        private static int initialX
        {
            get
            {
                return -width / 2 + 120;
            }
        }
        private static int initialY
        {
            get
            {
                return height / 2 - 60;
            }
        }
        public static bool CursorVisible = false;
        public static CursorLockMode lockMode = CursorLockMode.None;
        public static bool optionToggle
        {
            get => _optionToggle;
            set
            {
                _optionToggle = value;
                if (_optionToggle)
                {
                    //MainController.Instance.AppServices.InputService.Config.Disable();
                }
                else
                {
                    //MainController.Instance.AppServices.InputService.Config.Enable();
                    DragAndDrog.isMouseDrag = false;
                }
                if (!initialized)
                {
                    instance.CreateUI();
                }
            }
        }

        #endregion

        internal static GameObject Create(string name)
        {
            obj = new GameObject(name);
            DontDestroyOnLoad(obj);

            var component = new MainWindow();

            toolTipComp = new TooltipGUI();
            toolTipComp.enabled = false;

            return obj;
        }

        public MainWindow()
        {
            instance = this;
        }

        public static void Initialize()
        {
            #region[初始化资源]
            instance.CreateUI();
            Canvas.ForceUpdateCanvases();
            initialized = true;
            #endregion
        }

        #region[创建UI]
        private void CreateUI()
        {
            if (canvas == null)
            {
                Debug.Log("创建 UI 元素");

                canvas = UIControls.createUICanvas();
                canvas.GetComponent<Canvas>().overrideSorting = true;
                canvas.GetComponent<Canvas>().sortingOrder= 99;
                Object.DontDestroyOnLoad(canvas);
                
                // 设置背景
                GameObject background = UIControls.createUIPanel(canvas, (height + 40).ToString(), (width + 40).ToString(), null);
                background.GetComponent<Image>().color = UIControls.HTMLString2Color("#2D2D30FF");
                // 将面板添加到画布, 请参阅 createUIPanel 了解我们将高度/宽度作为字符串传递的原因
                uiPanel = UIControls.createUIPanel(background, height.ToString(), width.ToString(), null);
                // 设置背景颜色
                uiPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");

                // 这就是我们将如何挂钩鼠标事件以进行窗口拖动
                #region[面板元素]


                #region[创建标题 和 关闭按钮]
                var title = UIWindows.AddTitle(uiPanel, $"{ScriptTrainer.Instance.Info.Metadata.Name} V{ScriptTrainer.Instance.Info.Metadata.Version} by:Jim97");

                DragAndDrog = title.AddComponent<DragAndDrog>();
                DragAndDrog.SetTarget(background);

                GameObject closeButton = UIControls.createUIButton(uiPanel, "#B71C1CFF", "X", () =>
                {
                    optionToggle = false;
                    canvas.SetActive(optionToggle);
                }, new Vector3(width / 2 + 10, height / 2 + 10, 0));
                RuntimeHelper.SetColorBlock(closeButton.GetComponent<Button>(), new Color?(new Color(0.63f, 0.32f, 0.31f)), new Color?(new Color(0.81f, 0.25f, 0.2f)), new Color?(new Color(0.6f, 0.18f, 0.16f)), null);
                closeButton.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
                // 字体颜色为白色
                closeButton.GetComponentInChildren<Text>().color = UIControls.HTMLString2Color("#FFFFFFFF");

                #endregion

                GameObject BasicScripts = UIControls.createUIPanel(uiPanel, "400", "600", null);
                BasicScripts.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                BasicScripts.GetComponent<RectTransform>().anchoredPosition = new Vector2(-60, -25);
                Vector2 point = UIWindows.ResetCoordinates(BasicScripts, new Vector2(), true, true);
                #region[添加功能按钮]
                UIWindows.AddH3(ref point,"常用功能：", BasicScripts);
                {
                    UIWindows.AddButton(ref point, "解锁全皮肤", BasicScripts, () =>
                    {
                        var env = RouterHelper.Env;
                        if (env != null)
                        {
                            foreach (var x in Env.Configs.LoreStore)
                            {
                                if (x.Key.StartsWith("SKIN_"))
                                {
                                    
                                    if(StatsHelper.GetStat(x.Key, env.User.Stats) <= 0)
                                    {
                                        //解锁
                                        StatsHelper.SetStat(x.Key, 0, env.User.Stats, false, true);
                                        int itemCost = LoreStoreHelper.GetItemCost(x.Key, env.User.Stats);
                                        //购买
                                        StatsHelper.IncrementStat(x.Key, env.User.Stats, false, true);
                                        //StatsHelper.AddStat("TOTAL_LORE", -itemCost, env.User.Stats, false, true);
                                        StatsHelper.IncrementStat(eUserStats.LORE_POINTS_SPENT, env.User.Stats, itemCost, false, true);
                                        StatsHelper.IncrementStat(eUserStats.LORE_STORE_PURCHASES, env.User.Stats, false, true);
                                        SaveGameHelper.SaveUser(env.User);
                                    }
                                }
                            }
                        }
                    });
                    UIWindows.AddButton(ref point, "解锁学识商店", BasicScripts, () =>
                    {
                        var env = RouterHelper.Env;
                        if (env != null)
                        {

                            foreach (var x in Env.Configs.LoreStore)
                            {
                                if (StatsHelper.GetStat(x.Key, env.User.Stats) <= 0)
                                {
                                    //解锁
                                    StatsHelper.SetStat(x.Key, 0, env.User.Stats, false, true);
                                    int itemCost = LoreStoreHelper.GetItemCost(x.Key, env.User.Stats);
                                    //购买
                                    for (int i = 0; i < x.Value.MaxState; i++)
                                    {
                                        StatsHelper.IncrementStat(x.Key, env.User.Stats, false, true);
                                        //StatsHelper.AddStat("TOTAL_LORE", -itemCost, env.User.Stats, false, true);
                                        StatsHelper.IncrementStat(eUserStats.LORE_POINTS_SPENT, env.User.Stats, itemCost, false, true);
                                        StatsHelper.IncrementStat(eUserStats.LORE_STORE_PURCHASES, env.User.Stats, false, true);
                                        SaveGameHelper.SaveUser(env.User);
                                    }
                                }
                            }
                        }
                    });
                    UIWindows.AddButton(ref point, "增加100学识", BasicScripts, () =>
                    {
                        var env = RouterHelper.Env;
                        if (env != null)
                        {
                            StatsHelper.AddStat("TOTAL_LORE", 100, env.User.Stats, false, true);
                        }
                    });
                    UIWindows.AddToggle(ref point, "无限准备点数", BasicScripts, ScriptPatch.InfiniteLoadoutPoints, (bool state) =>
                    {
                        ScriptPatch.InfiniteLoadoutPoints = state;
                    });
                    UIWindows.Hr(ref point, BasicScripts);

                    UIWindows.AddButton(ref point, "治疗选中角色", BasicScripts, Scripts.CurePlayer);
                    UIWindows.AddButton(ref point, "治疗全队", BasicScripts, Scripts.CurePlayers);
                    //StackInput = UIWindows.AddInputField(ref point, 120, ScriptTrainer.StackSizeMult.Value.ToString(), BasicScripts, (string text) =>
                    // {
                    //     ScriptTrainer.StackSizeMult.Value = text.ConvertToIntDef(10);
                    // });
                    //UIWindows.Hr(ref point, BasicScripts);
                    //UIWindows.AddToggle(ref point, "多倍海上物品刷新(仅限房主)", BasicScripts, ScriptTrainer.ChangeSpawnSettings, (bool state) =>
                    //{
                    //    SpawnSettingsInput?.SetActive(!state);
                    //});
                    //SpawnSettingsInput = UIWindows.AddInputField(ref point, 120, ScriptTrainer.SpawnSettingsMult.Value.ToString(), BasicScripts, (string text) =>
                    //{
                    //    ScriptTrainer.SpawnSettingsMult.Value = text.ConvertToFloatDef(2.0f);
                    //});
                    //UIWindows.Hr(ref point, BasicScripts);
                    //UIWindows.AddToggle(ref point, "工具耐久度倍率", BasicScripts, ScriptTrainer.ChangeToolUse, (bool state) =>
                    //{
                    //    ToolUseInput?.SetActive(!state);
                    //});
                    //ToolUseInput = UIWindows.AddInputField(ref point, 120, ScriptTrainer.ToolUseMult.Value.ToString(), BasicScripts, (string text) =>
                    //{
                    //    ScriptTrainer.ToolUseMult.Value = text.ConvertToIntDef(2);
                    //});

                    //UIWindows.AddButton(ref point, "开启游戏debug", BasicScripts, () => { 
                    //    ScriptPatch.AllowCheat = true;
                    //    Cheat.LoadSteamUserSO();
                    //});



                    //PickInput?.SetActive(!ScriptTrainer.ChangePick.Value);
                    //StackInput?.SetActive(!ScriptTrainer.ChangeStackSize.Value);
                    //SpawnSettingsInput?.SetActive(!ScriptTrainer.ChangeSpawnSettings.Value);
                    //ToolUseInput?.SetActive(!ScriptTrainer.ChangeToolUse.Value);
                }

                #endregion

                #region[获取卡牌]
                //ResetCoordinates(true, true);
                GameObject ItemScripts = UIControls.createUIPanel(uiPanel, "410", "600", null);
                ItemScripts.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                ItemScripts.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, -20);

                ItemWindow itemWindow = new ItemWindow(ItemScripts, elementX, elementY);


                #endregion

                //#region[获取法宝]
                //ResetCoordinates(true, true);
                //GameObject RelicScripts = UIControls.createUIPanel(uiPanel, "410", "600", null);
                //RelicScripts.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                //RelicScripts.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, -20);

                //RelicWindow relicWindow = new RelicWindow(RelicScripts, elementX, elementY);
                //#endregion

                //#region[获取消耗品]
                //ResetCoordinates(true, true);
                //GameObject PotionScripts = UIControls.createUIPanel(uiPanel, "410", "600", null);
                //PotionScripts.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                //PotionScripts.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, -20);

                //PotionWindow potionWindow = new PotionWindow(PotionScripts, elementX, elementY);
                //#endregion

                #endregion


                #region[创建导航栏]
                // 分割线
                GameObject DividingLine = UIControls.createUIPanel(uiPanel, (height - 40).ToString(), "10", null);
                DividingLine.GetComponent<Image>().color = UIControls.HTMLString2Color("#2D2D30FF");
                DividingLine.GetComponent<RectTransform>().anchoredPosition = new Vector3(width / 2 - 200 + 80, -20, 0);

                //// 按钮
                GameObject NavPanel = UIControls.createUIPanel(uiPanel, (height - 40).ToString(), "40", null);
                NavPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                NavPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(width / 2 - 100, -20, 0);

                Navigation[] nav = new Navigation[]
                {
                    new Navigation("BasicScripts","基础功能", BasicScripts, true),
                    new Navigation("ItemScripts", "物品添加", ItemScripts, false),
                    //new Navigation("RelicScripts", "法宝添加", RelicScripts, false),
                    //new Navigation("PotionScripts", "消耗品添加", PotionScripts, false),
                };

                UINavigation.Initialize(nav, NavPanel);

                #endregion

                canvas.SetActive(optionToggle);
                Debug.Log("初始化完成!");
            }
        }

        #region[添加组件]
        // 添加下拉框
        public GameObject AddDropdown(string Text, int width, List<string> options, GameObject panel, UnityAction<int> action)
        {
            // 计算x轴偏移
            elementX += width / 2 - 30;

            // label
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(panel, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<Text>().text = Text;
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);
            //uiText.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 30);
            uiText.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

            // 坐标偏移
            elementX += 65;

            // 创建下拉框
            Sprite dropdownBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));      // 背景颜色
            Sprite dropdownScrollbarSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));   // 滚动条颜色 (如果有的话
            Sprite dropdownDropDownSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));    // 框右侧小点的颜色
            Sprite dropdownCheckmarkSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));   // 选中时的颜色
            Sprite dropdownMaskSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#E65100FF"));        // 不知道是哪的颜色
            Color LabelColor = UIControls.HTMLString2Color("#EFEBE9FF");
            GameObject uiDropDown = UIControls.createUIDropDown(panel, dropdownBgSprite, dropdownScrollbarSprite, dropdownDropDownSprite, dropdownCheckmarkSprite, dropdownMaskSprite, options, LabelColor);
            Object.DontDestroyOnLoad(uiDropDown);
            uiDropDown.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);

            // 下拉框选中时触发方法
            uiDropDown.GetComponent<Dropdown>().onValueChanged.AddListener(action);

            elementX += width / 2 + 60;
            return uiDropDown;
        }

        #endregion



        #endregion




    }
}
