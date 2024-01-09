using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZGScriptTrainer.UI
{
    public class TooltipGUI : MonoBehaviour
    {
#if CPP
        public TooltipGUI(System.IntPtr ptr) : base(ptr) { }
#endif
        public bool EnableTooltip = false;

    }
}
