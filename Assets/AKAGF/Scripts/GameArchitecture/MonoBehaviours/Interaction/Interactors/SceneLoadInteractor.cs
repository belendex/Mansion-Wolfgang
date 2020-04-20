using AKAGF.GameArchitecture.MonoBehaviours.SceneControl;
using UnityEngine;


namespace AKAGF.GameArchitecture.MonoBehaviours.Interaction.Interactors {
    [RequireComponent(typeof(Interactable))]
    public class SceneLoadInteractor : MonoBehaviour {

        public enum SCENE_LOAD_EVENT { AFTER_SCENE_LOAD, BEFORE_SCENE_UNLOAD}
        public SCENE_LOAD_EVENT sceneLoadEvent;
        public float delay = 1f;

        private SceneController sceneController;    // Reference to the SceneController so that this Interactor can be subscribed to events that happen after scene loads.
        private Interactable sceneLoadingInteractable;
        


        void Awake() {
            //// Find the SceneController and store a reference to it.
            sceneController = FindObjectOfType<SceneController>();

            //// If the SceneController couldn't be found throw an exception so it can be added.
            if (!sceneController)
               Debug.LogWarning("Scene Controller could not be found, ensure that it exists in the Persistent scene.");

            if (!(sceneLoadingInteractable = GetComponent<Interactable>())) {
                Debug.LogError("No Interactable Component found on: " + name + " gameObject.");
                enabled = false;
            }
            
        }


        private void OnEnable() {

            if (!sceneController) 
                return;

            if (sceneLoadEvent == SCENE_LOAD_EVENT.AFTER_SCENE_LOAD) {
                // Subscribe the sceneLoading function to the AfterSceneLoad event.
                sceneController.AfterSceneLoad += sceneLoading;
            }
            else if (sceneLoadEvent == SCENE_LOAD_EVENT.BEFORE_SCENE_UNLOAD) {
                // Subscribe the sceneLoading function to the AfterSceneLoad event.
                sceneController.BeforeSceneUnload += sceneLoading;
            }
                
        }

        private void OnDisable() {

            if (!sceneController)
                return;

            if (sceneLoadEvent == SCENE_LOAD_EVENT.AFTER_SCENE_LOAD) {
                // Unsubscribe the sceneLoading function from the AfterSceneLoad event.
                sceneController.AfterSceneLoad -= sceneLoading;
            }
            else if (sceneLoadEvent == SCENE_LOAD_EVENT.BEFORE_SCENE_UNLOAD) {
                // Unsubscribe the sceneLoading function from the BeforeSceneUnLoad event.
                sceneController.BeforeSceneUnload -= sceneLoading;
            }

        }

        public void sceneLoading() {
            sceneLoadingInteractable.delayedInteract(delay);
        }

    }
}
