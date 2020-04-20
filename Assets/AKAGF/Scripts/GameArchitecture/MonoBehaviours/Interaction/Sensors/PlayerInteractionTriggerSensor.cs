using System.Collections.Generic;
using AKAGF.GameArchitecture.MonoBehaviours.Interaction.Abstracts;
using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Interaction.Sensors {

    [RequireComponent(typeof(Collider))]
    public class PlayerInteractionTriggerSensor : PlayerInteractionSensor {

        private List<InteractionTrigger> interactionTriggersInRange = new List<InteractionTrigger>();
        private int currentInteractorIndex;

        private void Update() {

            if (interactionTriggersInRange.Count > 0) {

                int auxIndex = currentInteractorIndex;
                // Suppose that the closest object index is the currenInteractorIndex
                float closestObjectDistance = (transform.position - interactionTriggersInRange[currentInteractorIndex].interactionLocation.position).sqrMagnitude;

                // Set the current interactor Trigger based on the 
                // distance from the player to each interactor in range
                for (int i = 0; i < interactionTriggersInRange.Count; i++) {

                    float distanceToCheck = (transform.position - interactionTriggersInRange[currentInteractorIndex].interactionLocation.position).sqrMagnitude;

                    // Closer Interactable found
                    if (distanceToCheck < closestObjectDistance * closestObjectDistance) {
                        closestObjectDistance = distanceToCheck;
                        auxIndex = i;
                    }
                }

                // If current interactable changes, restart reaction timer
                if (currentInteractorIndex != auxIndex) {
                    currentInteractorIndex = auxIndex;
                    currentButtonPressedTime = 0;
                }

                currentInteractable = interactionTriggersInRange[currentInteractorIndex];
                // Handle timing reaction with the correct index
                timingInteraction(currentInteractable);
            }
        }


        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag(interactableTag)) {

                InteractionTrigger interAux = other.gameObject.GetComponent<InteractionTrigger>();

                if (interAux.autoReact) {
                    interAux.React();
                }
                else if(!interactionTriggersInRange.Exists(x => x.GetInstanceID() == interAux.GetInstanceID())) {
                    interactionTriggersInRange.Add(interAux);
                    Debug.Log("Interactable Added");
                }
                
            }
        }


        private void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag(interactableTag)) {

                InteractionTrigger interAux = other.gameObject.GetComponent<InteractionTrigger>();

                if (!interAux.autoReact) {
                    interactionTriggersInRange.Remove(interAux);
                    Debug.Log("Interactable Removed");
                }
            }
        }
    }
}
