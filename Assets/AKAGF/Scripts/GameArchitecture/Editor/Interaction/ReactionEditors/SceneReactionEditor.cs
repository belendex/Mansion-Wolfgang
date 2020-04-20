using System.Collections.Generic;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.ImmediateReactions;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (SceneReaction))]
public class SceneReactionEditor : ReactionEditor {

    private const string tooltipText = "This Reaction is used to change between scenes." +
        " Though there is a delay while the scene fades out, this is done with the SceneController class and so" +
        " this is just a Reaction not a DelayedReaction.";

    protected override GUIContent GetFoldoutLabel() {
        return new GUIContent("Scene Reaction", tooltipText);
    }

    private SceneReaction targetReaction;
    

    protected override void Init(){

        targetReaction = target as SceneReaction;
    }

    protected override void DrawReaction() {

        serializedObject.Update();

        GUIContent fCon = new GUIContent("Fade", "Will be the scene switching hided by a screen fade in and out effect?");
        targetReaction.fade = EditorGUILayout.Toggle(fCon, targetReaction.fade);

        SceneControllerEditor.drawSceneEditor(ref targetReaction.sceneName, ref targetReaction.startingPointInLoadedScene);

        serializedObject.ApplyModifiedProperties() ;
        
    }

    
}
