using DV;
using JTW;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScriptTrainer.Cards
{
    public interface IModCard
    {
        void SetModInfo(ModCardInfo info);
        ModCardInfo GetModInfo();
        void AddValue(string key, object value);
        string GetValue(string key);
        int GetIntValue(string key);
        float GetFloatValue(string key);
        List<string> GetValueKeys();
        DV.Action GetAction(string key);
        void SetActionActive(string key, bool active);
        int GetActionCount();
    }
    public static class ModCardUtil
    {
        public const string UpgradeAdditon = "_UpgradeValue";
        public static void ModCardUpgrade(this IModCard card, bool Upgrade = true)
        {
            var upgradeKeys = card.GetValueKeys().Where(key => key.EndsWith(UpgradeAdditon)).ToList();

            foreach (var ukey in upgradeKeys)
            {
                var newkey = ukey.Replace(UpgradeAdditon, "");

                float uf = card.GetFloatValue(ukey);
                int ui = card.GetIntValue(ukey);

                float of = card.GetFloatValue(newkey);
                int oi = card.GetIntValue(newkey);

                ATCombat value2 = (ATCombat)card.GetAction(newkey);
                if (!string.IsNullOrEmpty(card.GetValue(newkey)))
                {
                    if (newkey.StartsWith("F:"))
                    {
                        if (Upgrade)
                            card.AddValue(newkey, $"{of + uf}");
                        else
                            card.AddValue(newkey, $"{of - uf}");
                    }
                    else
                    {
                        if (Upgrade)
                            card.AddValue(newkey, $"{oi + ui}");
                        else
                            card.AddValue(newkey, $"{oi - ui}");
                    }
                }
                else if(value2 != null)
                {
                    if(value2 is ATAttack aT)
                    {
                        if (Upgrade)
                        {
                            aT.Damage += ui;
                        }
                        else
                        {
                            aT.Damage -= ui;
                        }
                    }
                    else if(value2 is ATBlock aTBlock)
                    {
                        if (Upgrade)
                        {
                            aTBlock.BlockValue += ui;
                        }
                        else
                        {
                            aTBlock.BlockValue -= ui;
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"{ukey}:卡牌升级失败，没有获取到{newkey}参数！");
                }
            }
        }
        public static CombatObject RandomPickAllTarget(bool hasSelf)
        {
            CombatObject self = CombatAction.GetCurrentSource();
            STTaunted status = self.GetStatus<STTaunted>("Taunted");
            if (status != null)
            {
                return status.Target;
            }
            List<CombatObject> list = new List<CombatObject>();

            List<CombatObject> old = new List<CombatObject>();
            old.AddRange(Combat.Get().GetAllies(self, false));
            old.AddRange(Combat.Get().GetEnemies(self, false));
            if(!hasSelf)
                old.Remove(self);

            foreach (CombatObject combatObject in old)
            {
                if (!combatObject.CheckDeadOrEscaping())
                {
                    List<Status> statuses = combatObject.GetStatuses(typeof(STProtected));
                    bool flag = false;
                    using (List<Status>.Enumerator enumerator2 = statuses.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            if (((STProtected)enumerator2.Current).Protector == self)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!flag && combatObject.CheckCanDirectTarget())
                    {
                        list.Add(combatObject);
                    }
                }
            }
            if (list.Count != 0)
            {
                int index = RandomInt.Next(list.Count);
                return list[index];
            }
            return null;
        }
        public static CombatObject RandomPickAlliesTarget(bool hasSelf)
        {
            CombatObject self = CombatAction.GetCurrentSource();
            STTaunted status = self.GetStatus<STTaunted>("Taunted");
            if (status != null)
            {
                return status.Target;
            }
            List<CombatObject> list = new List<CombatObject>();

            List<CombatObject> old = new List<CombatObject>();
            old.AddRange(Combat.Get().GetAllies(self, false));
            if (!hasSelf)
                old.Remove(self);

            foreach (CombatObject combatObject in old)
            {
                if (!combatObject.CheckDeadOrEscaping())
                {
                    List<Status> statuses = combatObject.GetStatuses(typeof(STProtected));
                    bool flag = false;
                    using (List<Status>.Enumerator enumerator2 = statuses.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            if (((STProtected)enumerator2.Current).Protector == self)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!flag && combatObject.CheckCanDirectTarget())
                    {
                        list.Add(combatObject);
                    }
                }
            }
            if (list.Count != 0)
            {
                int index = RandomInt.Next(list.Count);
                return list[index];
            }
            return null;
        }
    }
}
