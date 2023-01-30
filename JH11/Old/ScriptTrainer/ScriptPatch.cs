using System;
using System.Collections.Generic;
using GameEventType;
using HarmonyLib;

namespace ScriptTrainer
{
	// Token: 0x02000011 RID: 17
	public class ScriptPatch
	{
		// Token: 0x0400005E RID: 94
		public static ValueName<bool> NieRen = new ValueName<bool>
		{
			Value = false
		};

		// Token: 0x0400005F RID: 95
		public static string NieRenQianWindow = "";

		// Token: 0x02000027 RID: 39
		[HarmonyPatch(typeof(DateJieDuanManager), "AddJieDuan")]
		public class DateJieDuanManagerOverridePatch_AddJieDuan
		{
			// Token: 0x0600011D RID: 285 RVA: 0x0000AEC8 File Offset: 0x000090C8
			[HarmonyPrefix]
			public static bool Prefix(ref float n)
			{
				bool flag = ScriptTrainer.MultipleCanWu.Value && ScriptTrainer.MultipleCanWuRate.Value > 0f;
				if (flag)
				{
					Action<float> action = delegate(float N)
					{
						JiNengUpgrader.PushJieDuan(1f);
					};
					float num;
					for (num = n * (ScriptTrainer.MultipleCanWuRate.Value - 1f); num > 1f; num -= 1f)
					{
						action(1f);
					}
					bool flag2 = num > float.Epsilon;
					if (flag2)
					{
						action(num);
					}
				}
				bool value = ScriptTrainer.StopTime.Value;
				bool result;
				if (value)
				{
					Action<float> action2 = delegate(float N)
					{
						float jieDuanCount = DataManager.Instance.GameData.GameRuntimeData.JieDuanCount;
						JieDuanExecutor.PushJieDuan(1f);
						CharacterHealthManager.PushJieDuan(1f);
						JiNengUpgrader.PushJieDuan(1f);
						TrainingManager.UpdateTrainings(1f);
						DateJieDuanManager.OnLargeJieDuanChange(jieDuanCount);
						JieDuanExecutor.CharacterPushJieDuan(1f);
						TimeLineManager.TimeLineOnChange(jieDuanCount);
						JieDuanExecutor.QuestPoolPushJieDuan(1f);
						ActionManager_Buff.OnJieDuanChange(1f);
						bool flag4 = DataManager.Instance.GameData.GameRuntimeData.CurrentSceneName == "JHWorld";
						if (flag4)
						{
							GameEventSystem.Publish<StartCacheDialogue>(default(StartCacheDialogue));
						}
						UIManager.GetAcitveUIManager().UIDataUtil.RefreshJieDuanPanel();
						ActionManager.OnJieDuanChange(N);
						CaravanManager.OnJieDuanChange(N);
					};
					while (n > 1f)
					{
						action2(1f);
						n -= 1f;
					}
					bool flag3 = n > float.Epsilon;
					if (flag3)
					{
						action2(n);
					}
					result = false;
				}
				else
				{
					result = true;
				}
				return result;
			}
		}

		// Token: 0x02000028 RID: 40
		[HarmonyPatch(typeof(FightManager), "FightAfter")]
		public class FightManagerOverridePatch_FightAfter
		{
			// Token: 0x0600011F RID: 287 RVA: 0x0000AFF8 File Offset: 0x000091F8
			[HarmonyPostfix]
			public static void Postfix()
			{
				bool value = ScriptTrainer.AutoRecoverNeiLi.Value;
				if (value)
				{
					ScriptTrainer.Instance.Log("已捕获自动恢复内力");
					Character player = DataManager.Instance.GameData.GameRuntimeData.Player;
					CharacterBattleData battleData = player.BattleData;
					battleData.ZhanDouShuXing.ZhanDouNeiLi.Value = player.Deeds.NeiLiSum;
				}
			}
		}

		// Token: 0x02000029 RID: 41
		[HarmonyPatch(typeof(DataHelp), "GetXiuLianBattle")]
		public class DataHelpOverridePatch_GetXiuLianBattle
		{
			// Token: 0x06000121 RID: 289 RVA: 0x0000B068 File Offset: 0x00009268
			[HarmonyPostfix]
			public static void Postfix(ref XiuLianBattle __result)
			{
				bool flag = ScriptTrainer.MultipleExperience.Value && ScriptTrainer.MultipleExperienceRate.Value > 0f;
				if (flag)
				{
					ScriptTrainer.Instance.Log(string.Format("内功经验获得后增加:{0}", __result.NeiGongJingYan * (ScriptTrainer.MultipleExperienceRate.Value - 1f)));
					__result.NeiGongJingYan *= ScriptTrainer.MultipleExperienceRate.Value;
					ScriptTrainer.Instance.Log(string.Format("轻功经验获得后增加:{0}", __result.QingGongJingYan * (ScriptTrainer.MultipleExperienceRate.Value - 1f)));
					__result.QingGongJingYan *= ScriptTrainer.MultipleExperienceRate.Value;
					ScriptTrainer.Instance.Log(string.Format("外功经验获得后增加:{0}", __result.WuGongJingYan * (ScriptTrainer.MultipleExperienceRate.Value - 1f)));
					__result.WuGongJingYan *= ScriptTrainer.MultipleExperienceRate.Value;
				}
			}
		}

		// Token: 0x0200002A RID: 42
		[HarmonyPatch(typeof(Character), "GetYangChengQingGongXiuLianSuDu")]
		public class CharacterOverridePatch_GetYangChengQingGongXiuLianSuDu
		{
			// Token: 0x06000123 RID: 291 RVA: 0x0000B188 File Offset: 0x00009388
			[HarmonyPostfix]
			public static void Postfix(ref float __result)
			{
				bool flag = ScriptTrainer.MultipleExperience.Value && ScriptTrainer.MultipleExperienceRate.Value > 0f;
				if (flag)
				{
					__result = ScriptTrainer.MultipleExperienceRate.Value * (__result + 1f) - 1f;
				}
			}
		}

		// Token: 0x0200002B RID: 43
		[HarmonyPatch(typeof(PlaceRelationManager), "ChangePlaceRelationValue")]
		public class PlaceRelationManagerOverridePatch_ChangePlaceRelationValue
		{
			// Token: 0x06000125 RID: 293 RVA: 0x0000B1E0 File Offset: 0x000093E0
			[HarmonyPrefix]
			public static void Prefix(ref float Value)
			{
				bool value = ScriptTrainer.MultiplePlaceRelation.Value;
				if (value)
				{
					bool flag = Value > 0f && ScriptTrainer.MultiplePlaceRate.Value > 0f;
					if (flag)
					{
						Value *= ScriptTrainer.MultiplePlaceRate.Value;
					}
				}
			}
		}

		// Token: 0x0200002C RID: 44
		[HarmonyPatch(typeof(RelationMananger), "InitXiangShi")]
		public class RelationManangerOverridePatch_InitXiangShi
		{
			// Token: 0x06000127 RID: 295 RVA: 0x0000B23C File Offset: 0x0000943C
			[HarmonyPrefix]
			public static void Prefix(ref Character other)
			{
				bool isXiangShiPlayer = other.SocialData.SocialRelation.IsXiangShiPlayer;
				if (!isXiangShiPlayer)
				{
					ScriptTrainer.Instance.Log("额外添加初始好感度捕获");
					List<string> tianFu = DataManager.Instance.GameData.GameRuntimeData.Player.SocialData.TianFu;
					float num = 0f;
					bool flag = !tianFu.IsNullOrEmpty<string>();
					if (flag)
					{
						foreach (string id in tianFu.FindAll((string x) => DataHelp.GetTianFu(x).Type == CharacterTianFuType.WanRenMi))
						{
							CharacterTianFu tianFu2 = DataHelp.GetTianFu(id);
							num += tianFu2.RelationValue;
						}
					}
					num += other.SocialData.SocialRelation.GetXiangXing().AddRelationValue(other);
					ScriptTrainer.Instance.Log(string.Format("额外添加初始好感度验证数值：{0}", num));
					bool value = ScriptTrainer.MultipleCharacterRelation.Value;
					if (value)
					{
						bool flag2 = num > 0f && ScriptTrainer.MultipleCharacterRate.Value - 1f > 0f;
						if (flag2)
						{
							num *= ScriptTrainer.MultipleCharacterRate.Value - 1f;
							ScriptTrainer.Instance.Log(string.Format("额外添加初始好感度：{0}", num));
							other.SocialData.SocialRelation.RelationValue += num;
						}
					}
				}
			}
		}

		// Token: 0x0200002D RID: 45
		[HarmonyPatch(typeof(RelationMananger), "ChangeRelationValue")]
		public class RelationManangerOverridePatch_ChangeRelationValue
		{
			// Token: 0x06000129 RID: 297 RVA: 0x0000B3E8 File Offset: 0x000095E8
			[HarmonyPrefix]
			public static void Prefix(ref float value)
			{
				bool value2 = ScriptTrainer.MultipleCharacterRelation.Value;
				if (value2)
				{
					bool flag = value > 0f && ScriptTrainer.MultipleCharacterRate.Value > 0f;
					if (flag)
					{
						value *= ScriptTrainer.MultipleCharacterRate.Value;
						ScriptTrainer.Instance.Log(string.Format("好感度倍率获得后增加:{0}", value));
					}
				}
			}
		}

		// Token: 0x0200002E RID: 46
		[HarmonyPatch(typeof(JiNengUpgrader), "PushJieDuan")]
		public class JiNengUpgraderOverridePatch_PushJieDuan
		{
			// Token: 0x0600012B RID: 299 RVA: 0x0000B460 File Offset: 0x00009660
			[HarmonyPrefix]
			public static bool Prefix(ref float jieduan)
			{
				bool flag = ScriptTrainer.MultipleCanWuShuXing.Value && ScriptTrainer.MultipleCanWuShuXingRate.Value > 1;
				bool result;
				if (flag)
				{
					List<JNUpgradeCache> jnupgradeCaches = DataManager.Instance.GameData.GameRuntimeData.JNUpgradeCaches;
					List<JNUpgradeCache> list = new List<JNUpgradeCache>();
					foreach (JNUpgradeCache jnupgradeCache in jnupgradeCaches)
					{
						jnupgradeCache.CompleteJieDuan -= jieduan;
						bool flag2 = jnupgradeCache.CompleteJieDuan <= 0f;
						if (flag2)
						{
							for (int i = 0; i < ScriptTrainer.MultipleCanWuShuXingRate.Value; i++)
							{
								ScriptTrainer.Instance.Log("ZG:升级参悟技能");
								JiNengUpgrader.Upgrade(jnupgradeCache);
							}
							list.Add(jnupgradeCache);
						}
					}
					foreach (JNUpgradeCache item in list)
					{
						DataManager.Instance.GameData.GameRuntimeData.JNUpgradeCaches.Remove(item);
					}
					result = false;
				}
				else
				{
					result = true;
				}
				return result;
			}
		}
	}
}
