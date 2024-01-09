using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.Image;
using Random = UnityEngine.Random;
using Type = System.Type;

namespace ScriptTrainer
{
    public class ScriptPatch
    {
        #region[全局参数]
        public static List<Item_Base> allAvailableItems = new List<Item_Base>();

        public static bool AllowCheat = false;
        #endregion

        #region 前补丁
        //Prefix 前补丁，在补丁的函数前执行//return false跳过源程序

        //示例
        //[HarmonyPatch(typeof(RecyclingWells), "OnNetworkSpawn")]    RecyclingWells为类型名称，OnNetworkSpawn为函数名称
        //public class RecyclingWellsOverridePatch_OnNetworkSpawn
        //{
        //    [HarmonyPrefix]                                         前置补丁的补丁函数前的声明
        //    public static void Prefix(RecyclingWells __instance)    __instance为特殊名称，当前注入类型入口
        //    {
        //        Debug.Log($"ZG:修改前{Traverse.Create(__instance).Field("initialNumberOfUses").GetValue()}");
        //        Traverse.Create(__instance).Field("initialNumberOfUses").SetValue(99);
        //        Debug.Log($"ZG:修改后{99}");
        //    }
        //}


        #endregion

        #region 后补丁
        //Postfix后补丁，在函数执行后执行
        //[HarmonyPatch(typeof(Test), "Updata")]    Test为类型名称，Updata为函数名称
        //public class TestOverridePatch_Updata
        //{
        //    [HarmonyPostfix]                                         后置补丁的补丁函数前的声明
        //    public static void Postfix(ref int __result)    __result为特殊名称，当前函数的返回值
        //    {
        //        __result \= 2;
        //    }
        //}
        #endregion

        #region 多个同名函数补丁制作
        //[HarmonyPatch(typeof(HexMapManager), "GenerateNewMap", new Type[] { typeof(Sector) })]
        //在HexMapManager类里有多个名为GenerateNewMap的函数时，HarmonyPatch的第三个参数是函数输入变量的类型，第四个参数是函数out输出变量的类型
        #endregion

        #region 可读写属性补丁制作
        //[HarmonyPatch(typeof(MinigameChest), "Price", MethodType.Getter)]
        //[HarmonyPatch(typeof(MinigameChest), "Price", MethodType.Setter)]
        #endregion

        #region 成员修改
        //Traverse.Create(__instance).Field("initialNumberOfUses").GetValue<T>();
        //Traverse.Create(__instance).Field("initialNumberOfUses").SetValue(99);
        //initialNumberOfUses 是成员名称， T为该成员类型
        #endregion

        //[HarmonyPatch(typeof(JTW.ga), "ProgressCurrentResearch")]
        //public class GameManagerOverridePatch_ProgressCurrentResearch
        //{
        //    [HarmonyPrefix]
        //    public static void Prefix(GameManager __instance)
        //    {
        //        if (!ResearchRate)
        //            return;

        //        GraphicsManager GameGraphics = Traverse.Create(__instance).Field("GameGraphics").GetValue<GraphicsManager>();
        //        if (GameGraphics == null)
        //            return;

        //        CardData card = UniqueIDScriptable.GetFromID<CardData>(GameGraphics.BlueprintModelsPopup.CurrentResearch.UniqueID);
        //        Debug.Log(card.CardName);

        //        if (ChangedValue.Contains(GameGraphics.BlueprintModelsPopup.CurrentResearch))
        //            return;

        //        GameGraphics.BlueprintModelsPopup.CurrentResearch.BlueprintUnlockSunsCost = card.BlueprintUnlockSunsCost / 2;
        //        ChangedValue.Add(GameGraphics.BlueprintModelsPopup.CurrentResearch);
        //    }

        //}


        [HarmonyPatch(typeof(ItemManager), "LoadAllItems")]
        public class ItemManagerOverridePatch_LoadAllItems
        {
            [HarmonyPostfix]
            public static void Postfix(ItemManager __instance)
            {
                if (ScriptTrainer.ChangeStackSize.Value || ScriptTrainer.ChangeToolUse.Value)
                {
                    allAvailableItems.Clear();
                    allAvailableItems = Resources.LoadAll<Item_Base>("IItems").ToList<Item_Base>();
                    foreach (var item in allAvailableItems)
                    {
                        var tmp = item.settings_Inventory;
                        if (ScriptTrainer.ChangeStackSize.Value && tmp.StackSize != 1)
                        {
                            int newsize = tmp.StackSize * ScriptTrainer.StackSizeMult.Value;
                            Traverse.Create(tmp).Field("stackSize").SetValue(newsize);
                            item.settings_Inventory = tmp;
                        }
                        if (ScriptTrainer.ChangeToolUse.Value && item.MaxUses != 1)
                        {
                            Traverse.Create(item).Field("maxUses").SetValue(item.MaxUses * ScriptTrainer.ToolUseMult.Value);
                        }
                    }
                    Traverse.Create(__instance).Field("allAvailableItems").SetValue(allAvailableItems);
                }
            }
        }

        [HarmonyPatch(typeof(Pickup), "AddItemToInventory")]
        public class PickupOverridePatch_AddItemToInventory
        {
            [HarmonyPrefix]
            public static bool Prefix(Pickup __instance, PickupItem item)
            {
                if(ScriptTrainer.ChangePick.Value)
                {
                    if (item == null)
                    {
                        return false;
                    }
                    PlayerInventory inventory = Traverse.Create(__instance).Field("playerInventory").GetValue<PlayerInventory>();
                    if (item.itemInstance != null && item.itemInstance.Valid)
                    {
                        inventory.AddItem(item.itemInstance, true);
                        return false;
                    }
                    if (item.yieldHandler != null)
                    {
                        if (item.yieldHandler.Yield.Count == 0)
                        {
                            goto IL_D7;
                        }
                        using (List<Cost>.Enumerator enumerator = item.yieldHandler.Yield.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                Cost cost = enumerator.Current;
                                inventory.AddItem(cost.item.UniqueName, cost.amount * ScriptTrainer.PickMult.Value);
                            }
                            goto IL_D7;
                        }
                    }
                    if (item != null && !(item is QuestItemPickup))
                    {
                        Debug.Log("Yield handler is null" + item.gameObject.name, __instance.gameObject);
                    }
                IL_D7:
                    if (item.dropper != null)
                    {
                        Item_Base[] randomItems = item.dropper.GetRandomItems();
                        for (int i = 0; i < randomItems.Length; i++)
                        {
                            if (randomItems[i] != null)
                            {
                                inventory.AddItem(randomItems[i].UniqueName, 1 * ScriptTrainer.PickMult.Value);
                            }
                        }
                    }
                    if (item.specificPickups != null)
                    {
                        Pickup_Specific[] specificPickups = item.specificPickups;
                        for (int j = 0; j < specificPickups.Length; j++)
                        {
                            specificPickups[j].PickupSpecific(inventory);
                        }
                    }
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(SO_ObjectSpawner), "GetSettings")]
        public class SO_ObjectSpawnerOverridePatch_GetSettings
        {
            [HarmonyPostfix]
            public static void Postfix(ref ObjectSpawnerAssetSettings __result)
            {
                if (ScriptTrainer.ChangeSpawnSettings.Value)
                {
                    if(ScriptTrainer.SpawnSettingsMult.Value > 1)
                    {
                        __result.spawnAmount.minValue = (int)(__result.spawnAmount.minValue * ScriptTrainer.SpawnSettingsMult.Value);
                        __result.spawnAmount.maxValue = (int)(__result.spawnAmount.maxValue * ScriptTrainer.SpawnSettingsMult.Value);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Slot), "SetItem", new Type[]
        {
            typeof(ItemInstance)
        })]
        public class SlotOverridePatch_SetItem
        {
            [HarmonyPrefix]
            public static void Prefix(ItemInstance newInstance)
            {
                if(newInstance != null && ScriptTrainer.ChangeToolUse.Value)
                {
                    if(newInstance.baseItem.MaxUses > 1)
                        newInstance.SetUsesToMax();
                }
            }
        }

        [HarmonyPatch(typeof(PlayerStats), "Start")]
        public class PlayerStatsOverridePatch_Start
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerStats __instance)
            {
                if (__instance.stat_hunger != null)
                {
                    __instance.stat_hunger.normalConsumable.SetMaxValue(__instance.stat_hunger.normalConsumable.Max * 5);
                    __instance.stat_hunger.normalConsumable.statTarget.SetMaxValue(__instance.stat_hunger.normalConsumable.statTarget.Max * 5);
                    __instance.stat_hunger.normalConsumable.statTarget.Value *= 5;
                    __instance.stat_hunger.normalConsumable.Value *= 5;
                }
                if (__instance.stat_thirst != null)
                {
                    __instance.stat_thirst.normalConsumable.SetMaxValue(__instance.stat_thirst.normalConsumable.Max * 5);
                    __instance.stat_thirst.normalConsumable.statTarget.SetMaxValue(__instance.stat_thirst.normalConsumable.statTarget.Max * 5);
                    __instance.stat_thirst.normalConsumable.statTarget.Value *= 5;
                    __instance.stat_thirst.normalConsumable.Value *= 5;
                }
                __instance.stat_health?.SetMaxValue(__instance.stat_health.Max * 5);
                __instance.stat_oxygen?.SetMaxValue(__instance.stat_oxygen.Max * 5);
            }
        }

        [HarmonyPatch(typeof(Cheat), "LoadSteamUserSO")]
        public class CheatOverridePatch_LoadSteamUserSO
        {
            [HarmonyPostfix]
            public static void Postfix(Cheat __instance)
            {
                if (AllowCheat)
                {
                    var steamUsers = Resources.LoadAll<SO_SteamUser>("SteamUsers");
                    if (ComponentManager<Network_Player>.Value != null)
                    {
                        steamUsers[0].ID = ComponentManager<Network_Player>.Value.steamID.m_SteamID;
                    }
                    Traverse.Create(__instance).Field("steamUsers").SetValue(steamUsers);
                    Cheat.UseCheats = true;
                }
            }
        }


        [HarmonyPatch(typeof(FishingRod), "Update")]
        public class FishingRodOverridePatch_Update
        {
            [HarmonyPostfix]
            public static void Postfix(FishingRod __instance)
            {
                if (__instance.GetComponentInParent<Network_Player>().IsLocalPlayer)
                {
                    if (__instance.bobber.FishIsOnHook)
                    {
                        var m = AccessTools.Method(typeof(FishingRod), "PullItemsFromSea");
                        m.Invoke(__instance, null);
                        return;
                    }
                }
                
            }
        }

        [HarmonyPatch(typeof(Bobber), "FishOnHook")]
        public class BobberOverridePatch_FishOnHook
        {
            [HarmonyPrefix]
            public static void Prefix(ref float timeDelay)
            {
                timeDelay *= 0.5f;
            }
        }


        //[HarmonyPatch(typeof(ChunkManager), "Update")]
        //public class ChunkManagerOverridePatch_Update
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(ChunkManager __instance)
        //    {
        //        //Debug.Log("泽哥：");
        //        //Debug.Log(Traverse.Create(__instance).Field("chunkCheckUpdateProgress").GetValue());
        //        //Debug.Log(Traverse.Create(__instance).Field("chunkCheckUpdateRate").GetValue());
        //    }
        //}
    }
}
