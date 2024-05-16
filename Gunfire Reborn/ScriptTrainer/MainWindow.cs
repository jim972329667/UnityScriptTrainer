using ScriptTrainer.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using Object = UnityEngine.Object;
using Navigation = ScriptTrainer.UI.Navigation;
using static UnityEngine.Random;
using Item;
using HeroWarSign;
using Relic;
using DYPublic.Duonet;
using System.Security.Cryptography;
using System.IO;
using Il2CppSystem.Threading.Tasks;

namespace ScriptTrainer
{
    public class MainWindow : MonoBehaviour
    {
        public MainWindow(IntPtr handle) : base(handle) { }
        #region[声明]
        // Trainer Base
        public static GameObject obj = null;
        public static MainWindow instance;
        public static bool initialized = false;
        public static bool _optionToggle = false;
        public static DragAndDrog dragAndDrog = null;

        // UI
        public static GameObject canvas = null;
        private static GameObject uiPanel = null;
        public static readonly int width = Mathf.Min(Screen.width, 740);
        private static readonly int height = (Screen.height < 400) ? Screen.height : (450);

        public static Vector2 Position = new Vector2(initialX-40, initialY);
        // 按钮位置
        private static int elementX = initialX;
        private static int elementY = initialY;
        private static int initialX
        {
            get
            {
                return -width / 2 + 120;
            }
        }
        private static int initialY
        {
            get
            {
                return height / 2 - 60;
            }
        }
        public static bool optionToggle
        {
            get => _optionToggle;
            set
            {
                _optionToggle = value;

                if (_optionToggle)
                {

                }
                else
                {
                    dragAndDrog.isMouseDrag = false;
                }

                if (!initialized)
                {
                    instance.CreateUI();
                }
            }
        }

        #endregion

        internal static GameObject Create(string name)
        {
            obj = new GameObject(name);
            DontDestroyOnLoad(obj);

            var component = new MainWindow();


            return obj;
        }

        public MainWindow()
        {
            instance = this;
        }
        public static void Initialize()
        {
            #region[初始化资源]

            #endregion

            instance.CreateUI();

            initialized = true;
        }

        #region[创建UI]
        private void CreateUI()
        {
            if (canvas == null)
            {
                Debug.Log("创建 UI 元素");

                canvas = UIControls.createUICanvas(ScriptTrainer.WindowSizeFactor.Value);
                Object.DontDestroyOnLoad(canvas);
                // 设置背景
                GameObject background = UIControls.createUIPanel(canvas, (height + 40).ToString(), (width + 40).ToString(), null);
                background.GetComponent<Image>().color = UIControls.HTMLString2Color("#2D2D30FF");

                // 将面板添加到画布, 请参阅 createUIPanel 了解我们将高度/宽度作为字符串传递的原因
                uiPanel = UIControls.createUIPanel(background, height.ToString(), width.ToString(), null);
                // 设置背景颜色
                uiPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");

                // 这就是我们将如何挂钩鼠标事件以进行窗口拖动
                MainWindow.dragAndDrog = background.AddComponent<DragAndDrog>();
                dragAndDrog.WindowSizeFactor = ScriptTrainer.WindowSizeFactor.Value;
                #region[面板元素]


                #region[创建标题 和 关闭按钮]
                GameObject title = AddTitle(ScriptTrainer.PluginName + " by:Jim97 版本:" + ScriptTrainer.Version, uiPanel);

                GameObject closeButton = UIControls.createUIButton(uiPanel, "#B71C1CFF", "X", () =>
                {
                    optionToggle = false;
                    canvas.SetActive(optionToggle);
                }, new Vector3(width / 2 + 10, height / 2 + 10, 0));
                closeButton.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
                // 字体颜色为白色
                closeButton.GetComponentInChildren<Text>().color = UIControls.HTMLString2Color("#FFFFFFFF");

                #endregion

                GameObject BasicScripts = UIControls.createUIPanel(uiPanel, "410", "600", null);
                BasicScripts.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                BasicScripts.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, -20);

                #region[添加功能按钮]
                BasicScripts.AddH3(ref Position, "基础功能：");
                {
                    BasicScripts.AddButton(ref Position, "添加技能点", () =>
                    {
                        //int pid = 0;
                        //foreach (var item in NewPlayerManager.PlayerTeamerAccountIdDict)
                        //{
                        //    pid = item.Key;
                        //    break;
                        //}
                        //var player = NewPlayerManager.GetPlayer(pid);
                        //if (pid == 0 || player == null)
                        //    return;


                        //var prop = player.playerProp;
                        //prop.ShieldMax = 9999;
                        //prop.ClientShield = 9999;
                        //prop.Shield = 9999;
                        //prop.WarGSCash = 999999;
                        //prop.WarCash = 999999;
                        //player.ShieldCalCom.SetServerProp(prop);

                        //List<string> strings = new List<string>();
                        //foreach (var x in DataHelper.DataMgr.RelicModule.m_RelicDataDict)
                        //{
                        //    strings.Add($"{x.Key};{x.value.IconID};{x.value.Name};{x.value.RelicType};{x.value.Desc}");
                        //}
                        //File.WriteAllLines("F:\\GR_Relics.txt", strings);
                        //int pos = RelicManager.m_EquipRelic.Count + 1;
                        //foreach (var x in DataHelper.DataMgr.RelicModule.m_RelicDataDict)
                        //{
                        //    if(x.value.RelicType == "RELIC_TYPE_NORMAL")
                        //    {
                        //        ScriptTrainer.WriteLog($"Relic:{x};");
                        //        RelicManager.AddRelicObject(new RelicObject(x.Key, pos, 1, true, 0, false, true));
                        //        pos++;
                        //    }

                        //}

                        //for(int i = 1001; i < 1050; i++)
                        //{
                        //    var task = NewSeasonMgr.Instance.GetCurSeasonTask(i);
                        //    if (task != null)
                        //    {
                        //        string text = $"Season Task {i}:{task.CurValue};{task.TotalValue};Ext:";
                        //        foreach (var y in task.Exts)
                        //        {
                        //            text += $"type:{y.type};";
                        //            foreach(var z in y.datas)
                        //            {
                        //                text += $"{z};";
                        //            }
                        //        }
                        //        ScriptTrainer.WriteLog(text);
                        //    }
                        //}

                        var season = NewSeasonMgr.Instance.CurSeasonData;
                        ScriptTrainer.WriteLog($"Season :{season.Grade} {season.CurGradeExp} {season.Season} {season.TotalEx}");
                        var xx = new homecontainer_GS2CReceiveRewardResultClass();
                        xx.lstResult = new Il2CppSystem.Collections.Generic.List<homecontainer_GS2CReceiveRewardResultClass.ClasslstResult>();

                        //NewSeasonMgr.Instance.ReceiveSeasonRewards(1);
                        foreach (var x in season.TaskLst)
                        {
                            if (x.Exts.Count == 0 && x.CurValue != x.TotalValue)
                            {
                                var tmp = new homecontainer_GS2CReceiveRewardResultClass.ClasslstResult();
                                tmp.SID = x.SID;
                                tmp.iResult = x.TotalValue;
                                xx.lstResult.Add(tmp);
                                //NewSeasonMgr.Instance.InitCurSeasonTask(x.SID, x.CurValue, x.TotalValue, x.Exts);
                            }
                            string text = $"Season Task {x.SID}/{NewSeasonMgr.Instance.GetCurSeasonRewardStatus(x.SID)}:{x.CurValue};{x.TotalValue};";
                            foreach (var y in x.Exts)
                            {
                                text += $"Ext: type:{y.type};";
                                foreach (var z in y.datas)
                                {
                                    text += $"{z};";
                                }
                            }
                            ScriptTrainer.WriteLog(text);
                        }

                        NewSeasonMgr.Instance.UpdateCurSeasonRewardsOnReceive(xx);
                        //foreach (var x in CharaterData.GetAllHeroListA())
                        //{
                        //    if (!CharaterData.OwnHeroSIDDict.ContainsKey(x))
                        //    {
                        //        CharaterData.OwnHeroSIDDict.Add(x,new CharaterData.HeroInfo(x, 5));
                        //        ScriptTrainer.WriteLog($"steamDlc:{x}");
                        //    }

                        //}
                        //foreach (var x in DataHelper.DataMgr.DlcDataModule.m_DlcData)
                        //{
                        //    ScriptTrainer.WriteLog($"{x.key}:{x.value.Name};{x.value.DlcType};{x.value.HomeShow}");
                        //    x.value.DlcType = 2;
                        //}
                        //foreach (var x in DataHelper.DataMgr.RelicModule.m_RelicDataDict)
                        //{
                        //    ScriptTrainer.WriteLog($"{x.key}:{x.value.Name};{x.value.DropModelID};{x.value.RelicType};");
                        //}
                        //foreach(var x in RelicManager.GetRelicByType("RELIC_TYPE_NORMAL"))
                        //{
                        //    ScriptTrainer.WriteLog($"{x.RelicID}:{x.relicType};{x.Pos};");
                        //}
                        //var weapon = SurvivalModeManager.instance.GetCurWeaponObject();
                        //if(weapon != null)
                        //{
                        //    weapon.SIProp.CurBullet = weapon.SIProp.MaxBullet;
                        //    weapon.SIProp.CurPFBullet =weapon.SIProp.MaxPFBullet;
                        //    weapon.SIProp.ClientCurBullet = weapon.SIProp.MaxBullet;
                        //}
                    });
                    BasicScripts.AddToggle(ref Position, "最大全选/刷新",180, (bool state) =>
                    {
                        ScriptPatch.SetMaxChoose = state;
                    });
                    BasicScripts.AddToggle(ref Position, "无限弹药", 180, (bool state) =>
                    {
                        ZGGameObject.IsInfBullet = state;
                    });
                    BasicScripts.AddButton(ref Position, "添加技能点", () => 
                    {
                        Il2CppSystem.Collections.Generic.List<int> ints = new Il2CppSystem.Collections.Generic.List<int>();
                        ints.Add(1004);
                        ints.Add(1004);
                        NewSeasonMgr.Instance.ReceiveRewards(ints);
                    });
                    //BasicScripts.AddButton(ref Position, "添加技能点", () =>
                    //{
                    //    ScriptTrainer.WriteLog($"{UserCenter.User_URL};{UserCenter.UserReportUrl};{UserCenter.PayReportUrl}");
                    //    //UIWindows.SpawnInputDialog($"您想添加多少技能点？", "添加", "100", (string count) =>
                    //    //{
                    //    //    Scripts.AddSkillPoint(count.ConvertToIntDef(100));
                    //    //});
                    //});

                }
                BasicScripts.hr(ref Position);
                //BasicScripts.AddH3(ref Position, "背包功能：");
                //{
                //    BasicScripts.AddToggle(ref Position, "最大背包", 110, (bool state) =>
                //    {
                //        Scripts.MaxBackpackSize(state);
                //    });

                //}
                BasicScripts.hr(ref Position);
                //BasicScripts.AddH3(ref Position, "人物功能：");
                {
                }
                #endregion

                #region[获取物品]
                ResetCoordinates(true, true);

                GameObject ItemScripts = UIControls.createUIPanel(uiPanel, "410", "600", null);
                ItemScripts.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                ItemScripts.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, -20);

                ItemWindow itemWindow = new ItemWindow(ItemScripts, elementX, elementY);


                #endregion
                #endregion


                #region[创建导航栏]
                // 分割线
                GameObject DividingLine = UIControls.createUIPanel(uiPanel, (height - 40).ToString(), "10", null);
                DividingLine.GetComponent<Image>().color = UIControls.HTMLString2Color("#2D2D30FF");
                DividingLine.GetComponent<RectTransform>().anchoredPosition = new Vector3(width / 2 - 200 + 80, -20, 0);

                //// 按钮
                GameObject NavPanel = UIControls.createUIPanel(uiPanel, (height - 40).ToString(), "40", null);
                NavPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                NavPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(width / 2 - 100, -20, 0);

                Navigation[] nav = new Navigation[]
                {
                    new Navigation("BasicScripts","基础功能", BasicScripts, true),
                    new Navigation("ItemScripts", "物品添加", ItemScripts, false),
                };

                UINavigation.Initialize(nav, NavPanel);

                #endregion

                canvas.SetActive(optionToggle);
                Debug.Log("初始化完成!");
            }
        }

        #region[添加组件]
        
        // 添加标题
        public static GameObject AddTitle(string Title, GameObject background)
        {
            GameObject TitleBackground = UIControls.createUIPanel(background, "30", (width - 20).ToString(), null);
            TitleBackground.GetComponent<Image>().color = UIControls.HTMLString2Color("#2D2D30FF");
            TitleBackground.GetComponent<RectTransform>().localPosition = new Vector3(0, height / 2 - 30, 0);

            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(TitleBackground, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 10, 30);
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            Text text = uiText.GetComponent<Text>();
            text.text = Title;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 16;

            return uiText;
        }
        // 重置坐标
        public void ResetCoordinates(bool x, bool y = false)
        {
            if (x) elementX = initialX;
            if (y) elementY = initialY;

            if (x) Position.x = initialX - 40;
            if (y) Position.y = initialY;
        }
        #endregion



        #endregion




    }
}
