using BepInEx;
using BepInEx.Configuration;
using DV;
using HarmonyLib;
using JTW;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace ScriptTrainer
{
    [BepInPlugin("aoe.top.plugins.ScriptTrainer", "神州志西游 内置修改器", "1.0.0.0")]
    public class ScriptTrainer: BaseUnityPlugin
    {
        public static ScriptTrainer Instance;
        // 窗口相关
        public GameObject YourTrainer;
        // 启动按键
        public static ConfigEntry<KeyCode> ShowCounter { get; set; }
        public static ConfigEntry<int> UpgradeCardCount { get; set; }
        public static ConfigEntry<int> RemoveCardCount { get; set; }
        public static ConfigEntry<int> HandSizeCount { get; set; }
        public void Awake()
        {
            Instance = this;

            #region[注入游戏补丁]
            var harmony = new Harmony("ScriptTrainer");
            harmony.PatchAll();
            #endregion

            #region 读取游戏配置
            ShowCounter = Config.Bind("修改器快捷键", "Key", KeyCode.F9);
            UpgradeCardCount = Config.Bind("选择卡牌升级界面的选择数量", "Count", 1);
            RemoveCardCount = Config.Bind("选择卡牌移除界面的选择数量", "Count", 1);
            HandSizeCount = Config.Bind("增加最大手牌数量", "Count", 1);
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
