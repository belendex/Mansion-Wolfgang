using AKAGF.GameArchitecture.MonoBehaviours.Characters.Abstracts;
using AKAGF.GameArchitecture.MonoBehaviours.Interaction.Abstracts;
using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Characters.Static {

    public class PlayerControllerStaticFP : PlayerControllerPnC {

        public override void OnImmediateInteractableClick(InteractionTrigger interactable, int clickCount) {

            Debug.Log("Click on Interactable: " + interactable.gameObject.transform.name);
            //If the handle input flag is set to false then do nothing.
            if (!handleInput)
                return;

            //Interact onClick
            interactable.React();
        }
    }
}