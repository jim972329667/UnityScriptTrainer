﻿using System.Reflection;
using UnityEngine;
using UniverseLib.Input;
using UniverseLib;
using ZGScriptTrainer.ItemSpwan;
using ZGScriptTrainer.UI.Panels;
using ZGScriptTrainer.UI;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;


#if IL2CPP_6E
using Il2CppInterop.Runtime.Injection;
#elif IL2CPP_6
using UnhollowerRuntimeLib;
#endif

namespace ZGScriptTrainer
{
    public class ZGTrainerBehaviour : MonoBehaviour
    {
        internal static ZGTrainerBehaviour Instance { get; private set; }
        public static Dictionary<string, Action> Actions { get; set; } = new Dictionary<string, Action>();

#if CPP
        public ZGTrainerBehaviour(System.IntPtr ptr) : base(ptr) { }
#endif

        internal static void Setup()
        {
#if CPP
            ClassInjector.RegisterTypeInIl2Cpp<ZGTrainerBehaviour>();
#endif
            GameObject obj = new GameObject("ZGTrainerBehaviour");
            DontDestroyOnLoad(obj);
            obj.hideFlags = HideFlags.HideAndDontSave;
            Instance = obj.AddComponent<ZGTrainerBehaviour>();
            RuntimeHelper.StartCoroutine(GetText());
        }
        internal void Update()
        {
            //if (!ItemWindow.Instance.Initialized)
            //{
            //    if (ZGItemUtil.CanGetItemData())
            //    {
            //        ZGItemUtil.GetBaseItemData();
            //        ItemWindow.Instance.Initialize();
            //    }
            //}
            if (InputManager.GetKeyDown(ZGScriptTrainer.ShowCounter.Value))
            {
                ZGUIManager.ShowMenu = !ZGUIManager.ShowMenu;
            }
            foreach(var action in Actions)
            {
                action.Value.Invoke();
            }
        }
        internal void OnDestroy()
        {
            OnApplicationQuit();
        }

        internal bool quitting;
        internal void OnApplicationQuit()
        {
            if (quitting) return;
            quitting = true;

            TryDestroy(ZGUIManager.UIRoot?.transform.root.gameObject);

            TryDestroy((typeof(Universe).Assembly.GetType("UniverseLib.UniversalBehaviour")
                .GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(null, null)
                as Component).gameObject);

            TryDestroy(this.gameObject);
        }
        internal void TryDestroy(GameObject obj)
        {
            try
            {
                if (obj)
                    Destroy(obj);
            }
            catch { }
        }
        private static IEnumerator GetText()
        {
            UnityWebRequest www = UnityWebRequest.Get(ZGBepInExInfo.PLUGIN_UPDATE_API);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                ZGScriptTrainer.WriteLog(www.error);
            }
            else
            {
                // 以文本形式显示结果
                //ScriptTrainer.Instance.Log(www.downloadHandler.text);
                if (www.downloadHandler.text.Contains("\"mods_version\":\""))
                {
                    int index = www.downloadHandler.text.IndexOf("\"mods_version\":\"") + 16;
                    int endindex = www.downloadHandler.text.IndexOf("\"", index);

                    Version version = new Version(www.downloadHandler.text.Substring(index, endindex - index));
                    if (version > new Version(ZGBepInExInfo.PLUGIN_VERSION))
                    {
                        ZGUIManager.CheckUpdate();
                        ZGScriptTrainer.WriteLog("检查更新！");
                    }
                }
                // 或者获取二进制数据形式的结果
                //byte[] results = www.downloadHandler.data;
            }
        }
    }
}