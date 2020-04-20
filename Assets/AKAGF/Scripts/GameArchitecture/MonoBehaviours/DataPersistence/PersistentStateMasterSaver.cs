using System.Collections.Generic;
using AKAGF.GameArchitecture.MonoBehaviours.SceneControl;
using AKAGF.GameArchitecture.ScriptableObjects.DataPersistence;
using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.DataPersistence
{
    [global::System.Serializable]
    public class PersistentStateMasterSaver : MonoBehaviour {

        public SaveData saveData;
        public StateSaverElement[] saversData = new StateSaverElement[1];
        private SceneController sceneController;    // Reference to the SceneController so that this can subscribe to events that happen before and after scene loads.

        private void Awake() {
            // Find the SceneController and store a reference to it.
            sceneController = FindObjectOfType<SceneController>();

            // If the SceneController couldn't be found throw an exception so it can be added.
            if (!sceneController)
                throw new UnityException("Scene Controller could not be found, ensure that it exists in the Persistent scene.");

            if (!saveData) {
                Debug.LogWarning("No persistent SaveData reference found in the persistent master saver: " + name);
                enabled = false;
            }

            // Set the key based on information in inheriting classes.
            for (int i = 0; i < saversData.Length; i++) {
                saversData[i].initSavers(saveData);
            }
        }

        private void OnEnable(){

            for (int i = 0; i < saversData.Length; i++) {
                saversData[i].subscribeToSceneController(ref sceneController);
            }
        }


        private void OnDisable(){

            for (int i = 0; i < saversData.Length; i++) {
                saversData[i].unsubscribeFromSceneController(ref sceneController);
            }
        }
    }


    [global::System.Serializable]
    public class StateSaverElement {

        public string uniquePrefixID = "Unique Prefix ID";
        public GameObject persistentGameObject;
        

        public bool saveGameObjectState;
        public bool savePositionState;
        public bool saveRotationState;
        public bool saveBehaviourEnableState;

        public List<Behaviour> persistentGameObjectSelectedBehaviours = new List<Behaviour>();
        public int behaviorStatesflags = 0;

        public BehaviourEnableStateSaver[] behaviourEnableStateSavers;
        public GameObjectActivitySaver gameObjectActivitySaver;
        public PositionSaver positionSaver;
        public RotationSaver rotationSaver;

        public void subscribeToSceneController(ref SceneController sceneController) {

            if (saveBehaviourEnableState) {
                for (int i = 0; i < behaviourEnableStateSavers.Length; i++)
                    behaviourEnableStateSavers[i].subscribeToAction(ref sceneController);            
            }

            if (saveGameObjectState) {
                gameObjectActivitySaver.subscribeToAction(ref sceneController);
            }

            if (savePositionState) {
                positionSaver.subscribeToAction(ref sceneController);
            }

            if (savePositionState) {
                rotationSaver.subscribeToAction(ref sceneController);
            }
        }


        public void unsubscribeFromSceneController(ref SceneController sceneController) {
            if (saveBehaviourEnableState) {
                for (int i = 0; i < behaviourEnableStateSavers.Length; i++)
                    behaviourEnableStateSavers[i].unsubscribeFromAction(ref sceneController);            
            }

            if (saveGameObjectState) {
                gameObjectActivitySaver.unsubscribeFromAction(ref sceneController);
            }

            if (savePositionState) {
                positionSaver.unsubscribeFromAction(ref sceneController);
            }

            if (savePositionState) {
                rotationSaver.unsubscribeFromAction(ref sceneController);
            }

        }


        public void initSavers(SaveData saveData) {

            setKeys();
            setSaveData(saveData);
        }

        private void setSaveData(SaveData saveData) {

            if (saveBehaviourEnableState) {
                for (int i = 0; i < behaviourEnableStateSavers.Length; i++)
                    behaviourEnableStateSavers[i].saveData = saveData;
            }

            if (saveGameObjectState) {
                gameObjectActivitySaver.saveData = saveData;
            }

            if (savePositionState) {
                positionSaver.saveData = saveData;
            }

            if (savePositionState) {
                rotationSaver.saveData = saveData;
            }

        }

        private void setKeys() {

            if (saveBehaviourEnableState) {
                for (int i = 0; i < behaviourEnableStateSavers.Length; i++)
                    behaviourEnableStateSavers[i].SetKey(uniquePrefixID);
            }

            if (saveGameObjectState) {
                gameObjectActivitySaver.SetKey(uniquePrefixID);
            }

            if (savePositionState) {
                positionSaver.SetKey(uniquePrefixID);
            }

            if (savePositionState) {
                rotationSaver.SetKey(uniquePrefixID);
            }
        }


        public override string ToString() {

            string behaviours = "";

            for (int i = 0; i < behaviourEnableStateSavers.Length; i++) {
                behaviours += behaviourEnableStateSavers[i].behaviourToSave.name + ": " + behaviourEnableStateSavers[i].GetKey() + "\n";
            }

            return " uniquePrefixID: " + uniquePrefixID + "\n" +
                   "Behaviours Saved:" + behaviours +
                   "GameObjectActivitySaver: " + gameObjectActivitySaver.GetKey() + "\n" + 
                   "PositionSaver: " + positionSaver.GetKey() + "\n" +
                   "RotationSaver: " + rotationSaver.GetKey() + "\n";
        }
    }
}