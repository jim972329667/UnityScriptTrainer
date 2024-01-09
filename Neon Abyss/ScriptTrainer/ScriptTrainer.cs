using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using NEON.Debuger;
using NEON.Framework;
using NEON.Game.Actors;
using NEON.Game.GameModes;
using NEON.Game.LootSystem;
using NEON.Game.PowerUps;
using Rewired;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityGameUI;

namespace ScriptTrainer
{
    [BepInPlugin("ScriptTrainer.Jim97.Neon_Abyss", "霓虹深渊 内置修改器", "1.0.0")]
    public class ScriptTrainer: BaseUnityPlugin
    {
        public const string UIPath = "/Jim97Trainer/AssetBundle/Jim97TrainerUI";
        public static ScriptTrainer Instance;
        // 窗口相关
        public GameObject YourTrainer;
        // 启动按键
        public static ConfigEntry<KeyCode> ShowCounter { get; set; }
        public void Log(object target) => Logger.LogMessage(target);
        public void Awake()
        {
            Instance = this;

            #region 读取游戏配置
            ShowCounter = Config.Bind("修改器快捷键", "Key", KeyCode.F9);

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
        public static ZGGameObject Instance { get; private set; }
        //public MainWindow mw;
        public static AssetBundle guiAssetBundle;
        private static GameObject guiPrefab;
        private static GameObject guiCanvas;
        private static GameObject guiPanel;
        private static GameObject ItemPanel;
        private static DragAndDrog DragAndDrog;

        List<GameObject> Items = new List<GameObject>();
        private void LoadAsset()
        {
            if (File.Exists(Paths.PluginPath + ScriptTrainer.UIPath))
            {
                ScriptTrainer.Instance.Log("开始加载界面资源");
                guiAssetBundle = AssetBundle.LoadFromFile(Paths.PluginPath + ScriptTrainer.UIPath);
                if (guiAssetBundle != null)
                {
                    ScriptTrainer.Instance.Log("开始加载界面");
                    guiPrefab = guiAssetBundle.LoadAsset<GameObject>("TrainerCanvas");
                    if (guiPrefab != null)
                    {
                        ScriptTrainer.Instance.Log("加载界面");
                        guiCanvas = UnityEngine.Object.Instantiate<GameObject>(guiPrefab);
                        UnityEngine.Object.DontDestroyOnLoad(guiCanvas);
                        guiPanel = guiCanvas.transform.Find("TrainerPanel")?.gameObject;
                        DragAndDrog = guiPanel?.AddComponent<DragAndDrog>();
                        guiPanel.transform.Find("CloseButton").gameObject.GetComponent<Button>().onClick.AddListener(new UnityAction(this.HideTrainer));
                        guiPanel.transform.Find("Navigation").gameObject.AddComponent<TrainerNavigation>();

                        //guiPanel.transform.Find("Navigation/NavigationTab").GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener();

                        ItemPanel = guiCanvas.transform.Find("ItemPanel")?.gameObject;

                        var temp = Instantiate<GameObject>(ItemPanel);


                        ScriptTrainer.Instance.Log(guiPanel.transform.Find("Navigation/NavigationTab").GetChild(0).gameObject);
                        //bo.onClick.AddListener(new UnityAction(this.HideTrainer));
                        guiCanvas.SetActive(false);
                    }
                    else
                    {
                        ScriptTrainer.Instance.Log("加载界面失败");
                    }
                }
                else
                {
                    ScriptTrainer.Instance.Log("加载界面资源失败");
                }
            }
        }

        public void GetItemList()
        {
            foreach(var item in Items)
            {
                Destroy(item.gameObject);
            }
            Items.Clear();
            if (Global.GetDataAsset<PowerupSimpleList>("AllPowerup") != null)
            {
                GameObject parent = guiPanel.transform.Find("Navigation/NavigationPanel/Panel2/Scroll View/Viewport/Content").gameObject;

                PlayerActor player = Global.GetGameMode<ControlPlayerMode>().Player;
                PowerupSimpleList dataAsset = Global.GetDataAsset<PowerupSimpleList>("AllPowerup");
                foreach (var item in dataAsset.data)
                {
                    var temp = Instantiate<GameObject>(ItemPanel);
                    temp.transform.Find("Image").gameObject.GetComponent<Image>().sprite = item.sprite;
                    temp.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = item.GetName();
                    var tip = temp.AddComponent<TooltipGUI>();
                    tip.Initialize(item.Description);
                    temp.transform.Find("Button").gameObject.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        Global.GetService<LootManager>(false).SpawnLoot(item, player.transform.position, null, SpawnDirection.Random, 1f);
                    });
                    temp.SetActive(true);
                    temp.transform.SetParent(parent.transform);
                    Items.Add(temp);
                }
            }
        }
        public void ShowTrainer()
        {
            guiCanvas?.SetActive(true);
        }
        public void HideTrainer()
        {
            guiCanvas?.SetActive(false);
            DragAndDrog.isMouseDrag = false;
        }
        public void Start()
        {
            Instance = this;
            LoadAsset();
            //mw = new MainWindow();
        }
        public void Update()
        {
            //if (!MainWindow.initialized)
            //{
            //    MainWindow.Initialize();
            //}
            
            if (ReInput.controllers.Keyboard.GetKeyDown(ScriptTrainer.ShowCounter.Value))
            {
                //if (!MainWindow.initialized)
                //{
                //    return;
                //}
                if(Items.Count == 0)
                {
                    GetItemList();
                }
                if (guiCanvas.activeSelf)
                {
                    HideTrainer();
                }
                else
                {
                    ShowTrainer();
                }
                //MainWindow.optionToggle = !MainWindow.optionToggle;
                //MainWindow.canvas.SetActive(MainWindow.optionToggle);
                //UnityEngine.Event.current.Use();
            }
        }
    }
  
}
