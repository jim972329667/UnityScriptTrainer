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
using ZGScriptTrainer.UI.Models;

namespace ZGScriptTrainer.UI
{
    public static class ZGUIUtility
    {
        internal static Vector2 largeElementSize = new Vector2(100f, 30f);

        internal static Vector2 smallElementSize = new Vector2(25f, 25f);

        internal static Color defaultTextColor = Color.white;
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
            text.fontSize = ZGScriptTrainer.FontSize.Value;
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

        public static InputButton CreateInputButton(GameObject parent, string name, string placeHolderText, string buttonText, Color? normalColor = null)
        {
            var input = UIFactory.CreateInputField(parent, name + "_Input", placeHolderText);
            var button = UIFactory.CreateButton(parent, name + "_Button", buttonText, normalColor);
            return new InputButton(button, input);
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
