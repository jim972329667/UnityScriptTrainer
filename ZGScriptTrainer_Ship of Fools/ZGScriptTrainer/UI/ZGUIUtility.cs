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
    }
}
