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
using BepInEx.IL2CPP.Utils.Collections;

// From UnhollowerBaseLib.dll  BepInEx/core folder
using UnhollowerRuntimeLib;

// From UnityEngine.CoreModule.dll in BepInEx\unhollowed folder
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using BepInEx.IL2CPP.UnityEngine;

using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using Il2CppSystem.Collections.Generic;
using ScriptTrainer.UI;
using System.IO;
using ScriptTrainer.Runtime;


// Also make a reference in your library to Il2Cppmscorlib.dll, from BepInEx\unhollowed folder

namespace ScriptTrainer
{
    [BepInPlugin("ScriptTrainer.Jim97.LingXu", PluginName, Version)]
    public class ScriptTrainer : BasePlugin
    {
        public const string PluginName = "枪火重生 内置修改器";
        public const string Version = "1.0.0";

        internal static new ManualLogSource Log;

        private ConfigEntry<bool> EnableTrainer;
        public static ConfigEntry<KeyCode> ShowTrainer { get; set; }
        public static ConfigEntry<float> WindowSizeFactor { get; set; }

        public GameObject Jim97_Trainer;
        public static ScriptTrainer Instance;


        public static void WriteLog(object message)
        {
            Log.LogMessage(message);
        }
        public static void WriteLog(string message, LogType type)
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

            if (!EnableTrainer.Value)
                return;

            IL2CPP_Runtime.Init();

            Harmony harmony = new Harmony("ScriptTrainer.Jim97.Harmony");
            harmony.PatchAll();

            #region[注入修改器界面]
            // IL2CPP don't automatically inherits MonoBehaviour, so needs to add a component separatelly
            ClassInjector.RegisterTypeInIl2Cpp<ZGGameObject>();
            ClassInjector.RegisterTypeInIl2Cpp<UIControls>();
            ClassInjector.RegisterTypeInIl2Cpp<TooltipGUI>();
            ClassInjector.RegisterTypeInIl2Cpp<DragAndDrog>();
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
        public ZGGameObject(IntPtr handle) : base(handle) { }

        public static ZGGameObject Instance;
        public MainWindow mw;
        Action<object> Log;

        public void Start()
        {
            Log = ScriptTrainer.Log.LogMessage;
            Instance = this;
            mw = new MainWindow();
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                if (!MainWindow.initialized)
                {
                    MainWindow.Initialize();
                }

                MainWindow.optionToggle = !MainWindow.optionToggle;
                MainWindow.canvas.SetActive(MainWindow.optionToggle);
                UnityEngine.Event.current.Use();
            }

        }
    }
}