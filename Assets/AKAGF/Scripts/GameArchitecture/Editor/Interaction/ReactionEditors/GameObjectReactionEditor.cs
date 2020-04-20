using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.DelayedReactions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameObjectReaction))]
public class GameObjectReactionEditor : ReactionEditor {

    private const string tooltipText = "This reaction set the given gameObject activation state to the activeState +" +
        " variable value setted in the inspector. It's the equivalent of calling the " +
        "function gameObject.SetActive(activeState).";

    protected override GUIContent GetFoldoutLabel () {
        return new GUIContent("GameObject Reaction", tooltipText);
    }
}
