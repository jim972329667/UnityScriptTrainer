using BepInEx;
using HarmonyLib;

using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using BepInEx.Configuration;
using Assets.Code;
using Assets.Code.Things;
using System.IO.Compression;
using System.Security.Cryptography;
using System;
using System.Collections;

namespace ScriptTrainer
{
    [BepInPlugin("ScriptTrainer.Jim97.MagicSkin", "魔法换肤", "1.0.0")]
    public class ScriptTrainer: BaseUnityPlugin
    {
        public static ScriptTrainer Instance;

        private bool Initialized = false;
        // 窗口相关
        public static Harmony harmony = null;
        private readonly GameObject root = new GameObject();
        private readonly List<GameSkin> Skins = new List<GameSkin>();
        private int SkinIndex = 0;
        private static readonly byte[] savedKey = { 0x7A, 0x67, 0x77, 0x64, 0x7A, 0x67, 0x7A, 0x73, 0x7A, 0x67, 0x62, 0x66, 0x61, 0x73, 0x64, 0x66 };
        //public static ConfigEntry<KeyCode> ChangeSkin { get; set; }
        public void Awake()
        {
            Instance = this;

            #region[注入游戏补丁]
            //harmony = new Harmony(Info.Metadata.GUID);
            //harmony.PatchAll();
            #endregion

            //ChangeSkin = Config.Bind("快捷键", "一键换肤", KeyCode.F9, "游戏内用于快速更换皮肤的按键。");
            //DontDestroyOnLoad(root);
            //root.SetActive(false);
            StartCoroutine(waiter());
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
        IEnumerator waiter()
        {
            // 等待4秒
            yield return new WaitForSeconds(4);

            // 执行下一个操作
            // ...
            LoadAsset();
        }
        public void Update()
        {
            if(!Initialized && Skins.Count > 0)
            {
                if(PlayerController.Instance != null)
                {
                    GameObject baseobj = PlayerController.Instance.gameObject;
                    Fish fish = baseobj.GetComponent<Fish>();
                    if (fish == null) return;
                    var randers = fish.GetDetailRenderers;

                    if (Skins[0].IsHaveBody)
                    {
                        foreach (Material material in randers[0].Renderer.materials)
                        {
                            if (Skins[0].BodyTexture[0] != null)
                                material.SetTexture("_Albeto", Skins[0].BodyTexture[0]);
                            if (Skins[0].BodyTexture[1] != null)
                                material.SetTexture("_Normal", Skins[0].BodyTexture[1]);
                            if (Skins[0].BodyTexture[2] != null)
                                material.SetTexture("_Roughness", Skins[0].BodyTexture[2]);
                            if (Skins[0].BodyTexture[3] != null)
                                material.SetTexture("_Occlusion", Skins[0].BodyTexture[3]);
                        }
                        Initialized = true;
                        Log("成功加载皮肤！");
                    }
                }
            }

            if(Skins.Count > 1)
            {
                if (Initialized && Input.GetKeyDown(KeyCode.F9))
                {
                    if (PlayerController.Instance != null)
                    {
                        SkinIndex++;
                        if (SkinIndex >= Skins.Count) SkinIndex = 0;
                        GameObject baseobj = PlayerController.Instance.gameObject;
                        Fish fish = baseobj.GetComponent<Fish>();
                        var randers = fish.GetDetailRenderers;
                        foreach (var renderer in randers)
                        {
                            if (renderer.name == "Plane")
                            {
                                if (Skins[SkinIndex].IsHaveBody)
                                {
                                    foreach (Material material in renderer.Renderer.materials)
                                    {
                                        if (Skins[SkinIndex].BodyTexture[0] != null)
                                            material.SetTexture("_Albeto", Skins[SkinIndex].BodyTexture[0]);
                                        if (Skins[SkinIndex].BodyTexture[1] != null)
                                            material.SetTexture("_Normal", Skins[SkinIndex].BodyTexture[1]);
                                        if (Skins[SkinIndex].BodyTexture[2] != null)
                                            material.SetTexture("_Roughness", Skins[SkinIndex].BodyTexture[2]);
                                        if (Skins[SkinIndex].BodyTexture[3] != null)
                                            material.SetTexture("_Occlusion", Skins[SkinIndex].BodyTexture[3]);
                                    }
                                    Log("换肤成功！");
                                }
                            }
                        }
                    }
                }
            }
        }
        private bool FixGame(ref GameObject gameObject)
        {
            //ClimberMain climberMain = gameObject.GetComponent<ClimberMain>();
            //GameObject hero = climberMain.body.transform.Find("HeroCharacter").gameObject;
            //if (hero == null) return false;
            //var control = climberMain.body.GetComponent<IKControl>();
            //if (control == null) return false;

            //GameObject newskin = Instantiate(MySkin);
            //SkinnedMeshRenderer myskin = newskin.GetComponentInChildren<SkinnedMeshRenderer>();
            //if (myskin == null) return false;

            //GameObject lhand = newskin.transform.Find("Armature/root/pelvis/spine_01/spine_02/spine_03/spine_04/spine_05/clavicle_l/upperarm_l/lowerarm_l/hand_l").gameObject;
            //if (lhand == null) return false;
            //GameObject rhand = newskin.transform.Find("Armature/root/pelvis/spine_01/spine_02/spine_03/spine_04/spine_05/clavicle_r/upperarm_r/lowerarm_r/hand_r").gameObject;
            //if (rhand == null) return false;
            ////设定初始模型位置
            //newskin.transform.SetPositionAndRotation(hero.transform.position, hero.transform.rotation);
            ////蒙皮定位
            //Traverse.Create(control).Field("bodyRenderer").SetValue(myskin);
            ////骨骼右手定位
            //Traverse.Create(control).Field("handBone_R").SetValue(rhand);
            ////骨骼左手定位
            //Traverse.Create(control).Field("handBone_L").SetValue(lhand);
            ////动画定位
            //Traverse.Create(control).Field("animator").SetValue(newskin.GetComponent<Animator>());
            //newskin.GetComponent<Animator>().runtimeAnimatorController = climberMain.body.GetComponent<Animator>().runtimeAnimatorController;
            //Destroy(hero);
            ////hero.SetActive(false);
            //newskin.transform.SetParent(climberMain.body.transform);
            //climberMain.bodyScript.bodyModel = newskin;
            return true;
        }
        private bool FixGame2(ref GameObject gameObject, GameSkin skin)
        {
            //ClimberMain climberMain = gameObject.GetComponent<ClimberMain>();
            //GameObject hero = climberMain.body.transform.Find("HeroCharacter").gameObject;
            //if (hero == null) return false;

            //GameObject BumCoverCloth = hero.transform.Find("BumCoverCloth").gameObject;
            //if (BumCoverCloth == null) return false;
            //BumCoverCloth.GetComponent<SkinnedMeshRenderer>().enabled = !skin.PresetData.hideCloth;
            //GameObject Body = hero.transform.Find("Body").gameObject;
            //if (Body == null) return false;

            //SkinnedMeshRenderer BodySkinMesh = Body.GetComponent<SkinnedMeshRenderer>();
            //Material[] materials = BodySkinMesh.materials;
            //if (!skin.PresetData.skinColor.IsNullOrWhiteSpace())
            //{
            //    if (ColorUtility.TryParseHtmlString(skin.PresetData.skinColor, out Color value))
            //    {
            //        for (int j = 0; j < materials.Length; j++)
            //        {
            //            materials[j].SetColor("_Color", value);
            //        }
            //    }
            //}


            //if (skin.ClothSkin != null)
            //{
            //    SkinnedMeshRenderer BumCoverClothComponent = BumCoverCloth.GetComponent<SkinnedMeshRenderer>();
            //    if (BumCoverClothComponent == null) return false;
            //    BumCoverClothComponent.material.mainTexture = skin.ClothSkin;
            //}
            //if (skin.IsHaveBody)
            //{
            //    foreach (Material material in BodySkinMesh.materials)
            //    {
            //        if (skin.BodyTexture[0] != null)
            //            material.SetTexture("_Albeto", skin.BodyTexture[0]);
            //        if (skin.BodyTexture[1] != null)
            //            material.SetTexture("_Normal", skin.BodyTexture[1]);
            //        if (skin.BodyTexture[2] != null)
            //            material.SetTexture("_Roughness", skin.BodyTexture[2]);
            //        if (skin.BodyTexture[3] != null)
            //            material.SetTexture("_Occlusion", skin.BodyTexture[3]);
            //    }
            //}

            //for (int i = 0; i < materials.Length; i++)
            //{
            //    materials[i].SetFloat("_SmoothAdd", skin.PresetData.bodySmoothness);
            //}

            //if (skin.CrownModel != null)
            //{
            //    Vector3 pos = new Vector3(skin.PresetData.crownPositionX, skin.PresetData.crownPositionY, skin.PresetData.crownPositionZ);
            //    Vector3 rot = new Vector3(skin.PresetData.crownRotationX, skin.PresetData.crownRotationY, skin.PresetData.crownRotationZ);

            //    var crownParent = hero.transform.Find("Armature/pelvis/spine01/spine02/spine03/neck/head/PaperCrown")?.gameObject;
            //    if (crownParent == null) return false;
            //    crownParent.SetActive(true);
            //    crownParent.transform.GetChild(0).gameObject.SetActive(false);
            //    if (crownParent.transform.childCount > 1)
            //    {
            //        Object.Destroy(crownParent.transform.GetChild(1).gameObject);
            //    }

            //    GameObject gameObject3 = Instantiate(skin.CrownModel);
            //    gameObject3.transform.SetParent(crownParent.transform);
            //    gameObject3.transform.localPosition = pos;
            //    gameObject3.transform.localScale = new Vector3(skin.PresetData.crownScale, skin.PresetData.crownScale, skin.PresetData.crownScale);
            //    gameObject3.transform.localRotation = Quaternion.Euler(rot);
            //}

            return true;
        }
        private void LoadAsset()
        {
            foreach(var file in Directory.GetFiles(Paths.PluginPath, "*.eskin", SearchOption.AllDirectories))
            {
                Log($"正在加载eskin文件:{file}");

                var skin = file.ReadSkinFile();

                if (skin != null)
                {
                    //skin.CrownModel?.transform.SetParent(root.transform);
                    Log($"成功加载eskin文件:{file}");
                    Skins.Add(skin);
                }

            }
        }
    }

}
