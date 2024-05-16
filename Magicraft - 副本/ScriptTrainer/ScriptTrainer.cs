using BepInEx;
using HarmonyLib;

using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using BepInEx.Configuration;

namespace ScriptTrainer
{
    [BepInPlugin("ScriptTrainer.Jim97.MagicSkin", "魔法换肤", "1.0.0")]
    public class ScriptTrainer: BaseUnityPlugin
    {
        public static ScriptTrainer Instance;

        private bool Initialized = false;
        // 窗口相关
        public static Harmony harmony = null;
        private readonly GameObject MySkin = null;
        private readonly GameObject root = new GameObject();
        private readonly List<GameSkin> Skins = new List<GameSkin>();
        private int SkinIndex = 0;
        //public static ConfigEntry<KeyCode> ChangeSkin { get; set; }
        public void Awake()
        {
            Instance = this;

            #region[注入游戏补丁]
            //harmony = new Harmony(Info.Metadata.GUID);
            //harmony.PatchAll();
            #endregion

            //ChangeSkin = Config.Bind("快捷键", "一键换肤", KeyCode.F9, "游戏内用于快速更换皮肤的按键。");
            DontDestroyOnLoad(root);
            root.SetActive(false);

            LoadAsset();
        }
        public void Log(object message, LogType logType = LogType.Log)
        {
            string text = (message?.ToString()) ?? "";
            switch (logType)
            {
                case 0:
                case (LogType)4:
                    Logger.LogMessage(text);
                    break;
                case (LogType)1:
                case (LogType)3:
                    Logger.LogMessage(text);
                    break;
                case (LogType)2:
                    Logger.LogMessage(text);
                    break;
            }
        }
        public void Start()
        {
            
        }

        public void Update()
        {
            if (!Initialized && MySkin != null)
            {
                PlayerSpawn playerSpawn = FindObjectOfType<PlayerSpawn>();
                if (playerSpawn != null)
                {
                    //游戏现有的模型
                    GameObject p = Traverse.Create(playerSpawn).Field("p").GetValue<GameObject>();
                    //游戏基类模型
                    GameObject player = Traverse.Create(playerSpawn).Field("player").GetValue<GameObject>();
                    if (p == null || player == null) return;

                    if (!FixGame(ref player))
                        return;
                    if(!FixGame(ref p))
                        return;

                    //因为模型是私有变量所以要用Traverse
                    Traverse.Create(playerSpawn).Field("p").SetValue(p);
                    Traverse.Create(playerSpawn).Field("player").SetValue(player);
                    Log("成功替换模型");
                    Initialized = true;
                }
            }

            if(!Initialized && Skins.Count > 0)
            {
                PlayerSpawn playerSpawn = FindObjectOfType<PlayerSpawn>();
                if (playerSpawn != null)
                {
                    SkinIndex = 0;
                    //游戏现有的模型
                    GameObject p = Traverse.Create(playerSpawn).Field("p").GetValue<GameObject>();
                    //游戏基类模型
                    GameObject player = Traverse.Create(playerSpawn).Field("player").GetValue<GameObject>();
                    if (p == null || player == null) return;

                    if (!FixGame2(ref player, Skins[SkinIndex]))
                        return;
                    if (!FixGame2(ref p, Skins[SkinIndex]))
                        return;

                    //因为模型是私有变量所以要用Traverse
                    Traverse.Create(playerSpawn).Field("p").SetValue(p);
                    Traverse.Create(playerSpawn).Field("player").SetValue(player);

                    Log("成功替换模型");
                    Initialized = true;
                }
            }

            if(Skins.Count > 1)
            {
                if (Initialized && Input.GetKeyDown(KeyCode.F9))
                {
                    PlayerSpawn playerSpawn = FindObjectOfType<PlayerSpawn>();
                    if (playerSpawn != null)
                    {
                        SkinIndex++;
                        if (SkinIndex >= Skins.Count) SkinIndex = 0;
                        //游戏现有的模型
                        GameObject p = Traverse.Create(playerSpawn).Field("p").GetValue<GameObject>();
                        //游戏基类模型
                        GameObject player = Traverse.Create(playerSpawn).Field("player").GetValue<GameObject>();
                        if (p == null || player == null) return;

                        if (!FixGame2(ref player, Skins[SkinIndex]))
                            return;
                        if (!FixGame2(ref p, Skins[SkinIndex]))
                            return;

                        //因为模型是私有变量所以要用Traverse
                        Traverse.Create(playerSpawn).Field("p").SetValue(p);
                        Traverse.Create(playerSpawn).Field("player").SetValue(player);

                        Log("成功替换模型");
                    }
                }
            }
        }
        private bool FixGame(ref GameObject gameObject)
        {
            ClimberMain climberMain = gameObject.GetComponent<ClimberMain>();
            GameObject hero = climberMain.body.transform.Find("HeroCharacter").gameObject;
            if (hero == null) return false;
            var control = climberMain.body.GetComponent<IKControl>();
            if (control == null) return false;

            GameObject newskin = Instantiate(MySkin);
            SkinnedMeshRenderer myskin = newskin.GetComponentInChildren<SkinnedMeshRenderer>();
            if (myskin == null) return false;

            GameObject lhand = newskin.transform.Find("Armature/root/pelvis/spine_01/spine_02/spine_03/spine_04/spine_05/clavicle_l/upperarm_l/lowerarm_l/hand_l").gameObject;
            if (lhand == null) return false;
            GameObject rhand = newskin.transform.Find("Armature/root/pelvis/spine_01/spine_02/spine_03/spine_04/spine_05/clavicle_r/upperarm_r/lowerarm_r/hand_r").gameObject;
            if (rhand == null) return false;
            //设定初始模型位置
            newskin.transform.SetPositionAndRotation(hero.transform.position, hero.transform.rotation);
            //蒙皮定位
            Traverse.Create(control).Field("bodyRenderer").SetValue(myskin);
            //骨骼右手定位
            Traverse.Create(control).Field("handBone_R").SetValue(rhand);
            //骨骼左手定位
            Traverse.Create(control).Field("handBone_L").SetValue(lhand);
            //动画定位
            Traverse.Create(control).Field("animator").SetValue(newskin.GetComponent<Animator>());
            newskin.GetComponent<Animator>().runtimeAnimatorController = climberMain.body.GetComponent<Animator>().runtimeAnimatorController;
            Destroy(hero);
            //hero.SetActive(false);
            newskin.transform.SetParent(climberMain.body.transform);
            climberMain.bodyScript.bodyModel = newskin;
            return true;
        }
        private bool FixGame2(ref GameObject gameObject, GameSkin skin)
        {
            ClimberMain climberMain = gameObject.GetComponent<ClimberMain>();
            GameObject hero = climberMain.body.transform.Find("HeroCharacter").gameObject;
            if (hero == null) return false;

            GameObject BumCoverCloth = hero.transform.Find("BumCoverCloth").gameObject;
            if (BumCoverCloth == null) return false;
            BumCoverCloth.GetComponent<SkinnedMeshRenderer>().enabled = !skin.PresetData.hideCloth;
            GameObject Body = hero.transform.Find("Body").gameObject;
            if (Body == null) return false;

            SkinnedMeshRenderer BodySkinMesh = Body.GetComponent<SkinnedMeshRenderer>();
            Material[] materials = BodySkinMesh.materials;
            if (!skin.PresetData.skinColor.IsNullOrWhiteSpace())
            {
                if (ColorUtility.TryParseHtmlString(skin.PresetData.skinColor, out Color value))
                {
                    for (int j = 0; j < materials.Length; j++)
                    {
                        materials[j].SetColor("_Color", value);
                    }
                }
            }
            if (skin.ClothSkin != null)
            {
                SkinnedMeshRenderer BumCoverClothComponent = BumCoverCloth.GetComponent<SkinnedMeshRenderer>();
                if (BumCoverClothComponent == null) return false;
                BumCoverClothComponent.material.mainTexture = skin.ClothSkin;
            }
            if (skin.IsHaveBody)
            {
                foreach (Material material in BodySkinMesh.materials)
                {
                    if (skin.BodyTexture[0] != null)
                        material.SetTexture("_Albeto", skin.BodyTexture[0]);
                    if (skin.BodyTexture[1] != null)
                        material.SetTexture("_Normal", skin.BodyTexture[1]);
                    if (skin.BodyTexture[2] != null)
                        material.SetTexture("_Roughness", skin.BodyTexture[2]);
                    if (skin.BodyTexture[3] != null)
                        material.SetTexture("_Occlusion", skin.BodyTexture[3]);
                }
            }

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetFloat("_SmoothAdd", skin.PresetData.bodySmoothness);
            }
            
            if (skin.CrownModel != null)
            {
                Vector3 pos = new Vector3(skin.PresetData.crownPositionX, skin.PresetData.crownPositionY, skin.PresetData.crownPositionZ);
                Vector3 rot = new Vector3(skin.PresetData.crownRotationX, skin.PresetData.crownRotationY, skin.PresetData.crownRotationZ);

                var crownParent = hero.transform.Find("Armature/pelvis/spine01/spine02/spine03/neck/head/PaperCrown")?.gameObject;
                if (crownParent == null) return false;
                crownParent.SetActive(true);
                crownParent.transform.GetChild(0).gameObject.SetActive(false);
                if(crownParent.transform.childCount > 1)
                {
                    Object.Destroy(crownParent.transform.GetChild(1).gameObject);
                }

                GameObject gameObject3 = Instantiate(skin.CrownModel);
                gameObject3.transform.SetParent(crownParent.transform);
                gameObject3.transform.localPosition = pos;
                gameObject3.transform.localScale = new Vector3(skin.PresetData.crownScale, skin.PresetData.crownScale, skin.PresetData.crownScale);
                gameObject3.transform.localRotation = Quaternion.Euler(rot);
            }

            return true;
        }
        private void LoadAsset()
        {
            //Log($"开始获取AssetBundle");
            //string resourceName = Assembly.GetExecutingAssembly().GetManifestResourceNames().First(n => n.EndsWith(".untitle"));
            //// 使用资源流创建一个临时文件
            //using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            //{
            //    var ass = AssetBundle.LoadFromStream(stream);
            //    var tmp2 = ass.LoadAsset<GameObject>("untitled");
            //    if (tmp2 != null)
            //    {
            //        Log($"成功替换MySkin");
            //        //MySkin = tmp2;
            //    }
            //}
            //UpdateZipAndOutputAsByteArray(Paths.PluginPath + "/Mario-Set.zip","111.txt",Encoding.UTF8.GetBytes("121"));
            
            //LoadFilesFromZip(Paths.PluginPath + "/Mario-Set.zip");

            foreach(var file in Directory.GetFiles(Paths.PluginPath, "*.eskin", SearchOption.AllDirectories))
            {
                var skin = file.ReadSkinFile();
                if( skin != null)
                {
                    skin.CrownModel?.transform.SetParent(root.transform);
                    Skins.Add(skin);
                }
                    
            }

            //writeFile();

            //ReadFile(Paths.PluginPath + "/Mario-Set.eskin");
            //    if (File.Exists(Paths.PluginPath + "/untitle"))
            //{
            //    var ass = AssetBundle.LoadFromFile(Paths.PluginPath + "/untitle");
            //    Log($"AssetBundle = {ass}");
            //    var tmp2 = ass.LoadAsset<GameObject>("untitled");
            //    var tmp3 = ass.LoadAllAssets();
            //    foreach ( var item in tmp3)
            //    {
            //        Log($"资源文件：{item}");
            //    }

            //    if(tmp2 != null)
            //    {
            //        Log($"成功替换MySkin");
            //        MySkin = tmp2;
            //    }
            //}
        }
    }

}
