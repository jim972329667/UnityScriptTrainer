using GameResources;
using HarmonyLib;
using Platforms;
using Singletons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UI;
using UI.TestingTool;
using UnityEngine;
using UnityEngine.UI;
using UserInput;
using Random = UnityEngine.Random;
using Type = System.Type;

namespace ScriptTrainer
{
    public class ScriptPatch
    {
        #region[全局参数]
        public static readonly Dictionary<string,string> TranslateDic = new Dictionary<string, string>() 
        {
            {"체력","体力"},
            {"공격력/모두","攻击力/全部"},
            {"공격력/물리","攻击力/物理"},
            {"공격력/마법","攻击力/魔法"},
            {"받는피해감소/모두","受到的伤害减少/全部"},
            {"받는피해감소/물리","受到的伤害减少/物理"},
            {"받는피해감소/마법","受到的伤害减少/魔法"},
            {"공격속도/모두","攻击速度/全部"},
            {"이동속도","移动速度"},
            {"치명타 확률","暴击率"},
            {"치명타 피해량","暴击伤害量"},
            {"회피율/모두","回避率/全部"},
            {"회피율/근접","回避率/接近"},
            {"회피율/원거리","回避率/远程"},
            {"경직저항","僵直抵抗"},
            {"넉백저항","击退抵抗"},
            {"상태이상저항/모두","状态异常抵抗/全部"},
            {"회피율/투사체","回避率/投射体"},
            {"저지력","抵抗力"},
            {"공격력/기본","攻击力/基本"},
            {"공격력/스킬","攻击力/技能"},
            {"쿨다운가속/모두","冷却加速/全部"},
            {"쿨다운가속/스킬","冷却加速/技能"},
            {"쿨다운가속/대시","冷却加速/破折号"},
            {"쿨다운가속/교대","冷却加速/换班"},
            {"쿨다운가속/정수","冷却加速/整数"},
            {"버프 지속시간","Buff持续时间"},
            {"공격속도/기본","攻击速度/基本"},
            {"공격속도/스킬","攻击速度/技能"},
            {"크기","大小"},
            {"대시거리","冲刺距离"},
            {"상태이상저항/스턴","抵抗状态异常/晕厥"},
            {"상태이상저항/빙결","状态异常抵抗/冰冻"},
            {"상태이상저항/화상","状态异常抵抗/烧伤"},
            {"상태이상저항/출혈","状态异常抵抗/出血"},
            {"상태이상저항/중독","抵抗状态异常/中毒"},
            {"상태이상/중독피해빈도감소량","状态异常/中毒伤害频率减少量"},
            {"상태이상/출혈데미지","状态异常/出血伤害"},
            {"상태이상/화상주변데미지","状态异常/火伤周边伤害"},
            {"상태이상/추가빙결지속시간","状态异常/追加冰冻持续时间"},
            {"상태이상/추가스턴지속시간","状态异常/追加晕厥持续时间"},
            {"정령/쿨다운가속","精灵/冷却加速"},
            {"공격력/투사체","攻击力/投射体"},
            {"받는 회복량","获得的恢复量"},
            {"공격속도/차지","攻击速度/占用"},
            {"피해량감소무시","无视伤害量减少"},
            { "최종","最终:"},
            { "적용","应用"},
            { "Constant","常数"},
            { "Percent","百分比"},
            { "Percent Point","百分比+"},
            {"다음 스테이지","下一阶段"},
            {"다음 맵","下一张地图"},
            {"장비 목록","物品列表"},
            {"데이터 컨트롤","数据控制"},
            {"로그 보기","查看日志"},
            {"UI 숨기기","隐藏UI"},
            {"스킬 리롤","刷新技能"},
            {"10,000 골드","10000金币"},
            {"1,000 마석","1000魔石"},
            {"100 뼛조각","100骨头碎片"},
            {"100 심장마석","100心脏魔石"},
            {"오른쪽 3개 전부","右边3个全部"},
            {"공격력 100배","攻击力100倍"},
            {"쿨다운제거","无冷却"},
            {"체력 1만","体力1万"},
            {"Stat","属性"},
            {"방어막 +10","防御膜+10"},
            {"테스트 맵","测试图"},
            {"맵 목록","地图列表"},
            {"무한부활","无限复活"},
            {"체력수치표시","显示体力数值"},
            {"신화팩\nDLC","神话包\nDLC"},
            {"하드모드","硬核模式"},
            {"단계","阶段"},
            {"클리어한 최대 레벨","通关的最大等级"},
            {"클리어한 횟수","通关次数"},
            {"각성","升级头骨"},
            {"추가 Stat, + 되는 양","附加属性，+的数量"},
            {"뒤로","后退"},
            {"뒤로가기","后退"},
            {"적 생성","创建敌人"},
            {"필드 NPC","野外NPC"},
            {"검은적","黑敌人"},
            {"드랍시 해금","掉落时解锁"},
            {"드랍시 해금해제","掉落时解除解锁"},
            {"Head","头骨"},
            {"Item","道具"},
            {"Essence","精华"},
            {"Upgrade","升级"},

            {"1회차 클리어","第一次通关"},
            {"시드 초기화","种子初始化"},
            {"데이터 초기화","初始化数据"},
            {"다크 미러","暗镜"},
            {"마왕성 방어전용\nn회 클리어","仅魔王城防御\nn通关"},
            {"랜덤 아이템 드랍","随机物品掉落"},
            {"기어 해금","齿轮解锁"},
            {"검은능력 해금","黑色能力解锁"},
            {"인벤토리 초기화","库存重置"},
            {"검은능력 해금 초기화","黑色能力解锁重置"},
            {"아이템 초기화","物品重置"},
            {"검은능력 초기화","黑色能力重置"},
            { "회 클리어","通关"},

        };
        #endregion

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


        [HarmonyPatch(typeof(PlatformManager), "cheatEnabled", MethodType.Getter)]
        public class PlatformManagerOverridePatch_cheatEnabled
        {
            [HarmonyPostfix]
            public static void Postfix(ref bool __result)
            {
                __result = true;
            }
        }

        [HarmonyPatch(typeof(Panel), "Awake")]
        public class PanelOverridePatch_Awake
        {
            [HarmonyPostfix]
            public static void Postfix(Panel __instance)
            {
                foreach (var item in __instance.gameObject.GetComponentsInChildren<TMP_Text>())
                {
                    var tmp = item.text.Trim();
                    if (TranslateDic.ContainsKey(tmp))
                    {
                        item.text = TranslateDic[tmp];
                    }
                }

                var _bonusStatPanel = Traverse.Create(__instance).Field("_bonusStatPanel").GetValue<GameObject>();
                foreach (var item in _bonusStatPanel.GetComponentsInChildren<TMP_Text>())
                {
                    var tmp = item.text.Trim();
                    if (TranslateDic.ContainsKey(tmp))
                    {
                        item.text = TranslateDic[tmp];
                    }
                    //Debug.Log(item.text);
                }

                var _mapList = Traverse.Create(__instance).Field("_mapList").GetValue<GameObject>();
                foreach (var item in _mapList.GetComponentsInChildren<TMP_Text>())
                {
                    var tmp = item.text.Trim();
                    if (TranslateDic.ContainsKey(tmp))
                    {
                        item.text = TranslateDic[tmp];
                    }
                }

                var _dataControl = Traverse.Create(__instance).Field("_dataControl").GetValue<GameObject>();
                foreach (var item in _dataControl.GetComponentsInChildren<TMP_Text>())
                {
                    var tmp = item.text.Trim();
                    if (TranslateDic.ContainsKey(tmp))
                    {
                        item.text = TranslateDic[tmp];
                    }
                    else
                        Debug.Log(item.text);
                }

                var _gearList = Traverse.Create(__instance).Field("_gearList").GetValue<GameObject>();
                foreach (var item in _gearList.GetComponentsInChildren<TMP_Text>())
                {
                    var tmp = item.text.Trim();
                    if (TranslateDic.ContainsKey(tmp))
                    {
                        item.text = TranslateDic[tmp];
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerStatElement), "Set")]
        public class PlayerStatElementOverridePatch_Set
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerStatElement __instance)
            {
                foreach ( var item in __instance.gameObject.GetComponentsInChildren<TMP_Text>())
                {
                    var tmp = item.text.Trim();
                    if (TranslateDic.ContainsKey(tmp))
                    {
                        item.text = TranslateDic[tmp];
                    }
                }
            }
        }

        [HarmonyPatch(typeof(GearList), "Awake")]
        public class GearListOverridePatch_Awake
        {
            [HarmonyPostfix]
            public static void Postfix(GearList __instance)
            {
                var _inscriptionListElements = Traverse.Create(__instance).Field("_inscriptionListElements").GetValue<List<Button>>();
                foreach(var x in _inscriptionListElements)
                {
                    var tmp = x.GetComponentInChildren<Text>().text;
                    if (tmp != "None")
                        x.GetComponentInChildren<Text>().text = "类型:" + Localization.GetLocalizedString(string.Format("synergy/key/{0}/name", tmp));
                    else
                        x.GetComponentInChildren<Text>().text = "类型:空";
                }
                //foreach(var x in Enum.GetValues(typeof(Achievement.Type)))
                //{
                //    PersistentSingleton<PlatformManager>.Instance.SetAchievement((Achievement.Type)x);
                //}
            }
        }
    }
}
