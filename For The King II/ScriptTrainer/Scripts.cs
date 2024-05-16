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
        public static void CurePlayer()
        {
            Entity player = ItemWindow.GetSelectEntity();
            if (player != null)
            {
                CharacterHelper.SetToMaxHealth(player);
                CharacterHelper.SetToMaxFocus(player);
            }
        }
        public static void CurePlayers()
        {
            var players = ItemWindow.GetPartyEntity();
            if (players.Count > 0)
            {
                foreach (var player in players)
                {
                    CharacterHelper.SetToMaxHealth(player);
                    CharacterHelper.SetToMaxFocus(player);
                }
            }
        }
    }
}
