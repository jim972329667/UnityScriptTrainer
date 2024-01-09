using Assets.Code.Game;
using Assets.Code.Item;
using Assets.Code.Profile;
using Assets.Code.Rules;
using Assets.Code.Run;
using Assets.Code.Source;
using Assets.Code.Utils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Code.Debugging.ReorderableListTestBhv;


namespace ScriptTrainer
{
    public class Scripts : MonoBehaviour
    {
        public static void AddCandles(int count)
        {
            int old = (int)SingletonMonoBehaviour<ProfileBhv>.Instance.GetCurrentProfile().ProfileValues.GetValue(ProfileValueType.CANDLES);
            SingletonMonoBehaviour<ProfileBhv>.Instance.GetCurrentProfile().ProfileValues.SetValue(ProfileValueType.CANDLES, count + old);
        }
        public static void AddGold(int count)
        {
            ItemDefinition gold = RulesManager.GetRules<InventoryRules>().GOLD;
            bool isPurchase = false;
            SingletonMonoBehaviour<RunBhv>.Instance.PlayerInventory.AddItems(gold, count, isPurchase);
        }
        public static void AddTorch(int count)
        {
            SingletonMonoBehaviour<RunBhv>.Instance.RunValues.ChangeValue(RunValueType.TORCH, (float)count, SourceType.DEBUG);
        }
        public static void AddInventorySize(int count)
        {
            int old = Traverse.Create(SingletonMonoBehaviour<RunBhv>.Instance.PlayerInventory).Field("m_NumberOfBaseSlots").GetValue<int>();
            Traverse.Create(SingletonMonoBehaviour<RunBhv>.Instance.PlayerInventory).Field("m_NumberOfBaseSlots").SetValue(old + count);
            SingletonMonoBehaviour<RunBhv>.Instance.PlayerInventory.AddSlots(count, false);
        }
    }
}
