using Eremite;
using Eremite.Controller;
using Eremite.Model;
using Eremite.Model.Meta;
using Eremite.Model.State;
using Eremite.Services;
using Eremite.View;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace ScriptTrainer
{
    public class Scripts : MonoBehaviour
    {
        public static void AddExp(int exp)
        {
            LevelState SavedState = MetaController.Instance.MetaServices.MetaStateService.Level;

            if (exp == 0 || MetaController.Instance.MetaServices.MetaEconomyService.IsLastLevel(SavedState))
            {
                return;
            }
            int b = SavedState.targetExp - SavedState.exp;
            int num = Mathf.Min(exp, b);
            int exp2 = exp - num;
            MetaController.Instance.MetaServices.MetaStateService.Economy.currentCycleExp += num;
            SavedState.exp += num;
            CheckForLevelUp();
            AddExp(exp2);
        }
        public static void CheckForLevelUp()
        {
            LevelState SavedState = MetaController.Instance.MetaServices.MetaStateService.Level;
            if (SavedState.exp == SavedState.targetExp)
            {
                LevelModel nextLevel = MainController.Instance.Settings.metaConfig.levels[SavedState.level + 1];
                SavedState.level++;
                SavedState.exp = 0;
                SavedState.targetExp = nextLevel.expCeiling;

                LevelModel toGrant = MainController.Instance.Settings.metaConfig.levels[SavedState.level];
                MetaRewardModel[] rewards = toGrant.rewards;
                for (int i = 0; i < rewards.Length; i++)
                {
                    rewards[i].Consume();
                }
            }
        }

        public static void InfinitePreparationPoints(bool state)
        {
            ScriptPatch.InfinitePreparationPoints = state;
        }

        public static void SaveStartBonuses(List<GoodPickState> goods)
        {
            string value = string.Empty;
            foreach(GoodPickState good in goods)
            {
                if (value != string.Empty)
                    value += '\n';
                value += $"{good.name};{good.amount}";
            }
            ScriptTrainer.StartBonuses.Value = value;
        }
        public static List<GoodPickState> LoadStartBonuses()
        {
            List<GoodPickState> goods = new List<GoodPickState>();
            string values = ScriptTrainer.StartBonuses.Value;

            foreach(string value in values.Split('\n'))
            {
                GoodPickState good = new GoodPickState()
                {
                    name = value.Split(';')[0],
                    amount = int.Parse(value.Split(';')[1]),
                    cost = 0
                };
                goods.Add(good);
            }

            return goods;
        }
        public static void SaveEffects(List<string> effects)
        {
            string value = string.Empty;
            foreach (string effect in effects)
            {
                if (value != string.Empty)
                    value += '\n';
                value += effect;
            }
            ScriptTrainer.SaveEffects.Value = value;
        }
        public static List<string> LoadEffects()
        {
            List<string> effects = new List<string>();
            string values = ScriptTrainer.SaveEffects.Value;

            foreach (string value in values.Split('\n'))
            {
                effects.Add(value);
            }

            return effects;
        }
    }
}
