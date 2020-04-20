using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.DelayedReactions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameEventReaction))]
public class GameEventReactionEditor : ReactionEditor {

    private const string tooltipText = "Reaction to Raise the Scriptable GameEvents inside " +
        "gameEvents To Raise list. Remember to create Monobehaviour gameEventListeners for each gameEvent" +
        " to take any effect.";

    protected override GUIContent GetFoldoutLabel() {
        return new GUIContent("GameEvent Reaction", tooltipText);
    }
}
