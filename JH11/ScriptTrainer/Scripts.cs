using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Reflection.Emit;
using static DynamicBoneColliderBase;
using GameEventType;

namespace ScriptTrainer
{
    public class Scripts : MonoBehaviour
    {
        private static Action<object> Log = ScriptTrainer.Instance.Log;
        public static void DebugButton()
        {
            if (DataManager.Instance.GameData.GameRuntimeData.Player != null)
            {
                
                Character character = DataManager.Instance.GameData.GameRuntimeData.Player;

                foreach (Place place in DataManager.Instance.GameData.GameRuntimeData.Places)
                {
                    Log($"ZG:{place.GetPlaceRelationValue()}");
                }
                //Log("ZG:");
                //Log($"Name:{character.BasicData.Name.Value}");
                //Log($"Age:{character.BasicData.Age.Value}");
                //Log($"WuXing:{character.BasicData.WuXing.Value}");
                //Log($"WuXingInfoName:{character.BasicData.WuXingInfoName.Value}");
                //Log($"JianShi:{character.BasicData.JianShi.Value}");
                //Log($"JianShiInfoName:{character.BasicData.JianShiInfoName.Value}");
                //Log($"JiaoShe:{character.BasicData.JiaoShe.Value}");
                //Log($"JiaoSheInfoName:{character.BasicData.JiaoSheInfoName.Value}");
                //Log($"FuYuan:{character.BasicData.FuYuan.Value}");
                //Log($"FuYuanInfoName:{character.BasicData.FuYuanInfoName.Value}");
                //Log($"YiDongLi:{character.BasicData.YiDongLi.Value}");
                //Log($"YiDongLiInfoName:{character.BasicData.YiDongLiInfoName.Value}");
                //Log($"ChuShen:{character.BasicData.ChuShen.Value}");
                //Log("ChuShenValueMap:");
                //foreach(var item in character.BasicData.ChuShenValueMap)
                //{
                //    Log($"{item.Key}:{item.Value}");
                //}
            }
            else
                Log("Player == null");
        }
        public static void AddMoney(int count) 
        {
            if (DataManager.Instance.GameData.GameRuntimeData.Player != null)
            {
                DataManager.Instance.GameData.GameRuntimeData.Player.BasicData.Money.Value += count * 1000;
                CollectWindow.RefreshPanel(false);
            }
        }
        public static void AddJiangHuYueLi(float count)
        {
            if (DataManager.Instance.GameData.GameRuntimeData.Player != null)
            {
                if(-count > DataManager.Instance.GameData.GameRuntimeData.Player.BattleData.CanWuBase)
                    DataManager.Instance.GameData.GameRuntimeData.Player.BattleData.CanWuBase = 0;
                else
                    DataManager.Instance.GameData.GameRuntimeData.Player.BattleData.CanWuBase += count;
            }
        }
        public static void AddNeiLi(float count)
        {
            if (DataManager.Instance.GameData.GameRuntimeData.Player != null)
            { 
                Character player = DataManager.Instance.GameData.GameRuntimeData.Player;
                Log(player.BattleData.ZhanDouShuXing.NeiLiBase.Value);
                if (player.BattleData.JiChuShuXing.JiChuNeiLi.Value < 0)
                    player.BattleData.JiChuShuXing.JiChuNeiLi.Value = 3000;
                if (player.BattleData.ZhanDouShuXing.NeiLiBase.Value < 0)
                    player.BattleData.ZhanDouShuXing.NeiLiBase.Value = 0;

                if (-count > player.BattleData.ZhanDouShuXing.NeiLiBase.Value)
                    player.BattleData.ZhanDouShuXing.NeiLiBase.Value = 0;
                else
                    player.BattleData.ZhanDouShuXing.NeiLiBase.Value += count;

                float neili = (float)Math.Round((double)GameFormula.CalculateCharacerZanDouNeiLi(player, true), MidpointRounding.AwayFromZero);
                player.Deeds.NeiLiSum = neili;
                player.BattleData.ZhanDouShuXing.ZhanDouNeiLi.Value = neili;

                CollectWindow.RefreshPanel(false);
            }
        }
        public static void Recover()
        {
            if (DataManager.Instance.GameData.GameRuntimeData.Player != null)
            {

                Character character = DataManager.Instance.GameData.GameRuntimeData.Player;
                Log("恢复内力");
                CharacterBattleData battleData = character.BattleData;

                battleData.ZhanDouShuXing.ZhanDouNeiLi.Value = character.Deeds.NeiLiSum;
                battleData.ChuangZuoXinLi.Value = 100;
                
                if(battleData.Recover != null)
                {
                    Log("恢复中毒");
                    if (battleData.Recover.DuRecoverJieDuan != null)
                    {
                        battleData.Debuff.ZhongDus.Enable = false;
                        battleData.Recover.DuRecoverJieDuan = null;
                    }
                    Log("恢复疾病");
                    if (battleData.Recover.IllRecoverJieDuan != null)
                    {
                        Log("恢复疾病Debuff");
                        battleData.Debuff.Ills.Clear();
                        Log("删除疾病信息");
                        battleData.Recover.IllRecoverJieDuan = null;
                    }

                    if (battleData.Recover.RecoverDatas.Count > 0)
                    {
                        battleData.Recover.RecoverDatas.Clear();
                    }
                }


                Log("恢复部位");
                if (battleData.BuWei.TouMian.Value < (float)100)
                {
                    battleData.BuWei.TouMian.Value = 100;
                }
                if (battleData.BuWei.ShouWan.Value < (float)100)
                {
                    battleData.BuWei.ShouWan.Value = 100;
                }
                if (battleData.BuWei.ShouBi.Value < (float)100)
                {
                    battleData.BuWei.ShouBi.Value = 100;
                }
                if (battleData.BuWei.XiongBei.Value < (float)100)
                {
                    battleData.BuWei.XiongBei.Value = 100;
                }
                if (battleData.BuWei.YaoFu.Value < (float)100)
                {
                    battleData.BuWei.YaoFu.Value = 100;
                }
                if (battleData.BuWei.TuiJiao.Value < (float)100)
                {
                    battleData.BuWei.TuiJiao.Value = 100;
                }
                if (battleData.BuWei.JinMai.Value < (float)100)
                {
                    battleData.BuWei.JinMai.Value = 100;
                }
                Log("恢复心态");
                if (battleData.XinTai.Value < 100)
                {
                    battleData.XinTai.Value = 100;
                }
                Log("刷新菜单");
                CollectWindow.RefreshPanel(false);
            }
        }
        public static void SkipTime(float time)
        {
            if (DataManager.Instance.GameData.GameRuntimeData.Player != null)
            {
                if(time >= 0)
                    DateJieDuanManager.AddJieDuan(time);
                else
                {
                    QuestManager.Instance.OnJieDuanChange(time);
                    DataManager.Instance.GameData.GameRuntimeData.JieDuanCount += time - 1;
                    DateJieDuanManager.AddJieDuan(1);
                }
            }
        }

        public static void AddCangWuPoint(int amount)
        {
            if (DataManager.Instance.GameData.GameRuntimeData.Player != null)
            {
                CangWuShuYuanSaveData cangWuShuYuanSaveData = DataManager.Instance.GameData.GameRuntimeData.CangWuShuYuanSaveData;
                int max = Config.Instance.CangWuShuYuan.MaxPoint;

                int num = cangWuShuYuanSaveData.ObscuredTotalPoint ^ cangWuShuYuanSaveData.TotalPointKey;
                int addnum = 0;

                if (num + amount > Config.Instance.CangWuShuYuan.MaxPoint)
                {
                    num = Config.Instance.CangWuShuYuan.MaxPoint;
                    addnum = Config.Instance.CangWuShuYuan.MaxPoint - num;
                }
                else
                {
                    num += amount;
                    addnum = amount;
                }

                cangWuShuYuanSaveData.TotalPoint = num;
                cangWuShuYuanSaveData.TotalPointKey = Util.RandomRange(0, int.MaxValue - cangWuShuYuanSaveData.TotalPoint);
                cangWuShuYuanSaveData.ObscuredTotalPoint = (cangWuShuYuanSaveData.TotalPoint ^ cangWuShuYuanSaveData.TotalPointKey);

                for (int i = 0; i < addnum; i++)
                {
                    CangWuShuYuan zg = DataManager.Instance.GameData.GameBasicData.CangWuShuYuans.First<CangWuShuYuan>();
                    DataManager.Instance.GameData.GameRuntimeData.CangWuShuYuanSaveData.FinishedCangWuShuYuan.Add(zg.ID);
                }
                

                #region[修复使用过多问题]

                int count = cangWuShuYuanSaveData.GetGongFas.Count;
                Log($"ZG:已获得功法数量 :{cangWuShuYuanSaveData.GetGongFas.Count}");
                cangWuShuYuanSaveData.UsedPoint = count;
                cangWuShuYuanSaveData.UsedPointKey = Util.RandomRange(0, int.MaxValue - cangWuShuYuanSaveData.UsedPoint);
                cangWuShuYuanSaveData.ObscuredUsedPoint = (cangWuShuYuanSaveData.UsedPoint ^ cangWuShuYuanSaveData.UsedPointKey);
                #endregion

                Log($"ZG:Point :{CangWuShuYuanManager.Instance.GetPoint()}");
            }

        }

    }
}
