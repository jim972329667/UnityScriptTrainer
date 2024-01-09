using Assets.Code.Inputs;
using Assets.Code.Locale.Events;
using Assets.Code.Locale;
using Assets.Code.Utils;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UniverseLib.Config;
using UniverseLib;
using BepInEx.Logging;
using ScriptTrainer.UI;

namespace ScriptTrainer
{
    [BepInPlugin("aoe.top.plugins.ScriptTrainer", "Darkest Dungeon II 内置修改器", "1.0.0")]
    public class ScriptTrainer : BaseUnityPlugin
    {
        public static ScriptTrainer Instance;
        // 窗口相关
        public GameObject YourTrainer;
        public ManualLogSource LogSource
        {
            get
            {
                return base.Logger;
            }
        }
        // 启动按键
        public static ConfigEntry<KeyCode> ShowCounter { get; set; }
        public static ConfigEntry<float> WindowSizeFactor { get; set; }
        public void Awake()
        {
            Instance = this;

            #region[注入游戏补丁]
            var harmony = new Harmony("ScriptTrainer");
            harmony.PatchAll();
            #endregion

            #region 读取游戏配置
            ShowCounter = Config.Bind("修改器设置", "开启按键", KeyCode.F9);
            WindowSizeFactor = Config.Bind("修改器设置", "修改器尺寸", 1f);
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

            //Universe.Init(1f, new Action(LateInit), new Action<string, LogType>(Log), new UniverseLibConfig
            //{
            //    Disable_EventSystem_Override = false,
            //    Force_Unlock_Mouse = true,
            //});
            //ExplorerBehaviour.Setup();
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
        private void Log(object message, LogType logType)
        {
            string text = ((message != null) ? message.ToString() : null) ?? "";
            switch (logType)
            {
                case 0:
                case (LogType)4:
                    LogSource.LogError(text);
                    break;
                case (LogType)1:
                case (LogType)3:
                    LogSource.LogMessage(text);
                    break;
                case (LogType)2:
                    LogSource.LogWarning(text);
                    break;
            }
        }

        
        public void OnDestroy()
        {
            // 移除 MainWindow.testAssetBundle 加载时的资源
            //AssetBundle.UnloadAllAssetBundles(true);

        }
    }
    public class ZGGameObject : MonoBehaviour
    {
        public static ZGGameObject Instance;
        public static MainWindow mw;
        public bool isload = false;
        public bool isopen = false;
        public void Start()
        {
            Instance = this;
            mw = new MainWindow();
            Debug.Log("创建MainWindow");
        }
        public static void Close()
        {
            MainWindow.canvas.SetActive(false);
        }
        public static void HandleOpenZGWindow(string action, InputActionDelegateValues values)
        {
            //SingletonMonoBehaviour<CommonUiBhv>.Instance.ShowDebugCombatActorEditor();
            if (values.m_started)
            {
                Debug.Log("打开修改器");
                if (!MainWindow.initialized)
                {
                    MainWindow.Initialize();
                    DontDestroyOnLoad(mw);
                }
                bool activeSelf = MainWindow.canvas.activeSelf;
                Debug.Log(activeSelf);
                MainWindow.canvas.SetActive(!activeSelf);
                Event.current.Use();
            }
        }
        public void Update()
        {
            //if (!MainWindow.initialized)
            //{
            //    MainWindow.Initialize();
            //    Event.current.Use();
            //}
            if (!isload)
            {
                if (SingletonMonoBehaviour<InputSystemBhv>.Instance != null)
                {
                    Debug.Log(ScriptTrainer.ShowCounter.Value.ToString());
                    List<string> inputActionMapNames = SingletonMonoBehaviour<InputSystemBhv>.Instance.GetInputActionMapNames(true);
                    inputActionMapNames.AddRange(InputSystemBhv.s_defaultEnabledMapNames);
                    inputActionMapNames.RemoveAllDuplicates<string>();
                    foreach (string mapName in inputActionMapNames)
                    {
                        SingletonMonoBehaviour<InputSystemBhv>.Instance.SetInputActionMapEnabled(mapName, false);
                    }
                    SingletonMonoBehaviour<InputSystemBhv>.Instance.GenerateInputAction("OpenZGGameObject", ScriptTrainer.ShowCounter.Value.ToString(), "", "UI");
                    InputSystemBhv.AddListener("OpenZGGameObject", new InputActionDelegate(HandleOpenZGWindow));
                    Debug.Log("加载修改器chenggong");
                    isload =true;
                    foreach (string mapName2 in inputActionMapNames)
                    {
                        SingletonMonoBehaviour<InputSystemBhv>.Instance.SetInputActionMapEnabled(mapName2, true);
                    }
                }
            }
            
        }
    }
}
