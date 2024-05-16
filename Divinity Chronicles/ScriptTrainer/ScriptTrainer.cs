using DV;
using HarmonyLib;
using I2.Loc;
using JTW;
using ScriptTrainer.Cards;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ScriptTrainer
{
    public class ScriptTrainer: ModComponent
    {
        public static string ModCardImgPath
        {
            get
            {
                var dir = new DirectoryInfo(Path.GetDirectoryName(typeof(ScriptTrainer).Assembly.Location));
                var card = Path.Combine(dir.Parent.FullName, "Cards");
                if(!Directory.Exists(card))
                    Directory.CreateDirectory(card);
                return card;
            }
        }
        

        public static ScriptTrainer Instance;
        public static AssetBundle Asset;
        public static GameObject CardUI = null;
        // 窗口相关
        public GameObject YourTrainer;
        // 启动按
        public void Awake()
        {
            Instance = this;
            #region[注入游戏补丁]
            var harmony = new Harmony("ScriptTrainer");
            harmony.PatchAll();
            #endregion
            Debug.Log("Harmony Success!");
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
            Asset = AssetBundle.LoadFromStream(Assembly.GetAssembly(typeof(ScriptTrainer)).GetManifestResourceStream("ScriptTrainer.dynamiccardui"));
            Debug.Log($"载入Asset成功！{Asset}");
            CardUI  = Asset.LoadAsset<GameObject>("assets/dynamiccardui.prefab").transform.Find("ModCardsCanvas").gameObject;
            Debug.Log($"载入GameObject成功！{CardUI}");

            foreach (string file in Directory.GetFiles(ModCardImgPath, "*.card"))
            {
                DynamicCardCreator.AppendCardToGame(file);
            }
        }
        public byte[] StreamToByteArray(string input)
        {
            var x = Assembly.GetAssembly(typeof(ScriptTrainer)).GetManifestResourceStream(input);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                x.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        public void Update()
        {
            //if (Input.GetKeyDown(KeyCode.F8))
            //{
            //    IEnumerable<Type> enumerable = from t in Assembly.GetAssembly(typeof(ATCombat)).GetTypes()
            //                                   where t.IsSubclassOf(typeof(ATCombat))
            //                                   select t;
            //    foreach (Type type in enumerable)
            //    {
            //        try
            //        {
            //            if (!type.IsAbstract && !(type.GetConstructor(Type.EmptyTypes) == null))
            //            {
            //                ATCombat card = (ATCombat)Activator.CreateInstance(type);
            //                Debug.Log($"{type}:{card.GetDescription().Text}");
            //            }
            //        }
            //        catch 
            //        {
                        
            //        }
            //    }
            //}
            //if (Input.GetKeyDown(KeyCode.F6))
            //{
            //    IEnumerable<Type> enumerable = from t in Assembly.GetAssembly(typeof(Status)).GetTypes()
            //                                   where t.IsSubclassOf(typeof(STTurnBased)) || t.IsSubclassOf(typeof(STWithCount))
            //                                   select t;
            //    foreach (Type type in enumerable)
            //    {
            //        try
            //        {
            //            if (!type.IsAbstract && !(type.GetConstructor(Type.EmptyTypes) == null))
            //            {
            //                Status card = (Status)Activator.CreateInstance(type);
            //                Debug.Log($"{type}:{card.GetDescription(true, true)}");
            //            }
            //        }
            //        catch
            //        {

            //        }
            //    }
            //}
            //if (Input.GetKeyDown(KeyCode.F5))
            //{
            //    List<string> strings = new List<string>();
            //    IEnumerable<Type> enumerable = from t in Assembly.GetAssembly(typeof(Status)).GetTypes()
            //                                   where t.IsSubclassOf(typeof(Status))
            //                                   select t;
            //    foreach (Type type in enumerable)
            //    {
            //        try
            //        {
            //            if (!type.IsAbstract && !(type.GetConstructor(Type.EmptyTypes) == null))
            //            {
            //                Status card = (Status)Activator.CreateInstance(type);
            //                string text = "";
            //                if (type.IsSubclassOf(typeof(STTurnBased)))
            //                {
            //                    text = $"{type}:STTurnBased:{card.GetDescription(true, true).Replace("\n", "").Replace("\r", "")}";
            //                }
            //                else if (type.IsSubclassOf(typeof(STWithCount)))
            //                {
            //                    text = $"{type}:STWithCount:{card.GetDescription(true, true).Replace("\n", "").Replace("\r", "")}";
            //                }
            //                else
            //                {
            //                    text = $"{type}:{type.BaseType}:{card.GetDescription(true, true).Replace("\n", "").Replace("\r", "")}";
            //                }
            //                strings.Add(text);
            //                Debug.Log($"{type}:{card.GetDescription(true, true).Replace("\n","").Replace("\r","")}");
            //            }
            //        }
            //        catch
            //        {

            //        }
            //    }
            //    File.WriteAllLines("F:\\Status.txt", strings);
            //}
            //if (Input.GetKeyDown(KeyCode.F4))
            //{
            //    List<string> values = new List<string>();
            //    List<string> values2 = new List<string>();
            //    IEnumerable<Type> enumerable = from t in Assembly.GetAssembly(typeof(CombatAction)).GetTypes()
            //                                   where t.IsSubclassOf(typeof(CombatAction))
            //                                   select t;
            //    foreach (Type type in enumerable)
            //    {
            //        try
            //        {
            //            if (!type.IsAbstract && !(type.GetConstructor(Type.EmptyTypes) == null))
            //            {
            //                CombatAction action = (CombatAction)Activator.CreateInstance(type);
                            
            //                foreach(var x in action.ActionInternal.GetActions())
            //                {
            //                    if (x is ATAnimation x2)
            //                    {
            //                        if (!values.Contains(x2.AnimationName))
            //                        {
            //                            values.Add(x2.AnimationName);
            //                        }
            //                    }
            //                    else
            //                    {
            //                        if (!values2.Contains(x.GetType().ToString()))
            //                        {
            //                            values2.Add(x.GetType().ToString());
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        catch
            //        {

            //        }
            //    }

            //    Debug.Log("ATAnimation : " + string.Join(";", values));
            //    Debug.Log("Actions : " + string.Join(";", values2));
            //}
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
        public bool LoadEntranceSceneController = false;
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
            if (Input.GetKeyDown(KeyCode.F9))
            {
                if (!MainWindow.initialized)
                {
                    return;
                }
                
                MainWindow.optionToggle = !MainWindow.optionToggle;
                MainWindow.canvas.SetActive(MainWindow.optionToggle);
                Event.current.Use();
            }
        }
    }
  
}
