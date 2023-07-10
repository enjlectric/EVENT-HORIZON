using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Enjlectric.ScriptableData.Editor
{
    /// <summary>
    /// Resets values using the AutoResetAttribute based on whether or not Production values should be used in testing. Only takes effect in the Editor.
    /// </summary>
    [ExecuteAlways]
    public class ResetScriptableObjects : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            Reset(true);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void ResetVariables()
        {
            Reset(EditorPrefs.GetBool("ScriptableDataUseProductionValues", false));
        }

        private static void Reset(bool production)
        {
            Dictionary<Type, object[]> loaded = new Dictionary<Type, object[]>();
            foreach (Type type in Assembly.GetAssembly(typeof(AutoResetAttribute)).GetTypes())
            {
                foreach (var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    var attr = field.GetCustomAttribute<AutoResetAttribute>();

                    if (attr != null)
                    {
                        if (!loaded.ContainsKey(type))
                        {
                            loaded[type] = GetAllInstances(type);
                        }

                        foreach (var obj in loaded[type])
                        {
                            var source = obj.GetType().GetField(production ? attr.copySourceName : attr.copySourceTestName, BindingFlags.NonPublic | BindingFlags.Instance);
                            var val = source.GetValue(obj);
                            var fieldname = obj.GetType().GetField(field.Name, BindingFlags.NonPublic | BindingFlags.Instance);
                            switch (val)
                            {
                                case ScriptableObject:
                                case MonoBehaviour:
                                case GameObject:
                                    fieldname.SetValue(obj, Activator.CreateInstance(val.GetType(), val));
                                    break;

                                default:
                                    fieldname.SetValue(obj, val);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private static object[] GetAllInstances(Type t)
        {
            string[] guids = AssetDatabase.FindAssets("t:" + t.Name);  //FindAssets uses tags check documentation for more info
            object[] a = new object[guids.Length];
            for (int i = 0; i < guids.Length; i++)         //probably could get optimized
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath(path, typeof(object));
            }

            return a;
        }
    }
}