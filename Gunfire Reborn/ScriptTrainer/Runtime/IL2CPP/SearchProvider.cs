using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Reflection;
using ScriptTrainer.Runtime;

namespace ScriptTrainer
{
    public enum SearchContext
    {
        UnityObject,
        Singleton,
        Class
    }

    public enum ChildFilter
    {
        Any,
        RootObject,
        HasParent
    }

    public enum SceneFilter
    {
        Any,
        ActivelyLoaded,
        DontDestroyOnLoad,
        HideAndDontSave,
    }

    public static class SearchProvider
    {
        private static bool Filter(Scene scene, SceneFilter filter)
        {
            bool result;
            switch (filter)
            {
                case SceneFilter.Any:
                    result = true;
                    break;
                case SceneFilter.ActivelyLoaded:
                    result = (scene.buildIndex != -1);
                    break;
                case SceneFilter.DontDestroyOnLoad:
                    result = (scene.handle == -12);
                    break;
                case SceneFilter.HideAndDontSave:
                    result = (scene == default(Scene));
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }
        internal static List<object> UnityObjectSearch(string input, string customTypeInput, ChildFilter childFilter, SceneFilter sceneFilter)
        {
            List<object> list = new List<object>();
            Type type = null;
            bool flag = !string.IsNullOrEmpty(customTypeInput);
            if (flag)
            {
                Type typeByName = ReflectionUtility.GetTypeByName(customTypeInput);
                bool flag2 = typeByName != null;
                if (flag2)
                {
                    bool flag3 = typeof(UnityEngine.Object).IsAssignableFrom(typeByName);
                    if (flag3)
                    {
                        type = typeByName;
                    }
                    else
                    {
                        Debug.LogWarning("Custom type '" + typeByName.FullName + "' is not assignable from UnityEngine.Object!");
                    }
                }
                else
                {
                    Debug.LogWarning("Could not find any type by name '" + customTypeInput + "'!");
                }
            }
            bool flag4 = type == null;
            if (flag4)
            {
                type = typeof(UnityEngine.Object);
            }
            UnityEngine.Object[] array = RuntimeHelper.FindObjectsOfTypeAll(type);
            string text = null;
            bool flag5 = !string.IsNullOrEmpty(input);
            if (flag5)
            {
                text = input;
            }
            bool flag6 = type == typeof(GameObject) || typeof(Component).IsAssignableFrom(type);
            foreach (UnityEngine.Object @object in array)
            {
                bool flag7 = !string.IsNullOrEmpty(text) && !@object.name.ContainsIgnoreCase(text);
                if (!flag7)
                {
                    GameObject gameObject = null;
                    Type actualType = @object.GetActualType();
                    bool flag8 = actualType == typeof(GameObject);
                    if (flag8)
                    {
                        gameObject = @object.TryCast<GameObject>();
                    }
                    else
                    {
                        bool flag9 = typeof(Component).IsAssignableFrom(actualType);
                        if (flag9)
                        {
                            Component component = @object.TryCast<Component>();
                            gameObject = ((component != null) ? component.gameObject : null);
                        }
                    }
                    bool flag10 = gameObject;
                    if (flag10)
                    {
                        bool flag11 = gameObject.transform.root.name == "UniverseLibCanvas";
                        if (flag11)
                        {
                            goto IL_240;
                        }
                        bool flag12 = flag6;
                        if (flag12)
                        {
                            bool flag13 = sceneFilter > SceneFilter.Any;
                            if (flag13)
                            {
                                bool flag14 = !SearchProvider.Filter(gameObject.scene, sceneFilter);
                                if (flag14)
                                {
                                    goto IL_240;
                                }
                            }
                            bool flag15 = childFilter > ChildFilter.Any;
                            if (flag15)
                            {
                                bool flag16 = !gameObject;
                                if (flag16)
                                {
                                    goto IL_240;
                                }
                                bool flag17 = childFilter == ChildFilter.HasParent && !gameObject.transform.parent;
                                if (flag17)
                                {
                                    goto IL_240;
                                }
                                bool flag18 = childFilter == ChildFilter.RootObject && gameObject.transform.parent;
                                if (flag18)
                                {
                                    goto IL_240;
                                }
                            }
                        }
                    }
                    list.Add(@object);
                }
            IL_240:;
            }
            return list;
        }

        internal static List<object> ClassSearch(string input)
        {
            List<object> list = new List<object>();

            string nameFilter = "";
            if (!string.IsNullOrEmpty(input))
                nameFilter = input;

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in asm.GetTypes())
                {
                    if (!string.IsNullOrEmpty(nameFilter) && !type.FullName.ContainsIgnoreCase(nameFilter))
                        continue;
                    list.Add(type);
                }
            }

            return list;
        }

        internal static string[] instanceNames = new string[]
        {
            "m_instance",
            "m_Instance",
            "s_instance",
            "s_Instance",
            "_instance",
            "_Instance",
            "instance",
            "Instance",
            "<Instance>k__BackingField",
            "<instance>k__BackingField",
        };

        internal static List<object> InstanceSearch(string input)
        {
            List<object> instances = new List<object>();

            string nameFilter = "";
            if (!string.IsNullOrEmpty(input))
                nameFilter = input;

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Search all non-static, non-enum classes.
                foreach (Type type in asm.GetTypes().Where(it => !(it.IsSealed && it.IsAbstract) && !it.IsEnum))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(nameFilter) && !type.FullName.ContainsIgnoreCase(nameFilter))
                            continue;

                        ReflectionUtility.FindSingleton(instanceNames, type, flags, instances);
                    }
                    catch { }
                }
            }

            return instances;
        }

    }
}
