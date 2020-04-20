using UnityEngine;

namespace AKAGF.GameArchitecture.ScriptableObjects.SceneControl
{
    [System.Serializable]
    public class ScriptableScene : ScriptableObject {

        public string scenePath;
        public string[] sceneStartingPositionsNames;

#if UNITY_EDITOR
        public bool isExpanded { get; set; } // If this variable is declared in the editor script the gui foldout doesn't work in custom window.
#endif
    }
}
