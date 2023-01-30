using System;
using System.Collections.Generic;
using UnityEngine;


namespace ScriptTrainer
{
    public class Scripts : MonoBehaviour
    {
        public static Vector3 GetPlayerPosition()
        {
            GameObject player = PlayerManager.GetPlayer();
            return player.transform.position;
        }
        public static void DropCoin(int amount)
        {
            ItemDropManager.DropGold(amount, GetPlayerPosition(), false, true);
        }
        public static void GetRecovery()
        {
            PlayerController playerController = PlayerManager.GetPlayerController();
            playerController.SetHealth((float)playerController.ActualMaxHealth, true, true);
            playerController.SetMana((float)playerController.ActualMaxMana, true, true);
        }
        public static void GetGodMod()
        {
            RelicDrop relic = new RelicDrop(RelicType.GodMode, RelicModType.None);
            ItemDropManager.DropSpecialItem(relic);
        }
    }
}
