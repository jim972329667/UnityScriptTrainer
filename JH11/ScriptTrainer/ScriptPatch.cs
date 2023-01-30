using GameEventType;
using HarmonyLib;
using SWS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VWVO.ConfigClass.JiNeng;
using VWVO.ConfigClass.JNUpgrade;
using VWVO.ConfigClass.WuQi;
using static AQUAS_Parameters;
using static TimeHelperComponent;
using static UnityEngine.UI.Image;
using Random = UnityEngine.Random;
using Type = System.Type;

namespace ScriptTrainer
{
    public class ScriptPatch
    {
        #region[全局参数]
        public static ValueName<bool> NieRen = new ValueName<bool>() { Value = false };
        public static string NieRenQianWindow = "";
        private static float SumTime = 0;
        #endregion

        #region[示例]
        #region 前补丁
        //Prefix 前补丁，在补丁的函数前执行

        //示例
        //[HarmonyPatch(typeof(RecyclingWells), "OnNetworkSpawn")]    RecyclingWells为类型名称，OnNetworkSpawn为函数名称
        //public class RecyclingWellsOverridePatch_OnNetworkSpawn
        //{
        //    [HarmonyPrefix]                                         前置补丁的补丁函数前的声明
        //    public static void Prefix(RecyclingWells __instance)    __instance为特殊名称，当前注入类型入口
        //    {
        //        Debug.Log($"ZG:修改前{Traverse.Create(__instance).Field("initialNumberOfUses").GetValue()}");
        //        Traverse.Create(__instance).Field("initialNumberOfUses").SetValue(99);
        //        Debug.Log($"ZG:修改后{99}");
        //    }
        //}


        #endregion

        #region 后补丁
        //Postfix后补丁，在函数执行后执行
        //[HarmonyPatch(typeof(Test), "Updata")]    Test为类型名称，Updata为函数名称
        //public class TestOverridePatch_Updata
        //{
        //    [HarmonyPostfix]                                         后置补丁的补丁函数前的声明
        //    public static void Postfix(ref int __result)    __result为特殊名称，当前函数的返回值
        //    {
        //        __result \= 2;
        //    }
        //}
        #endregion

        #region 多个同名函数补丁制作
        //[HarmonyPatch(typeof(HexMapManager), "GenerateNewMap", new Type[] { typeof(Sector) })]
        //在HexMapManager类里有多个名为GenerateNewMap的函数时，HarmonyPatch的第三个参数是函数输入变量的类型，第四个参数是函数out输出变量的类型
        #endregion

        #region 可读写属性补丁制作
        //[HarmonyPatch(typeof(MinigameChest), "Price", MethodType.Getter)]
        //[HarmonyPatch(typeof(MinigameChest), "Price", MethodType.Setter)]
        #endregion

        #region 成员修改
        //Traverse.Create(__instance).Field("initialNumberOfUses").GetValue<T>();
        //Traverse.Create(__instance).Field("initialNumberOfUses").SetValue(99);
        //initialNumberOfUses 是成员名称， T为该成员类型
        #endregion

        #endregion

        #region[暂停时间&倍率参悟属性]
        [HarmonyPatch(typeof(DateJieDuanManager), "AddJieDuan")]
        public class DateJieDuanManagerOverridePatch_AddJieDuan
        {
            [HarmonyPrefix]
            public static bool Prefix(ref float n)
            {
                if (ScriptTrainer.StopTime.Value)
                {
                    float old = DataManager.Instance.GameData.GameRuntimeData.JieDuanCount;
                    Action ResetTrigger = delegate ()
                    {
                        DataManager.Instance.GameData.GameParamData.NewYearTrigger = false;
                    };
                    Action<float> action = delegate (float N)
                    {
                        float jieDuanCount = DataManager.Instance.GameData.GameRuntimeData.JieDuanCount;
                        DataManager.Instance.GameData.GameRuntimeData.JieDuanCount += N;
                        float jieDuanCount2 = DataManager.Instance.GameData.GameRuntimeData.JieDuanCount;
                        if ((int)jieDuanCount2 - (int)jieDuanCount > 0)
                        {
                            ScriptTrainer.Instance.Log("JieDuanExecutor运行");
                            JieDuanExecutor.PushJieDuan(1f);
                            ScriptTrainer.Instance.Log("CharacterHealthManager运行");
                            CharacterHealthManager.PushJieDuan(1f);
                            ScriptTrainer.Instance.Log("JiNengUpgrader运行");
                            JiNengUpgrader.PushJieDuan(1f);
                            ScriptTrainer.Instance.Log("TrainingManager运行");
                            TrainingManager.UpdateTrainings(1f);
                            ScriptTrainer.Instance.Log("JieDuanExecutor.CharacterPushJieDuan运行");
                            JieDuanExecutor.CharacterPushJieDuan(1f);
                            ScriptTrainer.Instance.Log("JieDuanExecutor.QuestPoolPushJieDuan运行");
                            JieDuanExecutor.QuestPoolPushJieDuan(1f);
                            ActionManager_Buff.OnJieDuanChange(1f);
                            if (DataManager.Instance.GameData.GameRuntimeData.CurrentSceneName == "JHWorld")
                            {
                                GameEventSystem.Publish<StartCacheDialogue>(default(StartCacheDialogue));
                            }

                            UIManager.GetAcitveUIManager().UIDataUtil.RefreshJieDuanPanel();
                        }
                        else
                        {
                            SumTime += N;
                        }

                        if (SumTime >= 1)
                        {
                            ScriptTrainer.Instance.Log("JieDuanExecutor运行");
                            JieDuanExecutor.PushJieDuan(1f);
                            ScriptTrainer.Instance.Log("CharacterHealthManager运行");
                            CharacterHealthManager.PushJieDuan(1f);
                            ScriptTrainer.Instance.Log("JiNengUpgrader运行");
                            JiNengUpgrader.PushJieDuan(1f);
                            ScriptTrainer.Instance.Log("TrainingManager运行");
                            TrainingManager.UpdateTrainings(1f);
                            ScriptTrainer.Instance.Log("JieDuanExecutor.CharacterPushJieDuan运行");
                            JieDuanExecutor.CharacterPushJieDuan(1f);
                            ScriptTrainer.Instance.Log("JieDuanExecutor.QuestPoolPushJieDuan运行");
                            JieDuanExecutor.QuestPoolPushJieDuan(1f);
                            ActionManager_Buff.OnJieDuanChange(1f);
                            if (DataManager.Instance.GameData.GameRuntimeData.CurrentSceneName == "JHWorld")
                            {
                                GameEventSystem.Publish<StartCacheDialogue>(default(StartCacheDialogue));
                            }

                            UIManager.GetAcitveUIManager().UIDataUtil.RefreshJieDuanPanel();
                            SumTime -= 1f;
                        }

                        //QuestManager.Instance.OnJieDuanChange(N);
                        ActionManager.OnJieDuanChange(N);
                        CaravanManager.OnJieDuanChange(N);
                        ResetTrigger();
                    };
                    while (n > 1f)
                    {
                        action(1f);
                        n -= 1f;
                    }
                    if (n > 1E-45f)
                    {
                        action(n);
                    }
                    if (UIManager.ClickDialog != null)
                    {
                        Dialog clickDialog = UIManager.ClickDialog;
                        clickDialog.FinishInvoke = (Dialog.OnFinish)Delegate.Combine(clickDialog.FinishInvoke, new Dialog.OnFinish(delegate ()
                        {
                            GameEventSystem.Publish<StartCacheDialogue>(default(StartCacheDialogue));
                        }));
                    }
                    else
                    {
                        GameEventSystem.Publish<StartCacheDialogue>(default(StartCacheDialogue));
                    }
                    DataManager.Instance.GameData.GameRuntimeData.JieDuanCount = old;
                    return false;
                }
                return true;
            }

        }
        #endregion

        #region[战斗结束自动恢复内力]
        [HarmonyPatch(typeof(FightOverManager), "UnInit")]
        public class FightOverManagerOverridePatch_UnInit
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (ScriptTrainer.AutoRecoverNeiLi.Value)
                {
                    ScriptTrainer.Instance.Log($"已捕获自动恢复内力");
                    Character character = DataManager.Instance.GameData.GameRuntimeData.Player;
                    CharacterBattleData battleData = character.BattleData;
                    battleData.ZhanDouShuXing.ZhanDouNeiLi.Value = character.Deeds.NeiLiSum;
                }
            }
        }
        #endregion

        #region[武功经验倍率]
        [HarmonyPatch(typeof(DataHelp), "GetXiuLianBattle")]
        public class DataHelpOverridePatch_GetXiuLianBattle
        {
            [HarmonyPostfix]
            public static void Postfix(ref XiuLianBattle __result)
            {
                if(ScriptTrainer.MultipleExperience.Value && ScriptTrainer.MultipleExperienceRate.Value > 0)
                {
                    ScriptTrainer.Instance.Log($"内功经验获得后增加:{__result.NeiGongJingYan * (ScriptTrainer.MultipleExperienceRate.Value - 1)}");
                    __result.NeiGongJingYan *= ScriptTrainer.MultipleExperienceRate.Value;
                    ScriptTrainer.Instance.Log($"轻功经验获得后增加:{__result.QingGongJingYan * (ScriptTrainer.MultipleExperienceRate.Value - 1)}");
                    __result.QingGongJingYan *= ScriptTrainer.MultipleExperienceRate.Value;
                    ScriptTrainer.Instance.Log($"外功经验获得后增加:{__result.WuGongJingYan * (ScriptTrainer.MultipleExperienceRate.Value - 1)}");
                    __result.WuGongJingYan *= ScriptTrainer.MultipleExperienceRate.Value;
                }
            }
        }
        [HarmonyPatch(typeof(Character), "GetYangChengQingGongXiuLianSuDu")]
        public class CharacterOverridePatch_GetYangChengQingGongXiuLianSuDu
        {
            [HarmonyPostfix]
            public static void Postfix(ref float __result)
            {
                if (ScriptTrainer.MultipleExperience.Value && ScriptTrainer.MultipleExperienceRate.Value > 0)
                {
                    ScriptTrainer.Instance.Log("轻功修炼倍率获取");
                    __result = ScriptTrainer.MultipleExperienceRate.Value * (__result + 1) - 1;
                }
            }
        }
        [HarmonyPatch(typeof(Character), "GetYangChengWuGongXiuLianSuDu")]
        public class CharacterOverridePatch_GetYangChengWuGongXiuLianSuDu
        {
            [HarmonyPostfix]
            public static void Postfix(ref float __result)
            {
                if (ScriptTrainer.MultipleExperience.Value && ScriptTrainer.MultipleExperienceRate.Value > 0)
                {
                    ScriptTrainer.Instance.Log("武功修炼倍率获取");
                    __result = ScriptTrainer.MultipleExperienceRate.Value * (__result + 1) - 1;
                }
            }
        }
        [HarmonyPatch(typeof(Character), "GetYangChengNeiGongXiuLianSuDu")]
        public class CharacterOverridePatch_GetYangChengNeiGongXiuLianSuDu
        {
            [HarmonyPostfix]
            public static void Postfix(ref float __result)
            {
                if (ScriptTrainer.MultipleExperience.Value && ScriptTrainer.MultipleExperienceRate.Value > 0)
                {
                    ScriptTrainer.Instance.Log("内功修炼倍率获取");

                    __result = ScriptTrainer.MultipleExperienceRate.Value * (__result + 1) - 1;
                }
            }
        }
        //[HarmonyPatch(typeof(GuiZe), "WuGongAdd", MethodType.Getter)]
        //public class GuiZeOverridePatch_WuGongAdd
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(ref int __result)
        //    {
        //        if (MultipleExperience && MultipleExperienceRate > 0)
        //        {
        //            ScriptTrainer.Instance.Log($"外功经验获得后增加:{__result * (MultipleExperienceRate - 1)}");
        //            __result = (int)(MultipleExperienceRate * __result);
        //        }
        //    }
        //}
        //[HarmonyPatch(typeof(GuiZe), "QingGongAdd", MethodType.Getter)]
        //public class GuiZeOverridePatch_QingGongAdd
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(ref int __result)
        //    {
        //        if (MultipleExperience && MultipleExperienceRate > 0)
        //        {
        //            ScriptTrainer.Instance.Log($"轻功经验获得后增加:{__result * (MultipleExperienceRate - 1)}");
        //            __result = (int)(MultipleExperienceRate * __result);
        //        }
        //    }
        //}
        #endregion

        #region[地点好感度倍率]
        [HarmonyPatch(typeof(PlaceRelationManager), "ChangePlaceRelationValue")]
        public class PlaceRelationManagerOverridePatch_ChangePlaceRelationValue
        {
            [HarmonyPrefix]
            public static void Prefix(ref float Value)
            {
                if (ScriptTrainer.MultiplePlaceRelation.Value)
                {
                    if(Value > 0 && ScriptTrainer.MultiplePlaceRate.Value > 0)
                    {
                        Value *= ScriptTrainer.MultiplePlaceRate.Value;
                    }
                }
            }

        }
        #endregion

        #region[人物好感度倍率]
        [HarmonyPatch(typeof(RelationMananger), "InitXiangShi")]
        public class RelationManangerOverridePatch_InitXiangShi
        {
            [HarmonyPrefix]
            public static void Prefix(ref Character other)
            {
                if (other.SocialData.SocialRelation.IsXiangShiPlayer)
                {
                    return;
                }
                ScriptTrainer.Instance.Log($"额外添加初始好感度捕获");
                List<string> tianFu = DataManager.Instance.GameData.GameRuntimeData.Player.SocialData.TianFu;
                float num = 0f;

                if (!tianFu.IsNullOrEmpty<string>())
                {
                    foreach (string id in tianFu.FindAll((string x) => DataHelp.GetTianFu(x).Type == CharacterTianFuType.WanRenMi))
                    {
                        CharacterTianFu tianFu2 = DataHelp.GetTianFu(id);
                        num += tianFu2.RelationValue;
                    }
                }
                num += other.SocialData.SocialRelation.GetXiangXing().AddRelationValue(other);
                ScriptTrainer.Instance.Log($"额外添加初始好感度验证数值：{num}");
                if (ScriptTrainer.MultipleCharacterRelation.Value)
                {
                    if (num > 0 && ScriptTrainer.MultipleCharacterRate.Value - 1 > 0)
                    {
                        num *= ScriptTrainer.MultipleCharacterRate.Value - 1;
                        ScriptTrainer.Instance.Log($"额外添加初始好感度：{num}");
                        other.SocialData.SocialRelation.RelationValue += num;
                    }
                }
            }
        }
        [HarmonyPatch(typeof(RelationMananger), "ChangeRelationValue")]
        public class RelationManangerOverridePatch_ChangeRelationValue
        {
            [HarmonyPrefix]
            public static void Prefix(ref float value)
            {
                if (ScriptTrainer.MultipleCharacterRelation.Value)
                {
                    if (value > 0 && ScriptTrainer.MultipleCharacterRate.Value > 0)
                    {
                        value *= ScriptTrainer.MultipleCharacterRate.Value;
                        ScriptTrainer.Instance.Log($"好感度倍率获得后增加:{value}");
                    }
                }
            }

        }
        #endregion

        #region[参悟属性倍率]
        [HarmonyPatch(typeof(JiNengUpgrader), "PushJieDuan")]
        public class JiNengUpgraderOverridePatch_PushJieDuan
        {
            [HarmonyPrefix]
            public static bool Prefix(ref float jieduan)
            {
                if (ScriptTrainer.MultipleCanWu.Value && ScriptTrainer.MultipleCanWuRate.Value > 0)
                {
                    List<JNUpgradeCache> list = new List<JNUpgradeCache>();

                    foreach (JNUpgradeCache jNUpgradeCache in DataManager.Instance.GameData.GameRuntimeData.JNUpgradeCaches)
                    {
                        if (jNUpgradeCache.CharacterID == DataManager.Instance.GameData.GameRuntimeData.Player.ID)
                            jNUpgradeCache.CompleteJieDuan -= jieduan * ScriptTrainer.MultipleCanWuRate.Value;
                        else
                            jNUpgradeCache.CompleteJieDuan -= jieduan;

                        if (jNUpgradeCache.CompleteJieDuan <= 0f)
                        {
                            JiNengUpgrader.Upgrade(jNUpgradeCache);
                            list.Add(jNUpgradeCache);
                        }
                    }

                    foreach (JNUpgradeCache item in list)
                    {
                        DataManager.Instance.GameData.GameRuntimeData.JNUpgradeCaches.Remove(item);
                    }

                    return false;
                }
                return true;
            }

        }


        [HarmonyPatch(typeof(JiNengUpgrader), "Upgrade")]
        public class JiNengUpgraderOverridePatch_Upgrade
        {
            [HarmonyPrefix]
            public static bool Prefix(JNUpgradeCache upgradeCache)
            {
                if (ScriptTrainer.MultipleCanWuShuXing.Value && ScriptTrainer.MultipleCanWuShuXingRate.Value > 1 && upgradeCache.CharacterID == DataManager.Instance.GameData.GameRuntimeData.Player.ID)
                {
                    JiNengBase jiNengBase = DataHelp.GetJiNengBase(upgradeCache.JiNengBase);
                    JNUpgrade jnupgrade = DataHelp.GetJNUpgrade(jiNengBase.Upgrade);
                    JinXiuGuiZe guize = jnupgrade.GuiZe.First((JinXiuGuiZe x) => x.Flag == upgradeCache.flag);
                    Character character = DataHelp.GetCharacter(upgradeCache.CharacterID, false);
                    List<JiNeng> wuGongList = character.BattleData.WuGongList;
                    JiNeng jiNeng = (wuGongList != null) ? wuGongList.FirstOrDefault((JiNeng x) => x.JiNengBase == upgradeCache.JiNengBase) : null;
                    if (jiNeng == null)
                    {
                        List<JiNeng> neiGongList = character.BattleData.NeiGongList;
                        jiNeng = ((neiGongList != null) ? neiGongList.FirstOrDefault((JiNeng x) => x.JiNengBase == upgradeCache.JiNengBase) : null);
                    }
                    if (jiNeng == null)
                    {
                        List<JiNeng> qingGongList = character.BattleData.QingGongList;
                        jiNeng = ((qingGongList != null) ? qingGongList.FirstOrDefault((JiNeng x) => x.JiNengBase == upgradeCache.JiNengBase) : null);
                    }
                    if (jiNeng.JiNengUpgrades == null)
                    {
                        jiNeng.JiNengUpgrades = new List<JiNengUpgrade>();
                    }
                    if (guize.JinXiuType == "ValueChange")
                    {
                        int currentTime = jiNeng.JiNengUpgrades.Count((JiNengUpgrade x) => !Config.Instance.JNUpgrade.ValueChangeGuiZe.NoTimesFlags.Contains(x.Flag));
                        float num = 1f;
                        List<ValueChangeGuiZe> list = null;
                        switch (jiNengBase.GetProductQuality())
                        {
                            case ProductQuality.White:
                                list = Config.Instance.JNUpgrade.ValueChangeGuiZe.White;
                                break;
                            case ProductQuality.Green:
                                list = Config.Instance.JNUpgrade.ValueChangeGuiZe.Green;
                                break;
                            case ProductQuality.Blue:
                                list = Config.Instance.JNUpgrade.ValueChangeGuiZe.Blue;
                                break;
                            case ProductQuality.Gold:
                                list = Config.Instance.JNUpgrade.ValueChangeGuiZe.Gold;
                                break;
                        }
                        ValueChangeGuiZe valueChangeGuiZe = list.Find((ValueChangeGuiZe x) => Util.InRange(currentTime, x.Times) && x.Flags.Contains(guize.Flag));
                        if (valueChangeGuiZe != null)
                        {
                            num = valueChangeGuiZe.coefficient;
                        }
                        JiNengUpgrade jiNengUpgrade = new JiNengUpgrade();
                        jiNeng.JiNengUpgrades.Add(jiNengUpgrade);
                        jiNengUpgrade.JinXiuType = guize.JinXiuType;
                        jiNengUpgrade.Flag = guize.Flag;
                        jiNengUpgrade.CanWuXiaoHao = guize.CanWuXiaoHao;
                        if (guize.ValueBelong == "Battle")
                        {
                            jiNengUpgrade.RelatedID = guize.RelatedID[0];
                        }
                        else if (guize.ValueBelong == "Character")
                        {
                            string text = null;
                            if (upgradeCache.WuYi == "WuGong")
                            {
                                if (jiNengBase.WuGongType.Value == WuGongType.QuanFa)
                                {
                                    text = character.BattleData.WaiGong.QuanZhang.ID;
                                }
                                else if (jiNengBase.WuGongType.Value == WuGongType.ZhangFa)
                                {
                                    text = character.BattleData.WaiGong.QuanZhang.ID;
                                }
                                else if (jiNengBase.WuGongType.Value == WuGongType.ShouZhi)
                                {
                                    text = character.BattleData.WaiGong.ZhiLi.ID;
                                }
                                else if (jiNengBase.WuGongType.Value == WuGongType.DaoFa)
                                {
                                    text = character.BattleData.WaiGong.DaoFa.ID;
                                }
                                else if (jiNengBase.WuGongType.Value == WuGongType.QiangFa)
                                {
                                    text = character.BattleData.WaiGong.YongQiang.ID;
                                }
                                else if (jiNengBase.WuGongType.Value == WuGongType.JianFa)
                                {
                                    text = character.BattleData.WaiGong.JianFa.ID;
                                }
                                else if (jiNengBase.WuGongType.Value == WuGongType.FuFa)
                                {
                                    text = character.BattleData.WaiGong.ChuiFu.ID;
                                }
                                else if (jiNengBase.WuGongType.Value == WuGongType.ChuiFa)
                                {
                                    text = character.BattleData.WaiGong.ChuiFu.ID;
                                }
                                else if (jiNengBase.WuGongType.Value == WuGongType.GouFa)
                                {
                                    text = character.BattleData.WaiGong.GouFa.ID;
                                }
                                else if (jiNengBase.WuGongType.Value == WuGongType.BiFa)
                                {
                                    text = character.BattleData.WaiGong.BiFa.ID;
                                }
                                else if (jiNengBase.WuGongType.Value == WuGongType.GunFa)
                                {
                                    text = character.BattleData.WaiGong.ShuaGun.ID;
                                }
                                else if (jiNengBase.WuGongType.Value == WuGongType.BianFa)
                                {
                                    text = character.BattleData.WaiGong.ShiBian.ID;
                                }
                                else if (jiNengBase.WuGongType.Value == WuGongType.TeShuFa)
                                {
                                    text = character.BattleData.WaiGong.BiFa.ID;
                                }
                                else if (jiNengBase.WuGongType.Value == WuGongType.AnQiFa)
                                {
                                    text = character.BattleData.WaiGong.CunJin.ID;
                                }
                            }
                            else if (upgradeCache.WuYi == "NeiGong")
                            {
                                List<string> list2 = new List<string>();
                                if (character.BattleData.NeiGong.QiLiang.Value < (float)Config.Instance.CharacterBattle.NeiGong.QiLiang.ValueRange.Max())
                                {
                                    list2.Add(character.BattleData.NeiGong.QiLiang.ID);
                                }
                                if (character.BattleData.NeiGong.TiaoXi.Value < (float)Config.Instance.CharacterBattle.NeiGong.TiaoXi.ValueRange.Max())
                                {
                                    list2.Add(character.BattleData.NeiGong.TiaoXi.ID);
                                }
                                if (character.BattleData.NeiGong.QiFa.Value < (float)Config.Instance.CharacterBattle.NeiGong.QiFa.ValueRange.Max())
                                {
                                    list2.Add(character.BattleData.NeiGong.QiFa.ID);
                                }
                                if (character.BattleData.NeiGong.ShenXing.Value < (float)Config.Instance.CharacterBattle.NeiGong.ShenXing.ValueRange.Max())
                                {
                                    list2.Add(character.BattleData.NeiGong.ShenXing.ID);
                                }
                                if (list2.Count > 0)
                                {
                                    text = list2[Util.RandomRange(0, list2.Count)];
                                }
                            }
                            else if (upgradeCache.WuYi == "JingShen")
                            {
                                List<string> list3 = new List<string>();
                                if (character.BattleData.JingShen.YiZhi.Value < (float)Config.Instance.CharacterBattle.JingShen.YiZhi.ValueRange.Max())
                                {
                                    list3.Add(character.BattleData.JingShen.YiZhi.ID);
                                }
                                if (character.BattleData.JingShen.JiZhong.Value < (float)Config.Instance.CharacterBattle.JingShen.JiZhong.ValueRange.Max())
                                {
                                    list3.Add(character.BattleData.JingShen.JiZhong.ID);
                                }
                                if (character.BattleData.JingShen.ZhenDing.Value < (float)Config.Instance.CharacterBattle.JingShen.ZhenDing.ValueRange.Max())
                                {
                                    list3.Add(character.BattleData.JingShen.ZhenDing.ID);
                                }
                                if (character.BattleData.JingShen.JueDuan.Value < (float)Config.Instance.CharacterBattle.JingShen.JueDuan.ValueRange.Max())
                                {
                                    list3.Add(character.BattleData.JingShen.JueDuan.ID);
                                }
                                if (character.BattleData.JingShen.ShenLai.Value < (float)Config.Instance.CharacterBattle.JingShen.ShenLai.ValueRange.Max())
                                {
                                    list3.Add(character.BattleData.JingShen.ShenLai.ID);
                                }
                                if (list3.Count > 0)
                                {
                                    text = list3[Util.RandomRange(0, list3.Count)];
                                }
                            }
                            else if (upgradeCache.WuYi == "ShenTi")
                            {
                                List<string> list4 = new List<string>();
                                if (character.BattleData.WaiGong.YaoMa.Value < (float)Config.Instance.CharacterBattle.WaiGong.YaoMa.ValueRange.Max())
                                {
                                    list4.Add(character.BattleData.WaiGong.YaoMa.ID);
                                }
                                if (character.BattleData.WaiGong.XiaPan.Value < (float)Config.Instance.CharacterBattle.WaiGong.XiaPan.ValueRange.Max())
                                {
                                    list4.Add(character.BattleData.WaiGong.XiaPan.ID);
                                }
                                if (character.BattleData.WaiGong.TiPo.Value < (float)Config.Instance.CharacterBattle.WaiGong.TiPo.ValueRange.Max())
                                {
                                    list4.Add(character.BattleData.WaiGong.TiPo.ID);
                                }
                                if (character.BattleData.WaiGong.JinGu.Value < (float)Config.Instance.CharacterBattle.WaiGong.JinGu.ValueRange.Max())
                                {
                                    list4.Add(character.BattleData.WaiGong.JinGu.ID);
                                }
                                if (list4.Count > 0)
                                {
                                    text = list4[Util.RandomRange(0, list4.Count)];
                                }
                            }
                            else if (upgradeCache.WuYi == "MaiLuo")
                            {
                                List<string> list5 = new List<string>();
                                if (jiNengBase.WuGongYYShuXing.Value == YinYangType.Yang)
                                {
                                    list5.Add(character.BattleData.MaiLuo.DuMai.ID);
                                }
                                else if (jiNengBase.WuGongYYShuXing.Value == YinYangType.Yin)
                                {
                                    list5.Add(character.BattleData.MaiLuo.RenMai.ID);
                                }
                                else if (jiNengBase.WuGongYYShuXing.Value == YinYangType.YinYang)
                                {
                                    list5.Add(character.BattleData.MaiLuo.RenMai.ID);
                                    list5.Add(character.BattleData.MaiLuo.DuMai.ID);
                                }
                                if (list5.Count > 0)
                                {
                                    text = list5[Util.RandomRange(0, list5.Count)];
                                }
                            }
                            if (text != null)
                            {
                                jiNengUpgrade.RelatedID = text;
                                float value = num * guize.Value * ScriptTrainer.MultipleCanWuShuXingRate.Value;
                                ScriptTrainer.Instance.Log($"ZG:添加参悟技能：{text}  数值：{value}");
                                float num2 = CharacterDataTrace.FindFloatValue(character, DataTrace.GetHashCode(text));
                                Character.ChangeFloatValue(character, DataTrace.GetHashCode(text), (ValueOperator)Enum.Parse(typeof(ValueOperator), guize.ValueOperator), value);
                                float num3 = CharacterDataTrace.FindFloatValue(character, DataTrace.GetHashCode(text));
                                jiNengUpgrade.RealChangeValue = num3 - num2;
                            }
                        }
                        else if (guize.ValueBelong == "JiNeng")
                        {
                            jiNengUpgrade.RelatedID = guize.RelatedID[0];
                            float value2 = num * guize.Value * ScriptTrainer.MultipleCanWuShuXingRate.Value;
                            ScriptTrainer.Instance.Log($"ZG:添加参悟技能：{jiNengUpgrade.RelatedID}  数值：{value2}");
                            float num4 = JiNengDataTrace.FindFloatValue(jiNeng, jiNengUpgrade.RelatedID.GetHashCode());
                            JiNengDataTrace.CalculateValue(jiNeng, jiNengUpgrade.RelatedID.GetHashCode(), value2, (ValueOperator)Enum.Parse(typeof(ValueOperator), guize.ValueOperator));
                            float num5 = JiNengDataTrace.FindFloatValue(jiNeng, jiNengUpgrade.RelatedID.GetHashCode());
                            jiNengUpgrade.RealChangeValue = num5 - num4;
                        }
                    }
                    else if (guize.JinXiuType == "TeJiReplace")
                    {
                        JiNengUpgrade upgrade = new JiNengUpgrade();
                        upgrade.TeSe = new ZhaoShiTeSe();
                        upgrade.RelatedID = upgradeCache.zhaoshiID;
                        upgrade.JinXiuType = guize.JinXiuType;
                        upgrade.Flag = guize.Flag;
                        upgrade.CanWuXiaoHao = guize.CanWuXiaoHao;
                        ZhaoShiTeSe tese = JiNengUpgrader.GetReplaceTese(character, jiNengBase.ZhaoShis.First((ZhaoShi x) => x.ID == upgradeCache.zhaoshiID));
                        if (tese == null)
                        {
                            tese = new ZhaoShiTeSe
                            {
                                TeSeLeiXing = new ValueName<TeSeLeiXing>
                                {
                                    Value = TeSeLeiXing.None,
                                    ValueInfo = Config.Instance.I18N.Fight.Wu
                                }
                            };
                        }
                        upgradeCache.BaseTese = tese;
                        ReplaceGuiZe replaceGuiZe = guize.ReplaceGuiZe.First((ReplaceGuiZe x) => x.BaseTeSe == tese.TeSeLeiXing.Value.ToString());
                        upgrade.TeSe = (replaceGuiZe.ReplaceTeSes[upgradeCache.teseIndex].Clone() as ZhaoShiTeSe);
                        if (upgrade.TeSe.TeSeLeiXing.Value == TeSeLeiXing.BuWeiJiShang || upgrade.TeSe.TeSeLeiXing.Value == TeSeLeiXing.DianXue || upgrade.TeSe.TeSeLeiXing.Value == TeSeLeiXing.CuiDu)
                        {
                            List<string> list6 = new List<string>
                {
                    "TouMian_0000000001",
                    "ShouWan_0000000001",
                    "ShouBi_0000000001",
                    "XiongBei_0000000001",
                    "YaoFu_0000000001",
                    "TuiJiao_0000000001",
                    "JinMai_0000000001"
                };
                            if (tese.TeSeLeiXing.Value == upgrade.TeSe.TeSeLeiXing.Value)
                            {
                                foreach (string item in tese.RelatedID)
                                {
                                    list6.Remove(item);
                                }
                            }
                            upgrade.TeSe.RelatedID = new List<string>
                {
                    list6[Util.RandomRange(0, list6.Count)]
                };
                            string id = upgrade.TeSe.RelatedID[0].Split(new char[]
                            {
                    '_'
                            })[0] + "Info_0000000001";
                            upgrade.TeSe.TeSeLeiDes.Value = upgrade.TeSe.TeSeLeiDes.Value.Replace("[BuWei]", DataTrace.FindValue(character, id, false) as string);
                        }
                        if (upgrade.TeSe.TeSeLeiXing.Value == TeSeLeiXing.WuGongTypeYingXiang)
                        {
                            upgrade.TeSe.WuGongType = upgradeCache.WuGongTypes;
                            string name = Config.Instance.JiNeng.WuGongDescription.First((WuGongDescription x) => x.Type == upgrade.TeSe.WuGongType[0].ToString()).Name;
                            upgrade.TeSe.TeSeLeiXing.ValueInfo = upgrade.TeSe.TeSeLeiXing.ValueInfo.Replace("[WuGongType]", name);
                            upgrade.TeSe.TeSeLeiDes.Value = upgrade.TeSe.TeSeLeiDes.Value.Replace("[WuGongType]", name);
                        }
                        jiNeng.JiNengUpgrades.Add(upgrade);
                    }
                    else if (guize.JinXiuType == "AddCaoWei")
                    {
                        JiNengUpgrade jiNengUpgrade2 = new JiNengUpgrade();
                        jiNeng.JiNengUpgrades.Add(jiNengUpgrade2);
                        jiNengUpgrade2.RelatedID = jiNeng.JiNengBase;
                        jiNengUpgrade2.JinXiuType = guize.JinXiuType;
                        jiNengUpgrade2.Flag = guize.Flag;
                        jiNengUpgrade2.CanWuXiaoHao = guize.CanWuXiaoHao;
                    }
                    if (character.ID == DataManager.Instance.GameData.GameRuntimeData.Player.ID && jiNeng.JiNengUpgrades.Count > 0)
                    {
                        GameEventSystem.Publish<JNUpgraderEnd>(new JNUpgraderEnd
                        {
                            UpgradeCache = upgradeCache,
                            Upgrade = jiNeng.JiNengUpgrades.Last<JiNengUpgrade>()
                        });
                    }
                    return false;
                }
                return true; 
            }

        }
        #endregion

        #region[锁定BUFF]
        [HarmonyPatch(typeof(Character), "Push")]
        public class CharacterOverridePatch_Push
        {
            [HarmonyPrefix]
            public static bool Prefix(Character __instance, ref float n)
            {
                if (ScriptTrainer.InfiniteBuff.Value && __instance.ID == DataManager.Instance.GameData.GameRuntimeData.Player.ID)
                {
                    BuffManager.UpDateWineDeBuff(__instance);
                    Character.CharacterPush pushJieDuan = __instance.PushJieDuan;
                    if (pushJieDuan == null)
                    {
                        return false;
                    }
                    pushJieDuan(n);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BuffManager), "UpDateShiXueBuff")]
        public class BuffManagerOverridePatch_UpDateShiXueBuff
        {
            [HarmonyPrefix]
            public static bool Prefix(Character c)
            {
                if (ScriptTrainer.InfiniteBuff.Value && c.ID == DataManager.Instance.GameData.GameRuntimeData.Player.ID)
                {
                    return false;
                }
                return true;
            }
        }
        #endregion

        #region[战斗无冷却]
        [HarmonyPatch(typeof(JiNengDataRoute), "CalculateFightZhaoShiLengQue")]
        public class JiNengDataRouteOverridePatch_CalculateFightZhaoShiLengQue
        {
            [HarmonyPostfix]
            public static void Postfix(FightCharacter f, ZhaoShi z, ref float __result)
            {
                if (ScriptTrainer.NoCoolDown.Value)
                {
                    if (f.Character.ID == DataManager.Instance.GameData.GameRuntimeData.Player.ID)
                    {
                        __result = 0;
                    }
                } 
            }
        }
        #endregion

        #region[无修罗场事件]
        [HarmonyPatch(typeof(GanQingZhiWen_ZhuJueDuoGeQingLv), "Check")]
        public class GanQingZhiWen_ZhuJueDuoGeQingLvOverridePatch_Check
        {
            [HarmonyPrefix]
            public static bool Prefix(ref bool __result)
            {
                if (ScriptTrainer.DuoQingLv.Value)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }

        //[HarmonyPatch(typeof(UI_Relation_QiuHun), "Init")]
        //public class UI_Relation_QiuHunOverrideInit
        //{
        //    private static List<string> _relations = new List<string>();

        //    [HarmonyPrefix]
        //    public static void Prefix()
        //    {
        //        if (ScriptTrainer.DuoQingLv.Value)
        //        {
        //            Character character = DataManager.Instance.GameData.GameRuntimeData.Player;
        //            _relations = character.SocialData.SocialRelation.JieFa.Value;
        //            character.SocialData.SocialRelation.JieFa.Value.Clear();
        //        }
        //    }

        //    [HarmonyPostfix]
        //    public static void Postfix()
        //    {
        //        if (ScriptTrainer.DuoQingLv.Value)
        //        {
        //            if (!_relations.IsNullOrEmpty())
        //            {
        //                Character character = DataManager.Instance.GameData.GameRuntimeData.Player;
        //                character.SocialData.SocialRelation.JieFa.Value.InsertRange(0, _relations);
        //                foreach(var x in character.SocialData.SocialRelation.JieFa.Value)
        //                {
        //                    ScriptTrainer.Instance.Log(x);
        //                }
        //                _relations.Clear();
        //            }
        //        }
        //    }
        //}
        #endregion
        //#region 捏人
        //[HarmonyPatch(typeof(UI_NieRen_Manager), "Init")]
        //public class UI_NieRen_ManagerOverridePatch_Init
        //{
        //    [HarmonyPrefix]
        //    public static void Postfix(UI_NieRen_Manager __instance)
        //    {
        //        if (NieRen.Value)
        //        {
        //            List<UI_NieRen_Base> windows = Traverse.Create(__instance).Field("Bases").GetValue<List<UI_NieRen_Base>>();

        //            foreach (UI_NieRen_Base ui_NieRen_Base in windows)
        //            {
        //                ui_NieRen_Destroy();
        //            }
        //            windows.Clear();
        //            windows.Add(new UI_NieRen_Basic());
        //            windows.Add(new UI_NieRen_WaiMao());
        //            windows.Add(new UI_NieRen_ZongLan());
        //            foreach (UI_NieRen_Base ui_NieRen_Base2 in windows)
        //            {
        //                ui_NieRen_Base2.PreLoad();
        //            }

        //            int Currentindex = Traverse.Create(__instance).Field("Currentindex").GetValue<int>();

        //            windows[Currentindex].Init();
        //            Currentindex = 0;

        //            Transform transform = GameObject.Find("JueSe_Maker V1").transform;

        //            var nextBtn = transform.Find("Next Button").gameObject;
        //            nextBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        //            nextBtn.GetComponent<Button>().onClick.AddListener(delegate ()
        //            {
        //                nextBtn.SetActive(false);
        //                foreach (UI_NieRen_Base ui_NieRen_Base3 in windows)
        //                {
        //                    ui_NieRen_Base3.Hide();
        //                }
        //                windows[Currentindex].Init();
        //            });
        //            var zlBtn = transform.Find("ZL Button ").gameObject;
        //            zlBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        //            zlBtn.GetComponent<Button>().onClick.AddListener(delegate ()
        //            {
        //                nextBtn.SetActive(false);
        //                foreach (UI_NieRen_Base ui_NieRen_Base3 in windows)
        //                {
        //                    ui_NieRen_Base3.Hide();
        //                }
        //                windows[windows.Count - 1].Init();
        //            });

        //            Traverse.Create(__instance).Field("Bases").SetValue(windows);
        //        }
        //    }

        //}

        //[HarmonyPatch(typeof(UI_NieRen_ZongLan), "Confirm_Confirm")]
        //public class UI_NieRen_ZongLanOverridePatch_Confirm_Confirm
        //{
        //    [HarmonyPrefix]
        //    public static bool Prefix(UI_NieRen_ZongLan __instance)
        //    {
        //        if (NieRen)
        //        {
        //            DataHelp.RemoveCharacter_NieRenScene(DataManager.Instance.GameData.GameRuntimeData.Player);
        //            DataHelp.AddCharacter(DataManager.Instance.GameData.GameRuntimeData.Player);

        //            UI_NieRen_Manager.Instance.End();
        //            LoadingSceneManager.JumpScene(NieRenQianWindow, 0f);

        //            ScriptTrainer.Instance.Log("更改捏人命令成功");
        //            NieRen = false;
        //            return false;
        //        }

        //        return true;
        //    }

        //}

        //#endregion
    }
}
