using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ScriptTrainer.Runtime;
using ScriptTrainer.UI;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UniverseLib;
using UniverseLib.Config;
using UniverseLib.Input;


namespace ScriptTrainer
{
    [BepInPlugin("ScriptTrainer.Jim97.For_The_King_II", "为了吾王 内置修改器", "1.0.0")]
    public class ScriptTrainer: BaseUnityPlugin
    {
        public static ScriptTrainer Instance;
        // 启动按键
        #region[设置信息]
        public static ConfigEntry<KeyCode> ShowCounter { get; set; }
        public static ConfigEntry<bool> Hide_On_Startup { get; set; }
        public static ConfigEntry<float> Startup_Delay_Time{ get; set; }
        public static ConfigEntry<bool> Disable_EventSystem_Override { get; set; }
        public static ConfigEntry<bool> Force_Unlock_Mouse { get; set; }
        public string UnhollowedModulesFolder
        {
            get
            {
                return Path.Combine(Paths.BepInExRootPath, "interop");
            }
        }
        #endregion
        public void Awake()
        {
            Instance = this;
            ConfigInit();
            Universe.Init(Startup_Delay_Time.Value,new Action(LateInit), new Action<string, LogType>(Log), new UniverseLibConfig
            {
                Disable_EventSystem_Override = Disable_EventSystem_Override.Value,
                Force_Unlock_Mouse = Force_Unlock_Mouse.Value,
                Unhollowed_Modules_Folder = UnhollowedModulesFolder
            });
            ExplorerBehaviour.Setup();
            UnityCrashPrevention.Init();
            Log("脚本已启动");
        }
        public void Log(object message, LogType logType = LogType.Log)
        {
            string text = (message?.ToString()) ?? "";
            switch (logType)
            {
                case LogType.Error:
                case LogType.Exception:
                    Logger.LogError(text);
                    break;
                case LogType.Assert:
                case LogType.Log:
                    Logger.LogMessage(text);
                    break;
                case LogType.Warning:
                    Logger.LogWarning(text);
                    break;
            }
        }
        public void ConfigInit()
        {
            #region[注入游戏补丁]
            var harmony = new Harmony(Info.Metadata.GUID);
            harmony.PatchAll();
            #endregion

            ShowCounter = Config.Bind("界面信息", "ShowCounter", KeyCode.F9, "修改器快捷键");
            Hide_On_Startup = Config.Bind("界面信息", "Hide_On_Startup", true, "是否启动时隐藏修改器");
            Startup_Delay_Time = Config.Bind("界面信息", "Startup_Delay_Time", 1f, "修改器启动延迟");
            Disable_EventSystem_Override = Config.Bind("界面信息", "Disable_EventSystem_Override", false, "如果启用，修改器将不会覆盖游戏事件系统");
            Force_Unlock_Mouse = Config.Bind("界面信息", "Force_Unlock_Mouse", false, "当修改器开始时强制光标可见");
        }
        public void LateInit()
        {
            Log("开始绘制UI界面。。。");
            UIManager.InitUI();

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
    public class ExplorerBehaviour : MonoBehaviour
    {
        internal static ExplorerBehaviour Instance { get; private set; }

        internal static void Setup()
        {
            GameObject gameObject = new GameObject("ExplorerBehaviour");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            ExplorerBehaviour.Instance = gameObject.AddComponent<ExplorerBehaviour>();
        }

        internal void Update()
        {
            bool keyDown = InputManager.GetKeyDown(ScriptTrainer.ShowCounter.Value);
            if (keyDown)
            {
                UIManager.ShowMenu = !UIManager.ShowMenu;
            }
        }

        internal void OnDestroy()
        {
            this.OnApplicationQuit();
        }

        internal void OnApplicationQuit()
        {
            bool flag = this.quitting;
            if (!flag)
            {
                this.quitting = true;
                GameObject uiroot = UIManager.UIRoot;
                this.TryDestroy((uiroot != null) ? uiroot.transform.root.gameObject : null);
                this.TryDestroy((typeof(Universe).Assembly.GetType("UniverseLib.UniversalBehaviour").GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null) as Component).gameObject);
                this.TryDestroy(base.gameObject);
            }
        }

        internal void TryDestroy(GameObject obj)
        {
            try
            {
                bool flag = obj;
                if (flag)
                {
                    UnityEngine.Object.Destroy(obj);
                }
            }
            catch
            {
            }
        }

        internal bool quitting;
    }
}
