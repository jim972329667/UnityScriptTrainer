using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityGameUI;
using static UnityEngine.UI.CanvasScaler;

namespace ScriptTrainer
{
    public class Scripts : MonoBehaviour
    {

        public static bool IsChangeMusic = false;
        public static BagItemData HeadItem = null;
        public Scripts()
        {
            
        }
        public static void ChangeAnthorMusic()
        {
            if (Singleton<BuffManager>.Instance.ExistEquipmentBuffById(Singleton<GameManager>.Instance.player, 90047, EquipType.Head))
            {
                IsChangeMusic = true;
                Singleton<WwiseManager>.Instance.SetState("GameMusicState", "None");
                Debug.Log("ZG:wancheng1");
            }
        }
        public static void ChangeAnthorMusicEnd()
        {
            Singleton<WwiseManager>.Instance.SetState("GameMusicState", "Author");
            Debug.Log("ZG:wancheng2");
            IsChangeMusic = false;
        }
        // 添加现金
        public static void AddMoney()
        {
            UIWindows.SpawnInputDialog("需要几倍金币掉落？", "添加", "5", (string money) => {
                if(money.ConvertToFloatDef(5) == 1)
                    ScriptPatch.IsDropMoney = false;
                else
                {
                    ScriptPatch.IsDropMoney = true;
                    if(money.ConvertToFloatDef(5) > 10)
                        ScriptPatch.DropMoneyRate = 10;
                    else
                        ScriptPatch.DropMoneyRate = money.ConvertToFloatDef(5);
                }
            });
        }
        public static void RemoveNeedMaterials(bool state)
        {
            ScriptPatch.IsRemoveNeedMaterials = state;
        }
        
        public static void AddDeadNum()
        {
            UIWindows.SpawnInputDialog("您需要添加多少死亡次数？", "添加", "100", (string dead) => {
                for(int i = 0;i< dead.ConvertToIntDef(100); i++)
                {
                    Singleton<AchivementManager>.Instance.AddUnitDeadNum(GameConst.PlayerUnitId);
                }
            });
        }
        public static void OpenCheat()
        {
            CombatTestMenu component = Singleton<GameManager>.Instance.player.gameObject.GetComponent<CombatTestMenu>();
            if (component != null)
            {
                UnityEngine.Object.Destroy(component);
            }
            else
            {
                Singleton<GameManager>.Instance.player.gameObject.AddComponent<CombatTestMenu>();
            }
        }
        public static void SkillCd(bool state)
        {
            DebugSetting.isClosePlayerSkillCd = state;
        }
        public static void WuDi(bool state)
        {
            if (!state)
            {
                Singleton<BuffManager>.Instance.RemoveById(Singleton<GameManager>.Instance.player, 102);
                return;
            }
            Singleton<BuffManager>.Instance.AddBuffByConfigId(Singleton<GameManager>.Instance.player, Singleton<GameManager>.Instance.player, 102, BuffSourceType.Normal);
        }
        public static void AutoPickItem(bool state)
        {
            if (!state)
            {
                Singleton<BuffManager>.Instance.RemoveById(Singleton<GameManager>.Instance.player, 90019);
                return;
            }
            Singleton<BuffManager>.Instance.AddBuffByConfigId(Singleton<GameManager>.Instance.player, Singleton<GameManager>.Instance.player, 90019, BuffSourceType.Normal);
        }
    }
}
