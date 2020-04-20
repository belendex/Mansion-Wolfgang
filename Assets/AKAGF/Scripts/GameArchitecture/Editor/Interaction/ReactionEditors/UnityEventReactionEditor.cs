using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.DelayedReactions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnityEventReaction))]
public class UnityEventReactionEditor : ReactionEditor {

    private const string tooltipText = "Reaction to raise all the functions subcribed to the Unity Event OnReaction.";

    protected override GUIContent GetFoldoutLabel() {
        return new GUIContent("UnityEvent Reaction", tooltipText);
    }
}
