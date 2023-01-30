using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections;
using System.Xml.Linq;
using UnityEngine;

namespace ScriptTrainer
{
    [BepInPlugin("aoe.top.plugins.ScriptTrainer", "SkyHill 内置修改器", "1.0.0.0")]
    public class ScriptTrainer : BaseUnityPlugin
    {
        // 窗口相关
        public GameObject YourName;
        // 启动按键
        public void Awake()
        {
            
        }

        public void Start()
        {
            #region[注入游戏补丁]
            Harmony.CreateAndPatchAll(typeof(ScriptPatch), "aoe.top.plugins.ScriptTrainer");
            #endregion

            YourName = new GameObject("YourName");
            GameObject.DontDestroyOnLoad(YourName);
            YourName.hideFlags = HideFlags.HideAndDontSave;
            YourName.AddComponent<MonoBehaviourExamples>();
            Debug.Log("脚本已启动");
        }

        public void OnDestroy()
        {
            // 移除 MainWindow.testAssetBundle 加载时的资源
            //AssetBundle.UnloadAllAssetBundles(true);

        }
    }
    public class MonoBehaviourExamples : MonoBehaviour
    {
        // Constructor needed to use Start, Update, etc...
        private void Start()
        {
            Debug.Log("脚本已启动");
        }


        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                ArrayList tmp = DbManager.instance.fetch("game_items");

                IEnumerator enumerator = tmp.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        object obj = enumerator.Current;
                        DbObject dbObject = (DbObject)obj;
                        string type = dbObject.getDataValue("type");
                        string name = dbObject.getDataValue("identity");
                        if (type == "material" || type == "food" || type == "medical")
                        {
                            if(!name.StartsWith("overdue"))
                                Character.getInstance().model.addInventoryItem(name, 20);
                        }
                        else if(type == "weapon")
                        {
                            Character.getInstance().model.addInventoryItem(name, 1);
                        }
                        Debug.Log($"ZG;{dbObject.getDataValue("identity")}");
                    }
                }
                finally
                {
                    IDisposable disposable;
                    if ((disposable = (enumerator as IDisposable)) != null)
                    {
                        disposable.Dispose();
                    }
                }
                Character.getInstance().model.addInventoryItem("coin", 200);
            }
            if (Input.GetKeyDown(KeyCode.F8))
            {
                DbObject specs = Character.getInstance().model.getSpecs();
                int num4 = Convert.ToInt32(specs.getDataValue("free_skill_points"));
                num4 += 100;
                specs.setDataValue("free_skill_points", num4);
                DbManager.instance.save(specs);
                Debug.Log("ZG:添加技能点！");
            }
            if (Input.GetKeyDown(KeyCode.F7))
            {
                Character.getInstance().model.health += (float)(100);
                Character.getInstance().model.food += (float)(100);
            }
            if (Input.GetKeyDown(KeyCode.F6))
            {
                ArrayList tmp = DbManager.instance.fetch("room_objects");

                IEnumerator enumerator = tmp.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        object obj = enumerator.Current;
                        DbObject dbObject = (DbObject)obj;
                        string state = dbObject.getDataValue("state");
                        string name = dbObject.getDataValue("name");
                        if (state == "live" )
                        {
                            dbObject.setDataValue("state", "dead_not_used");
                            DbManager.instance.save(dbObject);
                        }
                        if(name == "electricity_shield")
                        {
                            dbObject.setDataValue("state", "used");
                            DbManager.instance.save(dbObject);
                        }
                    }
                }
                finally
                {
                    IDisposable disposable;
                    if ((disposable = (enumerator as IDisposable)) != null)
                    {
                        disposable.Dispose();
                    }
                }

            }
        }

    }
}
