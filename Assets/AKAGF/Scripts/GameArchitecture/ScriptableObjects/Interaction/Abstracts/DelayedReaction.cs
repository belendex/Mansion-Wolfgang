using System.Collections;
using AKAGF.GameArchitecture.MonoBehaviours.Interaction;
using UnityEngine;



namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Abstracts {

    /// <summary>
    /// This is a base class for Reactions that need to have a delay before starting 
    /// or need to be waited until they end to send a callback to the interactable
    /// object that trigger them. This is useful in order to include two unity events
    /// in the Interactable class, one for when the interaction starts, and the other 
    /// one for when the interaction ends.
    /// </summary>

    public abstract class DelayedReaction : Reaction {

        public float delay;             // All DelayedReactions need to have a time that they are delayed by.
        protected WaitForSeconds wait;  // Storing the wait created from the delay so it doesn't need to be created each time.


        // This function 'hides' the Init function from the Reaction class.
        // Hiding generally happens when the original function doesn't meet
        // the requirements for the function in the inheriting class.
        // Previously it was assumed that all Reactions just needed to call
        // SpecificInit but with DelayedReactions, wait needs to be set too.
        public new void Init () {
            wait = new WaitForSeconds (delay);
            SpecificInit ();
        }


        // This function 'hides' the React function from the Reaction class.
        // It replaces the functionality with starting a coroutine instead.
        // This function pass an event subscriber through parameter and 
        // subscribes to it, so is very important to avoid infinite loops 
        // unsubscribing in the child class that hinerites from this one 
        public new void React(ref Interactable publisher) {

            publisher.StartCoroutine(ReactCoroutine(publisher));
        }


        protected IEnumerator ReactCoroutine (Interactable publisher) {
            // Wait for the specified time.
            yield return wait;
        
            // Then call the ImmediateReaction function which must be defined in inherting classes.
            ImmediateReaction(ref publisher);
        }
    }
}
