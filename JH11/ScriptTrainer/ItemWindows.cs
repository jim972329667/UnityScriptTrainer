using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityGameUI;
using VWVO.ConfigClass.JH_UI;

namespace ScriptTrainer
{
    // Token: 0x0200000F RID: 15
    internal class ItemWindow : MonoBehaviour
    {
        // Token: 0x17000003 RID: 3
        // (get) Token: 0x0600006F RID: 111 RVA: 0x00007260 File Offset: 0x00005460
        private static string uiText_text
        {
            get
            {
                return string.Format("{0} / {1}", ItemWindow.page, ItemWindow.maxPage);
            }
        }

        // Token: 0x06000070 RID: 112 RVA: 0x00007290 File Offset: 0x00005490
        public ItemWindow(GameObject panel, int x, int y)
        {
            ItemWindow.Panel = panel;
            ItemWindow.initialX = (ItemWindow.elementX = x + 50);
            ItemWindow.elementY = y;
            ItemWindow.initialY = y;
            this.Initialize();
        }

        // Token: 0x06000071 RID: 113 RVA: 0x000072C4 File Offset: 0x000054C4
        public void Initialize()
        {
            this.SearchBar(ItemWindow.Panel);
            this.typeBar(ItemWindow.Panel);
            ItemWindow.elementX += 280;
            ItemWindow.hr();
            bool flag = ItemWindow.TryGetData();
            if (flag)
            {
                ItemWindow.container();
            }
            else
            {
                ItemWindow.elementY = -175;
            }
            this.PageBar(ItemWindow.Panel);
        }

        // Token: 0x06000072 RID: 114 RVA: 0x0000732C File Offset: 0x0000552C
        private void typeBar(GameObject panel)
        {
            ItemWindow.elementX += 350;
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject = UIControls.createUIText(panel, bgSprite, "#FFFFFFFF");
            gameObject.GetComponent<Text>().text = "分类";
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)ItemWindow.elementX, (float)ItemWindow.elementY, 0f);
            gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            ItemWindow.elementX += 60;
            List<string> options = new List<string>
            {
                "灵丹妙药",
                "秘籍宝典",
                "医术毒经",
                "食材佳肴",
                "酒器名酒",
                "茶具茶叶",
                "书法绘画",
                "诗词书刊",
                "文玩收藏",
                "兵器戎具",
                "乐器乐谱"
            };
            Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            Sprite scrollbarSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));
            Sprite dropDownSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            Sprite checkmarkSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));
            Sprite customMaskSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#E65100FF"));
            Color labelColor = UIControls.HTMLString2Color("#EFEBE9FF");
            GameObject gameObject2 = UIControls.createUIDropDown(panel, bgSprite2, scrollbarSprite, dropDownSprite, checkmarkSprite, customMaskSprite, options, labelColor);
            //UnityEngine.Object.DontDestroyOnLoad(gameObject2);
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector3((float)ItemWindow.elementX, (float)ItemWindow.elementY, 0f);
            gameObject2.GetComponent<Dropdown>().onValueChanged.AddListener(delegate (int call)
            {
                ItemWindow.type = call;
                ItemWindow.page = 1;
                bool flag = ItemWindow.TryGetData();
                if (flag)
                {
                    ItemWindow.container();
                }
                ItemWindow.uiText.GetComponent<Text>().text = ItemWindow.uiText_text;
            });
        }

        // Token: 0x06000073 RID: 115 RVA: 0x00007514 File Offset: 0x00005714
        private void SearchBar(GameObject panel)
        {
            ItemWindow.elementY += 10;
            ItemWindow.elementX = -MainWindow.width / 2 + 120;
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject = UIControls.createUIText(panel, bgSprite, "#FFFFFFFF");
            gameObject.GetComponent<Text>().text = "搜索";
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)ItemWindow.elementX, (float)ItemWindow.elementY, 0f);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 30f);
            gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            ItemWindow.elementX += 60;
            int num = 260;
            Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject gameObject2 = UIControls.createUIInputField(panel, bgSprite2, "#FFFFFFFF");
            gameObject2.GetComponent<InputField>().text = ItemWindow.searchText;
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector3((float)(ItemWindow.elementX + 100), (float)ItemWindow.elementY, 0f);
            gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2((float)num, 30f);

            gameObject2.GetComponent<InputField>().onEndEdit.AddListener(delegate (string text)
            {
                ItemWindow.page = 1;
                ItemWindow.searchText = text;
                bool flag = ItemWindow.TryGetData();
                if (flag)
                {
                    ItemWindow.container();
                }
                ItemWindow.uiText.GetComponent<Text>().text = ItemWindow.uiText_text;
            });
        }

        // Token: 0x06000074 RID: 116 RVA: 0x0000766C File Offset: 0x0000586C
        private void PageBar(GameObject panel)
        {
            GameObject gameObject = UIControls.createUIPanel(panel, "40", "500", null);
            gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, (float)ItemWindow.elementY, 0f);
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            bool flag = ItemWindow.uiText == null;
            if (flag)
            {
                ItemWindow.uiText = UIControls.createUIText(gameObject, bgSprite, "#ffFFFFFF");
                ItemWindow.uiText.GetComponent<Text>().text = ItemWindow.uiText_text;
                ItemWindow.uiText.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
                ItemWindow.uiText.GetComponent<Text>().fontSize = 20;
                ItemWindow.uiText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            }
            string backgroundColor = "#8C9EFFFF";
            GameObject gameObject2 = UIControls.createUIButton(gameObject, backgroundColor, "上一页", delegate
            {
                ItemWindow.page--;
                bool flag2 = ItemWindow.page <= 0;
                if (flag2)
                {
                    ItemWindow.page = 1;
                }
                bool flag3 = ItemWindow.TryGetData();
                if (flag3)
                {
                    ItemWindow.container();
                }
                ItemWindow.uiText.GetComponent<Text>().text = ItemWindow.uiText_text;
            }, default(Vector3));
            gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 20f);
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector3(-100f, 0f, 0f);
            GameObject gameObject3 = UIControls.createUIButton(gameObject, backgroundColor, "下一页", delegate
            {
                ItemWindow.page++;
                bool flag2 = ItemWindow.page >= ItemWindow.maxPage;
                if (flag2)
                {
                    ItemWindow.page = ItemWindow.maxPage;
                }
                bool flag3 = ItemWindow.TryGetData();
                if (flag3)
                {
                    ItemWindow.container();
                }
                ItemWindow.uiText.GetComponent<Text>().text = ItemWindow.uiText_text;
            }, default(Vector3));
            gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 20f);
            gameObject3.GetComponent<RectTransform>().localPosition = new Vector3(100f, 0f, 0f);
        }

        // Token: 0x06000075 RID: 117 RVA: 0x00007844 File Offset: 0x00005A44
        private static void container()
        {
            ItemWindow.elementX = -200;
            ItemWindow.elementY = 120;
            foreach (GameObject obj in ItemWindow.ItemButtons)
            {
                UnityEngine.Object.Destroy(obj);
            }
            ItemWindow.ItemButtons.Clear();
            ItemWindow.ItemPanel = UIControls.createUIPanel(ItemWindow.Panel, "315", "600", null);
            ItemWindow.ItemPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            //ItemWindow.ItemPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#FFFFFFFF");
            ItemWindow.ItemPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(10f, -7.5f);
            
            ScriptTrainer.Instance.Log("ZG:添加物品");
            if(type != 9)
            {
                int num = 0;
                #region[老添加物品]
                //using (List<Item>.Enumerator enumerator2 = ItemWindow.GetItemData().GetEnumerator())
                //{
                //    while (enumerator2.MoveNext())
                //    {
                //        Item item = enumerator2.Current;
                //        ScriptTrainer.Instance.Log("ZG:物品介绍:" + ItemWindow.GetItemDescription(item));
                //        GameObject item2 = ItemWindow.CreateItemButton("获得", ItemWindow.GetItemName(item), ItemWindow.GetItemIcon(item), ItemWindow.ItemPanel, delegate ()
                //        {
                //            string prompt = "添加" + ItemWindow.GetItemName(item) + "的数量";
                //            string title = "获得";
                //            string defaultText = "1";
                //            UIWindows.SpawnInputDialog(prompt, title, defaultText, (string text) =>
                //            {
                //                ItemWindow.SpawnItem(item, text.ConvertToIntDef(1));
                //            });
                //        });
                //        ItemWindow.ItemButtons.Add(item2);
                //        num++;
                //        bool flag = num % 3 == 0;
                //        if (flag)
                //        {
                //            ItemWindow.hr();
                //        }
                //    }
                //}
                #endregion

                using (List<IEntityItem>.Enumerator enumerator2 = ItemWindow.GetEntityItemData().GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        IEntityItem item = enumerator2.Current;
                        GameObject item2 = ItemWindow.CreateItemButton("获得", ItemWindow.GetEntityItemName(item), ItemWindow.GetEntityItemIcon(item), ItemWindow.ItemPanel, delegate ()
                        {
                            string prompt = "添加" + ItemWindow.GetEntityItemName(item) + "的数量";
                            string title = "获得";
                            string defaultText = "1";
                            UIWindows.SpawnInputDialog(prompt, title, defaultText, (string text) =>
                            {
                                ItemWindow.SpawnEntityItem(item, text.ConvertToIntDef(1));
                            });
                        });
                        ItemWindow.ItemButtons.Add(item2);
                        num++;
                        bool flag = num % 3 == 0;
                        if (flag)
                        {
                            ItemWindow.hr();
                        }
                    }
                }
            }
            else
            {
                int num = 0;
                using (List<WeaponEntity>.Enumerator enumerator2 = ItemWindow.GetWeaponData().GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        WeaponEntity item = enumerator2.Current;
                        ScriptTrainer.Instance.Log("ZG:物品介绍:" + ItemWindow.GetWeaponDescription(item));
                        GameObject item2 = ItemWindow.CreateItemButton("获得", ItemWindow.GetWeaponName(item), ItemWindow.GetWeaponIcon(item), ItemWindow.ItemPanel, delegate ()
                        {
                            string prompt = "添加" + ItemWindow.GetWeaponName(item) + "的数量";
                            string title = "获得";
                            string defaultText = "1";
                            UIWindows.SpawnInputDialog(prompt, title, defaultText, (string text) =>
                            {
                                ItemWindow.SpawnWeapon(item, text.ConvertToIntDef(1));
                            });
                        });
                        ItemWindow.ItemButtons.Add(item2);
                        num++;
                        bool flag = num % 3 == 0;
                        if (flag)
                        {
                            ItemWindow.hr();
                        }
                    }
                }
            }
        }

        // Token: 0x06000076 RID: 118 RVA: 0x000079F0 File Offset: 0x00005BF0
        private static GameObject CreateItemButton(string ButtonText, string ItemName, Sprite ItemIcon, GameObject panel, UnityAction action)
        {
            int num = 190;
            int num2 = 50;
            string htmlcolorstring = "#FFFFFFFF";
            GameObject gameObject = UIControls.createUIPanel(panel, num2.ToString(), num.ToString(), null);
            gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#455A64FF");
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)ItemWindow.elementX, (float)ItemWindow.elementY, 0f);
            GameObject gameObject2 = UIControls.createUIPanel(gameObject, num2.ToString(), "50", null);
            gameObject2.GetComponent<Image>().color = UIControls.HTMLString2Color(htmlcolorstring);
            gameObject2.GetComponent<RectTransform>().anchoredPosition = new Vector2(70f, 0f);
            bool flag = ItemIcon != null;
            if (flag)
            {
                GameObject gameObject3 = UIControls.createUIImage(gameObject2, ItemIcon);
                gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 60f);
                gameObject3.GetComponent<RectTransform>().localPosition = new Vector2(0f, 0f);
            }
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject4 = UIControls.createUIText(gameObject, bgSprite, ColorUtility.ToHtmlStringRGBA(Color.white));
            gameObject4.GetComponent<Text>().text = ItemName;
            gameObject4.GetComponent<RectTransform>().localPosition = new Vector3(0f, 5f, 0f);
            string backgroundColor = "#8C9EFFFF";
            GameObject gameObject5 = UIControls.createUIButton(gameObject, backgroundColor, ButtonText, action, default(Vector3));
            gameObject5.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 20f);
            gameObject5.GetComponent<RectTransform>().localPosition = new Vector3(-50f, -10f, 0f);
            ItemWindow.elementX += 200;
            return gameObject;
        }

        // Token: 0x06000077 RID: 119 RVA: 0x00007BC8 File Offset: 0x00005DC8
        private static GameObject CreateItemButton(string ButtonText, string ItemName, Item ItemIcon, GameObject panel, UnityAction action)
        {
            int num = 190;
            int num2 = 50;
            string htmlcolorstring = "#FFFFFFFF";
            GameObject gameObject = UIControls.createUIPanel(panel, num2.ToString(), num.ToString(), null);
            gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#455A64FF");
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)ItemWindow.elementX, (float)ItemWindow.elementY, 0f);
            GameObject gameObject2 = UIControls.createUIPanel(gameObject, num2.ToString(), "50", null);
            gameObject2.GetComponent<Image>().color = UIControls.HTMLString2Color(htmlcolorstring);
            gameObject2.GetComponent<RectTransform>().anchoredPosition = new Vector2(70f, 0f);
            GameObject gameObject3 = Resources.Load<GameObject>(ItemIcon.Prefabs.GetRandomItem<string>());
            gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 60f);
            gameObject3.GetComponent<RectTransform>().localPosition = new Vector2(0f, 0f);
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject4 = UIControls.createUIText(gameObject, bgSprite, ColorUtility.ToHtmlStringRGBA(Color.white));
            gameObject4.GetComponent<Text>().text = ItemName;
            gameObject4.GetComponent<RectTransform>().localPosition = new Vector3(0f, 5f, 0f);
            string backgroundColor = "#8C9EFFFF";
            GameObject gameObject5 = UIControls.createUIButton(gameObject, backgroundColor, ButtonText, action, default(Vector3));
            gameObject5.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 20f);
            gameObject5.GetComponent<RectTransform>().localPosition = new Vector3(-50f, -10f, 0f);
            ItemWindow.elementX += 200;
            return gameObject5;
        }

        // Token: 0x06000078 RID: 120 RVA: 0x00007D98 File Offset: 0x00005F98
        public static GameObject AddToggle(string Text, int width, GameObject panel, UnityAction<bool> action)
        {
            ItemWindow.elementX += width / 2 - 30;
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture(ColorUtility.ToHtmlStringRGBA(Color.white)));
            Sprite customCheckmarkSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#18FFFFFF"));
            GameObject gameObject = UIControls.createUIToggle(panel, bgSprite, customCheckmarkSprite);
            gameObject.GetComponentInChildren<Text>().color = Color.white;
            gameObject.GetComponentInChildren<Toggle>().isOn = false;
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)ItemWindow.elementX, (float)ItemWindow.elementY, 0f);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2((float)width, 20f);
            gameObject.GetComponentInChildren<Text>().text = Text;
            gameObject.GetComponentInChildren<Toggle>().onValueChanged.AddListener(action);
            ItemWindow.elementX += width / 2 + 10;
            return gameObject;
        }

        // Token: 0x06000079 RID: 121 RVA: 0x00007E72 File Offset: 0x00006072
        private static void hr()
        {
            ItemWindow.elementX = ItemWindow.initialX;
            ItemWindow.elementY -= 60;
        }

        // Token: 0x0600007A RID: 122 RVA: 0x00007E8C File Offset: 0x0000608C
        public static void SpawnItem(Item item, int count)
        {
            bool flag = ItemWindow.type == 1;
            if (flag)
            {
                MiJiBaoDian miJiBaoDian = item as MiJiBaoDian;
                DataHelp.GetBag(DataManager.Instance.GameData.GameRuntimeData.Player.BasicData.PersonalBag, false).AddItemByItemId(miJiBaoDian.GetBaseID(), count);
            }
            else
            {
                bool flag2 = ItemWindow.type == 9;
                if (flag2)
                {
                    ScriptTrainer.Instance.Log("ZG:尝试通过武器ID获取武器");
                    WeaponEntity weaponEntity = DataManager.Instance.GameData.GameBasicData.WeaponEntities.Find((WeaponEntity o) => o.BaseID == item.ID);
                    bool flag3 = weaponEntity == null;
                    if (flag3)
                    {
                        ScriptTrainer.Instance.Log("ZG:尝试通过武器名称获取武器");
                        weaponEntity = DataManager.Instance.GameData.GameBasicData.WeaponEntities.Find((WeaponEntity o) => o.GetName() == item.Name.ValueInfo);
                        bool flag4 = weaponEntity == null;
                        if (flag4)
                        {
                            ScriptTrainer.Instance.Log("ZG:添加武器失败");
                            return;
                        }
                    }
                    DataHelp.GetBag(DataManager.Instance.GameData.GameRuntimeData.Player.BasicData.PersonalBag, false).AddItemByEntityId(weaponEntity.ID, count);
                }
                else
                {
                    DataHelp.GetBag(DataManager.Instance.GameData.GameRuntimeData.Player.BasicData.PersonalBag, false).AddItemByItemId(item.ID, count);
                }
            }
        }

        // Token: 0x0600007B RID: 123 RVA: 0x0000800C File Offset: 0x0000620C
        private static bool TryGetData()
        {
            bool flag = DataManager.Instance.GameData.GameRuntimeData.Player == null;
            return !flag;
        }

        #region[武器数据]
        private static List<WeaponEntity> weaponEntities= new List<WeaponEntity>();
        private static List<WeaponEntity> GetWeaponData()
        {
            List<WeaponEntity> ItemData = new List<WeaponEntity>();

            if (weaponEntities.IsNullOrEmpty())
            {
                foreach (Weapon item in DataManager.Instance.GameData.GameBasicData.Weapons)
                {
                    WeaponEntity obj = DataManager.Instance.GameData.GameBasicData.WeaponEntities.FirstOrDefault((WeaponEntity x) => x.BaseID == item.ID && x.ItemType == ItemEntityType.Template);
                    if (obj == null)
                        continue;

                    var weapon = obj.ProtobufDeepCopy();
                    weapon.ID = "Weapon_" + Util.GenerateUUID();
                    weapon.ItemType = ItemEntityType.Entity;
                    weapon.ZhiMingDu = new ValueName<float>();
                    //weapon.ZhiMingDu.Value = Util.RandomRange(obj.ConfigData.ZhiMingDu[0], obj.ConfigData.ZhiMingDu[1] + 1);
                    weapon.ZhiMingDu.Value = obj.ConfigData.ZhiMingDu[1];

                    if (weapon != null)
                    {
                        if (GetWeaponIcon(weapon) != null)
                        {
                            foreach (TianShuMingHao tianShuMingHao in DataManager.Instance.GameData.GameRuntimeData.TianShuMingHaos)
                            {
                                if (!tianShuMingHao.WeaponType.Contains(item.ItemType.Value))
                                    continue;
                                WeaponEntity tmpweapon = obj.ProtobufDeepCopy();
                                tmpweapon.ID = "Weapon_" + Util.GenerateUUID();
                                tmpweapon.ItemType = ItemEntityType.Entity;
                                tmpweapon.ZhiMingDu = new ValueName<float>();
                                //tmpweapon.ZhiMingDu.Value = Util.RandomRange(obj.ConfigData.ZhiMingDu[0], obj.ConfigData.ZhiMingDu[1] + 1);
                                tmpweapon.ZhiMingDu.Value = obj.ConfigData.ZhiMingDu[1];

                                tmpweapon.TianShuMingHao = new ValueName<string>();
                                List<string> list1 = new List<string>();
                                list1.Add(tianShuMingHao.ID);
                                tmpweapon.TianShuMingHao.RelatedID = list1.ToArray();

                                ItemData.Add(tmpweapon);
                                DataHelp.AddWuQiEntity(tmpweapon);
                            }
                            weapon.TianShuMingHao = new ValueName<string>();
                            ItemData.Add(weapon);
                            DataHelp.AddWuQiEntity(weapon);


                        }
                    }
                }
            }
            else
            {
                ItemData = weaponEntities;
            }
          

            bool flag = ItemWindow.searchText != "";
            if (flag)
            {
                ItemData = ItemWindow.FilterWeaponData(ItemData);
            }
            List<WeaponEntity> list = new List<WeaponEntity>();
            int num = (ItemWindow.page - 1) * ItemWindow.conunt;
            ScriptTrainer.Instance.Log(string.Format("ZG:页面起始Index:{0}", num));
            int num2 = num + ItemWindow.conunt;
            ScriptTrainer.Instance.Log(string.Format("ZG:页面结束Index:{0}", num2));

            for (int i = num; i < num2; i++)
            {
                bool flag2 = i < ItemData.Count;
                if (flag2)
                {
                    list.Add(ItemData[i]);
                }
            }
            bool flag3 = ItemData.Count % ItemWindow.conunt != 0;
            if (flag3)
            {
                ItemWindow.maxPage = ItemData.Count / ItemWindow.conunt + 1;
            }
            else
            {
                ItemWindow.maxPage = ItemData.Count / ItemWindow.conunt;
            }
            return list;
        }
        private static List<WeaponEntity> FilterWeaponData(List<WeaponEntity> dataList)
        {
            bool flag = ItemWindow.searchText == "";
            List<WeaponEntity> result;
            if (flag)
            {
                result = dataList;
            }
            else
            {
                List<WeaponEntity> list = new List<WeaponEntity>();
                foreach (WeaponEntity item in dataList)
                {
                    string itemName = ItemWindow.GetWeaponName(item);
                    string itemDescription = ItemWindow.GetWeaponDescription(item);
                    bool flag2 = itemName.Contains(ItemWindow.searchText.Replace(" ", "")) || itemDescription.Contains(ItemWindow.searchText.Replace(" ", ""));
                    if (flag2)
                    {
                        list.Add(item);
                    }
                }
                result = list;
            }
            return result;
        }
        private static string GetWeaponName(WeaponEntity item)
        {
            if (!item.TianShuMingHao.RelatedID.IsNullOrEmpty<string>())
            {
                TianShuMingHao tianShuMingHao = DataHelp.GetTianShuMingHao(item.TianShuMingHao.RelatedID[0]);
                string text = null;
                switch (tianShuMingHao.Quality.Value)
                {
                    case ProductQuality.White:
                        text = Config.Instance.JH_UI.Color.White;
                        break;
                    case ProductQuality.Green:
                        text = Config.Instance.JH_UI.Color.Green;
                        break;
                    case ProductQuality.Blue:
                        text = Config.Instance.JH_UI.Color.Blue;
                        break;
                    case ProductQuality.Gold:
                        text = Config.Instance.JH_UI.Color.Gold;
                        break;
                }
                string name = tianShuMingHao.Name;
                string format = text.Replace("{0}", "");
                return name + "-" + DataHelp.GetWuQi(item.BaseID).Name.ValueInfo + format;
            }
            return DataHelp.GetWuQi(item.BaseID).Name.ValueInfo;
        }
        private static string GetWeaponDescription(WeaponEntity item)
        {
            Weapon weapon = DataHelp.GetWuQi(item.BaseID);
            return weapon.ItemDescription.Value;
        }
        private static Sprite GetWeaponIcon(WeaponEntity item)
        {
            GameObject gameObject = new GameObject("ZG_Tmp");

            item.CreateIcon(gameObject.transform);

            Image componentInChildren = gameObject.GetComponentInChildren<Image>();
            bool flag = componentInChildren != null;
            Sprite result;
            if (flag)
            {
                result = componentInChildren.sprite;
            }
            else
            {
                result = null;
            }
            Destroy(gameObject);
            return result;
        }

        public static void SpawnWeapon(WeaponEntity item, int count)
        {
            DataHelp.GetBag(DataManager.Instance.GameData.GameRuntimeData.Player.BasicData.PersonalBag, false).AddItemByEntityId(item.ID, count);
        }
        #endregion

        #region[物品数据]
        private static List<Item> GetItemData()
        {
            List<Item> ItemData = new List<Item>();
            switch (ItemWindow.type)
            {
                case 0:
                    DataManager.Instance.GameData.GameBasicData.Herbs.ForEach(delegate (Herb x)
                    {
                        ItemData.Add(x);
                    });
                    break;
                case 1:
                    DataManager.Instance.GameData.GameBasicData.MiJiBaoDians.ForEach(delegate (MiJiBaoDian x)
                    {
                        ItemData.Add(x);
                    });
                    break;
                case 2:
                    DataManager.Instance.GameData.GameBasicData.Books.ForEach(delegate (Book x)
                    {
                        ItemData.Add(x);
                    });
                    break;
                case 3:
                    DataManager.Instance.GameData.GameBasicData.Foods.ForEach(delegate (Food x)
                    {
                        ItemData.Add(x);
                    });
                    break;
                case 4:
                    DataManager.Instance.GameData.GameBasicData.Wines.ForEach(delegate (Wine x)
                    {
                        ItemData.Add(x);
                    });
                    break;
                case 5:
                    DataManager.Instance.GameData.GameBasicData.Teas.ForEach(delegate (Tea x)
                    {
                        ItemData.Add(x);
                    });
                    break;
                case 6:
                    DataManager.Instance.GameData.GameBasicData.Arts.ForEach(delegate (Art x)
                    {
                        ItemData.Add(x);
                    });
                    break;
                case 7:
                    DataManager.Instance.GameData.GameBasicData.Poems.ForEach(delegate (Poem x)
                    {
                        ItemData.Add(x);
                    });
                    break;
                case 8:
                    DataManager.Instance.GameData.GameRuntimeData.AntiqueEntities.ForEach(delegate (AntiqueEntity x)
                    {
                        Antique antique = DataHelp.GetAntique(x.BaseID);
                        if(antique != null)
                            ItemData.Add(antique);
                    });
                    break;
                case 9:
                    foreach (WeaponEntity item in DataManager.Instance.GameData.GameRuntimeData.WeaponEntities)
                    {
                        Weapon wuqi = DataHelp.GetWuQi(item.BaseID);
                        if(wuqi != null)
                            ItemData.Add(wuqi);
                    }
                    break;
                case 10:
                    foreach (Music item2 in DataManager.Instance.GameData.GameBasicData.Musics)
                    {
                        ItemData.Add(item2);
                    }
                    break;
                default:
                    DataManager.Instance.GameData.GameBasicData.Herbs.ForEach(delegate (Herb x)
                    {
                        ItemData.Add(x);
                    });
                    break;
            }
            ScriptTrainer.Instance.Log(string.Format("ZG:全物品数量:{0}", ItemData.Count));
            bool flag = ItemWindow.searchText != "";
            if (flag)
            {
                ItemData = ItemWindow.FilterItemData(ItemData);
            }
            List<Item> list = new List<Item>();
            int num = (ItemWindow.page - 1) * ItemWindow.conunt;
            ScriptTrainer.Instance.Log(string.Format("ZG:页面起始Index:{0}", num));
            int num2 = num + ItemWindow.conunt;
            ScriptTrainer.Instance.Log(string.Format("ZG:页面结束Index:{0}", num2));
            for (int i = num; i < num2; i++)
            {
                bool flag2 = i < ItemData.Count;
                if (flag2)
                {
                    list.Add(ItemData[i]);
                }
            }
            bool flag3 = ItemData.Count % ItemWindow.conunt != 0;
            if (flag3)
            {
                ItemWindow.maxPage = ItemData.Count / ItemWindow.conunt + 1;
            }
            else
            {
                ItemWindow.maxPage = ItemData.Count / ItemWindow.conunt;
            }
            return list;
        }
        
        private static List<Item> FilterItemData(List<Item> dataList)
        {
            bool flag = ItemWindow.searchText == "";
            List<Item> result;
            if (flag)
            {
                result = dataList;
            }
            else
            {
                List<Item> list = new List<Item>();
                foreach (Item item in dataList)
                {
                    string itemName = ItemWindow.GetItemName(item);
                    string itemDescription = ItemWindow.GetItemDescription(item);
                    bool flag2 = itemName.Contains(ItemWindow.searchText.Replace(" ", "")) || itemDescription.Contains(ItemWindow.searchText.Replace(" ", ""));
                    if (flag2)
                    {
                        list.Add(item);
                    }
                }
                result = list;
            }
            return result;
        }

        private static string GetItemName(Item item)
        {
            bool flag = ItemWindow.type == 1;
            string result;
            if (flag)
            {
                MiJiBaoDian miJiBaoDian = item as MiJiBaoDian;
                result = miJiBaoDian.GetName();
            }
            else
            {
                result = item.Name.ValueInfo;
            }
            return result;
        }

        private static string GetItemDescription(Item item)
        {
            bool flag = ItemWindow.type == 1;
            string result;
            if (flag)
            {
                MiJiBaoDian miJiBaoDian = item as MiJiBaoDian;
                result = miJiBaoDian.GetName();
            }
            else
            {
                result = item.ItemDescription.Value;
            }
            return result;
        }

        private static Sprite GetItemIcon(Item item)
        {
            GameObject gameObject = Resources.Load<GameObject>(item.Prefabs.GetRandomItem<string>());
            Image componentInChildren = gameObject.GetComponentInChildren<Image>();
            bool flag = componentInChildren != null;
            Sprite result;
            if (flag)
            {
                result = componentInChildren.sprite;
            }
            else
            {
                result = null;
            }
            return result;
        }
        #endregion

        #region[游戏数据]
        private static List<IEntityItem> GetEntityItemData()
        {
            List<IEntityItem> ItemData = new List<IEntityItem>();
            switch (ItemWindow.type)
            {
                case 0:
                    DataManager.Instance.GameData.GameRuntimeData.HerbEntities.ForEach(delegate (HerbEntity x)
                    {
                        ItemData.Add(x);
                    });
                    break;
                case 1:
                    DataManager.Instance.GameData.GameBasicData.MiJiBaoDians.ForEach(delegate (MiJiBaoDian x)
                    {
                        ItemData.Add(x);
                    });
                    break;
                case 2:
                    DataManager.Instance.GameData.GameRuntimeData.BookEntities.ForEach(delegate (BookEntity x)
                    {
                        ItemData.Add(x);
                    });
                    break;
                case 3:
                    DataManager.Instance.GameData.GameRuntimeData.FoodEntities.ForEach(delegate (FoodEntity x)
                    {
                        if(x.GetItem()!= null)
                            ItemData.Add(x);
                    });
                    break;
                case 4:
                    DataManager.Instance.GameData.GameRuntimeData.WineEntities.ForEach(delegate (WineEntity x)
                    {
                        ItemData.Add(x);
                    });
                    break;
                case 5:
                    DataManager.Instance.GameData.GameRuntimeData.TeaEntities.ForEach(delegate (TeaEntity x)
                    {
                        ItemData.Add(x);
                    });
                    break;
                case 6:
                    DataManager.Instance.GameData.GameRuntimeData.ArtEntities.ForEach(delegate (ArtEntity x)
                    {
                        ItemData.Add(x);
                    });
                    break;
                case 7:
                    DataManager.Instance.GameData.GameRuntimeData.PoemEntities.ForEach(delegate (PoemEntity x)
                    {
                        //if (x.GetItem() is Poem)
                            ItemData.Add(x);
                    });
                    break;
                case 8:
                    DataManager.Instance.GameData.GameRuntimeData.AntiqueEntities.ForEach(delegate (AntiqueEntity x)
                    {
                        ItemData.Add(x);
                    });
                    break;
                case 9:
                    DataManager.Instance.GameData.GameRuntimeData.WeaponEntities.ForEach(delegate (WeaponEntity x)
                    {
                        ItemData.Add(x);
                    });
                    break;
                case 10:
                    DataManager.Instance.GameData.GameRuntimeData.MusicEntities.ForEach(delegate (MusicEntity x)
                    {
                        ItemData.Add(x);
                    });
                    break;
                default:
                    DataManager.Instance.GameData.GameRuntimeData.HerbEntities.ForEach(delegate (HerbEntity x)
                    {
                        ItemData.Add(x);
                    });
                    break;
            }
            ScriptTrainer.Instance.Log(string.Format("ZG:全物品数量:{0}", ItemData.Count));
            bool flag = ItemWindow.searchText != "";
            if (flag)
            {
                ItemData = ItemWindow.FilterEntityItemData(ItemData);
            }
            List<IEntityItem> list = new List<IEntityItem>();
            int num = (ItemWindow.page - 1) * ItemWindow.conunt;
            int num2 = num + ItemWindow.conunt;
            for (int i = num; i < num2; i++)
            {
                bool flag2 = i < ItemData.Count;
                if (flag2)
                {
                    list.Add(ItemData[i]);
                }
            }
            bool flag3 = ItemData.Count % ItemWindow.conunt != 0;
            if (flag3)
            {
                ItemWindow.maxPage = ItemData.Count / ItemWindow.conunt + 1;
            }
            else
            {
                ItemWindow.maxPage = ItemData.Count / ItemWindow.conunt;
            }
            return list;
        }

        private static List<IEntityItem> FilterEntityItemData(List<IEntityItem> dataList)
        {
            bool flag = ItemWindow.searchText == "";
            List<IEntityItem> result;
            if (flag)
            {
                result = dataList;
            }
            else
            {
                List<IEntityItem> list = new List<IEntityItem>();
                foreach (IEntityItem item in dataList)
                {
                    string itemName = ItemWindow.GetEntityItemName(item);
                    string itemDescription = ItemWindow.GetEntityItemDescription(item);
                    bool flag2 = itemName.Contains(ItemWindow.searchText.Replace(" ", "")) || itemDescription.Contains(ItemWindow.searchText.Replace(" ", ""));
                    if (flag2)
                    {
                        list.Add(item);
                    }
                }
                result = list;
            }
            return result;
        }

        private static string GetEntityItemName(IEntityItem item)
        {
            return item.GetName();
        }

        private static string GetEntityItemDescription(IEntityItem item)
        {
            return item.GetName();
        }

        private static Sprite GetEntityItemIcon(IEntityItem item)
        {
            GameObject gameObject = new GameObject("ZG_Tmp");

            item.CreateIcon(gameObject.transform);

            Image componentInChildren = gameObject.GetComponentInChildren<Image>();
            bool flag = componentInChildren != null;
            Sprite result;
            if (flag)
            {
                result = componentInChildren.sprite;
            }
            else
            {
                result = null;
            }
            Destroy(gameObject);
            return result;
        }
        public static void SpawnEntityItem(IEntityItem item, int count)
        {
            ItemEntity item2  = item as ItemEntity;

            if (item2 == null && type != 1)
                return;
            if (type == 1)
            {
                MiJiBaoDian miJiBaoDian = item as MiJiBaoDian;
                DataHelp.GetBag(DataManager.Instance.GameData.GameRuntimeData.Player.BasicData.PersonalBag, false).AddItemByItemId(miJiBaoDian.GetBaseID(), count);
            }
            else if(type == 7)
            {
                Poem item3 = DataHelp.GetPoem(item2.BaseID);
                if (item3 != null)
                {
                    //if (item3.ItemType.Value != PoemType.ShiCi || !item3.Conditions.IsNullOrEmpty<Condition>())
                        DataHelp.GetBag(DataManager.Instance.GameData.GameRuntimeData.Player.BasicData.PersonalBag, false).AddItemByEntityId(item2.ID, count);
                }
            }
            else
                DataHelp.GetBag(DataManager.Instance.GameData.GameRuntimeData.Player.BasicData.PersonalBag, false).AddItemByEntityId(item2.ID, count);

            if (type == 8)
            {
                Antique antique = DataHelp.GetAntique(item2.BaseID);
                if (antique.ItemType.Value > AntiqueType.Yan)
                    CollectSystem.Instance.PlayerJianBaoSuccess(item2.BaseID);
            }
            else if(type == 3)
            {
                Food food = DataHelp.GetFood(item2.BaseID);

                if (food.ItemType.Value == FoodType.CaiYao && !food.PeiFangs.IsNullOrEmpty<PeiFang>())
                {
                    CollectSystem.Instance.PlayerCookSuccess(item2.BaseID);
                    CollectSystem.Instance.PlayerGetCP(item2.BaseID);
                }
            }   
            else if (type == 6)
            {
                Art art = DataHelp.GetArt(item2.BaseID);
                if (!art.Conditions.IsNullOrEmpty())
                {
                    CollectSystem.Instance.PlayerXieZiZuoHuaSuccess(item2.BaseID);
                    CollectSystem.Instance.PlayerGetXieZiZuoHuaLingGan(item2.BaseID);
                }
            }
            else if (type == 7)
            {
                Poem item3 = DataHelp.GetPoem(item2.BaseID);
                if(item3 != null)
                {
                    if(!item3.ReadDisable && item3.ItemType.Value == PoemType.ShuJi)
                    {
                        CollectSystem.Instance.PlayerReadFinish(item2.BaseID);
                    }
                    else if(item3.ItemType.Value == PoemType.ShiCi && !item3.Conditions.IsNullOrEmpty<Condition>())
                    {
                        CollectSystem.Instance.PlayerFuShiSuccess(item2.BaseID);
                        CollectSystem.Instance.PlayerGetFuShiLingGan(item2.BaseID);
                    }
                    else
                    {
                        //if (!DataManager.Instance.GameData.GameRuntimeData.Player.BasicData.LearnFoods.Contains((PeiFang x) => x.Result == item2.BaseID))
                        //{
                        //    ScriptTrainer.Instance.Log(item2.BaseID);
                        //    ScriptTrainer.Instance.Log(item2.ID);
                        //    List<PeiFang> list = PeiFang.Create(item2.BaseID);
                        //    if (list.IsNullOrEmpty())
                        //        return;
                        //    PeiFang.PeiFangComparer comparer = new PeiFang.PeiFangComparer();
                        //    List<PeiFang> list2 = new List<PeiFang>();
                        //    using (List<PeiFang>.Enumerator enumerator4 = list.GetEnumerator())
                        //    {
                        //        while (enumerator4.MoveNext())
                        //        {
                        //            PeiFang pf = enumerator4.Current;
                        //            if (!DataManager.Instance.GameData.GameRuntimeData.Player.BasicData.LearnFoods.Contains((PeiFang x) => comparer.Equals(pf, x)))
                        //            {
                        //                list2.Add(pf);
                        //            }
                        //        }
                        //    }
                        //    DataManager.Instance.GameData.GameRuntimeData.Player.BasicData.LearnFoods.AddRange(list2);
                        //}
                    }
                }
                
            }

        }
        #endregion
        // Token: 0x0400003F RID: 63
        private static GameObject Panel;

        // Token: 0x04000040 RID: 64
        private static int initialX;

        // Token: 0x04000041 RID: 65
        private static int initialY;

        // Token: 0x04000042 RID: 66
        private static int elementX;

        // Token: 0x04000043 RID: 67
        private static int elementY;

        // Token: 0x04000044 RID: 68
        private static GameObject ItemPanel;

        // Token: 0x04000045 RID: 69
        private static List<GameObject> ItemButtons = new List<GameObject>();

        // Token: 0x04000046 RID: 70
        private static int type = 0;

        // Token: 0x04000047 RID: 71
        private static int page = 1;

        // Token: 0x04000048 RID: 72
        private static int maxPage = 1;

        // Token: 0x04000049 RID: 73
        private static int conunt = 15;

        // Token: 0x0400004A RID: 74
        private static string searchText = "";

        // Token: 0x0400004B RID: 75
        private static GameObject uiText;
    }
}
