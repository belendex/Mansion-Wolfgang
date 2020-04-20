using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.DelayedReactions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PickedUpItemReaction))]
public class PickedUpItemReactionEditor : ReactionEditor {

    private const string tooltipText = "This reaction add an item to an Inventory.";

    protected override GUIContent GetFoldoutLabel() {
        return new GUIContent("Picked Up Item Reaction", tooltipText);
    }
}
