using System;
using System.Collections;
using AKAGF.GameArchitecture.MonoBehaviours.GUI;
using AKAGF.GameArchitecture.MonoBehaviours.Interaction;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Abstracts;
using UnityEngine;
using UnityEngine.Events;

namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.CallbackReactions {
// This Reaction has a delay but is not a DelayedReaction.
// This is because the TextManager component handles the
// delay instead of the Reaction.
    public class TextReaction : CallbackReaction {

        public TextManager textManager;                 // Reference to the component to display the text. There could be more than one TextMananger in the scene.
        public Message[] messages;
   
        private bool enabled = true;                    // Bool variable equivalent to enabled in a MonoBehaviour Script. Used to avoid call a non referenced TextManager Script


        protected override void SpecificInit(){

                if (!textManager) {
                    Debug.Log("No TextManager assigned to this TextReaction.");
                    enabled = false;
                }
        }


        protected override void ImmediateReaction(ref Interactable publisher) {

            // If there is no reference to a TextManager script, avoid execute the rest of the code
            if (!enabled)
                return;

            // Add all messages to TextManager Queue
            for (int i = 0; i < messages.Length; i++) 
                textManager.DisplayMessage(messages[i], i == messages.Length - 1 ? true : false); // blocks the textManager in the last message addition
        
            // Starts the coroutine that will unsubscribe this reaction
            // from the waitingEvent in publisher when TextManager finish
            // displaying all messages
            if (waitForThisReaction)
                publisher.StartCoroutine(OnReactionEnd(publisher));
        }


        protected override IEnumerator OnReactionEnd(Interactable publisher) {
            // This is a delayed reaction, so it's not 
            // necessary to subscribe it again to the delegate
            while (textManager.blocked)
                yield return null;

            // Unsubscribe this reaction from the reactionsEnded event
            publisher.reactionsEnded -= OnInteractionStart;
            Debug.Log("TextReaction Finished");
        }
    }


    
}