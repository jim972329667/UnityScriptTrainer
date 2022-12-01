using PrefabEntities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityGameUI;
using Zenject;
using Object = UnityEngine.Object;

namespace ScriptTrainer
{
    internal class ItemWindow : MonoBehaviour
    {
        private static GameObject Panel;
        private static int initialX;
        private static int initialY;
        private static int elementX;
        private static int elementY;

        #region[数据分页相关]
        private static GameObject ItemPanel;
        private static List<GameObject> ItemButtons = new List<GameObject>();
        private static int page = 1;
        private static int maxPage = 1;
        private static int conunt = 15;
        private static string searchText = "";
        private static GameObject uiText;
        private static string uiText_text
        {
            get
            {
                return $"{page} / {maxPage}";
            }
        }
        #endregion

        public ItemWindow(GameObject panel, int x, int y)
        {
            Panel = panel;
            initialX = elementX = x + 50;
            initialY = elementY = y;
            Initialize();
        }

        public void Initialize()
        {
            //创建搜索框
            searchBar(Panel);
            elementY += 10;
            hr();

            //创建物品列表
            container();

            elementX += 200;
            elementY = 125 - 60 *5;
            //创建分页

            pageBar(Panel);
        }

        #region[创建详细]

        //搜索框
        private void searchBar(GameObject panel)
        {
            elementY += 10;
            elementX = -MainWindow.width / 2 + 120;
            //label
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(panel, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<Text>().text = "搜索";
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);
            uiText.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 30);
            uiText.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

            //坐标偏移
            elementX += 60;

            //输入框
            int w = 260;
            Sprite inputFieldSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject uiInputField = UIControls.createUIInputField(panel, inputFieldSprite, "#FFFFFFFF");
            uiInputField.GetComponent<InputField>().text = searchText;
            uiInputField.GetComponent<RectTransform>().localPosition = new Vector3(elementX + 100, elementY, 0);
            uiInputField.GetComponent<RectTransform>().sizeDelta = new Vector2(w, 30);


            //文本框失去焦点时触发方法
            uiInputField.GetComponent<InputField>().onEndEdit.AddListener((string text) =>
            {
                //Debug.Log(text);
                page = 1;
                searchText = text;
                container();
                ItemWindow.uiText.GetComponent<Text>().text = uiText_text;
                //Destroy(ItemPanel);
            });
        }

        //分页
        private void pageBar(GameObject panel)
        {
            //背景
            GameObject pageObj = UIControls.createUIPanel(panel, "40", "500");
            pageObj.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            pageObj.GetComponent<RectTransform>().localPosition = new Vector3(0, elementY, 0);

            //当前页数 / 总页数
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));

            if (uiText == null)
            {
                uiText = UIControls.createUIText(pageObj, txtBgSprite, "#ffFFFFFF");
                uiText.GetComponent<Text>().text = uiText_text;
                uiText.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                //设置字体
                uiText.GetComponent<Text>().fontSize = 20;
                uiText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            }


            //上一页
            string backgroundColor = "#8C9EFFFF";
            GameObject prevBtn = UIControls.createUIButton(pageObj, backgroundColor, "上一页", () =>
            {

                page--;
                if (page <= 0) page = 1;
                container();
                uiText.GetComponent<Text>().text = uiText_text;
            }, new Vector3());
            prevBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 20);
            prevBtn.GetComponent<RectTransform>().localPosition = new Vector3(-100, 0, 0);

            //下一页
           GameObject nextBtn = UIControls.createUIButton(pageObj, backgroundColor, "下一页", () =>
           {
               page++;
               if (page >= maxPage) page = maxPage;
               container();
               uiText.GetComponent<Text>().text = uiText_text;
           });
            nextBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 20);
            nextBtn.GetComponent<RectTransform>().localPosition = new Vector3(100, 0, 0);
        }

        private static void container()
        {
            if (ScriptPatch.pool != null)
            {
                //Debug.Log($"x:{elementX}, y:{elementY}");
                elementX = -200;
                elementY = 125;

                //先清空旧的 ItemPanel
                foreach (var item in ItemButtons)
                {
                    UnityEngine.Object.Destroy(item);
                }
                ItemButtons.Clear();


                ItemPanel = UIControls.createUIPanel(Panel, "300", "600");
                ItemPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
                ItemPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, 0);

                int num = 0;
                foreach (PrefabEntity item in GetItemData())
                {
                    var btn = createItemButton(item, ItemPanel, () =>
                    {
                        SpawnItem(item);
                    });
                    ItemButtons.Add(btn);
                    num++;
                    if (num % 3 == 0)
                    {
                        hr();
                    }
                }
            }
        }
        //public static void SpawnEquipmentInputDialog(string title, int id, Action<string> onFinish)
        //{
        //    GameObject canvas = UIControls.createUICanvas();    // 创建画布
        //    Object.DontDestroyOnLoad(canvas);
        //    //设置置顶显示
        //    canvas.GetComponent<Canvas>().overrideSorting = true;
        //    canvas.GetComponent<Canvas>().sortingOrder = 100;

        //    string xx = StrTool.GetPlayerItemInfoStr(id).Replace("color", "").Replace("/", "").Replace("=", "");
        //    var xxx = xx.Split('<', '>');
        //    List<string> outline = new List<string>();
        //    for (int i = 0; i < xxx.Length; i++)
        //    {
        //        if (xxx[i].Contains('\n'))
        //        {
        //            outline.Add("#FFFFFF");
        //            string[] tmps = xxx[i].Split('\n');
        //            foreach (string tmp in tmps)
        //            {
        //                string tmp_2 = tmp.Replace(" ", "").Trim('\r', '\n');
        //                if (tmp_2.Length > 1)
        //                {
        //                    outline.Add(tmp_2);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            string text = xxx[i].Replace(" ", "").Trim('\r', '\n');
        //            if (text.Length > 1)
        //            {
        //                if (text.Length > 20)
        //                {
        //                    var tmps = GetSeparateSubString(text, 20);
        //                    foreach (var tmp in tmps)
        //                    {
        //                        outline.Add(tmp.ToString());
        //                    }
        //                }
        //                else
        //                    outline.Add(text);
        //            }
        //        }
        //    }
        //    for (int i = 0; i < outline.Count; i++)
        //    {
        //        if (outline[i].Contains("受到伤害增加"))
        //        {
        //            if (i + 1 < outline.Count)
        //            {
        //                outline[i + 1] = "";
        //            }
        //            if (i + 2 < outline.Count)
        //            {
        //                outline[i] += outline[i + 2];
        //                outline[i + 2] = "";
        //            }
        //        }
        //    }
        //    int size = GetLineCount(outline) * 20 + 20 + 40;



        //    int index = 0;
        //    foreach (var txt in outline)
        //    {
        //        Debug.Log($"{index}:{txt}");
        //        index++;
        //    }

        //    GameObject uiPanel = UIControls.createUIPanel(canvas, size.ToString(), "300", null);  // 创建面板
        //    uiPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#37474FFF"); // 设置背景颜色

        //    创建标题
        //   Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));


        //    string DefaultColor = "#FFFFFFFF";



        //    int j = 0;
        //    foreach (var line in outline)
        //    {
        //        if (line.StartsWith("#"))
        //        {
        //            DefaultColor = line + "FF";
        //        }
        //        else if (line == "green")
        //        {
        //            DefaultColor = ColorUtility.ToHtmlStringRGBA(Color.green);
        //        }
        //        else if (line == "red")
        //        {
        //            DefaultColor = ColorUtility.ToHtmlStringRGBA(Color.red);
        //        }
        //        else if (line != "")
        //        {
        //            GameObject uiText = UIControls.createUIText(uiPanel, txtBgSprite, DefaultColor);
        //            uiText.GetComponent<Text>().text = line;
        //            uiText.GetComponent<RectTransform>().localPosition = new Vector2(0, size / 2 - 20 - 20 * j);
        //            uiText.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 20);
        //            uiText.GetComponent<Text>().fontSize = 14;
        //            j++;
        //        }
        //    }

        //    //创建确定按钮
        //   GameObject uiButton = UIControls.createUIButton(uiPanel, "#8C9EFFFF", title, () =>
        //   {
        //       onFinish("1");
        //       Object.Destroy(canvas);
        //   }, new Vector3(100, -size / 2 + 30, 0));

        //    //创建关闭按钮
        //   GameObject closeButton = UIControls.createUIButton(uiPanel, "#B71C1CFF", "X", () =>
        //   {
        //       Object.Destroy(canvas);
        //   }, new Vector3(350 / 2 - 10, size / 2 - 10, 0));
        //    //设置closeButton宽高
        //    closeButton.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
        //    //字体颜色为白色
        //    closeButton.GetComponentInChildren<Text>().color = UIControls.HTMLString2Color("#FFFFFFFF");
        //}
        private static GameObject createItemButton(PrefabEntity item, GameObject panel, UnityAction action)
        {
            //按钮宽 200 高 50
            int buttonWidth = 190;
            int buttonHeight = 50;

            //根据品质设置背景颜色
            string qualityColor = "#FFFFFFFF";

            //创建一个背景
            GameObject background = UIControls.createUIPanel(panel, buttonHeight.ToString(), buttonWidth.ToString(), null);
            background.GetComponent<Image>().color = UIControls.HTMLString2Color("#455A64FF");
            background.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);


            GameObject background_icon = UIControls.createUIPanel(background, buttonHeight.ToString(), "50", null);
            background_icon.GetComponent<Image>().color = UIControls.HTMLString2Color(qualityColor);
            background_icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(70, 0);

            Sprite BgSprite = GetSprite(item);
            GameObject icon = UIControls.createUIImage(background_icon, BgSprite);
            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
            icon.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);

            //创建文字
            //Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#455A64FF"));
            //GameObject uiText = UIControls.createUIText(background, txtBgSprite, text);
            //uiText.GetComponent<Text>().fontSize = 20;
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(background, txtBgSprite, ColorUtility.ToHtmlStringRGBA(Color.white));
            uiText.GetComponent<Text>().text = GetName(item);
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(0, 5, 0);

            //创建按钮
            string backgroundColor_btn = "#8C9EFFFF";
            GameObject button = UIControls.createUIButton(background, backgroundColor_btn, "获得", action, new Vector3());
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 20);
            button.GetComponent<RectTransform>().localPosition = new Vector3(-50, -10, 0);

            elementX += 200;

            return button;
        }
        public static GameObject AddToggle(string Text, int width, GameObject panel, UnityAction<bool> action)
        {
            //计算x轴偏移
           elementX += width / 2 - 30;

            Sprite toggleBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture(ColorUtility.ToHtmlStringRGBA(Color.white)));
            Sprite toggleSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#18FFFFFF"));
            GameObject uiToggle = UIControls.createUIToggle(panel, toggleBgSprite, toggleSprite);
            uiToggle.GetComponentInChildren<Text>().color = Color.white;
            uiToggle.GetComponentInChildren<Toggle>().isOn = false;
            uiToggle.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);

            uiToggle.GetComponentInChildren<Text>().text = Text;
            uiToggle.GetComponentInChildren<Toggle>().onValueChanged.AddListener(action);


            elementX += width / 2 + 10;

            return uiToggle;
        }
        private static void hr()
        {
            elementX = initialX;
            elementY -= 60;
        }

        #endregion

        //public static string GetQuailityColor(QualityType quality)
        //{
        //    if (quality == QualityType.Green)
        //        return ColorUtility.ToHtmlStringRGBA(Color.green);
        //    else if (quality == QualityType.Blue)
        //        return ColorUtility.ToHtmlStringRGBA(Color.blue);
        //    else if (quality == QualityType.Purple)
        //        return ColorUtility.ToHtmlStringRGBA(new Color(143f / 255f, 7f / 255f, 131f / 255f));
        //    else if (quality == QualityType.Orange)
        //        return ColorUtility.ToHtmlStringRGBA(new Color(1f, 165f / 255f, 0f));
        //    else if (quality == QualityType.Red)
        //        return ColorUtility.ToHtmlStringRGBA(Color.red);
        //    else
        //        return ColorUtility.ToHtmlStringRGBA(Color.white);
        //}

        public static void SpawnItem(PrefabEntity item)
        {
            if(ScriptPatch.factory != null && ScriptPatch.player != null)
            {
                PrefabID prefabID = ScriptPatch.factory.Create<PrefabID>(item.prefab, new GameObjectCreationParameters
                {
                    Position = ScriptPatch.player.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f))
                }, null, null);
                Debug.Log($"ZG:添加物品{item.name}");
                foreach (var type in item.Types)
                {
                    Debug.Log($"ZG:添加物品类型{type}");
                }
            }   
        }

        #region[获取数据相关函数]
        private static List<PrefabEntity> GetItemData()
        {
            
            List<PrefabEntity> ItemData = ScriptPatch.pool.GetAllBy(null, new PrefabEntities.Type[]
            {
                PrefabEntities.Type.Resource,
                //PrefabEntities.Type.AmmoBox,
                //PrefabEntities.Type.Artifact,
                //PrefabEntities.Type.Bird,
                //PrefabEntities.Type.Chest,
                //PrefabEntities.Type.Egg,
                //PrefabEntities.Type.Item,
                //PrefabEntities.Type.Trinket,
                //PrefabEntities.Type.CurseReward,
            }, new PrefabEntities.Type[] { PrefabEntities.Type.Unlockable }, null,Pool.IncludeRemoved.No,false).ToList<PrefabEntity>();
            ItemData.AddRange(ScriptPatch.pool.GetAllBy(null, new PrefabEntities.Type[]{
                PrefabEntities.Type.AmmoBox}, new PrefabEntities.Type[] { PrefabEntities.Type.Unlockable }, null, Pool.IncludeRemoved.No, false).ToList<PrefabEntity>());

            ItemData.AddRange(ScriptPatch.pool.GetAllBy(null, new PrefabEntities.Type[]{
                PrefabEntities.Type.Artifact}, new PrefabEntities.Type[] { PrefabEntities.Type.Unlockable,PrefabEntities.Type.Curse }, null, Pool.IncludeRemoved.No, false).ToList<PrefabEntity>());

            ItemData.AddRange(ScriptPatch.pool.GetAllBy(null, new PrefabEntities.Type[]{
                PrefabEntities.Type.Bird}, new PrefabEntities.Type[] { PrefabEntities.Type.Unlockable}, null, Pool.IncludeRemoved.No, false).ToList<PrefabEntity>());

            ItemData.AddRange(ScriptPatch.pool.GetAllBy(null, new PrefabEntities.Type[]{
                PrefabEntities.Type.Trinket}, new PrefabEntities.Type[] { PrefabEntities.Type.Unlockable }, null, Pool.IncludeRemoved.No, false).ToList<PrefabEntity>());

            Debug.Log($"ZG:{ItemData.Count}");
            if (searchText != "")
            {
                ItemData = FilterItemData(ItemData);
            }
            //Curse诅咒 Magazine
            //对 DataList 进行分页
            List<PrefabEntity> list = new List<PrefabEntity>();
            int start = (page - 1) * conunt;
            int end = start + conunt;
            for (int i = start; i < end; i++)
            {
                if (i < ItemData.Count)
                {
                    list.Add(ItemData[i]);
                }
            }
            maxPage = (ItemData.Count - 1) / conunt + 1;

            return list;
        }

        //搜索过滤
        private static List<PrefabEntity> FilterItemData(List<PrefabEntity> dataList)
        {
            if (searchText == "")
            {
                return dataList;
            }
            List<PrefabEntity> list = new List<PrefabEntity>();

            foreach (var item in dataList)
            {
                string text = GetName(item);
                if (text.Contains(searchText.Replace(" ", "")))
                {
                    list.Add(item);
                }
            }

            return list;
        }

        private static ArrayList GetSeparateSubString(string txtString, int charNumber)
        {
            ArrayList arrlist = new ArrayList();
            string tempStr = txtString;
            for (int i = 0; i < tempStr.Length; i += charNumber)
            {
                if ((tempStr.Length - i) > charNumber)//如果是，就截取
                {
                    arrlist.Add(tempStr.Substring(i, charNumber));
                }
                else
                {
                    arrlist.Add(tempStr.Substring(i));//如果不是，就截取最后剩下的那部分
                }
            }
            return arrlist;
        }
        private static string GetName(PrefabEntity item)
        {
            item.prefab.TryGetComponent<Item>(out Item item1);
            string text = string.Empty;
            if (item1 != null)
            {
                text = item1.itemDescription.itemName.GetLocalizedString();
            }
            else
            {
                text = item.name;
            }
            return text;
        }
        private static Sprite GetSprite(PrefabEntity item)
        {
            item.prefab.TryGetComponent<Item>(out Item item1);
            Sprite sprite = null;
            if (item1 != null)
            {
                sprite = item1.itemDescription.sprite;
            }
            return sprite;
        }
        #endregion
    }

}
