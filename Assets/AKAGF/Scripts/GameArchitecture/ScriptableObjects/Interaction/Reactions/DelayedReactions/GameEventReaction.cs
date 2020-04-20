using AKAGF.GameArchitecture.MonoBehaviours.Interaction;
using AKAGF.GameArchitecture.ScriptableObjects.Events;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Abstracts;

namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.DelayedReactions
{
    public class GameEventReaction : DelayedReaction {

        public GameEvent[] gameEventsToRaise;


        protected override void ImmediateReaction(ref Interactable publisher) {

            // Raise all gameEvents
            for (int i = 0; i < gameEventsToRaise.Length; i++) {
                gameEventsToRaise[i].Raise();
            }
        }
    }
}
