using AKAGF.GameArchitecture.MonoBehaviours.Interaction;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Abstracts;
using UnityEngine.Events;

namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.DelayedReactions {

    public class UnityEventReaction : DelayedReaction {

        public UnityEvent OnReaction;


        protected override void ImmediateReaction(ref Interactable publisher) {

            OnReaction.Invoke();

        }
    }
}
