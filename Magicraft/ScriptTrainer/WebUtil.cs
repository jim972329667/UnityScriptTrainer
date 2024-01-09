using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json;

namespace ScriptTrainer
{
    public class ModData
    {
        public string id { get; set; }
        public string mods_version { get; set; }
    }
    public class WebUtil : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(GetText());
        }

        IEnumerator GetText()
        {
            UnityWebRequest www = UnityWebRequest.Get("https://mod.3dmgame.com/mod/API/204543");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                ScriptTrainer.Instance.Log(www.error);
            }
            else
            {
                // 以文本形式显示结果
                //ScriptTrainer.Instance.Log(www.downloadHandler.text);
                var data = JsonConvert.DeserializeObject<ModData>(www.downloadHandler.text);
                if(data?.mods_version != null)
                {
                    Version version = new Version(data.mods_version);
                    if(version > ScriptTrainer.Instance.Info.Metadata.Version)
                    {
                        MainWindow.CheckUpdate = true;
                        ScriptTrainer.Instance.Log("检查更新！");
                    }
                }
                // 或者获取二进制数据形式的结果
                byte[] results = www.downloadHandler.data;
            }
        }

    }
}
