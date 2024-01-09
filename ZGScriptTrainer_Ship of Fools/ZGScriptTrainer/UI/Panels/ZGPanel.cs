using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI.Models;
using UniverseLib.UI.Panels;
using UniverseLib.UI;
using UniverseLib;

namespace ZGScriptTrainer.UI.Panels
{
    public abstract class ZGPanel : PanelBase
    {
        protected ZGPanel(UIBase owner) : base(owner) { }
        public abstract UIManager.Panels PanelType { get; }
        public virtual bool ShowByDefault => false;
        public virtual bool NavButtonWanted => true;
        public override Vector2 DefaultPosition => new Vector2(-MinWidth / 2, MinHeight / 2);
        public ButtonRef NavButton { get; internal set; }

        public override void SetActive(bool active)
        {
            if (this.Enabled != active)
            {
                base.SetActive(active);

                if (NavButtonWanted && NavButton != null)
                {
                    Color color = active ? UniversalUI.EnabledButtonColor : UniversalUI.DisabledButtonColor;
                    RuntimeHelper.SetColorBlock(NavButton.Component, color, color * 1.2f);
                }
            }

            if (!active)
            {
                if (Dragger != null)
                    this.Dragger.WasDragging = false;
            }
            else
            {
                this.UIRoot.transform.SetAsLastSibling();
                //this.Owner.Panels.InvokeOnPanelsReordered();
            }
        }
        public override void ConstructUI()
        {
            base.ConstructUI();

            if (NavButtonWanted)
            {
                // create navbar button

                NavButton = UIFactory.CreateButton(UIManager.NavbarTabButtonHolder, $"Button_{PanelType}", Name);
                GameObject navBtn = NavButton.Component.gameObject;
                navBtn.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(navBtn, false, true, true, true, 0, 0, 0, 5, 5, TextAnchor.MiddleCenter);
                ZGUIUtility.SetLayoutElement(NavButton, minWidth: 80);

                RuntimeHelper.SetColorBlock(NavButton.Component, UniversalUI.DisabledButtonColor, UniversalUI.DisabledButtonColor * 1.2f);
                NavButton.OnClick += () => { UIManager.TogglePanel(PanelType); };

                GameObject txtObj = navBtn.transform.Find("Text").gameObject;
                txtObj.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

            base.TitleBar.GetComponentInChildren<Text>().fontSize = ZGScriptTrainer.FontSize.Value;
            this.SetActive(true);
            this.SetActive(false);
            this.SetActive(ShowByDefault);
        }
        protected override void LateConstructUI()
        {
            base.LateConstructUI();
            Dragger.OnEndResize();
        }
    }
}
