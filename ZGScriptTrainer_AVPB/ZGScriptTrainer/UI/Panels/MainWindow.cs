using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI;
using UnityEngine.Events;
using ZGScriptTrainer.UI.Models;
using System.Reflection;

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

        public MainWindow(UIBase owner) : base(owner)
        {
            Instance = this;
        }
        protected override void ConstructPanelContent()
        {
            // Tab bar
            GameObject tabGroup = UIFactory.CreateVerticalGroup(ContentRoot, "MainWindowVerticalGroup", true, false, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f));
            UIFactory.SetLayoutElement(tabGroup, minHeight: 40, flexibleHeight: 0);

            var label_1 = UIFactory.CreateLabel(tabGroup, "Title1", "人物修改：");
            label_1.SetLayoutElement(minHeight: 25, minWidth: 80);

            GameObject Horizontal_0 = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowHorizontal_0", false, false, true, true, 5, new Vector4(5, 5, 5, 5),
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Horizontal_0, minHeight: 40, flexibleWidth: 9999);

            var InB0 = ZGUIUtility.CreateInputButton(Horizontal_0, "InB0", "5", "增加额外手部格子");
            InB0.InputField.SetLayoutElement(minWidth: 80, minHeight: 40, flexibleWidth: 0);
            InB0.Button.SetLayoutElement(minHeight: 25, minWidth: 80);
            InB0.OnClick = new Action<string>((string target) =>
            {
                int.TryParse(target, out int value);
                try
                {
                    var farmer = HudManager.Instance.m_InventoryBar.m_Farmer;
                    var farmerCarry = farmer.m_FarmerCarry;
                    farmerCarry.SetExtraCarry(farmerCarry.m_ExtraCapacity + value);
                }
                catch { }
            });

            var InB1 = ZGUIUtility.CreateInputButton(Horizontal_0, "InB1", "5", "增加额外背包格子");
            InB1.InputField.SetLayoutElement(minWidth: 80, minHeight: 40, flexibleWidth: 0);
            InB1.Button.SetLayoutElement(minHeight: 25, minWidth: 80);
            InB1.OnClick = new Action<string>((string target) =>
            {
                int.TryParse(target, out int value);
                try
                {
                    var farmer = HudManager.Instance.m_InventoryBar.m_Farmer;
                    var farmerInventory = farmer.m_FarmerInventory;
                    farmerInventory.SetExtraCapacity(farmerInventory.m_ExtraCapacity + value);
                }
                catch { }
            });

            var InB2 = ZGUIUtility.CreateInputButton(Horizontal_0, "InB2", "5", "增加额外升级格子");
            InB2.InputField.SetLayoutElement(minWidth: 80, minHeight: 40, flexibleWidth: 0);
            InB2.Button.SetLayoutElement(minHeight: 25, minWidth: 80);
            InB2.OnClick = new Action<string>((string target) =>
            {
                int.TryParse(target, out int value);
                try
                {
                    var bar = HudManager.Instance.m_InventoryBar;
                    var farmerUpgrades = bar.m_Farmer.m_FarmerUpgrades;
                    farmerUpgrades.SetCapacity(farmerUpgrades.m_Capacity + value);
                    bar.GetType().GetMethod("SetupSlots", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(bar, null);

                }
                catch { }
            });

            //var input0 = UIFactory.CreateInputField(Horizontal_0, "input0", "请输入。。。");

            //var Btn0 = UIFactory.CreateButton(Horizontal_0, "MainWindowHorizontal_0Btn0", "增加额外手部格子");
            //ZGUIUtility.SetLayoutElement(Btn0, minHeight: 25, minWidth: 80);
            //Btn0.OnClick = new Action(() => 
            //{ 

            //});
            //var Btn1 = UIFactory.CreateButton(Horizontal_0, "MainWindowHorizontal_0Btn1", "测试按钮1");
            //ZGUIUtility.SetLayoutElement(Btn1, minHeight: 25, minWidth: 80);
            //Btn1.OnClick = new Action(() => { ZGScriptTrainer.WriteLog(Btn1.ButtonText.text); });
            //var Btn2 = UIFactory.CreateButton(Horizontal_0, "MainWindowHorizontal_0Btn2", "测试按钮222222222");
            //ZGUIUtility.SetLayoutElement(Btn2, minHeight: 25, minWidth: 80);
            //Btn2.OnClick = new Action(() => { ZGScriptTrainer.WriteLog(Btn2.ButtonText.text); });

            GameObject toggleObj = UIFactory.CreateToggle(tabGroup, "UseGameCameraToggle", out var useGameCameraToggle, out Text toggleText);
            UIFactory.SetLayoutElement(toggleObj, minHeight: 25, flexibleWidth: 9999);
            useGameCameraToggle.onValueChanged.AddListener((UnityAction<bool>)((bool state) => { ZGScriptTrainer.WriteLog(state); }));
            useGameCameraToggle.isOn = false;
            toggleText.text = "Use Game Camera?";
            toggleText.fontSize = ZGScriptTrainer.FontSize.Value;

            string titleTxt = $"UE <i><color=grey>1.0.0</color></i>";
            Text title = UIFactory.CreateLabel(tabGroup, "Title", titleTxt, TextAnchor.MiddleLeft, default, true, ZGScriptTrainer.FontSize.Value);
            UIFactory.SetLayoutElement(title.gameObject, minWidth: 75, flexibleWidth: 0);



            Text title2 = UIFactory.CreateLabel(tabGroup, "Title", titleTxt + "233", TextAnchor.MiddleLeft, default, true, ZGScriptTrainer.FontSize.Value);
            UIFactory.SetLayoutElement(title.gameObject, minWidth: 75, flexibleWidth: 0);
            // default active state: Active
            this.SetActive(true);
        }
    }
}
