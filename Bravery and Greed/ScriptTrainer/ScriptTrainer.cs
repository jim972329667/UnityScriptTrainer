using System;
using System.Collections;

// From BepInEx.core.dll in BepInEx/core folder
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

//from 0Harmony.dll in BepInEx/core folder
using HarmonyLib;

// From BepInEx.IL2CPP.dll in BepInEx/core folder
using BepInEx.IL2CPP;
using BepInEx.IL2CPP.Utils.Collections;

// From UnhollowerBaseLib.dll  BepInEx/core folder
using UnhollowerRuntimeLib;

// From UnityEngine.CoreModule.dll in BepInEx\unhollowed folder
using UnityEngine;
using UnityEngine.SceneManagement;
using BepInEx.IL2CPP.UnityEngine;

using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using Il2CppSystem.Collections.Generic;
using NPOI.HSSF.Record.Formula.Functions;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.StandaloneInputModule;
using TMPro;
using Il2CppSystem.Reflection;

namespace ScriptTrainer
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class ScriptTrainer : BasePlugin
    {
        public const string PluginName = "英勇无厌 内置修改器";

        public const string GUID = "Jim97_Trainer";

        public const string Version = "1.0.0";

        internal static new ManualLogSource Log;
        public static ConfigEntry<KeyCode> ShowCounter { get; set; }

        public bool Loading = true;

        public GameObject Jim97_Trainer;
        public static ScriptTrainer Instance;

        public override void Load()
        {
            Log = base.Log;
            Instance = this;

            ShowCounter = Config.Bind("修改器快捷键", "Key", KeyCode.F9);

            #region[注入游戏补丁]
            var harmony = new Harmony(GUID);
            harmony.PatchAll();

            #endregion

            #region[注入修改器界面]

            ClassInjector.RegisterTypeInIl2Cpp<ZGGameObject>();
            
            // Add the monobehavior component to your personal GameObject. Try to not duplicate.
            Jim97_Trainer = GameObject.Find("Jim97_Trainer");
            if (Jim97_Trainer == null)
            {
                Jim97_Trainer = new GameObject("Jim97_Trainer");
                GameObject.DontDestroyOnLoad(Jim97_Trainer);
                Jim97_Trainer.hideFlags = HideFlags.HideAndDontSave;
                Jim97_Trainer.AddComponent<ZGGameObject>();
            }
            else Jim97_Trainer.AddComponent<ZGGameObject>();
            #endregion
        }
    }
    public class ZGGameObject : MonoBehaviour
    {
        public ZGGameObject(IntPtr handle) : base(handle) { }

        public bool Loading = true;
        public static ZGGameObject Instance;
        internal GameObject Title = null;
        Action<object> Log;
        public void Start()
        {
            Log = ScriptTrainer.Log.LogMessage;
            Instance = this;
        }
        public void Update()
        {
            if (Title == null && DebugMenu.Instance != null)
            {
                var root = DebugMenu.Instance.transform.Find("Fixed Size Panel/Button Panel/Info/Viewport");
                var title = Instantiate(root.Find("CurrentDifficulty").gameObject);
                title.GetComponent<TextMeshProUGUI>().text = $"{ScriptTrainer.PluginName} V{ScriptTrainer.Version} by:Jim97";
                title.transform.SetParent(root.transform, false);
                title.transform.localPosition = new Vector3(0, 0, -10);
                title.GetComponent<RectTransform>().sizeDelta = new Vector2(title.GetComponent<TextMeshProUGUI>().preferredWidth, title.GetComponent<TextMeshProUGUI>().preferredHeight);
            }

            if (DebugMenu.Instance.gameObject.activeSelf)
            {
                Cursor.visible = true;
            }

            if(Input.GetKeyDown(KeyCode.F9))
            {
                if (DebugMenu.Instance.gameObject.activeSelf)
                {
                    DebugMenu.Instance.gameObject.SetActive(false);
                }
                else
                {
                    DebugMenu.Instance.gameObject.SetActive(true);
                }
            }
        }
    }
}