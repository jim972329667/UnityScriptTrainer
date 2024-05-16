using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets.ScrollView;
using UniverseLib.UI;
using ZGScriptTrainer.ItemSpwan;
using ZGScriptTrainer.UI.Cells;
using System.Xml;
using ZGScriptTrainer.UI.Models;

namespace ZGScriptTrainer.UI.Panels
{
    public class ItemWindow : ZGPanel/*, ICellPoolDataSource<ItemCell>*/
    {
        public static ItemWindow Instance { get; private set; }
        public override ZGUIManager.Panels PanelType => ZGUIManager.Panels.ItemWindow;
        public override string Name => ZGUIManager.PanelNames[PanelType];
        public override int MinWidth => 350;
        public override int MinHeight => 200;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.1f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.85f);

        private readonly List<ZGItem> ZGItems = new List<ZGItem>();
        public int ItemCount => ZGItems.Count;

        //public ScrollPool<ItemCell> ScrollPool { get; private set; }
        public bool Initialized { get; private set; } = false;

        public int ItemSpawnAmount = 1;

        public Dropdown ItemTypeDropdown;

        public static InputFieldRef SearchInput;

        public GameObject ItemGrid;
        public UniverseLib.UI.Widgets.AutoSliderScrollbar autoSliderScrollbar;
        public List<GameObject> Items = new List<GameObject>();

        public GameObject ItemGridScrollView;
        private bool CanAddListener = true;
        private int ItemTypeDropdownIndex = 0;

        public override void OnFinishResize()
        {
            base.OnFinishResize();
        }
        public ItemWindow(UIBase owner) : base(owner)
        {
            Instance = this;
            Initialize();

            //foreach (KeyValuePair<string, IConfigElement> entry in ConfigManager.ConfigElements)
            //{
            //    configEntries.Add(cache);
            //}

            //foreach (CacheConfigEntry config in configEntries)
            //    config.UpdateValueFromSource();
        }
        public void Initialize()
        {
            if (ZGItemUtil.BaseItems.Count != 0)
            {
                ZGItems.AddRange(ZGItemUtil.GetTypeItems());
                Initialized = true;
                PrintItems();
            }
        }
        public void SetCell(ItemCell cell, int index)
        {
            if (index < 0 || index >= ZGItems.Count)
            {
                cell.Disable();
                return;
            }

            ZGItem entry = ZGItems[index];
            cell.NameLabel.text = entry.GetItemName();
            cell.ItemImage.sprite = entry.GetItemIcon();
            cell.DescriptionLabel.text = entry.GetItemDescription();
            cell.SubmitButton.OnClick = new Action(() => {
                entry.Count = ItemSpawnAmount;
                ZGItemUtil.SpwanItem(entry);
                ZGScriptTrainer.WriteLog($"添加物品数量：{entry.Count}");
            });
        }
        public void OnCellBorrowed(ItemCell cell)
        {

        }
        public void SearchItem(string text)
        {
            ZGItems.Clear();
            ZGItems.AddRange(text.FilterItemData(ItemTypeDropdown.value));
            //ScrollPool.Refresh(true, true);
            PrintItems();
        }
        public void ChangeItemCount(string text)
        {
            if (int.TryParse(text, out int result))
            {
                ItemSpawnAmount = result;
            }
        }
        private void ItemTypeDropdownValueChange(int value)
        {
            ZGItems.Clear();
            ZGItems.AddRange(SearchInput.Text.FilterItemData(value));
            //ScrollPool.Refresh(true, true);
            PrintItems();
        }
        public void PrintItems()
        {
            foreach(var item in Items)
            {
                UnityEngine.Object.Destroy(item);
            }
            Items.Clear();
            foreach(var item in ZGItems)
            {
                var UIRoot = UIFactory.CreateUIObject("GridUIRoot", ItemGrid, new Vector2(200, 60));
                var Rect = UIRoot.GetComponent<RectTransform>();

                //物品图片
                var ItemUI = UIFactory.CreateUIObject("ItemUI", UIRoot, new Vector2(50, 50));
                var ItemImage = ItemUI.AddComponent<Image>();
                ItemImage.preserveAspect = true;
                ItemImage.sprite = item.GetItemIcon();
                UIFactory.SetLayoutElement(ItemUI, minHeight: 50, minWidth: 50, flexibleWidth: 0, flexibleHeight: 0);
                ItemUI.GetComponent<RectTransform>().localPosition = new Vector2(-70, 0);

                var SubmitButton = UIFactory.CreateButton(UIRoot, "SubmitButton", "添加", new Color(0.15f, 0.19f, 0.15f));
                SubmitButton.OnClick = new Action(() => {
                    item.Count = ItemSpawnAmount;
                    ZGItemUtil.SpwanItem(item);
                    ZGScriptTrainer.WriteLog($"添加物品数量：{item.Count}");
                });
                SubmitButton.ButtonText.fontSize = ZGScriptTrainer.FontSize.Value;
                float size = 40f;
                SubmitButton.ButtonText.GetComponent<RectTransform>().sizeDelta = new Vector2(size, 45);
                size = SubmitButton.ButtonText.preferredWidth;
                SubmitButton.ButtonText.GetComponent<RectTransform>().sizeDelta = new Vector2(size, 45);
                SubmitButton.Component.GetComponent<RectTransform>().sizeDelta = new Vector2(size + 5, 50);
                SubmitButton.Component.GetComponent<RectTransform>().localPosition = new Vector2(95 - SubmitButton.Component.GetComponent<RectTransform>().sizeDelta.x/2, 0);

                var NameLabel = UIFactory.CreateLabel(UIRoot, "NameLabel", item.GetItemName(), TextAnchor.MiddleLeft, fontSize: ZGScriptTrainer.FontSize.Value);
                NameLabel.horizontalOverflow = HorizontalWrapMode.Wrap;
                
                float labelw = 200 - 60 - SubmitButton.Component.GetComponent<RectTransform>().sizeDelta.x;
                NameLabel.GetComponent<RectTransform>().sizeDelta = new Vector2(labelw, 50);
                NameLabel.GetComponent<RectTransform>().localPosition = new Vector2(-40 + labelw/2, 0);

                NameLabel.resizeTextForBestFit = true;
                NameLabel.resizeTextMinSize = 10;
                NameLabel.resizeTextMaxSize = ZGScriptTrainer.FontSize.Value;

                var tool = UIRoot.AddComponent<TooltipGUI>();
                tool.Initialize(item.GetItemDescription(), ItemGridScrollView);

                Items.Add(UIRoot);
            }

            //for(int i = 0; i < ZGItems.Count; i++)
            //{
            //    var tool = Items[i].AddComponent<TooltipGUI>();
            //    tool.Initialize(ZGItems[i].GetItemDescription(), ZGUIManager.UIToolTip/*Items.Last()*/);
            //}
        }

        public override void Update()
        {
            base.Update();
            if (!CanAddListener)
            {
                if(ItemTypeDropdownIndex != ItemTypeDropdown.value)
                {
                    ItemTypeDropdownIndex = ItemTypeDropdown.value;
                    ItemTypeDropdownValueChange(ItemTypeDropdownIndex);
                }
            }
        }
        protected override void ConstructPanelContent()
        {
            GameObject tabGroup = UIFactory.CreateVerticalGroup(ContentRoot, "ItemWindowVerticalGroup", true, false, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f));
            UIFactory.SetLayoutElement(tabGroup, minHeight: 40, flexibleHeight: 9999);
            // Save button
            GameObject Row1 = UIFactory.CreateHorizontalGroup(tabGroup, "Row1", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row1, minHeight: 40, flexibleWidth: 9999);

            var lable1 = UIFactory.CreateLabel(Row1, "Lable", "类别:", fontSize: ZGScriptTrainer.FontSize.Value);
            ZGUIUtility.SetLayoutElement(lable1, minHeight: 40, flexibleWidth: 0);

            GameObject gameObject2 = UIFactory.CreateDropdown(Row1, "ItemTypeDropdown", out ItemTypeDropdown, "选择物品类型", ZGScriptTrainer.FontSize.Value, null, null);
            for (int i = 0; i < ZGItemUtil.ItemTypeDic.Count; i++)
            {
                ItemTypeDropdown.options.Add(new Dropdown.OptionData(ZGItemUtil.ItemTypeDic[i]));
            }

            //GameObject gameObject2 = Row1.AddDropdown("选择物品类型", ZGItemUtil.ItemTypeDic, ItemTypeDropdownValueChange);
            UIFactory.SetLayoutElement(gameObject2, minHeight: 40, minWidth: 180);
            ItemTypeDropdown.captionText.fontSize = ZGScriptTrainer.FontSize.Value;
            try
            {
                ItemTypeDropdown.onValueChanged.AddListener((UnityEngine.Events.UnityAction<int>)ItemTypeDropdownValueChange);
            }
            catch (Exception ex)
            {
                ZGScriptTrainer.WriteLog($"AddListener错误：\n{ex}", LogType.Error);
                CanAddListener = false;
                //if (ItemTypeDropdown != null)
                //{
                //    UnityEngine.Object.Destroy(ItemTypeDropdown);
                //}
                //for (int i = 0; i < ZGItemUtil.ItemTypeDic.Count; i++)
                //{
                //    var button = UIFactory.CreateButton(Row1, "Row1Button", ZGItemUtil.ItemTypeDic[i]);
                //    button.SetLayoutElement(minHeight: 40, flexibleWidth: 0);
                //    int tmpi = i;
                //    button.OnClick = () => { ItemTypeDropdownValueChange(tmpi); ZGScriptTrainer.WriteLog($"ZG Button:{tmpi}"); };
                //}
            }

            GameObject Row2 = UIFactory.CreateHorizontalGroup(tabGroup, "Row2", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row2, minHeight: 40, flexibleWidth: 0);

            var lable2 = UIFactory.CreateLabel(Row2, "Lable", "搜索:", fontSize: ZGScriptTrainer.FontSize.Value);
            ZGUIUtility.SetLayoutElement(lable2, minHeight: 40, flexibleWidth: 0);

            SearchInput = UIFactory.CreateInputField(Row2, "ItemSearchInput", ZGItemUtil.DefaultSearchText);
            ZGUIUtility.SetLayoutElement(SearchInput, minWidth: 180, minHeight: 40);
            SearchInput.OnValueChanged += new Action<string>(SearchItem);


            GameObject Row3 = UIFactory.CreateHorizontalGroup(tabGroup, "Row3", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row3, minHeight: 40, flexibleWidth: 0);

            var lable3 = UIFactory.CreateLabel(Row3, "Lable", "数量:", fontSize: ZGScriptTrainer.FontSize.Value);
            ZGUIUtility.SetLayoutElement(lable3, minHeight: 40, flexibleWidth: 0);

            var CountInput = UIFactory.CreateInputField(Row3, "ItemCountInput", "1");
            ZGUIUtility.SetLayoutElement(CountInput, minWidth: 180, minHeight: 40);
            CountInput.OnValueChanged += new Action<string>(ChangeItemCount);



            // Config entries

            //ScrollPool = UIFactory.CreateScrollPool<ItemCell>(
            //    this.ContentRoot,
            //    "ZGItems",
            //    out GameObject scrollObj,
            //    out GameObject scrollContent);

            //ScrollPool.Initialize(this);

            ItemGridScrollView = tabGroup.CreateGridScrollView("ItemGridScrollView", new Vector2(200, 60), new Vector2(10, 5), out ItemGrid, out autoSliderScrollbar);
        }
    }
}
