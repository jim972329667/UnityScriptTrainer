using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using System.Collections;
using HarmonyLib;
using System.IO;
using Codes;

namespace ScriptTrainer
{
    public class Scripts : MonoBehaviour
    {
        public static void ChangeLevelUpChance(bool state)
        {
            if (state)
            {
                Time.timeScale = 5f;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
        public static void UnlockAllCombos()
        {
            Singleton<GManager>.Instance.ComboList.UnlockAllCombos();
            ES3.Save<List<Combo>>("ComboList_214", Singleton<GManager>.Instance.ComboList.Combos);
        }
        public static void GetAllCoins()
        {
            Singleton<MapManager>.Instance.PickAllCoins(Singleton<PlayerManager>.Instance.PlayerTransform);
            Singleton<MapManager>.Instance.PickAllGolds(Singleton<PlayerManager>.Instance.PlayerTransform);
        }
        public static void StopExp(bool state)
        {
            Singleton<MapManager>.Instance.stopExp = state;
        }
    }
}
