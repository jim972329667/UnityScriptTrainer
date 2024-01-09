using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections;
using System.Xml.Linq;
using UnityEngine;

namespace ScriptTrainer
{
    [BepInPlugin("ScriptTrainer.Jim97.Skul", "小骨：英雄杀手 内置修改器", "1.0.0")]
    public class ScriptTrainer : BaseUnityPlugin
    {
        public static ScriptTrainer Instance;
        public static Harmony Harmony { get; private set; }
        // 启动按键
        public void Awake()
        {
            
        }

        public void Start()
        {
            Instance = this;
            #region[注入游戏补丁]

            Harmony = new Harmony(Instance.Info.Metadata.GUID);
            Harmony.PatchAll();

            #endregion
        }
        public static void WriteLog(object mess,LogType type = LogType.Log)
        {
            switch (type)
            {
                case LogType.Error:
                case LogType.Exception:
                    Instance.Logger.LogError(mess);
                    break;
                case LogType.Warning:
                    Instance.Logger.LogWarning(mess);
                    break;
                case LogType.Log:
                    Instance.Logger.LogInfo(mess);
                    break;
                case LogType.Assert: 
                    Instance.Logger.LogDebug(mess); 
                    break;
            }
            
        }
        public void OnDestroy()
        {
            // 移除 MainWindow.testAssetBundle 加载时的资源
            //AssetBundle.UnloadAllAssetBundles(true);

        }
    }
}
