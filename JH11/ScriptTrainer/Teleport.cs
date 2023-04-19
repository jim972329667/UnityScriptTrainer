using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityGameUI;

namespace ScriptTrainer
{
    // Token: 0x0200000D RID: 13
    internal class TeleportWindow : MonoBehaviour
    {
        // Token: 0x17000002 RID: 2
        // (get) Token: 0x0600005A RID: 90 RVA: 0x00005FB4 File Offset: 0x000041B4
        private static string uiText_text
        {
            get
            {
                return string.Format("{0} / {1}", TeleportWindow.page, TeleportWindow.maxPage);
            }
        }

        // Token: 0x0600005B RID: 91 RVA: 0x00005FE4 File Offset: 0x000041E4
        public TeleportWindow(GameObject panel, int x, int y)
        {
            TeleportWindow.Panel = panel;
            TeleportWindow.initialX = (TeleportWindow.elementX = x + 50);
            TeleportWindow.elementY = y;
            TeleportWindow.initialY = y;
            this.Initialize();
        }

        // Token: 0x0600005C RID: 92 RVA: 0x00006018 File Offset: 0x00004218
        public void Initialize()
        {
            this.SearchBar(TeleportWindow.Panel);
            TeleportWindow.elementX += 280;
            TeleportWindow.hr();
            bool flag = TeleportWindow.TryGetData();
            if (flag)
            {
                TeleportWindow.container();
            }
            else
            {
                TeleportWindow.elementY = -175;
            }
            this.PageBar(TeleportWindow.Panel);
        }

        // Token: 0x0600005E RID: 94 RVA: 0x00006214 File Offset: 0x00004414
        private void SearchBar(GameObject panel)
        {
            TeleportWindow.elementY += 10;
            TeleportWindow.elementX = -MainWindow.width / 2 + 120;
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject = UIControls.createUIText(panel, bgSprite, "#FFFFFFFF");
            gameObject.GetComponent<Text>().text = "搜索";
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)TeleportWindow.elementX, (float)TeleportWindow.elementY, 0f);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 30f);
            gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            TeleportWindow.elementX += 60;
            int num = 260;
            Sprite bgSprite2 = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject gameObject2 = UIControls.createUIInputField(panel, bgSprite2, "#FFFFFFFF");
            gameObject2.GetComponent<InputField>().text = TeleportWindow.searchText;
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector3((float)(TeleportWindow.elementX + 100), (float)TeleportWindow.elementY, 0f);
            gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2((float)num, 30f);
            gameObject2.GetComponent<InputField>().onEndEdit.AddListener(delegate (string text)
            {
                TeleportWindow.page = 1;
                TeleportWindow.searchText = text;
                bool flag = TeleportWindow.TryGetData();
                if (flag)
                {
                    TeleportWindow.container();
                }
                TeleportWindow.uiText.GetComponent<Text>().text = TeleportWindow.uiText_text;
            });
        }

        // Token: 0x0600005F RID: 95 RVA: 0x0000636C File Offset: 0x0000456C
        private void PageBar(GameObject panel)
        {
            GameObject gameObject = UIControls.createUIPanel(panel, "40", "500", null);
            gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, (float)TeleportWindow.elementY, 0f);
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            bool flag = TeleportWindow.uiText == null;
            if (flag)
            {
                TeleportWindow.uiText = UIControls.createUIText(gameObject, bgSprite, "#ffFFFFFF");
                TeleportWindow.uiText.GetComponent<Text>().text = TeleportWindow.uiText_text;
                TeleportWindow.uiText.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
                TeleportWindow.uiText.GetComponent<Text>().fontSize = 20;
                TeleportWindow.uiText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            }
            string backgroundColor = "#8C9EFFFF";
            GameObject gameObject2 = UIControls.createUIButton(gameObject, backgroundColor, "上一页", delegate
            {
                TeleportWindow.page--;
                bool flag2 = TeleportWindow.page <= 0;
                if (flag2)
                {
                    TeleportWindow.page = 1;
                }
                bool flag3 = TeleportWindow.TryGetData();
                if (flag3)
                {
                    TeleportWindow.container();
                }
                TeleportWindow.uiText.GetComponent<Text>().text = TeleportWindow.uiText_text;
            }, default(Vector3));
            gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 20f);
            gameObject2.GetComponent<RectTransform>().localPosition = new Vector3(-100f, 0f, 0f);
            GameObject gameObject3 = UIControls.createUIButton(gameObject, backgroundColor, "下一页", delegate
            {
                TeleportWindow.page++;
                bool flag2 = TeleportWindow.page >= TeleportWindow.maxPage;
                if (flag2)
                {
                    TeleportWindow.page = TeleportWindow.maxPage;
                }
                bool flag3 = TeleportWindow.TryGetData();
                if (flag3)
                {
                    TeleportWindow.container();
                }
                TeleportWindow.uiText.GetComponent<Text>().text = TeleportWindow.uiText_text;
            }, default(Vector3));
            gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 20f);
            gameObject3.GetComponent<RectTransform>().localPosition = new Vector3(100f, 0f, 0f);
        }

        // Token: 0x06000060 RID: 96 RVA: 0x00006544 File Offset: 0x00004744
        private static void container()
        {
            TeleportWindow.elementX = -200;
            TeleportWindow.elementY = 125;
            foreach (GameObject obj in TeleportWindow.ItemButtons)
            {
                UnityEngine.Object.Destroy(obj);
            }
            TeleportWindow.ItemButtons.Clear();
            TeleportWindow.ItemPanel = UIControls.createUIPanel(TeleportWindow.Panel, "300", "600", null);
            TeleportWindow.ItemPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            TeleportWindow.ItemPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(10f, 0f);
            ScriptTrainer.Instance.Log("ZG:添加物品");
            int num = 0;
            using (List<Place>.Enumerator enumerator3 = TeleportWindow.GetItemData().GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    Place item = enumerator3.Current;
                    GameObject item2 = TeleportWindow.CreateItemButton("瞬移", item, TeleportWindow.ItemPanel, delegate
                    {
                        TeleportWindow.Teleport(item);
                    });
                    TeleportWindow.ItemButtons.Add(item2);
                    num++;
                    bool flag3 = num % 3 == 0;
                    if (flag3)
                    {
                        TeleportWindow.hr();
                    }
                }
            }
        }
        private static GameObject CreateItemButton(string ButtonText, Place place, GameObject panel, UnityAction action)
        {
            int num = 190;
            int num2 = 50;
            string htmlcolorstring = "#FFFFFFFF";
            GameObject gameObject = UIControls.createUIPanel(panel, num2.ToString(), num.ToString(), null);
            gameObject.GetComponent<Image>().color = UIControls.HTMLString2Color("#455A64FF");
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)TeleportWindow.elementX, (float)TeleportWindow.elementY, 0f);
            GameObject gameObject2 = UIControls.createUIPanel(gameObject, num2.ToString(), "50", null);
            gameObject2.GetComponent<Image>().color = UIControls.HTMLString2Color(htmlcolorstring);
            gameObject2.GetComponent<RectTransform>().anchoredPosition = new Vector2(70f, 0f);
            Sprite itemIcon = TeleportWindow.GetItemIcon(place);
            bool flag = itemIcon != null;
            if (flag)
            {
                GameObject gameObject3 = UIControls.createUIImage(gameObject2, itemIcon);
                gameObject3.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 60f);
                gameObject3.GetComponent<RectTransform>().localPosition = new Vector2(0f, 0f);
            }
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject4 = UIControls.createUIText(gameObject, bgSprite, ColorUtility.ToHtmlStringRGBA(Color.white));
            gameObject4.GetComponent<Text>().text = TeleportWindow.GetItemName(place);
            gameObject4.GetComponent<RectTransform>().localPosition = new Vector3(0f, 5f, 0f);
            string backgroundColor = "#8C9EFFFF";
            GameObject gameObject5 = UIControls.createUIButton(gameObject, backgroundColor, ButtonText, action, default(Vector3));
            gameObject5.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 20f);
            gameObject5.GetComponent<RectTransform>().localPosition = new Vector3(-50f, -10f, 0f);
            TeleportWindow.elementX += 200;
            return gameObject;
        }

        // Token: 0x06000064 RID: 100 RVA: 0x00006C3A File Offset: 0x00004E3A
        private static void hr()
        {
            TeleportWindow.elementX = TeleportWindow.initialX;
            TeleportWindow.elementY -= 60;
        }
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
            if (item.BasicData.PlaceType.Value == PlaceType.ZiZhai)
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
        private static bool TryGetData()
        {
            bool flag = DataManager.Instance.GameData.GameRuntimeData.Player == null;
            return !flag;
        }
        private static List<Place> GetItemData()
        {
            List<Place> list = new List<Place>();
            foreach (Place item in DataManager.Instance.GameData.GameRuntimeData.Places)
            {
                list.Add(item);
            }
            bool flag4 = TeleportWindow.searchText != "";
            if (flag4)
            {
                list = TeleportWindow.FilterItemData(list);
            }
            List<Place> list2 = new List<Place>();
            int num = (TeleportWindow.page - 1) * TeleportWindow.conunt;
            int num2 = num + TeleportWindow.conunt;
            for (int i = num; i < num2; i++)
            {
                bool flag5 = i < list.Count;
                if (flag5)
                {
                    list2.Add(list[i]);
                }
            }
            bool flag6 = list.Count % TeleportWindow.conunt != 0;
            if (flag6)
            {
                TeleportWindow.maxPage = list.Count / TeleportWindow.conunt + 1;
            }
            else
            {
                TeleportWindow.maxPage = list.Count / TeleportWindow.conunt;
            }
            return list2;
        }
        private static List<Place> FilterItemData(List<Place> dataList)
        {
            bool flag = TeleportWindow.searchText == "";
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
                    string itemName = TeleportWindow.GetItemName(item);
                    string itemDescription = TeleportWindow.GetItemDescription(item);
                    bool flag2 = itemName.Contains(TeleportWindow.searchText.Replace(" ", "")) || itemDescription.Contains(TeleportWindow.searchText.Replace(" ", ""));
                    if (flag2)
                    {
                        list.Add(item);
                    }
                }
                result = list;
            }
            return result;
        }
        private static string GetItemName(Place item)
        {
            return item.BasicData.Name.ValueInfo;
        }
        private static string GetItemDescription(Place item)
        {
            return item.BasicData.Name.ValueInfo;
        }
        private static Sprite GetItemIcon(Place item)
        {
            return Resources.Load<Sprite>(item.IconPath);
        }
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
