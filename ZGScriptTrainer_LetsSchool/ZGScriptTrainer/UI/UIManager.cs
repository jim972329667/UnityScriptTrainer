using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI.Models;
using UniverseLib.UI;
using UniverseLib;
using ZGScriptTrainer.UI.Panels;

namespace ZGScriptTrainer.UI
{
    public static class UIManager
    {
        public enum Panels
        {
            MainWindow,
            ItemWindow
        }
        public static readonly Dictionary<Panels, string> PanelNames = new Dictionary<Panels, string>() 
        { 
            { Panels.MainWindow, "主界面" }, 
            { Panels.ItemWindow, "物品界面" } 
        };
        public static bool Initializing { get; internal set; } = true;
        internal static UIBase UiBase { get; private set; }

        public const string UiBaseGUID = "ScriptTrainer.Jim97.UiBase";
        public static GameObject UIRoot => UiBase?.RootObject;
        public static RectTransform UIRootRect { get; private set; }
        public static Canvas UICanvas { get; private set; }

        internal static readonly Dictionary<Panels, ZGPanel> UIPanels = new Dictionary<Panels, ZGPanel>();

        public static RectTransform NavBarRect;

        public static GameObject NavbarTabButtonHolder;

        private static readonly Vector2 NAVBAR_DIMENSIONS = new Vector2(1020f, 40f);

        private static ButtonRef closeBtn;
        public static bool ShowMenu
        {
            get => UiBase != null && UiBase.Enabled;
            set
            {
                if (UiBase == null || !UIRoot || UiBase.Enabled == value)
                    return;

                UniversalUI.SetUIActive(UiBaseGUID, value);
            }
        }

        internal static void InitUI()
        {
            UiBase = UniversalUI.RegisterUI(UiBaseGUID, null);

            UIRootRect = UIRoot.GetComponent<RectTransform>();
            UICanvas = UIRoot.GetComponent<Canvas>();


            // Create UI.
            CreateTopNavBar();
            // This could be automated with Assembly.GetTypes(),
            // but the order is important and I'd have to write something to handle the order.
            UIPanels.Add(Panels.MainWindow, new MainWindow(UiBase));
            UIPanels.Add(Panels.ItemWindow, new ItemWindow(UiBase));
            //UIPanels.Add(Panels.Inspector, new InspectorPanel(UiBase));
            //UIPanels.Add(Panels.CSConsole, new CSConsolePanel(UiBase));
            //UIPanels.Add(Panels.HookManager, new HookManagerPanel(UiBase));
            //UIPanels.Add(Panels.Freecam, new FreeCamPanel(UiBase));
            //UIPanels.Add(Panels.Clipboard, new ClipboardPanel(UiBase));
            //UIPanels.Add(Panels.ConsoleLog, new LogPanel(UiBase));
            //UIPanels.Add(Panels.Options, new OptionsPanel(UiBase));
            //UIPanels.Add(Panels.UIInspectorResults, new MouseInspectorResultsPanel(UiBase));

            // Failsafe fix, in some games all dropdowns displayed values are blank on startup for some reason.
            foreach (Dropdown dropdown in UIRoot.GetComponentsInChildren<Dropdown>(true))
                dropdown.RefreshShownValue();

            Initializing = false;
            ShowMenu = false;
        }

        #region[NavPanel]
        public static ZGPanel GetPanel(Panels panel) => UIPanels[panel];
        public static T GetPanel<T>(Panels panel) where T : ZGPanel => (T)UIPanels[panel];
        public static void TogglePanel(Panels panel)
        {
            ZGPanel uiPanel = GetPanel(panel);
            SetPanelActive(panel, !uiPanel.Enabled);
        }
        public static void SetPanelActive(Panels panelType, bool active)
        {
            GetPanel(panelType).SetActive(active);
        }
        public static void SetPanelActive(ZGPanel panel, bool active)
        {
            panel.SetActive(active);
        }
        private static void CreateTopNavBar()
        {
            GameObject navbarPanel = UIFactory.CreateUIObject("MainNavbar", UIRoot);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(navbarPanel, false, false, true, true, 5, 4, 4, 4, 4, TextAnchor.MiddleCenter);
            navbarPanel.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f);
            NavBarRect = navbarPanel.GetComponent<RectTransform>();
            NavBarRect.pivot = new Vector2(0.5f, 1f);

            NavBarRect.anchorMin = new Vector2(0.5f, 1f);
            NavBarRect.anchorMax = new Vector2(0.5f, 1f);
            NavBarRect.anchoredPosition = new Vector2(NavBarRect.anchoredPosition.x, 0);
            NavBarRect.sizeDelta = NAVBAR_DIMENSIONS;

            //标题

            string titleTxt = $"{ZGBepInExInfo.PLUGIN_TITLE} <i><color=grey>{ZGBepInExInfo.PLUGIN_VERSION}</color></i>";
            Text title = UIFactory.CreateLabel(navbarPanel, "Title", titleTxt, TextAnchor.MiddleCenter, default, true, ZGScriptTrainer.FontSize.Value);
            UIFactory.SetLayoutElement(title.gameObject, minWidth: 75, flexibleWidth: 0);

            //界面

            NavbarTabButtonHolder = UIFactory.CreateUIObject("NavTabButtonHolder", navbarPanel);
            UIFactory.SetLayoutElement(NavbarTabButtonHolder, minHeight: 25, flexibleHeight: 999, flexibleWidth: 999);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(NavbarTabButtonHolder, false, true, true, true, 4, 2, 2, 2, 2);


            //间隔
            GameObject spacer = UIFactory.CreateUIObject("Spacer", navbarPanel);
            UIFactory.SetLayoutElement(spacer, minWidth: 15);

            // Hide menu button

            closeBtn = UIFactory.CreateButton(navbarPanel, "CloseButton", ZGScriptTrainer.ShowCounter.Value.ToString());
            ZGUIUtility.SetLayoutElement(closeBtn, minHeight: 25, minWidth: 60, flexibleWidth: 0);
            RuntimeHelper.SetColorBlock(closeBtn.Component, new Color(0.63f, 0.32f, 0.31f),
                new Color(0.81f, 0.25f, 0.2f), new Color(0.6f, 0.18f, 0.16f));


            closeBtn.OnClick += OnCloseButtonClicked;
        }
        private static void OnCloseButtonClicked()
        {
            ShowMenu = false;
        }
        #endregion
    }
}
