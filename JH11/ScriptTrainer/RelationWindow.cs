using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityGameUI;
using XDPaint.Core.Materials;
using static UI_DWSign;
using Object = UnityEngine.Object;

namespace ScriptTrainer
{
    // Token: 0x0200000F RID: 15
    internal class RelationWindow : MonoBehaviour
    {
        // Token: 0x17000003 RID: 3
        // (get) Token: 0x0600006F RID: 111 RVA: 0x00007260 File Offset: 0x00005460
        private static string uiText_text
        {
            get
            {
                return string.Format("{0} / {1}", RelationWindow.page, RelationWindow.maxPage);
            }
        }

        // Token: 0x06000070 RID: 112 RVA: 0x00007290 File Offset: 0x00005490
        public RelationWindow(GameObject panel, int x, int y)
        {
            RelationWindow.Panel = panel;
            RelationWindow.initialX = (RelationWindow.elementX = x + 50);
            RelationWindow.elementY = y;
            RelationWindow.initialY = y;
            this.Initialize();
        }

        // Token: 0x06000071 RID: 113 RVA: 0x000072C4 File Offset: 0x000054C4
        public void Initialize()
        {
            this.SearchBar(RelationWindow.Panel);
            this.typeBar(RelationWindow.Panel);
            RelationWindow.elementX += 280;
            RelationWindow.hr();
            bool flag = RelationWindow.TryGetData();
            if (flag)
            {
                RelationWindow.container();
            }
            else
            {
                RelationWindow.elementY = -200;
            }
            this.PageBar(RelationWindow.Panel);
        }

        // Token: 0x06000072 RID: 114 RVA: 0x0000732C File Offset: 0x0000552C
        private void typeBar(GameObject panel)
        {
            RelationWindow.elementX += 350;
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject = UIControls.createUIText(panel, bgSprite, "#FFFFFFFF");
            gameObject.GetComponent<Text>().text = "分类";
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)RelationWindow.elementX, (float)RelationWindow.elementY, 0f);
            gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            RelationWindow.elementX += 60;
            List<string> options = new List<string>
            {
                "男",
                "女",
                "阉",
                "全",
            };
            Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            Sprite scrollbarSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));
            Sprite dropDownSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            Sprite checkmarkSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));
            Sprite customMaskSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#E65100FF"));
            Color labelColor = UIControls.HTMLString2Color("#EFEBE9FF");
            GameObject gameObject2 = UIControls.createUIDropDown(panel, bgSprite2, scrollbarSprite, dropDownSprite, checkmarkSprite, customMaskSprite, options, labelColor);
            //UnityEngine.Object.DontDestroyOnLoad(gameObject2);
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector3((float)RelationWindow.elementX, (float)RelationWindow.elementY, 0f);
            gameObject2.GetComponent<Dropdown>().onValueChanged.AddListener(delegate (int call)
            {
                RelationWindow.type = call;
                RelationWindow.page = 1;
                bool flag = RelationWindow.TryGetData();
                if (flag)
                {
                    RelationWindow.container();
                }
                RelationWindow.uiText.GetComponent<Text>().text = RelationWindow.uiText_text;
            });
        }

        // Token: 0x06000073 RID: 115 RVA: 0x00007514 File Offset: 0x00005714
        private void SearchBar(GameObject panel)
        {
            RelationWindow.elementY += 10;
            RelationWindow.elementX = -MainWindow.width / 2 + 120;
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject = UIControls.createUIText(panel, bgSprite, "#FFFFFFFF");
            gameObject.GetComponent<Text>().text = "搜索";
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)RelationWindow.elementX, (float)RelationWindow.elementY, 0f);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 30f);
            gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            RelationWindow.elementX += 60;
            int num = 260;
            Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject gameObject2 = UIControls.createUIInputField(panel, bgSprite2, "#FFFFFFFF");
            gameObject2.GetComponent<InputField>().text = RelationWindow.searchText;
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector3((float)(RelationWindow.elementX + 100), (float)RelationWindow.elementY, 0f);
            gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2((float)num, 30f);

            gameObject2.GetComponent<InputField>().onEndEdit.AddListener(delegate (string text)
            {
                RelationWindow.page = 1;
                RelationWindow.searchText = text;
                bool flag = RelationWindow.TryGetData();
                if (flag)
                {
                    RelationWindow.container();
                }
                RelationWindow.uiText.GetComponent<Text>().text = RelationWindow.uiText_text;
            });
        }

        // Token: 0x06000074 RID: 116 RVA: 0x0000766C File Offset: 0x0000586C
        private void PageBar(GameObject panel)
        {
            GameObject gameObject = UIControls.createUIPanel(panel, "40", "500", null);
            gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, (float)RelationWindow.elementY, 0f);
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));

            bool flag = RelationWindow.uiText == null;
            if (flag)
            {
                RelationWindow.uiText = UIControls.createUIText(gameObject, bgSprite, "#ffFFFFFF");
                RelationWindow.uiText.GetComponent<Text>().text = RelationWindow.uiText_text;
                RelationWindow.uiText.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
                RelationWindow.uiText.GetComponent<Text>().fontSize = 20;
                RelationWindow.uiText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            }
            string backgroundColor = "#8C9EFFFF";
            GameObject gameObject2 = UIControls.createUIButton(gameObject, backgroundColor, "上一页", delegate
            {
                RelationWindow.page--;
                bool flag2 = RelationWindow.page <= 0;
                if (flag2)
                {
                    RelationWindow.page = 1;
                }
                bool flag3 = RelationWindow.TryGetData();
                if (flag3)
                {
                    RelationWindow.container();
                }
                RelationWindow.uiText.GetComponent<Text>().text = RelationWindow.uiText_text;
            }, default(Vector3));
            gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 20f);
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector3(-100f, 0f, 0f);
            GameObject gameObject3 = UIControls.createUIButton(gameObject, backgroundColor, "下一页", delegate
            {
                RelationWindow.page++;
                bool flag2 = RelationWindow.page >= RelationWindow.maxPage;
                if (flag2)
                {
                    RelationWindow.page = RelationWindow.maxPage;
                }
                bool flag3 = RelationWindow.TryGetData();
                if (flag3)
                {
                    RelationWindow.container();
                }
                RelationWindow.uiText.GetComponent<Text>().text = RelationWindow.uiText_text;
            }, default(Vector3));
            gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 20f);
            gameObject3.GetComponent<RectTransform>().localPosition = new Vector3(100f, 0f, 0f);
        }

        // Token: 0x06000075 RID: 117 RVA: 0x00007844 File Offset: 0x00005A44
        private static void container()
        {
            RelationWindow.elementX = -205;
            RelationWindow.elementY = 85;
            foreach (GameObject obj in RelationWindow.ItemButtons)
            {
                UnityEngine.Object.Destroy(obj);
            }
            RelationWindow.ItemButtons.Clear();
            RelationWindow.ItemPanel = UIControls.createUIPanel(RelationWindow.Panel, "340", "630", null);
            RelationWindow.ItemPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            //RelationWindow.ItemPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#FFFFFFFF");
            RelationWindow.ItemPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -10f);
            ScriptTrainer.Instance.Log("ZG:添加物品");
            int num = 0;
            using (List<Character>.Enumerator enumerator2 = RelationWindow.GetItemData().GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    Character item = enumerator2.Current;
                    GameObject item2 = RelationWindow.CreateItemButton("修改关系", item , RelationWindow.ItemPanel, delegate ()
                    {
                        SpawnChangeDialog(item);
                    });
                    RelationWindow.ItemButtons.Add(item2);
                    num++;
                    bool flag = num % 3 == 0;
                    if (flag)
                    {
                        RelationWindow.hr();
                        RelationWindow.elementX = -205;
                    }
                }
            }
        }

        // Token: 0x06000076 RID: 118 RVA: 0x000079F0 File Offset: 0x00005BF0
        private static GameObject CreateItemButton(string ButtonText, Character item, GameObject panel, UnityAction action)
        {
            int num = 190;
            int num2 = 100;
            string htmlcolorstring = "#FFFFFFFF";
            GameObject gameObject = UIControls.createUIPanel(panel, num2.ToString(), num.ToString(), null);
            gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#455A64FF");
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)RelationWindow.elementX, (float)RelationWindow.elementY, 0f);
            GameObject gameObject2 = UIControls.createUIPanel(gameObject, num2.ToString(), "90", null);
            gameObject2.GetComponent<Image>().color = UIControls.HTMLString2Color(htmlcolorstring);
            gameObject2.GetComponent<RectTransform>().anchoredPosition = new Vector2(50f, 0f);

            GameObject gameObject3 = GetItemIcon(item, gameObject2);
            if(gameObject3 != null)
            {
                gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2(40f, 40f);
                gameObject3.GetComponent<RectTransform>().localPosition = new Vector2(0f, 0f);
            }

            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(gameObject, bgSprite, ColorUtility.ToHtmlStringRGBA(Color.white));
            uiText.GetComponent<Text>().text = GetItemName(item);
            uiText.GetComponent<RectTransform>().localPosition = new Vector2(-10f, 30f);

            GameObject uiText2 = UIControls.createUIText(gameObject, bgSprite, ColorUtility.ToHtmlStringRGBA(Color.white));
            uiText2.GetComponent<Text>().text = $"{GetPlaceName(item)}";
            uiText2.GetComponent<RectTransform>().localPosition = new Vector2(-10f, 10f);

            GameObject uiText3 = UIControls.createUIText(gameObject, bgSprite, ColorUtility.ToHtmlStringRGBA(Color.white));
            GameObject uiText4 = UIControls.createUIText(gameObject, bgSprite, ColorUtility.ToHtmlStringRGBA(Color.white));
            if (item.SocialData.SocialRelation.IsXiangShiPlayer)
            {
                uiText3.GetComponent<Text>().text = $"好感度：{(int)item.GetRelation()}";
                uiText4.GetComponent<Text>().text = $"亲密度：{(int)item.Getlove()}";
            }
            else
            {
                uiText3.GetComponent<Text>().text = $"未相识此人";
                uiText4.GetComponent<Text>().text = "";
            }
            uiText3.GetComponent<RectTransform>().localPosition = new Vector2(-10f, -10f);
            uiText4.GetComponent<RectTransform>().localPosition = new Vector2(-10f, -30f);
            string backgroundColor = "#8C9EFFFF";
            GameObject button = UIControls.createUIButton(gameObject, backgroundColor, ButtonText, action);
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(90f, 20f);
            button.GetComponent<RectTransform>().localPosition = new Vector2(-45f, -40f);
            RelationWindow.elementX += 200;
            return button;
        }
        public static void SpawnChangeDialog(Character character)
        {
            GameObject canvas = UIControls.createUICanvas();    // 创建画布
            Object.DontDestroyOnLoad(canvas);
            // 设置置顶显示
            canvas.GetComponent<Canvas>().overrideSorting = true;
            canvas.GetComponent<Canvas>().sortingOrder = 10001;

            GameObject uiPanel = UIControls.createUIPanel(canvas, "120", "350", null);  // 创建面板
            uiPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#37474FFF"); // 设置背景颜色

            //// 创建标题
            //Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            //GameObject uiText = UIControls.createUIText(uiPanel, txtBgSprite, "#FFFFFFFF");
            //uiText.GetComponent<Text>().text = prompt;
            //uiText.GetComponent<RectTransform>().localPosition = new Vector3(0, 10, 0);

            string backgroundColor = "#8C9EFFFF";
            GameObject button = UIControls.createUIButton(uiPanel, backgroundColor, "瞬移到人物身边", () =>
            {
                Place place = DataHelp.GetPlace(character.Location.PlaceID);
                if (place == null)
                {
                    Object.Destroy(canvas);
                }
                else
                {
                    PlaceWindow.Teleport(place);
                    Object.Destroy(canvas);
                }
            });
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(115f, 30f);
            button.GetComponent<RectTransform>().localPosition = new Vector2(0, 40);

            if (character.SocialData.SocialRelation.IsXiangShiPlayer)
            {
                GameObject button2 = UIControls.createUIButton(uiPanel, "#8C9EFFFF", "添加好感度", () =>
                {
                    SpawnInputDialog("添加多少好感度", "添加", "100", (string text) =>
                    {
                        character.SocialData.SocialRelation.RelationValue += text.ConvertToFloatDef(100);
                        ScriptTrainer.Instance.Log($"添加{text}好感度成功");
                    });
                    //Object.Destroy(canvas);
                });
                button2.GetComponent<RectTransform>().sizeDelta = new Vector2(115f, 30f);
                button2.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);

                GameObject button3 = UIControls.createUIButton(uiPanel, "#8C9EFFFF", "添加亲密度", () =>
                {
                    SpawnInputDialog("添加多少亲密度", "添加", "100", (string text) =>
                    {
                        character.SocialData.SocialRelation.LoveValue += text.ConvertToFloatDef(100);
                        ScriptTrainer.Instance.Log($"添加{text}亲密度成功");
                    });
                    //Object.Destroy(canvas);
                });
                button3.GetComponent<RectTransform>().sizeDelta = new Vector2(115f, 30f);
                button3.GetComponent<RectTransform>().localPosition = new Vector2(0, -40);
            }
            // 创建确定按钮
            

            // 创建关闭按钮
            GameObject closeButton = UIControls.createUIButton(uiPanel, "#B71C1CFF", "X", () =>
            {
                Object.Destroy(canvas);
            }, new Vector3(350 / 2 - 10, 120 / 2 - 10, 0));
            // 设置closeButton宽高
            closeButton.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            // 字体颜色为白色
            closeButton.GetComponentInChildren<Text>().color = UIControls.HTMLString2Color("#FFFFFFFF");
        }
        public static void SpawnInputDialog(string prompt, string title, string defaultText, Action<string> onFinish)
        {
            GameObject canvas = UIControls.createUICanvas();    // 创建画布
            Object.DontDestroyOnLoad(canvas);
            // 设置置顶显示
            canvas.GetComponent<Canvas>().overrideSorting = true;
            canvas.GetComponent<Canvas>().sortingOrder = 10002;

            GameObject uiPanel = UIControls.createUIPanel(canvas, "70", "300", null);  // 创建面板
            uiPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#37474FFF"); // 设置背景颜色

            // 创建标题
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(uiPanel, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<Text>().text = prompt;
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(0, 10, 0);

            // 创建输入框
            Sprite inputFieldSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject uiInputField = UIControls.createUIInputField(uiPanel, inputFieldSprite, "#FFFFFFFF");
            uiInputField.GetComponent<InputField>().text = defaultText;
            uiInputField.GetComponent<RectTransform>().localPosition = new Vector3(-50, -10, 0);

            // 创建确定按钮
            GameObject uiButton = UIControls.createUIButton(uiPanel, "#8C9EFFFF", title, () =>
            {
                onFinish(uiInputField.GetComponent<InputField>().text);
                Object.Destroy(canvas);
            }, new Vector3(100, -10, 0));

            // 创建关闭按钮
            GameObject closeButton = UIControls.createUIButton(uiPanel, "#B71C1CFF", "X", () =>
            {
                Object.Destroy(canvas);
            }, new Vector3(350 / 2 - 10, 70 / 2 - 10, 0));
            // 设置closeButton宽高
            closeButton.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            // 字体颜色为白色
            closeButton.GetComponentInChildren<Text>().color = UIControls.HTMLString2Color("#FFFFFFFF");
        }

        // Token: 0x06000078 RID: 120 RVA: 0x00007D98 File Offset: 0x00005F98
        public static GameObject AddToggle(string Text, int width, GameObject panel, UnityAction<bool> action)
        {
            RelationWindow.elementX += width / 2 - 30;
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture(ColorUtility.ToHtmlStringRGBA(Color.white)));
            Sprite customCheckmarkSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#18FFFFFF"));
            GameObject gameObject = UIControls.createUIToggle(panel, bgSprite, customCheckmarkSprite);
            gameObject.GetComponentInChildren<Text>().color = Color.white;
            gameObject.GetComponentInChildren<Toggle>().isOn = false;
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)RelationWindow.elementX, (float)RelationWindow.elementY, 0f);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2((float)width, 20f);
            gameObject.GetComponentInChildren<Text>().text = Text;
            gameObject.GetComponentInChildren<Toggle>().onValueChanged.AddListener(action);
            RelationWindow.elementX += width / 2 + 10;
            return gameObject;
        }

        // Token: 0x06000079 RID: 121 RVA: 0x00007E72 File Offset: 0x00006072
        private static void hr()
        {
            RelationWindow.elementX = RelationWindow.initialX;
            RelationWindow.elementY -= 105;
        }

        // Token: 0x0600007A RID: 122 RVA: 0x00007E8C File Offset: 0x0000608C
        public static void SpawnItem(Item item, int count)
        {
            bool flag = RelationWindow.type == 1;
            if (flag)
            {
                MiJiBaoDian miJiBaoDian = item as MiJiBaoDian;
                DataHelp.GetBag(DataManager.Instance.GameData.GameRuntimeData.Player.BasicData.PersonalBag, false).AddItemByItemId(miJiBaoDian.GetBaseID(), count);
            }
            else
            {
                bool flag2 = RelationWindow.type == 9;
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

        // Token: 0x0600007C RID: 124 RVA: 0x00008040 File Offset: 0x00006240
        private static List<Character> GetItemData()
        {
            List<Character> ItemData = new List<Character>();
            var data = DataManager.Instance.GameData.GameRuntimeData.Characters;
            switch (RelationWindow.type)
            {
                case 0:
                    foreach(Character character in data)
                    {
                        if(character.BasicData.Gender.Value == Gender.Nan)
                        {
                            ItemData.Add(character);
                        }
                    }
                    break;
                case 1:
                    foreach (Character character in data)
                    {
                        if (character.BasicData.Gender.Value == Gender.Nv)
                        {
                            ItemData.Add(character);
                        }
                    }
                    break;
                case 2:
                    foreach (Character character in data)
                    {
                        if (character.BasicData.Gender.Value == Gender.Yan)
                        {
                            ItemData.Add(character);
                        }
                    }
                    break;
                case 3:
                    foreach (Character character in data)
                    {
                        ItemData.Add(character);
                    }
                    break;
                default:
                    foreach (Character character in data)
                    {
                        ItemData.Add(character);
                    }
                    break;
            }
            ScriptTrainer.Instance.Log(string.Format("ZG:全物品数量:{0}", ItemData.Count));
            bool flag = RelationWindow.searchText != "";
            if (flag)
            {
                ItemData = RelationWindow.FilterItemData(ItemData);
            }
            List<Character> list = new List<Character>();
            int num = (RelationWindow.page - 1) * RelationWindow.conunt;
            int num2 = num + RelationWindow.conunt;
            for (int i = num; i < num2; i++)
            {
                bool flag2 = i < ItemData.Count;
                if (flag2)
                {
                    list.Add(ItemData[i]);
                }
            }
            bool flag3 = ItemData.Count % RelationWindow.conunt != 0;
            if (flag3)
            {
                RelationWindow.maxPage = ItemData.Count / RelationWindow.conunt + 1;
            }
            else
            {
                RelationWindow.maxPage = ItemData.Count / RelationWindow.conunt;
            }
            return list;
        }

        // Token: 0x0600007D RID: 125 RVA: 0x00008450 File Offset: 0x00006650
        private static List<Character> FilterItemData(List<Character> dataList)
        {
            bool flag = RelationWindow.searchText == "";
            List<Character> result;
            if (flag)
            {
                result = dataList;
            }
            else
            {
                List<Character> list = new List<Character>();
                foreach (Character item in dataList)
                {
                    string itemName = RelationWindow.GetItemName(item);
                    string itemDescription = RelationWindow.GetItemDescription(item);
                    bool flag2 = itemName.Contains(RelationWindow.searchText.Replace(" ", "")) || itemDescription.Contains(RelationWindow.searchText.Replace(" ", ""));
                    if (flag2)
                    {
                        list.Add(item);
                    }
                }
                result = list;
            }
            return result;
        }

        // Token: 0x0600007E RID: 126 RVA: 0x00008524 File Offset: 0x00006724
        private static string GetItemName(Character item)
        {
            return item.BasicData.Name.Value;
        }

        // Token: 0x0600007F RID: 127 RVA: 0x00008560 File Offset: 0x00006760
        private static string GetItemDescription(Character item)
        {
            return item.BasicData.Name.Value;
        }
        private static string GetPlaceName(Character item)
        {
            Place place = DataHelp.GetPlace(item.Location.PlaceID);
            if (place == null)
                return "未获取到地点";
            else
                return place.BasicData.Name.ValueInfo;
        }
        // Token: 0x06000080 RID: 128 RVA: 0x0000859C File Offset: 0x0000679C
        private static GameObject GetItemIcon(Character item, GameObject panel)
        {
            GameObject tmp = AvatarPartChange.CreteAvatar2D(UnityEngine.Object.Instantiate<GameObject>(Resources.Load(DataHelp.GetAvatar2D(item.BattleData.Avatar.Last<string>(), false).PortratAvatarPath) as GameObject, panel.transform), item);

            return tmp;
        }
        private static Sprite GetItemIcon(Character item)
        {
            GameObject tmpp = new GameObject("ZG_Tmp");
            GameObject tmp = AvatarPartChange.CreteAvatar2D(UnityEngine.Object.Instantiate<GameObject>(Resources.Load(DataHelp.GetAvatar2D(item.BattleData.Avatar.Last<string>(), false).PortratAvatarPath) as GameObject, tmpp.transform), item);

            Image image = tmp.GetComponentInChildren<Image>();
            if (image != null)
            {
                ScriptTrainer.Instance.Log("获取图片成功");
                Destroy(tmp);
                Destroy(tmpp);
                return image.sprite;
            }
            Destroy(tmp);
            Destroy(tmpp);
            return null;
        }
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
        private static int conunt = 9;

        // Token: 0x0400004A RID: 74
        private static string searchText = "";

        // Token: 0x0400004B RID: 75
        private static GameObject uiText;
    }
}
