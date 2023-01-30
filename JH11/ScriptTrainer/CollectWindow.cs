using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityGameUI;
using VWVO.ConfigClass.Clothes;
using VWVO.ConfigClass.JH_UI;
using static DuloGames.UI.UITooltipLines;

namespace ScriptTrainer
{
    internal class CollectWindow
    {
        private static GameObject Panel;

        private static int initialX;

        private static int initialY;

        private static int elementX;

        private static int elementY;
        public CollectWindow(GameObject panel, int x, int y)
        {
            Panel = panel;
            initialX = x;
            elementX = x;
            elementY = y;
            initialY = y;
            Initialize();
        }

        public void Initialize()
        {
            AddH3("江湖录修改：", Panel);
            {
                AddButton("收集全部人物", Panel, CollectAllCharacter);

                AddButton("收集全部武学", Panel, CollectAllGonFa);

                AddButton("收集全部美食", Panel, CollectAllMeiShi);

                AddButton("收集全部诗词", Panel, CollectAllShiCi);

                AddButton("收集全部书画", Panel, CollectAllShuHua);
                hr(10);
                AddButton("收集全部书籍", Panel, CollectAllShuJi);

                AddButton("收集全部藏品", Panel, CollectAllCangPin);

                AddButton("收集全部服装", Panel, CollectAllClothes);
            }
        }

        private bool TryGetData()
        {
            bool flag = DataManager.Instance.GameData.GameRuntimeData.Player == null;
            return !flag;
        }

        #region[函数]

        public static void RefreshPanel(bool save = true)
        {
            UIManager acitveUIManager = UIManager.GetAcitveUIManager();
            if (acitveUIManager != null)
            {
                UIDataUtil uidataUtil = acitveUIManager.UIDataUtil;
                if (uidataUtil != null)
                {
                    uidataUtil.RefreshMainDHPanel();
                }
            }
            if(save)
                SaveManager.SaveCollected();
        }
        public void CollectAllCharacter()
        {
            if (TryGetData())
            {
                foreach (Collect collect in DataManager.Instance.GameData.GameRuntimeData.Collects)
                {
                    if(collect.Type == CollectType.Character)
                    {
                        if (!DataManager.Instance.GameData.GameCollectedData.Collects.Contains(collect.ID))
                        {
                            DataManager.Instance.GameData.GameCollectedData.NewCollected.Add(collect.ID);
                            DataManager.Instance.GameData.GameCollectedData.Collects.Add(collect.ID);
                        }
                    }
                }
                RefreshPanel();
            }
        }
        public void CollectAllGonFa()
        {
            if (TryGetData())
            {
                foreach (var item in DataManager.Instance.GameData.GameBasicData.JiNengBases)
                {
                    string collectID = item.ID;
                    ScriptTrainer.Instance.Log(collectID);
                    if (!DataManager.Instance.GameData.GameCollectedData.UnLockedGongFa.Contains(collectID))
                    {
                        DataManager.Instance.GameData.GameCollectedData.NewCollected.Add(collectID);
                        DataManager.Instance.GameData.GameCollectedData.UnLockedGongFa.Add(collectID);
                    }
                }
                RefreshPanel();
            }
        }
        public void CollectAllMeiShi()
        {
            if (TryGetData())
            {
                //foreach(string line in DataManager.Instance.GameData.GameCollectedData.UnLockedFood)
                //{
                //    DataManager.Instance.GameData.GameCollectedData.NewCollected.Remove(line);
                //}
                //DataManager.Instance.GameData.GameCollectedData.UnLockedFood.Clear();


                foreach (Food meishi in DataManager.Instance.GameData.GameBasicData.Foods)
                {
                    if (meishi.ItemType.Value == FoodType.CaiYao && !meishi.PeiFangs.IsNullOrEmpty<PeiFang>())
                    {
                        if (!DataManager.Instance.GameData.GameCollectedData.UnLockedFood.Contains(meishi.ID))
                        {
                            DataManager.Instance.GameData.GameCollectedData.NewCollected.Add(meishi.ID);
                            DataManager.Instance.GameData.GameCollectedData.UnLockedFood.Add(meishi.ID);
                        }
                        if (!DataManager.Instance.GameData.GameCollectedData.UnLockedCaiPu.Contains(meishi.ID))
                        {
                            DataManager.Instance.GameData.GameCollectedData.UnLockedCaiPu.Add(meishi.ID);
                        }
                    }
                }
                RefreshPanel();
            }
        }
        public void CollectAllShiCi()
        {
            if (TryGetData())
            {
                //foreach (string line in DataManager.Instance.GameData.GameCollectedData.UnLockedShiCi)
                //{
                //    DataManager.Instance.GameData.GameCollectedData.NewCollected.Remove(line);
                //}
                //DataManager.Instance.GameData.GameCollectedData.UnLockedShiCi.Clear();

                foreach (Poem poem in DataManager.Instance.GameData.GameBasicData.Poems)
                {
                    if (!poem.Conditions.IsNullOrEmpty<Condition>() && poem.ItemType.Value == PoemType.ShiCi)
                    {
                        if (!DataManager.Instance.GameData.GameCollectedData.UnLockedShiCi.Contains(poem.ID))
                        {
                            DataManager.Instance.GameData.GameCollectedData.NewCollected.Add(poem.ID);
                            DataManager.Instance.GameData.GameCollectedData.UnLockedShiCi.Add(poem.ID);
                        }
                        if (!DataManager.Instance.GameData.GameCollectedData.UnLockedShiCiLingGan.Contains(poem.ID))
                        {
                            DataManager.Instance.GameData.GameCollectedData.UnLockedShiCiLingGan.Add(poem.ID);
                        }
                    }
                }
                RefreshPanel();
            }
        }
        public void CollectAllShuHua()
        {
            if (TryGetData())
            {
                //foreach (string line in DataManager.Instance.GameData.GameCollectedData.UnLockedShuHua)
                //{
                //    DataManager.Instance.GameData.GameCollectedData.NewCollected.Remove(line);
                //}
                //DataManager.Instance.GameData.GameCollectedData.UnLockedShuHua.Clear();


                foreach (Art art in DataManager.Instance.GameData.GameBasicData.Arts)
                {
                    if (!art.Conditions.IsNullOrEmpty())
                    {
                        if (!DataManager.Instance.GameData.GameCollectedData.UnLockedShuHua.Contains(art.ID))
                        {
                            DataManager.Instance.GameData.GameCollectedData.NewCollected.Add(art.ID);
                            DataManager.Instance.GameData.GameCollectedData.UnLockedShuHua.Add(art.ID);
                        }
                        if (!DataManager.Instance.GameData.GameCollectedData.UnLockedShuHuaLingGan.Contains(art.ID))
                        {
                            DataManager.Instance.GameData.GameCollectedData.UnLockedShuHuaLingGan.Add(art.ID);
                        }
                    }
                }
                RefreshPanel();
            }
        }
        public void CollectAllShuJi()
        {
            if (TryGetData())
            {
                //foreach (string line in DataManager.Instance.GameData.GameCollectedData.UnLockedShuJi)
                //{
                //    ScriptTrainer.Instance.Log("ZG：删除：" + line);
                //    DataManager.Instance.GameData.GameCollectedData.NewCollected.Remove(line);
                //}
                //DataManager.Instance.GameData.GameCollectedData.UnLockedShuJi.Clear();

                foreach (Poem poem in DataManager.Instance.GameData.GameBasicData.Poems)
                {
                    if (!poem.ReadDisable && poem.ItemType.Value == PoemType.ShuJi)
                    {
                        if (!DataManager.Instance.GameData.GameCollectedData.UnLockedShuJi.Contains(poem.ID))
                        {
                            ScriptTrainer.Instance.Log("ZG：添加：" + poem.ID);
                            DataManager.Instance.GameData.GameCollectedData.NewCollected.Add(poem.ID);
                            DataManager.Instance.GameData.GameCollectedData.UnLockedShuJi.Add(poem.ID);
                        }
                    }
                }
                RefreshPanel();
            }
        }
        public void CollectAllCangPin()
        {
            if (TryGetData())
            {
                //foreach (string line in DataManager.Instance.GameData.GameCollectedData.UnLockedCangPin)
                //{
                //    DataManager.Instance.GameData.GameCollectedData.NewCollected.Remove(line);
                //}
                //DataManager.Instance.GameData.GameCollectedData.UnLockedCangPin.Clear();

                foreach (Antique antique in DataManager.Instance.GameData.GameBasicData.Antiques)
                {
                    if(antique.ItemType.Value > AntiqueType.Yan)
                    {
                        if (!DataManager.Instance.GameData.GameCollectedData.UnLockedCangPin.Contains(antique.ID))
                        {
                            DataManager.Instance.GameData.GameCollectedData.NewCollected.Add(antique.ID);
                            DataManager.Instance.GameData.GameCollectedData.UnLockedCangPin.Add(antique.ID);
                        }
                    }
                }
                RefreshPanel();
            }
        }
        public void CollectAllClothes()
        {
            if (TryGetData())
            {
                foreach(Clothes clothes in Config.Instance.Clothes.Male)
                {
                    if (!DataManager.Instance.GameData.GameCollectedData.CollectedClothes_Nan.Contains(clothes.Index))
                    {
                        DataManager.Instance.GameData.GameCollectedData.CollectedClothes_Nan.Add(clothes.Index);
                        DataManager.Instance.GameData.GameCollectedData.NewCollectedClothes_Nan.Add(clothes.Index);
                    }
                }
                foreach (Clothes clothes in Config.Instance.Clothes.Female)
                {
                    if (!DataManager.Instance.GameData.GameCollectedData.CollectedClothes_Nv.Contains(clothes.Index))
                    {
                        DataManager.Instance.GameData.GameCollectedData.CollectedClothes_Nv.Add(clothes.Index);
                        DataManager.Instance.GameData.GameCollectedData.NewCollectedClothes_Nv.Add(clothes.Index);
                    }
                }

                RefreshPanel();
            }
        }
        #endregion

        private static void hr()
        {
            elementX = initialX;
            elementY -= 60;
        }
        public void hr(int offsetX = 0, int offsetY = 0)
        {
            this.ResetCoordinates(true, false);
            elementX += offsetX;
            elementY -= 50 + offsetY;
        }
        public void ResetCoordinates(bool x, bool y = false)
        {
            if (x)
            {
                elementX = initialX;
            }
            if (y)
            {
                elementY = initialY;
            }
        }

        public GameObject AddH3(string text, GameObject panel)
        {
            elementX += 40;
            Sprite bgSprite = UIControls.createSpriteFrmTexture(UIControls.createDefaultTexture("#7AB900FF"));
            GameObject gameObject = UIControls.createUIText(panel, bgSprite, "#FFFFFFFF");
            gameObject.GetComponent<Text>().text = text;
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3((float)elementX, (float)elementY, 0f);
            gameObject.GetComponent<Text>().fontSize = 14;
            gameObject.GetComponent<Text>().fontStyle = FontStyle.Bold;
            this.hr(0, 0);
            elementY += 20;
            elementX += 10;
            return gameObject;
        }
        public static GameObject AddButton(string Text, GameObject panel, UnityAction action)
        {
            string backgroundColor = "#8C9EFFFF";
            Vector3 localPosition = new Vector3((float)elementX, (float)elementY, 0f);
            elementX += 110;
            GameObject gameObject = UIControls.createUIButton(panel, backgroundColor, Text, action, localPosition);
            gameObject.AddComponent<Shadow>().effectColor = UIControls.HTMLString2Color("#000000FF");
            gameObject.GetComponent<Shadow>().effectDistance = new Vector2(2f, -2f);
            gameObject.GetComponentInChildren<Text>().fontSize = 14;
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 30f);
            return gameObject;
        }
    }
}
