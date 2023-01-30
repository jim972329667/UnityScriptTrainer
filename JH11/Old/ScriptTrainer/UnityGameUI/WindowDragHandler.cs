using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityGameUI
{
	// Token: 0x0200000B RID: 11
	public class WindowDragHandler : MonoBehaviour
	{
		// Token: 0x06000042 RID: 66 RVA: 0x0000451E File Offset: 0x0000271E
		public WindowDragHandler()
		{
			WindowDragHandler.instance = this;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x0000452E File Offset: 0x0000272E
		public void Awake()
		{
			WindowDragHandler.rectTransform = base.gameObject.GetComponent<RectTransform>();
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00004544 File Offset: 0x00002744
		[HarmonyPostfix]
		[HarmonyPatch(typeof(EventTrigger), "OnBeginDrag")]
		public static void OnBeginDrag(PointerEventData eventData)
		{
			bool flag = WindowDragHandler.pointerId != -98456;
			if (flag)
			{
				eventData.pointerDrag = null;
			}
			else
			{
				WindowDragHandler.pointerId = eventData.pointerId;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(WindowDragHandler.rectTransform, eventData.position, Camera.current, out WindowDragHandler.initialTouchPos);
			}
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00004598 File Offset: 0x00002798
		[HarmonyPostfix]
		[HarmonyPatch(typeof(EventTrigger), "OnDrag")]
		public static void OnDrag(PointerEventData eventData)
		{
			bool flag = eventData.pointerId != WindowDragHandler.pointerId;
			if (!flag)
			{
				Vector2 a;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(WindowDragHandler.rectTransform, eventData.position, Camera.current, out a);
				Vector2 vector = a - WindowDragHandler.initialTouchPos;
				WindowDragHandler.rectTransform.gameObject.transform.position += new Vector3(vector.x, vector.y, Camera.current.nearClipPlane);
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x0000461C File Offset: 0x0000281C
		[HarmonyPostfix]
		[HarmonyPatch(typeof(EventTrigger), "OnEndDrag")]
		public static void OnEndDrag(PointerEventData eventData)
		{
			bool flag = eventData.pointerId != WindowDragHandler.pointerId;
			if (!flag)
			{
				WindowDragHandler.pointerId = -98456;
			}
		}

		// Token: 0x0400001B RID: 27
		public static WindowDragHandler instance;

		// Token: 0x0400001C RID: 28
		private const int NON_EXISTING_TOUCH = -98456;

		// Token: 0x0400001D RID: 29
		private static RectTransform rectTransform;

		// Token: 0x0400001E RID: 30
		private static int pointerId = -98456;

		// Token: 0x0400001F RID: 31
		private static Vector2 initialTouchPos;
	}
}
