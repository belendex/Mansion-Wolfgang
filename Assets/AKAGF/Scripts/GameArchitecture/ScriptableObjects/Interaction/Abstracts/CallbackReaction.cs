using System.Collections;
using AKAGF.GameArchitecture.MonoBehaviours.Interaction;


namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Abstracts {

    /// <summary>
    /// This is a base class for Reactions that need to have a delay before starting 
    /// or need to be waited until they end to send a callback to the interactable
    /// object that trigger them. This is useful in order to include two unity events
    /// in the Interactable class, one for when the interaction starts, and the other 
    /// one for when the interaction ends.
    /// </summary>

    public abstract class CallbackReaction : DelayedReaction {

        public bool waitForThisReaction; // should be this reaction subcribed to Interactable publisher

        // This function 'hides' the React function from the Reaction class.
        // It replaces the functionality with starting a coroutine instead.
        // This function pass an event subscriber through parameter and 
        // subscribes to it, so is very important to avoid infinite loops 
        // unsubscribing in the child class that hinerites from this one 
        public new void React(ref Interactable publisher) {

            // All delayed reactions will be subscribe to the Interactable 
            // publisher because all of them have waiting behaviours
            if(waitForThisReaction)
                publisher.reactionsEnded += OnInteractionStart;

            publisher.StartCoroutine(ReactCoroutine(publisher));
        }

        // Method to override in the child class. It's responsible of unsubscribe this reaction from the publisher
        protected abstract IEnumerator OnReactionEnd(Interactable publisher);
    }
}
