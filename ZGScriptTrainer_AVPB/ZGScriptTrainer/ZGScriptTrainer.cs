using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

#if IL2CPP_6E
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime.Injection;
#elif IL2CPP_6
using BepInEx.IL2CPP;
using UnhollowerRuntimeLib;
#endif

using System;
using System.IO;
using UnityEngine;
using UniverseLib;
using ZGScriptTrainer.UI;
using ZGScriptTrainer.UI.Models;


namespace ZGScriptTrainer
{
    public static class ZGBepInExInfo
    {
        public const string PLUGIN_GUID = "ScriptTrainer.Jim97.AVPB";

        public const string PLUGIN_NAME = "奥拓星球：强敌内置修改器";

        public const string PLUGIN_VERSION = "1.0.0";

        public const string PLUGIN_UPDATE_MODID = "204543";

        public const string PLUGIN_UPDATE_BASEURL = "https://mod.3dmgame.com/mod/{0}";
        public static string PLUGIN_UPDATE_URL
        {
            get
            {
                return string.Format(PLUGIN_UPDATE_BASEURL, PLUGIN_UPDATE_MODID);
            }
        }
        public static string PLUGIN_UPDATE_API
        {
            get
            {
                return string.Format(PLUGIN_UPDATE_BASEURL, "API/" + PLUGIN_UPDATE_MODID);
            }
        }
    }


    [BepInPlugin(ZGBepInExInfo.PLUGIN_GUID, ZGBepInExInfo.PLUGIN_NAME, ZGBepInExInfo.PLUGIN_VERSION)]
    public class ZGScriptTrainer :
#if MONO
        BaseUnityPlugin
#else
        BasePlugin
#endif
    {
        public static ZGScriptTrainer Instance;
        public ManualLogSource LogSource
#if MONO
            => Logger;
#else
            => Log;
#endif
        const string IL2CPP_LIBS_FOLDER =
#if UNHOLLOWER
            "unhollowed";
#else
            "interop";
#endif
        public string UnhollowedModulesFolder => Path.Combine(Paths.BepInExRootPath, IL2CPP_LIBS_FOLDER);
        public static Harmony Harmony { get; } = new Harmony(ZGBepInExInfo.PLUGIN_GUID);

        #region[配置]
        public static ConfigEntry<KeyCode> ShowCounter { get; set; }
        public static ConfigEntry<float> StartDelay { get; set; }
        public static ConfigEntry<int> FontSize { get; set; }
        public static ConfigEntry<int> FarmerExtraCarry { get; set; }
        public static ConfigEntry<int> FarmerExtraInventory { get; set; }
        public static ConfigEntry<int> FarmerExtraUpgrades { get; set; }
        public static ConfigEntry<float> FarmerMoveSpeedScale { get; set; }
        public static ConfigEntry<int> WorkerExtraCarry { get; set; }
        public static ConfigEntry<int> WorkerExtraInventory { get; set; }
        public static ConfigEntry<int> WorkerExtraUpgrades { get; set; }
        public static ConfigEntry<float> WorkerMoveSpeedScale { get; set; }
        public static ConfigEntry<int> WorkerExtraSearchRange { get; set; }
        public static ConfigEntry<int> WorkerExtraSearchDelay { get; set; }
        public static ConfigEntry<int> WorkerExtraMemory { get; set; }
        public static ConfigEntry<int> WorkerExtraEnergy { get; set; }
        public static ConfigEntry<int> StorageMult { get; set; }
        public static ConfigEntry<int> StorageSize { get; set; }
        public static ConfigEntry<int> ToolMult { get; set; }
        public static ConfigEntry<int> ToolSize { get; set; }
        public static ConfigEntry<int> WorkerExtraMovementDelay { get; set; }
        public static ConfigEntry<float> WorkerExtraMovementScale { get; set; }
        #endregion
        public static void WriteLog(object message, LogType logType = LogType.Log)
        {
            ZGScriptTrainer.Instance.ILog(message, logType);
        }
        public void ILog(object message, LogType logType = LogType.Log)
        {
            string text = (message?.ToString()) ?? "";
            switch (logType)
            {
                case LogType.Error:
                case LogType.Exception:
                    LogSource.LogError(text);
                    break;
                case LogType.Assert:
                case LogType.Log:
                    LogSource.LogMessage(text);
                    break;
                case LogType.Warning:
                    LogSource.LogWarning(text);
                    break;
            }
        }

        private void Init()
        {
            Instance = this;

            #region 读取游戏配置
            ShowCounter = Config.Bind("修改器初始设置", "快捷键", KeyCode.Home);
            StartDelay = Config.Bind("修改器初始设置", "启动延迟时间", 1f);
            FontSize = Config.Bind("修改器初始设置", "字体大小", 17);

            FarmerExtraCarry = Config.Bind("玩家修改", "额外手部格子", 5, "设置为0关闭");
            FarmerExtraInventory = Config.Bind("玩家修改", "额外背包格子", 5, "设置为0关闭");
            FarmerExtraUpgrades = Config.Bind("玩家修改", "额外升级格子", 5, "设置为0关闭");
            FarmerMoveSpeedScale = Config.Bind("玩家修改", "移动速度倍率", 1.5f, "设置为1关闭");

            WorkerExtraCarry = Config.Bind("机器人修改", "额外手部格子", 5, "设置为0关闭");
            WorkerExtraInventory = Config.Bind("机器人修改", "额外背包格子", 5, "设置为0关闭");
            WorkerExtraUpgrades = Config.Bind("机器人修改", "额外升级格子", 5, "设置为0关闭");
            WorkerMoveSpeedScale = Config.Bind("机器人修改", "移动速度倍率", 1.5f, "设置为1关闭");
            WorkerExtraSearchRange = Config.Bind("机器人修改", "额外搜索范围", 20, "设置为0关闭");
            WorkerExtraSearchDelay = Config.Bind("机器人修改", "额外搜索延迟", 5, "设置为0关闭,正常搜索延迟是10，如果设置额外延迟为5，延迟就是10 - 5 = 5");
            WorkerExtraMemory = Config.Bind("机器人修改", "额外内存", 200, "设置为0关闭");
            WorkerExtraEnergy = Config.Bind("机器人修改", "额外能量", 500, "设置为0关闭");
            WorkerExtraMovementDelay = Config.Bind("机器人修改", "额外移动延迟", 15, "设置为0关闭");
            WorkerExtraMovementScale = Config.Bind("机器人修改", "额外移动速率", 2f, "设置为0关闭");

            StorageMult = Config.Bind("储物上限拓展", "存储倍率", 100, "设置为1关闭，开启后默认关闭存储上限设置。");
            StorageSize = Config.Bind("储物上限拓展", "存储上限设置", -1, "设置为-1关闭，与存储倍率不兼容");

            ToolMult = Config.Bind("工具修改", "工具使用倍率", 100, "设置为1关闭，开启后默认关闭工具使用数量设置，包括工具，可多次使用的食物，维修数量。");
            ToolSize = Config.Bind("工具修改", "工具使用数量设置", -1, "设置为-1关闭，与工具使用倍率不兼容，包括工具，可多次使用的食物，维修数量。");
            #endregion

            #region[注入游戏补丁]
            Harmony.PatchAll();
            #endregion

            Universe.Init(StartDelay.Value, new Action(LateInit), new Action<string, LogType>(ILog), new UniverseLib.Config.UniverseLibConfig()
            {
                Force_Unlock_Mouse = true,
                Unhollowed_Modules_Folder = UnhollowedModulesFolder,
                Disable_EventSystem_Override = true
            });
            ILog("脚本已启动");
        }
        private void LateInit()
        {
            //ILog($"正在获取物品数据...");
            //ZGItemUtil.GetBaseItemData();

            ILog($"正在生成UI界面...");

            UIManager.InitUI();

            ILog($"正在注入修改器...");
            ZGTrainerBehaviour.Setup();
        }

#if MONO // Mono
        internal void Awake()
        {
            Init();
        }

#else   // Il2Cpp
        public override void Load()
        {
            ClassInjector.RegisterTypeInIl2Cpp<TooltipGUI>();
            Init();
        }
#endif
    }
}
