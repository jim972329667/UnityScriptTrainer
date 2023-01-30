using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityGameUI;

namespace ScriptTrainer
{
	// Token: 0x0200000C RID: 12
	internal class CharacterWindow
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000048 RID: 72 RVA: 0x00004658 File Offset: 0x00002858
		private static string uiText_text
		{
			get
			{
				return string.Format("{0} / {1}", CharacterWindow.page, 5);
			}
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00004684 File Offset: 0x00002884
		public CharacterWindow(GameObject panel, int x, int y)
		{
			CharacterWindow.Panel = panel;
			CharacterWindow.initialX = (CharacterWindow.elementX = x + 10);
			CharacterWindow.elementY = y;
			CharacterWindow.initialY = y;
			this.Initialize();
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000046B8 File Offset: 0x000028B8
		public void Initialize()
		{
			CharacterWindow.elementX = CharacterWindow.initialX;
			CharacterWindow.elementY = CharacterWindow.initialY;
			GameObject gameObject = UIControls.createUIButton(CharacterWindow.Panel, "#8C9EFFFF", "获取角色信息", delegate
			{
				ScriptTrainer.Instance.Log("ZG:获取角色信息");
				CharacterWindow.GetCharacterValue(1);
				CharacterWindow.container();
			}, new Vector3((float)CharacterWindow.elementX, (float)CharacterWindow.elementY, 0f));
			gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 30f);
			CharacterWindow.elementX += 110;
			CharacterWindow.hr();
			bool flag = CharacterWindow.TryGetData();
			if (flag)
			{
				CharacterWindow.container();
			}
			else
			{
				CharacterWindow.elementY = -175;
			}
			this.PageBar(CharacterWindow.Panel);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x0000477C File Offset: 0x0000297C
		private static void CharacterBar(GameObject panel, int width, CharacterWindow.CharacterValueInfo<string> Name)
		{
			int num = 70;
			int num2 = 70 + width + 10;
			int num3 = -num2 / 2 + 45;
			GameObject gameObject = UIControls.createUIPanel(panel, "40", num2.ToString(), null);
			gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#455A64FF");
			gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)CharacterWindow.elementX, (float)CharacterWindow.elementY, 0f);
			Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
			GameObject gameObject2 = UIControls.createUIText(gameObject, bgSprite, "#FFFFFFFF");
			gameObject2.GetComponent<Text>().text = Name.Name;
			gameObject2.GetComponent<RectTransform>().localPosition = new Vector2((float)num3, 0f);
			gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2((float)num, 30f);
			gameObject2.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
			num3 += num + width / 2 - 40;
			Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
			GameObject gameObject3 = UIControls.createUIInputField(gameObject, bgSprite2, "#FFFFFFFF");
			gameObject3.GetComponent<InputField>().text = Name.BaseValue.Value;
			gameObject3.GetComponent<RectTransform>().localPosition = new Vector2((float)num3, 0f);
			gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2((float)width, 30f);
			gameObject3.GetComponent<InputField>().onEndEdit.AddListener(delegate(string text)
			{
				Name.BaseValue.Value = text;
			});
			CharacterWindow.elementX += num2;
			CharacterWindow.ItemButtons.Add(gameObject);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00004934 File Offset: 0x00002B34
		private static void CharacterBar(GameObject panel, int width, CharacterWindow.CharacterValueInfo<int> Name)
		{
			int num = 70;
			int num2 = 70 + width + 10;
			int num3 = -num2 / 2 + 45;
			GameObject gameObject = UIControls.createUIPanel(panel, "40", num2.ToString(), null);
			gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#455A64FF");
			gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)CharacterWindow.elementX, (float)CharacterWindow.elementY, 0f);
			Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
			GameObject gameObject2 = UIControls.createUIText(gameObject, bgSprite, "#FFFFFFFF");
			gameObject2.GetComponent<Text>().text = Name.Name;
			gameObject2.GetComponent<RectTransform>().localPosition = new Vector2((float)num3, 0f);
			gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2((float)num, 30f);
			gameObject2.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
			num3 += num + width / 2 - 40;
			Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
			GameObject gameObject3 = UIControls.createUIInputField(gameObject, bgSprite2, "#FFFFFFFF");
			gameObject3.GetComponent<InputField>().text = Name.BaseValue.Value.ToString();
			gameObject3.GetComponent<RectTransform>().localPosition = new Vector2((float)num3, 0f);
			gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2((float)width, 30f);
			gameObject3.GetComponent<InputField>().onEndEdit.AddListener(delegate(string text)
			{
				Name.BaseValue.Value = text.ConvertToIntDef(Name.BaseValue.Value);
			});
			CharacterWindow.elementX += num2;
			CharacterWindow.ItemButtons.Add(gameObject);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00004AF0 File Offset: 0x00002CF0
		private static void CharacterBar(GameObject panel, int width, CharacterWindow.CharacterValueInfo<float> Name)
		{
			int num = 70;
			int num2 = 70 + width + 10;
			int num3 = -num2 / 2 + 45;
			GameObject gameObject = UIControls.createUIPanel(panel, "40", num2.ToString(), null);
			gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#455A64FF");
			gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)CharacterWindow.elementX, (float)CharacterWindow.elementY, 0f);
			Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
			GameObject gameObject2 = UIControls.createUIText(gameObject, bgSprite, "#FFFFFFFF");
			gameObject2.GetComponent<Text>().text = Name.Name;
			gameObject2.GetComponent<RectTransform>().localPosition = new Vector2((float)num3, 0f);
			gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2((float)num, 30f);
			gameObject2.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
			num3 += num + width / 2 - 40;
			Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
			GameObject gameObject3 = UIControls.createUIInputField(gameObject, bgSprite2, "#FFFFFFFF");
			gameObject3.GetComponent<InputField>().text = Name.BaseValue.Value.ToString();
			gameObject3.GetComponent<RectTransform>().localPosition = new Vector2((float)num3, 0f);
			gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2((float)width, 30f);
			gameObject3.GetComponent<InputField>().onEndEdit.AddListener(delegate(string text)
			{
				Name.BaseValue.Value = text.ConvertToFloatDef(Name.BaseValue.Value);
			});
			CharacterWindow.elementX += num2;
			CharacterWindow.ItemButtons.Add(gameObject);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004CAC File Offset: 0x00002EAC
		private void PageBar(GameObject panel)
		{
			GameObject gameObject = UIControls.createUIPanel(panel, "40", "500", null);
			gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
			gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, (float)CharacterWindow.elementY, 0f);
			Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
			bool flag = CharacterWindow.uiText == null;
			if (flag)
			{
				CharacterWindow.uiText = UIControls.createUIText(gameObject, bgSprite, "#ffFFFFFF");
				CharacterWindow.uiText.GetComponent<Text>().text = CharacterWindow.uiText_text;
				CharacterWindow.uiText.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
				CharacterWindow.uiText.GetComponent<Text>().fontSize = 20;
				CharacterWindow.uiText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
			}
			string backgroundColor = "#8C9EFFFF";
			GameObject gameObject2 = UIControls.createUIButton(gameObject, backgroundColor, "上一页", delegate
			{
				CharacterWindow.page--;
				bool flag2 = CharacterWindow.page <= 0;
				if (flag2)
				{
					CharacterWindow.page = 1;
				}
				CharacterWindow.GetCharacterValue(CharacterWindow.page);
				bool flag3 = CharacterWindow.TryGetData();
				if (flag3)
				{
					CharacterWindow.container();
				}
				CharacterWindow.uiText.GetComponent<Text>().text = CharacterWindow.uiText_text;
			}, default(Vector3));
			gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 20f);
			gameObject2.GetComponent<RectTransform>().localPosition = new Vector3(-100f, 0f, 0f);
			GameObject gameObject3 = UIControls.createUIButton(gameObject, backgroundColor, "下一页", delegate
			{
				CharacterWindow.page++;
				bool flag2 = CharacterWindow.page >= 5;
				if (flag2)
				{
					CharacterWindow.page = 5;
				}
				CharacterWindow.GetCharacterValue(CharacterWindow.page);
				bool flag3 = CharacterWindow.TryGetData();
				if (flag3)
				{
					CharacterWindow.container();
				}
				CharacterWindow.uiText.GetComponent<Text>().text = CharacterWindow.uiText_text;
			}, default(Vector3));
			gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 20f);
			gameObject3.GetComponent<RectTransform>().localPosition = new Vector3(100f, 0f, 0f);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00004E84 File Offset: 0x00003084
		private static void container()
		{
			CharacterWindow.elementX = -195;
			CharacterWindow.elementY = 125;
			CharacterWindow.ItemPanel = UIControls.createUIPanel(CharacterWindow.Panel, "300", "600", null);
			CharacterWindow.ItemPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
			CharacterWindow.ItemPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(10f, 0f);
			foreach (GameObject obj in CharacterWindow.ItemButtons)
			{
				UnityEngine.Object.Destroy(obj);
			}
			CharacterWindow.ItemButtons.Clear();
			int num = 0;
			foreach (CharacterWindow.CharacterValueInfo<string> name in CharacterWindow.CharacterStringValue)
			{
				CharacterWindow.CharacterBar(CharacterWindow.ItemPanel, 100, name);
				num++;
				bool flag = num % 3 == 0;
				if (flag)
				{
					CharacterWindow.hr(-195);
				}
				else
				{
					CharacterWindow.elementX += 10;
				}
			}
			foreach (CharacterWindow.CharacterValueInfo<int> name2 in CharacterWindow.CharacterIntValue)
			{
				CharacterWindow.CharacterBar(CharacterWindow.ItemPanel, 100, name2);
				num++;
				bool flag2 = num % 3 == 0;
				if (flag2)
				{
					CharacterWindow.hr(-195);
				}
				else
				{
					CharacterWindow.elementX += 10;
				}
			}
			foreach (CharacterWindow.CharacterValueInfo<float> name3 in CharacterWindow.CharacterFloatValue)
			{
				CharacterWindow.CharacterBar(CharacterWindow.ItemPanel, 100, name3);
				num++;
				bool flag3 = num % 3 == 0;
				if (flag3)
				{
					CharacterWindow.hr(-195);
				}
				else
				{
					CharacterWindow.elementX += 10;
				}
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000050C8 File Offset: 0x000032C8
		private static void typeBar(GameObject panel)
		{
			Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
			GameObject gameObject = UIControls.createUIText(panel, bgSprite, "#FFFFFFFF");
			gameObject.GetComponent<Text>().text = "性别";
			gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)CharacterWindow.elementX, (float)CharacterWindow.elementY, 0f);
			gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
			CharacterWindow.elementX += 40;
			List<string> options = new List<string>
			{
				"男",
				"女",
				"阉"
			};
			Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
			Sprite scrollbarSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));
			Sprite dropDownSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
			Sprite checkmarkSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));
			Sprite customMaskSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#E65100FF"));
			Color labelColor = UIControls.HTMLString2Color("#EFEBE9FF");
			GameObject gameObject2 = UIControls.createUIDropDown(panel, bgSprite2, scrollbarSprite, dropDownSprite, checkmarkSprite, customMaskSprite, options, labelColor);
			UnityEngine.Object.DontDestroyOnLoad(gameObject2);
			gameObject2.GetComponent<RectTransform>().localPosition = new Vector3((float)CharacterWindow.elementX, (float)CharacterWindow.elementY, 0f);
			gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 30f);
			bool flag = CharacterWindow.CharacterGenderValue.Count != 0;
			if (flag)
			{
				gameObject2.GetComponent<Dropdown>().value = CharacterWindow.ParseGenderToInt(CharacterWindow.CharacterGenderValue[0].BaseValue.Value);
			}
			gameObject2.GetComponent<Dropdown>().onValueChanged.AddListener(delegate(int call)
			{
				bool flag2 = CharacterWindow.CharacterGenderValue.Count != 0;
				if (flag2)
				{
					ScriptTrainer.Instance.Log(string.Format("更改角色性别；{0}", CharacterWindow.ParseIntToGender(call)));
					CharacterWindow.CharacterGenderValue[0].BaseValue.Value = CharacterWindow.ParseIntToGender(call);
					bool flag3 = CharacterWindow.CharacterGenderValue[0].BaseValue.Value == Gender.Nan;
					if (flag3)
					{
						CharacterWindow.CharacterGenderValue[0].BaseValue.ValueInfo = Config.Instance.I18N.Global.Gender.Nan;
					}
					else
					{
						bool flag4 = CharacterWindow.CharacterGenderValue[0].BaseValue.Value == Gender.Nv;
						if (flag4)
						{
							CharacterWindow.CharacterGenderValue[0].BaseValue.ValueInfo = Config.Instance.I18N.Global.Gender.Nv;
						}
						else
						{
							bool flag5 = CharacterWindow.CharacterGenderValue[0].BaseValue.Value == Gender.Yan;
							if (flag5)
							{
								CharacterWindow.CharacterGenderValue[0].BaseValue.ValueInfo = Config.Instance.I18N.Global.Gender.Yan;
							}
						}
					}
				}
			});
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00005298 File Offset: 0x00003498
		private static GameObject CreateItemButton(string ButtonText, string ItemName, Sprite ItemIcon, GameObject panel, UnityAction action)
		{
			int num = 190;
			int num2 = 50;
			string htmlcolorstring = "#FFFFFFFF";
			GameObject gameObject = UIControls.createUIPanel(panel, num2.ToString(), num.ToString(), null);
			gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#455A64FF");
			gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)CharacterWindow.elementX, (float)CharacterWindow.elementY, 0f);
			GameObject gameObject2 = UIControls.createUIPanel(gameObject, num2.ToString(), "50", null);
			gameObject2.GetComponent<Image>().color = UIControls.HTMLString2Color(htmlcolorstring);
			gameObject2.GetComponent<RectTransform>().anchoredPosition = new Vector2(70f, 0f);
			GameObject gameObject3 = UIControls.createUIImage(gameObject2, ItemIcon);
			gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 60f);
			gameObject3.GetComponent<RectTransform>().localPosition = new Vector2(0f, 0f);
			Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
			GameObject gameObject4 = UIControls.createUIText(gameObject, bgSprite, ColorUtility.ToHtmlStringRGBA(Color.white));
			gameObject4.GetComponent<Text>().text = ItemName;
			gameObject4.GetComponent<RectTransform>().localPosition = new Vector3(0f, 5f, 0f);
			string backgroundColor = "#8C9EFFFF";
			GameObject gameObject5 = UIControls.createUIButton(gameObject, backgroundColor, ButtonText, action, default(Vector3));
			gameObject5.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 20f);
			gameObject5.GetComponent<RectTransform>().localPosition = new Vector3(-50f, -10f, 0f);
			CharacterWindow.elementX += 200;
			return gameObject5;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00005460 File Offset: 0x00003660
		public static GameObject AddToggle(string Text, int width, GameObject panel, UnityAction<bool> action)
		{
			CharacterWindow.elementX += width / 2 - 30;
			Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture(ColorUtility.ToHtmlStringRGBA(Color.white)));
			Sprite customCheckmarkSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#18FFFFFF"));
			GameObject gameObject = UIControls.createUIToggle(panel, bgSprite, customCheckmarkSprite);
			gameObject.GetComponentInChildren<Text>().color = Color.white;
			gameObject.GetComponentInChildren<Toggle>().isOn = false;
			gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)CharacterWindow.elementX, (float)CharacterWindow.elementY, 0f);
			gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2((float)width, 20f);
			gameObject.GetComponentInChildren<Text>().text = Text;
			gameObject.GetComponentInChildren<Toggle>().onValueChanged.AddListener(action);
			CharacterWindow.elementX += width / 2 + 10;
			return gameObject;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x0000553A File Offset: 0x0000373A
		private static void hr()
		{
			CharacterWindow.elementX = CharacterWindow.initialX;
			CharacterWindow.elementY -= 60;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00005554 File Offset: 0x00003754
		private static void hr(int x)
		{
			CharacterWindow.elementX = x;
			CharacterWindow.elementY -= 60;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x0000556C File Offset: 0x0000376C
		private static bool TryGetData()
		{
			bool flag = DataManager.Instance.GameData.GameRuntimeData.Player == null;
			return !flag;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000055A0 File Offset: 0x000037A0
		private static void GetCharacterValue(int page = 1)
		{
			Character player = DataManager.Instance.GameData.GameRuntimeData.Player;
			CharacterBasicData basicData = player.BasicData;
			CharacterSocialData socialData = player.SocialData;
			CharacterBattleData battleData = player.BattleData;
			CharacterWindow.CharacterStringValue.Clear();
			CharacterWindow.CharacterIntValue.Clear();
			CharacterWindow.CharacterFloatValue.Clear();
			CharacterWindow.CharacterGenderValue.Clear();
			switch (page)
			{
			case 1:
				CharacterWindow.CharacterStringValue.Add(new CharacterWindow.CharacterValueInfo<string>(basicData.Name, "名字"));
				CharacterWindow.CharacterIntValue.Add(new CharacterWindow.CharacterValueInfo<int>(basicData.Age, "年龄"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.XinTai, "心态"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(basicData.MingSheng.MingSheng, "名声"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(basicData.LiChang, "立场"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(basicData.WuXing, "悟性"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(basicData.JiaoShe, "交涉"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(basicData.JianShi, "见识"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(basicData.FuYuan, "福源"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.XiuWei.DaoDe, "道德"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.XiuWei.MeiLi, "魅力"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.XiuWei.HanYang, "涵养"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.XiuWei.ShaYi, "杀意"));
				break;
			case 2:
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.WaiGong.QuanZhang, "拳掌"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.WaiGong.JianFa, "用剑"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.WaiGong.DaoFa, "使刀"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.WaiGong.ZhiLi, "指力"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.WaiGong.YongQiang, "弄枪"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.WaiGong.ShuaGun, "刷棍"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.WaiGong.ChuiFu, "运斧"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.WaiGong.ShiBian, "鞭术"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.WaiGong.GouFa, "钩法"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.WaiGong.CunJin, "暗器"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.WaiGong.BiFa, "笔法"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.WaiGong.YaoMa, "腰马"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.WaiGong.XiaPan, "下盘"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.WaiGong.TiPo, "体魄"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.WaiGong.JinGu, "筋骨"));
				break;
			case 3:
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.NeiGong.QiLiang, "气量"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.NeiGong.TiaoXi, "调息"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.NeiGong.QiFa, "气法"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.NeiGong.ShenXing, "身形"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.JingShen.YiZhi, "意志"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.JingShen.JiZhong, "集中"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.JingShen.ZhenDing, "镇定"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.JingShen.JueDuan, "决断"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.JingShen.ShenLai, "神来"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.MaiLuo.DuMai, "督脉"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.MaiLuo.RenMai, "任脉"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.MaiLuo.ChongDai, "冲带(未知)"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.MaiLuo.QiaoMai, "巧脉(未知)"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(battleData.MaiLuo.WeiMai, "微脉(未知)"));
				break;
			case 4:
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.ShiCi, "诗词"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.YinJiu, "饮酒"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.PengRen, "烹饪"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.GuanPu, "关扑"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.ChaDao, "茶道"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.ShouCang, "收藏"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.HuiHua, "书画"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.YiShu, "医术"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.YueQi, "乐器"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.DuShu, "毒术"));
				break;
			case 5:
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.ShiCiLike, "诗词喜爱"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.YinJiuLike, "饮酒喜爱"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.PengRenLike, "烹饪喜爱"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.GuanPuLike, "关扑喜爱"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.ChaDaoLike, "茶道喜爱"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.ShouCangLike, "收藏喜爱"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.HuiHuaLike, "书画喜爱"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.YiShuLike, "医术喜爱"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.YueQiLike, "乐器喜爱"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.Hobby.DuShuLike, "毒术喜爱"));
				break;
			default:
				CharacterWindow.CharacterStringValue.Add(new CharacterWindow.CharacterValueInfo<string>(basicData.Name, "名字"));
				CharacterWindow.CharacterIntValue.Add(new CharacterWindow.CharacterValueInfo<int>(basicData.Age, "年龄"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(basicData.WuXing, "悟性"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(basicData.JiaoShe, "交涉"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(basicData.JianShi, "见识"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(basicData.FuYuan, "福源"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.XiuWei.DaoDe, "道德"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.XiuWei.MeiLi, "魅力"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.XiuWei.HanYang, "涵养"));
				CharacterWindow.CharacterFloatValue.Add(new CharacterWindow.CharacterValueInfo<float>(socialData.XiuWei.ShaYi, "杀意"));
				break;
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00005F08 File Offset: 0x00004108
		public static int ParseGenderToInt(Gender gender)
		{
			bool flag = gender == Gender.Nan;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = gender == Gender.Nv;
				if (flag2)
				{
					result = 1;
				}
				else
				{
					bool flag3 = gender == Gender.Yan;
					if (flag3)
					{
						result = 2;
					}
					else
					{
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00005F40 File Offset: 0x00004140
		public static Gender ParseIntToGender(int value)
		{
			bool flag = value == 0;
			Gender result;
			if (flag)
			{
				result = Gender.Nan;
			}
			else
			{
				bool flag2 = value == 1;
				if (flag2)
				{
					result = Gender.Nv;
				}
				else
				{
					bool flag3 = value == 2;
					if (flag3)
					{
						result = Gender.Yan;
					}
					else
					{
						result = Gender.Nan;
					}
				}
			}
			return result;
		}

		// Token: 0x04000020 RID: 32
		private static GameObject Panel;

		// Token: 0x04000021 RID: 33
		private static int initialX;

		// Token: 0x04000022 RID: 34
		private static int initialY;

		// Token: 0x04000023 RID: 35
		private static int elementX;

		// Token: 0x04000024 RID: 36
		private static int elementY;

		// Token: 0x04000025 RID: 37
		private static GameObject ItemPanel;

		// Token: 0x04000026 RID: 38
		private static List<GameObject> ItemButtons = new List<GameObject>();

		// Token: 0x04000027 RID: 39
		private static int page = 1;

		// Token: 0x04000028 RID: 40
		private const int maxPage = 5;

		// Token: 0x04000029 RID: 41
		private static GameObject uiText;

		// Token: 0x0400002A RID: 42
		private static List<CharacterWindow.CharacterValueInfo<string>> CharacterStringValue = new List<CharacterWindow.CharacterValueInfo<string>>();

		// Token: 0x0400002B RID: 43
		private static List<CharacterWindow.CharacterValueInfo<int>> CharacterIntValue = new List<CharacterWindow.CharacterValueInfo<int>>();

		// Token: 0x0400002C RID: 44
		private static List<CharacterWindow.CharacterValueInfo<float>> CharacterFloatValue = new List<CharacterWindow.CharacterValueInfo<float>>();

		// Token: 0x0400002D RID: 45
		private static List<CharacterWindow.CharacterValueInfo<Gender>> CharacterGenderValue = new List<CharacterWindow.CharacterValueInfo<Gender>>();

		// Token: 0x02000018 RID: 24
		private class CharacterValueInfo<T>
		{
			// Token: 0x060000D1 RID: 209 RVA: 0x0000A42A File Offset: 0x0000862A
			public CharacterValueInfo(ValueName<T> value, string name)
			{
				this.BaseValue = value;
				this.Name = name;
			}

			// Token: 0x0400007E RID: 126
			public string Name;

			// Token: 0x0400007F RID: 127
			public ValueName<T> BaseValue;
		}
	}
}
