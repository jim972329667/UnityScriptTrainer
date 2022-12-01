using PrefabEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityGameUI;
using Zenject;
using static UnityEngine.UI.CanvasScaler;
using static UnityEngine.UI.Image;

namespace ScriptTrainer
{
    public class Scripts : MonoBehaviour
    {

        public static bool IsChangeMusic = false;
        public Scripts()
        {
        }
        public static void ChangeStormDelay(bool state)
        {
            ScriptPatch.IsChangeStormDelay = state;
        }
        public static void AddItem()
        {
            if(ScriptPatch.factory != null && ScriptPatch.pool != null && ScriptPatch.player != null)
            {
                var xx = ScriptPatch.pool.GetAllBy(null, null, null, null, Pool.IncludeRemoved.Yes, true).ToArray<PrefabEntity>();
                foreach(PrefabEntity x in xx)
                {
                    PrefabID prefabID = ScriptPatch.factory.Create<PrefabID>(x.prefab, new GameObjectCreationParameters
                    {
                        Position = ScriptPatch.player.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f))
                    }, null, null);

                }
            }
        }
    }
}
