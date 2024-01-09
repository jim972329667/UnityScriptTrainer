using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace ScriptTrainer
{
    [BepInPlugin("ScriptTrainer.Jim97.Raft", "木筏 内置修改器", "1.0.1")]
    public class ScriptTrainer: BaseUnityPlugin
    {
        public static ScriptTrainer Instance;
        // 窗口相关
        public GameObject YourTrainer;
        // 启动按键
        public static ConfigEntry<KeyCode> ShowCounter { get; set; }
        public static ConfigEntry<bool> ChangePick { get; set; }
        public static ConfigEntry<int> PickMult { get; set; }
        public static ConfigEntry<bool> ChangeStackSize { get; set; }
        public static ConfigEntry<int> StackSizeMult { get; set; }
        public static ConfigEntry<bool> ChangeToolUse { get; set; }
        public static ConfigEntry<int> ToolUseMult { get; set; }
        public static ConfigEntry<bool> ChangeSpawnSettings { get; set; }
        public static ConfigEntry<float> SpawnSettingsMult { get; set; }
        public void Awake()
        {
            Instance = this;

            #region 读取游戏配置
            ShowCounter = Config.Bind("修改器快捷键", "Key", KeyCode.F9);
            ChangePick = Config.Bind("是否开启拾取倍率", "Count", true);
            PickMult = Config.Bind("拾取倍率", "Count", 2);
            ChangeStackSize = Config.Bind("是否开启背包堆叠倍率", "Count", true);
            StackSizeMult = Config.Bind("堆叠倍率", "Count", 10);
            ChangeToolUse = Config.Bind("是否开启物品耐久倍率", "Count", true);
            ToolUseMult = Config.Bind("物品耐久倍率", "Count", 2);
            ChangeSpawnSettings = Config.Bind("是否开启物品刷新数量倍率(仅限房主)", "Count", true);
            SpawnSettingsMult = Config.Bind("物品刷新数量倍率", "Count", 2.0f);
            #endregion

            #region[注入游戏补丁]
            var harmony = new Harmony(Info.Metadata.GUID);
            harmony.PatchAll();
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
            if (Input.GetKeyDown(KeyCode.Home))
            {
                ScriptTrainer.ChangePick.Value = !ScriptTrainer.ChangePick.Value;
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
