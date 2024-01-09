using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;



namespace ScriptTrainer
{
    public class Scripts
    {
        [MonoPInvokeCallback]
        public static void AddSkillPoint(int point)
        {
            if (NewGameSubMonitorPanel_NewCharacter.m_instance != null)
            {
                CharacterData newdata = NewGameSubMonitorPanel_NewCharacter.m_instance.characterData;
                newdata.characterSkillPoint += point;
            }
            else
            {
                GameData data = GameController.instance.gameData;
                data.playerData.characterSkillPoint += point;
            }
        }
        [MonoPInvokeCallback]
        public static void AddPerkPoint(int point)
        {
            if (NewGameSubMonitorPanel_NewCharacter.m_instance != null)
            {
                CharacterData newdata = NewGameSubMonitorPanel_NewCharacter.m_instance.characterData;
                newdata.characterPerkPoint += point;
            }
            else
            {
                GameData data = GameController.instance.gameData;
                data.playerData.characterPerkPoint += point;
            }
        }
        [MonoPInvokeCallback]
        public static void AddAttrPoint(int point)
        {
            if (NewGameSubMonitorPanel_NewCharacter.m_instance != null)
            {
                CharacterData newdata = NewGameSubMonitorPanel_NewCharacter.m_instance.characterData;
                newdata.characterAttrPoint += point;
            }
            else
            {
                GameData data = GameController.instance.gameData;
                data.playerData.characterAttrPoint += point;
            }
        }
        [MonoPInvokeCallback]
        public static void MaxBackpackSize(bool state)
        {
            if (state)
            {
                GameData data = GameController.instance.gameData;
                data.playerData.backpackSize = GetBackpackSize();
                CharacterData.defaultInventorySize = GetBackpackSize();
                ScriptPatch.UnChangeBackpackSize = true;
            }
            else
            {
                ScriptPatch.UnChangeBackpackSize = false;
            }
        }
        [MonoPInvokeCallback]
        public static void ZeroBackpackWeight(bool state)
        {
            if (state)
            {
                GameData data = GameController.instance.gameData;
                data.playerData.inventoryData.totalItemWeight = 0;
                data.playerData.totalEquipmentWeight = 0;
                ScriptPatch.UnChangeBackpackWeight = true;
            }
            else
            {
                ScriptPatch.UnChangeBackpackWeight = false;
            }
        }
        [MonoPInvokeCallback]
        public static Vector2Int GetBackpackSize()
        {
            string[] lines = ScriptTrainer.BackpackSize.Value.Replace("(", "").Replace(")", "").Replace(" ", "").Split(',');
            if(lines.Length>=2)
            {
                if (!int.TryParse(lines[0],out int X))
                {
                    X = 26;
                }
                if (!int.TryParse(lines[1],out int Y))
                {
                    Y = 41;
                }
                return new Vector2Int(X, Y);
            }
            else
            {
                return new Vector2Int(26, 41);
            }
        }
        [MonoPInvokeCallback]
        public static void AddCar()
        {
            var car = GameController.instance.GetRandomVehicleData();
            car.vehicleDurabilityTemp = 100;
            List<int> list = new List<int>();
            int tire = -1;
            for(int i = 0; i <= 10; i++)
            {
                VehiclePartData data = car.GetPartDataByPartType((VehiclePartType)i);
                data.durability = 100;
                data.hp = data.maxhp;
                if ((VehiclePartType)i == VehiclePartType.FuelTank)
                    data.fuel = car.vehicleFuelTankSize;
                if ((VehiclePartType)i >= VehiclePartType.Tire_FrontLeft)
                {
                    if (data.partItem != null)
                    {
                        tire = i;
                    }
                    else
                    {
                        ScriptTrainer.WriteLog($"缺少：{(VehiclePartType)i}");
                        list.Add(i);
                    }

                }
            }

            if(tire != -1 && list.Count > 0)
            {
                foreach(int i in list)
                {
                    ItemData Tir = car.GetPartDataByPartType((VehiclePartType)tire).partItem.CopyItemData();
                    ScriptTrainer.WriteLog($"添加：{(VehiclePartType)i}");
                    car.GetPartDataByPartType((VehiclePartType)i).partItem= Tir;
                }
            }

            Vector2 vector = GameController.instance.gameData.playerData.characterPosition;
            vector.y -= 5;
            car.worldCoordinate = vector;
            InGameController.instance.AddVehicle(car, GameController.instance.playerCharacter.currentChunk).fuelTankData.fuel = car.vehicleFuelTankSize;

        }
        [MonoPInvokeCallback]
        public static void Cure()
        {
            CharacterData data = GameController.instance.gameData.playerData;
            CharacterStatusData status = data.characterStatusData;
            status.hp = status.maxhp;
            status.bloodVolume = CharacterStatusData.maxBloodVolume;
            status.bodyTemperature = CharacterStatusData.standardBodyTemperature;

            foreach(var part in data.bodypartsData.bodypartDataAry)
            {
                part.hp = part.maxHP;
                part.bandageExpireTime = 0;
                part.bleedingStopTime = 0;
                if (part.bandageData != null)
                    part.bandageData = null;
            }
            var x = data.bodypartsData.bodypartDataAry;
            for(int i = 0;i<x.Length;i++)
            {
                x[i].hp = x[i].maxHP;
                x[i].bandageExpireTime = 0;
                x[i].bleedingStopTime = 0;
                if (x[i].bandageData != null)
                    x[i].bandageData = null;
            }
        }
    }
}
