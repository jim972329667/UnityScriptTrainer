using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Panels;

namespace ScriptTrainer.UI
{
    public abstract class UEPanel : PanelBase
    {
        protected UEPanel(UIBase owner) : base(owner)
        {
        }
        public abstract UIManager.Panels PanelType { get; }
        public virtual bool ShowByDefault
        {
            get
            {
                return false;
            }
        }
        public virtual bool ShouldSaveActiveState
        {
            get
            {
                return true;
            }
        }
        public virtual bool NavButtonWanted
        {
            get
            {
                return true;
            }
        }

        private bool setDefault = false;
        public ButtonRef NavButton { get; internal set; }
        protected override PanelDragger CreatePanelDragger()
        {
            return new UEPanelDragger(this);
        }
        public override void OnFinishDrag()
        {
            base.OnFinishDrag();
        }
        public override void OnFinishResize()
        {
            base.OnFinishResize();
        }
        public override void SetActive(bool active)
        {
            if (base.Enabled != active)
            {
                base.SetActive(active);
                if (this.NavButtonWanted && this.NavButton != null)
                {
                    Color color = active ? UniversalUI.EnabledButtonColor : UniversalUI.DisabledButtonColor;
                    RuntimeHelper.SetColorBlock(this.NavButton.Component, new Color?(color), new Color?(color * 1.2f), null, null);
                }
            }
            if (!active)
            {
                if (base.Dragger != null)
                {
                    base.Dragger.WasDragging = false;
                }
            }
            else
            {
                this.UIRoot.transform.SetAsLastSibling();
                (base.Owner.Panels as UEPanelManager).DoInvokeOnPanelsReordered();
            }
        }
        public override void SetDefaultSizeAndPosition()
        {
            bool flag = this.setDefault;
            if (!flag)
            {
                this.setDefault = true;
                base.SetDefaultSizeAndPosition();
            }
        }
        public override void ConstructUI()
        {
            base.ConstructUI();
            bool navButtonWanted = this.NavButtonWanted;
            if (navButtonWanted)
            {
                this.NavButton = UIFactory.CreateButton(UIManager.NavbarTabButtonHolder, string.Format("Button_{0}", this.PanelType), this.Name, null);
                GameObject gameObject = this.NavButton.Component.gameObject;
                gameObject.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(gameObject, new bool?(false), new bool?(true), new bool?(true), new bool?(true), new int?(0), new int?(0), new int?(0), new int?(5), new int?(5), new TextAnchor?((TextAnchor)4));
                UIFactory.SetLayoutElement(gameObject, new int?(80), null, null, null, null, null, null);
                RuntimeHelper.SetColorBlock(this.NavButton.Component, new Color?(UniversalUI.DisabledButtonColor), new Color?(UniversalUI.DisabledButtonColor * 1.2f), null, null);
                ButtonRef navButton = this.NavButton;
                navButton.OnClick = (System.Action)Delegate.Combine(navButton.OnClick, new System.Action(delegate ()
                {
                    UIManager.TogglePanel(this.PanelType);
                }));
                GameObject gameObject2 = gameObject.transform.Find("Text").gameObject;
                gameObject2.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            this.SetActive(true);
            this.SetActive(false);
            this.SetActive(this.ShowByDefault);
        }
        protected override void LateConstructUI()
        {
            base.LateConstructUI();
            base.Dragger.OnEndResize();
        }
    }
}
