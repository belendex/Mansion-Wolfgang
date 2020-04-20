// This is an abstract MonoBehaviour that is the base class
// for all classes that want to save data to persist between
// scene loads and unloads.
// For an example of using this class, see the PositionSaver
// script.

using AKAGF.GameArchitecture.MonoBehaviours.SceneControl;
using AKAGF.GameArchitecture.ScriptableObjects.DataPersistence;

namespace AKAGF.GameArchitecture.MonoBehaviours.DataPersistence {
    [global::System.Serializable]
    public abstract class Saver /*: MonoBehaviour*/{

        public string uniqueIdentifier;             // A unique string set by a scene designer to identify what is being saved.
        public SaveData saveData;                   // Reference to the SaveData scriptable object where the data is stored.

        protected string key;                       // A string to identify what is being saved.  This should be set using information about the data as well as the uniqueIdentifier.


        public Saver() {
            uniqueIdentifier = GetHashCode().ToString();
        }

        // On enable
        public void subscribeToAction(ref SceneController sceneController) {
            // Subscribe the Save function to the BeforeSceneUnload event.
            sceneController.BeforeSceneUnload += Save;

            // Subscribe the Load function to the AfterSceneLoad event.
            sceneController.AfterSceneLoad += Load;
        }

        // On disable
        public void unsubscribeFromAction(ref SceneController sceneController) {
            // Unsubscribe the Save function from the BeforeSceneUnloud event.
            sceneController.BeforeSceneUnload -= Save;

            // Unsubscribe the Load function from the AfterSceneLoad event.
            sceneController.AfterSceneLoad -= Load;
        }


        // This function will be called in the custom editor of PersistentStateMasterSaver Script.
        // The key must be totally unique across all Saver scripts.
        public abstract void SetKey (string prefix);

        // This function is used only by ToString method of PersistentStateMasterSaver Script.
        public string GetKey() { return key; }

        // This function will be called just before a scene is unloaded.
        // It must call saveData.Save and pass in the key and the relevant data.
        protected abstract void Save ();

        // This function will be called just after a scene is finished loading.
        // It must call saveData.Load with a ref parameter to get the data out.
        protected abstract void Load ();
    }
}
