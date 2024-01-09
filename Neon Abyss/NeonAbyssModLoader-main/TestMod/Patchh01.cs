using HarmonyLib;
using I2.Loc;
using NEON.Debuger;
using NEON.Framework;
using NEON.Game;
using NEON.Game.Actors;
using NEON.Game.GameModes;
using NEON.Game.LootSystem;
using NEON.Game.Managers;
using NEON.Game.PowerUps;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TestMod
{
    [HarmonyPatch(typeof(PlayerActor))]
    [HarmonyPatch("Jump")]
    class Patchh01
    {
        public static void Prefix(PlayerActor __instance)
        {
            try
            {
                var loot = new ChestLoot();
                loot.ChestTypes = ChestTypes.Stone;
                Global.GetService<LootManager>(false)
                    .SpawnLoot(loot, __instance.transform.position);

                var playerstate = Global.GetPlayerState<NEONPlayerState>();
                var powerups = playerstate.PowerUps;
                var powerup = new BodyGrowthPowerUp
                {
                    sprite = Utils.sprite,
                    name = "Bigger is better",
                    Name = "Bigger is better",
                    Description =  new LocalizedString { mTerm = "A lot bigger than expected" },
                    BackgroundSrite = Utils.sprite,
                };


                var pickup = Pickup.Create(__instance.transform.position, powerup, __instance.CurrentRoom, true);
                Utils.fs.WriteLine($"{powerups?.Count} items on character");
                foreach (var item in powerups)
                {
                    Utils.fs.WriteLine($"Item [{item?.id}]:{item?.name} {item?.GetType()?.FullName}");
                }

                
                Utils.fs.Flush();
            }
            catch (Exception e)
            {
                Utils.fs.WriteLine(e);
                Utils.fs.Flush();
            }
        }

        public static void Postfix()
        {
        }
    }
}
