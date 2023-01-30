using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityGameUI;

namespace ScriptTrainer
{
    // Token: 0x0200000C RID: 12
    internal class CharacterWindow
    {
        #region[界面信息]
        private static GameObject Panel;
        private static int initialX;
        private static int initialY;
        private static int elementX;
        private static int elementY;
        private static GameObject ItemPanel;
        private static List<GameObject> ItemButtons = new List<GameObject>();
        private static int page = 1;
        private const int maxPage = 5;

        private static GameObject uiText;
        private static string uiText_text
        {
            get
            {
                return string.Format("{0} / {1}", page, 5);
            }
        }
        #endregion

        #region[数据信息]
        private static List<CharacterValueInfo<string>> CharacterStringValue = new List<CharacterValueInfo<string>>();
        private static List<CharacterValueInfo<int>> CharacterIntValue = new List<CharacterValueInfo<int>>();
        private static List<CharacterValueInfo<float>> CharacterFloatValue = new List<CharacterValueInfo<float>>();
        private static List<CharacterValueInfo<Gender>> CharacterGenderValue = new List<CharacterValueInfo<Gender>>();
        private static bool IsPlayer = true;
        #endregion
        
        public CharacterWindow(GameObject panel, int x, int y)
        {
            Panel = panel;
            initialX = (elementX = x + 10);
            elementY = y;
            initialY = y;
            Initialize();
        }
        public void Initialize()
        {
            elementX = initialX;
            elementY = initialY;

            GameObject button_1 = UIControls.createUIButton(Panel, "#8C9EFFFF", "获取角色信息", delegate
            {
                ScriptTrainer.Instance.Log("ZG:获取角色信息");
                GetCharacterValue(1);
                container();
            }, new Vector2(elementX, elementY));
            button_1.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 30f);
            elementX += 110;

            hr();
            bool flag = TryGetData();
            if (flag)
            {
                container();
            }
            else
            {
                elementY = -175;
            }
            PageBar(Panel);
        }

        #region[绘制角色控件]
        private static void CharacterBar(GameObject panel, int width, CharacterValueInfo<string> Name)
        {
            int num = 70;
            int num2 = 70 + width + 10;
            int num3 = -num2 / 2 + 45;
            GameObject gameObject = UIControls.createUIPanel(panel, "40", num2.ToString(), null);
            gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#455A64FF");
            gameObject.GetComponent<RectTransform>().localPosition = new Vector2(elementX, elementY);
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject2 = UIControls.createUIText(gameObject, bgSprite, "#FFFFFFFF");
            gameObject2.GetComponent<Text>().text = Name.Name;
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector2((float)num3, 0f);
            gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2((float)num, 30f);
            gameObject2.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            num3 += num + width / 2 - 40;
            Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject gameObject3;
            if (IsPlayer)
            {
                gameObject3 = UIControls.createUIInputField(gameObject, bgSprite2, "#FFFFFFFF");
                gameObject3.GetComponent<InputField>().text = Name.BaseValue.Value;
                gameObject3.GetComponent<RectTransform>().localPosition = new Vector2((float)num3, 0f);
                gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2((float)width, 30f);
                gameObject3.GetComponent<InputField>().onEndEdit.AddListener(delegate (string text)
                {
                    Name.BaseValue.Value = text;
                });
            }
            else
            {
                gameObject3 = UIControls.createUIText(gameObject, bgSprite2, "#FFFFFFFF");
                gameObject3.GetComponent<Text>().text = Name.BaseValue.Value;
                gameObject3.GetComponent<RectTransform>().localPosition = new Vector2((float)num3, 0f);
                gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2((float)width, 30f);
            }
            elementX += num2;
            ItemButtons.Add(gameObject);
        }
        private static void CharacterBar(GameObject panel, int width, CharacterValueInfo<int> Name)
        {
            int num = 70;
            int num2 = 70 + width + 10;
            int num3 = -num2 / 2 + 45;
            GameObject gameObject = UIControls.createUIPanel(panel, "40", num2.ToString(), null);
            gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#455A64FF");
            gameObject.GetComponent<RectTransform>().localPosition = new Vector2(elementX, elementY);
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject2 = UIControls.createUIText(gameObject, bgSprite, "#FFFFFFFF");
            gameObject2.GetComponent<Text>().text = Name.Name;
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector2((float)num3, 0f);
            gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2((float)num, 30f);
            gameObject2.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            num3 += num + width / 2 - 40;
            Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject gameObject3 = UIControls.createUIInputField(gameObject, bgSprite2, "#FFFFFFFF");
            gameObject3.GetComponent<InputField>().text = Name.BaseValue.Value.ToString();
            gameObject3.GetComponent<RectTransform>().localPosition = new Vector2((float)num3, 0f);
            gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2((float)width, 30f);
            gameObject3.GetComponent<InputField>().onEndEdit.AddListener(delegate (string text)
            {
                Name.BaseValue.Value = text.ConvertToIntDef(Name.BaseValue.Value);
            });
            elementX += num2;
            ItemButtons.Add(gameObject);
        }
        private static void CharacterBar(GameObject panel, int width, CharacterValueInfo<float> Name)
        {
            int num = 70;
            int num2 = 70 + width + 10;
            int num3 = -num2 / 2 + 45;
            GameObject gameObject = UIControls.createUIPanel(panel, "40", num2.ToString(), null);
            gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#455A64FF");
            gameObject.GetComponent<RectTransform>().localPosition = new Vector2(elementX, elementY);
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject2 = UIControls.createUIText(gameObject, bgSprite, "#FFFFFFFF");
            gameObject2.GetComponent<Text>().text = Name.Name;
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector2((float)num3, 0f);
            gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2((float)num, 30f);
            gameObject2.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            num3 += num + width / 2 - 40;
            Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject gameObject3 = UIControls.createUIInputField(gameObject, bgSprite2, "#FFFFFFFF");
            gameObject3.GetComponent<InputField>().text = Name.BaseValue.Value.ToString();
            gameObject3.GetComponent<RectTransform>().localPosition = new Vector2((float)num3, 0f);
            gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2((float)width, 30f);
            gameObject3.GetComponent<InputField>().onEndEdit.AddListener(delegate (string text)
            {
                Name.BaseValue.Value = text.ConvertToFloatDef(Name.BaseValue.Value);
            });
            elementX += num2;
            ItemButtons.Add(gameObject);
        }
        #endregion

        #region[绘制更换页面按键]
        private void PageBar(GameObject panel)
        {
            GameObject gameObject = UIControls.createUIPanel(panel, "40", "500", null);
            gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, (float)elementY, 0f);
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            bool flag = uiText == null;
            if (flag)
            {
                uiText = UIControls.createUIText(gameObject, bgSprite, "#ffFFFFFF");
                uiText.GetComponent<Text>().text = uiText_text;
                uiText.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
                uiText.GetComponent<Text>().fontSize = 20;
                uiText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            }
            string backgroundColor = "#8C9EFFFF";
            GameObject gameObject2 = UIControls.createUIButton(gameObject, backgroundColor, "上一页", delegate
            {
                page--;
                bool flag2 = page <= 0;
                if (flag2)
                {
                    page = 1;
                }
                GetCharacterValue(page);
                bool flag3 = TryGetData();
                if (flag3)
                {
                    container();
                }
                uiText.GetComponent<Text>().text = uiText_text;
            }, default(Vector3));
            gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 20f);
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector3(-100f, 0f, 0f);
            GameObject gameObject3 = UIControls.createUIButton(gameObject, backgroundColor, "下一页", delegate
            {
                page++;
                bool flag2 = page >= 5;
                if (flag2)
                {
                    page = 5;
                }
                GetCharacterValue(page);
                bool flag3 = TryGetData();
                if (flag3)
                {
                    container();
                }
                uiText.GetComponent<Text>().text = uiText_text;
            }, default(Vector3));
            gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 20f);
            gameObject3.GetComponent<RectTransform>().localPosition = new Vector3(100f, 0f, 0f);
        }
        #endregion

        #region[加载页面]
        private static void container()
        {
            elementX = -195;
            elementY = 125;
            ItemPanel = UIControls.createUIPanel(Panel, "300", "600", null);
            ItemPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            ItemPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(10f, 0f);
            foreach (GameObject obj in ItemButtons)
            {
                UnityEngine.Object.Destroy(obj);
            }
            ItemButtons.Clear();
            int num = 0;
            foreach (CharacterValueInfo<string> name in CharacterStringValue)
            {
                CharacterBar(ItemPanel, 100, name);
                num++;
                bool flag = num % 3 == 0;
                if (flag)
                {
                    hr(-195, true);
                }
                else
                {
                    elementX += 10;
                }
            }
            foreach (CharacterValueInfo<int> name2 in CharacterIntValue)
            {
                CharacterBar(ItemPanel, 100, name2);
                num++;
                bool flag2 = num % 3 == 0;
                if (flag2)
                {
                    hr(-195, true);
                }
                else
                {
                    elementX += 10;
                }
            }
            foreach (CharacterValueInfo<float> name3 in CharacterFloatValue)
            {
                CharacterBar(ItemPanel, 100, name3);
                num++;
                bool flag3 = num % 3 == 0;
                if (flag3)
                {
                    hr(-195, true);
                }
                else
                {
                    elementX += 10;
                }
            }
        }
        #endregion

        #region[界面换行]
        private static void hr(int x = 0, bool setX = false, int y = -60)
        {
            if (setX)
                ChangePoint(x, y, setX);
            else
                ChangePoint(initialX, y, true);
        }
        private static void ChangePoint(int x = 0, int y = 0, bool setX = false, bool setY = false)
        {
            if(setX)
                elementX = x;
            else
                elementX += x;

            if(setY)
                elementY = y;
            else
                elementY += y;
        }
        private static void ResetCoordinates(bool x, bool y = false)
        {
            if (x)
            {
                ChangePoint(initialX, elementY, true, true);
            }
            if (y)
            {
                ChangePoint(elementX, initialY, true, true);
            }
        }
        #endregion

        #region[判断是否能读取游戏数据]
        private static bool TryGetData()
        {
            bool flag = DataManager.Instance.GameData.GameRuntimeData.Player == null;
            return !flag;
        }
        #endregion

        #region[获取游戏数据]
        private static void GetCharacterValue(int page = 1)
        {
            Character player;


            if (UIManager.ClickCharacter != null && UIManager.GetAcitveUIManager().UIDataUtil.mUIRoot.transform.Find("JueSe_Self_BN").gameObject.activeSelf)
            {
                player = UIManager.ClickCharacter;
                IsPlayer = player == DataManager.Instance.GameData.GameRuntimeData.Player;
            }
            else
            {
                IsPlayer = true;
                player = DataManager.Instance.GameData.GameRuntimeData.Player;
            }

            //Character player = DataManager.Instance.GameData.GameRuntimeData.Player;
            CharacterBasicData basicData = player.BasicData;
            CharacterSocialData socialData = player.SocialData;
            CharacterBattleData battleData = player.BattleData;
            CharacterStringValue.Clear();
            CharacterIntValue.Clear();
            CharacterFloatValue.Clear();
            CharacterGenderValue.Clear();
            switch (page)
            {
                case 1:
                    CharacterStringValue.Add(new CharacterValueInfo<string>(basicData.Name, "名字"));
                    CharacterIntValue.Add(new CharacterValueInfo<int>(basicData.Age, "年龄"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.XinTai, "心态"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(basicData.MingSheng.MingSheng, "名声"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(basicData.LiChang, "立场"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(basicData.WuXing, "悟性"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(basicData.JiaoShe, "交涉"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(basicData.JianShi, "见识"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(basicData.FuYuan, "福源"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.XiuWei.DaoDe, "道德"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.XiuWei.MeiLi, "魅力"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.XiuWei.HanYang, "涵养"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.XiuWei.ShaYi, "杀意"));
                    break;
                case 2:
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.WaiGong.QuanZhang, "拳掌"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.WaiGong.JianFa, "用剑"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.WaiGong.DaoFa, "使刀"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.WaiGong.ZhiLi, "指力"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.WaiGong.YongQiang, "弄枪"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.WaiGong.ShuaGun, "刷棍"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.WaiGong.ChuiFu, "运斧"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.WaiGong.ShiBian, "鞭术"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.WaiGong.GouFa, "钩法"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.WaiGong.CunJin, "暗器"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.WaiGong.BiFa, "笔法"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.WaiGong.YaoMa, "腰马"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.WaiGong.XiaPan, "下盘"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.WaiGong.TiPo, "体魄"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.WaiGong.JinGu, "筋骨"));
                    break;
                case 3:
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.NeiGong.QiLiang, "气量"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.NeiGong.TiaoXi, "调息"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.NeiGong.QiFa, "气法"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.NeiGong.ShenXing, "身形"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.JingShen.YiZhi, "意志"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.JingShen.JiZhong, "集中"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.JingShen.ZhenDing, "镇定"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.JingShen.JueDuan, "决断"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.JingShen.ShenLai, "神来"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.MaiLuo.DuMai, "督脉"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.MaiLuo.RenMai, "任脉"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.MaiLuo.ChongDai, "冲带(未知)"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.MaiLuo.QiaoMai, "巧脉(未知)"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(battleData.MaiLuo.WeiMai, "微脉(未知)"));
                    break;
                case 4:
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.ShiCi, "诗词"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.YinJiu, "饮酒"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.PengRen, "烹饪"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.GuanPu, "关扑"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.ChaDao, "茶道"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.ShouCang, "收藏"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.HuiHua, "书画"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.YiShu, "医术"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.YueQi, "乐器"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.DuShu, "毒术"));
                    break;
                case 5:
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.ShiCiLike, "诗词喜爱"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.YinJiuLike, "饮酒喜爱"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.PengRenLike, "烹饪喜爱"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.GuanPuLike, "关扑喜爱"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.ChaDaoLike, "茶道喜爱"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.ShouCangLike, "收藏喜爱"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.HuiHuaLike, "书画喜爱"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.YiShuLike, "医术喜爱"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.YueQiLike, "乐器喜爱"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.Hobby.DuShuLike, "毒术喜爱"));
                    break;
                default:
                    CharacterStringValue.Add(new CharacterValueInfo<string>(basicData.Name, "名字"));
                    CharacterIntValue.Add(new CharacterValueInfo<int>(basicData.Age, "年龄"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(basicData.WuXing, "悟性"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(basicData.JiaoShe, "交涉"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(basicData.JianShi, "见识"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(basicData.FuYuan, "福源"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.XiuWei.DaoDe, "道德"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.XiuWei.MeiLi, "魅力"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.XiuWei.HanYang, "涵养"));
                    CharacterFloatValue.Add(new CharacterValueInfo<float>(socialData.XiuWei.ShaYi, "杀意"));
                    break;
            }
        }
        public static int ParseGenderToInt(Gender gender)
        {
            bool flag = gender == Gender.Nan;
            int result;
            if (flag)
            {
                result = 0;
            }
            else
            {
                bool flag2 = gender == Gender.Nv;
                if (flag2)
                {
                    result = 1;
                }
                else
                {
                    bool flag3 = gender == Gender.Yan;
                    if (flag3)
                    {
                        result = 2;
                    }
                    else
                    {
                        result = 0;
                    }
                }
            }
            return result;
        }
        public static Gender ParseIntToGender(int value)
        {
            bool flag = value == 0;
            Gender result;
            if (flag)
            {
                result = Gender.Nan;
            }
            else
            {
                bool flag2 = value == 1;
                if (flag2)
                {
                    result = Gender.Nv;
                }
                else
                {
                    bool flag3 = value == 2;
                    if (flag3)
                    {
                        result = Gender.Yan;
                    }
                    else
                    {
                        result = Gender.Nan;
                    }
                }
            }
            return result;
        }
        #endregion
        private class CharacterValueInfo<T>
        {
            public string Name;

            public ValueName<T> BaseValue;
            public CharacterValueInfo(ValueName<T> value, string name)
            {
                BaseValue = value;
                Name = name;
            }
        }
    }
}
