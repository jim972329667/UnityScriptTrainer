using System;
using System.Diagnostics;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityGameUI;
using VWVO.ConfigClass.Global;

namespace ScriptTrainer
{
    [BepInPlugin("aoe.top.plugins.ScriptTrainer", "江湖十一 内置修改器", "1.1.4")]
    public class ScriptTrainer : BaseUnityPlugin
    {
        public static ScriptTrainer Instance;

        public GameObject YourTrainer;

        #region [配置信息]
        public static ConfigEntry<KeyCode> ShowCounter { get; set; }
        public static ConfigEntry<bool> StopTime { get; set; }
        public static ConfigEntry<bool> MultipleExperience { get; set; }
        public static ConfigEntry<float> MultipleExperienceRate { get; set; }
        public static ConfigEntry<bool> MultiplePlaceRelation { get; set; }
        public static ConfigEntry<float> MultiplePlaceRate { get; set; }
        public static ConfigEntry<bool> MultipleCharacterRelation { get; set; }
        public static ConfigEntry<float> MultipleCharacterRate { get; set; }
        public static ConfigEntry<bool> MultipleCanWu { get; set; }
        public static ConfigEntry<float> MultipleCanWuRate { get; set; }
        public static ConfigEntry<bool> MultipleCanWuShuXing { get; set; }
        public static ConfigEntry<int> MultipleCanWuShuXingRate { get; set; }
        public static ConfigEntry<bool> AutoRecoverNeiLi { get; set; }
        public static ConfigEntry<bool> InfiniteBuff { get; set; }
        public static ConfigEntry<bool> NoCoolDown { get; set; }
        public static ConfigEntry<bool> DuoQingLv { get; set; }
        public static ConfigEntry<bool> TimeScale { get; set; }
        public static ConfigEntry<float> TimeScaleRate { get; set; }
        public static bool IsChangedTime = false;
        public static ConfigEntry<bool> RestartTrainer { get; set; }
        public static ConfigEntry<int> RestartTrainerTime { get; set; }
        public static ConfigEntry<float> WindowSizeFactor { get; set; }
        #endregion

        public void Awake()
        {
            ScriptTrainer.Instance = this;


            #region[读取配置文件]
            ScriptTrainer.ShowCounter = Config.Bind<KeyCode>("修改器快捷键", "Key", KeyCode.F9);
            ScriptTrainer.StopTime = Config.Bind<bool>("暂停时间", "Value", false);
            ScriptTrainer.MultipleExperience = Config.Bind<bool>("武功修炼经验", "Value", false);
            ScriptTrainer.MultipleExperienceRate = Config.Bind<float>("武功修炼经验倍率", "Value", 2f);
            ScriptTrainer.MultiplePlaceRelation = Config.Bind<bool>("地区好感度", "Value", false);
            ScriptTrainer.MultiplePlaceRate = Config.Bind<float>("地区好感度倍率", "Value", 2f);
            ScriptTrainer.MultipleCharacterRelation = Config.Bind<bool>("人物好感度", "Value", false);
            ScriptTrainer.MultipleCharacterRate = Config.Bind<float>("人物好感度倍率", "Value", 2f);
            ScriptTrainer.MultipleCanWu = Config.Bind<bool>("缩短参悟时间", "Value", false);
            ScriptTrainer.MultipleCanWuRate = Config.Bind<float>("缩短参悟时间倍率", "Value", 2f);
            ScriptTrainer.MultipleCanWuShuXing = Config.Bind<bool>("参悟获得属性", "Value", false);
            ScriptTrainer.MultipleCanWuShuXingRate = Config.Bind<int>("参悟获得属性倍率", "Value", 2);
            ScriptTrainer.NoCoolDown = Config.Bind<bool>("战斗无冷却", "Value", false);
            ScriptTrainer.InfiniteBuff = Config.Bind<bool>("锁定所有正向buff时间", "Value", false);
            ScriptTrainer.DuoQingLv = Config.Bind<bool>("修罗场时间修改", "Value", false);
            ScriptTrainer.AutoRecoverNeiLi = Config.Bind<bool>("战斗结束自动回复内力", "Value", false);
            ScriptTrainer.TimeScale = Config.Bind<bool>("游戏时间修改", "Value", false);
            ScriptTrainer.TimeScaleRate = Config.Bind<float>("游戏时间倍率", "Value", 2f);
            ScriptTrainer.TimeScale = Config.Bind<bool>("游戏时间修改", "Value", false);
            ScriptTrainer.RestartTrainerTime = Config.Bind<int>("重新载入修改器间隔(分钟)", "time", 30);
            ScriptTrainer.RestartTrainer = Config.Bind<bool>("重新载入修改器", "Value", false);
            WindowSizeFactor = Config.Bind("修改器缩放倍率", "Factor", 1f, "控制修改器界面放大或缩小");
            #endregion


            Log("开始载入补丁");
            #region[载入补丁]
            Harmony harmony = new Harmony("ScriptTrainer");
            harmony.PatchAll();
            Log("成功载入补丁");
            #endregion

            

            Log("注入界面");
            #region[注入修改器界面]

            LoadTrainer();
            
            #endregion
        }
        public void Start()
        {
        }
        public void LoadTrainer()
        {
            YourTrainer = GameObject.Find("ZG_Trainer");
            bool flag = YourTrainer == null;
            if (flag)
            {
                YourTrainer = new GameObject("ZG_Trainer");
                UnityEngine.Object.DontDestroyOnLoad(YourTrainer);
                YourTrainer.hideFlags = HideFlags.HideAndDontSave;
                YourTrainer.AddComponent<ZGGameObject>();
            }
            else
            {
                YourTrainer.AddComponent<ZGGameObject>();
            }
        }
        public void Update()
        {
            if (TimeScale.Value)
            {
                Time.timeScale = TimeScaleRate.Value;
            }
        }
        public void FixedUpdate()
        {
        }
        public void Log(object message)
        {
            Logger.LogMessage(message);
        }
        public void OnDestroy()
        {
        }
    }

    public class ZGGameObject : MonoBehaviour
    {
        public MainWindow mw;
        Action<object> Log;
        private static Stopwatch stopwatch = null;
        public void Start()
        {
            mw = new MainWindow();
            Log = ScriptTrainer.Instance.Log;

            if (stopwatch == null)
            {
                stopwatch = new Stopwatch();
                stopwatch.Start();
            }
            else
                stopwatch.Restart();
        }
        public void Update()
        {
            if (ScriptTrainer.RestartTrainer.Value)
            {
                if (stopwatch.ElapsedMilliseconds / 60000 > ScriptTrainer.RestartTrainerTime.Value && !MainWindow.optionToggle)
                {
                    Log("重新载入修改器");
                    UnityEngine.Object.Destroy(mw);
                    Start();
                    return;
                }
            }
            

            bool flag = !MainWindow.initialized;
            if (flag)
            {
                Log("修改器主页面生成");
                MainWindow.Initialize();
            }
            bool keyDown = Input.GetKeyDown(ScriptTrainer.ShowCounter.Value);
            if (keyDown)
            {
                Log($"检测到修改器按键{ScriptTrainer.ShowCounter.Value}");
                bool flag2 = !MainWindow.initialized;
                if (!flag2)
                {
                    MainWindow.optionToggle = !MainWindow.optionToggle;
                    MainWindow.canvas.SetActive(MainWindow.optionToggle);
                    Event.current.Use();
                }
            }
        }
    }
}
