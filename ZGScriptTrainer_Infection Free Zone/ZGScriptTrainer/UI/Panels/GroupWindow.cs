using Commands;
using Controllers;
using Gameplay.Units.Enemy;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI;
using Gameplay.Units;
using Gameplay.GameResources;
using ZGScriptTrainer.ItemSpwan;
using UniverseLib.UI.Models;
using Controllers.CharacterLogic;
using ScritableObjectData.Face;
using Gameplay.Vehicles;
using MapOperator.Runtime.Utils.Extensions;

namespace ZGScriptTrainer.UI.Panels
{
    public class GroupWindow : ZGPanel
    {
        public static GroupWindow Instance { get; private set; }
        public override UIManager.Panels PanelType => UIManager.Panels.GroupWindow;
        public override string Name => UIManager.PanelNames[PanelType];
        public override int MinWidth => 350;
        public override int MinHeight => 200;
        public override Vector2 DefaultAnchorMin => new Vector2(0.35f, 0.175f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.8f, 0.925f);
        public bool Initialized { get; private set; } = false;
        public GameObject NavbarHolder;
        public Dropdown MouseInspectDropdown;
        public GameObject ContentHolder;
        public RectTransform ContentRect;

        public static float CurrentPanelWidth => Instance.Rect.rect.width;
        public static float CurrentPanelHeight => Instance.Rect.rect.height;

        private GroupBuilder GroupBuilder = null;
        private CitizensController CitizensController = null;
        private GroupsConfig GroupsConfig = null;
        private GameObject TabGroup = null;
        private GameObject ButtonGroup = null;
        private GameObject CharacterPanel = null;
        private Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();
        public Dictionary<string, ResourceID> Weapons = new Dictionary<string, ResourceID>();


        private GroupDraft draft;
        private Dropdown WeaponDropDown = null;
        private InputFieldRef GroupNameInput = null;
        private bool InitCharacter = false;
        private float CurHP = 100;
        public GroupWindow(UIBase owner) : base(owner)
        {
            Instance = this;
        }
        public override void Update()
        {
            base.Update();
            if (GroupBuilder == null)
            {
                if(GameConsoleCommandHandler.Instance != null)
                {
                    GroupBuilder = Traverse.Create(GameConsoleCommandHandler.Instance).Field("_groupBuilder").GetValue<GroupBuilder>();
                }
            }
            if (CitizensController == null)
            {
                if (GroupBuilder != null)
                {
                    CitizensController = Traverse.Create(GroupBuilder).Field("_citizensController").GetValue<CitizensController>();
                }
            }
            if (GroupsConfig == null)
            {
                if (GameConsoleCommandHandler.Instance != null)
                {
                    GroupsConfig = Traverse.Create(GameConsoleCommandHandler.Instance).Field("_groupsConfig").GetValue<GroupsConfig>();
                }
            }
            if (draft == null && GroupsConfig != null)
            {
                draft = GroupsConfig.GetDraftByName("player");
            }
            if (Weapons.Count == 0)
            {
                if (ZGItemUtil.BaseItems.Count != 0)
                {
                    foreach(var item in ZGItemUtil.BaseItems[2])
                    {
                        Weapons.Add(item.GetItemName(), item.ID);
                    }
                }
            }
            if (!Initialized)
            {
                if(Weapons.Count > 0 && GroupBuilder != null && GroupsConfig != null && CitizensController != null)
                {
                    AddGroupUI();
                    ButtonGroup.SetActive(true);
                    Initialized = true;
                }
            }
        }
        protected override void ConstructPanelContent()
        {
            UIFactory.CreateScrollView(ContentRoot, $"{PanelType}_ScrollView", out GameObject content, out var autoSlider);
            //Tab bar
            TabGroup = UIFactory.CreateVerticalGroup(content, $"{PanelType}_VerticalGroup_TabGroup", true, false, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f));
            UIFactory.SetLayoutElement(TabGroup, minHeight: 40, flexibleHeight: 0);

            

            ButtonGroup = UIFactory.CreateVerticalGroup(content, $"{PanelType}_VerticalGroup_ButtonGroup", true, true, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleCenter);
            UIFactory.SetLayoutElement(ButtonGroup, minHeight: 40, flexibleHeight: 0);

            ButtonGroup.CreateButton("添加队员", AddCharacterUI);
            ButtonGroup.SetLayoutElement(minWidth: 60, minHeight: 40);
            ButtonGroup.SetActive(false);
            // default active state: Active
            this.SetActive(true);
        }
        private void AddGroupUI()
        {
            //if(GroupBuilder == null)
            //    return;
            CharacterPanel = UIFactory.CreateVerticalGroup(TabGroup, $"{PanelType}_VerticalGroup_GroupUI", true, false, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f));
            UIFactory.SetLayoutElement(CharacterPanel, minHeight: 40, flexibleHeight: 0);

            var line1 = UIFactory.CreateHorizontalGroup(CharacterPanel, $"line1", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(line1, minHeight: 40, flexibleWidth: 9999);
            
            UIFactory.CreateLabel(line1, $"GroupNameLable", "小队名称：").SetLayoutElement(minHeight: 40);
            GroupNameInput = UIFactory.CreateInputField(line1, $"GroupNameLable", $"小队 {GetGroupCount()}");
            GroupNameInput.SetLayoutElement(minHeight: 40, minWidth: 80);

            UIFactory.CreateLabel(line1, $"GroupWeaponLable", "小队武器：").SetLayoutElement(minHeight: 40);
            UIFactory.CreateDropdown(line1, "GroupWeaponDropdown", out WeaponDropDown, Weapons.Keys.First(), ZGScriptTrainer.FontSize.Value, null, Weapons.Keys.ToArray()).SetLayoutElement(minHeight: 40, minWidth: 140);


            

            var line1_1 = UIFactory.CreateHorizontalGroup(line1, $"{PanelType}_HorizontalGroup1_1_GroupUI", false, false, true, true, 5, default,
               new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleRight);
            UIFactory.SetLayoutElement(line1_1, minHeight: 40, flexibleWidth: 9999);
            var CreateGroupButton = line1_1.CreateButton("添加小队到基地", CreateGroup);
            CreateGroupButton.SetLayoutElement(minHeight: 40, minWidth: 60);
        }
        private void AddCharacterUI()
        {
            if(CharacterPanel == null)
                return;
            InitCharacter = true;
            int index = Characters.Count;
            var split1 =  CharacterPanel.CreateSplitPanel(Color.white, 3);
            var NewCharacterPanel = UIFactory.CreateHorizontalGroup(CharacterPanel, $"CharacterPanel_{Characters.Count}", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(NewCharacterPanel, minHeight: 30, flexibleWidth: 9999);


            var info = CitizensController.GenerateCitizenInfo();
            var face = CitizensController.GetFace(info.FaceId);
            ZGScriptTrainer.WriteLog(face.Age);

            var ItemUI = UIFactory.CreateUIObject("ItemUI", NewCharacterPanel, new Vector2(60, 60));
            var ItemImage = ItemUI.AddComponent<Image>();
            ItemImage.preserveAspect = true;
            ItemImage.sprite = face.Face;
            UIFactory.SetLayoutElement(ItemUI, 60, 60, 60, 60, 60, 60);

            var FaceId = UIFactory.CreateLabel(NewCharacterPanel, "FaceId", info.FaceId);
            FaceId.gameObject.SetActive(false);
            
            var InfoPanel = UIFactory.CreateVerticalGroup(NewCharacterPanel, $"InfoPanel", true, false, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f));
            UIFactory.SetLayoutElement(InfoPanel, minHeight: 30, flexibleHeight: 0);


            Dropdown AgeDropDown = null;

            var line1 = UIFactory.CreateHorizontalGroup(InfoPanel, $"Line1", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(line1, minHeight: 30, flexibleWidth: 9999);

            var line2 = UIFactory.CreateHorizontalGroup(InfoPanel, $"Line2", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(line2, minHeight: 30, flexibleWidth: 9999);

            UIFactory.CreateLabel(line1, $"CharaterNameLable", "名称：").SetLayoutElement(minHeight: 30);
            var CharaterNameInput = UIFactory.CreateInputField(line1, $"CharaterNameInput", info.Name);
            CharaterNameInput.SetLayoutElement(minHeight: 30, minWidth: 80);

            var GenderToggle = line1.CreateToggle("男性", (bool state) =>
            {
                if(AgeDropDown != null && AgeDropDown?.value >= 0)
                {
                    var newface = FacesData.Instance.GetRandomFace((Age)(AgeDropDown.value + 1), state);
                    ItemImage.sprite = newface.Face;
                    FaceId.text = newface.Id;
                }
            }, "CharaterGender", isOn: (info.Gender == Plugins.CharacterNameGenerator.GenderType.Male)).SetLayoutElement(minHeight: 30, minWidth: 80);

            UIFactory.CreateLabel(line1, $"CharaterNameLable", "年龄：").SetLayoutElement(minHeight: 30);

            UIFactory.CreateDropdown(line1, $"CharaterAge", out AgeDropDown, "青年", ZGScriptTrainer.FontSize.Value, (int index) =>
            {
                if(index >= 0 && !InitCharacter)
                {
                    var newface = FacesData.Instance.GetRandomFace((Age)(index + 1), GenderToggle.GetComponent<Toggle>().isOn);
                    ItemImage.sprite = newface.Face;
                    FaceId.text = newface.Id;
                }
            }, new string[2] {"青年", "老年" }).SetLayoutElement(minHeight: 30, minWidth: 70);
            AgeDropDown.value = (int)face.Age - 1;
            AgeDropDown.RefreshShownValue();

            line1.CreateButton("刷新面貌", () => 
            {
                if (AgeDropDown != null && AgeDropDown?.value >= 0)
                {
                    var newface = FacesData.Instance.GetRandomFace((Age)(AgeDropDown.value + 1), GenderToggle.GetComponent<Toggle>().isOn);
                    ItemImage.sprite = newface.Face;
                    FaceId.text = newface.Id;
                }
            }).SetLayoutElement(minHeight: 30, minWidth: 40);

            UIFactory.CreateLabel(line2, $"CharaterHPLable", "最大血量：").SetLayoutElement(minHeight: 30);
            var CharaterHPInput = UIFactory.CreateInputField(line2, $"CharaterHPInput", CurHP.ToString());
            CharaterHPInput.SetLayoutElement(minHeight: 30, minWidth: 80);
            CharaterHPInput.OnValueChanged += (string value) => { CurHP = value.ConvertToFloatDef(100); };

            var split2 = CharacterPanel.CreateSplitPanel(Color.white, 3);

            var DelPanel = UIFactory.CreateHorizontalGroup(NewCharacterPanel, $"DelPanel", false, false, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleRight);
            UIFactory.SetLayoutElement(DelPanel, minHeight: 30, flexibleWidth : 40);
            DelPanel.CreateButton("删除", () =>
            {
                if(Characters.ContainsKey(index))
                    Characters.Remove(index);
                GameObject.Destroy(split1);
                GameObject.Destroy(split2);
                GameObject.Destroy(NewCharacterPanel);
            }).SetLayoutElement(40, 30);
            InitCharacter = false;
            Characters.Add(index, NewCharacterPanel);
        }
        private void CreateGroup()
        {
            if(Characters.Count > 0 && BuildingsController.MainHeadquarter != null)
            {
                if(!Weapons.TryGetValue(WeaponDropDown.captionText.text, out var resource))
                {
                    resource = ResourceID.None;
                }
                var group = GroupBuilder.SpawnGroupAt(BuildingsController.MainHeadquarter.GetEntrances().GetRandom<Vector3>(), draft, Characters.Count, resource, 0f, BuildingsController.MainHeadquarter, VehicleType.None);
                group.SetGroupName(GroupNameInput.Text);

                foreach (var ch in Characters)
                {
                    group.Characters[ch.Key].CharacterInfo.FaceId = ch.Value.transform.Find("FaceId").GetComponent<Text>().text;
                    string name = ch.Value.transform.Find("InfoPanel/Line1/CharaterNameInput").GetComponent<InputField>().text;
                    if (String.IsNullOrEmpty(name))
                    {
                        name = ch.Value.transform.Find("InfoPanel/Line1/CharaterNameInput/TextArea/Placeholder").GetComponent<Text>().text;
                    }
                    group.Characters[ch.Key].CharacterInfo.Name = name;
                    group.Characters[ch.Key].CharacterInfo.Gender = ch.Value.transform.Find("InfoPanel/Line1/CharaterGender").GetComponent<Toggle>().isOn ? Plugins.CharacterNameGenerator.GenderType.Male : Plugins.CharacterNameGenerator.GenderType.Female;


                    string hp = ch.Value.transform.Find("InfoPanel/Line2/CharaterHPInput").GetComponent<InputField>().text;
                    if (String.IsNullOrEmpty(hp))
                    {
                        hp = ch.Value.transform.Find("InfoPanel/Line2/CharaterHPInput/TextArea/Placeholder").GetComponent<Text>().text;
                    }
                    group.Characters[ch.Key].Health.SetMaxHp(hp.ConvertToFloatDef(100));
                    group.Characters[ch.Key].Health.AddHp01(1f);
                }
                group.Show();
            }
        }
        private int GetGroupCount()
        {
            if(GroupBuilder != null)
            {
                return Traverse.Create(GroupBuilder).Field("_createdSquadsCount").GetValue<int>();
            }
            return 0;
        }
    }
}
