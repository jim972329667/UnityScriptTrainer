using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI;
using UnityEngine;
using UniverseLib;
using UnityEngine.Events;
using UniverseLib.UI.Widgets;
using BepInEx.Unity.IL2CPP.UnityEngine;
using ZGScriptTrainer.ItemSpwan;


namespace ZGScriptTrainer.UI
{
    public static class ZGUIUtility
    {
        internal static Vector2 largeElementSize = new Vector2(100f, 30f);

        internal static Vector2 smallElementSize = new Vector2(25f, 25f);

        internal static Color defaultTextColor = Color.white;

        public static int ConvertToIntDef(this string input, int defaultValue)
        {
            int result;
            if (int.TryParse(input, out result))
            {
                return result;
            }
            return defaultValue;
        }
        public static float ConvertToFloatDef(this string input, float defaultValue)
        {
            float result;
            if (float.TryParse(input, out result))
            {
                return result;
            }
            return defaultValue;
        }
        internal static void SetDefaultTextValues(Text text)
        {
            text.color = defaultTextColor;
            text.font = UniversalUI.DefaultFont;
            text.fontSize = ZGScriptTrainer.FontSize.Value;
        }
        public static Color32 HexToColor(this string hexString)
        {
            string tmp = hexString;

            if (tmp.IndexOf('#') != -1)
                tmp = tmp.Replace("#", "");

            byte r = 0, g = 0, b = 0, a = 0;

            r = Convert.ToByte(tmp.Substring(0, 2), 16);
            g = Convert.ToByte(tmp.Substring(2, 2), 16);
            b = Convert.ToByte(tmp.Substring(4, 2), 16);
            if (tmp.Length == 8)
                a = Convert.ToByte(tmp.Substring(6, 2), 16);
            else
            {
                return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f);
            }
            return new Color32(r, g, b, a);
        }
        public static Color32 HTMLString2Color(this string htmlcolorstring)
        {
            Color32 color = htmlcolorstring.HexToColor();

            return color;
        }
        public static Sprite createSpriteFrmTexture(this Texture2D SpriteTexture)
        {
            // Create a new Sprite from Texture
            Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), 100.0f, 0, SpriteMeshType.Tight);

            return NewSprite;
        }
        public static Texture2D createDefaultTexture(this string htmlcolorstring)
        {
            Color32 color = HTMLString2Color(htmlcolorstring);

            // Make a new sprite from a texture
            Texture2D SpriteTexture = new(1, 1);
            SpriteTexture.SetPixel(0, 0, color);
            SpriteTexture.Apply();

            return SpriteTexture;
        }
        public static LayoutElement SetLayoutElement(this ButtonRef button, int? minWidth = null, int? minHeight = null, int? flexibleWidth = null, int? flexibleHeight = null, int? preferredWidth = null, int? preferredHeight = null, bool? ignoreLayout = null)
        {
            button.ButtonText.fontSize = ZGScriptTrainer.FontSize.Value;
            if (!preferredWidth.HasValue)
                return UIFactory.SetLayoutElement(button.Component.gameObject, minWidth, minHeight, flexibleWidth, flexibleHeight, (int)button.Component.gameObject.GetComponentInChildren<Text>().preferredWidth + 10, preferredHeight, ignoreLayout);
            else
                return UIFactory.SetLayoutElement(button.Component.gameObject, minWidth, minHeight, flexibleWidth, flexibleHeight, preferredWidth, preferredHeight, ignoreLayout);
        }
        public static LayoutElement SetLayoutElement(this Text text, int? minWidth = null, int? minHeight = null, int? flexibleWidth = null, int? flexibleHeight = null, int? preferredWidth = null, int? preferredHeight = null, bool? ignoreLayout = null)
        {
            if (!preferredWidth.HasValue)
                return UIFactory.SetLayoutElement(text.gameObject, minWidth, minHeight, flexibleWidth, flexibleHeight, (int)text.preferredWidth + 10, preferredHeight, ignoreLayout);
            else
                return UIFactory.SetLayoutElement(text.gameObject, minWidth, minHeight, flexibleWidth, flexibleHeight, preferredWidth, preferredHeight, ignoreLayout);
        }
        public static LayoutElement SetLayoutElement(this InputFieldRef input, int? minWidth = null, int? minHeight = null, int? flexibleWidth = null, int? flexibleHeight = null, int? preferredWidth = null, int? preferredHeight = null, bool? ignoreLayout = null)
        {
            input.PlaceholderText.fontSize = ZGScriptTrainer.FontSize.Value;
            return UIFactory.SetLayoutElement(input.GameObject, minWidth, minHeight, flexibleWidth, flexibleHeight, preferredWidth, preferredHeight, ignoreLayout);
        }
        public static GameObject SetLayoutElement(this GameObject gameObject, int? minWidth = null, int? minHeight = null, int? flexibleWidth = null, int? flexibleHeight = null, int? preferredWidth = null, int? preferredHeight = null, bool? ignoreLayout = null)
        {
            UIFactory.SetLayoutElement(gameObject, minWidth, minHeight, flexibleWidth, flexibleHeight, preferredWidth, preferredHeight, ignoreLayout);
            return gameObject;
        }



        public static GameObject CreateSplitPanel(this GameObject parent, Color bgColor = default(Color), int height = 5)
        {
            GameObject gameObject = UIFactory.CreateUIObject($"{parent.name ?? "ZGScriptTrainer"}_SplitPanel_{DateTime.UtcNow.Ticks}", parent);
            RectTransform component = gameObject.GetComponent<RectTransform>();
            component.anchorMin = Vector2.zero;
            component.anchorMax = Vector2.one;
            component.anchoredPosition = Vector2.zero;
            component.sizeDelta = Vector2.zero;
            var image= gameObject.AddComponent<Image>();
            image.type = Image.Type.Filled;
            image.color = bgColor == default ? new Color(0.07f, 0.07f, 0.07f) : bgColor;
            gameObject.AddComponent<RectMask2D>();
            gameObject.SetLayoutElement(minHeight: height, flexibleWidth: 9999);
            return gameObject;
        }
        public static GameObject CreateInputEditButton(this GameObject parent, string lableText, string defaultInput, string buttonText, Action<string> action)
        {
            string tmpname = $"{parent.name ?? "ZGScriptTrainer"}_InputEditButton_{DateTime.UtcNow.Ticks}";
            var tmp = UIFactory.CreateHorizontalGroup(parent, tmpname, false, false, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);

            var tmplabel = UIFactory.CreateLabel(tmp, $"{tmpname}_lable", lableText, TextAnchor.MiddleLeft, default, true, ZGScriptTrainer.FontSize.Value);
            tmplabel.SetLayoutElement(minWidth: 20, minHeight: 40, flexibleWidth: 300);

            var tmpInput = UIFactory.CreateInputField(tmp, $"{tmpname}_Input", defaultInput);
            tmpInput.SetLayoutElement(minWidth: 40, minHeight: 40, preferredWidth: 40);

            var tmpbutton = tmp.CreateButton(buttonText, () =>
            {
                action.Invoke(tmpInput.Text);
            });
            tmpbutton.SetLayoutElement(minWidth: 40, minHeight: 30, preferredWidth: 40);

            var max = tmplabel.GetComponent<RectTransform>().sizeDelta.x + tmpInput.GameObject.GetComponent<RectTransform>().sizeDelta.x + tmpbutton.GameObject.GetComponent<RectTransform>().sizeDelta.x;

            return tmp.SetLayoutElement(minHeight: 40, minWidth: 150);
        }
        public static GameObject CreateToggle(this GameObject parent, string text, Action<bool> action, Color bgColor = default(Color), int checkWidth = 20, int checkHeight = 20, bool isOn = false)
        {
            var tmp = UIFactory.CreateToggle(parent, $"{parent.name ?? "ZGScriptTrainer"}_{text}_{DateTime.UtcNow.Ticks}", out var tmpToggle, out Text toggleText, bgColor == default ? Color.white : bgColor);
            tmpToggle.onValueChanged.AddListener((UnityAction<bool>)action);
            tmpToggle.isOn = isOn;
            toggleText.text = text;
            toggleText.fontSize = ZGScriptTrainer.FontSize.Value;
            return tmp;
        }
        public static ButtonRef CreateButton(this GameObject parent, string text, Action onclick)
        {
            var Btn0 = UIFactory.CreateButton(parent, $"{parent.name ?? "ZGScriptTrainer"}_{text}_{DateTime.UtcNow.Ticks}", text);
            Btn0.OnClick = onclick;
            return Btn0;
        }
        public static GameObject CreateGridScrollView(this GameObject parent, string name, Vector2 cellSize, Vector2 spacing, out GameObject content, out AutoSliderScrollbar autoScrollbar,
            Color color = default)
        {
            GameObject mainObj = UIFactory.CreateUIObject(name, parent);
            RectTransform mainRect = mainObj.GetComponent<RectTransform>();
            mainRect.anchorMin = Vector2.zero;
            mainRect.anchorMax = Vector2.one;
            Image mainImage = mainObj.AddComponent<Image>();
            mainImage.type = Image.Type.Filled;
            mainImage.color = (color == default) ? new Color(0.3f, 0.3f, 0.3f, 1f) : color;

            UIFactory.SetLayoutElement(mainObj, flexibleHeight: 9999, flexibleWidth: 9999);

            GameObject viewportObj = UIFactory.CreateUIObject("Viewport", mainObj);
            RectTransform viewportRect = viewportObj.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.pivot = new Vector2(0.0f, 1.0f);
            viewportRect.offsetMax = new Vector2(-28, 0);
            viewportObj.AddComponent<Image>().color = new(0.1f, 0.1f, 0.1f, 1);
            viewportObj.AddComponent<Mask>().showMaskGraphic = false;

            content = UIFactory.CreateGridGroup(viewportObj, "Content", cellSize, spacing);
            //var grid = content.GetComponent<GridLayoutGroup>();
            //grid.padding.top = 5;
            //grid.padding.left = 5;
            //grid.padding.right = 5;
            //grid.padding.bottom = 5;
            //UIFactory.SetLayoutGroup<VerticalLayoutGroup>(content, true, false, true, true, childAlignment: TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(content, flexibleHeight: 9999);
            RectTransform contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.pivot = new Vector2(0.0f, 1.0f);
            content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Slider

            GameObject scrollBarObj = UIFactory.CreateUIObject("AutoSliderScrollbar", mainObj);
            RectTransform scrollBarRect = scrollBarObj.GetComponent<RectTransform>();
            scrollBarRect.anchorMin = new Vector2(1, 0);
            scrollBarRect.anchorMax = Vector2.one;
            scrollBarRect.offsetMin = new Vector2(-25, 0);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(scrollBarObj, false, true, true, true);
            scrollBarObj.AddComponent<Image>().color = new(0.1f, 0.1f, 0.1f, 1);
            scrollBarObj.AddComponent<Mask>().showMaskGraphic = false;

            GameObject hiddenBar = UIFactory.CreateScrollbar(scrollBarObj, "HiddenScrollviewScroller", out Scrollbar hiddenScrollbar);
            hiddenScrollbar.SetDirection(Scrollbar.Direction.BottomToTop, true);

            for (int i = 0; i < hiddenBar.transform.childCount; i++)
            {
                Transform child = hiddenBar.transform.GetChild(i);
                child.gameObject.SetActive(false);
            }

            UIFactory.CreateSliderScrollbar(scrollBarObj, out Slider scrollSlider);
            //autoScrollbar = null;

            autoScrollbar = new AutoSliderScrollbar(hiddenScrollbar, scrollSlider, contentRect, viewportRect);
            

            // Set up the ScrollRect component

            ScrollRect scrollRect = mainObj.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.verticalScrollbar = hiddenScrollbar;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 35;
            scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
            scrollRect.viewport = viewportRect;
            scrollRect.content = contentRect;

            return mainObj;
        }
        
    }
}
