using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

#if IL2CPP_6E
using BepInEx.Unity.IL2CPP;
#elif IL2CPP_6
using BepInEx.IL2CPP;
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniverseLib;
using ZGScriptTrainer.ItemSpwan;
using ZGScriptTrainer.UI;

namespace ZGScriptTrainer
{
    public static class ZGBepInExInfo
    {
        public const string PLUGIN_GUID = "ScriptTrainer.Jim97.Game";

        public const string PLUGIN_NAME = "Game";

        public const string PLUGIN_VERSION = "1.0.0";

        public const string PLUGIN_TITLE = "魔法工艺内置修改器";
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

            #region[注入游戏补丁]
            Harmony.PatchAll();
            #endregion

            #region 读取游戏配置
            ShowCounter = Config.Bind("修改器初始设置", "快捷键", KeyCode.Home);
            StartDelay = Config.Bind("修改器初始设置", "启动延迟时间", 1f);
            FontSize = Config.Bind("修改器初始设置", "字体大小", 17);
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
            Init();
        }
#endif
    }
}
