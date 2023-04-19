using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityGameUI;

namespace ScriptTrainer
{
    // Token: 0x0200000D RID: 13
    internal class PlaceWindow : MonoBehaviour
    {
        // Token: 0x17000002 RID: 2
        // (get) Token: 0x0600005A RID: 90 RVA: 0x00005FB4 File Offset: 0x000041B4
        private static string uiText_text
        {
            get
            {
                return string.Format("{0} / {1}", PlaceWindow.page, PlaceWindow.maxPage);
            }
        }

        // Token: 0x0600005B RID: 91 RVA: 0x00005FE4 File Offset: 0x000041E4
        public PlaceWindow(GameObject panel, int x, int y)
        {
            PlaceWindow.Panel = panel;
            PlaceWindow.initialX = (PlaceWindow.elementX = x + 50);
            PlaceWindow.elementY = y;
            PlaceWindow.initialY = y;
            this.Initialize();
        }

        // Token: 0x0600005C RID: 92 RVA: 0x00006018 File Offset: 0x00004218
        public void Initialize()
        {
            this.SearchBar(PlaceWindow.Panel);
            this.typeBar(PlaceWindow.Panel);
            PlaceWindow.elementX += 280;
            PlaceWindow.hr();
            bool flag = PlaceWindow.TryGetData();
            if (flag)
            {
                PlaceWindow.container();
            }
            else
            {
                PlaceWindow.elementY = -175;
            }
            this.PageBar(PlaceWindow.Panel);
        }

        // Token: 0x0600005D RID: 93 RVA: 0x00006080 File Offset: 0x00004280
        private void typeBar(GameObject panel)
        {
            PlaceWindow.elementX += 350;
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject = UIControls.createUIText(panel, bgSprite, "#FFFFFFFF");
            gameObject.GetComponent<Text>().text = "分类";
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)PlaceWindow.elementX, (float)PlaceWindow.elementY, 0f);
            gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            PlaceWindow.elementX += 60;
            List<string> options = new List<string>
            {
                "门派",
                "商户",
                "贼寇"
            };
            Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            Sprite scrollbarSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));
            Sprite dropDownSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            Sprite checkmarkSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#8C9EFFFF"));
            Sprite customMaskSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#E65100FF"));
            Color labelColor = UIControls.HTMLString2Color("#EFEBE9FF");
            GameObject gameObject2 = UIControls.createUIDropDown(panel, bgSprite2, scrollbarSprite, dropDownSprite, checkmarkSprite, customMaskSprite, options, labelColor);
            //UnityEngine.Object.DontDestroyOnLoad(gameObject2);
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector3((float)PlaceWindow.elementX, (float)PlaceWindow.elementY, 0f);
            gameObject2.GetComponent<Dropdown>().onValueChanged.AddListener(delegate (int call)
            {
                PlaceWindow.type = call;
                PlaceWindow.page = 1;
                bool flag = PlaceWindow.TryGetData();
                if (flag)
                {
                    PlaceWindow.container();
                }
                PlaceWindow.uiText.GetComponent<Text>().text = PlaceWindow.uiText_text;
            });
        }

        // Token: 0x0600005E RID: 94 RVA: 0x00006214 File Offset: 0x00004414
        private void SearchBar(GameObject panel)
        {
            PlaceWindow.elementY += 10;
            PlaceWindow.elementX = -MainWindow.width / 2 + 120;
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject = UIControls.createUIText(panel, bgSprite, "#FFFFFFFF");
            gameObject.GetComponent<Text>().text = "搜索";
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)PlaceWindow.elementX, (float)PlaceWindow.elementY, 0f);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 30f);
            gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            PlaceWindow.elementX += 60;
            int num = 260;
            Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject gameObject2 = UIControls.createUIInputField(panel, bgSprite2, "#FFFFFFFF");
            gameObject2.GetComponent<InputField>().text = PlaceWindow.searchText;
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector3((float)(PlaceWindow.elementX + 100), (float)PlaceWindow.elementY, 0f);
            gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2((float)num, 30f);
            gameObject2.GetComponent<InputField>().onEndEdit.AddListener(delegate (string text)
            {
                PlaceWindow.page = 1;
                PlaceWindow.searchText = text;
                bool flag = PlaceWindow.TryGetData();
                if (flag)
                {
                    PlaceWindow.container();
                }
                PlaceWindow.uiText.GetComponent<Text>().text = PlaceWindow.uiText_text;
            });
        }

        // Token: 0x0600005F RID: 95 RVA: 0x0000636C File Offset: 0x0000456C
        private void PageBar(GameObject panel)
        {
            GameObject gameObject = UIControls.createUIPanel(panel, "40", "500", null);
            gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, (float)PlaceWindow.elementY, 0f);
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            bool flag = PlaceWindow.uiText == null;
            if (flag)
            {
                PlaceWindow.uiText = UIControls.createUIText(gameObject, bgSprite, "#ffFFFFFF");
                PlaceWindow.uiText.GetComponent<Text>().text = PlaceWindow.uiText_text;
                PlaceWindow.uiText.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
                PlaceWindow.uiText.GetComponent<Text>().fontSize = 20;
                PlaceWindow.uiText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            }
            string backgroundColor = "#8C9EFFFF";
            GameObject gameObject2 = UIControls.createUIButton(gameObject, backgroundColor, "上一页", delegate
            {
                PlaceWindow.page--;
                bool flag2 = PlaceWindow.page <= 0;
                if (flag2)
                {
                    PlaceWindow.page = 1;
                }
                bool flag3 = PlaceWindow.TryGetData();
                if (flag3)
                {
                    PlaceWindow.container();
                }
                PlaceWindow.uiText.GetComponent<Text>().text = PlaceWindow.uiText_text;
            }, default(Vector3));
            gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 20f);
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector3(-100f, 0f, 0f);
            GameObject gameObject3 = UIControls.createUIButton(gameObject, backgroundColor, "下一页", delegate
            {
                PlaceWindow.page++;
                bool flag2 = PlaceWindow.page >= PlaceWindow.maxPage;
                if (flag2)
                {
                    PlaceWindow.page = PlaceWindow.maxPage;
                }
                bool flag3 = PlaceWindow.TryGetData();
                if (flag3)
                {
                    PlaceWindow.container();
                }
                PlaceWindow.uiText.GetComponent<Text>().text = PlaceWindow.uiText_text;
            }, default(Vector3));
            gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 20f);
            gameObject3.GetComponent<RectTransform>().localPosition = new Vector3(100f, 0f, 0f);
        }

        // Token: 0x06000060 RID: 96 RVA: 0x00006544 File Offset: 0x00004744
        private static void container()
        {
            PlaceWindow.elementX = -200;
            PlaceWindow.elementY = 125;
            foreach (GameObject obj in PlaceWindow.ItemButtons)
            {
                UnityEngine.Object.Destroy(obj);
            }
            PlaceWindow.ItemButtons.Clear();
            PlaceWindow.ItemPanel = UIControls.createUIPanel(PlaceWindow.Panel, "300", "600", null);
            PlaceWindow.ItemPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            PlaceWindow.ItemPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(10f, 0f);
            ScriptTrainer.Instance.Log("ZG:添加物品");
            int num = 0;
            bool flag = PlaceWindow.type != 3;
            if (flag)
            {
                using (List<Place>.Enumerator enumerator2 = PlaceWindow.GetItemData().GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        Place item = enumerator2.Current;
                        GameObject item3 = PlaceWindow.CreateItemButton("获得", item, PlaceWindow.ItemPanel, delegate
                        {
                            string prompt = "添加" + PlaceWindow.GetItemName(item) + "地区多少好感";
                            string title = "添加";
                            string relation = string.Format("当前好感:{0}", item.GetPlaceRelationValue());
                            string defaultText = "1000";
                            PlaceWindow.SpawnInputDialog(prompt, title, relation, defaultText, (string text) =>
                            {
                                PlaceWindow.SpawnItem(item, text.ConvertToIntDef(1000));
                            });
                        });
                        PlaceWindow.ItemButtons.Add(item3);
                        num++;
                        bool flag2 = num % 3 == 0;
                        if (flag2)
                        {
                            PlaceWindow.hr();
                        }
                    }
                }
            }
            else
            {
                using (List<Place>.Enumerator enumerator3 = PlaceWindow.GetItemData().GetEnumerator())
                {
                    while (enumerator3.MoveNext())
                    {
                        Place item = enumerator3.Current;
                        GameObject item2 = PlaceWindow.CreateItemButton("瞬移", item, PlaceWindow.ItemPanel, delegate
                        {
                            PlaceWindow.Teleport(item);
                        });
                        PlaceWindow.ItemButtons.Add(item2);
                        num++;
                        bool flag3 = num % 3 == 0;
                        if (flag3)
                        {
                            PlaceWindow.hr();
                        }
                    }
                }
            }
        }

        // Token: 0x06000061 RID: 97 RVA: 0x00006768 File Offset: 0x00004968
        private static GameObject CreateItemButton(string ButtonText, Place place, GameObject panel, UnityAction action)
        {
            int num = 190;
            int num2 = 50;
            string htmlcolorstring = "#FFFFFFFF";
            GameObject gameObject = UIControls.createUIPanel(panel, num2.ToString(), num.ToString(), null);
            gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#455A64FF");
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)PlaceWindow.elementX, (float)PlaceWindow.elementY, 0f);
            GameObject gameObject2 = UIControls.createUIPanel(gameObject, num2.ToString(), "50", null);
            gameObject2.GetComponent<Image>().color = UIControls.HTMLString2Color(htmlcolorstring);
            gameObject2.GetComponent<RectTransform>().anchoredPosition = new Vector2(70f, 0f);
            Sprite itemIcon = PlaceWindow.GetItemIcon(place);
            bool flag = itemIcon != null;
            if (flag)
            {
                GameObject gameObject3 = UIControls.createUIImage(gameObject2, itemIcon);
                gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 60f);
                gameObject3.GetComponent<RectTransform>().localPosition = new Vector2(0f, 0f);
            }
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject4 = UIControls.createUIText(gameObject, bgSprite, ColorUtility.ToHtmlStringRGBA(Color.white));
            gameObject4.GetComponent<Text>().text = PlaceWindow.GetItemName(place);
            gameObject4.GetComponent<RectTransform>().localPosition = new Vector3(0f, 5f, 0f);
            string backgroundColor = "#8C9EFFFF";
            GameObject gameObject5 = UIControls.createUIButton(gameObject, backgroundColor, ButtonText, action, default(Vector3));
            gameObject5.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 20f);
            gameObject5.GetComponent<RectTransform>().localPosition = new Vector3(-50f, -10f, 0f);
            PlaceWindow.elementX += 200;
            return gameObject;
        }

        // Token: 0x06000062 RID: 98 RVA: 0x0000694C File Offset: 0x00004B4C
        private static void SpawnInputDialog(string prompt, string title, string relation, string defaultText, Action<string> onFinish)
        {
            GameObject canvas = UIControls.createUICanvas(ScriptTrainer.WindowSizeFactor.Value);
            UnityEngine.Object.DontDestroyOnLoad(canvas);
            canvas.GetComponent<Canvas>().overrideSorting = true;
            canvas.GetComponent<Canvas>().sortingOrder = 10001;
            GameObject gameObject = UIControls.createUIPanel(canvas, "100", "300", null);
            gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#37474FFF");
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject2 = UIControls.createUIText(gameObject, bgSprite, "#FFFFFFFF");
            gameObject2.GetComponent<Text>().text = prompt;
            gameObject2.GetComponent<RectTransform>().anchoredPosition = new Vector3(-50f, 30f, 0f);
            GameObject gameObject3 = UIControls.createUIText(gameObject, bgSprite, "#FFFFFFFF");
            gameObject3.GetComponent<Text>().text = relation;
            gameObject3.GetComponent<RectTransform>().anchoredPosition = new Vector3(-50f, 0f, 0f);
            Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject uiInputField = UIControls.createUIInputField(gameObject, bgSprite2, "#FFFFFFFF");
            uiInputField.GetComponent<InputField>().text = defaultText;
            uiInputField.GetComponent<RectTransform>().localPosition = new Vector3(-50f, -30f, 0f);
            GameObject gameObject4 = UIControls.createUIButton(gameObject, "#8C9EFFFF", title, delegate
            {
                onFinish(uiInputField.GetComponent<InputField>().text);
                UnityEngine.Object.Destroy(canvas);
            }, new Vector3(100f, -10f, 0f));
            GameObject gameObject5 = UIControls.createUIButton(gameObject, "#B71C1CFF", "X", delegate
            {
                UnityEngine.Object.Destroy(canvas);
            }, new Vector3(165f, 40f, 0f));
            gameObject5.GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 20f);
            gameObject5.GetComponentInChildren<Text>().color = UIControls.HTMLString2Color("#FFFFFFFF");
        }

        // Token: 0x06000063 RID: 99 RVA: 0x00006B60 File Offset: 0x00004D60
        public static GameObject AddToggle(string Text, int width, GameObject panel, UnityAction<bool> action)
        {
            PlaceWindow.elementX += width / 2 - 30;
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture(ColorUtility.ToHtmlStringRGBA(Color.white)));
            Sprite customCheckmarkSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#18FFFFFF"));
            GameObject gameObject = UIControls.createUIToggle(panel, bgSprite, customCheckmarkSprite);
            gameObject.GetComponentInChildren<Text>().color = Color.white;
            gameObject.GetComponentInChildren<Toggle>().isOn = false;
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)PlaceWindow.elementX, (float)PlaceWindow.elementY, 0f);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2((float)width, 20f);
            gameObject.GetComponentInChildren<Text>().text = Text;
            gameObject.GetComponentInChildren<Toggle>().onValueChanged.AddListener(action);
            PlaceWindow.elementX += width / 2 + 10;
            return gameObject;
        }

        // Token: 0x06000064 RID: 100 RVA: 0x00006C3A File Offset: 0x00004E3A
        private static void hr()
        {
            PlaceWindow.elementX = PlaceWindow.initialX;
            PlaceWindow.elementY -= 60;
        }

        // Token: 0x06000065 RID: 101 RVA: 0x00006C54 File Offset: 0x00004E54
        public static void SpawnItem(Place item, int count)
        {
            item.ChangePlaceRelationValue((float)count);
        }

        // Token: 0x06000066 RID: 102 RVA: 0x00006C60 File Offset: 0x00004E60
        public static void Teleport(Place item)
        {
            bool flag = Map2DUIManager.Map2DRunning();
            if (flag)
            {
                Map2DUIManager map2DUIManager = UnityEngine.Object.FindObjectOfType<Map2DUIManager>();
                bool flag2 = map2DUIManager;
                if (flag2)
                {
                    map2DUIManager.Return3DSceneNoFade();
                }
            }
            if(item.BasicData.PlaceType.Value == PlaceType.ZiZhai)            
            {
                var zizhai = DataHelp.GetPlace(item.ID, false);
                DataManager.Instance.GameData.GameRuntimeData.Player.Location.ZiZhaiID = item.ID;
                DataManager.Instance.GameData.GameRuntimeData.Player.Location.ZiZhaiSheShiID = null;
                foreach (var sheshi in zizhai.SheShis)
                {
                    if (!sheshi.Lock)
                    {
                        DataManager.Instance.GameData.GameRuntimeData.Player.Location.ZiZhaiSheShiID = sheshi.ID;
                        break;
                    }
                }
                DataManager.Instance.GameData.GameRuntimeData.Player.Location.PlaceID = item.ID;
                DataManager.Instance.GameData.GameRuntimeData.Player.Location.SheShiID = null;
            }
            else
            {
                DataManager.Instance.GameData.GameRuntimeData.Player.Location.ZiZhaiID = null;
                DataManager.Instance.GameData.GameRuntimeData.Player.Location.ZiZhaiSheShiID = null;
                DataManager.Instance.GameData.GameRuntimeData.Player.Location.PlaceID = item.ID;
                DataManager.Instance.GameData.GameRuntimeData.Player.Location.SheShiID = null;
            }
            QuestManager.Instance.TanFangCountAdd(item.ID);
            LoadingSceneManager.JumpScene(LoadingSceneManager.PlaceScene, 0f);
        }

        // Token: 0x06000067 RID: 103 RVA: 0x00006D34 File Offset: 0x00004F34
        private static bool TryGetData()
        {
            bool flag = DataManager.Instance.GameData.GameRuntimeData.Player == null;
            return !flag;
        }

        // Token: 0x06000068 RID: 104 RVA: 0x00006D68 File Offset: 0x00004F68
        private static List<Place> GetItemData()
        {
            List<Place> list = new List<Place>();
            switch (PlaceWindow.type)
            {
                case 0:
                    foreach (Place place in DataManager.Instance.GameData.GameRuntimeData.Places)
                    {
                        bool flag = place.BasicData.PlaceType.Value == PlaceType.MenPai;
                        if (flag)
                        {
                            list.Add(place);
                        }
                    }
                    break;
                case 1:
                    foreach (Place place2 in DataManager.Instance.GameData.GameRuntimeData.Places)
                    {
                        bool flag2 = place2.BasicData.PlaceType.Value == PlaceType.ShangHu;
                        if (flag2)
                        {
                            list.Add(place2);
                        }
                    }
                    break;
                case 2:
                    foreach (Place place3 in DataManager.Instance.GameData.GameRuntimeData.Places)
                    {
                        bool flag3 = place3.BasicData.PlaceType.Value == PlaceType.ZeiKou;
                        if (flag3)
                        {
                            list.Add(place3);
                        }
                    }
                    break;
                case 3:
                    foreach (Place item in DataManager.Instance.GameData.GameRuntimeData.Places)
                    {
                        list.Add(item);
                    }
                    break;
                default:
                    foreach (Place item2 in DataManager.Instance.GameData.GameRuntimeData.Places)
                    {
                        list.Add(item2);
                    }
                    break;
            }
            ScriptTrainer.Instance.Log(string.Format("ZG:全物品数量:{0}", list.Count));
            bool flag4 = PlaceWindow.searchText != "";
            if (flag4)
            {
                list = PlaceWindow.FilterItemData(list);
            }
            List<Place> list2 = new List<Place>();
            int num = (PlaceWindow.page - 1) * PlaceWindow.conunt;
            ScriptTrainer.Instance.Log(string.Format("ZG:页面起始Index:{0}", num));
            int num2 = num + PlaceWindow.conunt;
            ScriptTrainer.Instance.Log(string.Format("ZG:页面结束Index:{0}", num2));
            for (int i = num; i < num2; i++)
            {
                bool flag5 = i < list.Count;
                if (flag5)
                {
                    list2.Add(list[i]);
                }
            }
            bool flag6 = list.Count % PlaceWindow.conunt != 0;
            if (flag6)
            {
                PlaceWindow.maxPage = list.Count / PlaceWindow.conunt + 1;
            }
            else
            {
                PlaceWindow.maxPage = list.Count / PlaceWindow.conunt;
            }
            return list2;
        }

        // Token: 0x06000069 RID: 105 RVA: 0x000070D0 File Offset: 0x000052D0
        private static List<Place> FilterItemData(List<Place> dataList)
        {
            bool flag = PlaceWindow.searchText == "";
            List<Place> result;
            if (flag)
            {
                result = dataList;
            }
            else
            {
                List<Place> list = new List<Place>();
                foreach (Place item in dataList)
                {
                    string itemName = PlaceWindow.GetItemName(item);
                    string itemDescription = PlaceWindow.GetItemDescription(item);
                    bool flag2 = itemName.Contains(PlaceWindow.searchText.Replace(" ", "")) || itemDescription.Contains(PlaceWindow.searchText.Replace(" ", ""));
                    if (flag2)
                    {
                        list.Add(item);
                    }
                }
                result = list;
            }
            return result;
        }

        // Token: 0x0600006A RID: 106 RVA: 0x000071A4 File Offset: 0x000053A4
        private static string GetItemName(Place item)
        {
            return item.BasicData.Name.ValueInfo;
        }

        // Token: 0x0600006B RID: 107 RVA: 0x000071C8 File Offset: 0x000053C8
        private static string GetItemDescription(Place item)
        {
            return item.BasicData.Name.ValueInfo;
        }

        // Token: 0x0600006C RID: 108 RVA: 0x000071EC File Offset: 0x000053EC
        private static Sprite GetItemIcon(Place item)
        {
            return Resources.Load<Sprite>(item.IconPath);
        }

        // Token: 0x0400002E RID: 46
        private static GameObject Panel;

        // Token: 0x0400002F RID: 47
        private static int initialX;

        // Token: 0x04000030 RID: 48
        private static int initialY;

        // Token: 0x04000031 RID: 49
        private static int elementX;

        // Token: 0x04000032 RID: 50
        private static int elementY;

        // Token: 0x04000033 RID: 51
        private static GameObject ItemPanel;

        // Token: 0x04000034 RID: 52
        private static List<GameObject> ItemButtons = new List<GameObject>();

        // Token: 0x04000035 RID: 53
        private static int type = 0;

        // Token: 0x04000036 RID: 54
        private static int page = 1;

        // Token: 0x04000037 RID: 55
        private static int maxPage = 1;

        // Token: 0x04000038 RID: 56
        private static int conunt = 15;

        // Token: 0x04000039 RID: 57
        private static string searchText = "";

        // Token: 0x0400003A RID: 58
        private static GameObject uiText;
    }
}
