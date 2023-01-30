using System;
using UnityEngine;

namespace ScriptTrainer
{
	// Token: 0x02000012 RID: 18
	public class Scripts : MonoBehaviour
	{
		// Token: 0x06000097 RID: 151 RVA: 0x000098C4 File Offset: 0x00007AC4
		public static void DebugButton()
		{
			bool flag = DataManager.Instance.GameData.GameRuntimeData.Player != null;
			if (flag)
			{
				Character player = DataManager.Instance.GameData.GameRuntimeData.Player;
				foreach (Place self in DataManager.Instance.GameData.GameRuntimeData.Places)
				{
					Scripts.Log(string.Format("ZG:{0}", self.GetPlaceRelationValue()));
				}
			}
			else
			{
				Scripts.Log("Player == null");
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00009988 File Offset: 0x00007B88
		public static void AddMoney(int count)
		{
			bool flag = DataManager.Instance.GameData.GameRuntimeData.Player != null;
			if (flag)
			{
				DataManager.Instance.GameData.GameRuntimeData.Player.BasicData.Money.Value += (float)(count * 1000);
			}
		}

		// Token: 0x06000099 RID: 153 RVA: 0x000099E8 File Offset: 0x00007BE8
		public static void AddJiangHuYueLi(float count)
		{
			bool flag = DataManager.Instance.GameData.GameRuntimeData.Player != null;
			if (flag)
			{
				bool flag2 = -count > DataManager.Instance.GameData.GameRuntimeData.Player.BattleData.CanWuBase;
				if (flag2)
				{
					DataManager.Instance.GameData.GameRuntimeData.Player.BattleData.CanWuBase = 0f;
				}
				else
				{
					DataManager.Instance.GameData.GameRuntimeData.Player.BattleData.CanWuBase += count;
				}
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00009A84 File Offset: 0x00007C84
		public static void AddNeiLi(float count)
		{
			bool flag = DataManager.Instance.GameData.GameRuntimeData.Player != null;
			if (flag)
			{
				Character player = DataManager.Instance.GameData.GameRuntimeData.Player;
				bool flag2 = player.BattleData.JiChuShuXing.JiChuNeiLi.Value < 0f;
				if (flag2)
				{
					player.BattleData.JiChuShuXing.JiChuNeiLi.Value = 3000f;
				}
				bool flag3 = player.BattleData.ZhanDouShuXing.NeiLiBase.Value < 0f;
				if (flag3)
				{
					player.BattleData.ZhanDouShuXing.NeiLiBase.Value = 0f;
				}
				bool flag4 = -count > player.BattleData.ZhanDouShuXing.NeiLiBase.Value;
				if (flag4)
				{
					player.BattleData.ZhanDouShuXing.NeiLiBase.Value = 0f;
				}
				else
				{
					player.BattleData.ZhanDouShuXing.NeiLiBase.Value += count;
				}
				float num = (float)Math.Round((double)GameFormula.CalculateCharacerZanDouNeiLi(player, true), MidpointRounding.AwayFromZero);
				player.Deeds.NeiLiSum = num;
				player.BattleData.ZhanDouShuXing.ZhanDouNeiLi.Value = num;
				UIManager.GetAcitveUIManager().UIDataUtil.RefreshJieDuanPanel();
			}
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00009BD8 File Offset: 0x00007DD8
		public static void Recover()
		{
			bool flag = DataManager.Instance.GameData.GameRuntimeData.Player != null;
			if (flag)
			{
				Character player = DataManager.Instance.GameData.GameRuntimeData.Player;
				Scripts.Log("恢复内力");
				CharacterBattleData battleData = player.BattleData;
				battleData.ZhanDouShuXing.ZhanDouNeiLi.Value = player.Deeds.NeiLiSum;
				bool flag2 = battleData.Recover != null;
				if (flag2)
				{
					Scripts.Log("恢复中毒");
					bool flag3 = battleData.Recover.DuRecoverJieDuan != null;
					if (flag3)
					{
						battleData.Debuff.ZhongDus.Enable = false;
						battleData.Recover.DuRecoverJieDuan = null;
					}
					Scripts.Log("恢复疾病");
					bool flag4 = battleData.Recover.IllRecoverJieDuan != null;
					if (flag4)
					{
						Scripts.Log("恢复疾病Debuff");
						battleData.Debuff.Ills.Clear();
						Scripts.Log("删除疾病信息");
						battleData.Recover.IllRecoverJieDuan = null;
					}
					bool flag5 = battleData.Recover.RecoverDatas.Count > 0;
					if (flag5)
					{
						battleData.Recover.RecoverDatas.Clear();
					}
				}
				Scripts.Log("恢复部位");
				bool flag6 = battleData.BuWei.TouMian.Value < 100f;
				if (flag6)
				{
					battleData.BuWei.TouMian.Value = 100f;
				}
				bool flag7 = battleData.BuWei.ShouWan.Value < 100f;
				if (flag7)
				{
					battleData.BuWei.ShouWan.Value = 100f;
				}
				bool flag8 = battleData.BuWei.ShouBi.Value < 100f;
				if (flag8)
				{
					battleData.BuWei.ShouBi.Value = 100f;
				}
				bool flag9 = battleData.BuWei.XiongBei.Value < 100f;
				if (flag9)
				{
					battleData.BuWei.XiongBei.Value = 100f;
				}
				bool flag10 = battleData.BuWei.YaoFu.Value < 100f;
				if (flag10)
				{
					battleData.BuWei.YaoFu.Value = 100f;
				}
				bool flag11 = battleData.BuWei.TuiJiao.Value < 100f;
				if (flag11)
				{
					battleData.BuWei.TuiJiao.Value = 100f;
				}
				bool flag12 = battleData.BuWei.JinMai.Value < 100f;
				if (flag12)
				{
					battleData.BuWei.JinMai.Value = 100f;
				}
				Scripts.Log("恢复心态");
				bool flag13 = battleData.XinTai.Value < 100f;
				if (flag13)
				{
					battleData.XinTai.Value = 100f;
				}
				Scripts.Log("刷新菜单");
				UIManager.GetAcitveUIManager().UIDataUtil.RefreshJieDuanPanel();
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00009F24 File Offset: 0x00008124
		public static void SkipTime(float time)
		{
			bool flag = DataManager.Instance.GameData.GameRuntimeData.Player != null;
			if (flag)
			{
				bool flag2 = time >= 0f;
				if (flag2)
				{
					DateJieDuanManager.AddJieDuan(time);
				}
				else
				{
					QuestManager.Instance.OnJieDuanChange(time);
					DataManager.Instance.GameData.GameRuntimeData.JieDuanCount += time - 1f;
					DateJieDuanManager.AddJieDuan(1f);
				}
			}
		}

		// Token: 0x04000060 RID: 96
		private static Action<string> Log = new Action<string>(ScriptTrainer.Instance.Log);
	}
}
