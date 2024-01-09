using ScriptTrainer.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace ScriptTrainer.Panels
{
    public class MainPanel : UEPanel
    {
        public static MainPanel Instance { get; private set; }

        private static readonly Vector2Int MinSize = new Vector2Int(810, 350);

        public static float PanelWidth;
        public override string Name
        {
            get
            {
                return "主界面";
            }
        }
        public override UIManager.Panels PanelType
        {
            get
            {
                return UIManager.Panels.MainPanel;
            }
        }
        public override bool ShouldSaveActiveState
        {
            get
            {
                return false;
            }
        }
        public override int MinWidth
        {
            get
            {
                return MinSize.x;
            }
        }
        public override int MinHeight
        {
            get
            {
                return MinSize.y;
            }
        }
        public override Vector2 DefaultAnchorMin
        {
            get
            {
                return new Vector2(0.35f, 0.175f);
            }
        }
        public override Vector2 DefaultAnchorMax
        {
            get
            {
                return new Vector2(0.8f, 0.925f);
            }
        }
        public static float CurrentPanelWidth
        {
            get
            {
                return MainPanel.Instance.Rect.rect.width;
            }
        }
        public static float CurrentPanelHeight
        {
            get
            {
                return MainPanel.Instance.Rect.rect.height;
            }
        }
        public MainPanel(UIBase owner) : base(owner)
        {
            MainPanel.Instance = this;
        }
        public override void Update()
        {

        }
        public override void OnFinishResize()
        {
            base.OnFinishResize();
            MainPanel.PanelWidth = base.Rect.rect.width;
            MainPanel.OnPanelResized(base.Rect.rect.width);
        }
        internal static void OnPanelResized(float width)
        {
            MainPanel.PanelWidth = width;
            //foreach (InspectorBase inspectorBase in InspectorManager.Inspectors)
            //{
            //    ReflectionInspector reflectionInspector = inspectorBase as ReflectionInspector;
            //    bool flag = reflectionInspector != null;
            //    if (flag)
            //    {
            //        reflectionInspector.SetLayouts();
            //    }
            //}
        }
        protected override void ConstructPanelContent()
        {
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(ContentRoot, true, true, true, true, 4, null, null, 5, 5, null);
            NavbarHolder = UIFactory.CreateGridGroup(ContentRoot, "Navbar", new Vector2(200f, 22f), new Vector2(4f, 4f), new Color(0.05f, 0.05f, 0.05f));
            NavbarHolder.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            ContentHolder = UIFactory.CreateVerticalGroup(ContentRoot, "ContentHolder", true, true, true, true, 0, default(Vector4), new Color(0.1f, 0.1f, 0.1f), null);
            //UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(ContentHolder, false, false, true, true, 2);
            UIFactory.SetLayoutElement(ContentHolder, null, null, null, 9999, null, null, null);


            var l1 = UIFactory.CreateToggle(ContentHolder, "LoadoutPoints", out var toggle, out Text text);
            toggle.isOn = ScriptPatch.InfiniteLoadoutPoints;
            toggle.onValueChanged.AddListener((bool value) =>
            {
                ScriptPatch.InfiniteLoadoutPoints = value;
            });
            text.text = "无限起始点数";
            UIFactory.SetLayoutElement(l1, null, 25);

            ContentRect = ContentHolder.GetComponent<RectTransform>();
            SetActive(false);
        }
        public void CloseAll()
        {
            UIManager.SetPanelActive(UIManager.Panels.MainPanel, false);
        }
        public GameObject NavbarHolder;
        public GameObject ContentHolder;
        public RectTransform ContentRect;
    }
}
