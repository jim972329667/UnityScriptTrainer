using System.Reflection;
using UnityEngine;
using UniverseLib.Input;
using UniverseLib;
using ZGScriptTrainer.ItemSpwan;
using ZGScriptTrainer.UI.Panels;
using ZGScriptTrainer.UI;

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
        }
        internal void Update()
        {
            if (!ItemWindow.Instance.Initialized)
            {
                if (ZGItemUtil.CanGetItemData())
                {
                    ZGItemUtil.GetBaseItemData();
                    ItemWindow.Instance.Initialize();
                }
            }
            if (InputManager.GetKeyDown(ZGScriptTrainer.ShowCounter.Value))
            {
                UIManager.ShowMenu = !UIManager.ShowMenu;
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

            TryDestroy(UIManager.UIRoot?.transform.root.gameObject);

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
    }
}
