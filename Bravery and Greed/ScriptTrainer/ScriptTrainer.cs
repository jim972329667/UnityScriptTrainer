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

// Also make a reference in your library to Il2Cppmscorlib.dll, from BepInEx\unhollowed folder

namespace ScriptTrainer
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class ScriptTrainer : BasePlugin
    {
        /// <summary>
        /// Human-readable name of the plugin. In general, it should be short and concise.
        /// This is the name that is shown to the users who run BepInEx and to modders that inspect BepInEx logs. 
        /// </summary>
        public const string PluginName = "英勇无厌 内置修改器";

        /// <summary>
        /// Unique ID of the plugin. Will be used as the default config file name.
        /// This must be a unique string that contains only characters a-z, 0-9 underscores (_) and dots (.)
        /// When creating Harmony patches or any persisting data, it's best to use this ID for easier identification.
        /// </summary>
        public const string GUID = "Jim97_Trainer";

        /// <summary>
        /// Version of the plugin. Must be in form <major>.<minor>.<build>.<revision>.
        /// Major and minor versions are mandatory, but build and revision can be left unspecified.
        /// </summary>
        public const string Version = "1.0.0";

        internal static new ManualLogSource Log;

        private ConfigEntry<bool> _exampleConfigEntry;

        /// <summary>
        /// Host your MonoBehaviour components in the same GameObject
        /// that is shared between all your projects
        /// </summary>
        public GameObject Jim97_Trainer;
        public static ScriptTrainer Instance;

        public override void Load()
        {
            Log = base.Log;
            Instance = this;

            _exampleConfigEntry = Config.Bind("通用设置",
                                              "开启修改器",
                                              true,
                                              "如果是false,这个修改器不会注入");

            if (_exampleConfigEntry.Value)
            {
                Harmony harmony = new Harmony("ScriptTrainer");
                harmony.PatchAll();
            }

            #region[注入修改器界面]
            // IL2CPP don't automatically inherits MonoBehaviour, so needs to add a component separatelly
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

        private static class Hooks
        {
            
        }
    }
    public class ZGGameObject : MonoBehaviour
    {
        public ZGGameObject(IntPtr handle) : base(handle) { }

        public static ZGGameObject Instance;
        //public MainWindow mw;
        Action<object> Log;

        public void Start()
        {
            //mw = new MainWindow();
            Log = ScriptTrainer.Log.LogMessage;
            Instance = this;
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                List<NetCharacter> players = NetGame.Instance.GetActiveCharacters();
                if (!players.IsNullOrEmpty())
                {
                    players[0].CmdDropGold(10000,0,false,true,100);
                }
            }
            if(Input.GetKeyDown(KeyCode.Keypad0))
            {
                List<NetCharacter> players = NetGame.Instance.GetActiveCharacters();
                if (DebugMenu.Instance.gameObject.activeSelf)
                {
                    DebugMenu.Instance.gameObject.SetActive(false);
                    //Menu.Instance.MainCursor.gameObject.SetActive(false);

                }
                else
                {
                    DebugMenu.Instance.gameObject.SetActive(true);
                    //Log(Compendium.Instance.Categories.NavigationOverride.HideCursor);

                    Menu.Instance.MainCursor.SetTarget(DebugMenu.Instance.GetComponent<RectTransform>(),0);
                    //Log(Menu.Instance.OptionsCategories.NavigationOverride.HideCursor);
                    //Log(LobbyMenu.Instance.OptionsScrollableCategories.NavigationOverride.HideCursor);
                    //Log(Menu.Instance.MainCursor.GetTargetPosition().ToString());
                    //Menu.Instance.MainCursor.gameObject.SetActive(false);
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                List<NetCharacter> players = NetGame.Instance.GetActiveCharacters();
                if (!players.IsNullOrEmpty())
                {
                    Vector2 local = players[0].gameObject.transform.localPosition;
                    DebugMenu.Instance.GiveGifts(players[0]);
                }
            }
        }
    }
}