using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniverseLib.UI.Panels;
using UniverseLib.UI;

namespace ScriptTrainer.UI
{
    public class UEPanelManager : PanelManager
    {
        public UEPanelManager(UIBase owner) : base(owner)
        {
        }

        protected override Vector3 MousePosition
        {
            get
            {
                return DisplayManager.MousePosition;
            }
        }

        protected override Vector2 ScreenDimensions
        {
            get
            {
                return new Vector2((float)DisplayManager.Width, (float)DisplayManager.Height);
            }
        }
        protected override bool MouseInTargetDisplay
        {
            get
            {
                return DisplayManager.MouseInTargetDisplay;
            }
        }
        internal void DoInvokeOnPanelsReordered()
        {
            this.InvokeOnPanelsReordered();
        }

        protected override void SortDraggerHeirarchy()
        {
            base.SortDraggerHeirarchy();
            //bool flag = !UIManager.Initializing && AutoCompleteModal.Instance != null;
            //if (flag)
            //{
            //    this.draggerInstances.Remove(AutoCompleteModal.Instance.Dragger);
            //    this.draggerInstances.Insert(0, AutoCompleteModal.Instance.Dragger);
            //}
        }
    }
}
