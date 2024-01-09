using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniverseLib.UI.Panels;

namespace ScriptTrainer.UI
{
    public class UEPanelDragger : PanelDragger
    {
        public UEPanelDragger(PanelBase uiPanel) : base(uiPanel)
        {
        }

        protected override bool MouseInResizeArea(Vector2 mousePos)
        {
            return !UIManager.NavBarRect.rect.Contains(UIManager.NavBarRect.InverseTransformPoint(mousePos)) && base.MouseInResizeArea(mousePos);
        }
    }
}
