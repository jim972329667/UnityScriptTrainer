using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI;
using UnityEngine.Events;

namespace ZGScriptTrainer.UI.Panels
{
    public class MainWindow : ZGPanel
    {
        public static MainWindow Instance { get; private set; }
        public override UIManager.Panels PanelType => UIManager.Panels.MainWindow;
        public override string Name => UIManager.PanelNames[PanelType];
        public override int MinWidth => 350;
        public override int MinHeight => 200;
        public override Vector2 DefaultAnchorMin => new Vector2(1f, 1f) /*new Vector2(0.35f, 0.175f)*/;
        public override Vector2 DefaultAnchorMax => new Vector2(1f, 1f)/*new Vector2(0.8f, 0.925f)*/;
        

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
                new Vector4(5, 5, 5, 5), new Color(0.065f, 0.065f, 0.065f));
            UIFactory.SetLayoutElement(tabGroup, minHeight: 40, flexibleHeight: 0);

            string titleTxt = $"基础修改：";
            Text title = UIFactory.CreateLabel(tabGroup, "Title", titleTxt, TextAnchor.MiddleLeft, default, true, ZGScriptTrainer.FontSize.Value);
            UIFactory.SetLayoutElement(title.gameObject, minWidth: 75, flexibleWidth: 0);

            var DiscountToggleUI = UIFactory.CreateToggle(tabGroup, "DiscountToggle", out Toggle DiscountToggle, out Text DiscountText);

            UIFactory.SetLayoutElement(DiscountToggleUI, minWidth: 150, flexibleWidth: 0, minHeight: 40);
            DiscountText.alignment = TextAnchor.MiddleLeft;
            DiscountText.text = "开启商品折扣";
            DiscountToggle.onValueChanged.AddListener((UnityAction<bool>)((bool val) => { ZGScriptTrainer.IsDiscount.Value = val; }));
            DiscountToggle.isOn = ZGScriptTrainer.IsDiscount.Value;

            //GameObject Horizontal_0 = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowHorizontal_0", false, false, true, true, 5, default,
            //    new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            //UIFactory.SetLayoutElement(Horizontal_0, minHeight: 40, flexibleWidth: 9999);

            //var Btn0 = UIFactory.CreateButton(Horizontal_0, "MainWindowHorizontal_0Btn0", "测试按钮0");
            //ZGUIUtility.SetLayoutElement(Btn0, minHeight: 25, minWidth: 80);
            //Btn0.OnClick = new Action(() => { ZGScriptTrainer.WriteLog(Btn0.ButtonText.text); });
            //var Btn1 = UIFactory.CreateButton(Horizontal_0, "MainWindowHorizontal_0Btn1", "测试按钮1");
            //ZGUIUtility.SetLayoutElement(Btn1, minHeight: 25, minWidth: 80);
            //Btn1.OnClick = new Action(() => { ZGScriptTrainer.WriteLog(Btn1.ButtonText.text); });
            //var Btn2 = UIFactory.CreateButton(Horizontal_0, "MainWindowHorizontal_0Btn2", "测试按钮222222222");
            //ZGUIUtility.SetLayoutElement(Btn2, minHeight: 25, minWidth: 80);
            //Btn2.OnClick = new Action(() => { ZGScriptTrainer.WriteLog(Btn2.ButtonText.text); });





            //Text title2 = UIFactory.CreateLabel(tabGroup, "Title", titleTxt + "233", TextAnchor.MiddleLeft, default, true, ZGScriptTrainer.FontSize.Value);
            //UIFactory.SetLayoutElement(title.gameObject, minWidth: 75, flexibleWidth: 0);
            // default active state: Active


            this.SetActive(true);
        }
    }
}
