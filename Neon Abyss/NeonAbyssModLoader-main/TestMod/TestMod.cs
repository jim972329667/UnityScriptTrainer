using HarmonyLib;
using I2.Loc;
using NEON.Game.Cine;
using NEON.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Veewo.Framework;

namespace TestMod
{
    public static class Utils
    {
        public static TextWriter fs;
        public static bool flying = false;
        public static Sprite sprite;

        public static object get(this object obj, string fieldname)
        {
            var prop = obj.GetType().GetField(fieldname,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return prop.GetValue(obj);
        }
        public static void set(this object obj, string fieldname, object value)
        {
            var prop = obj.GetType().GetField(fieldname,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            prop.SetValue(obj, value);
        }
    }

    public class TestMod
    {
        private static void Patch()
        {
            try
            {
                Utils.fs.WriteLine("Patching code");
                Harmony harmony = new Harmony("com.exemple.patch");
                harmony.PatchAll();
                Utils.fs.WriteLine("Patching successful");
            }
            catch (Exception e)
            {
                Utils.fs.WriteLine($"Patching failed {e}");
            }
        }

        public void Initialize()
        {

            var logFilePath = "TestMod.log";
            if (File.Exists(logFilePath))
                File.Delete(logFilePath);

            Utils.fs = File.CreateText(logFilePath);
            Utils.fs.WriteLine("Hello from Test Mod");
            
            LoadAssets();
            Patch();
            Utils.fs.Flush();
        }

        private void LoadAssets()
        {
            var bundle = AssetBundle.LoadFromFile(@"mods\TestMod\trihard");
            Utils.sprite = bundle.LoadAsset<Sprite>("trihard");
        }
    }
}
