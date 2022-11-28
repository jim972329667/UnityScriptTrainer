
using LayerLab;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityGameUI;
using Navigation = UnityGameUI.Navigation;
using Object = UnityEngine.Object;

namespace ScriptTrainer
{
    public class MainWindow : MonoBehaviour
    {
        #region[声明]
        // Trainer Base
        public static GameObject obj = null;
        public static MainWindow instance;
        public static bool initialized = false;
        public static bool _optionToggle = false;
        private static TooltipGUI toolTipComp = null;

        // UI
        public static AssetBundle testAssetBundle = null;
        public static GameObject canvas = null;
        private static GameObject uiPanel = null;
        private static readonly int width = Mathf.Min(Screen.width, 740);
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
                Object.DontDestroyOnLoad(canvas);

                // 设置背景
                GameObject background = UIControls.createUIPanel(canvas, (height + 40).ToString(), (width + 40).ToString(), null);
                background.GetComponent<Image>().color = UIControls.HTMLString2Color("#2D2D30FF");

                // 将面板添加到画布, 请参阅 createUIPanel 了解我们将高度/宽度作为字符串传递的原因
                uiPanel = UIControls.createUIPanel(canvas, height.ToString(), width.ToString(), null);
                // 设置背景颜色
                uiPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");

                // 这就是我们将如何挂钩鼠标事件以进行窗口拖动
                //EventTrigger comp1 = background.AddComponent<EventTrigger>();
                //WindowDragHandler comp2 = background.AddComponent<WindowDragHandler>();


                #region[面板元素]


                #region[创建标题 和 关闭按钮]
                AddTitle("【奇怪的RPG】内置修改器 By:Jim97");

                GameObject closeButton = UIControls.createUIButton(uiPanel, "#B71C1CFF", "X", () =>
                {
                    optionToggle = false;
                    canvas.SetActive(optionToggle);
                }, new Vector3(width / 2 + 10, height / 2 + 10, 0));
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
                    AddButton("金币倍率", BasicScripts, () =>
                    {
                        Scripts.AddMoney();
                    });
                    AddButton("添加死亡次数", BasicScripts, () =>
                    {
                        Scripts.AddDeadNum();
                    });
                    AddButton("切歌", BasicScripts, () =>
                    {
                        Scripts.ChangeAnthorMusic();
                    });
                    hr();
                    AddToggle("无需材料购买", 150, BasicScripts, (bool state) =>
                    {
                        Scripts.RemoveNeedMaterials(state);
                    });
                    AddToggle("无限CD", 150, BasicScripts, (bool state) =>
                    {
                        Scripts.SkillCd(state);
                    });
                    AddToggle("无敌", 150, BasicScripts, (bool state) =>
                    {
                        Scripts.WuDi(state);
                    });
                    AddToggle("自动拾取掉落物", 150, BasicScripts, (bool state) =>
                    {
                        Scripts.AutoPickItem(state);
                    });
                    hr(10);
                }

                #endregion

                #region[获取物品]
                ResetCoordinates(true, true);
                GameObject ItemScripts = UIControls.createUIPanel(uiPanel, "410", "600", null);
                ItemScripts.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                ItemScripts.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, -20);

                ItemWindow itemWindow = new ItemWindow(ItemScripts, elementX, elementY);


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
                    new Navigation("ItemScripts", "随行商店", ItemScripts, false),
                };

                UINavigation.Initialize(nav, NavPanel);

                #endregion

                //log.LogMessage("Complete!");
                canvas.SetActive(optionToggle);
                Debug.Log("初始化完成!");
            }
        }

        #region[添加组件]
        public static GameObject AddButton(string Text, GameObject panel, UnityAction action)
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
        public static GameObject AddTitle(string Title)
        {
            GameObject TitleBackground = UIControls.createUIPanel(canvas, "30", (width - 20).ToString(), null);
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
