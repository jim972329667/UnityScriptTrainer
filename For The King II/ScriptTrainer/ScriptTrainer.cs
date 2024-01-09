using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Windows;
using UniverseLib.Config;
using UniverseLib;
using UniverseLib.Input;
using BepInEx.Logging;

namespace ScriptTrainer
{
    [BepInPlugin("ScriptTrainer.Jim97.For_The_King_II", "为了吾王 内置修改器", "1.0.2")]
    public class ScriptTrainer: BaseUnityPlugin
    {
        public static ScriptTrainer Instance;
        public bool Initialized = false;
        // 窗口相关
        public GameObject YourTrainer;
        // 启动按键
        public static ConfigEntry<KeyCode> ShowCounter { get; set; }

        public void Awake()
        {
            Instance = this;

            #region[注入游戏补丁]
            var harmony = new Harmony(Info.Metadata.GUID);
            harmony.PatchAll();
            #endregion
        }
        public void Log(object message, LogType logType = LogType.Log)
        {
            string text = ((message != null) ? message.ToString() : null) ?? "";
            switch (logType)
            {
                case 0:
                case (LogType)4:
                    Logger.LogMessage(text);
                    break;
                case (LogType)1:
                case (LogType)3:
                    Logger.LogMessage(text);
                    break;
                case (LogType)2:
                    Logger.LogMessage(text);
                    break;
            }
        }
        public void Init()
        {
            Universe.Init(new Action(LateInit), new Action<string, LogType>(Log));
            Initialized = true;
            Debug.Log("脚本已启动");
        }
        public void LateInit()
        {
            #region 读取游戏配置
            ShowCounter = Config.Bind("修改器快捷键", "Key", KeyCode.F9);

            #endregion

            #region 注入游戏修改器UI
            YourTrainer = GameObject.Find("ZG_Trainer");
            if (YourTrainer == null)
            {
                YourTrainer = new GameObject("ZG_Trainer");
                GameObject.DontDestroyOnLoad(YourTrainer);
                YourTrainer.hideFlags = HideFlags.HideAndDontSave;
                YourTrainer.AddComponent<ZGGameObject>();
            }
            else YourTrainer.AddComponent<ZGGameObject>();
            #endregion
        }
        public void Start()
        {
            
        }

        public void Update()
        {

        }
        public void FixedUpdate()
        {
        }

        public void OnDestroy()
        {
            // 移除 MainWindow.testAssetBundle 加载时的资源
            //AssetBundle.UnloadAllAssetBundles(true);

        }
    }
    public class ZGGameObject : MonoBehaviour
    {
        public MainWindow mw;
        public void Start()
        {
            mw = new MainWindow();
        }
        public void Update()
        {
            if (!MainWindow.initialized)
            {
                MainWindow.Initialize();
            }
            if (InputManager.GetKeyDown(ScriptTrainer.ShowCounter.Value))
            {
                if (!MainWindow.initialized)
                {
                    return;
                }
                
                MainWindow.optionToggle = !MainWindow.optionToggle;
                MainWindow.canvas.SetActive(MainWindow.optionToggle);
                UnityEngine.Event.current.Use();
            }
        }
    }
  
}
