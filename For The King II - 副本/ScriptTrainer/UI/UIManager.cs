using ScriptTrainer.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UniverseLib;
using UniverseLib.Input;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace ScriptTrainer.UI
{
    public static class UIManager
    {
        public enum Panels
        {
            MainPanel,
            ObjectExplorer,
            Inspector,
            CSConsole,
            Options,
            ConsoleLog,
            AutoCompleter,
            UIInspectorResults,
            HookManager,
            Clipboard,
            Freecam
        }
        public enum VerticalAnchor
        {
            Top,
            Bottom
        }
        public static bool Initializing { get; internal set; } = true;

        private static int lastScreenWidth;

        private static int lastScreenHeight;

        public static UIManager.VerticalAnchor NavbarAnchor = UIManager.VerticalAnchor.Top;

        private static readonly Vector2 NAVBAR_DIMENSIONS = new Vector2(1020f, 35f);
        internal static UIBase UiBase { get; private set; }
        public static GameObject UIRoot
        {
            get
            {
                return UiBase?.RootObject;
            }
        }
        public static RectTransform UIRootRect { get; private set; }
        public static Canvas UICanvas { get; private set; }
        public static bool ShowMenu
        {
            get
            {
                return UIManager.UiBase != null && UIManager.UiBase.Enabled;
            }
            set
            {
                if (!(UIManager.UiBase == null || !UIManager.UIRoot || UIManager.UiBase.Enabled == value))
                {
                    UniversalUI.SetUIActive("ScriptTrainer.Jim97.UnityExplorer", value);
                    //SetPanelActive(Panels.MainPanel, value);
                    //UniversalUI.SetUIActive(MouseInspector.UIBaseGUID, value);
                }
            }
        }

        internal static readonly Dictionary<UIManager.Panels, UEPanel> UIPanels = new Dictionary<UIManager.Panels, UEPanel>();
        public static RectTransform NavBarRect;
        internal static void InitUI()
        {
            UIManager.UiBase = UniversalUI.RegisterUI<ExplorerUIBase>("ScriptTrainer.Jim97.UnityExplorer", new System.Action(UIManager.Update));
            UIManager.UIRootRect = UIManager.UIRoot.GetComponent<RectTransform>();
            UIManager.UICanvas = UIManager.UIRoot.GetComponent<Canvas>();
            DisplayManager.Init();
            Display activeDisplay = DisplayManager.ActiveDisplay;
            UIManager.lastScreenWidth = activeDisplay.renderingWidth;
            UIManager.lastScreenHeight = activeDisplay.renderingHeight;
            UIManager.CreateTopNavBar();
            UIManager.UIPanels.Add(UIManager.Panels.MainPanel, new MainPanel(UIManager.UiBase));
            //UIManager.UIPanels.Add(UIManager.Panels.ObjectExplorer, new ObjectExplorerPanel(UIManager.UiBase));
            //UIManager.UIPanels.Add(UIManager.Panels.Inspector, new InspectorPanel(UIManager.UiBase));
            //UIManager.UIPanels.Add(UIManager.Panels.CSConsole, new CSConsolePanel(UIManager.UiBase));
            //UIManager.UIPanels.Add(UIManager.Panels.HookManager, new HookManagerPanel(UIManager.UiBase));
            //UIManager.UIPanels.Add(UIManager.Panels.Freecam, new FreeCamPanel(UIManager.UiBase));
            //UIManager.UIPanels.Add(UIManager.Panels.Clipboard, new ClipboardPanel(UIManager.UiBase));
            //UIManager.UIPanels.Add(UIManager.Panels.ConsoleLog, new LogPanel(UIManager.UiBase));
            //UIManager.UIPanels.Add(UIManager.Panels.Options, new OptionsPanel(UIManager.UiBase));
            //UIManager.UIPanels.Add(UIManager.Panels.UIInspectorResults, new MouseInspectorResultsPanel(UIManager.UiBase));
            //MouseInspector.inspectorUIBase = UniversalUI.RegisterUI(MouseInspector.UIBaseGUID, null);
            //new MouseInspector(MouseInspector.inspectorUIBase);
            //Notification.Init();
            //foreach (Dropdown dropdown in UIManager.UIRoot.GetComponentsInChildren<Dropdown>(true))
            //{
            //    dropdown.RefreshShownValue();
            //}
            UIManager.Initializing = false;
            if (ScriptTrainer.Hide_On_Startup.Value)
            {
                UIManager.ShowMenu = false;
            }
        }
        private static void CreateTopNavBar()
        {
            GameObject MainNavbar = UIFactory.CreateUIObject("MainNavbar", UIManager.UIRoot, new Vector2(1020f, 35f));
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(MainNavbar, false, false, true, true, 5, 4, 4, 4, 4, TextAnchor.MiddleCenter);
            MainNavbar.AddComponent<UnityEngine.UI.Image>().color = new Color(0.1f, 0.1f, 0.1f);
            UIManager.NavBarRect = MainNavbar.GetComponent<RectTransform>();
            UIManager.NavBarRect.pivot = new Vector2(0.5f, 1f);
            UIManager.NavbarAnchor = VerticalAnchor.Top;
            UIManager.SetNavBarAnchor();

            string defaultText = $"{ScriptTrainer.Instance.Info.Metadata.Name} V{ScriptTrainer.Instance.Info.Metadata.Version} by:Jim97";
            Text text = UIFactory.CreateLabel(MainNavbar, "Title", defaultText, TextAnchor.MiddleCenter, default(Color), true, 14);
            UIFactory.SetLayoutElement(text.gameObject, 75);

            UIManager.NavbarTabButtonHolder = UIFactory.CreateUIObject("NavTabButtonHolder", MainNavbar, default(Vector2));
            GameObject navbarTabButtonHolder = UIManager.NavbarTabButtonHolder;

            UIFactory.SetLayoutElement(navbarTabButtonHolder, null, 25, 999, 999, null, null, null);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(UIManager.NavbarTabButtonHolder, new bool?(false), new bool?(true), new bool?(true), new bool?(true), new int?(4), new int?(2), new int?(2), new int?(2), new int?(2), null);
            GameObject gameObject3 = UIFactory.CreateUIObject("Spacer", MainNavbar, default(Vector2));
            UIFactory.SetLayoutElement(gameObject3, 15);
            UIManager.closeBtn = UIFactory.CreateButton(MainNavbar, "CloseButton", ScriptTrainer.ShowCounter.Value.ToString(), null);

            UIFactory.SetLayoutElement(UIManager.closeBtn.Component.gameObject, 60, 25, 0);
            RuntimeHelper.SetColorBlock(UIManager.closeBtn.Component, new Color?(new Color(0.63f, 0.32f, 0.31f)), new Color?(new Color(0.81f, 0.25f, 0.2f)), new Color?(new Color(0.6f, 0.18f, 0.16f)), null);

            ButtonRef buttonRef = UIManager.closeBtn;
            buttonRef.OnClick = (System.Action)Delegate.Combine(buttonRef.OnClick, new System.Action(UIManager.OnCloseButtonClicked));
        }
        public static void Update()
        {
            //if (UIManager.UIRoot)
            //{
            //    if (!MouseInspector.Instance.TryUpdate())
            //    {
            //        Notification.Update();
            //        UIManager.timeScaleWidget.Update();
            //        Display activeDisplay = DisplayManager.ActiveDisplay;
            //        if (activeDisplay.renderingWidth != UIManager.lastScreenWidth || activeDisplay.renderingHeight != UIManager.lastScreenHeight)
            //        {
            //            UIManager.OnScreenDimensionsChanged();
            //        }
            //    }
            //}
        }
        public static UEPanel GetPanel(UIManager.Panels panel)
        {
            return UIManager.UIPanels[panel];
        }
        public static T GetPanel<T>(UIManager.Panels panel) where T : UEPanel
        {
            return (T)((object)UIManager.UIPanels[panel]);
        }
        public static void SetPanelActive(UIManager.Panels panelType, bool active)
        {
            UIManager.GetPanel(panelType).SetActive(active);
        }
        public static void SetPanelActive(UEPanel panel, bool active)
        {
            panel.SetActive(active);
        }
        public static void TogglePanel(UIManager.Panels panel)
        {
            UEPanel panel2 = UIManager.GetPanel(panel);
            UIManager.SetPanelActive(panel, !panel2.Enabled);
        }
        public static void SetNavBarAnchor()
        {
            UIManager.VerticalAnchor navbarAnchor = UIManager.NavbarAnchor;
            UIManager.VerticalAnchor verticalAnchor = navbarAnchor;
            if (verticalAnchor != UIManager.VerticalAnchor.Top)
            {
                if (verticalAnchor == UIManager.VerticalAnchor.Bottom)
                {
                    UIManager.NavBarRect.anchorMin = new Vector2(0.5f, 0f);
                    UIManager.NavBarRect.anchorMax = new Vector2(0.5f, 0f);
                    UIManager.NavBarRect.anchoredPosition = new Vector2(UIManager.NavBarRect.anchoredPosition.x, 35f);
                    UIManager.NavBarRect.sizeDelta = UIManager.NAVBAR_DIMENSIONS;
                }
            }
            else
            {
                UIManager.NavBarRect.anchorMin = new Vector2(0.5f, 1f);
                UIManager.NavBarRect.anchorMax = new Vector2(0.5f, 1f);
                UIManager.NavBarRect.anchoredPosition = new Vector2(UIManager.NavBarRect.anchoredPosition.x, 0f);
                UIManager.NavBarRect.sizeDelta = UIManager.NAVBAR_DIMENSIONS;
            }
        }
        private static void OnCloseButtonClicked()
        {
            UIManager.ShowMenu = false;
        }

        public static GameObject NavbarTabButtonHolder;
        private static ButtonRef closeBtn;
    }
}
