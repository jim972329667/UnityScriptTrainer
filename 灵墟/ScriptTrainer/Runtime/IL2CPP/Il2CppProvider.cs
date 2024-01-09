using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;

namespace ScriptTrainer.Runtime
{
    internal class Il2CppProvider : RuntimeHelper
    {

        internal delegate IntPtr d_LayerToName(int layer);

        internal delegate IntPtr d_FindObjectsOfTypeAll(IntPtr type);

        internal delegate void d_GetRootGameObjects(int handle, IntPtr list);

        internal delegate int d_GetRootCountInternal(int handle);

        protected internal override void OnInitialize()
        {
            
        }


        /// <inheritdoc/>
        protected internal override T Internal_AddComponent<T>(GameObject obj, Type type)
            => obj.AddComponent(Il2CppType.From(type)).TryCast<T>();

        /// <inheritdoc/>
        protected internal override ScriptableObject Internal_CreateScriptable(Type type)
            => ScriptableObject.CreateInstance(Il2CppType.From(type));


        /// <inheritdoc/>
        protected internal override string Internal_LayerToName(int layer)
        {
            d_LayerToName iCall = ICallManager.GetICall<d_LayerToName>("UnityEngine.LayerMask::LayerToName");
            return IL2CPP.Il2CppStringToManaged(iCall.Invoke(layer));
        }

        /// <inheritdoc/>
        protected internal override UnityEngine.Object[] Internal_FindObjectsOfTypeAll(Type type)
        {
            return new Il2CppReferenceArray<UnityEngine.Object>(
                    ICallManager.GetICallUnreliable<d_FindObjectsOfTypeAll>(
                        "UnityEngine.Resources::FindObjectsOfTypeAll",
                        "UnityEngine.ResourcesAPIInternal::FindObjectsOfTypeAll") // Unity 2020+ updated to this
                    .Invoke(Il2CppType.From(type).Pointer));
        }

        /// <inheritdoc/>
        protected internal override T[] Internal_FindObjectsOfTypeAll<T>()
        {
            return new Il2CppReferenceArray<T>(
                    ICallManager.GetICallUnreliable<d_FindObjectsOfTypeAll>(
                        "UnityEngine.Resources::FindObjectsOfTypeAll",
                        "UnityEngine.ResourcesAPIInternal::FindObjectsOfTypeAll") // Unity 2020+ updated to this
                    .Invoke(Il2CppType.From(typeof(T)).Pointer));
        }

        /// <inheritdoc/>
        protected internal override GameObject[] Internal_GetRootGameObjects(Scene scene)
        {
            if (!scene.isLoaded || scene.handle == -1)
                return new GameObject[0];

            int count = GetRootCount(scene.handle);
            if (count < 1)
                return new GameObject[0];

            Il2CppSystem.Collections.Generic.List<GameObject> list = new Il2CppSystem.Collections.Generic.List<GameObject>(count);
            ICallManager.GetICall<d_GetRootGameObjects>("UnityEngine.SceneManagement.Scene::GetRootGameObjectsInternal")
                .Invoke(scene.handle, list.Pointer);
            return list.ToArray();
        }

        /// <inheritdoc/>
        protected internal override int Internal_GetRootCount(Scene scene) => GetRootCount(scene.handle);

        /// <summary>
        /// Gets the <see cref="Scene.rootCount"/> for the provided scene handle.
        /// </summary>
        protected internal static int GetRootCount(int sceneHandle)
        {
            return ICallManager.GetICall<d_GetRootCountInternal>("UnityEngine.SceneManagement.Scene::GetRootCountInternal")
                   .Invoke(sceneHandle);
        }

    }
}
