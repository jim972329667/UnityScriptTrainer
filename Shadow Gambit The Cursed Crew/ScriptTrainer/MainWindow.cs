using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityGameUI;
using static UnityEngine.RectTransform;
using Navigation = UnityGameUI.Navigation;
using Object = UnityEngine.Object;

namespace ScriptTrainer
{
    public class MainWindow : MonoBehaviour
    {
        public MainWindow(IntPtr handle) : base(handle) { }
        #region[声明]
        // Trainer Base
        public static GameObject obj = null;
        public static MainWindow instance;
        public static bool initialized = false;
        public static bool _optionToggle = false;
        private static TooltipGUI toolTipComp = null;
        public static DragAndDrog dragAndDrog = null;

        // UI
        public static GameObject canvas = null;
        private static GameObject uiPanel = null;
        public static readonly int width = Mathf.Min(Screen.width, 740);
        private static readonly int height = (Screen.height < 400) ? Screen.height : (450);

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
        public static bool optionToggle
        {
            get => _optionToggle;
            set
            {
                _optionToggle = value;

                if (!initialized)
                {
                    instance.CreateUI();
                }
                canvas.SetActive(value);
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
            
        }
        public void Start()
        {
            instance = this;
        }
        public static void Initialize()
        {
            #region[初始化资源]

            #endregion

            instance.CreateUI();

            initialized = true;
        }

        #region[创建UI]
        private void CreateUI()
        {
            if (canvas == null)
            {
                Debug.Log("创建 UI 元素");

                canvas = UIControls.createUICanvas();
                canvas.GetComponent<Canvas>().overrideSorting = true;
                canvas.GetComponent<Canvas>().sortingOrder = 10000;
                Object.DontDestroyOnLoad(canvas);
                // 设置背景
                GameObject background = UIControls.createUIPanel(canvas, (height + 40).ToString(), (width + 40).ToString(), null);
                background.GetComponent<Image>().color = UIControls.HTMLString2Color("#2D2D30FF");

                // 将面板添加到画布, 请参阅 createUIPanel 了解我们将高度/宽度作为字符串传递的原因
                uiPanel = UIControls.createUIPanel(background, height.ToString(), width.ToString(), null);
                // 设置背景颜色
                uiPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");

                // 这就是我们将如何挂钩鼠标事件以进行窗口拖动
                MainWindow.dragAndDrog = MainWindow.canvas.AddComponent<DragAndDrog>();

                #region[面板元素]


                #region[创建标题 和 关闭按钮]
                AddTitle(ScriptTrainer.PluginName + " by:Jim97 版本:" + ScriptTrainer.Version, background);

                GameObject closeButton = UIControls.createUIButton(uiPanel, "#B71C1CFF", "X", (UnityAction)(() =>
                {
                    optionToggle = false;
                    canvas.SetActive(optionToggle);
                }), new Vector3(width / 2 + 10, height / 2 + 10, 0));
                closeButton.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
                // 字体颜色为白色
                closeButton.GetComponentInChildren<Text>().color = UIControls.HTMLString2Color("#FFFFFFFF");

                #endregion

                GameObject BasicScripts = UIControls.createUIPanel(uiPanel, "410", "600", null);
                BasicScripts.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                BasicScripts.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, -20);

                #region[添加功能按钮]
                AddH3("常用功能：", BasicScripts);
                {
                    //AddToggle("风暴变为30回合", 150, BasicScripts, (bool state) =>
                    //{
                    //    Scripts.ChangeStormDelay(state);
                    //});
                    AddButton("全部角色成就", BasicScripts, () =>
                    {
                        var save = SaveGameManager.instance.saveGameUser;
                        int session = 0;
                        int guid = 0;
                        string text = string.Empty;
                        List<string> list = new List<string>()
                        {
                            "badge_cha_bln_finish_mission",
                            "badge_cha_bln_kill_ko",
                            "badge_cha_bln_travel_blinking",
                            "badge_cha_bsp_finish_mission",
                            "badge_cha_bsp_hide_corpses",
                            "badge_cha_bsp_kill_ko",
                            "badge_cha_bsp_walk_away_distance",
                            "badge_cha_gcw_finish_mission",
                            "badge_cha_gcw_kill_ko",
                            "badge_cha_snp_finish_mission",
                            "badge_cha_snp_kill_ko",
                            "badge_cha_snp_kill_or_ko_sharpshooters",
                            "badge_cha_tpb_finish_mission",
                            "badge_cha_tpb_kill_ko",
                            "badge_cha_cnn_finish_mission",
                            "badge_cha_cnn_kill_ko",
                            "badge_cha_cnn_carry_players_around",
                            "badge_cha_fsh_finish_mission",
                            "badge_cha_fsh_kill_ko",
                            "badge_cha_fsh_fish_hide_bodies",
                            "badge_cha_ovt_finish_mission",
                            "badge_cha_ovt_kill_ko",
                            "badge_cha_ovt_travel_overtaken"
                        };
                        for(int i = 0;i< save.missions.m_liBadgeSaveData.Count; i++)
                        {
                            BadgeAsset badgeAsset = save.missions.m_liBadgeSaveData[i].badgeAsset;
                            text += $"{badgeAsset.name};{badgeAsset.m_lstrName.strText}\n";
                            if (save.missions.m_liBadgeSaveData[i].bCompleted)
                            {
                                session = save.missions.m_liBadgeSaveData[i].iCompletedInSessionId;
                                guid = save.missions.m_liBadgeSaveData[i].iCompletedInMissionWithGuid;
                            }
                        }
                        File.WriteAllText("E:\\Games\\Shadow Gambit The Cursed Crew\\BepInEx\\test.txt", text);
                        foreach (var badge in save.missions.m_liBadgeSaveData)
                        {
                            if (badge.m_badgeAsset.m_eType == BadgeAsset.Type.Character)
                            {
                                badge.trySetComplete(session, guid);
                            }
                            if (list.Contains(badge.m_badgeAsset.name))
                            {
                                if (badge.m_badgeAsset.m_badgeUnlockSettings.bSaveProgress)
                                {
                                    if (badge.iSavedProgress == badge.m_badgeAsset.m_badgeUnlockSettings.iCount)
                                        continue;
                                }
                                
                                badge.debug_resetComplete();
                                Debug.Log($"成功重置{badge.m_badgeAsset.m_lstrName.strText}");
                            }
                        }




                    });
                    AddButton("人数上限修改", BasicScripts, () =>
                    {
                        foreach (var mission in MissionDataCollection.instance.m_liMissionData)
                        {
                            mission.m_iMaxCharacter = 8;
                            mission.m_iMaxCharacterReplay = 8;//mis_fortress_01_mission1_missiondata_00
                        }
                    });
                    AddButton("No CD", BasicScripts, () =>
                    {
                        foreach (PlayerSkillData skill in Resources.FindObjectsOfTypeAll<PlayerSkillData>())
                        {
                            skill.m_fCooldown = new BalancedFloat(0);
                        }
                    });
                    AddButton("修改弹药数量", BasicScripts, () =>
                    {
                        foreach(var character in MiGameInput.instance.liPlayerCharacter)
                        {
                            var inv = character.inventory;
                            foreach(var item in inv.m_lItems)
                            {
                                item.iCount = 999;
                            }
                        }  
                    });
                    AddButton("保存", BasicScripts, () =>
                    {
                        var save = MiSingletonNoMono<SaveGameManager>.instance.saveGameUser;
                        string text = string.Empty;
                        for (int i = 0; i < save.missions.m_liBadgeSaveData.Count; i++)
                        {
                            BadgeAsset badgeAsset = save.missions.m_liBadgeSaveData[i].badgeAsset;
                            text += $"{badgeAsset.name};{badgeAsset.m_lstrName.strText}\n";
                        }
                        File.WriteAllText("E:\\Games\\Shadow Gambit The Cursed Crew\\BepInEx\\test.txt", text);
                    });


                    hr();

                    hr(10);
                }

                #endregion

                #region[获取物品]
                //ResetCoordinates(true, true);
                //GameObject ItemScripts = UIControls.createUIPanel(uiPanel, "410", "600", null);
                //ItemScripts.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                //ItemScripts.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, -20);

                //ItemWindow itemWindow = new ItemWindow(ItemScripts, elementX, elementY);


                #endregion
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
                    //new Navigation("ItemScripts", "物品添加", ItemScripts, false),
                };

                UINavigation.Initialize(nav, NavPanel);

                #endregion

                canvas.SetActive(optionToggle);
                Debug.Log("初始化完成!");
            }
        }

        #region[添加组件]
        //添加按钮
        public static GameObject AddButton(string Text, GameObject panel, Action action)
        {
            string backgroundColor = "#8C9EFFFF";
            Vector3 localPosition = new Vector3(elementX, elementY, 0);
            elementX += 110;

            GameObject button = UIControls.createUIButton(panel, backgroundColor, Text, action, localPosition);

            // 按钮样式
            button.AddComponent<Shadow>().effectColor = UIControls.HTMLString2Color("#000000FF");   // 添加阴影
            button.GetComponent<Shadow>().effectDistance = new Vector2(2, -2);              // 设置阴影偏移
            button.GetComponentInChildren<Text>().fontSize = 14;     // 设置字体大小           
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);    // 设置按钮大小


            return button;
        }
        // 添加复选框
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
        // 添加输入框
        public static GameObject AddInputField(string Text, int width, string defaultText, GameObject panel, UnityAction<string> action)
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
            elementX += 10;

            // 输入框
            Sprite inputFieldSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject uiInputField = UIControls.createUIInputField(panel, inputFieldSprite, "#FFFFFFFF");
            uiInputField.GetComponent<InputField>().text = defaultText;
            uiInputField.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);
            uiInputField.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 60, 30);

            // 文本框失去焦点时触发方法
            uiInputField.GetComponent<InputField>().onEndEdit.AddListener(action);

            elementX += width / 2 + 10;
            return uiInputField;
        }
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
            elementX += 60;

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
        // 添加小标题
        public GameObject AddH3(string text, GameObject panel)
        {
            elementX += 40;

            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(panel, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<Text>().text = text;
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);

            // 设置字体样式为h3小标题
            uiText.GetComponent<Text>().fontSize = 14;
            uiText.GetComponent<Text>().fontStyle = FontStyle.Bold;
            hr();
            elementY += 20;
            elementX += 10;
            return uiText;
        }
        // 添加标题
        public static GameObject AddTitle(string Title, GameObject background)
        {
            GameObject TitleBackground = UIControls.createUIPanel(background, "30", (width - 20).ToString(), null);
            TitleBackground.GetComponent<Image>().color = UIControls.HTMLString2Color("#2D2D30FF");
            TitleBackground.GetComponent<RectTransform>().localPosition = new Vector3(0, height / 2 - 30, 0);

            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(TitleBackground, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 10, 30);
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            Text text = uiText.GetComponent<Text>();
            text.text = Title;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 16;

            return uiText;
        }
        // 换行
        public void hr(int offsetX = 0, int offsetY = 0)
        {
            ResetCoordinates(true);
            elementX += offsetX;
            elementY -= 50 + offsetY;

        }
        // 重置坐标
        public void ResetCoordinates(bool x, bool y = false)
        {
            if (x) elementX = initialX;
            if (y) elementY = initialY;
        }
        #endregion



        #endregion




    }
}
