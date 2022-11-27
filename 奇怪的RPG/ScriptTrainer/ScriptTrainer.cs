﻿using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using UnityEngine;

namespace ScriptTrainer
{
    [BepInPlugin("aoe.top.plugins.ScriptTrainer", "奇怪的RPG 内置修改器", "1.0.0.0")]
    public class ScriptTrainer: BaseUnityPlugin
    {
        // 窗口相关
        MainWindow mw;
        
        // 启动按键
        private ConfigEntry<BepInEx.Configuration.KeyboardShortcut> ShowCounter { get; set; }

        public void Awake()
        {
            
        }

        public void Start()
        {
            #region[注入游戏补丁]
            var harmony = new Harmony("ScriptTrainer");
            harmony.PatchAll();
            #endregion

            ShowCounter = Config.Bind("修改器快捷键", "Key", new KeyboardShortcut(KeyCode.F9));
            Debug.Log("脚本已启动");
            mw = new MainWindow();

        }

        public void Update()
        {
            if (!MainWindow.initialized)
            {
                MainWindow.Initialize();
            }

            // 切换UI开关
            //if (ShowCounter.IsDown())
            // if (new KeyboardShortcut(KeyCode.F9).IsDown())
            if (ShowCounter.Value.IsDown())
            {
                if (!MainWindow.initialized)
                {
                    return;
                }

                MainWindow.optionToggle = !MainWindow.optionToggle;
                MainWindow.canvas.SetActive(MainWindow.optionToggle);
                Event.current.Use();
            }
        }

        public void OnDestroy()
        {
            // 移除 MainWindow.testAssetBundle 加载时的资源
            AssetBundle.UnloadAllAssetBundles(true);

        }
    }
}
