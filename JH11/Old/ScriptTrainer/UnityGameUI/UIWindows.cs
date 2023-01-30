using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGameUI
{
	// Token: 0x0200000A RID: 10
	public class UIWindows
	{
		// Token: 0x06000040 RID: 64 RVA: 0x00004128 File Offset: 0x00002328
		public static void SpawnInputDialog(string prompt, string title, string defaultText, Action<string> onFinish)
		{
			GameObject canvas = UIControls.createUICanvas();
			UnityEngine.Object.DontDestroyOnLoad(canvas);
			canvas.GetComponent<Canvas>().overrideSorting = true;
			canvas.GetComponent<Canvas>().sortingOrder = 10001;
			GameObject gameObject = UIControls.createUIPanel(canvas, "70", "300", null);
			gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#37474FFF");
			Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
			GameObject gameObject2 = UIControls.createUIText(gameObject, bgSprite, "#FFFFFFFF");
			gameObject2.GetComponent<Text>().text = prompt;
			gameObject2.GetComponent<RectTransform>().localPosition = new Vector3(0f, 10f, 0f);
			Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
			GameObject uiInputField = UIControls.createUIInputField(gameObject, bgSprite2, "#FFFFFFFF");
			uiInputField.GetComponent<InputField>().text = defaultText;
			uiInputField.GetComponent<RectTransform>().localPosition = new Vector3(-50f, -10f, 0f);
			GameObject gameObject3 = UIControls.createUIButton(gameObject, "#8C9EFFFF", title, delegate
			{
				onFinish(uiInputField.GetComponent<InputField>().text);
				UnityEngine.Object.Destroy(canvas);
			}, new Vector3(100f, -10f, 0f));
			GameObject gameObject4 = UIControls.createUIButton(gameObject, "#B71C1CFF", "X", delegate
			{
				UnityEngine.Object.Destroy(canvas);
			}, new Vector3(165f, 25f, 0f));
			gameObject4.GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 20f);
			gameObject4.GetComponentInChildren<Text>().color = UIControls.HTMLString2Color("#FFFFFFFF");
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000042F4 File Offset: 0x000024F4
		public static void SpawnDropdownDialog(string prompt, string title, List<string> options, Action<int> onFinish)
		{
			GameObject canvas = UIControls.createUICanvas();
			UnityEngine.Object.DontDestroyOnLoad(canvas);
			canvas.GetComponent<Canvas>().overrideSorting = true;
			canvas.GetComponent<Canvas>().sortingOrder = 100;
			GameObject gameObject = UIControls.createUIPanel(canvas, "70", "300", null);
			gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#37474FFF");
			Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
			GameObject gameObject2 = UIControls.createUIText(gameObject, bgSprite, "#FFFFFFFF");
			gameObject2.GetComponent<Text>().text = prompt;
			gameObject2.GetComponent<RectTransform>().localPosition = new Vector3(0f, 10f, 0f);
			Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
			Sprite scrollbarSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));
			Sprite dropDownSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
			Sprite checkmarkSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));
			Sprite customMaskSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#E65100FF"));
			Color labelColor = UIControls.HTMLString2Color("#EFEBE9FF");
			GameObject gameObject3 = UIControls.createUIDropDown(gameObject, bgSprite2, scrollbarSprite, dropDownSprite, checkmarkSprite, customMaskSprite, options, labelColor);
			UnityEngine.Object.DontDestroyOnLoad(gameObject3);
			gameObject3.GetComponent<RectTransform>().localPosition = new Vector3(-50f, -10f, 0f);
			int m_call = 0;
			gameObject3.GetComponent<Dropdown>().onValueChanged.AddListener(delegate(int call)
			{
				m_call = call;
			});
			GameObject gameObject4 = UIControls.createUIButton(gameObject, "#8C9EFFFF", title, delegate
			{
				onFinish(m_call);
				UnityEngine.Object.Destroy(canvas);
			}, new Vector3(100f, -10f, 0f));
			GameObject gameObject5 = UIControls.createUIButton(gameObject, "#B71C1CFF", "X", delegate
			{
				UnityEngine.Object.Destroy(canvas);
			}, new Vector3(165f, 25f, 0f));
			gameObject5.GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 20f);
			gameObject5.GetComponentInChildren<Text>().color = Color.white;
		}
	}
}
