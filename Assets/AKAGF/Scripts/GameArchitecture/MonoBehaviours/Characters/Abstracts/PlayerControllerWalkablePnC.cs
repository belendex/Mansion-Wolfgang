using AKAGF.GameArchitecture.MonoBehaviours.Input.Interfaces;
using AKAGF.GameArchitecture.MonoBehaviours.Input.Mouse;
using AKAGF.GameArchitecture.MonoBehaviours.Interaction.Abstracts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AKAGF.GameArchitecture.MonoBehaviours.Characters.Abstracts
{
    [RequireComponent(typeof(MouseInput))]
    public abstract class PlayerControllerWalkablePnC : PlayerControllerPnC, IWalkablePointNClick {

        // The interactable that is currently being headed towards.
        protected InteractionTrigger currentInteractable;

        /************** Point & Click GameObject Tags ****************/
        [TagSelector]
        public string walkableTag = "";
        [TagSelector]
        public string unwalkableTag = "";
        /*************************************************************/


        public abstract void OnWalkableLayerClick(PointerEventData data, int clickCount);
        public abstract void OnUnWalkableLayerClick(PointerEventData data, int clickCount);
        public abstract void OnReachableInteractableClick(InteractionTrigger interactable, int clickCount);


        protected override void Start() {
            base.Start();
            mouseInput = GetComponent<MouseInput>();
        }

        public override void InteractionMouseClick(PointerEventData data) {

            string gameObjectTag = data.pointerPress.tag;


            if (gameObjectTag.Equals(walkableTag)) {
                OnWalkableLayerClick(data, data.clickCount);
            }
            else if (gameObjectTag.Equals(unwalkableTag)) {
                OnUnWalkableLayerClick(data, data.clickCount);

            } else if (gameObjectTag.Equals(interactableTag)) {
                // GameObject should has Interactable controller attached to it because it is tagged as such.
                InteractionTrigger interactable = data.pointerPress.GetComponent<InteractionTrigger>();

                // If null, stop executing the method and inform
                if (!interactable) {
                    Debug.Log("No Interactable Trigger Component attached to GameObject: " + data.pointerPress.name);
                }
                else {

                    // The intertable trigger does not have a Transform, 
                    // so the reaction should be immediate.
                    if (interactable.interactionLocation == null)
                        OnImmediateInteractableClick(interactable, data.clickCount);
                    else
                        OnReachableInteractableClick(interactable, data.clickCount);
                }

            }
            else {
                Debug.Log("No Interactable, Walkable or UnWalkable GameObject clicked: " + data.pointerPress.name);
            }

        } 
    }
}
