using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UniverseLib;
using UniverseLib.Config;
using UniverseLib.Input;

namespace ScriptTrainer.UI
{
    public static class DisplayManager
    {
        public static int ActiveDisplayIndex { get; private set; }

        private static Camera canvasCamera;
        public static Display ActiveDisplay
        {
            get
            {
                return Display.displays[DisplayManager.ActiveDisplayIndex];
            }
        }
        public static int Width
        {
            get
            {
                return DisplayManager.ActiveDisplay.renderingWidth;
            }
        }
        public static int Height
        {
            get
            {
                return DisplayManager.ActiveDisplay.renderingHeight;
            }
        }
        public static Vector3 MousePosition
        {
            get
            {
                return Application.isEditor ? InputManager.MousePosition : Display.RelativeMouseAt(InputManager.MousePosition);
            }
        }
        public static bool MouseInTargetDisplay
        {
            get
            {
                return DisplayManager.MousePosition.z == (float)DisplayManager.ActiveDisplayIndex;
            }
        }
        internal static void Init()
        {
            DisplayManager.SetDisplay(0);
        }
        public static void SetDisplay(int display)
        {
            bool flag = DisplayManager.ActiveDisplayIndex == display;
            if (!flag)
            {
                if (Display.displays.Length > display)
                {
                    DisplayManager.ActiveDisplayIndex = display;
                    DisplayManager.ActiveDisplay.Activate();
                    UIManager.UICanvas.targetDisplay = display;
                    bool flag4 = !Camera.main || Camera.main.targetDisplay != display;
                    if (flag4)
                    {
                        bool flag5 = !DisplayManager.canvasCamera;
                        if (flag5)
                        {
                            DisplayManager.canvasCamera = new GameObject("UnityExplorer_CanvasCamera").AddComponent<Camera>();
                            UnityEngine.Object.DontDestroyOnLoad(DisplayManager.canvasCamera.gameObject);
                            DisplayManager.canvasCamera.hideFlags = (HideFlags)61;
                        }
                        DisplayManager.canvasCamera.targetDisplay = display;
                    }
                    RuntimeHelper.StartCoroutine(DisplayManager.FixPanels());
                }
            }
        }
        private static IEnumerator FixPanels()
        {
            yield return null;
            yield return null;
            foreach (UEPanel panel in UIManager.UIPanels.Values)
            {
                panel.EnsureValidSize();
                panel.EnsureValidPosition();
                panel.Dragger.OnEndResize();
                //panel = null;
            }
            Dictionary<UIManager.Panels, UEPanel>.ValueCollection.Enumerator enumerator = default(Dictionary<UIManager.Panels, UEPanel>.ValueCollection.Enumerator);
            yield break;
        }
    }
}
