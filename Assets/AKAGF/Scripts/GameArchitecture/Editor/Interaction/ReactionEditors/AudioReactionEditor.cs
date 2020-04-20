using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.ImmediateReactions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioReaction))]
public class AudioReactionEditor : ReactionEditor {

    private const string tooltipText = "This Reaction is used to play sounds through a given AudioSource. " +
        "Since the AudioSource itself handles delay, this is a Reaction rather than an DelayedReaction";

    protected override GUIContent GetFoldoutLabel () {
        return new GUIContent("Audio Reaction", tooltipText);
    }
}
