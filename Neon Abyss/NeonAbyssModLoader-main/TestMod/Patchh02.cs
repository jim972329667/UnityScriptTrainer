using HarmonyLib;
using NEON.Framework;
using NEON.Game;
using NEON.Game.Actors;
using NEON.Game.LootSystem;
using NEON.Game.Managers;
using NEON.Game.PowerUps;
using System;

namespace TestMod
{
    [HarmonyPatch(typeof(LevelConfig))]
    [HarmonyPatch("CreateGameObjects")]
    class Patchh02
    {
        public static void Prefix()
        {
           
        }

        public static void Postfix()
        {
        }
    }
}
