using System;
using AKAGF.GameArchitecture.MonoBehaviours.Characters.Abstracts;
using AKAGF.GameArchitecture.MonoBehaviours.Input;
using AKAGF.GameArchitecture.MonoBehaviours.Input.Mouse;
using AKAGF.GameArchitecture.MonoBehaviours.Interaction.Abstracts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AKAGF.GameArchitecture.MonoBehaviours.Characters._3D
{
    [RequireComponent(typeof(GamepadInput))]
    [RequireComponent(typeof(PlayerAnimation3D))]
    public class PlayerController3D : PlayerControllerWalkablePnC {

        public enum INPUT_MODE { POINTNCLICK, GAMEPAD };
        public INPUT_MODE inputMode;
        public float interactionDistance = .1f;                          // The minimun distance the player must to be to interact whit currentInteractable  

        private GamepadInput gamepadInput; 

        private PlayerMovement3D playerMovement;
        private PlayerAnimation3D playerAnimation;
        public PlayerInteractionSensor interactionZone;

        protected override void Start() {
            base.Start();
        
            gamepadInput = GetComponent<GamepadInput>();
            mouseInput = GetComponent<MouseInput>();
            playerAnimation = GetComponent<PlayerAnimation3D>();
            checkInputMode();

        }


        private void Update() {

            checkInputMode();   //Only for debug porpouses

            if (handleInput) {
                if (inputMode == INPUT_MODE.GAMEPAD) {
                    //activateInteraction is the access point to Interact with framework interaction system using external player controllers
                    interactionZone.interactionButton = gamepadInput.controlsScheme.actionInput;
                }

                //handleRotation
                //The target rotation is the rotation of the interactionLocation if the player is headed to an interactable, or the player's own rotation if not.
                Quaternion targetRotation = currentInteractable ? currentInteractable.interactionLocation.rotation : transform.rotation;
                playerMovement.handleRotation(targetRotation);

                //handleAnimation
                playerAnimation.handleAnimation(playerMovement.currentSpeed, playerMovement.speedDampTime);

                //handleInteraction
                handleInteraction();
            }
            else {

                //handleAnimation
                playerAnimation.handleAnimation(0, playerMovement.speedDampTime);
            }
        }


        private void FixedUpdate() {
            if (handleInput && inputMode == INPUT_MODE.GAMEPAD) {
                playerMovement.handleMovement(transform.forward);
            }
        }


        private void OnAnimatorMove() {
            if (handleInput && inputMode == INPUT_MODE.POINTNCLICK) {
                // Set the velocity of the nav mesh agent (which is moving the player) based on the speed that the animator would move the player.
                playerMovement.handleMovement(playerAnimation.animator.deltaPosition / Time.deltaTime);
            }
        }


        private void checkInputMode() {

            switch (inputMode) {
                case INPUT_MODE.GAMEPAD:
                    mouseInput.activeInput = false;
                    playerMovement = GetComponent<PlayerMovement3D_GPad>();
                    break;

                case INPUT_MODE.POINTNCLICK:
                    mouseInput.activeInput = true;
                    playerMovement = GetComponent<PlayerMovement3D_PNC>();
                    break;
            }
        }


        private void handleInteraction() {
            if (currentInteractable) {
                // If the player is stopping at an interactable...
                float distanceToInteractable = Vector3.Distance(transform.position, currentInteractable.interactionLocation.position);
                if (distanceToInteractable < interactionDistance) {
                    // ... set the player's rotation to match the interactionLocation's.
                    transform.rotation = currentInteractable.interactionLocation.rotation;

                    // Interact with the interactable and then null it out so this interaction only happens once.
                    currentInteractable.React();
                    currentInteractable = null;
                }
            }
        }


        /********************* IPointNClick Interface Methods ********************/
        public override void OnReachableInteractableClick(InteractionTrigger interactable, int numCliks) {
            // If the handle input flag is set to false then do nothing.
            if (!handleInput || inputMode == INPUT_MODE.GAMEPAD)
                return;

            // Store the interactble that was clicked on.
            if(!interactable.autoReact)
                currentInteractable = interactable;

            // Set the destination to the interaction location of the interactable.
            PlayerMovement3D_PNC pmPNC = playerMovement as PlayerMovement3D_PNC;

            // Agent speed
            if (numCliks == 1) {
                pmPNC.agent.speed = pmPNC.walkSpeed;

            }
            else if (numCliks == 2) {
                pmPNC.agent.speed = pmPNC.runSpeed;
            }

            // Agent destination
            pmPNC.setDestination(interactable.interactionLocation.position);
        }

        public override void OnUnWalkableLayerClick(PointerEventData data, int numCliks) {
            throw new NotImplementedException();
        }

        public override void OnWalkableLayerClick(PointerEventData data, int numCliks) {
            // If the handle input flag is set to false then do nothing.
            if (!handleInput || inputMode == INPUT_MODE.GAMEPAD)
                return;

            // The player is no longer headed for an interactable so set it to null.
            currentInteractable = null;

            // Try and find a point on the nav mesh nearest to the world position of the click and set the destination to that.
            PlayerMovement3D_PNC pmPNC = playerMovement as PlayerMovement3D_PNC;

            // Agent speed
            if (numCliks == 1) {
                pmPNC.agent.speed = pmPNC.walkSpeed;

            } else if (numCliks == 2) {
                pmPNC.agent.speed = pmPNC.runSpeed;
            }

            // Agent destination
            pmPNC.setDestination(pmPNC.getPointFromMesh(data.pointerCurrentRaycast.worldPosition));
        }

        public override void OnImmediateInteractableClick(InteractionTrigger interactable, int numCliks) {
            throw new NotImplementedException();
        }

    }
}


