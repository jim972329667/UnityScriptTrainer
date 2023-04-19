using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

namespace ScriptTrainer.UI
{
    public static class UIObject
    {
        #region[复选框]
        public static GameObject AddToggle(this GameObject panel, ref Vector2 Position, string Text, int width,  UnityAction<bool> action)
        {
            //计算x轴偏移
            Position.x += width / 2;

            Sprite toggleBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#FFFFFFFF"));
            Sprite toggleSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#18FFFFFF"));
            GameObject uiToggle = UIControls.createUIToggle(panel, toggleBgSprite, toggleSprite);
            uiToggle.GetComponentInChildren<Text>().color = Color.white;
            uiToggle.GetComponentInChildren<Toggle>().isOn = false;
            uiToggle.GetComponent<RectTransform>().localPosition = Position;

            uiToggle.GetComponentInChildren<Text>().text = Text;
            uiToggle.GetComponentInChildren<Toggle>().onValueChanged.AddListener(action);

            uiToggle.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 10, 20);
            Position.x += width / 2 + 10;

            return uiToggle;
        }
        public static GameObject AddToggle(this GameObject panel, ref Vector2 Position, string Text, int width, Action<bool> action)
        {
            //计算x轴偏移
            Position.x += width / 2;

            Sprite toggleBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#FFFFFFFF"));
            Sprite toggleSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#18FFFFFF"));
            GameObject uiToggle = UIControls.createUIToggle(panel, toggleBgSprite, toggleSprite);
            uiToggle.GetComponentInChildren<Text>().color = Color.white;
            uiToggle.GetComponentInChildren<Toggle>().isOn = false;
            uiToggle.GetComponent<RectTransform>().localPosition = Position;

            uiToggle.GetComponentInChildren<Text>().text = Text;
            uiToggle.GetComponentInChildren<Toggle>().onValueChanged.AddListener((UnityAction<bool>)action);
            uiToggle.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 10, 20);

            Position.x += width / 2 + 10;

            return uiToggle;
        }
        #endregion

        #region[按钮]
        public static GameObject AddButton(this GameObject panel, ref Vector2 Position, string Text, Action action,int width = 110)
        {
            string backgroundColor = "#8C9EFFFF";

            Position.x += width/2;

            GameObject button = UIControls.createUIButton(panel, backgroundColor, Text, action, Position);

            // 按钮样式
            button.AddComponent<Shadow>().effectColor = UIControls.HTMLString2Color("#000000FF");// 添加阴影
            button.GetComponent<Shadow>().effectDistance = new Vector2(2, -2);// 设置阴影偏移
            button.GetComponentInChildren<Text>().fontSize = 14;     // 设置字体大小           
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 10, 30);// 设置按钮大小
            Position.x += width / 2 + 10;

            return button;
        }


        #endregion

        #region[输入框]
        public static GameObject AddInputField(this GameObject panel, ref Vector2 Position, string Text, int width, string defaultText,  UnityAction<string> action)
        {
            // label
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(panel, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<Text>().text = Text;
            uiText.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

            Text text1 = uiText.GetComponent<Text>();
            uiText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30);
            uiText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, text1.preferredWidth);

            // 坐标偏移
            Position.x += (text1.preferredWidth + 10) / 2;//字体开始，字体两边间隔5

            uiText.GetComponent<RectTransform>().localPosition = Position;
            // 坐标偏移
            Position.x += (text1.preferredWidth + 10) / 2;//字体结束


            // 坐标偏移
            Position.x += (width - text1.preferredWidth - 15) / 2;//输入框开始

            // 输入框
            Sprite inputFieldSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject uiInputField = UIControls.createUIInputField(panel, inputFieldSprite, "#FFFFFFFF");
            uiInputField.GetComponent<InputField>().text = defaultText;
            uiInputField.GetComponent<RectTransform>().localPosition = Position;
            uiInputField.GetComponent<RectTransform>().sizeDelta = new Vector2(width - text1.preferredWidth - 15, 30);

            // 文本框失去焦点时触发方法
            uiInputField.GetComponent<InputField>().onEndEdit.AddListener(action);

            //输入框结束
            Position.x += (width - text1.preferredWidth - 15) / 2 + 15;//与下一个控件间隔10
            return uiInputField;
        }

        public static GameObject AddInputField(this GameObject panel, ref Vector2 Position, string Text, int width, string defaultText, Action<string> action)
        {
            // label
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(panel, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<Text>().text = Text;
            uiText.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

            Text text1 = uiText.GetComponent<Text>();
            uiText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30);
            uiText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, text1.preferredWidth);

            // 坐标偏移
            Position.x += (text1.preferredWidth + 10) / 2;//字体开始，字体两边间隔5

            uiText.GetComponent<RectTransform>().localPosition = Position;
            // 坐标偏移
            Position.x += (text1.preferredWidth + 10) / 2;//字体结束


            // 坐标偏移
            Position.x += (width - text1.preferredWidth - 15) / 2;//输入框开始

            // 输入框
            Sprite inputFieldSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject uiInputField = UIControls.createUIInputField(panel, inputFieldSprite, "#FFFFFFFF");
            uiInputField.GetComponent<InputField>().text = defaultText;
            uiInputField.GetComponent<RectTransform>().localPosition = Position;
            uiInputField.GetComponent<RectTransform>().sizeDelta = new Vector2(width - text1.preferredWidth - 15, 30);

            // 文本框失去焦点时触发方法
            uiInputField.GetComponent<InputField>().onEndEdit.AddListener((UnityAction<string>)action);

            //输入框结束
            Position.x += (width - text1.preferredWidth - 15) / 2 + 15;//与下一个控件间隔10
            return uiInputField;
        }
        #endregion

        #region[下拉框]
        public static GameObject AddDropdown(this GameObject panel, ref Vector2 Position, string Text, int width, List<string> options, UnityAction<int> action)
        {
            // label
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(panel, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<Text>().text = Text;
            uiText.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

            Text text1 = uiText.GetComponent<Text>();
            uiText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30);
            uiText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, text1.preferredWidth);
            // 坐标偏移
            Position.x += (text1.preferredWidth + 10) / 2;//字体开始，字体两边间隔5
            uiText.GetComponent<RectTransform>().localPosition = Position;
            // 坐标偏移
            Position.x += (text1.preferredWidth + 10) / 2;//字体结束


            // 坐标偏移
            Position.x += (width - text1.preferredWidth - 15) / 2;//下拉框开始
            // 创建下拉框
            Sprite dropdownBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));      // 背景颜色
            Sprite dropdownScrollbarSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));   // 滚动条颜色 (如果有的话
            Sprite dropdownDropDownSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));    // 框右侧小点的颜色
            Sprite dropdownCheckmarkSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));   // 选中时的颜色
            Sprite dropdownMaskSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#E65100FF"));        // 不知道是哪的颜色
            Color LabelColor = UIControls.HTMLString2Color("#EFEBE9FF");
            GameObject uiDropDown = UIControls.createUIDropDown(panel, dropdownBgSprite, dropdownScrollbarSprite, dropdownDropDownSprite, dropdownCheckmarkSprite, dropdownMaskSprite, options, LabelColor);
            UnityEngine.Object.DontDestroyOnLoad(uiDropDown);
            uiDropDown.GetComponent<RectTransform>().localPosition = Position;
            uiDropDown.GetComponent<RectTransform>().sizeDelta = new Vector2(width - text1.preferredWidth - 15, 30);
            // 下拉框选中时触发方法
            uiDropDown.GetComponent<Dropdown>().onValueChanged.AddListener(action);

            //下拉框结束
            Position.x += (width - text1.preferredWidth - 15) / 2 + 15;//与下一个控件间隔10
            return uiDropDown;
        }
        #endregion

        #region[小标题]
        public static GameObject AddH3(this GameObject panel, ref Vector2 Position, string text)
        {
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(panel, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<Text>().text = text;
            // 设置字体样式为h3小标题
            uiText.GetComponent<Text>().fontSize = 14;
            uiText.GetComponent<Text>().fontStyle = FontStyle.Bold;
            Text text1 = uiText.GetComponent<Text>();
            uiText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30);
            uiText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, text1.preferredWidth);

            Position.x += (text1.preferredWidth) / 2;//字体开始，字体两边间隔5
            uiText.GetComponent<RectTransform>().localPosition = Position;

            panel.hr(ref Position);

            Position.y += 20;
            return uiText;
        }
        #endregion

        #region[换行]
        public static void hr(this GameObject panel, ref Vector2 Position,   int offsetX = 0, int offsetY = 0)
        {
            Vector2 temp = panel.GetComponent<RectTransform>().sizeDelta;
            Position.x = -temp.x / 2 + 10;
            

            Position.x += offsetX;
            Position.y -= 50 + offsetY;
        }
        #endregion

    }
}
