using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

using System;
using UnityEngine;
using UniverseLib.Input;
using UniverseLib;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Security.Policy;

namespace ScriptTrainer
{
    [BepInPlugin("ScriptTrainer.Jim97.Magicraft", "魔法工艺 内置修改器", "1.0.6")]
    public class ScriptTrainer: BaseUnityPlugin
    {
        public static ScriptTrainer Instance;

        public bool Initialized = false;
        // 窗口相关
        public GameObject YourTrainer;
        public static Harmony harmony = null;
        // 启动按键
        public static ConfigEntry<KeyCode> ShowCounter { get; set; }

        public void Awake()
        {
            Instance = this;

            #region[注入游戏补丁]
            harmony = new Harmony(Info.Metadata.GUID);
            //harmony.PatchAll();
            #endregion

            Init();
        }
        public void Log(object message, LogType logType = LogType.Log)
        {
            string text = (message?.ToString()) ?? "";
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
            ScriptTrainer.Instance.Log("脚本已启动");
        }
        public void LateInit()
        {
            #region 读取游戏配置
            ShowCounter = Config.Bind("修改器快捷键", "Key", KeyCode.F9);

            #endregion

            LoadConfig();

            #region 注入游戏修改器UI
            YourTrainer = GameObject.Find("ZG_Trainer");
            if (YourTrainer == null)
            {
                YourTrainer = new GameObject("ZG_Trainer");
                GameObject.DontDestroyOnLoad(YourTrainer);
                YourTrainer.hideFlags = HideFlags.HideAndDontSave;
                YourTrainer.AddComponent<ZGGameObject>();
                YourTrainer.AddComponent<WebUtil>();
            }
            else YourTrainer.AddComponent<ZGGameObject>();
            #endregion

            FindGameObject();
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

        #region[FindGameObject]
        public static MonoBehaviour CmdObject { get; private set; }
        public static MethodInfo Cmd_RunDebugCmd { get; private set; }
        public static MonoBehaviour PlayerManagerObject { get; private set; }
        public static TestController TestController { get; private set; }
        public static Assembly MainAssembly {  get; private set; }

        public static void Prefix_Cmd_PrintLog(string __0)
        {
            ScriptTrainer.Instance.Log(__0);
        }
        public void FindGameObject()
        {
            // 获取所有的MonoBehaviour类
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                //Debug.Log($"ZG:{assembly.GetName().Name}");
                if (assembly.GetName().Name == "Assembly-CSharp")
                {
                    MainAssembly = assembly;
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.IsSubclassOf(typeof(MonoBehaviour)))
                        {
                            if(type.GetMethod("RunDebugCmd") != null)
                            {
                                PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
                                foreach (PropertyInfo property in properties)
                                {
                                    object value = property.GetValue(null);
                                    if (value.GetType() == type)
                                    {
                                        CmdObject = (MonoBehaviour)value;
                                        Log($"ZG:CMD已找到:{value}");
                                    }
                                }
                                Cmd_RunDebugCmd = type.GetMethod("RunDebugCmd");
                                Log($"ZG:CMD函数RunDebugCmd已找到:{Cmd_RunDebugCmd.Name}");

                                
                                if(type.GetMethod("PrintLog") != null)
                                {
                                    PatchProcessor patchProcessor = harmony.CreateProcessor(type.GetMethod("PrintLog"));
                                    patchProcessor.AddPrefix(AccessTools.Method(typeof(ScriptTrainer), "Prefix_Cmd_PrintLog", null, null));
                                    patchProcessor.Patch();
                                    Log($"ZG:CMD函数PrintLog已找到!");
                                }
                            }
                            
                            if(type.GetMethod("AllWandFullMP") != null)
                            {
                                PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
                                foreach (PropertyInfo property in properties)
                                {
                                    object value = property.GetValue(null);
                                    if (value.GetType() == type)
                                    {
                                        PlayerManagerObject = (MonoBehaviour)value;
                                        Log($"ZG:PlayerManager已找到:{value}");
                                    }
                                }
                            }

                            if (type.GetFields().Where(o => o.FieldType == typeof(TestController)).ToArray().Length > 0)
                            {
                                PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
                                foreach (PropertyInfo property in properties)
                                {
                                    object value = property.GetValue(null);
                                    if (value.GetType() == type)
                                    {
                                        var tmp = type.GetFields().Where(o => o.FieldType == typeof(TestController)).ToArray()[0];
                                        TestController = tmp.GetValue(value) as TestController;
                                        Log($"ZG:TestController已找到:{TestController}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
        public MethodInfo GetZGMethod<T>(List<string> targets, Type returnType)
        {
            foreach(var me in typeof(T).GetMethods())
            {
                if (targets.Contains(me.Name) && (returnType == null || returnType == me.ReturnType))
                {
                    return me;
                }
            }
            return null;
        }
        public static void WriteGameCmd(string cmd)
        {
            try
            {
                if (CmdObject != null)
                {
                    Cmd_RunDebugCmd?.Invoke(CmdObject, new string[] { cmd });
                }
            }
            catch (Exception e) 
            {
                Instance.Log(e, LogType.Error);
            }
        }
        public static object PlayerManagerInvoke(string method, params object[] parameters)
        {
            try
            {
                if (PlayerManagerObject != null)
                {
                    MethodInfo Method = PlayerManagerObject.GetType().GetMethod(method);
                    return Method?.Invoke(PlayerManagerObject, parameters);
                }
                return null;
            }
            catch (Exception e)
            {
                Instance.Log(e, LogType.Error);
                return null;
            }
        }
        public static object PlayerManagerInvoke(string[] methods, params object[] parameters)
        {
            try
            {
                if (PlayerManagerObject != null)
                {
                    foreach (var method in methods)
                    {
                        MethodInfo Method = PlayerManagerObject.GetType().GetMethod(method);
                        if (Method != null)
                        {
                            var tmp = Method.Invoke(PlayerManagerObject, parameters);
                            if (tmp != null)
                            {
                                return tmp;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                Instance.Log(e, LogType.Error);
                return null;
            }
        }
        public void OnDestroy()
        {
            // 移除 MainWindow.testAssetBundle 加载时的资源
            //AssetBundle.UnloadAllAssetBundles(true);

        }

        public void LoadConfig()
        {
            List<ZGItem> list = JsonConvert.DeserializeObject<List<ZGItem>>(Resources.Load<TextAsset>("Configs/" + "RelicConfig").text);
            foreach(var item in list)
            {
                item.Type = "RelicConfig";
            }
            ItemWindow.ItemDic[0] = list;
            Log($"RelicConfig : {list.Count}");

            list = JsonConvert.DeserializeObject<List<ZGItem>>(Resources.Load<TextAsset>("Configs/" + "PotionConfig").text);
            foreach (var item in list)
            {
                item.Type = "PotionConfig";
            }
            ItemWindow.ItemDic[1] = list;
            Log($"PotionConfig : {list.Count}");

            list = JsonConvert.DeserializeObject<List<ZGItem>>(Resources.Load<TextAsset>("Configs/" + "SpellConfig").text);
            foreach (var item in list)
            {
                item.Type = "SpellConfig";
            }
            ItemWindow.ItemDic[2] = list;
            Log($"SpellConfig : {list.Count}");

            list = JsonConvert.DeserializeObject<List<ZGItem>>(Resources.Load<TextAsset>("Configs/" + "WandConfig").text);
            foreach (var item in list)
            {
                item.Type = "WandConfig";
            }
            ItemWindow.ItemDic[3] = list;
            Log($"WandConfig : {list.Count}");

            list = JsonConvert.DeserializeObject<List<ZGItem>>(Resources.Load<TextAsset>("Configs/" + "CurseConfig").text);
            foreach (var item in list)
            {
                item.Type = "CurseConfig";
            }
            ItemWindow.ItemDic[4] = list;
            Log($"CurseConfig : {list.Count}");

            list = JsonConvert.DeserializeObject<List<ZGItem>>(Resources.Load<TextAsset>("Configs/" + "UnitConfig").text);
            foreach (var item in list)
            {
                item.Type = "UnitConfig";
            }
            ItemWindow.ItemDic[5] = list;
            Log($"UnitConfig : {list.Count}");
        }

        public static void WriteCmdLog()
        {

        }
    }


    public class ZGGameObject : MonoBehaviour
    {
        public MainWindow mw;
        public bool GetName = false;
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
            ItemWindow.Instance?.Update();
        }
    }
  
}
