using System;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Abstracts;
using System.Collections.Generic;
using UnityEngine;

// This script acts as a collection for all the
// individual Reactions that happen as a result
// of an interaction.
namespace AKAGF.GameArchitecture.MonoBehaviours.Interaction
{
    [Serializable]
    public class ReactionCollection : MonoBehaviour {
        public Reaction[] reactions = new Reaction[0];      // Array of all the Reactions to play when React is called.

#if UNITY_EDITOR

        public static List<int> ReactionReferences = new List<int>();
#endif

        private void Start ()  {
            // Go through all the Reactions and call their Init function.
            for (int i = 0; i < reactions.Length; i++)  {

                CallbackReaction callbackReaction = reactions[i] as CallbackReaction;

                if (callbackReaction) {
                    callbackReaction.Init();
                    continue;
                } 
                
                // The DelayedReaction 'hides' the Reaction's Init function with it's own.
                // This means that we have to try to cast the Reaction to a DelayedReaction and then if it exists call it's Init function.
                // Note that this mainly done to demonstrate hiding and not especially for functionality.
                DelayedReaction delayedReaction = reactions[i] as DelayedReaction;

                if (delayedReaction) {
                    delayedReaction.Init();
                    continue;
                }


                reactions[i].Init ();
            }
        }


        public void React(Interactable publisher) {
            // Go through all the Reactions and call their React function.
            for (int i = 0; i < reactions.Length; i++) {

                CallbackReaction callbackReaction = reactions[i] as CallbackReaction;

                if (callbackReaction) {
                    callbackReaction.React(ref publisher);
                    continue;
                }

                // The DelayedReaction hides the Reaction.React function.
                // Note again this is mainly done for demonstration purposes.
                DelayedReaction delayedReaction = reactions[i] as DelayedReaction;

                if (delayedReaction) {
                    delayedReaction.React(ref publisher);
                    continue;
                }

                reactions[i].React(ref publisher);
            }
        }

    }
}
