using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Interaction.Abstracts {

    /// <summary>
    /// This is the base class to create triggers that can be detected by the 
    /// playerInteractionSensor in order to interact with physicall interactables objects
    /// within a 2d or 3d environment.
    /// </summary>

    public abstract class InteractionTrigger : MonoBehaviour {

        /// <summary>
        /// The location and rotation of the interactable object where the interaction will start.
        /// This field is only used by agents that drives a character to a location, like in a 
        /// point & click game or npc interactions. 
        /// </summary>
        public Transform interactionLocation;   
        
        /// <summary>
        /// Determines if the interaction with the interactable object is trigger when 
        /// the character pass through the physicall trigger.
        /// </summary>
        public bool autoReact;

        /// <summary>
        /// Reference to the interactable object which interaction will be 
        /// triggered by this InteractionTrigger.
        /// </summary>
        public Interactable interactable;

        // GUI variables
        //public Vector2 outsideTextureOffset;                
        //public Texture2D outsideTexture;                    // Image to display in Interactable Position when player is outside
        //public Vector2 insideTextureTextureOffset;
        //public Texture2D insideTexture;
        //public Vector2 interactionSpinnerTexturesOffset;
        //public Texture2D[] interactionSpinnerTextures;


        private void Start() {
            if (!interactable && !(interactable = GetComponent<Interactable>())) {
                Debug.LogError("No Interactable reference or component found on: " + name + " GameObject.");
                enabled = false;
            }
        }

        public void React() {

            if (interactable.isInteracting)
                return;

            interactable.Interact();
            Debug.Log("Interacting with Interactable: " + interactable.gameObject.name);
        }

        //private void OnGUI() {
        //    if (!interactionLocation) {
        //        Debug.LogError("No Interactable Position found. Missing reference.");
        //        return;
        //    }
            
        //    // Get the worldToscreen position of the interactable
        //    Vector2 screenPosition = Camera.main.WorldToScreenPoint(interactionLocation.position);
 
        //    if (!insideTrigger && outsideTexture)
        //        GUI.DrawTexture(new Rect(screenPosition.x - (outsideTexture.width / 2) + outsideTextureOffset.x, 
        //                                (Screen.height - screenPosition.y) - (outsideTexture.height / 2) + outsideTextureOffset.y,
        //                                outsideTexture.width, outsideTexture.width),
        //                                outsideTexture, ScaleMode.ScaleToFit,
        //                                true,
        //                                0.0f);
        //    else if (insideTrigger && interactionSpinnerTextures.Length > 0) {
        //        GUI.DrawTexture(new Rect(screenPosition.x - (insideTexture.width / 2) + insideTextureTextureOffset.x, 
        //                                (Screen.height - screenPosition.y) - (insideTexture.height / 2) + insideTextureTextureOffset.y,
        //                                insideTexture.width, insideTexture.width),
        //                                insideTexture,
        //                                ScaleMode.ScaleToFit,
        //                                true,
        //                                0.0f);

        //        //get the corresponding texture array index
        //        float percentagePerImage = 100 / interactionSpinnerTextures.Length;
        //        float PressedTimePercentage =  (currentButtonPressedTime * 100) / buttonDownSecondsToInteract;
        //        int textureIndex = (int)(PressedTimePercentage / percentagePerImage);

        //        if (textureIndex == interactionSpinnerTextures.Length)
        //            textureIndex--;

        //        //adjusting alpha based on player's left health percetage
        //        //float alpha = (1 - (currentButtonPressedTime * 100 / buttonDownSecondsToInteract) / 100);

        //        //drawing the texture
        //        //GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        //        //GUI.depth = drawDepth;
        //        GUI.DrawTexture(new Rect(screenPosition.x - (interactionSpinnerTextures[textureIndex].width / 2) + interactionSpinnerTexturesOffset.x, 
        //                                (Screen.height - screenPosition.y) - (interactionSpinnerTextures[textureIndex].height / 2) + interactionSpinnerTexturesOffset.y,
        //                                interactionSpinnerTextures[textureIndex].width,
        //                                interactionSpinnerTextures[textureIndex].height),
        //                                interactionSpinnerTextures[textureIndex],
        //                                ScaleMode.ScaleToFit);
        //    }
        //}

    }
}
