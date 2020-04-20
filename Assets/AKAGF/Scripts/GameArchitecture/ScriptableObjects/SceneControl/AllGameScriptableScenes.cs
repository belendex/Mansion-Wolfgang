using AKAGF.GameArchitecture.ScriptableObjects.Abstracts;

// This script works as a singleton asset.  That means that
// it is globally accessible through a static instance
// reference.  
namespace AKAGF.GameArchitecture.ScriptableObjects.SceneControl
{
    public class AllGameScriptableScenes : ScriptableSingleton<AllGameScriptableScenes> {

        public ScriptableScene[] scriptableScenes;
    
    }
}
