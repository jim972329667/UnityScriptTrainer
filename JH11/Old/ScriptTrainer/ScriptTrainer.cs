using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace ScriptTrainer
{
	// Token: 0x02000013 RID: 19
	[BepInPlugin("aoe.top.plugins.ScriptTrainer", "江湖十一 内置修改器", "1.0.0.0")]
	public class ScriptTrainer : BaseUnityPlugin
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x0000A01C File Offset: 0x0000821C
		// (set) Token: 0x060000A7 RID: 167 RVA: 0x0000A023 File Offset: 0x00008223
		public static ConfigEntry<KeyCode> ShowCounter { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x0000A02B File Offset: 0x0000822B
		// (set) Token: 0x060000A9 RID: 169 RVA: 0x0000A032 File Offset: 0x00008232
		public static ConfigEntry<bool> StopTime { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x060000AA RID: 170 RVA: 0x0000A03A File Offset: 0x0000823A
		// (set) Token: 0x060000AB RID: 171 RVA: 0x0000A041 File Offset: 0x00008241
		public static ConfigEntry<bool> MultipleExperience { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x060000AC RID: 172 RVA: 0x0000A049 File Offset: 0x00008249
		// (set) Token: 0x060000AD RID: 173 RVA: 0x0000A050 File Offset: 0x00008250
		public static ConfigEntry<float> MultipleExperienceRate { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060000AE RID: 174 RVA: 0x0000A058 File Offset: 0x00008258
		// (set) Token: 0x060000AF RID: 175 RVA: 0x0000A05F File Offset: 0x0000825F
		public static ConfigEntry<bool> MultiplePlaceRelation { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x0000A067 File Offset: 0x00008267
		// (set) Token: 0x060000B1 RID: 177 RVA: 0x0000A06E File Offset: 0x0000826E
		public static ConfigEntry<float> MultiplePlaceRate { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x0000A076 File Offset: 0x00008276
		// (set) Token: 0x060000B3 RID: 179 RVA: 0x0000A07D File Offset: 0x0000827D
		public static ConfigEntry<bool> MultipleCharacterRelation { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x0000A085 File Offset: 0x00008285
		// (set) Token: 0x060000B5 RID: 181 RVA: 0x0000A08C File Offset: 0x0000828C
		public static ConfigEntry<float> MultipleCharacterRate { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x0000A094 File Offset: 0x00008294
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x0000A09B File Offset: 0x0000829B
		public static ConfigEntry<bool> MultipleCanWu { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x0000A0A3 File Offset: 0x000082A3
		// (set) Token: 0x060000B9 RID: 185 RVA: 0x0000A0AA File Offset: 0x000082AA
		public static ConfigEntry<float> MultipleCanWuRate { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000BA RID: 186 RVA: 0x0000A0B2 File Offset: 0x000082B2
		// (set) Token: 0x060000BB RID: 187 RVA: 0x0000A0B9 File Offset: 0x000082B9
		public static ConfigEntry<bool> MultipleCanWuShuXing { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060000BC RID: 188 RVA: 0x0000A0C1 File Offset: 0x000082C1
		// (set) Token: 0x060000BD RID: 189 RVA: 0x0000A0C8 File Offset: 0x000082C8
		public static ConfigEntry<int> MultipleCanWuShuXingRate { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060000BE RID: 190 RVA: 0x0000A0D0 File Offset: 0x000082D0
		// (set) Token: 0x060000BF RID: 191 RVA: 0x0000A0D7 File Offset: 0x000082D7
		public static ConfigEntry<bool> AutoRecoverNeiLi { get; set; }

		// Token: 0x060000C0 RID: 192 RVA: 0x0000A0E0 File Offset: 0x000082E0
		public void Awake()
		{
			ScriptTrainer.Instance = this;
			Harmony harmony = new Harmony("ScriptTrainer");
			harmony.PatchAll();
			ScriptTrainer.ShowCounter = base.Config.Bind<KeyCode>("修改器快捷键", "Key", KeyCode.F9);
			ScriptTrainer.StopTime = base.Config.Bind<bool>("暂停时间", "Value", false);
			ScriptTrainer.MultipleExperience = base.Config.Bind<bool>("武功修炼经验", "Value", false);
			ScriptTrainer.MultipleExperienceRate = base.Config.Bind<float>("武功修炼经验倍率", "Value", 2f);
			ScriptTrainer.MultiplePlaceRelation = base.Config.Bind<bool>("地区好感度", "Value", false);
			ScriptTrainer.MultiplePlaceRate = base.Config.Bind<float>("地区好感度倍率", "Value", 2f);
			ScriptTrainer.MultipleCharacterRelation = base.Config.Bind<bool>("人物好感度", "Value", false);
			ScriptTrainer.MultipleCharacterRate = base.Config.Bind<float>("人物好感度倍率", "Value", 2f);
			ScriptTrainer.MultipleCanWu = base.Config.Bind<bool>("缩短参悟时间", "Value", false);
			ScriptTrainer.MultipleCanWuRate = base.Config.Bind<float>("缩短参悟时间倍率", "Value", 2f);
			ScriptTrainer.MultipleCanWuShuXing = base.Config.Bind<bool>("参悟获得属性", "Value", false);
			ScriptTrainer.MultipleCanWuShuXingRate = base.Config.Bind<int>("参悟获得属性倍率", "Value", 2);
			ScriptTrainer.AutoRecoverNeiLi = base.Config.Bind<bool>("战斗结束自动恢复内力", "Value", false);
			this.YourTrainer = GameObject.Find("ZG_Trainer");
			bool flag = this.YourTrainer == null;
			if (flag)
			{
				this.YourTrainer = new GameObject("ZG_Trainer");
				UnityEngine.Object.DontDestroyOnLoad(this.YourTrainer);
				this.YourTrainer.hideFlags = HideFlags.HideAndDontSave;
				this.YourTrainer.AddComponent<ZGGameObject>();
			}
			else
			{
				this.YourTrainer.AddComponent<ZGGameObject>();
			}
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x0000A2F9 File Offset: 0x000084F9
		public void Start()
		{
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x0000A2FC File Offset: 0x000084FC
		public void Update()
		{
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x0000A2FF File Offset: 0x000084FF
		public void FixedUpdate()
		{
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x0000A302 File Offset: 0x00008502
		public void Log(string message)
		{
			base.Logger.LogMessage(message);
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x0000A312 File Offset: 0x00008512
		public void OnDestroy()
		{
		}

		// Token: 0x04000061 RID: 97
		public static ScriptTrainer Instance;

		// Token: 0x04000062 RID: 98
		public GameObject YourTrainer;
	}
}
