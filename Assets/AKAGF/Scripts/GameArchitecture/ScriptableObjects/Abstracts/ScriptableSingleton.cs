using System.Linq;
using UnityEngine;

namespace AKAGF.GameArchitecture.ScriptableObjects.Abstracts
{
    /// <summary>
    /// Abstract class for making reload-proof singletons out of ScriptableObjects
    /// Returns the asset created on the editor, or null if there is none
    /// Based on https://www.youtube.com/watch?v=VBA1QCoEAX4
    /// </summary>
    /// <typeparam name="T">Singleton type</typeparam>

    [ExecuteInEditMode]
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject {
        static T instance = null;
        static int numCalls = 0;
        public static T Instance {
            get {
                if (!instance) {
                    instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault() ;
                    numCalls++;
                }

#if UNITY_EDITOR
                if (!instance) {
                    string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).ToString());

                    if (guids.Length > 0) {
                        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                        instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                    }
                }
#endif

                if (numCalls > 2 && !instance)
                    Debug.LogWarning(typeof(T).ToString() + " asset has not been created yet.  Go to Assets > Create > " + typeof(T).ToString() + ".");

                return instance;
            }

            set { instance = value; numCalls = 0; }
        }
    }
}