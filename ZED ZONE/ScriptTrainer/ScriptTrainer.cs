using System;
using System.Collections;

// From BepInEx.core.dll in BepInEx/core folder
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

//from 0Harmony.dll in BepInEx/core folder
using HarmonyLib;

// From BepInEx.IL2CPP.dll in BepInEx/core folder
using BepInEx.IL2CPP;

// From UnhollowerBaseLib.dll  BepInEx/core folder

// From UnityEngine.CoreModule.dll in BepInEx\unhollowed folder
using UnityEngine;

using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using ScriptTrainer.UI;
using Il2CppInterop.Runtime.Injection;
using BepInEx.Unity.IL2CPP;
using UnityEngine.Events;
using UnityEngine.UI;
using System.IO;
using Il2CppInterop.Runtime;
using UnityEngine.SceneManagement;
using UnityEditor;


// Also make a reference in your library to Il2Cppmscorlib.dll, from BepInEx\unhollowed folder

namespace ScriptTrainer
{
    [BepInPlugin("ScriptTrainer.Jim97.ZED_ZONE", PluginName, Version)]
    public class ScriptTrainer : BasePlugin
    {

        internal static new ManualLogSource Log;

        private ConfigEntry<bool> EnableTrainer;
        public static ConfigEntry<KeyCode> ShowTrainer { get; set; }
        public static ConfigEntry<float> WindowSizeFactor { get; set; }
        public static ConfigEntry<string> BackpackSize { get; set; }

        public GameObject Jim97_Trainer;
        public static ScriptTrainer Instance;

        public const string PluginName = "ZED ZONE 内置修改器";
        public const string Version = "1.0.2";

        public void WriteLogWithType(object message, LogType logType = LogType.Log)
        {
            string text = (message?.ToString()) ?? "";
            switch (logType)
            {
                case LogType.Error:
                case LogType.Exception:
                    Log.LogError(text);
                    break;
                case LogType.Assert:
                case LogType.Log:
                    Log.LogMessage(text);
                    break;
                case LogType.Warning:
                    Log.LogWarning(text);
                    break;
            }
        }

        [MonoPInvokeCallback]
        public static void WriteLog(string message)
        {
            Log.LogMessage(message);
        }
        public override void Load()
        {
            Log = base.Log;
            Instance = this;

            EnableTrainer = Config.Bind("通用设置", "开启修改器", true, "如果是false,这个修改器不会注入");
            ShowTrainer = Config.Bind("通用设置", "修改器快捷键", KeyCode.F9);
            WindowSizeFactor = Config.Bind("通用设置", "修改器缩放倍率", 1f, "控制修改器界面放大或缩小");
            BackpackSize = Config.Bind("修改数值设置", "背包大小", "(26,41)");

            if (!EnableTrainer.Value)
                return;

            Harmony harmony = new Harmony("ScriptTrainer.Jim97.Harmony");
            harmony.PatchAll();
            LateInit();
            //Universe.Init(1f, new Action(LateInit), new Action<string, LogType>(WriteLogWithType), new UniverseLibConfig()
            //{
            //    Unhollowed_Modules_Folder = Path.Combine(Paths.BepInExRootPath, "interop")
            //});

        }
        public void LateInit()
        {
            #region[注入修改器界面]
            // IL2CPP don't automatically inherits MonoBehaviour, so needs to add a component separatelly
            ClassInjector.RegisterTypeInIl2Cpp<ZGGameObject>();
            ClassInjector.RegisterTypeInIl2Cpp<UIControls>();
            ClassInjector.RegisterTypeInIl2Cpp<TooltipGUI>();
            ClassInjector.RegisterTypeInIl2Cpp<MainWindow>();
            //ClassInjector.RegisterTypeInIl2Cpp<ItemWindow>();
            // Add the monobehavior component to your personal GameObject. Try to not duplicate.
            Jim97_Trainer = GameObject.Find("Jim97_Trainer");
            if (Jim97_Trainer == null)
            {
                Jim97_Trainer = new GameObject("Jim97_Trainer");
                GameObject.DontDestroyOnLoad(Jim97_Trainer);
                Jim97_Trainer.hideFlags = HideFlags.HideAndDontSave;
                Jim97_Trainer.AddComponent<ZGGameObject>();
            }
            else
            {
                Jim97_Trainer.AddComponent<ZGGameObject>();
            }
            #endregion
        }
    }
    public class ZGGameObject : MonoBehaviour
    {
        public ZGGameObject(IntPtr handle) : base(handle) 
        {
            Instance = this;
            Log = ScriptTrainer.Log.LogMessage;
        }

        public static ZGGameObject Instance;
        public MainWindow mw = null;
        private GameObject UI;

        Action<object> Log;

        public void Update()
        {
            if (mw == null)
            {
                mw = this.gameObject.AddComponent<MainWindow>();
                Log($"Start MainWindow");
            }
            if (Input.GetKeyDown(KeyCode.F9) && GameController.instance.mainCanvas != null)
            {
                Log(GameController.instance.mainCanvas.renderMode);
                if (!MainWindow.initialized)
                {
                    MainWindow.Initialize();
                    Log($"MainWindow Initialized");
                }
                if (MainWindow.initialized)
                {
                    MainWindow.optionToggle = !MainWindow.optionToggle;
                    MainWindow.canvas.SetActive(MainWindow.optionToggle);
                    UnityEngine.Event.current.Use();
                }

            }
        }
    }

}