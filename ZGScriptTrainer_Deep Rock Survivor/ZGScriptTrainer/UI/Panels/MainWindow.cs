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
using DRS.UI;
using static Il2CppSystem.DateTimeParse;

namespace ZGScriptTrainer.UI.Panels
{
    public class MainWindow : ZGPanel
    {
        public static MainWindow Instance { get; private set; }
        public override ZGUIManager.Panels PanelType => ZGUIManager.Panels.MainWindow;
        public override string Name => ZGUIManager.PanelNames[PanelType];
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

            Text title = UIFactory.CreateLabel(tabGroup, "Title", "升级修改：", TextAnchor.MiddleLeft, default, true, ZGScriptTrainer.FontSize.Value);
            UIFactory.SetLayoutElement(title.gameObject, minWidth: 75, flexibleWidth: 0);
            tabGroup.CreateSplitPanel(Color.white);

            GameObject Horizontal_0 = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowHorizontal_0", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Horizontal_0, minHeight: 40, flexibleWidth: 9999);


            var Btn0 = Horizontal_0.CreateButton("添加10000金币", new Action(() =>
            {
                UIManager.instance.gameController.player.wallet.Add(ECurrency.CREDITS, 10000);
            }));
            ZGUIUtility.SetLayoutElement(Btn0, minHeight: 25, minWidth: 80);
            var Btn1 = Horizontal_0.CreateButton("每种资源加1000", new Action(() =>
            {
                UIManager.instance.gameController.player.wallet.Add(ECurrency.CROPPA, 1000);
                UIManager.instance.gameController.player.wallet.Add(ECurrency.ENOR_PEARL, 1000);
                UIManager.instance.gameController.player.wallet.Add(ECurrency.BISMOR, 1000);
                UIManager.instance.gameController.player.wallet.Add(ECurrency.JADIZ, 1000);
                UIManager.instance.gameController.player.wallet.Add(ECurrency.MAGNITE, 1000);
                UIManager.instance.gameController.player.wallet.Add(ECurrency.UMANITE, 1000);
            }));
            ZGUIUtility.SetLayoutElement(Btn1, minHeight: 25, minWidth: 80);
            var Btn2 = Horizontal_0.CreateButton("增加10000职业经验(结算时生效)", new Action(() =>
            {
                if(UIManager.instance.gameController.player.CurrentClass != null)
                {
                    ZGScriptTrainer.WriteLog($"{UIManager.instance.gameController.player.CurrentClass.DisplayName}");
                    UIManager.instance.milestoneTracker.playerRankManager.AddRankXp(10000, UIManager.instance.gameController.player.CurrentClass.Dwarf);
                }
                else
                {
                    ZGScriptTrainer.WriteLog($"CurrentClass == null");
                    UIManager.instance.milestoneTracker.playerRankManager.AddRankXp(10000, EDwarf.SCOUT);
                    UIManager.instance.milestoneTracker.playerRankManager.OnCoreRunEnd(ECoreRunEndCondition.WIN);
                }
                
                //UIManager.instance.gameController.player.GainXp(10000);
            }));
            ZGUIUtility.SetLayoutElement(Btn2, minHeight: 25, minWidth: 80);

            tabGroup.CreateSplitPanel(Color.white);
            UIFactory.CreateLabel(tabGroup, "Title", "游戏中修改：", TextAnchor.MiddleLeft, default, true, ZGScriptTrainer.FontSize.Value).SetLayoutElement(minWidth: 75, flexibleWidth: 0);
            tabGroup.CreateSplitPanel(Color.white);

            GameObject Horizontal_1 = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowHorizontal_1", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft).SetLayoutElement(minHeight: 40, flexibleWidth: 9999);

            Horizontal_1.CreateButton("升级", new Action(() =>
            {
                var player = UIManager.instance.gameController.player;
                if (player.CurrentClass != null)
                {
                    player.GainXp(10000);
                    player.currentXp = 0;
                }
            })).SetLayoutElement(minHeight: 25, minWidth: 80);

            Horizontal_1.CreateButton("黄金加1000", new Action(() =>
            {
                var player = UIManager.instance.gameController.player;
                if (player.CurrentClass != null)
                {
                    player.wallet.Add(ECurrency.GOLD, 1000);
                }
            })).SetLayoutElement(minHeight: 25, minWidth: 80);

            Horizontal_1.CreateButton("硝石加1000", new Action(() =>
            {
                var player = UIManager.instance.gameController.player;
                if (player.CurrentClass != null)
                {
                    player.wallet.Add(ECurrency.NITRA, 1000);
                }
            })).SetLayoutElement(minHeight: 25, minWidth: 80);

            Horizontal_1.CreateButton("回复血量", new Action(() =>
            {
                var player = UIManager.instance.gameController.player;
                if (player.CurrentClass != null)
                {
                    player.Heal(player.maxHp.IntValue);
                }
            })).SetLayoutElement(minHeight: 25, minWidth: 80);

            GameObject Horizontal_2 = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowHorizontal_2", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft).SetLayoutElement(minHeight: 40, flexibleWidth: 9999);

            Horizontal_2.CreateToggle("上帝模式", (bool state) => 
            {
                var player = UIManager.instance.gameController.player;
                player.GOD_MODE = state;
            }).SetLayoutElement(minHeight: 25, minWidth: 80);

            Horizontal_2.CreateToggle("无限撤退时间", (bool state) =>
            {
                if (state)
                {
                    ZGTrainerBehaviour.Actions.Add("无限撤退时间", () =>
                    {
                        var pod = UIManager.instance.gameController.DropPod;
                        if (pod != null)
                        {
                            if (pod.state == DropPod.EState.WAITING_FOR_PLAYER || pod.state == DropPod.EState.WAITING_FOR_PLAYER_EXIT)
                            {
                                pod.secondsToTimeOutLeft = pod.secondsToTimeOut;
                                ZGScriptTrainer.WriteLog($"pod : {pod.secondsToTimeOutLeft};{pod.timeLeft};{pod.timeToActivate};{pod.timeOutProgress}");
                            }
                        }
                    });
                }
                else
                {
                    ZGTrainerBehaviour.Actions.Remove("无限撤退时间");
                }
                var player = UIManager.instance.gameController.player;
                player.GOD_MODE = state;
                        }).SetLayoutElement(minHeight: 25, minWidth: 80);

            tabGroup.CreateSplitPanel(Color.white);
            UIFactory.CreateLabel(tabGroup, "Title", "属性修改：", TextAnchor.MiddleLeft, default, true, ZGScriptTrainer.FontSize.Value).SetLayoutElement(minWidth: 75, flexibleWidth: 0);
            tabGroup.CreateSplitPanel(Color.white);



            int count = 0;
            
            GameObject Horizontal_3 = UIFactory.CreateGridGroup(tabGroup, "Content", new Vector2(205, 40), new Vector2(4, 4));
            foreach (var x in Enum.GetValues(typeof(EStatType)))
            {
                EStatType t = (EStatType)x;
                if (t == EStatType.CREDIT_GAIN)
                    continue;
                
                var tmpinput = Horizontal_3.CreateInputEditButton($"{UIDisplay.StatName(t)}:", "10", "添加", (string text) =>
                {
                    var player = UIManager.instance.gameController.player;
                    if (player.CurrentClass != null)
                    {
                        float value = text.ConvertToFloatDef(10);
                        if (t != EStatType.MAX_HP && t != EStatType.ARMOR && t != EStatType.LIFE_REGEN && t != EStatType.CLIP_SIZE && t != EStatType.LUCK && t != EStatType.BEAM_COUNT && t != EStatType.DRONE_COUNT)
                        {
                            value /= 100;
                        }
                        player.stats.ModifyStat(t, value, EModGroupType.PICKUP, EStatModID.NONE);
                    }
                });
                count++;
            }

            // default active state: Active
            this.SetActive(true);
        }
    }
}
