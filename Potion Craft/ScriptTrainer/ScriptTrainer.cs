﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using PotionCraft.LocalizationSystem;
using PotionCraft.ManagersSystem.Input;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using Key = UnityEngine.InputSystem.Key;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using UniverseLib;

namespace ScriptTrainer
{
    [BepInPlugin("aoe.top.plugins.ScriptTrainer", "Potion Craft 内置修改器", "1.0.1")]
    public class ScriptTrainer: BaseUnityPlugin
    {
        public static ScriptTrainer Instance;
        // 窗口相关
        public GameObject YourTrainer;
        
        // 启动按键
        public static ConfigEntry<KeyCode> ShowCounter { get; set; }
        public static ConfigEntry<float> WindowSizeFactor { get; set; }

        public float TimeScaleRate = 1f;
        public bool TimeScale = false;
        public void WriteLog(string log, LogType type)
        {
            Logger.LogMessage(log);
        }
        public void Awake()
        {
            Instance = this;

            Universe.Init(null, WriteLog);

            #region[注入游戏补丁]
            var harmony = new Harmony("ScriptTrainer");
            harmony.PatchAll();
            #endregion

            #region 读取游戏配置
            ShowCounter = Config.Bind("修改器快捷键", "Key", KeyCode.Home, "控制修改器界面开启的按键");
            WindowSizeFactor = Config.Bind("修改器缩放倍率", "Factor", 1f, "控制修改器界面放大或缩小");
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

            Debug.Log("脚本已启动");
        }

        public void Start()
        {
            
        }

        public void Update()
        {
            if (TimeScale)
            {
                Time.timeScale = TimeScaleRate;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
        public void FixedUpdate()
        {
        }

        public void WriteLog(object message)
        {
            Logger.LogMessage(message);
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

            if (UniverseLib.Input.InputManager.GetKeyDown(ScriptTrainer.ShowCounter.Value))
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
