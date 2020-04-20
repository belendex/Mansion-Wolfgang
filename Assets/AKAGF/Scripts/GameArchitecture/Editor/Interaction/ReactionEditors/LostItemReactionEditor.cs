using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.DelayedReactions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LostItemReaction))]
public class LostItemReactionEditor : ReactionEditor {

    private const string tooltipText = "This reaction removes an item from an Inventory."; 

    protected override GUIContent GetFoldoutLabel () {
        return new GUIContent("Lost Item Reaction", tooltipText);
    }
}