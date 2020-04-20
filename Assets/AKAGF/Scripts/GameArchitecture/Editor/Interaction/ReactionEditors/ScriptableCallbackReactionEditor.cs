using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.CallbackReactions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScriptableCallbackReaction))]
public class ScriptableCallbackReactionEditor : ReactionEditor {

    private const string tooltipText = "This reaction keeps the Interactable object interacting while the value of the " +
        "callback state variable remains false, so it's important to assign it the true value outside the reaction.";

    protected override GUIContent GetFoldoutLabel() {
        return new GUIContent("Scriptable Callback Reaction", tooltipText);
    }
}
