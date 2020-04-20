using System.Linq;
using UnityEngine;

namespace AKAGF.GameArchitecture.ScriptableObjects.Abstracts
{
    [ExecuteInEditMode]
    public abstract class ResettableScriptableSingleton<T> : ResettableScriptableObject where T : ResettableScriptableObject {
        static T instance = null;
        static int numCalls = 0;
        public static T Instance {
            get {
                if (!instance) {
                    instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
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
                    Debug.LogError(typeof(T).ToString() + " asset has not been created yet.  Go to Assets > Create > " + typeof(T).ToString() + ".");

                return instance;
            }

            set { instance = value; numCalls = 0; }
        }
    }
}
