using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniverseLib.UI.Panels;
using UniverseLib.UI;

namespace ScriptTrainer.UI
{
    internal class ExplorerUIBase : UIBase
    {
        public ExplorerUIBase(string id, System.Action updateMethod) : base(id, updateMethod)
        {
        }

        protected override PanelManager CreatePanelManager()
        {
            return new UEPanelManager(this);
        }
    }
}
