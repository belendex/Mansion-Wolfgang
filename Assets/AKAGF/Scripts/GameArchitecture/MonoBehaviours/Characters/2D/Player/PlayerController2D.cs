using AKAGF.GameArchitecture.MonoBehaviours.Characters.Abstracts;
using AKAGF.GameArchitecture.MonoBehaviours.Interaction.Abstracts;
using UnityEngine;
using UnityEngine.EventSystems;


namespace AKAGF.GameArchitecture.MonoBehaviours.Characters._2D.Player {
    [RequireComponent (typeof(PlayerMovement2D))]
    [RequireComponent (typeof(PlayerAnimation2D))]
    public class PlayerController2D : PlayerControllerWalkablePnC {

        public PolygonCollider2D walkablePolCollider;
        private PlayerMovement2D playerMovement;
        private PlayerAnimation2D playerAnimation;


        protected override void Start() {

            base.Start();

            if (!walkablePolCollider) {
                Debug.LogError("No walkable Polygon Collider reference found.");
                enabled = false;
                return;
            }

            playerMovement = GetComponent<PlayerMovement2D>();
            playerAnimation = GetComponent<PlayerAnimation2D>();

        }

        private void Update() {

            //handleMovement
            playerMovement.handleMovement();
            //handleAnimation
            playerAnimation.handleAnimation(playerMovement.destinationPosition.position, playerMovement.isMoving, playerMovement.velocity);

            //handle interaction
            if (currentInteractable != null && transform.position == currentInteractable.interactionLocation.position) {
                Debug.Log("Interacting with " + currentInteractable.gameObject.name);
                currentInteractable.React();
                currentInteractable = null;
            }
        }


        // This function was called by the EventTrigger on the scene's walkable surface when it is clicked on, now it's called by the OnClick Events.
        public override void OnWalkableLayerClick(PointerEventData data, int numCliks) {

            //If the handle input flag is set to false then do nothing.
            if (!handleInput)
                return;

            //The player doesn't click on an interactable GameObject
            currentInteractable = null;


            //Set the player destination to clicked position
            playerMovement.destinationPosition.position = data.pointerCurrentRaycast.screenPosition;
        }

        //Player clicks/taps on the non walkable surface
        public override void OnUnWalkableLayerClick(PointerEventData data, int numCliks) {

            //If the handle input flag is set to false then do nothing.
            if (!handleInput)
                return;

            //The player didn't click on an interactable GameObject
            currentInteractable = null;

            //Here we have two approachs. 
            //First option, if the player clicks in the non_walkable layer, we can make the character says something like "I can't go there".
            //Second option, we can calculate the walkable layer nearest point to the player click point, and move the character there. 

            //second option   
            RaycastHit2D hit = Physics2D.Raycast(data.pointerCurrentRaycast.worldPosition, Vector2.down, Mathf.Infinity, 1 << LayerMask.NameToLayer("Walkable"));

            //We found the nearest point to the player click point in the walkable layer
            if (hit) {
                playerMovement.destinationPosition.position = hit.point;
            }
            else {
                //Make the character says that the point clicked/taped by the player is imposible to reach
                //TODO conversation system
                print("I can't go there.");
            }
        }

        //This function is called by the EventTrigger on an Interactable, the Interactable component is passed into it.
        public override void OnReachableInteractableClick(InteractionTrigger interactable, int numCliks) {
            Debug.Log("Click on Interactable: " + interactable.gameObject.transform.name);
            //If the handle input flag is set to false then do nothing.
            if (!handleInput)
                return;

            //Store the interactble that was clicked on.
            currentInteractable = interactable;

            //Set the destination to the interaction location of the interactable.
            playerMovement.destinationPosition.position = currentInteractable.interactionLocation.position;
        }

        // TODO refactor this funcionality with the pointClick Event System
        //This function is called by the EventTrigger on an Interactable, and trigger instantaneously
        public override void OnImmediateInteractableClick(InteractionTrigger interactable, int numCliks) {
            Debug.Log("Click on Interactable: " + interactable.gameObject.transform.name);
            //If the handle input flag is set to false then do nothing.
            if (!handleInput)
                return;

            //Interact onClick
            interactable.React();
        }
    }
}
