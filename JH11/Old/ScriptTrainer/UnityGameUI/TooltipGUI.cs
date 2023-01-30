using System;
using UnityEngine;

namespace UnityGameUI
{
	// Token: 0x02000006 RID: 6
	internal class TooltipGUI : MonoBehaviour
	{
		// Token: 0x06000013 RID: 19 RVA: 0x0000249D File Offset: 0x0000069D
		public TooltipGUI()
		{
			TooltipGUI.instance = this;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000024B0 File Offset: 0x000006B0
		public void OnGUI()
		{
			bool flag = !TooltipGUI.fired;
			if (flag)
			{
				TooltipGUI.fired = true;
			}
			bool flag2 = TooltipGUI.Tooltip != "" && TooltipGUI.EnableTooltip;
			if (flag2)
			{
				GUI.backgroundColor = Color.black;
				GUIContent content = new GUIContent(TooltipGUI.Tooltip);
				TooltipGUI.tooltipStyle = new GUIStyle(GUI.skin.box);
				TooltipGUI.tooltipStyle.normal.textColor = Color.white;
				float x = TooltipGUI.tooltipStyle.CalcSize(content).x;
				float y = TooltipGUI.tooltipStyle.CalcSize(content).y;
				Vector3 mousePosition = Input.mousePosition;
				GUI.Box(new Rect(mousePosition.x + 15f, (float)Screen.height - mousePosition.y + 15f, x, 25f), content, TooltipGUI.tooltipStyle);
			}
		}

		// Token: 0x04000005 RID: 5
		public static TooltipGUI instance = null;

		// Token: 0x04000006 RID: 6
		public static bool EnableTooltip = true;

		// Token: 0x04000007 RID: 7
		public static string Tooltip = "";

		// Token: 0x04000008 RID: 8
		private static GUIStyle tooltipStyle;

		// Token: 0x04000009 RID: 9
		private static bool fired = false;
	}
}
