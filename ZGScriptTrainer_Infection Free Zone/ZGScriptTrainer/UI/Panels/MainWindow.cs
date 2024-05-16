using System;
using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI;
using ZGScriptTrainer.UI.Models;
using Commands;
using Controllers;
using HarmonyLib;
using Gameplay.Research;
using GameInput.Debug;
using GameInput;
using Gameplay.Buildings;
using Gameplay.Units.Enemy;
using Gameplay.Units.Player.Workers;
using Gameplay.Units;
using MapEssentials;

namespace ZGScriptTrainer.UI.Panels
{
    public class MainWindow : ZGPanel
    {
        public static MainWindow Instance { get; private set; }
        public override UIManager.Panels PanelType => UIManager.Panels.MainWindow;
        public override string Name => UIManager.PanelNames[PanelType];
        public override int MinWidth => 350;
        public override int MinHeight => 200;
        public override Vector2 DefaultAnchorMin => new Vector2(0.35f, 0.175f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.8f, 0.925f);

        public GameObject NavbarHolder;
        public Dropdown MouseInspectDropdown;
        public GameObject ContentHolder;
        public RectTransform ContentRect;
        public static float CurrentPanelWidth => Instance.Rect.rect.width;
        public static float CurrentPanelHeight => Instance.Rect.rect.height;



        public WorkersController WorkersController = null;
        public SquadsController SquadsController = null;

        public MainWindow(UIBase owner) : base(owner)
        {
            Instance = this;
        }
        public override void Update()
        {
            base.Update();
            if (GameConsoleCommandHandler.Instance != null)
            {
                WorkersController ??= Traverse.Create(GameConsoleCommandHandler.Instance).Field("_workersController").GetValue<WorkersController>();

                SquadsController ??= Traverse.Create(GameConsoleCommandHandler.Instance).Field("_squadsController").GetValue<SquadsController>();
            }
        }
        public void ChangeMaxSpeed(string radio, bool state)
        {
            ScriptPatch.CanMaxSpeedMult = state;
            ScriptPatch.MaxSpeedMult = radio.ConvertToFloatDef(2);
            if (WorkersController != null && SquadsController != null)
            {
                float mult = ScriptPatch.MaxSpeedMult;
                if (!state)
                    mult = 1;
                foreach (var worker in WorkersController.Workers)
                {
                    float speed = Map.ConvertKphToUps(worker.Stats.Speed);
                    worker.Movement.MaxSpeed = mult * speed;
                }
                foreach (var group in SquadsController.Squads)
                {
                    foreach(var squad in group.Characters)
                    {
                        float speed = Map.ConvertKphToUps(squad.Stats.Speed);
                        squad.Movement.MaxSpeed = mult * speed;
                    }
                }
            }
        }
        protected override void ConstructPanelContent()
        {
            // Tab bar
            GameObject tabGroup = UIFactory.CreateVerticalGroup(ContentRoot, "MainWindowVerticalGroup", true, false, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f));
            UIFactory.SetLayoutElement(tabGroup, minHeight: 40, flexibleHeight: 0);

            Text title = UIFactory.CreateLabel(tabGroup, "Title", "总部修改：", TextAnchor.MiddleLeft, default, true, ZGScriptTrainer.FontSize.Value);
            UIFactory.SetLayoutElement(title.gameObject, minWidth: 75, flexibleWidth: 0);
            tabGroup.CreateSplitPanel(Color.white);

            GameObject Horizontal_0 = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowHorizontal_0", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Horizontal_0, minHeight: 40, flexibleWidth: 9999);


            Horizontal_0.CreateInputEditButton("添加人口：", "10", "添加", new Action<string>((string text) =>
            {
                int value = text.ConvertToIntDef(10);
                GameConsoleCommandHandler.Instance?.AddWorkersToHq(value);
            }));

            Horizontal_0.CreateInputEditButton("添加科学资料：", "10", "添加", new Action<string>((string text) =>
            {
                float value = text.ConvertToFloatDef(10);
                if (BuildingsController.MainHeadquarter != null && GameConsoleCommandHandler.Instance != null)
                {
                    ResearchController _researchController = Traverse.Create(GameConsoleCommandHandler.Instance).Field("_researchController").GetValue<ResearchController>();

                    _researchController?.AddScientificMaterials(value);
                }
            }));

            Horizontal_0.CreateInputEditButton("增加研究进度：", "10", "添加", new Action<string>((string text) =>
            {
                float value = text.ConvertToFloatDef(10);
                if (BuildingsController.MainHeadquarter != null && GameConsoleCommandHandler.Instance != null)
                {
                    ResearchController _researchController = Traverse.Create(GameConsoleCommandHandler.Instance).Field("_researchController").GetValue<ResearchController>();

                    _researchController?.AddTechnologyResearchProgress(value);

                    

                }
            }));

            GameObject Horizontal_1 = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowHorizontal_1", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Horizontal_1, minHeight: 40, flexibleWidth: 9999);

            Horizontal_1.CreateInputEditButton("增加情绪：", "10", "添加", new Action<string>((string text) =>
            {
                float value = text.ConvertToFloatDef(10);
                if (BuildingsController.MainHeadquarter != null && GameConsoleCommandHandler.Instance != null)
                {
                    ScriptPatch.ZG_Mood = value;
                    GameConsoleCommandHandler.Instance?.AddMoodModifier($"ZG_MoodMod");
                }
            }));

            tabGroup.CreateSplitPanel(Color.white);
            UIFactory.CreateLabel(tabGroup, "Title", "倍率修改：", TextAnchor.MiddleLeft, default, true, ZGScriptTrainer.FontSize.Value).SetLayoutElement(minWidth: 75, flexibleWidth: 0);
            tabGroup.CreateSplitPanel(Color.white);

            GameObject Horizontal_3 = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowHorizontal_3", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Horizontal_3, minHeight: 40, flexibleWidth: 9999);

            Horizontal_3.CreateInputEditToggle("搜索倍率:", "2", (string text, bool state) => 
            {
                ScriptPatch.ScavengeMult = text.ConvertToFloatDef(2);
                ScriptPatch.CanScavengeMult = state;
            });
            Horizontal_3.CreateInputEditToggle("经验倍率:", "2", (string text, bool state) =>
            {
                ScriptPatch.ExperienceMult = text.ConvertToFloatDef(2);
                ScriptPatch.CanExperienceMult = state;
            });

            Horizontal_3.CreateInputEditToggle("移动速度倍率:", "2", ChangeMaxSpeed);

            Horizontal_3.CreateToggle("工人无休", (bool state) => { ScriptPatch.NoSleep = state; });


            tabGroup.CreateSplitPanel(Color.white);
            UIFactory.CreateLabel(tabGroup, "Title", "其他修改：", TextAnchor.MiddleLeft, default, true, ZGScriptTrainer.FontSize.Value).SetLayoutElement(minWidth: 75, flexibleWidth: 0);
            tabGroup.CreateSplitPanel(Color.white);

            GameObject Horizontal_2 = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowHorizontal_2", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Horizontal_2, minHeight: 40, flexibleWidth: 9999);

            Horizontal_2.CreateButton("修复建筑", new Action(() =>
            {
                if (BuildingsController.MainHeadquarter != null && GameConsoleCommandHandler.Instance != null)
                {
                    UIManager.WorldToolTip.GetComponent<TooltipGUI>().SetText("鼠标左键选择建筑[鼠标右键退出]");
                    UIManager.WorldToolTip.GetComponent<TooltipGUI>().EnableTooltip = true;
                    CursorsManager.SelectCursor(new InvokeActionOnClickable<Building>(delegate (Building building)
                    {
                        building.Health.SetHealthAmount01(1f);
                    }), true);
                }
            })).SetLayoutElement(minHeight: 25, minWidth: 80);

            Horizontal_2.CreateButton("小队血量回复", new Action(() =>
            {
                if (BuildingsController.MainHeadquarter != null && GameConsoleCommandHandler.Instance != null)
                {
                    GroupsController _groupsController = Traverse.Create(GameConsoleCommandHandler.Instance).Field("_groupsController").GetValue<GroupsController>();
                    foreach(var group in _groupsController.Groups)
                    {
                        foreach(var character in group.Characters)
                        {
                            character.Health.SetHp(character.Health.MaxHp);
                        }
                    }

                }
            })).SetLayoutElement(minHeight: 25, minWidth: 80);

            Horizontal_2.CreateButton("瞬间完成建筑物", new Action(() =>
            {
                if (BuildingsController.MainHeadquarter != null && GameConsoleCommandHandler.Instance != null)
                {
                    UIManager.WorldToolTip.GetComponent<TooltipGUI>().SetText("鼠标左键选择建筑[鼠标右键退出]");
                    UIManager.WorldToolTip.GetComponent<TooltipGUI>().EnableTooltip = true;
                    CursorsManager.SelectCursor(new InvokeActionOnClickable<Building>(delegate (Building building)
                    {
                        building.BuildImmediately();
                        building.BuildWork?.BuildImmediately();
                        building.BuildStructureWork?.BuildImmediately();
                    }), true);
                }
            })).SetLayoutElement(minHeight: 25, minWidth: 80);


            // default active state: Active
            this.SetActive(true);
        }
    }
}
