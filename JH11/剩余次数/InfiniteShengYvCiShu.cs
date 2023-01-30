using BepInEx.Configuration;
using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace InfiniteShengYvCiShu
{
    [BepInPlugin("InfiniteShengYvCiShu", "江湖十一 无限剩余次数", "1.0.0.0")]
    public class InfiniteShengYvCiShu : BaseUnityPlugin
    {
        public static InfiniteShengYvCiShu Instance;
        public static ConfigEntry<bool> InfiniteIsOn { get; set; }
        public static ConfigEntry<int> ShengYvCiShu { get; set; }

        public GameObject YourTrainer;

        public void Awake()
        {
            Instance = this;

            #region 读取游戏配置
            InfiniteIsOn = this.Config.Bind("开启无限剩余次数", "InfiniteIsOn", true);
            ShengYvCiShu = this.Config.Bind("剩余次数", "ShengYvCiShu", 999);
            #endregion

            Log("加载无限剩余次数");
            YourTrainer = GameObject.Find("ZG_Trainer_InfiniteShengYvCiShu");
            bool flag = YourTrainer == null;
            if (flag)
            {
                YourTrainer = new GameObject("ZG_Trainer_InfiniteShengYvCiShu");
                DontDestroyOnLoad(YourTrainer);
                YourTrainer.hideFlags = HideFlags.HideAndDontSave;
                YourTrainer.AddComponent<ZGGameObject>();
            }
            else
            {
                YourTrainer.AddComponent<ZGGameObject>();
            }


        }

        public void Start()
        {

        }

        public void Update()
        {

        }
        public void FixedUpdate()
        {
        }
        public void Log(object message)
        {
            Logger.LogMessage(message);
        }
        public void OnDestroy()
        {
            // 移除 MainWindow.testAssetBundle 加载时的资源
            //AssetBundle.UnloadAllAssetBundles(true);

        }
    }

    public class ZGGameObject : MonoBehaviour
    {
        Action<object> Log = InfiniteShengYvCiShu.Instance.Log;
        bool IsAddPlayerRelationAction = false;
        bool IsAddPlayerRelationAction_GaoBai = false;
        bool IsAddPlayerRelationAction_QiuHun = false;
        public void Start()
        {
            
        }
        public void Update()
        {
            if (!InfiniteShengYvCiShu.InfiniteIsOn.Value)
                return;
            if (!IsAddPlayerRelationAction)
            {
                if(Config.Instance.PlayerRelationAction != null)
                {
                    int count = InfiniteShengYvCiShu.ShengYvCiShu.Value;
                    Config.Instance.PlayerRelationAction.XueXiJiYi.MeiYueXueXiJiYiCiShu = count;
                    Log("修改每月学习技艺次数成功");
                    Config.Instance.PlayerRelationAction.SongLi.MeiYueSongLiCiShu = count;
                    Log("修改每月送礼次数成功");
                    Config.Instance.PlayerRelationAction.TouQie.MeiHuiHeTouQieCiShu = count;
                    Log("修改每月偷窃次数成功");
                    Config.Instance.PlayerRelationAction.XiaDu.MeiHuiHeXiaDuCiShu = count;
                    Log("修改每月下毒次数成功");
                    Config.Instance.PlayerRelationAction.QieCuo.MeiYueQieCuoCiShu = count;
                    Log("修改每月切磋次数成功");
                    Config.Instance.PlayerRelationAction.XiaChuYanQing.MeiYueCiShu = count;
                    Log("修改每月下厨宴请次数成功");
                    Config.Instance.PlayerRelationAction.DuiYin.MeiYueCiShu = count;
                    Log("修改每月对饮次数成功");
                    Config.Instance.PlayerRelationAction.GongZou.MeiYueCiShu = count;
                    Log("修改每月共奏次数成功");
                    Config.Instance.PlayerRelationAction.XueXiJiYiPay.MeiYueXueXiJiYiCiShu = count;
                    Log("修改每月花钱学习技艺次数成功");
                    Config.Instance.PlayerRelationAction.DianCha.MeiYueCiShu = count;
                    Log("修改每月点茶次数成功");
                    
                    IsAddPlayerRelationAction = true;
                }
            }

            if (!IsAddPlayerRelationAction_GaoBai)
            {
                if (Config.Instance.PlayerRelationAction_GaoBai != null)
                {
                    int count = InfiniteShengYvCiShu.ShengYvCiShu.Value;
                    Config.Instance.PlayerRelationAction_GaoBai.GaoBai.MeiYueGaoBaiCiShu = count;
                    Log("修改每月告白次数成功");

                    IsAddPlayerRelationAction_GaoBai = true;
                }
            }
            if (!IsAddPlayerRelationAction_QiuHun)
            {
                if (Config.Instance.PlayerRelationAction_QiuHun != null)
                {
                    int count = InfiniteShengYvCiShu.ShengYvCiShu.Value;
                    Config.Instance.PlayerRelationAction_QiuHun.QiuHun.MeiYueQiuHunCiShu = count;
                    Log("修改每月求婚次数成功");

                    IsAddPlayerRelationAction_QiuHun = true;
                }
            }

            if (IsAddPlayerRelationAction && IsAddPlayerRelationAction_GaoBai && IsAddPlayerRelationAction_QiuHun)
            {
                Destroy(this);
            }
        }
    }
}
