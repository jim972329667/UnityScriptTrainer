using JTW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using DV;
using UnityEngine;

namespace ScriptTrainer.Cards
{
    public static class DynamicCardCreator
    {
        public static bool Init = false;
        public static Dictionary<string, List<Card>> Cards = new Dictionary<string, List<Card>>();

        public static Dictionary<string, string> ActionNames = new Dictionary<string, string>() 
        {
            {"Attack","攻击"},
            {"Block","防御"},
            {"DrawCards","抽牌"},
            {"ChangeHP","增加血量"},
            {"ChangeEnergy","增加能量"},
            {"Regeneration","回复"},
            {"LivingArmor","活体护甲"},
            {"Evasion","闪避"},
            {"Blessed","祝福"},
            {"Flying","飞行"},
            {"InvinciblePerDamage","霸体"},
            {"Invisible","隐身"},
            {"Artifact","守护"},
            {"Strength","力量"},
            {"Constitution","坚韧"},
            {"Poison","中毒"},
            {"Bleeding","流血"},
            {"Burning","燃烧"},
            {"Wet","潮湿"},
            {"Rupture","内伤"},
            {"Sleeping","熟睡"},
            {"Cursed","诅咒"},
            {"Entangled","缠绕"},
            {"Blind","失明"},
            {"Shocked","震惊"},
            {"Freezing","冰冻"},
            {"Weak","虚弱"},
            {"Frail","脆弱"},
            {"Vulnerable","易伤"},
            {"Confused","困惑"},
            {"Stunned","眩晕"},
            {"Apparition","凶蚀"},
            {"Good","善"},
            {"Evil","恶"},
            {"EnergyDrain","能量吸收"},
            {"Charmed","诱惑"},
            {"Strangled","绞杀"}
        };
        public static List<string> CardTypes = new List<string>()
        {
            "JTW.Wukong",
            "JTW.WhiteDragon",
            "JTW.HolyMonk",
            "JTW.FaithfulDevil",
            "JTW.TheSwineKing",
            "JTW.BladeSouls",
            "Neutral"
        };

        public static List<string> CardNames = new List<string>();

        public static List<ModCardInfo> ModCardInfos = new List<ModCardInfo>();

        public static List<string> BannedList = new List<string>();
        public static void GenerateCardName()
        {
            IEnumerable<Type> enumerable = from t in Assembly.GetAssembly(typeof(Card)).GetTypes()
                                           where t.IsSubclassOf(typeof(Card))
                                           select t;
            foreach (Type type in enumerable)
            {
                if (!type.IsAbstract && !(type.GetConstructor(Type.EmptyTypes) == null))
                {
                    Card card = (Card)Activator.CreateInstance(type);
                    CardNames.Add(card.GetOriginalName());
                }
            }
        }
        public static bool CheckCardName(this string name)
        {
            if (CardNames.Count == 0)
                GenerateCardName();
            return !CardNames.Contains(name);
        }
        public static Card CreateDynamicCard(ModCardInfo cardInfo)
        {
            IModCard card;
            switch (cardInfo.CardType)
            {
                case "JTW.Wukong":
                    card = new ModWukongCard(); break;
                case "JTW.WhiteDragon":
                    card = new ModWhiteDragonCard(); break;
                case "JTW.TheSwineKing":
                    card = new ModSwineKingCard(); break;
                case "JTW.HolyMonk":
                    card = new ModHolyMonkCard(); break;
                case "JTW.BladeSouls":
                    card = new ModBladeSoulsCard(); break;
                case "JTW.FaithfulDevil":
                    card = new ModFaithfulDevilCard(); break;
                case "Neutral":
                default:
                    cardInfo.CardType = "Neutral";
                    card = new ModNeutralCard(); break;

            }
            Card basecard = card as Card;
            card.SetModInfo(cardInfo);
            basecard.Rarity = cardInfo.CardRarity;//RARE,COMMON,UNCOMMON
            basecard.Name = cardInfo.CardName;
            basecard.EnergyCost = cardInfo.CardEnergyCost;
            basecard.Action = new CombatAction(cardInfo.CardName, cardInfo.CardCombatType, cardInfo.CardTargetType);

            basecard.Action.ActionInternal = CreatActionList(cardInfo, card);

            if (cardInfo.CardImageAssetPath != "")
            {
                basecard.ImageAssetPath = cardInfo.CardImageAssetPath;
            }
            return basecard;
        }
        public static ActionList CreatActionList(ModCardInfo cardInfo, IModCard card)
        {
            ActionList actionList = new ActionList();

            if (cardInfo.CardCombatType == CombatAction.CombatActionType.ATTACK)
            {
                actionList.AddAction<ATAnimation>(null).AnimationName = "Attack";
            }
            else
            {
                actionList.AddAction<ATAnimation>(null).AnimationName = "Skill";
            }

            foreach (string x in cardInfo.Actions)
            {
                x.SpawnAction(ref actionList, ref card, cardInfo.CardTargetType);
            }
            return actionList;
        }
        public static void AppendCardToGame(ModCardInfo cardInfo)
        {
            if(cardInfo == null) return;
            if (ModCardInfos.Exists(o => o.CardName == cardInfo.CardName))
            {
                Cards[cardInfo.CardType].RemoveAll(o => o is IModCard card && card.GetModInfo().CardName == cardInfo.CardName);
                ModCardInfos.RemoveAll(o => o.CardName == cardInfo.CardName);
                CardNames.Remove(cardInfo.CardName);
            }
            Card basecard = CreateDynamicCard(cardInfo);

            if (!Cards.ContainsKey(cardInfo.CardType))
            {
                Cards.Add(cardInfo.CardType, new List<Card>());
            }
            Cards[cardInfo.CardType].Add(basecard);
            CardNames.Add(cardInfo.CardName);
            ModCardInfos.Add(cardInfo);
            if (cardInfo.Banned)
            {
                if (!BannedList.Contains(cardInfo.CardName))
                {
                    BannedList.Add(cardInfo.CardName);
                }
            }
            else
            {
                if (BannedList.Contains(cardInfo.CardName))
                {
                    BannedList.Remove(cardInfo.CardName);
                }
            }
        }
        public static void AppendCardToGame(string cardFile)
        {
            ModCardInfo cardInfo = ModCardInfo.ReadFromFile(cardFile);
            var file = new FileInfo(cardFile);
            var name = file.Name.Replace(file.Extension, "");
            if (name != cardInfo.CardName)
                cardInfo.CardFilePath = cardFile;
            else
                cardInfo.CardFilePath = "";
            if (!CheckCardName(cardInfo.CardName))
                cardInfo.CardName = Guid.NewGuid().ToString();
            cardInfo.SaveToFile(cardFile);
            AppendCardToGame(cardInfo);
        }
        public static ATCombat SpawnAction(this string cmd, ref ActionList actionList, ref IModCard card, CombatAction.ActionTargetType CardTargetType)
        {
            var line = cmd.Split(';');
            //0:类型（Action,Value）
            //1:动作数值
            //2:作用对象
            //3？:升级后变化
            if(line.Length < 3)
            {
                return null;
            }

            int.TryParse(line[1], out int intValue);
            float.TryParse(line[1], out float floatValue);

            ATCombat action = null;
            string actionName = $"{line[0]}_{actionList.GetActionCount()}";
            switch (line[0])
            {
                case "Attack":
                    var Attack = actionList.AddAction<ATAttack>(actionName, null);
                    Attack.Damage = intValue;
                    Attack.DamageType = "Pierce";
                    action = Attack;
                    break;
                case "Block":
                    var Block = actionList.AddAction<ATBlock>(actionName, null);
                    Block.BlockValue = intValue;
                    action = Block;
                    break;
                case "DrawCards":
                    var DrawCards = actionList.AddAction<ATDrawCards>(actionName, null);
                    DrawCards.Count = intValue;
                    action = DrawCards;
                    break;
                case "ChangeHP":
                    var ChangeHP = actionList.AddAction<ATChangeHPs>(actionName, null);
                    ChangeHP.Change = intValue;
                    ChangeHP.PreFunc = delegate (DV.Action _action)
                    {
                        ChangeHP.Source = CombatAction.GetCurrentSource().GenerateDataSource();
                    };
                    action = ChangeHP;
                    break;
                case "ChangeEnergy":
                    var ChangeEnergy = actionList.AddAction<ATChangeEnergy>(actionName, null);
                    ChangeEnergy.Change = intValue;
                    ChangeEnergy.PreFunc = delegate (DV.Action _action)
                    {
                        ChangeEnergy.Source = CombatAction.GetCurrentSource().GenerateDataSource();
                    };
                    action = ChangeEnergy;
                    break;
                case "Regeneration"://回复
                    var Regeneration = actionList.AddAction<ATAddStatus>(actionName, null);
                    var Statu = new STRegeneration();
                    Statu.Count = intValue;
                    if(intValue > Statu.Cap)
                        Statu.Cap = intValue;
                    Regeneration.Status = Statu;
                    action = Regeneration;
                    break;
                case "LivingArmor"://活体护甲
                    var LivingArmor = actionList.AddAction<ATAddStatus>(actionName, null);
                    var Statu2 = new STLivingArmor();
                    Statu2.Count = intValue;
                    if (intValue > Statu2.Cap)
                        Statu2.Cap = intValue;
                    LivingArmor.Status = Statu2;
                    action = LivingArmor;
                    break;
                case "Evasion"://闪避
                    var Evasion = actionList.AddAction<ATAddStatus>(actionName, null);
                    Evasion.Status = new STEvasion
                    {
                        Count = intValue
                    };
                    action = Evasion;
                    break;
                case "Blessed"://祝福
                    var Blessed = actionList.AddAction<ATAddStatus>(actionName, null);
                    Blessed.Status = new STBlessed
                    {
                        Count = intValue
                    };
                    action = Blessed;
                    break;
                case "Flying"://飞行
                    var Flying = actionList.AddAction<ATAddStatus>(actionName, null);
                    Flying.Status = new STFlying
                    {
                        Count = intValue
                    };
                    action = Flying;
                    break;
                case "InvinciblePerDamage"://霸体
                    var InvinciblePerDamage = actionList.AddAction<ATAddStatus>(actionName, null);
                    InvinciblePerDamage.Status = new STInvinciblePerDamage
                    {
                        Count = intValue
                    };
                    action = InvinciblePerDamage;
                    break;
                case "Invisible"://隐身
                    var Invisible = actionList.AddAction<ATAddStatus>(actionName, null);
                    Invisible.Status = new STInvisible
                    {
                        Length = intValue
                    };
                    action = Invisible;
                    break;
                case "Artifact"://守护
                    var Artifact = actionList.AddAction<ATAddStatus>(actionName, null);
                    Artifact.Status = new STArtifact
                    {
                        Count = intValue
                    };
                    action = Artifact;
                    break;
                case "Strength"://力量
                    var Strength = actionList.AddAction<ATAddStatus>(actionName, null);
                    Strength.Status = new STStrength
                    {
                        Count = intValue
                    };
                    action = Strength;
                    break;
                case "Constitution"://坚韧
                    var Constitution = actionList.AddAction<ATAddStatus>(actionName, null);
                    Constitution.Status = new STConstitution
                    {
                        Count = intValue
                    };
                    action = Constitution;
                    break;
                case "Poison"://中毒
                    var Poison = actionList.AddAction<ATAddStatus>(actionName, null);
                    Poison.Status = new STPoison
                    {
                        Count = intValue
                    };
                    action = Poison;
                    break;
                case "Bleeding"://流血
                    var Bleeding = actionList.AddAction<ATAddStatus>(actionName, null);
                    Bleeding.Status = new STBleeding
                    {
                        Count = intValue
                    };
                    action = Bleeding;
                    break;
                case "Burning"://燃烧
                    var Burning = actionList.AddAction<ATAddStatus>(actionName, null);
                    Burning.Status = new STBurning
                    {
                        Count = intValue
                    };
                    action = Burning;
                    break;
                case "Wet"://潮湿
                    var Wet = actionList.AddAction<ATAddStatus>(actionName, null);
                    Wet.Status = new STWet
                    {
                        Count = intValue
                    };
                    action = Wet;
                    break;
                case "Rupture"://内伤
                    var Rupture = actionList.AddAction<ATAddStatus>(actionName, null);
                    Rupture.Status = new STRupture
                    {
                        Count = intValue
                    };
                    action = Rupture;
                    break;
                case "Sleeping"://熟睡
                    var Sleeping = actionList.AddAction<ATAddStatus>(actionName, null);
                    Sleeping.Status = new STSleeping
                    {
                        Count = intValue
                    };
                    action = Sleeping;
                    break;
                case "Cursed"://诅咒
                    var Cursed = actionList.AddAction<ATAddStatus>(actionName, null);
                    Cursed.Status = new STCursed
                    {
                        Count = intValue
                    };
                    action = Cursed;
                    break;
                case "Entangled"://缠绕
                    var Entangled = actionList.AddAction<ATAddStatus>(actionName, null);
                    Entangled.Status = new STEntangled();
                    action = Entangled;
                    break;
                case "Blind"://失明
                    var Blind = actionList.AddAction<ATAddStatus>(actionName, null);
                    Blind.Status = new STBlind() { Length = intValue };
                    action = Blind;
                    break;
                case "Shocked"://震惊
                    var Shocked = actionList.AddAction<ATAddStatus>(actionName, null);
                    Shocked.Status = new STShocked();
                    action = Shocked;
                    break;
                case "Freezing"://冰冻
                    var Freezing = actionList.AddAction<ATAddStatus>(actionName, null);
                    Freezing.Status = new STFreezing() { Length = intValue };
                    action = Freezing;
                    break;
                case "Weak"://虚弱
                    var Weak = actionList.AddAction<ATAddStatus>(actionName, null);
                    Weak.Status = new STWeak() { Length = intValue };
                    action = Weak;
                    break;
                case "Frail"://脆弱
                    var Frail = actionList.AddAction<ATAddStatus>(actionName, null);
                    Frail.Status = new STFrail() { Length = intValue };
                    action = Frail;
                    break;
                case "Vulnerable"://易伤
                    var Vulnerable = actionList.AddAction<ATAddStatus>(actionName, null);
                    Vulnerable.Status = new STVulnerable() { Length = intValue };
                    action = Vulnerable;
                    break;
                case "Confused"://困惑
                    var Confused = actionList.AddAction<ATAddStatus>(actionName, null);
                    Confused.Status = new STConfused() { Length = intValue };
                    action = Confused;
                    break;
                case "Stunned"://眩晕
                    var Stunned = actionList.AddAction<ATAddStatus>(actionName, null);
                    Stunned.Status = new STStunned();
                    action = Stunned;
                    break;
                case "Apparition"://凶蚀
                    var Apparition = actionList.AddAction<ATAddStatus>(actionName, null);
                    Apparition.Status = new STApparition() { Count = intValue };
                    action = Apparition;
                    break;
                case "Good"://善
                    var Good = actionList.AddAction<ATAddStatus>(actionName, null);
                    Good.Status = new STGood() { Count = intValue };
                    action = Good;
                    break;
                case "Evil"://恶
                    var Evil = actionList.AddAction<ATAddStatus>(actionName, null);
                    Evil.Status = new STEvil() { Count = intValue };
                    action = Evil;
                    break;
                case "EnergyDrain"://能量吸收
                    var EnergyDrain = actionList.AddAction<ATAddStatus>(actionName, null);
                    EnergyDrain.Status = new STEnergyDrain() { Count = intValue };
                    action = EnergyDrain;
                    break;
                case "Charmed"://诱惑
                    var Charmed = actionList.AddAction<ATAddStatus>(actionName, null);
                    Charmed.Status = new STCharmed() { Length = intValue };
                    action = Charmed;
                    break;
                case "Strangled"://绞杀
                    STStrangled sT = new STStrangled
                    {
                        Count = intValue
                    };
                    if(sT.MaxCount < intValue)
                        sT.MaxCount = intValue;
                    var Strangled = actionList.AddAction<ATAddStatus>(actionName, null);
                    Strangled.Status = sT;
                    Strangled.PreFunc = delegate (DV.Action _action)
                    {
                        sT.Source = CombatAction.GetCurrentSource();
                    };
                    action = Strangled;
                    break;
                default:
                    break;
            }

            //CombatObject self = CombatAction.GetCurrentSource();

            //List<CombatObject> allies = Combat.Get().GetAllies(self, false);
            //List<CombatObject> enemies = Combat.Get().GetEnemies(self, false);
            if (line[2] != CardTargetType.ToString())
            {
                if (action is ATAttack || action is ATBlock || action is ATChangeHPs || action is ATAddStatus)
                {
                    switch (line[2])
                    {
                        case "ENEMY_SINGLE":
                        case "ENEMY_SINGLE_RANDOM":
                            action.PreFunc += delegate (DV.Action _action)
                            {
                                var enemie = Combat.Get().RandomPickAnEnemyTarget(CombatAction.GetCurrentSource()).GenerateDataSourceList();
                                _action.GetType().GetProperty("Targets")?.SetValue(_action, enemie);
                                _action.GetType().GetField("Targets")?.SetValue(_action, enemie);
                            };
                            CombatAction.AdjustDescriptionTargetTypeTemporarily(action, CombatAction.ActionTargetType.ENEMY_SINGLE_RANDOM);
                            break;
                        case "ENEMY_ALL":
                            action.PreFunc += delegate (DV.Action _action)
                            {
                                CombatObject self = CombatAction.GetCurrentSource();
                                List<CombatObject> enemies = Combat.Get().GetEnemies(self, false);
                                _action.GetType().GetProperty("Targets")?.SetValue(_action, new DataSourceValue<List<CombatObject>>(enemies));
                                _action.GetType().GetField("Targets")?.SetValue(_action, new DataSourceValue<List<CombatObject>>(enemies));
                            };
                            CombatAction.AdjustDescriptionTargetTypeTemporarily(action, CombatAction.ActionTargetType.ENEMY_ALL, delegate ()
                            {
                                if (CombatAction.CheckSourceExists())
                                {
                                    var target = new DataSourceValue<List<CombatObject>>(Combat.Get().GetEnemies(CombatAction.GetCurrentSource(), false));
                                    action.GetType().GetProperty("Targets")?.SetValue(action, target);
                                    action.GetType().GetField("Targets")?.SetValue(action, target);
                                }
                            });
                            break;
                        case "SELF":
                            action.PreFunc += delegate (DV.Action _action)
                            {
                                CombatObject self = CombatAction.GetCurrentSource();
                                _action.GetType().GetProperty("Targets")?.SetValue(_action, self.GenerateDataSourceList());
                                _action.GetType().GetField("Targets")?.SetValue(_action, self.GenerateDataSourceList());
                            };
                            CombatAction.AdjustDescriptionTargetTypeTemporarily(action, CombatAction.ActionTargetType.SELF, delegate ()
                            {
                                if (CombatAction.CheckSourceExists())
                                {
                                    var target = CombatAction.GetCurrentSource().GenerateDataSourceList();
                                    action.GetType().GetProperty("Targets")?.SetValue(action, target);
                                    action.GetType().GetField("Targets")?.SetValue(action, target);
                                }
                            });

                            break;
                        case "ALLY_SINGLE":
                            action.PreFunc += delegate (DV.Action _action)
                            {
                                var ally = ModCardUtil.RandomPickAlliesTarget(true)?.GenerateDataSourceList();
                                _action.GetType().GetProperty("Targets")?.SetValue(_action, ally);
                                _action.GetType().GetField("Targets")?.SetValue(_action, ally);
                            };
                            CombatAction.AdjustDescriptionTargetTypeTemporarily(action, CombatAction.ActionTargetType.ALLY_SINGLE, delegate ()
                            {
                                if (CombatAction.CheckSourceExists())
                                {
                                    var target = ModCardUtil.RandomPickAlliesTarget(true)?.GenerateDataSourceList();
                                    action.GetType().GetProperty("Targets")?.SetValue(action, target);
                                    action.GetType().GetField("Targets")?.SetValue(action, target);
                                }
                            });
                            break;
                        case "ALLY_SINGLE_NO_SELF":
                            action.PreFunc += delegate (DV.Action _action)
                            {
                                var ally = ModCardUtil.RandomPickAlliesTarget(false)?.GenerateDataSourceList();
                                _action.GetType().GetProperty("Targets")?.SetValue(_action, ally);
                                _action.GetType().GetField("Targets")?.SetValue(_action, ally);
                            };
                            CombatAction.AdjustDescriptionTargetTypeTemporarily(action, CombatAction.ActionTargetType.ALLY_SINGLE_NO_SELF, delegate ()
                            {
                                if (CombatAction.CheckSourceExists())
                                {
                                    var target = ModCardUtil.RandomPickAlliesTarget(false)?.GenerateDataSourceList();
                                    action.GetType().GetProperty("Targets")?.SetValue(action, target);
                                    action.GetType().GetField("Targets")?.SetValue(action, target);
                                }
                            });
                            break;
                        case "ALLY_ALL":
                            action.PreFunc += delegate (DV.Action _action)
                            {
                                CombatObject self = CombatAction.GetCurrentSource();
                                List<CombatObject> allies = Combat.Get().GetAllies(self, false);
                                _action.GetType().GetProperty("Targets")?.SetValue(_action, new DataSourceValue<List<CombatObject>>(allies));
                                _action.GetType().GetField("Targets")?.SetValue(_action, new DataSourceValue<List<CombatObject>>(allies));
                            };
                            CombatAction.AdjustDescriptionTargetTypeTemporarily(action, CombatAction.ActionTargetType.ALLY_ALL, delegate ()
                            {
                                if (CombatAction.CheckSourceExists())
                                {
                                    var target = new DataSourceValue<List<CombatObject>>(Combat.Get().GetAllies(CombatAction.GetCurrentSource(), false));
                                    action.GetType().GetProperty("Targets")?.SetValue(action, target);
                                    action.GetType().GetField("Targets")?.SetValue(action, target);
                                }
                            });
                            break;
                        case "ALLY_ALL_NO_SELF":
                            action.PreFunc += delegate (DV.Action _action)
                            {
                                CombatObject self = CombatAction.GetCurrentSource();
                                List<CombatObject> allies = Combat.Get().GetAllies(self, false);
                                allies.Remove(self);
                                _action.GetType().GetProperty("Targets")?.SetValue(_action, new DataSourceValue<List<CombatObject>>(allies));
                                _action.GetType().GetField("Targets")?.SetValue(_action, new DataSourceValue<List<CombatObject>>(allies));
                            };
                            CombatAction.AdjustDescriptionTargetTypeTemporarily(action, CombatAction.ActionTargetType.ALLY_ALL_NO_SELF, delegate ()
                            {
                                if (CombatAction.CheckSourceExists())
                                {
                                    CombatObject self = CombatAction.GetCurrentSource();
                                    List<CombatObject> allies = Combat.Get().GetAllies(self, false);
                                    allies.Remove(self);
                                    var target = new DataSourceValue<List<CombatObject>>(allies);
                                    action.GetType().GetProperty("Targets")?.SetValue(action, target);
                                    action.GetType().GetField("Targets")?.SetValue(action, target);
                                }
                            });
                            break;
                        case "ALL":
                            action.PreFunc += delegate (DV.Action _action)
                            {
                                CombatObject self = CombatAction.GetCurrentSource();
                                List<CombatObject> allies = Combat.Get().GetAllies(self, false);
                                allies.AddRange(Combat.Get().GetEnemies(self, false));
                                _action.GetType().GetProperty("Targets")?.SetValue(_action, new DataSourceValue<List<CombatObject>>(allies));
                                _action.GetType().GetField("Targets")?.SetValue(_action, new DataSourceValue<List<CombatObject>>(allies));
                            };
                            CombatAction.AdjustDescriptionTargetTypeTemporarily(action, CombatAction.ActionTargetType.ALL, delegate ()
                            {
                                if (CombatAction.CheckSourceExists())
                                {
                                    CombatObject self = CombatAction.GetCurrentSource();
                                    List<CombatObject> allies = Combat.Get().GetAllies(self, false);
                                    allies.AddRange(Combat.Get().GetEnemies(self, false));
                                    var target = new DataSourceValue<List<CombatObject>>(allies);
                                    action.GetType().GetProperty("Targets")?.SetValue(action, target);
                                    action.GetType().GetField("Targets")?.SetValue(action, target);
                                }
                            });
                            break;
                        case "ALL_NO_SELF":
                            action.PreFunc += delegate (DV.Action _action)
                            {
                                CombatObject self = CombatAction.GetCurrentSource();
                                List<CombatObject> allies = Combat.Get().GetAllies(self, false);
                                allies.AddRange(Combat.Get().GetEnemies(self, false));
                                allies.Remove(self);
                                _action.GetType().GetProperty("Targets")?.SetValue(_action, new DataSourceValue<List<CombatObject>>(allies));
                                _action.GetType().GetField("Targets")?.SetValue(_action, new DataSourceValue<List<CombatObject>>(allies));
                            };
                            CombatAction.AdjustDescriptionTargetTypeTemporarily(action, CombatAction.ActionTargetType.ALLY_ALL_NO_SELF, delegate ()
                            {
                                CombatObject self = CombatAction.GetCurrentSource();
                                List<CombatObject> allies = Combat.Get().GetAllies(self, false);
                                allies.AddRange(Combat.Get().GetEnemies(self, false));
                                allies.Remove(self);
                                var target = new DataSourceValue<List<CombatObject>>(allies);
                                action.GetType().GetProperty("Targets")?.SetValue(action, target);
                                action.GetType().GetField("Targets")?.SetValue(action, target);
                            });
                            break;
                        case "SINGLE":
                            action.PreFunc += delegate (DV.Action _action)
                            {
                                var ally = ModCardUtil.RandomPickAllTarget(true)?.GenerateDataSourceList();
                                _action.GetType().GetProperty("Targets")?.SetValue(_action, ally);
                                _action.GetType().GetField("Targets")?.SetValue(_action, ally);
                            };
                            CombatAction.AdjustDescriptionTargetTypeTemporarily(action, CombatAction.ActionTargetType.SINGLE, delegate ()
                            {
                                if (CombatAction.CheckSourceExists())
                                {
                                    var target = ModCardUtil.RandomPickAlliesTarget(true)?.GenerateDataSourceList();
                                    action.GetType().GetProperty("Targets")?.SetValue(action, target);
                                    action.GetType().GetField("Targets")?.SetValue(action, target);
                                }
                            });
                            break;
                        case "SINGLE_NO_SELF":
                            action.PreFunc += delegate (DV.Action _action)
                            {
                                var ally = ModCardUtil.RandomPickAllTarget(false)?.GenerateDataSourceList();
                                _action.GetType().GetProperty("Targets")?.SetValue(_action, ally);
                                _action.GetType().GetField("Targets")?.SetValue(_action, ally);
                            };
                            CombatAction.AdjustDescriptionTargetTypeTemporarily(action, CombatAction.ActionTargetType.SINGLE, delegate ()
                            {
                                if (CombatAction.CheckSourceExists())
                                {
                                    var target = ModCardUtil.RandomPickAlliesTarget(false)?.GenerateDataSourceList();
                                    action.GetType().GetProperty("Targets")?.SetValue(action, target);
                                    action.GetType().GetField("Targets")?.SetValue(action, target);
                                }
                            });
                            break;
                        default: break;
                    }
                }
            }
            
            if (line.Length > 3)
            {
                card.AddValue(actionName + ModCardUtil.UpgradeAdditon, line[3]);
            }
            return action;
        }
        public static void RemoveModCard(ModCardInfo cardInfo)
        {
            string fileName = Path.Combine(ScriptTrainer.ModCardImgPath, cardInfo.CardName + ".card");
            if (cardInfo.CardFilePath != "")
                fileName = cardInfo.CardFilePath;
            ModCardInfos.RemoveAll(o => o.CardName == cardInfo.CardName);
            foreach (var cards in Cards)
            {
                cards.Value.RemoveAll(o => o is IModCard card && card.GetModInfo().CardName == cardInfo.CardName);
            }
            Debug.Log($"删除卡牌文件:{fileName}");
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
                Debug.Log($"删除卡牌文件:{fileName}");
            }
        }
        public static string MakeSafeFileName(string input)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            int validCharsIndex = 0;
            char[] validChars = new char[input.Length];

            foreach (char c in input)
            {
                if (Array.IndexOf(invalidChars, c) == -1)
                {
                    validChars[validCharsIndex] = c;
                    validCharsIndex++;
                }
            }

            if (validCharsIndex == 0) return string.Empty;
            if (validCharsIndex == input.Length) return input;
            return new string(validChars, 0, validCharsIndex);
        }
    }
}
