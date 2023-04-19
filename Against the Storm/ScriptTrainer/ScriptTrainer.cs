using BepInEx;
using BepInEx.Configuration;
using Eremite;
using Eremite.Controller;
using Eremite.Model;
using Eremite.Model.State;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace ScriptTrainer
{
    [BepInPlugin("aoe.top.plugins.ScriptTrainer", "Against the Storm 内置修改器", "1.0.1")]
    public class ScriptTrainer: BaseUnityPlugin
    {
        // 窗口相关
        
        public GameObject YourTrainer;
        public static ScriptTrainer Instance;
        
        // 启动按键
        public static ConfigEntry<KeyCode> ShowTrainer { get; set; }
        public static ConfigEntry<string> StartBonuses { get; set; }
        public static ConfigEntry<string> SaveEffects { get; set; }
        
        public void Awake()
        {
            Instance = this;

            #region[注入游戏补丁]
            var harmony = new Harmony("ScriptTrainer");
            harmony.PatchAll();
            #endregion

            #region 读取游戏配置
            ShowTrainer = Config.Bind("修改器快捷键", "ShowTrainer", KeyCode.F9);
            StartBonuses = Config.Bind("起始物品", "StartBonuses", string.Empty);
            SaveEffects = Config.Bind("保存效果", "SaveEffects", string.Empty);
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
        public string GetTitle()
        {
            return $"{Info.Metadata.Name} 版本:{Info.Metadata.Version} by:Jim97";
        }
    }
    public class ZGGameObject : MonoBehaviour
    {
        public MainWindow mw;
        public static List<GoodModel> GoodModels { get; private set; } = new List<GoodModel>();
        public static List<EffectModel> EffectModels { get; private set; } = new List<EffectModel>();

        public void Start()
        {
            mw = new MainWindow();
        }
        public void Update()
        {
            if (GoodModels.Count <= 0)
            {
                try
                {
                    GoodModels = MainController.Instance.Settings.Goods.ToList<GoodModel>();
                    Debug.Log("加载物品资源成功！");
                }
                catch
                {

                }
            }
            if (EffectModels.Count <= 0)
            {
                try
                {
                    EffectModels = MainController.Instance.Settings.effects.ToList<EffectModel>();
                    Debug.Log("加载效果资源成功！");
                }
                catch
                {

                }
            }

            if (!MainWindow.initialized)
            {
                MainWindow.Initialize();
            }

            if (Input.GetKeyDown(ScriptTrainer.ShowTrainer.Value))
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
