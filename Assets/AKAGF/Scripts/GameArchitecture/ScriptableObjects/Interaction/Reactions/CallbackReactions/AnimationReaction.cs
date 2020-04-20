using System.Collections;
using AKAGF.GameArchitecture.MonoBehaviours.Interaction;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Abstracts;
using UnityEngine;

namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.CallbackReactions
{
    public class AnimationReaction : CallbackReaction {

        public Animator animator;                            // The Animator that will have its trigger parameter set.
        public string trigger;                               // The name of the trigger parameter to be set.
        public string locomotionStateTag = "Locomotion";     // Tag that animator states must have if they are "handle by player Input States"


        private int triggerHash;    // The hash representing the trigger parameter to be set.


        protected override void SpecificInit () {
            triggerHash = Animator.StringToHash(trigger);
        }


        protected override void ImmediateReaction (ref Interactable publisher) {
            animator.SetTrigger (triggerHash);

            if(waitForThisReaction)
                publisher.StartCoroutine(OnReactionEnd(publisher));
        }


        protected override IEnumerator OnReactionEnd(Interactable publisher) {

            int locomotionTagHash = Animator.StringToHash(locomotionStateTag);

            // Wait for the transition from Locomotion state to non Locomotion state
            while(animator.GetCurrentAnimatorStateInfo(0).tagHash == locomotionTagHash)
                yield return null;

            // Wait until an animation with Locomotion State is being played again.
            while (animator.GetCurrentAnimatorStateInfo(0).tagHash != locomotionTagHash)
                yield return null;
       
            // Unsubscribe from Interactable publisher
            publisher.reactionsEnded -= OnInteractionStart;
        }
    }
}
