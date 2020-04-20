using AKAGF.GameArchitecture.MonoBehaviours.Input.Interfaces;
using AKAGF.GameArchitecture.MonoBehaviours.Input.Mouse;
using AKAGF.GameArchitecture.MonoBehaviours.Interaction.Abstracts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AKAGF.GameArchitecture.MonoBehaviours.Characters.Abstracts
{
    [RequireComponent(typeof(MouseInput))]
    public abstract class PlayerControllerPnC : PlayerControllerBase, IPointNClick  {

        /************** Point & Click GameObject Tags ****************/
        [TagSelector]
        public string interactableTag = "";
        /*************************************************************/

        [HideInInspector]
        public MouseInput mouseInput;

        public abstract void OnImmediateInteractableClick(InteractionTrigger interactable, int clickCount);

        protected override void Start() {
            base.Start();
            mouseInput = GetComponent<MouseInput>();
        }

        public virtual void InteractionMouseClick(PointerEventData data) {

            string gameObjectTag = data.pointerPress.tag;


             if (gameObjectTag.Equals(interactableTag)) {
                // GameObject should has Interactable controller attached to it because it is tagged as such.
                InteractionTrigger interactable = data.pointerPress.GetComponent<InteractionTrigger>();

                // If null, stop executing the method and inform
                if (!interactable) {
                    Debug.Log("No Interactable Trigger Component attached to GameObject: " + data.pointerPress.name);
                }
                else {
                    OnImmediateInteractableClick(interactable, data.clickCount);
                }
            }
            else {
                Debug.Log("No Interactable, Walkable or UnWalkable GameObject clicked: " + data.pointerPress.name);
            }

        }
    }
}
