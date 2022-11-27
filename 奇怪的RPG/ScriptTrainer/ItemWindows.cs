using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityGameUI;
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
        private static Image image;
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
            //Debug.Log(DataList.Count.ToString());
            // 创建搜索框
            searchBar(Panel);
            elementY += 10;
            hr();

            // 创建物品列表
            container();


            // 创建分页
            pageBar(Panel);
        }

        #region[创建详细]

        // 搜索框
        private void searchBar(GameObject panel)
        {
            elementY += 10;

            // label
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(panel, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<Text>().text = "搜索";
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);
            //uiText.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 30);
            uiText.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

            // 坐标偏移
            elementX += 10;

            // 输入框
            int w = 260;
            Sprite inputFieldSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#212121FF"));
            GameObject uiInputField = UIControls.createUIInputField(panel, inputFieldSprite, "#FFFFFFFF");
            uiInputField.GetComponent<InputField>().text = searchText;
            uiInputField.GetComponent<RectTransform>().localPosition = new Vector3(elementX + 100, elementY, 0);
            uiInputField.GetComponent<RectTransform>().sizeDelta = new Vector2(w, 30);

            // 文本框失去焦点时触发方法
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

        // 分页
        private void pageBar(GameObject panel)
        {
            // 背景
            GameObject pageObj = UIControls.createUIPanel(panel, "40", "500");
            pageObj.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            pageObj.GetComponent<RectTransform>().localPosition = new Vector3(0, elementY, 0);

            // 当前页数 / 总页数
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));

            if (uiText == null)
            {
                uiText = UIControls.createUIText(pageObj, txtBgSprite, "#ffFFFFFF");
                uiText.GetComponent<Text>().text = uiText_text;
                uiText.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                // 设置字体
                uiText.GetComponent<Text>().fontSize = 20;
                uiText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            }


            // 上一页
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

            // 下一页            
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
            //Debug.Log($"x:{elementX}, y:{elementY}");
            elementX = -200;
            elementY = 125;

            // 先清空旧的 ItemPanel
            foreach (var item in ItemButtons)
            {
                UnityEngine.Object.Destroy(item);
            }
            ItemButtons.Clear();


            ItemPanel = UIControls.createUIPanel(Panel, "300", "600");
            ItemPanel.GetComponent<Image>().color = UIControls.HTMLString2Color("#424242FF");
            ItemPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, 0);

            int num = 0;
            foreach (Config_Item item in GetItemData())
            {
                var btn = createItemButton(item.itemName, ItemPanel, item.icon, item.quality, () =>
                {
                    UIWindows.SpawnInputDialog($"您想获取多少个{item.itemName}？", "添加", "1", (string count) =>
                    {
                        Debug.Log($"已添加{count}个{item.itemName}到背包");
                        Singleton<BagManager>.Instance.AddItem(item.id, count.ConvertToIntDef(1), true);
                    });
                });
                ItemButtons.Add(btn);

                num++;
                if (num % 3 == 0)
                {
                    hr();
                }
            }
        }

        private static GameObject createItemButton(string text, GameObject panel, string itemIcon, QualityType quality, UnityAction action)
        {
            // 按钮宽 200 高 50
            int buttonWidth = 190;
            int buttonHeight = 50;

            // 根据品质设置背景颜色
            string qualityColor = "#FFFFFFFF";
            qualityColor = StrTool.GetQuailityBgColorStr(quality);

            // 创建一个背景
            GameObject background = UIControls.createUIPanel(panel, buttonHeight.ToString(), buttonWidth.ToString(), null);
            background.GetComponent<Image>().color = UIControls.HTMLString2Color("#455A64FF");
            background.GetComponent<RectTransform>().localPosition = new Vector3(elementX, elementY, 0);

            // 创建图标  60x60
            Sprite BgSprite = null;
            Singleton<ResManager>.Instance.Load<Sprite>(itemIcon, delegate (Sprite sprite)
            {
                if (BgSprite == null)
                {
                    BgSprite = sprite;
                }
            });
            //if (BgSprite == null)
            //{
            //    BgSprite = ResManager.inst.LoadSprite("Item Icon/1");
            //}
            GameObject icon = UIControls.createUIImage(background, BgSprite);
            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
            icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(30, 0);

            // 创建文字
            //Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#455A64FF"));
            //GameObject uiText = UIControls.createUIText(background, txtBgSprite, text);
            //uiText.GetComponent<Text>().fontSize = 20;
            Sprite txtBgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject uiText = UIControls.createUIText(background, txtBgSprite, "#FFFFFFFF");
            uiText.GetComponent<Text>().text = text;
            uiText.GetComponent<RectTransform>().localPosition = new Vector3(0, 5, 0);

            // 创建按钮
            string backgroundColor_btn = "#8C9EFFFF";
            GameObject button = UIControls.createUIButton(background, backgroundColor_btn, "获取", action, new Vector3());
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 20);
            button.GetComponent<RectTransform>().localPosition = new Vector3(-50, -10, 0);


            elementX += 200;

            //return button;
            return background;
        }

        private static void hr()
        {
            elementX = initialX;
            elementY -= 60;
        }

        #endregion

        #region[获取数据相关函数]
        private static List<Config_Item> GetItemData()
        {
            var ItemData = ClearNoneItem(Config_Item.data.Values.ToList<Config_Item>());

            if (searchText != "")
            {
                ItemData = FilterItemData(ItemData);
            }

            // 对 DataList 进行分页
            List<Config_Item> list = new List<Config_Item>();
            int start = (page - 1) * conunt;
            int end = start + conunt;
            for (int i = start; i < end; i++)
            {
                if (i < ItemData.Count)
                {
                    list.Add(ItemData[i]);
                }
            }
            maxPage = ItemData.Count / conunt;

            return list;
        }

        // 搜索过滤
        private static List<Config_Item> FilterItemData(List<Config_Item> dataList)
        {
            if (searchText == "")
            {
                return dataList;
            }
            List<Config_Item> list = new List<Config_Item>();

            foreach (var item in dataList)
            {
                if (item.itemName.Contains(searchText.Replace(" ","")))
                {
                    list.Add(item);
                }
            }

            return list;
        }
        private static List<Config_Item> ClearNoneItem(List<Config_Item> dataList)
        {
            List<Config_Item> list = new List<Config_Item>();

            foreach (var item in dataList)
            {
                if (item.itemName != null && item.itemName != "" && item.itemName != string.Empty)
                {
                    list.Add(item);
                }
            }

            return list;
        }

        #endregion
    }

}
