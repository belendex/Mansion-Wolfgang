using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Abstracts
{
    public class GenericMonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour {

        private static T _instance;
        public static T instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType<T>();
                    if (instance == null) {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        void Awake() {
            if (_instance == null) {
                //If I am the first instance, make me the Singleton
                _instance = this as T;
                DontDestroyOnLoad(this.gameObject);
            }
            else {
                //If a Singleton already exists and you find
                //another reference in scene, destroy it!
                Destroy(gameObject);
            }
        }
    }
}
