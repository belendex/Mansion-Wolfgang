using AKAGF.GameArchitecture.MonoBehaviours.Interaction;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Abstracts;
using UnityEngine;

// This Reaction is for turning Behaviours on and
// off.  Behaviours are a subset of Components
// which have the enabled property, for example
// all MonoBehaviours are Behaviours as well as
// Animators, AudioSources and many more.
namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.DelayedReactions
{
    public class BehaviourReaction : DelayedReaction {

        public GameObject behavioursContainerObject;    // Reference to the gameobject that contains the behaviours
        public Behaviour[] behaviours;                 // The Behaviours to be turned on or off.
        public bool enabledState;                       // The state the Behaviours will be in after the Reaction.

        public int behaviorStatesflags = 0;                        // There could be more than one behavior attached to a GameObject, 
        // flag to keep tracking of all behaviour inside a GameObject

        protected override void ImmediateReaction(ref Interactable publisher) {

            for (int i = 0; i < behaviours.Length; i++) {
                behaviours[i].enabled = enabledState;
            }
        
        }
    }
}