using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Interaction.Abstracts {

    /// <summary>
    /// This is the base class to make a 2d or 3d trigger sensor of Interactables objects 
    /// that is physically "carried" by the character that can interacts with them.
    /// The sensor detection volume/surface is defined by a collider/2dCollider marked as
    /// trigger.
    /// </summary>
    public abstract class PlayerInteractionSensor : MonoBehaviour {

        /// <summary>
        /// The Tag that interactable objects must have in order to be recognized as such.
        /// </summary>
        public string interactableTag = "Interactable";

        /// <summary>
        /// Total button pressing time needed to interact
        /// </summary>
        public float buttonDownSecondsToInteract = 1f;      // 

        /// <summary>
        /// Current button pressing time
        /// </summary>
        public float currentButtonPressedTime { get; protected set; }       

        /// <summary>
        /// Interaction bool that must be setted by an input button.
        /// </summary>
        public bool interactionButton { get; set; }


        /// <summary>
        /// The current interactable present in the sensor.
        /// </summary>
        public InteractionTrigger currentInteractable { get; protected set; }        // Cached InteractableTrigger


        protected void timingInteraction(InteractionTrigger interactionTrigger) {
            if (interactionButton)
                currentButtonPressedTime += Time.deltaTime;
            else
                currentButtonPressedTime = (currentButtonPressedTime <= 0) ? 0 : currentButtonPressedTime - Time.deltaTime;

            if (currentButtonPressedTime >= buttonDownSecondsToInteract) {
                interactionTrigger.React();
                currentButtonPressedTime = 0;
            }
        }
 
    }
}
