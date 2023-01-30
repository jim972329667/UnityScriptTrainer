using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameUI
{
	// Token: 0x02000005 RID: 5
	public static class Extensions
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002200 File Offset: 0x00000400
		public static Color32 HexToColor(this string hexString)
		{
			string text = hexString;
			bool flag = text.IndexOf('#') != -1;
			if (flag)
			{
				text = text.Replace("#", "");
			}
			byte b = Convert.ToByte(text.Substring(0, 2), 16);
			byte b2 = Convert.ToByte(text.Substring(2, 2), 16);
			byte b3 = Convert.ToByte(text.Substring(4, 2), 16);
			bool flag2 = text.Length == 8;
			Color32 result;
			if (flag2)
			{
				byte a = Convert.ToByte(text.Substring(6, 2), 16);
				result = new Color32(b, b2, b3, a);
			}
			else
			{
				result = new Color((float)b / 255f, (float)b2 / 255f, (float)b3 / 255f);
			}
			return result;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000022C8 File Offset: 0x000004C8
		public static int ConvertToIntDef(this string input, int defaultValue)
		{
			int num;
			bool flag = int.TryParse(input, out num);
			int result;
			if (flag)
			{
				result = num;
			}
			else
			{
				result = defaultValue;
			}
			return result;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000022EC File Offset: 0x000004EC
		public static float ConvertToFloatDef(this string input, float defaultValue)
		{
			float num;
			bool flag = float.TryParse(input, out num);
			float result;
			if (flag)
			{
				result = num;
			}
			else
			{
				result = defaultValue;
			}
			return result;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002310 File Offset: 0x00000510
		public static List<string> GetSeparateSubString(this string input, int charNumber)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < input.Length; i += charNumber)
			{
				bool flag = input.Length - i > charNumber;
				if (flag)
				{
					list.Add(input.Substring(i, charNumber));
				}
				else
				{
					list.Add(input.Substring(i));
				}
			}
			return list;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002378 File Offset: 0x00000578
		public static List<string> ParseDescription(this string Description, int charnum)
		{
			string text = Description.Replace("<b>", "").Replace("</b>", "").Replace("</color>", "").Replace("<color=red", "").Replace("<color=yellow", "").Replace("<color=green", "");
			List<string> list = new List<string>();
			foreach (string text2 in text.Split(new char[]
			{
				'>'
			}))
			{
				bool flag = text2.StartsWith("<color=");
				if (flag)
				{
					list.Add(text2.ParseColor());
				}
				else
				{
					bool flag2 = text2.Length > charnum;
					if (flag2)
					{
						list.AddRange(text2.GetSeparateSubString(charnum));
					}
					else
					{
						list.Add(text2);
					}
				}
			}
			return list;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000246C File Offset: 0x0000066C
		public static string ParseColor(this string color)
		{
			return color.Replace("<color=", "").Replace(" ", "");
		}
	}
}
