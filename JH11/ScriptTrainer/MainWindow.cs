using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityGameUI;

namespace ScriptTrainer
{
    // Token: 0x02000010 RID: 16
    public class MainWindow : MonoBehaviour
    {
        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000082 RID: 130 RVA: 0x0000860C File Offset: 0x0000680C
        private static int initialX
        {
            get
            {
                return -MainWindow.width / 2 + 120;
            }
        }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000083 RID: 131 RVA: 0x0000862C File Offset: 0x0000682C
        private static int initialY
        {
            get
            {
                return MainWindow.height / 2 - 60;
            }
        }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000084 RID: 132 RVA: 0x00008648 File Offset: 0x00006848
        // (set) Token: 0x06000085 RID: 133 RVA: 0x00008650 File Offset: 0x00006850
        public static bool optionToggle
        {
            get
            {
                return MainWindow._optionToggle;
            }
            set
            {
                MainWindow._optionToggle = value;
                bool optionToggle = MainWindow._optionToggle;
                if (!optionToggle)
                {
                    ScriptTrainer.Instance.Log("关闭修改器菜单");
                    MainWindow.dragAndDrog.isMouseDrag = false;
                }
                else
                {
                    ScriptTrainer.Instance.Log("打开修改器菜单");
                }
                bool flag = !MainWindow.initialized;
                if (flag)
                {
                    MainWindow.instance.CreateUI();
                }
            }
        }

        // Token: 0x06000086 RID: 134 RVA: 0x00008698 File Offset: 0x00006898
        internal static GameObject Create(string name)
        {
            MainWindow.obj = new GameObject(name);
            UnityEngine.Object.DontDestroyOnLoad(MainWindow.obj);
            MainWindow mainWindow = new MainWindow();
            MainWindow.toolTipComp = new TooltipGUI();
            MainWindow.toolTipComp.enabled = false;
            return MainWindow.obj;
        }

        // Token: 0x06000087 RID: 135 RVA: 0x000086E1 File Offset: 0x000068E1
        public MainWindow()
        {
            MainWindow.instance = this;
        }

        // Token: 0x06000088 RID: 136 RVA: 0x000086F1 File Offset: 0x000068F1
        public static void Initialize()
        {
            MainWindow.instance.CreateUI();
            MainWindow.initialized = true;
        }

        // Token: 0x06000089 RID: 137 RVA: 0x00008708 File Offset: 0x00006908
        private void CreateUI()
        {
            bool flag = MainWindow.canvas == null;
            if (flag)
            {
                MainWindow.canvas = UIControls.createUICanvas(ScriptTrainer.WindowSizeFactor.Value);
                MainWindow.canvas.GetComponent<Canvas>().overrideSorting = true;
                MainWindow.canvas.GetComponent<Canvas>().sortingOrder = 10000;
                UnityEngine.Object.DontDestroyOnLoad(MainWindow.canvas);

                GameObject gameObject = UIControls.createUIPanel(MainWindow.canvas, (MainWindow.height + 40).ToString(), (MainWindow.width + 40).ToString(), null);

                gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#2D2D30FF");
                MainWindow.uiPanel = UIControls.createUIPanel(gameObject, MainWindow.height.ToString(), MainWindow.width.ToString(), null);
                MainWindow.uiPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");

                MainWindow.dragAndDrog = gameObject.AddComponent<DragAndDrog>();
                dragAndDrog.WindowSizeFactor = ScriptTrainer.WindowSizeFactor.Value;


                //MainWindow.dragAndDrog.WindowSize = new Vector2((float)(MainWindow.width + 40) * ScriptTrainer.ScaleFactor.Value, (float)(MainWindow.height + 40) * ScriptTrainer.ScaleFactor.Value);

                AddTitle(ScriptTrainer.Instance.Info.Metadata.Name + " by:Jim97 版本:" + ScriptTrainer.Instance.Info.Metadata.Version, gameObject);
                GameObject gameObject2 = UIControls.createUIButton(MainWindow.uiPanel, "#B71C1CFF", "X", delegate
                {
                    MainWindow.optionToggle = false;
                    MainWindow.canvas.SetActive(MainWindow.optionToggle);
                }, new Vector3((float)(MainWindow.width / 2 + 10), (float)(MainWindow.height / 2 + 10), 0f));
                gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 20f);
                gameObject2.GetComponentInChildren<Text>().color = UIControls.HTMLString2Color("#FFFFFFFF");
                GameObject BasicScripts = UIControls.createUIPanel(MainWindow.uiPanel, "410", "600", null);

                BasicScripts.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                BasicScripts.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70f, -20f);

                this.AddH3("常用功能：", BasicScripts);
                MainWindow.AddButton("添加金钱", BasicScripts, delegate ()
                {
                    UIWindows.SpawnInputDialog("您想获得多少贯？)", "获得", "100", delegate (string count)
                    {
                        Scripts.AddMoney(count.ConvertToIntDef(100));
                    });
                });
                MainWindow.AddButton("增加基础内力", BasicScripts, delegate ()
                {
                    UIWindows.SpawnInputDialog("您想获得多少内力？)", "获得", "1000", delegate (string count)
                    {
                        Scripts.AddNeiLi(count.ConvertToFloatDef(1000f));
                    });
                });
                MainWindow.AddButton("增加江湖阅历", BasicScripts, delegate ()
                {
                    UIWindows.SpawnInputDialog("您想获得多少江湖阅历？)", "获得", "1000", delegate (string count)
                    {
                        Scripts.AddJiangHuYueLi(count.ConvertToFloatDef(1000f));
                    });
                });
                MainWindow.AddButton("回复状态", BasicScripts, delegate ()
                {
                    Scripts.Recover();
                });
                MainWindow.AddButton("增加天书秘帛", BasicScripts, delegate ()
                {
                    UIWindows.SpawnInputDialog("您想获得多少天书秘帛？)", "获得", "5", delegate (string count)
                    {
                        Scripts.AddCangWuPoint(count.ConvertToIntDef(5));
                    });
                });
                this.hr(0, 0);
                MainWindow.AddToggle("战斗无冷却", 100, ScriptTrainer.NoCoolDown, BasicScripts, delegate (bool state)
                {
                });
                MainWindow.AddToggle("无限正面BUFF时间", 150, ScriptTrainer.InfiniteBuff, BasicScripts, delegate (bool state)
                {
                });
                MainWindow.AddToggle("取消修罗场事件", 150, ScriptTrainer.DuoQingLv, BasicScripts, delegate (bool state)
                {
                });
                MainWindow.AddToggle("战斗后自动恢复内力", 150, ScriptTrainer.AutoRecoverNeiLi, BasicScripts, delegate (bool state)
                {
                });
                this.hr(0, 0);
                this.AddH3("倍率修改：", BasicScripts);
                MainWindow.elementX -= 10;
                MainWindow.AddToggle("武功修炼经验倍率", 150, ScriptTrainer.MultipleExperience, BasicScripts, delegate (bool state)
                {
                    if (state)
                    {
                        MainWindow.ExpTextInput.SetActive(false);
                    }
                    else
                    {
                        MainWindow.ExpTextInput.SetActive(true);
                    }
                });
                MainWindow.elementX += 10;
                MainWindow.ExpTextInput = MainWindow.AddInputField(100, ScriptTrainer.MultipleExperienceRate.Value.ToString(), BasicScripts, delegate (string text)
                {
                    ScriptTrainer.MultipleExperienceRate.Value = text.ConvertToFloatDef((float)ScriptTrainer.MultipleExperienceRate.DefaultValue);
                });
                if(ScriptTrainer.MultipleExperience.Value)
                    ExpTextInput.SetActive(false);
                MainWindow.elementX += 40;
                MainWindow.AddToggle("地区好感度倍率", 150, ScriptTrainer.MultiplePlaceRelation, BasicScripts, delegate (bool state)
                {
                    if (state)
                    {
                        MainWindow.PlaceTextInput.SetActive(false);
                    }
                    else
                    {
                        MainWindow.PlaceTextInput.SetActive(true);
                    }
                });
                MainWindow.elementX += 10;
                MainWindow.PlaceTextInput = MainWindow.AddInputField(100, ScriptTrainer.MultiplePlaceRate.Value.ToString(), BasicScripts, delegate (string text)
                {
                    ScriptTrainer.MultiplePlaceRate.Value = text.ConvertToFloatDef((float)ScriptTrainer.MultiplePlaceRate.DefaultValue);
                });
                if (ScriptTrainer.MultiplePlaceRelation.Value)
                    PlaceTextInput.SetActive(false);

                this.hr(0, 0);
                MainWindow.AddToggle("人物好感度倍率", 150, ScriptTrainer.MultipleCharacterRelation, BasicScripts, delegate (bool state)
                {
                    if (state)
                    {
                        MainWindow.CharacterTextInput.SetActive(false);
                    }
                    else
                    {
                        MainWindow.CharacterTextInput.SetActive(true);
                    }
                });
                MainWindow.elementX += 10;
                MainWindow.CharacterTextInput = MainWindow.AddInputField(100, ScriptTrainer.MultiplePlaceRate.Value.ToString(), BasicScripts, delegate (string text)
                {
                    ScriptTrainer.MultiplePlaceRate.Value = text.ConvertToFloatDef((float)ScriptTrainer.MultiplePlaceRate.DefaultValue);
                });

                if (ScriptTrainer.MultipleCharacterRelation.Value)
                    CharacterTextInput.SetActive(false);

                MainWindow.elementX += 40;
                MainWindow.AddToggle("缩短参悟时间倍率", 150, ScriptTrainer.MultipleCanWu, BasicScripts, delegate (bool state)
                {
                    if (state)
                    {
                        MainWindow.CanWuTextInput.SetActive(false);
                    }
                    else
                    {
                        MainWindow.CanWuTextInput.SetActive(true);
                    }
                });
                MainWindow.elementX += 10;
                MainWindow.CanWuTextInput = MainWindow.AddInputField(100, ScriptTrainer.MultipleCanWuRate.Value.ToString(), BasicScripts, delegate (string text)
                {
                    ScriptTrainer.MultipleCanWuRate.Value = text.ConvertToFloatDef((float)ScriptTrainer.MultipleCanWuRate.DefaultValue);
                });

                if (ScriptTrainer.MultipleCanWu.Value)
                    CanWuTextInput.SetActive(false);

                this.hr(0, 0);
                MainWindow.AddToggle("参悟获得属性倍率", 150, ScriptTrainer.MultipleCanWuShuXing, BasicScripts, delegate (bool state)
                {
                    if (state)
                    {
                        MainWindow.CanWuShuXingTextInput.SetActive(false);
                    }
                    else
                    {
                        MainWindow.CanWuShuXingTextInput.SetActive(true);
                    }
                });
                MainWindow.elementX += 10;
                MainWindow.CanWuShuXingTextInput = MainWindow.AddInputField(100, ScriptTrainer.MultipleCanWuShuXingRate.Value.ToString(), BasicScripts, delegate (string text)
                {
                    ScriptTrainer.MultipleCanWuShuXingRate.Value = text.ConvertToIntDef((int)ScriptTrainer.MultipleCanWuShuXingRate.DefaultValue);
                });

                if (ScriptTrainer.MultipleCanWuShuXing.Value)
                    CanWuShuXingTextInput.SetActive(false);

                MainWindow.elementX += 40;
                MainWindow.AddToggle("修改游戏时间倍率", 150, ScriptTrainer.TimeScale, BasicScripts, delegate (bool state)
                {
                    if (state)
                    {
                        MainWindow.TimeScaleTextInput.SetActive(false);

                    }
                    else
                    {
                        Time.timeScale = 1f;
                        MainWindow.TimeScaleTextInput.SetActive(true);
                    }
                });
                MainWindow.elementX += 10;
                MainWindow.TimeScaleTextInput = MainWindow.AddInputField(100, ScriptTrainer.TimeScaleRate.Value.ToString(), BasicScripts, delegate (string text)
                {
                    ScriptTrainer.TimeScaleRate.Value = text.ConvertToFloatDef((float)ScriptTrainer.TimeScaleRate.DefaultValue);
                });

                if (ScriptTrainer.TimeScale.Value)
                    TimeScaleTextInput.SetActive(false);


                this.hr(0, 0);
                this.AddH3("时间操作：", BasicScripts);
                MainWindow.elementX -= 10;
                MainWindow.AddToggle("暂停时间", 120, ScriptTrainer.StopTime, BasicScripts, delegate (bool state)
                {
                });
                MainWindow.AddButton("跳过一阶段", BasicScripts, delegate ()
                {
                    Scripts.SkipTime(1f);
                });
                MainWindow.AddButton("跳过一天", BasicScripts, delegate ()
                {
                    Scripts.SkipTime(4f);
                });


                #region[角色修改]
                ResetCoordinates(true, true);
                GameObject CharacterScripts = UIControls.createUIPanel(uiPanel, "410", "600", null);
                CharacterScripts.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                CharacterScripts.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, -20);

                CharacterWindow characterWindow = new CharacterWindow(CharacterScripts, elementX, elementY);
                #endregion

                #region[角色修改]
                ResetCoordinates(true, true);
                GameObject ItemScripts = UIControls.createUIPanel(uiPanel, "410", "600", null);
                ItemScripts.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                ItemScripts.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, -20);

                ItemWindow itemWindow = new ItemWindow(ItemScripts, elementX, elementY);
                #endregion

                #region[地区修改]
                ResetCoordinates(true, true);
                GameObject PlaceScripts = UIControls.createUIPanel(uiPanel, "410", "600", null);
                PlaceScripts.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                PlaceScripts.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, -20);

                PlaceWindow placeWindow = new PlaceWindow(PlaceScripts, elementX, elementY);
                #endregion

                #region[地区好感度修改]
                ResetCoordinates(true, true);
                GameObject RelationScripts = UIControls.createUIPanel(uiPanel, "410", "600", null);
                RelationScripts.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                RelationScripts.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, -20);

                RelationWindow relationWindow = new RelationWindow(RelationScripts, elementX, elementY);
                #endregion

                #region[地区修改]
                ResetCoordinates(true, true);
                GameObject TeleportScripts = UIControls.createUIPanel(uiPanel, "410", "600", null);
                TeleportScripts.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                TeleportScripts.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, -20);

                TeleportWindow teleportWindow = new TeleportWindow(TeleportScripts, elementX, elementY);
                #endregion

                #region[江湖录修改]
                ResetCoordinates(true, true);
                GameObject CollectScripts = UIControls.createUIPanel(uiPanel, "410", "600", null);
                CollectScripts.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                CollectScripts.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, -20);

                CollectWindow collectWindow = new CollectWindow(CollectScripts, elementX, elementY);
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

                UnityGameUI.Navigation[] nav = new UnityGameUI.Navigation[]
                {
                    new UnityGameUI.Navigation("BasicScripts", "基础功能", BasicScripts, true),
                    new UnityGameUI.Navigation("CharacterScripts", "角色修改", CharacterScripts, false),
                    new UnityGameUI.Navigation("ItemScripts", "物品添加", ItemScripts, false),
                    new UnityGameUI.Navigation("TeleportScripts", "地点瞬移", TeleportScripts, false),
                    new UnityGameUI.Navigation("PlaceScripts", "地区好感度修改", PlaceScripts, false),
                    new UnityGameUI.Navigation("RelationScripts", "人物关系修改", RelationScripts, false),
                    new UnityGameUI.Navigation("CollectScripts", "江湖录修改", CollectScripts, false)
                };

                UINavigation.Initialize(nav, NavPanel);

                #endregion

                MainWindow.canvas.SetActive(MainWindow.optionToggle);
            }
        }

        // Token: 0x0600008A RID: 138 RVA: 0x0000903C File Offset: 0x0000723C
        public static GameObject AddButton(string Text, GameObject panel, UnityAction action)
        {
            string backgroundColor = "#8C9EFFFF";
            Vector3 localPosition = new Vector3((float)MainWindow.elementX, (float)MainWindow.elementY, 0f);
            MainWindow.elementX += 110;
            GameObject gameObject = UIControls.createUIButton(panel, backgroundColor, Text, action, localPosition);
            gameObject.AddComponent<Shadow>().effectColor = UIControls.HTMLString2Color("#000000FF");
            gameObject.GetComponent<Shadow>().effectDistance = new Vector2(2f, -2f);
            gameObject.GetComponentInChildren<Text>().fontSize = 14;
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 30f);
            return gameObject;
        }

        // Token: 0x0600008B RID: 139 RVA: 0x000090E4 File Offset: 0x000072E4
        public static GameObject AddButton(ref int elementX, ref int elementY, string Text, GameObject panel, UnityAction action)
        {
            string backgroundColor = "#8C9EFFFF";
            Vector3 localPosition = new Vector3((float)elementX, (float)elementY, 0f);
            elementX += 110;
            GameObject gameObject = UIControls.createUIButton(panel, backgroundColor, Text, action, localPosition);
            gameObject.AddComponent<Shadow>().effectColor = UIControls.HTMLString2Color("#000000FF");
            gameObject.GetComponent<Shadow>().effectDistance = new Vector2(2f, -2f);
            gameObject.GetComponentInChildren<Text>().fontSize = 14;
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 30f);
            return gameObject;
        }

        // Token: 0x0600008C RID: 140 RVA: 0x00009184 File Offset: 0x00007384
        public static GameObject AddToggle(string Text, int width, ConfigEntry<bool> isOn, GameObject panel, UnityAction<bool> action = null)
        {
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture(ColorUtility.ToHtmlStringRGBA(Color.white)));
            Sprite customCheckmarkSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture(ColorUtility.ToHtmlStringRGBA(Color.red)));
            GameObject gameObject = UIControls.createUIToggle(panel, bgSprite, customCheckmarkSprite);
            gameObject.GetComponentInChildren<Text>().color = Color.white;
            gameObject.GetComponentInChildren<Toggle>().isOn = isOn.Value;
            gameObject.GetComponentInChildren<Toggle>().GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 20f);
            gameObject.GetComponentInChildren<Toggle>().GetComponent<RectTransform>().localPosition = new Vector2((float)(MainWindow.elementX - 30), (float)MainWindow.elementY);
            gameObject.GetComponentInChildren<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2((float)(width - 20), 30f);
            gameObject.GetComponentInChildren<Text>().GetComponent<RectTransform>().localPosition = new Vector2((float)((width - 20) / 2 + 30), -17f);
            gameObject.GetComponentInChildren<Text>().text = Text;
            gameObject.GetComponentInChildren<Toggle>().onValueChanged.AddListener(delegate (bool state)
            {
                isOn.Value = state;
                action(state);
            });
            MainWindow.elementX += width + 10;
            return gameObject;
        }

        // Token: 0x0600008D RID: 141 RVA: 0x000092D4 File Offset: 0x000074D4
        public static GameObject AddInputField(int width, string defaultText, GameObject panel, UnityAction<string> action)
        {
            MainWindow.elementX += width / 2 - 30;
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject gameObject = UIControls.createUIInputField(panel, bgSprite, "#FFFFFFFF");
            gameObject.GetComponent<InputField>().text = defaultText;
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)MainWindow.elementX, (float)MainWindow.elementY, 0f);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2((float)width, 30f);
            gameObject.GetComponent<InputField>().onEndEdit.AddListener(action);
            MainWindow.elementX += width / 2 + 10;
            return gameObject;
        }

        // Token: 0x0600008E RID: 142 RVA: 0x00009380 File Offset: 0x00007580
        public static GameObject AddInputField(string Text, int width, string defaultText, GameObject panel, UnityAction<string> action)
        {
            MainWindow.elementX += width / 2 - 30;
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject = UIControls.createUIText(panel, bgSprite, "#FFFFFFFF");
            gameObject.GetComponent<Text>().text = Text;
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)MainWindow.elementX, (float)MainWindow.elementY, 0f);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(40f, 30f);
            gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            MainWindow.elementX += 10;
            Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject gameObject2 = UIControls.createUIInputField(panel, bgSprite2, "#FFFFFFFF");
            gameObject2.GetComponent<InputField>().text = defaultText;
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector3((float)MainWindow.elementX, (float)MainWindow.elementY, 0f);
            gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2((float)(width - 40), 30f);
            gameObject2.GetComponent<InputField>().onEndEdit.AddListener(action);
            MainWindow.elementX += width / 2 + 10;
            return gameObject2;
        }

        // Token: 0x0600008F RID: 143 RVA: 0x000094B4 File Offset: 0x000076B4
        public GameObject AddDropdown(string Text, int width, List<string> options, GameObject panel, UnityAction<int> action)
        {
            MainWindow.elementX += width / 2 - 30;
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject = UIControls.createUIText(panel, bgSprite, "#FFFFFFFF");
            gameObject.GetComponent<Text>().text = Text;
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)MainWindow.elementX, (float)MainWindow.elementY, 0f);
            gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            MainWindow.elementX += 60;
            Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            Sprite scrollbarSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));
            Sprite dropDownSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            Sprite checkmarkSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));
            Sprite customMaskSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#E65100FF"));
            Color labelColor = UIControls.HTMLString2Color("#EFEBE9FF");
            GameObject gameObject2 = UIControls.createUIDropDown(panel, bgSprite2, scrollbarSprite, dropDownSprite, checkmarkSprite, customMaskSprite, options, labelColor);
            UnityEngine.Object.DontDestroyOnLoad(gameObject2);
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector3((float)MainWindow.elementX, (float)MainWindow.elementY, 0f);
            gameObject2.GetComponent<Dropdown>().onValueChanged.AddListener(action);
            MainWindow.elementX += width / 2 + 60;
            return gameObject2;
        }

        // Token: 0x06000090 RID: 144 RVA: 0x0000960C File Offset: 0x0000780C
        public GameObject AddH3(string text, GameObject panel)
        {
            MainWindow.elementX += 40;
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject = UIControls.createUIText(panel, bgSprite, "#FFFFFFFF");
            gameObject.GetComponent<Text>().text = text;
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)MainWindow.elementX, (float)MainWindow.elementY, 0f);
            gameObject.GetComponent<Text>().fontSize = 14;
            gameObject.GetComponent<Text>().fontStyle = FontStyle.Bold;
            this.hr(0, 0);
            MainWindow.elementY += 20;
            MainWindow.elementX += 10;
            return gameObject;
        }

        // Token: 0x06000091 RID: 145 RVA: 0x000096B8 File Offset: 0x000078B8
        public static GameObject AddTitle(string Title, GameObject background)
        {
            GameObject gameObject = UIControls.createUIPanel(background, "30", (MainWindow.width - 20).ToString(), null);
            gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#2D2D30FF");
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, (float)(MainWindow.height / 2 - 30), 0f);

            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject2 = UIControls.createUIText(gameObject, bgSprite, "#FFFFFFFF");
            gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2((float)(MainWindow.width - 10), 30f);
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
            Text component = gameObject2.GetComponent<Text>();
            component.text = Title;
            component.alignment = TextAnchor.MiddleCenter;
            component.fontSize = 16;
            return gameObject2;
        }

        // Token: 0x06000092 RID: 146 RVA: 0x000097A8 File Offset: 0x000079A8
        public void hr(int offsetX = 0, int offsetY = 0)
        {
            this.ResetCoordinates(true, false);
            MainWindow.elementX += offsetX;
            MainWindow.elementY -= 50 + offsetY;
        }

        // Token: 0x06000093 RID: 147 RVA: 0x000097D0 File Offset: 0x000079D0
        public void ResetCoordinates(bool x, bool y = false)
        {
            if (x)
            {
                MainWindow.elementX = MainWindow.initialX;
            }
            if (y)
            {
                MainWindow.elementY = MainWindow.initialY;
            }
        }

        // Token: 0x0400004C RID: 76
        public static GameObject obj = null;

        // Token: 0x0400004D RID: 77
        public static MainWindow instance;

        // Token: 0x0400004E RID: 78
        public static bool initialized = false;

        // Token: 0x0400004F RID: 79
        public static bool _optionToggle = false;

        // Token: 0x04000050 RID: 80
        private static TooltipGUI toolTipComp = null;

        // Token: 0x04000051 RID: 81
        public static AssetBundle testAssetBundle = null;

        // Token: 0x04000052 RID: 82
        public static GameObject canvas = null;

        // Token: 0x04000053 RID: 83
        private static GameObject uiPanel = null;

        // Token: 0x04000054 RID: 84
        public static DragAndDrog dragAndDrog = null;

        // Token: 0x04000055 RID: 85
        public static readonly int width = Mathf.Min(Screen.width, 740);

        // Token: 0x04000056 RID: 86
        private static readonly int height = (Screen.height < 400) ? Screen.height : 450;

        // Token: 0x04000057 RID: 87
        private static GameObject ExpTextInput = null;

        // Token: 0x04000058 RID: 88
        private static GameObject PlaceTextInput = null;

        // Token: 0x04000059 RID: 89
        private static GameObject CharacterTextInput = null;

        // Token: 0x0400005A RID: 90
        private static GameObject CanWuTextInput = null;

        // Token: 0x0400005B RID: 91
        private static GameObject CanWuShuXingTextInput = null;

        private static GameObject TimeScaleTextInput = null;

        // Token: 0x0400005C RID: 92
        private static int elementX = MainWindow.initialX;

        // Token: 0x0400005D RID: 93
        private static int elementY = MainWindow.initialY;
    }
}
