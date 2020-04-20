using System.Collections;
using AKAGF.GameArchitecture.MonoBehaviours.Interaction;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Abstracts;
using AKAGF.GameArchitecture.ScriptableObjects.Variables;


namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.CallbackReactions { 

    public class ScriptableCallbackReaction : CallbackReaction {

        public BoolVar callbackState;

        private void OnValidate() {
            waitForThisReaction = true;
        }

        protected override void ImmediateReaction(ref Interactable publisher) {

            // At the beginning the callback will always be false.
            callbackState.value = false;

            if (waitForThisReaction)
                publisher.StartCoroutine(OnReactionEnd(publisher));
        }

        protected override IEnumerator OnReactionEnd(Interactable publisher) {

            // Waiting for the callbackstate to be set to true by 
            // another game entity in order to finish the reaction.
            while(callbackState.value == false)
                yield return null;

            // Unsubscribe from Interactable publisher
            publisher.reactionsEnded -= OnInteractionStart;
        }
    }
}
