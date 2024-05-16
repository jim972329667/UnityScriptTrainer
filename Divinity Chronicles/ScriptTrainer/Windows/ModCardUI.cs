using HarmonyLib;
using JTW;
using ScriptTrainer.Cards;
using SimpleFileBrowser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace ScriptTrainer.Windows
{
    public class ModCardUI : MonoBehaviour
    {
        public static ModCardUI instance;
        public GameObject BackGround
        {
            get
            {
                return gameObject.transform.Find("BackGround").gameObject;
            }
        }
        public GameObject Panel
        {
            get
            {
                return gameObject.transform.Find("Panel").gameObject;
            }
        }
        public GameObject Content
        {
            get
            {
                return Panel.transform.Find("ModCardList/Viewport/Content").gameObject;
            }
        }
        public GameObject ModCardCell
        {
            get
            {
                return Content.transform.Find("ModCardCell").gameObject;
            }
        }



        public List<GameObject> CardCells = new List<GameObject>();
        public List<ModCardInfo> CardInfos = new List<ModCardInfo>();
        public ModCardInfo CardInfo = new ModCardInfo();

        public Card Card = new ModWukongCard();
        public GameObject CardTool;

        public CreateCardUI CreateCardUI;
        public void Awake()
        {
            instance = this;
            Panel.transform.Find("CloseButton").gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                GameObject.Destroy(gameObject);
            });
            
            CreateCardUI = BackGround.AddComponent<CreateCardUI>();
            BackGround.SetActive(false);
            Panel.transform.Find("ButtonContainer").gameObject.SetActive(false);
            
            var CardDisplay = Panel.transform.Find("CardDisplay");
            CardTool = Instantiate(Resources.Load<GameObject>("CombatObjects/CardUINew"));
            CardTool.transform.SetParent(CardDisplay, false);
            CardTool.transform.localPosition = Vector3.zero;
            CardTool.GetComponent<CardComponentNew>().Card = Card;
            CardTool.GetComponent<RectTransform>().localScale = new Vector3(3, 3, 3);

            ModCardCell.SetActive(false);
            GetCardInfos();
            foreach (var card in CardInfos)
            {
                AddModCardCell(card);
            }


            Panel.transform.Find("CreateCardButton").gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                CreateCardUI.CardInfo = new ModCardInfo();
                BackGround.SetActive(true);
                Panel.SetActive(false);
            });
            Panel.transform.Find("EditCardButton").gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                CreateCardUI.CardInfo = CardInfo;
                BackGround.SetActive(true);
                Panel.SetActive(false);
            });
            Panel.transform.Find("CopyCardButton").gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                var tmp = ModCardInfo.Clone(CardInfo);
                tmp.SaveToFile(Path.Combine(ScriptTrainer.ModCardImgPath, $"{tmp.CardName}.card"));
                DynamicCardCreator.AppendCardToGame(tmp);
                CardInfos.Add(tmp);
                AddModCardCell(tmp);
            });
            Panel.transform.Find("DelCardButton").gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                int i = CardInfos.IndexOf(CardInfo);
                if(i != -1)
                {
                    GameObject.Destroy(CardCells[i]);
                    DynamicCardCreator.RemoveModCard(CardInfo);
                    CardCells.RemoveAt(i);
                    CardInfos.RemoveAt(i);
                    if(CardInfos.Count > 0)
                    {
                        CardInfo = CardInfos.First();
                    }
                    else
                    {
                        CardInfo = new ModCardInfo();
                    }
                    ChangeCard();
                }
            });
            Panel.transform.Find("OpenCardPath").gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = ScriptTrainer.ModCardImgPath,
                    UseShellExecute = true,
                    Verb = "open"
                });
            });
            gameObject.SetActive(true);
        }

        public void ChangeCard()
        {
            Card = DynamicCardCreator.CreateDynamicCard(CardInfo);
            CardTool.GetComponent<CardComponentNew>().Card = Card;
        }
        public void AddModCardCell(ModCardInfo cardInfo)
        {
            var Modle = GameObject.Instantiate(ModCardCell);
            Modle.GetComponent<Button>().onClick.AddListener(() =>
            {
                CardInfo = cardInfo;
                ChangeCard();
            });
            var Toggle = Modle.transform.Find("Toggle");
            Toggle.GetComponent<Toggle>().isOn = !cardInfo.Banned;
            Toggle.GetComponent<Toggle>().onValueChanged.AddListener((bool state) => { 
                cardInfo.Banned = !state; 
                cardInfo.Save();
                if (!state)
                {
                    if (!DynamicCardCreator.BannedList.Contains(cardInfo.CardName))
                    {
                        DynamicCardCreator.BannedList.Add(cardInfo.CardName);
                    }
                }
                else
                {
                    if (DynamicCardCreator.BannedList.Contains(cardInfo.CardName))
                    {
                        DynamicCardCreator.BannedList.Remove(cardInfo.CardName);
                    }
                }
            });

            Modle.transform.Find("GameObject/DisplayName").GetComponent<Text>().text = cardInfo.CardDisplayName;
            Modle.transform.Find("GameObject/FileName").GetComponent<Text>().text = cardInfo.CardFilePath != "" ? new FileInfo(cardInfo.CardFilePath).Name : cardInfo.CardName + ".card";
            Modle.transform.SetParent(Content.transform, false);
            Modle.SetActive(true);
            CardCells.Add(Modle);
        }
        public void GetCardInfos(int i = 0)
        {
            CardInfos.Clear();
            if (i == 0)
            {
                CardInfos.AddRange(DynamicCardCreator.ModCardInfos);
            }
        }
        public void UpdateCardInfo(ModCardInfo cardInfo)
        {
            int i = CardInfos.IndexOf(CardInfo);
            if(i == -1)
            {
                CardInfos.Add(cardInfo);
                AddModCardCell(cardInfo);
            }
            else
            {
                var cell = CardCells[i];
                cell.transform.Find("GameObject/DisplayName").GetComponent<Text>().text = cardInfo.CardDisplayName;
                cell.transform.Find("GameObject/FileName").GetComponent<Text>().text = cardInfo.CardFilePath != "" ? new FileInfo(cardInfo.CardFilePath).Name : cardInfo.CardName + ".card";
            }
        }
    }

    public class CreateCardUI : MonoBehaviour
    {
        #region 属性
        private ModCardInfo cardInfo = new ModCardInfo();
        public ModCardInfo CardInfo 
        { 
            get
            {
                return cardInfo;
            }
            set
            {
                cardInfo = value;
                UpdateDisplay();
            }
        }
        public GameObject Content
        {
            get
            {
                return transform.Find("Scroll View/Viewport/Content").gameObject;
            }
        }
        public GameObject ModelCell
        {
            get
            {
                return Content?.transform.Find("ModelCell").gameObject;
            }
        }
        public InputField CardDisplayName
        {
            get
            {
                return transform.Find("CardDisplayNameInputField").GetComponent<InputField>();
            }
        }
        public InputField CardEnergyCost
        {
            get
            {
                return transform.Find("CardEnergyCostInputField").GetComponent<InputField>();
            }
        }
        public Dropdown CardType
        {
            get
            {
                return transform.Find("CardTypeDropdown").GetComponent<Dropdown>();
            }
        }
        public Dropdown CardCombatType
        {
            get
            {
                return transform.Find("CardCombatTypeDropdown").GetComponent<Dropdown>();
            }
        }
        public Dropdown CardTargetType
        {
            get
            {
                return transform.Find("CardTargetTypeDropdown").GetComponent<Dropdown>();
            }
        }
        public Dropdown CardRarity
        {
            get
            {
                return transform.Find("CardRarityDropdown").GetComponent<Dropdown>();
            }
        }
        public Dropdown CardImage
        {
            get
            {
                return transform.Find("CardImgDropdown").GetComponent<Dropdown>();
            }
        }

        public List<string> CardImgs = new List<string>();
        public Card Card = new ModWukongCard();
        public GameObject CardTool;
        public List<GameObject> CardMods = new List<GameObject>();
        public Dictionary<string, List<string>> CardModsActions = new Dictionary<string, List<string>>();
        public int CardModsCount = 0;
        public bool IsUpdateDisplay = false;
        #endregion
        public void Awake()
        {
            base.transform.Find("CloseButton").gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                base.gameObject.SetActive(false);
                ModCardUI.instance?.Panel.SetActive(true);
            });
            CardDisplayName.onValueChanged.AddListener((string text) =>
            {
                if (IsUpdateDisplay) return;
                CardInfo.CardDisplayName = text;
                ChangeCard();
            });
            CardType.onValueChanged.AddListener((int index) =>
            {
                if (IsUpdateDisplay) return;
                if(index > 0 && index < DynamicCardCreator.CardTypes.Count)
                {
                    CardInfo.CardType = DynamicCardCreator.CardTypes[index];
                }
                ChangeCard();
            });
            CardEnergyCost.onValueChanged.AddListener((string text) =>
            {
                if (IsUpdateDisplay) return;
                int.TryParse(text, out int result);
                CardInfo.CardEnergyCost = result;
                ChangeCard();
            });
            CardCombatType.onValueChanged.AddListener((int index) =>
            {
                if (IsUpdateDisplay) return;
                switch (index)
                {
                    default:
                    case 0:
                        CardInfo.CardCombatType = CombatAction.CombatActionType.ATTACK;
                        break;
                    case 1:
                        CardInfo.CardCombatType = CombatAction.CombatActionType.SKILL;
                        break;
                    case 2:
                        CardInfo.CardCombatType = CombatAction.CombatActionType.POWER;
                        break;
                }
                ChangeCard();
            });
            CardTargetType.onValueChanged.AddListener((int index) =>
            {
                if (IsUpdateDisplay) return;
                CardInfo.CardTargetType = GetTargetType(index);
                ChangeCard();
            });
            CardRarity.onValueChanged.AddListener((int index) =>
            {
                if (IsUpdateDisplay) return;
                switch (index)
                {
                    default:
                    case 0:
                        CardInfo.CardRarity = Rarity.COMMON;
                        break;
                    case 1:
                        CardInfo.CardRarity = Rarity.UNCOMMON;
                        break;
                    case 2:
                        CardInfo.CardRarity = Rarity.RARE;
                        break;
                }
                ChangeCard();
            });

            var CardAction = ModelCell.transform.Find("CardActionDropdown").GetComponent<Dropdown>();
            CardAction.ClearOptions();
            foreach (var name in DynamicCardCreator.ActionNames)
            {
                CardAction.options.Add(new Dropdown.OptionData { text = name.Value });
            }
            ModelCell.SetActive(false);
            var CardDisplay = transform.Find("CardDisplay");
            CardTool = Instantiate(Resources.Load<GameObject>("CombatObjects/CardUINew"));
            CardTool.transform.SetParent(CardDisplay, false);
            CardTool.transform.localPosition = Vector3.zero;
            CardTool.GetComponent<CardComponentNew>().Card = Card;
            CardTool.GetComponent<RectTransform>().localScale = new Vector3(3, 3, 3);

            LoadImages();
            if (CardImgs.Count > 0)
                UpdateCardImageDropDown();

            CardImage.onValueChanged.AddListener((int index) =>
            {
                if (IsUpdateDisplay) return;
                if (index < 0) return;
                if (index == 0)
                {
                    CardInfo.CardImageAssetPath = "";
                    Debug.Log($"CardInfo.CardImageAssetPath : {CardInfo.CardImageAssetPath}");
                    ChangeCard();
                    return;
                }
                if (CardImgs.Count > 0)
                {
                    CardInfo.CardImageAssetPath = "@external_3227332234:Data/Cards/" + CardImgs[index - 1];
                    ChangeCard();
                }
            });
            transform.Find("OpenCardImg").gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                // 设置过滤器
                FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png"));

                // 设置默认过滤器
                FileBrowser.SetDefaultFilter(".jpg");

                FileBrowser.ShowLoadDialog((string[] paths) =>
                {
                    Debug.Log(paths[0]);
                    FileInfo fileInfo = new FileInfo(paths[0]);
                    string target = fileInfo.Name.Replace(fileInfo.Extension, $"_{DateTime.UtcNow.Ticks}{fileInfo.Extension}");
                    var targetFilePath = Path.Combine(ScriptTrainer.ModCardImgPath, target);
                    File.Copy(paths[0], targetFilePath);
                    CardImgs.Add(target);
                    UpdateCardImageDropDown();
                    CardImage.value = CardImage.options.Count - 1;
                }, () => Debug.Log("Save dialog cancelled"), false, false, null, "选择卡牌图片", "选择"
                );

            });


            transform.Find("SaveCardButton").gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                if(CardInfo.CardName == "")
                    CardInfo.CardName = Guid.NewGuid().ToString();
                CardInfo.Save();
                DynamicCardCreator.AppendCardToGame(CardInfo);
                EngineWrapper.Get().ShowAlertMessage("保存卡牌成功,已成功载入到游戏！");
                ModCardUI.instance?.UpdateCardInfo(CardInfo);
            });
            transform.Find("AddModleButton").gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                AddModle(TargetType: CardTargetType.value);
                ChangeCardActions();
            });
        }
        public void UpdateDisplay()
        {
            IsUpdateDisplay = true;
            CardDisplayName.text = CardInfo.CardDisplayName;
            CardType.value = DynamicCardCreator.CardTypes.IndexOf(CardInfo.CardType);
            CardEnergyCost.text = CardInfo.CardEnergyCost.ToString();
            switch (CardInfo.CardCombatType)
            {
                default:
                case CombatAction.CombatActionType.ATTACK:
                    CardCombatType.value = 0;
                    break;
                case CombatAction.CombatActionType.POWER:
                    CardCombatType.value = 2;
                    break;
                case CombatAction.CombatActionType.SKILL:
                    CardCombatType.value = 1;
                    break;
            }
            CardTargetType.value = (int)CardInfo.CardTargetType - 1;
            CardRarity.value = (int)CardInfo.CardRarity - 2;
            string img = CardInfo.CardImageAssetPath.Replace("@external_3227332234:Data/Cards/", "");
            int i = CardImgs.IndexOf(img);
            CardImage.value = i >= 0 ? i : 0;
            RemoveModles();
            foreach (var x in CardInfo.Actions)
            {
                var lines = x.Split(';');
                if (Enum.TryParse(lines[2], out CombatAction.ActionTargetType myStatus))
                {
                    if(lines.Length > 3)
                        AddModle(lines[0], lines[1], (int)myStatus - 1, lines[3]);
                    else
                        AddModle(lines[0], lines[1], (int)myStatus - 1);
                }
            }
            ChangeCard();
            IsUpdateDisplay = false;
        }
        public void ChangeCard()
        {
            Card = DynamicCardCreator.CreateDynamicCard(CardInfo);
            CardTool.GetComponent<CardComponentNew>().Card = Card;
        }
        public CombatAction.ActionTargetType GetTargetType(int index)
        {
            if (index < 0 || index > 11)
                return CombatAction.ActionTargetType.ENEMY_SINGLE;
            return (CombatAction.ActionTargetType)Enum.ToObject(typeof(CombatAction.ActionTargetType), index + 1);
        }
        public void UpdateCardImageDropDown()
        {
            CardImage.ClearOptions();
            CardImage.options.Add(new Dropdown.OptionData { text = "空" });
            foreach (var x in CardImgs)
            {
                CardImage.options.Add(new Dropdown.OptionData { text = x });
            }
        }
        public void LoadImages()
        {
            CardImgs.Clear();
            foreach (var x in Directory.GetFiles(ScriptTrainer.ModCardImgPath, "*.jpg"))
            {
                FileInfo fileInfo = new FileInfo(x);
                CardImgs.Add(fileInfo.Name);
            }
            foreach (var x in Directory.GetFiles(ScriptTrainer.ModCardImgPath, "*.png"))
            {
                FileInfo fileInfo = new FileInfo(x);
                CardImgs.Add(fileInfo.Name);
            }
        }
        public void ChangeCardActions()
        {
            CardInfo.Actions.Clear();
            foreach (var x in CardModsActions.Values)
            {
                CardInfo.Actions.Add(string.Join(";", x));
            }
            ChangeCard();
        }
        public void RemoveModles()
        {
            foreach (var item in CardMods)
            {
                UnityEngine.Object.Destroy(item);
            }
            CardMods.Clear();
            CardModsActions.Clear();
        }
        public void AddModle(string Action = "Attack", string Value = "1", int TargetType = 0, string Upgrade = "")
        {
            var Modle = GameObject.Instantiate(ModelCell);
            Modle.name = $"ModleCell_{CardModsCount++}";
            Modle.transform.Find("CardActionDropdown").GetComponent<Dropdown>().value = DynamicCardCreator.ActionNames.Keys.ToList().IndexOf(Action);
            Modle.transform.Find("CardActionDropdown").GetComponent<Dropdown>().onValueChanged.AddListener((int i) =>
            {
                if (i < 0)
                    return;
                string key = DynamicCardCreator.ActionNames.Keys.ToList()[i];
                CardModsActions[Modle.name][0] = key;
                ChangeCardActions();
            });

            Modle.transform.Find("CardTargetTypeDropdown").GetComponent<Dropdown>().value = TargetType;
            Modle.transform.Find("CardTargetTypeDropdown").GetComponent<Dropdown>().onValueChanged.AddListener((int i) =>
            {
                if (i < 0)
                    return;
                int index = CardMods.IndexOf(Modle);
                CardModsActions[Modle.name][2] = GetTargetType(i).ToString();
                ChangeCardActions();
            });

            if(Upgrade != "")
            {
                Modle.transform.Find("ModUpgradeInputField").GetComponent<InputField>().text = Upgrade;
            }
            Modle.transform.Find("ModUpgradeInputField").GetComponent<InputField>().onValueChanged.AddListener((string text) =>
            {
                int index = CardMods.IndexOf(Modle);
                if (CardModsActions[Modle.name].Count < 4)
                    CardModsActions[Modle.name].Add(text);
                else
                    CardModsActions[Modle.name][3] = text;
                ChangeCardActions();
            });
            Modle.transform.Find("ModValueInputField").GetComponent<InputField>().text = Value;
            Modle.transform.Find("ModValueInputField").GetComponent<InputField>().onValueChanged.AddListener((string text) =>
            {
                int index = CardMods.IndexOf(Modle);
                CardModsActions[Modle.name][1] = text;
                ChangeCardActions();
            });

            Modle.transform.Find("DelButton").gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                int index = CardMods.IndexOf(Modle);
                CardMods.RemoveAt(index);
                CardModsActions.Remove(Modle.name);
                GameObject.Destroy(Modle);
                ChangeCardActions();
            });
            Modle.transform.SetParent(Content.transform, false);
            Modle.SetActive(true);
            if (Upgrade != "")
                CardModsActions.Add(Modle.name, new List<string>() { Action, Value, GetTargetType(TargetType).ToString(), Upgrade });
            else
                CardModsActions.Add(Modle.name, new List<string>() { Action, Value, GetTargetType(TargetType).ToString() });
            CardMods.Add(Modle);
        }
    }
}
