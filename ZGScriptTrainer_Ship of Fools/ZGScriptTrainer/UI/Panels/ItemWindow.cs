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
using static UnityEngine.Localization.Metadata.SharedTableCollectionMetadata;

namespace ZGScriptTrainer.UI.Panels
{
    public class ItemWindow : ZGPanel, ICellPoolDataSource<ItemCell>
    {
        public static ItemWindow Instance { get; private set; }
        public override UIManager.Panels PanelType => UIManager.Panels.ItemWindow;
        public override string Name => UIManager.PanelNames[PanelType];
        public override int MinWidth => 350;
        public override int MinHeight => 200;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.1f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.85f);

        private readonly List<ZGItem> ZGItems = new List<ZGItem>();
        public int ItemCount => ZGItems.Count;
        public ScrollPool<ItemCell> ScrollPool { get; private set; }
        public bool Initialized { get; private set; } = false;

        public int ItemSpawnAmount = 1;

        public Dropdown ItemTypeDropdown;

        public static InputFieldRef SearchInput;


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
            if (ZGItemUtil.CanGetItemData())
            {
                ZGItemUtil.GetBaseItemData();
            }
            if (ZGItemUtil.BaseItems.Count != 0)
            {
                ZGItems.AddRange(ZGItemUtil.GetTypeItems());
                ScrollPool?.Refresh(true, true);
                Initialized = true;
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
            cell.SubmitButton.Component.onClick.RemoveAllListeners();
            cell.SubmitButton.Component.onClick.AddListener(new Action(() => {
                entry.Count = ItemSpawnAmount;
                ZGItemUtil.SpwanItem(entry);
                ZGScriptTrainer.WriteLog($"添加{entry.GetItemName()}数量：{entry.Count}");
            }));
        }
        public void OnCellBorrowed(ItemCell cell)
        {

        }
        public void SearchItem(string text)
        {
            ZGItems.Clear();
            ZGItems.AddRange(text.FilterItemData(ItemTypeDropdown.value));
            ScrollPool.Refresh(true, true);
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
            ScrollPool.Refresh(true, true);
        }
        protected override void ConstructPanelContent()
        {
            //// Save button
            GameObject Row1 = UIFactory.CreateHorizontalGroup(this.ContentRoot, "Row1", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row1, minHeight: 40, flexibleWidth: 9999);

            var lable1 = UIFactory.CreateLabel(Row1, "Lable", "类别:", fontSize: ZGScriptTrainer.FontSize.Value);
            //ZGUIUtility.SetLayoutElement(lable1, minHeight: 40, flexibleWidth: 0);
            //try
            //{
            GameObject gameObject2 = UIFactory.CreateDropdown(Row1, "ItemTypeDropdown", out ItemTypeDropdown, "选择物品类型", ZGScriptTrainer.FontSize.Value, ItemTypeDropdownValueChange, null);
            for (int i = 0; i < ZGItemUtil.ItemTypeDic.Count; i++)
            {
                ItemTypeDropdown.options.Add(new Dropdown.OptionData(ZGItemUtil.ItemTypeDic[i]));
            }

            //GameObject gameObject2 = Row1.AddDropdown("选择物品类型", ZGItemUtil.ItemTypeDic, ItemTypeDropdownValueChange);
            UIFactory.SetLayoutElement(gameObject2, minHeight: 40, minWidth: 180);
            ItemTypeDropdown.captionText.fontSize = ZGScriptTrainer.FontSize.Value;
            //}
            //catch (Exception ex)
            //{
            //    ZGScriptTrainer.WriteLog(ex, LogType.Error);
            //    if (ItemTypeDropdown != null)
            //    {
            //        UnityEngine.Object.Destroy(ItemTypeDropdown);
            //    }
            //    for (int i = 0; i < ZGItemUtil.ItemTypeDic.Count; i++)
            //    {
            //        var button = UIFactory.CreateButton(Row1, "Row1Button", ZGItemUtil.ItemTypeDic[i]);
            //        button.SetLayoutElement(minHeight: 40, flexibleWidth: 0);
            //        int tmpi = i;
            //        button.OnClick = () => { ItemTypeDropdownValueChange(tmpi); ZGScriptTrainer.WriteLog($"ZG Button:{tmpi}"); };
            //    }
            //}

            GameObject Row2 = UIFactory.CreateHorizontalGroup(this.ContentRoot, "Row2", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row2, minHeight: 40, flexibleWidth: 9999);

            var lable2 = UIFactory.CreateLabel(Row2, "Lable", "搜索:", fontSize: ZGScriptTrainer.FontSize.Value);
            ZGUIUtility.SetLayoutElement(lable2, minHeight: 40, flexibleWidth: 0);

            SearchInput = UIFactory.CreateInputField(Row2, "ItemSearchInput", ZGItemUtil.DefaultSearchText);
            ZGUIUtility.SetLayoutElement(SearchInput, minWidth: 180, minHeight: 40);
            SearchInput.OnValueChanged += (string val) => { SearchItem(val); };


            GameObject Row3 = UIFactory.CreateHorizontalGroup(this.ContentRoot, "Row3", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row3, minHeight: 40, flexibleWidth: 9999);

            var lable3 = UIFactory.CreateLabel(Row3, "Lable", "数量:", fontSize: ZGScriptTrainer.FontSize.Value);
            ZGUIUtility.SetLayoutElement(lable3, minHeight: 40, flexibleWidth: 0);

            var CountInput = UIFactory.CreateInputField(Row3, "ItemCountInput", "1");
            ZGUIUtility.SetLayoutElement(CountInput, minWidth: 180, minHeight: 40);
            CountInput.OnValueChanged += (string val) => { ChangeItemCount(val); };



            // Config entries

            ScrollPool = UIFactory.CreateScrollPool<ItemCell>(
                this.ContentRoot,
                "ZGItems",
                out GameObject scrollObj,
                out GameObject scrollContent);

            ScrollPool.Initialize(this);
        }
    }
}
