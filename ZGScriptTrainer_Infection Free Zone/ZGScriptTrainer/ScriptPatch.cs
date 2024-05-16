using Controllers.Time;
using GameInput;
using Gameplay.Mood;
using Gameplay.Units;
using Gameplay.Units.Characters;
using Gameplay.Units.Player.Workers;
using Gameplay.Units.Workers;
using Gameplay.Units.Workers.WorkSystem.Works;
using Gameplay.Vehicles;
using HarmonyLib;
using Installers;
using MapEssentials;
using Mono.CSharp;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ZGScriptTrainer.UI;
using ZGScriptTrainer.UI.Models;
using ZGScriptTrainer.UI.Panels;
using static Mono.CSharp.Parameter;

namespace ZGScriptTrainer
{
    public class ScriptPatch
    {
        #region[全局参数]
        public static VehiclesStats vehiclesStats = null;
        public static bool CanScavengeMult = false;
        public static float ScavengeMult = 2;
        public static bool CanExperienceMult = false;
        public static float ExperienceMult = 2;
        public static bool CanMaxSpeedMult = false;
        public static float MaxSpeedMult = 2;


        public static bool NoSleep = false;
        public static float ZG_Mood = 10;
        #endregion

        #region[教程]

        #region 前补丁
        //Prefix 前补丁，在补丁的函数前执行
        //RecyclingWells为类型名称，OnNetworkSpawn为函数名称
        //__instance，当前注入类型入口
        //__result，当前函数的返回值
        //__runOriginal,如果设置为 false，则不运行原始方法代码。
        //__<idx>,例如__0  函数的输入变量

        //[HarmonyPatch(typeof(RecyclingWells), "OnNetworkSpawn")]
        //public class RecyclingWellsOverridePatch_OnNetworkSpawn
        //{
        //    [HarmonyPrefix]
        //    public static void Prefix(RecyclingWells __instance)
        //    {
        //        Debug.Log($"ZG:修改前{Traverse.Create(__instance).Field("initialNumberOfUses").GetValue()}");
        //        Traverse.Create(__instance).Field("initialNumberOfUses").SetValue(99);
        //        Debug.Log($"ZG:修改后{99}");
        //    }
        //}
        #endregion

        #region 后补丁
        //Postfix后补丁，在函数执行后执行

        //[HarmonyPatch(typeof(Test), "Updata")]
        //public class TestOverridePatch_Updata
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(ref int __result)    
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

        #region 多个函数用一个补丁
        //[HarmonyPatch]
        //public class BasicVehiclePatch
        //{
        //    public static float DurabilityCostFloat = 0f;
        //    public static float Durability = 0f;
        //    public static float Energy = 0f;
        //    public static IEnumerable<MethodBase> TargetMethods()
        //    {
        //        yield return AccessTools.Method(typeof(BasicVehicle), "OnPlayerEnterRange");
        //        yield return AccessTools.Method(typeof(BasicVehicle), "GetInVehicle");
        //        yield return AccessTools.Method(typeof(BasicVehicle), "OnPlayerGetOutVehicle");
        //        yield return AccessTools.Method(typeof(BasicVehicle), "OnPlayerExitRange");
        //        yield return AccessTools.Method(typeof(BasicVehicle), "Fire_EnergyWeapon");
        //    }
        //    [HarmonyPrefix]
        //    public static void Prefix(ref BasicVehicle __instance, MethodBase __originalMethod)
        //    {
        //        ZGScriptTrainer.WriteLog($"{__originalMethod.Name}");
        //        ZGScriptTrainer.WriteLog($"{__instance.vehicleData.worldCoordinate};{__instance.currentChunk.chunkData.coordinate};{GameController.instance.gameData.playerData.characterPosition}");
        //    }
        //    [HarmonyPostfix]
        //    public static void Postfix(ref BasicVehicle __instance)
        //    {

        //    }
        //}
        #endregion

        #region 单独注入补丁

        //CreateProcessor  待补丁的原始函数
        //AddPrefix 补丁函数
        //PatchProcessor patchProcessor = harmony.CreateProcessor(TargetMethod);
        //patchProcessor.AddPrefix(AccessTools.Method(typeof(ScriptPatch), "PrefixMethod"));
        //patchProcessor.Patch();
        #endregion

        #endregion

        #region[心情修改]
        [HarmonyPatch(typeof(MoodConfig), "GetMoodModifierDraft")]
        public class MoodConfigOverridePatch_GetMoodModifierDraft
        {
            [HarmonyPrefix]
            public static bool Prefix(MoodConfig __instance, ref MoodModifierDraft __result, string modifierId)
            {
                if (modifierId == "ZG_MoodMod")
                {
                    var tmp3 = ScriptableObject.CreateInstance<MoodModifierDraft>();
                    Traverse.Create(tmp3).Field("_moodModifierId").SetValue(modifierId);
                    Traverse.Create(tmp3).Field("_durationTimeGTH").SetValue(24);
                    Traverse.Create(tmp3).Field("_initialValue").SetValue(ZG_Mood);
                    Traverse.Create(tmp3).Field("_divideByCitizensCount").SetValue(false);
                    Traverse.Create(tmp3).Field("_defaultDescriptionTranslationKey").SetValue("情绪由内置修改器提供！");
                    __result = tmp3;
                    ZGScriptTrainer.WriteLog($"MoodModifierDraft : {tmp3}");
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(MoodModifierContainer), "GetModifiersIds")]
        public class MoodModifierContainerPatch_GetModifiersIds
        {
            [HarmonyPostfix]
            public static void Postfix(ref string[] __result)
            {
                List<string> tmp = new();
                tmp.AddRange(__result);
                tmp.Add("ZG_MoodMod");
                __result = tmp.ToArray();
            }
        }
        #endregion

        [HarmonyPatch(typeof(CursorsManager), "SelectDefaultCursor")]
        public class CursorsManagerOverridePatch_SelectDefaultCursor
        {
            [HarmonyPrefix]
            public static void Prefix()
            {
                if(UIManager.WorldToolTip != null)
                {
                    UIManager.WorldToolTip.GetComponent<TooltipGUI>().EnableTooltip = false;
                }
            }
        }

        [HarmonyPatch(typeof(GameInstaller), "InitializeVehicles")]
        public class GameInstallerPatch_InitializeVehicles
        {
            [HarmonyPostfix]
            public static void Postfix(ref GameInstaller __instance)
            {
                vehiclesStats = Traverse.Create(__instance).Field("vehiclesStats").GetValue<VehiclesStats>();
            }
        }

        [HarmonyPatch(typeof(CharacterExperience), "GetCurrentScavengingSpeedModifier")]
        public class CharacterExperiencePatch_GetCurrentScavengingSpeedModifier
        {
            [HarmonyPostfix]
            public static void Postfix(ref float __result)
            {
                if (CanScavengeMult)
                {
                    __result *= ScavengeMult;
                }
            }
        }

        [HarmonyPatch(typeof(CharacterExperience), "AddExperienceForKill")]
        public class CharacterExperiencePatch_AddExperienceForKill
        {
            [HarmonyPrefix]
            public static void Prefix(ref int experienceForKill)
            {
                if (CanExperienceMult)
                {
                    experienceForKill = (int)(experienceForKill * ExperienceMult);
                }
            }
        }

        

        #region[工人无休]
        [HarmonyPatch(typeof(DefenseWork), "CustomUpdate")]
        public class DefenseWorkPatch_CustomUpdate
        {
            [HarmonyPostfix]
            public static void Postfix(ref DefenseWork __instance)
            {
                if (NoSleep)
                {
                    var tmp = typeof(DefenseWork).GetMethod("OrderToBringResources", BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (Character worker in __instance.Workers)
                    {
                        if (TimeController.IsNight())
                        {
                            tmp?.Invoke(__instance, new object[] { worker });
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Work), "IsWorkHour")]
        public class WorkOverridePatch_IsWorkHour
        {
            [HarmonyPrefix]
            public static bool Prefix(Work __instance, ref bool __result)
            {
                if (NoSleep)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }
        #endregion

        #region[移动速度修改]
        [HarmonyPatch(typeof(WorkersController), "AddWorker")]
        public class WorkersControllerPatch_AddWorker
        {
            [HarmonyPostfix]
            public static void Postfix(Character worker)
            {
                if (CanMaxSpeedMult)
                {
                    float speed = Map.ConvertKphToUps(worker.Stats.Speed);
                    worker.Movement.MaxSpeed = ScriptPatch.MaxSpeedMult * speed;
                }
            }
        }
        [HarmonyPatch(typeof(GroupBuilder), "Build")]
        public class GroupBuilderPatch_Build
        {
            [HarmonyPostfix]
            public static void Postfix(ref Group __result, GroupDraft groupDraft)
            {
                if (CanMaxSpeedMult && groupDraft.Fraction == Fraction.Player)
                {
                    foreach(var ch in __result.Characters)
                    {
                        float speed = Map.ConvertKphToUps(ch.Stats.Speed);
                        ch.Movement.MaxSpeed = ScriptPatch.MaxSpeedMult * speed;
                    }
                }
            }
        }
        #endregion
    }
}
