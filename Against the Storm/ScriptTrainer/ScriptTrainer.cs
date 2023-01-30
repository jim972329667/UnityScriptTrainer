using BepInEx;
using BepInEx.Configuration;
using Eremite;
using Eremite.Controller;
using Eremite.Model.State;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ScriptTrainer
{
    [BepInPlugin("aoe.top.plugins.ScriptTrainer", "Against the Storm 内置修改器", "1.0.0.0")]
    public class ScriptTrainer: BaseUnityPlugin
    {
        // 窗口相关
        
        public GameObject YourTrainer;
        // 启动按键
        public static ConfigEntry<KeyCode> ShowCounter { get; set; }
        public static ConfigEntry<string> StartBonuses { get; set; }
        public static ConfigEntry<string> SaveEffects { get; set; }
        public void Awake()
        {
            #region[注入游戏补丁]
            var harmony = new Harmony("ScriptTrainer");
            harmony.PatchAll();
            #endregion

            #region 读取游戏配置
            ShowCounter = Config.Bind("修改器快捷键", "Key", KeyCode.F9);
            StartBonuses = Config.Bind("起始物品", "Key", string.Empty);
            SaveEffects = Config.Bind("保存效果", "Key", string.Empty);
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

            if (Input.GetKeyDown(ScriptTrainer.ShowCounter.Value))
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
