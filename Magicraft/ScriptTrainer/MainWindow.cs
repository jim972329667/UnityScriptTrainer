using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityGameUI;
using UniverseLib;
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
        //private static TooltipGUI toolTipComp = null;
        public static DragAndDrog DragAndDrog=null;
        // UI
        public static GameObject canvas = null;
        private static GameObject uiPanel = null;
        public static readonly int width = Mathf.Min(Screen.width, 740);
        private static readonly int height = (Screen.height < 400) ? Screen.height : (450);

        public static GameObject PickInput = null;
        public static GameObject StackInput = null;
        public static GameObject SpawnSettingsInput = null;
        public static GameObject ToolUseInput = null;
        public static GameObject Title = null;
        private static bool checkUpdate = false;
        public static bool CheckUpdate
        {
            get 
            { 
                return checkUpdate;
            }
            set
            {
                if(value != checkUpdate)
                {
                    checkUpdate = value;
                    if (value)
                    {
                        if (!initialized)
                        {
                            Initialize();
                        }
                        Title.GetComponentInChildren<Text>().text = $"{ScriptTrainer.Instance.Info.Metadata.Name} V{ScriptTrainer.Instance.Info.Metadata.Version} by:Jim97 <color=red>需要更新</color>";
                    }
                }
            }
        }
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
        private static float time = 0f;
        public static void PauseGame(bool pause)
        {
            if (pause)
            {
                ScriptTrainer.Instance.Log("Pause Game!");
                time = Time.timeScale;
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = time;
            }
        }
        public static bool optionToggle
        {
            get => _optionToggle;
            set
            {
                if (!initialized)
                {
                    Initialize();
                }
                if (!ItemWindow.initialized)
                    ItemWindow.Instance.InitializeItem();
                _optionToggle = value;
                if (_optionToggle)
                {
                    if (CheckUpdate)
                    {
                        UIWindows.SpawnUpdateDialog("https://mod.3dmgame.com/mod/204543");
                        CheckUpdate = false;
                    }
                }
                else
                {
                    //MainController.Instance.AppServices.InputService.Config.Enable();
                    DragAndDrog.isMouseDrag = false;
                }
                PauseGame(_optionToggle);
            }
        }

        #endregion

        internal static GameObject Create(string name)
        {
            obj = new GameObject(name);
            DontDestroyOnLoad(obj);

            var component = new MainWindow();

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
                Title = UIWindows.AddTitle(uiPanel, $"{ScriptTrainer.Instance.Info.Metadata.Name} V{ScriptTrainer.Instance.Info.Metadata.Version} by:Jim97");

                DragAndDrog = Title.AddComponent<DragAndDrog>();
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
                    UIWindows.AddButton(ref point, "解锁/恢复 所有图鉴", BasicScripts, () =>
                    {
                        Scripts.UnlockGallery();
                    });
                    UIWindows.AddButton(ref point, "解锁/恢复 所有NPC", BasicScripts, () =>
                    {
                        Scripts.UnlockNPC();
                    });
                }
                UIWindows.Hr(ref point, BasicScripts);
                UIWindows.AddH3(ref point, "战斗功能：", BasicScripts);
                {
                    UIWindows.AddButton(ref point, "增加金币", BasicScripts, () =>
                    {
                        UIWindows.SpawnInputDialog("添加", "金币", "100", (string text) =>
                        {
                            Scripts.AddCoin(text.ConvertToIntDef(100));
                        });
                    });
                    UIWindows.AddButton(ref point, "增加钥匙", BasicScripts, () =>
                    {
                        UIWindows.SpawnInputDialog("添加", "钥匙", "100", (string text) =>
                        {
                            Scripts.AddKey(text.ConvertToIntDef(100));
                        });
                    });
                    UIWindows.AddButton(ref point, "增加水晶", BasicScripts, () =>
                    {
                        UIWindows.SpawnInputDialog("添加", "水晶", "100", (string text) =>
                        {
                            Scripts.AddCrystal(text.ConvertToIntDef(100));
                        });
                    });
                    UIWindows.AddButton(ref point, "增加旧神之血", BasicScripts, () =>
                    {
                        UIWindows.SpawnInputDialog("添加", "旧神之血", "100", (string text) =>
                        {
                            Scripts.AddBlood(text.ConvertToIntDef(100));
                        });
                    });
                    UIWindows.AddButton(ref point, "增加混沌核心", BasicScripts, () =>
                    {
                        UIWindows.SpawnInputDialog("添加", "混沌核心", "5", (string text) =>
                        {
                            ScriptTrainer.WriteGameCmd($"core {text.ConvertToIntDef(5)}");
                        });
                    });
                    UIWindows.Hr(ref point, BasicScripts);
                    UIWindows.AddButton(ref point, "开启/关闭 神仙模式", BasicScripts, () =>
                    {
                        Scripts.God();
                    });
                    UIWindows.AddButton(ref point, "恢复状态", BasicScripts, () =>
                    {
                        Scripts.FullHp();
                    });
                    UIWindows.AddButton(ref point, "移除诅咒", BasicScripts, () =>
                    {
                        Scripts.RemoveCurse();
                    });
                }
                UIWindows.Hr(ref point, BasicScripts);
                UIWindows.AddH3(ref point, "人物属性：", BasicScripts);
                {
                    UIWindows.AddButton(ref point, "增加生命上限", BasicScripts, () =>
                    {
                        UIWindows.SpawnInputDialog("添加", "生命上限", "100", (string text) =>
                        {
                            ScriptTrainer.PlayerManagerInvoke("ChangeHPMax", new object[] { text.ConvertToIntDef(100) ,null});
                            Scripts.FullHp();
                        });
                    });
                    UIWindows.AddButton(ref point, "增加移动速度", BasicScripts, () =>
                    {
                        UIWindows.SpawnInputDialog("添加", "移动速度", "1", (string text) =>
                        {
                            ScriptTrainer.PlayerManagerInvoke("ChangeMoveSpeed", new object[] { text.ConvertToFloatDef(1f) });
                        });
                    });
                    UIWindows.AddButton(ref point, "增加魔力上限", BasicScripts, () =>
                    {
                        UIWindows.SpawnInputDialog("添加", "魔力上限", "100", (string text) =>
                        {
                            ScriptTrainer.PlayerManagerInvoke("ChangeMPMax", new object[] { text.ConvertToIntDef(100), null });
                            ScriptTrainer.Instance.Log(ScriptTrainer.PlayerManagerInvoke("get_SelectedWand", null));
                        });
                    });
                    UIWindows.AddButton(ref point, "增加魔力回复", BasicScripts, () =>
                    {
                        UIWindows.SpawnInputDialog("添加", "魔力回复", "5", (string text) =>
                        {
                            ScriptTrainer.PlayerManagerInvoke("ChangeMPRecovery", new object[] { text.ConvertToIntDef(5) });
                        });
                    });
                    UIWindows.AddButton(ref point, "增加护甲", BasicScripts, () =>
                    {
                        UIWindows.SpawnInputDialog("添加", "护甲", "100", (string text) =>
                        {
                            ScriptTrainer.PlayerManagerInvoke("ChangeShield", new object[] { text.ConvertToIntDef(100), null });
                        });
                    });
                    UIWindows.AddButton(ref point, "增加临时护甲", BasicScripts, () =>
                    {
                        UIWindows.SpawnInputDialog("添加", "临时护甲", "100", (string text) =>
                        {
                            ScriptTrainer.PlayerManagerInvoke("ChangeShieldTemp", new object[] { text.ConvertToIntDef(100), null });
                        });
                    });
                    UIWindows.Hr(ref point, BasicScripts);
                }
                #endregion

                #region[获取物品]
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
