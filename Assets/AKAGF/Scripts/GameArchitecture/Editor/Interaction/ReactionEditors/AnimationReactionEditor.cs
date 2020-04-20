using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.CallbackReactions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimationReaction))]
public class AnimationReactionEditor : ReactionEditor {

    private const string tooltipText = "Reaction to trigger an animation for the animator and " +
        "parameter setted. The tag allow the possibility of waiting for the animation to end.";

    protected override GUIContent GetFoldoutLabel () {
        return new GUIContent("Animation Reaction", tooltipText); 
    }
}
