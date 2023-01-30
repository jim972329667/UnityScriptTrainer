using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityGameUI
{
	// Token: 0x02000008 RID: 8
	public static class UINavigation
	{
		// Token: 0x0600003A RID: 58 RVA: 0x00003F60 File Offset: 0x00002160
		public static void Initialize(Navigation[] nav, GameObject panel)
		{
			UINavigation.navigations = nav;
			UINavigation.panel = panel;
			UINavigation.elementX = (int)(-panel.GetComponent<RectTransform>().rect.width / 2f + 60f);
			UINavigation.elementY = (int)(panel.GetComponent<RectTransform>().rect.height / 2f - 20f);
			UINavigation.CreateNavigation();
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003FCC File Offset: 0x000021CC
		public static void CreateNavigation()
		{
			foreach (Navigation navigation in UINavigation.navigations)
			{
				string backgroundColor = "#616161FF";
				Vector3 localPosition = new Vector3((float)UINavigation.elementX, (float)UINavigation.elementY, 0f);
				GameObject gameObject = UIControls.createUIButton(UINavigation.panel, backgroundColor, navigation.button, new UnityAction(navigation.SetActive), localPosition);
				gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(120f, 30f);
				gameObject.GetComponentInChildren<Text>().color = new Color(255f, 255f, 255f, 1f);
				navigation.SetActive(navigation.show);
				UINavigation.elementY -= 30;
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00004098 File Offset: 0x00002298
		public static void SetActive(this Navigation nav)
		{
			foreach (Navigation navigation in UINavigation.navigations)
			{
				bool flag = navigation == nav;
				if (flag)
				{
					navigation.SetActive(true);
				}
				else
				{
					navigation.SetActive(false);
				}
			}
		}

		// Token: 0x04000013 RID: 19
		public static GameObject panel;

		// Token: 0x04000014 RID: 20
		private static Navigation[] navigations;

		// Token: 0x04000015 RID: 21
		private static int elementX;

		// Token: 0x04000016 RID: 22
		private static int elementY;
	}
}
