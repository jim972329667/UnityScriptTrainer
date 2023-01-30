using BepInEx.Configuration;
using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AQUAS_Parameters;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using UnityEngine;

namespace AutoSaveManager
{
    [BepInPlugin("AutoSaveManager", "江湖十一 自动存档保存", "1.0.0.0")]
    public class AutoSaveManager : BaseUnityPlugin
    {
        public static AutoSaveManager Instance;
        public static ConfigEntry<bool> AutoSave { get; set; }
        public static ConfigEntry<int> AutoSaveSlot { get; set; }
        public static ConfigEntry<int> AutoSaveTime { get; set; }


        public void Awake()
        {
            Instance = this;

            #region 读取游戏配置
            AutoSave = Config.Bind("自动存档", "Value", true);
            AutoSaveSlot = Config.Bind("自动存档栏位(最多只有三个)", "Value", 0);
            AutoSaveTime = Config.Bind("自动存档时间(分钟)", "Value", 15);
            #endregion
        }

        public void Start()
        {

        }

        public void Update()
        {
            if (AutoSave.Value)
            {
                if (DataManager.Instance.GameData.GameRuntimeData.Player != null)
                {
                    long MinuteTime = GameEventSystem.GetRuntimeLength() / 60;
                    if(MinuteTime >= AutoSaveTime.Value)
                    {
                        DataManager.Instance.GameData.GameRuntimeData.GameTime += GameEventSystem.GetRuntimeLength();
                        GameEventSystem.StartTimeCount();
                        GameObject autosave = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("UI_Prefabs/Main System/SL/Auto Save"), UIManager.GetAcitveUIManager().transform);
                        SaveDataMuiltProcess(delegate
                        {
                            Log("ZG：已完成自动存档");
                            SaveManager.AutoSave(AutoSaveSlot.Value % 3);
                        }, delegate
                        {
                            UnityEngine.Object.DestroyImmediate(autosave);
                        });
                    }
                }
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
            // 移除 MainWindow.testAssetBundle 加载时的资源
            //AssetBundle.UnloadAllAssetBundles(true);

        }
        private static void SaveDataMuiltProcess(Action on, Action after)
        {
            ThreadTask threadTask = GameEventSystem.CreateThreadTask(delegate
            {
                Action on2 = on;
                if (on2 != null)
                {
                    on2();
                }
                return true;
            });
            threadTask.Completed = after;
            threadTask.RunThreadTask();
        }
    }
}
