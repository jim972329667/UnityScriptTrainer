using ParadoxNotion.Serialization.FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ScriptTrainer.UI
{
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
        public static string PraseColor(this string text)
        {
            string temp = text.Replace(" ","");
            string low = temp.ToLower();
            List<char> chars = new List<char>();
            bool start = false;
            for(int i = 0; i < temp.Length; i++)
            {
                if (temp[i] == '<')
                {
                    if(i+7 < temp.Length)
                    {
                        if(low[i+1]=='c' && low[i+2]=='o' && low[i + 3] == 'l' && low[i + 4] == 'o' && low[i + 5] == 'r' && low[i + 6] == '=')
                        {
                            start = true;
                            i += 7;
                        }
                    }
                }
                if (start)
                {
                    if (temp[i] != '>')
                        chars.Add(temp[i]);
                    else
                        break;
                }
            }
            return new string(chars.ToArray());
        }
        public static string GetSeparateString(this string input, int charNumber)
        {
            string arrlist = string.Empty;
            string tempStr = input;

            List<string> Step1 = new List<string>();
            List<string> Step1Color = new List<string>();

            while (tempStr.Contains("<color="))
            {
                int startindex = tempStr.IndexOf("<color=");
                if (startindex > 0)
                {
                    Step1.Add(tempStr.Substring(0, startindex));
                    Step1Color.Add("#FFFFFF");
                }
                else if (startindex == -1)
                    break;

                int endindex = tempStr.IndexOf("</color>");
                ////获取字体颜色
                //if (tempStr[startindex + 14] == '>')
                //{
                //    Step1Color.Add(tempStr.Substring(startindex + 7, 7));
                //    startText = startindex + 15;
                //}
                //else if (tempStr[startindex + 16] == '>')
                //{
                //    Step1Color.Add(tempStr.Substring(startindex + 7, 9));
                //    startText = startindex + 17;
                //}
                //else
                //    break;
                string color = tempStr.Substring(startindex, endindex - startindex).PraseColor();
                if(color.Length > 0)
                    Step1Color.Add(color);
                else
                    break;
                //获取字体
                
                int startText = startindex + color.Length + 8;
                Step1.Add(tempStr.Substring(startText, endindex - startText));

                tempStr = tempStr.Substring(endindex + 8, tempStr.Length - endindex - 8);
            }
            if (tempStr.Length > 0)
            {
                Step1.Add(tempStr);
                Step1Color.Add("#FFFFFF");
            }

            List<string> Step2 = new List<string>();
            List<string> Step2Color = new List<string>();
            for (int i = 0; i < Step1.Count; i++)
            {
                string[] line = Step1[i].Split(new char[] { '\n', '\r' });
                foreach (string str2 in line)
                {
                    string str3 = str2.Trim();
                    if (str3.Length > 0)
                    {
                        if (str3.Length > charNumber)
                        {
                            for (int j = 0; j < str3.Length; j += charNumber)
                            {
                                if ((str3.Length - j) > charNumber)
                                {
                                    Step2.Add(str3.Substring(j, charNumber));
                                    Step2Color.Add(Step1Color[i]);
                                }
                                else
                                {
                                    Step2.Add(str3.Substring(j));
                                    Step2Color.Add(Step1Color[i]);
                                }
                            }
                        }
                        else
                        {
                            Step2.Add(str2);
                            Step2Color.Add(Step1Color[i]);
                        }
                    }
                }
            }

            for (int i = 0; i < Step2.Count; i++)
            {
                if (i == Step2.Count - 1)
                {
                    arrlist += Step2[i].GetColorText(Step2Color[i]);
                }
                else
                {
                    arrlist += Step2[i].GetColorText(Step2Color[i]) + '\n';
                }
            }

            return arrlist;
        }
        public static string GetColorText(this string text, string color)
        {
            if (color.ToUpper() == "#FFFFFF" || color.ToUpper() == "#FFFFFFFF")
                return text;
            else
                return $"<color={color}>{text}</color>";
        }
    }
}
