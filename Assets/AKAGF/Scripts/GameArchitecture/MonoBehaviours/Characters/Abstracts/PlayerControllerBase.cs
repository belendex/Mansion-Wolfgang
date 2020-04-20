using System.Collections;

using AKAGF.GameArchitecture.MonoBehaviours.SceneControl;
using AKAGF.GameArchitecture.ScriptableObjects.DataPersistence;
using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Characters.Abstracts
{
    public abstract class PlayerControllerBase : MonoBehaviour {

        public SaveData playerSaveData;                                  // Reference to the save data asset containing the player's starting position.
        
        public bool handleInput { get; set; }                            // Boolean to handle the player Input
        public float inputHoldDelay = 0.5f;                              // How long after reaching an interactable before input is allowed again.
        protected WaitForSeconds inputHoldWait;                          // The WaitForSeconds used to make the user wait before input is handled again.


        protected virtual void Start() {

            handleInput = false;
            // Create the wait based on the delay.
            inputHoldWait = new WaitForSeconds(inputHoldDelay);

            string startingPositionName = "";
            Transform startingPosition = null;

            if (!playerSaveData) {
                Debug.LogWarning("No PlayerSaveData Asset attached to Player controller: " + name);
            }
            else {
                //Load the starting position from the save data and find the transform from the starting position's name.
                playerSaveData.Load(StartingPosition.startingPositionKey, ref startingPositionName);
                startingPosition = StartingPosition.FindStartingPosition(startingPositionName);
            }

            
            //Set the player's position based on the starting position.
            if (startingPosition) {
                transform.position = startingPosition.position;
                transform.rotation = startingPosition.rotation;
            }
            else if (!startingPositionName.Equals("")) {
                Debug.LogWarning("No starting position found with name " + startingPositionName);
            }

            StartCoroutine(holdInput());
        }

        private IEnumerator holdInput() {

            yield return inputHoldWait;
            handleInput = true;
        }

    }
}
