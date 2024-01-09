using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using System.Collections;
using HarmonyLib;
using System.IO;
using static UnityEngine.EventSystems.EventTrigger;


namespace ScriptTrainer
{
    public static class Scripts
    {
        public static void God()
        {
            ScriptTrainer.WriteGameCmd($"god");
        }
        public static void FullHp()
        {
            ScriptTrainer.WriteGameCmd($"fullhp");
            ScriptTrainer.PlayerManagerInvoke("AllWandFullMP", null);
        }
        public static void RemoveCurse()
        {
            ScriptTrainer.WriteGameCmd($"removecurse");
        }
        public static void UnlockNPC()
        {
            ScriptTrainer.WriteGameCmd($"allnpc");
            if(ScriptTrainer.TestController != null)
                ScriptTrainer.TestController.masterSwitch = true;
        }
        public static void UnlockGallery()
        {
            ScriptTrainer.WriteGameCmd($"gallery");
            if (ScriptTrainer.TestController != null)
                ScriptTrainer.TestController.masterSwitch = true;
        }
        public static void AddCoin(int count)
        {
            ScriptTrainer.WriteGameCmd($"coin {count}");
        }
        public static void AddKey(int count)
        {
            ScriptTrainer.WriteGameCmd($"key {count}");
        }
        public static void AddCrystal(int count)
        {
            ScriptTrainer.WriteGameCmd($"crystal {count}");
        }
        public static void AddBlood(int count)
        {
            ScriptTrainer.WriteGameCmd($"blood {count}");
        }
    }
}
