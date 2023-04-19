using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UnityGameUI
{
    internal delegate bool d_LoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
    internal delegate bool parseHTMLString(IntPtr HTMLString, IntPtr result);

    public static class Extensions
    {
        // Load Image ICall

        // Convert Hex string to Color32
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
        public static Color32 ToUnityEngineColor(this System.Drawing.Color color)
        {
            Color32 value = new Color32
            {
                r = color.R,
                g = color.G,
                b = color.B,
                a = color.A
            };
            return value;
        }
        public static string ColorToHtml(this System.Drawing.Color color)
        {
            return System.Drawing.ColorTranslator.ToHtml(color);
        }
        public static string TextWithColor(this string text, System.Drawing.Color color)
        {
            return $"<color={color.ColorToHtml()}>{text}</color>";
        }
        public static string TextWithColor(this string text, string color)
        {
            return $"<color={color}>{text}</color>";
        }

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
        public static List<string> GetSeparateSubString(this string input, int charNumber)
        {
            List<string> arrlist = new List<string>();
            string tempStr = input;
            for (int i = 0; i < tempStr.Length; i += charNumber)
            {
                if ((tempStr.Length - i) > charNumber)//如果是，就截取
                {
                    arrlist.Add(tempStr.Substring(i, charNumber));
                }
                else
                {
                    arrlist.Add(tempStr.Substring(i));//如果不是，就截取最后剩下的那部分
                }
            }
            return arrlist;
        }

        public static List<string> ParseDescription(this string Description, int charnum)
        {
            string tmp = Description.Replace("<b>", "").Replace("</b>", "").Replace("</color>", "").Replace("<color=red","").Replace("<color=yellow", "").Replace("<color=green", "");
            List<string> list = new List<string>();

            foreach (string line in tmp.Split('>'))
            {
                if (line.StartsWith("<color="))
                {
                    list.Add(line.ParseColor());
                }
                else
                {
                    if (line.Length > charnum)
                    {
                        list.AddRange(line.GetSeparateSubString(charnum));
                    }
                    else
                        list.Add(line);
                }
            }

            return list;
        }
        public static string ParseColor(this string color)
        {
            return color.Replace("<color=", "").Replace(" ", "");
        }
    }
}
