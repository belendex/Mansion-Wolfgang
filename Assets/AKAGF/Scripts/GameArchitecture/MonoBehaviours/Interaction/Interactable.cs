using System;
using System.Collections;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Conditions;
using UnityEngine;
using UnityEngine.Events;

// This is one of the core features of the game.
// Each one acts like a hub for all things that transpire
// over the course of the game.
// The script must be on a gameobject with a collider and
// an event trigger.  The event trigger should tell the
// player to approach the interactionLocation and the 
// player should call the Interact function when they arrive.
namespace AKAGF.GameArchitecture.MonoBehaviours.Interaction
{
    public class Interactable : MonoBehaviour {

        public UnityEvent onInteractionStart;
        public UnityEvent onInteractionEnd;

        public event Action reactionsEnded;                                                     // All reactions will be subscribed to this event when they start and unsubscribed when they end

        public ConditionCollection[] conditionCollections = new ConditionCollection[0];         // All the different Conditions and relevant Reactions that can happen based on them.
        public ReactionCollection[] defaultReactionCollection = new ReactionCollection[0];      // If none of the ConditionCollections are reacted to, this one is used.
        

        public bool randomDefaultReaction = true;                                               // If there is more than one Default Reaction, will be selected randomly?
        public int defaultReactionIndex = 0;                                                    // The index of the defaultReaction when it's not randomly selected.
        public bool isInteracting { get; private set; }                                         // flag to know externaly if this Interactable is currently executing delayed reactions

        // This is called when the player arrives at the interactionLocation.
        public void Interact () {
            // Go through all the ConditionCollections...
            for (int i = 0; i < conditionCollections.Length; i++)  {
                // ... then check and potentially react to each.  If the reaction happens, exit the function.
                if (conditionCollections[i].CheckAndReact(this)) {
                    OnInteractionStart();
                    return;
                }
            }

            // If none of the reactions happened and there is at least one default ReactionsCollection, react.
            if (defaultReactionCollection.Length == 0) {
                Debug.LogWarning("No Reactions found in Interactable: " + name);
                return;
            }
                

            int defaultReaction = randomDefaultReaction ? UnityEngine.Random.Range(0, defaultReactionCollection.Length) : defaultReactionIndex;
            defaultReactionCollection[defaultReaction].React(this);
            OnInteractionStart();
        }

        public void delayedInteract(float delay) {
            StartCoroutine(delayInteraction(delay));
        }

        private IEnumerator delayInteraction(float delay) {

            yield return new WaitForSeconds(delay);
            Interact();
        }

        // This is the publisher method. All reactions
        // must have a method with exactly the same signature
        // to subscribe / unsubscribe to this publisher event
        // publisher --> Interactable
        protected virtual void OnInteractionStart() {

            Debug.Log("Starting reactions...");
            StartCoroutine(waitToCompleteReactions());
        }


        private IEnumerator waitToCompleteReactions() {

            // Disable required components
            onInteractionStart.Invoke();
            isInteracting = true;

            // If reactionsEnded has one or more subscribers 
            // it waits until all the reactions have been unsubscribed (finished Reaction)
            while (reactionsEnded != null) {
                yield return null;
                Debug.Log("Waiting for reactions to complete...");
            }

            onInteractionEnd.Invoke();
            isInteracting = false;
        }
    }
}
