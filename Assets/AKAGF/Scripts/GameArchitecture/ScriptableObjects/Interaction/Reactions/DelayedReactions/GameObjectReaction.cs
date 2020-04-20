using AKAGF.GameArchitecture.MonoBehaviours.Interaction;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Abstracts;
using UnityEngine;

namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.DelayedReactions
{
    /// <summary>
    /// This reaction set the given gameObject activation state to the activeState 
    /// variable value setted in the inspector. It's the equivalent of calling the 
    /// function gameObject.SetActive(activeState).
    /// </summary>
    
    public class GameObjectReaction : DelayedReaction {
        
        public GameObject gameObject = null;       // The gameobject to be turned on or off.
        public bool activeState;            // The state that the gameobject will be in after the Reaction.


        protected override void ImmediateReaction(ref Interactable publisher) {
            gameObject.SetActive(activeState);
        }

    }
}