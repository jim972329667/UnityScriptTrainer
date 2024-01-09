using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

namespace ScriptTrainer.Runtime
{
    public abstract class RuntimeHelper
    {
        internal static RuntimeHelper Instance { get; private set; }

        internal static void Init()
        {
            Instance = new Runtime.Il2CppProvider();

            Instance.OnInitialize();
        }

        protected internal abstract void OnInitialize();

      


        /// <summary>
        /// Helper to add a component of Type <paramref name="type"/>, and return it as Type <typeparamref name="T"/> (provided <typeparamref name="T"/> is assignable from <paramref name="type"/>).
        /// </summary>
        public static T AddComponent<T>(GameObject obj, Type type) where T : Component
            => Instance.Internal_AddComponent<T>(obj, type);

        protected internal abstract T Internal_AddComponent<T>(GameObject obj, Type type) where T : Component;


        protected internal abstract ScriptableObject Internal_CreateScriptable(Type type);


        protected internal abstract string Internal_LayerToName(int layer);

        /// <summary>
        /// Helper to invoke Unity's <see cref="Resources.FindObjectsOfTypeAll"/> method.
        /// </summary>
        public static T[] FindObjectsOfTypeAll<T>() where T : UnityEngine.Object
            => Instance.Internal_FindObjectsOfTypeAll<T>();

        /// <summary>
        /// Helper to invoke Unity's <see cref="Resources.FindObjectsOfTypeAll}"/> method.
        /// </summary>
        public static UnityEngine.Object[] FindObjectsOfTypeAll(Type type)
            => Instance.Internal_FindObjectsOfTypeAll(type);

        protected internal abstract T[] Internal_FindObjectsOfTypeAll<T>() where T : UnityEngine.Object;

        protected internal abstract UnityEngine.Object[] Internal_FindObjectsOfTypeAll(Type type);


        /// <summary>
        /// Helper to invoke Unity's <see cref="Scene.GetRootGameObjects"/> method.
        /// </summary>
        public static GameObject[] GetRootGameObjects(Scene scene)
            => Instance.Internal_GetRootGameObjects(scene);

        protected internal abstract GameObject[] Internal_GetRootGameObjects(Scene scene);

        /// <summary>
        /// Helper to get the value of Unity's <see cref="Scene.rootCount"/> property.
        /// </summary>
        public static int GetRootCount(Scene scene)
            => Instance.Internal_GetRootCount(scene);

        protected internal abstract int Internal_GetRootCount(Scene scene);



    }
}
