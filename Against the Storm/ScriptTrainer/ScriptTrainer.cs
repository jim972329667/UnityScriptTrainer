using BepInEx;
using BepInEx.Configuration;
using Eremite;
using Eremite.Controller;
using Eremite.Model;
using Eremite.Model.Effects;
using Eremite.Model.State;
using Eremite.View.Cameras;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ScriptTrainer
{
    [BepInPlugin("aoe.top.plugins.ScriptTrainer", "Against the Storm 内置修改器", "1.0.5")]
    public class ScriptTrainer: BaseUnityPlugin
    {
        // 窗口相关
        
        public GameObject YourTrainer;
        public static ScriptTrainer Instance;
        
        // 启动按键
        public static ConfigEntry<Key> ShowTrainer { get; set; }
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
            ShowTrainer = Config.Bind("修改器快捷键", "ShowTrainer", Key.F9);
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

            
            ScriptTrainer.WriteLog("脚本已启动");
        }
        public static void WriteLog(object log)
        {
            ScriptTrainer.Instance?.Logger.LogInfo(log);
        }
        private void ILog(object mess,LogType type)
        {

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
        public static Dictionary<string, List<EffectModel>>  EffectModels { get; private set; } = new Dictionary<string, List<EffectModel>>();
        public static List<string> EffectTypes { get; private set; } =new List<string>();

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
                    ScriptTrainer.WriteLog("加载物品资源成功！");
                }
                catch
                {

                }
            }
            if (EffectModels.Count <= 0)
            {
                try
                {
                    var tmp = MainController.Instance.Settings.effects.ToList<EffectModel>();
                    foreach (EffectModel model in tmp)
                    {
                        if (!EffectModels.ContainsKey("全部"))
                        {
                            EffectModels.Add("全部", new List<EffectModel> { model });
                        }
                        else
                        {
                            EffectModels["全部"].Add(model);
                        }

                        if (model.IsPerk)
                        {
                            if (!EffectModels.ContainsKey("技能"))
                            {
                                EffectModels.Add("技能", new List<EffectModel> { model });
                            }
                            else
                            {
                                EffectModels["技能"].Add(model);
                            }
                        }
                        if (model.IsPositive)
                        {
                            if (!EffectModels.ContainsKey("正面的"))
                            {
                                EffectModels.Add("正面的", new List<EffectModel> { model });
                            }
                            else
                            {
                                EffectModels["正面的"].Add(model);
                            }
                        }

                        if(!model.IsPerk && !model.IsPositive && !(model is ReplaceBuildingEffectModel) && !(model is CloningEffectModel))
                        {
                            if (!EffectModels.ContainsKey("其他"))
                            {
                                EffectModels.Add("其他", new List<EffectModel> { model });
                            }
                            else
                            {
                                EffectModels["其他"].Add(model);
                            }
                        }
                    }
                    ScriptTrainer.WriteLog("加载效果资源成功！");
                }
                catch
                {

                }
            }

            if (!MainWindow.initialized)
            {
                MainWindow.Initialize();
            }
            if (EffectModels.Count > 0 && GoodModels.Count > 0 && !EffectWindows.Initialized)
            {
                EffectWindows.Instance.Initialize();
            }
            if (GameController.Instance != null && GameController.Instance.CameraController)
            {
                //Traverse.Create(GameController.Instance.CameraController).Field("zoomSeed").SetValue(400f);
                GameController.Instance.CameraController.ZoomLimit = new Vector2(-100f, -8f);
            }
            if (GetKeyDown(ScriptTrainer.ShowTrainer.Value))
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

        public bool GetKeyDown(Key keyboardKey)
        {
            return Keyboard.current != null && Keyboard.current[keyboardKey].wasPressedThisFrame;
        }
    }
}
