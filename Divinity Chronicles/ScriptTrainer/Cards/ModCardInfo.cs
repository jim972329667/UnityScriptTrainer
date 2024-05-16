using JTW;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ScriptTrainer.Cards
{
    [Serializable]
    public class ModCardInfo : IEquatable<ModCardInfo>
    {
        public string CardName = "";
        public string CardType = "JTW.Wukong";
        public string CardDisplayName = "测试卡牌";
        public int CardEnergyCost = 1;
        public Rarity CardRarity = Rarity.COMMON;
        public CombatAction.CombatActionType CardCombatType = CombatAction.CombatActionType.ATTACK;
        public CombatAction.ActionTargetType CardTargetType = CombatAction.ActionTargetType.ENEMY_SINGLE;
        public string CardImageAssetPath = "";
        public bool Banned = false;
        public string CardFilePath = "";
        public List<string> Actions = new List<string>();

        public static ModCardInfo ReadFromFile(string file)
        {
            if (!File.Exists(file))
            {
                return null;
            }
            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            var value = new BinaryFormatter().Deserialize(fileStream) as ModCardInfo;
            fileStream.Close();
            return value;
        }
        public static ModCardInfo Clone(ModCardInfo file)
        {
            ModCardInfo tmp = new ModCardInfo
            {
                CardName = Guid.NewGuid().ToString(),
                CardType = file.CardType,
                CardDisplayName = file.CardDisplayName,
                CardEnergyCost = file.CardEnergyCost,
                CardRarity = file.CardRarity,
                CardCombatType = file.CardCombatType,
                CardTargetType = file.CardTargetType,
                CardImageAssetPath = file.CardImageAssetPath,
                Banned = file.Banned
            };
            tmp.Actions.Clear();
            tmp.Actions.AddRange(file.Actions);
            return tmp;
        }
        public void SaveToFile(string file)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(file, FileMode.Create);
            binaryFormatter.Serialize(fileStream, this);
            fileStream.Close();
        }
        public void Save()
        {
            string file = Path.Combine(ScriptTrainer.ModCardImgPath, $"{CardName}.card");
            if (CardFilePath != "")
                file = CardFilePath;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(file, FileMode.Create);
            binaryFormatter.Serialize(fileStream, this);
            fileStream.Close();
        }
        public static ModCardInfo ReadFromBytes(byte[] data)
        {
            var memStream = new MemoryStream(data);
            return (ModCardInfo)new BinaryFormatter().Deserialize(memStream);
        }
        public byte[] GetBytes()
        {
            BinaryFormatter serializer = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();
            serializer.Serialize(memStream, this);
            memStream.Close();
            return memStream.ToArray();
        }

        public bool Equals(ModCardInfo other)
        {
            return other.CardName == CardName;
        }
    }
}
