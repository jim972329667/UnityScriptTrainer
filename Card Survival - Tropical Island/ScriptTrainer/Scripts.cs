using System;
using System.Collections.Generic;
using UnityEngine;


namespace ScriptTrainer
{
    public class Scripts : MonoBehaviour
    {
        public static void AddSunCount(int count)
        {
            if (!GameLoad.Instance)
            {
                return;
            }
            GameLoad.Instance.SaveData.Suns += count;
            GameLoad.Instance.SaveDataToFile();
        }
        public static void AddMoonCount(int count)
        {
            if (!GameLoad.Instance)
            {
                return;
            }
            GameLoad.Instance.SaveData.Moons += count;
            GameLoad.Instance.SaveDataToFile();
        }
        public static void ChangeResearchRate(bool state)
        {
            ScriptPatch.ResearchRate= state;
        }
    }
}
