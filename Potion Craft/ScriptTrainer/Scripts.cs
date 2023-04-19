using PotionCraft.ManagersSystem;
using PotionCraft.ManagersSystem.Player;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace ScriptTrainer
{
    public class Scripts : MonoBehaviour
    {
        public static void AddGold(int count)
        {
            PlayerManager player = Managers.Player;
            player.AddGold(count);
        }
        public static void AddExperience(int count)
        {
            PlayerManager player = Managers.Player;
            player.AddExperience(count);
        }
        public static void AddKarma(int count)
        {
            PlayerManager player = Managers.Player;
            player.AddKarma(count);
        }
        public static void AddPopularity(int count)
        {
            PlayerManager player = Managers.Player;
            player.AddPopularity(count);
        }
        public static void AddTalentsPoints(int count)
        {
            PlayerManager.TalentsSubManager Talents = Managers.Player.talents;
            Talents.CurrentPoints += count;
        }
        public static void ChangeTimeScale(float scale)
        {
            if(scale != 1)
            {
                ScriptTrainer.Instance.TimeScaleRate = scale;
                ScriptTrainer.Instance.TimeScale = true;
            }
            else
            {
                ScriptTrainer.Instance.TimeScaleRate = scale;
                ScriptTrainer.Instance.TimeScale = false;
            }
        }
    }
}
