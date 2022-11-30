using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityGameUI;
using static UnityEngine.UI.CanvasScaler;

namespace ScriptTrainer
{
    public class Scripts : MonoBehaviour
    {

        public static bool IsChangeMusic = false;
        public Scripts()
        {
        }
        public static void DiscountItemPrice(bool state)
        {
            ScriptPatch.IsDiscount = state;
        }

    }
}
