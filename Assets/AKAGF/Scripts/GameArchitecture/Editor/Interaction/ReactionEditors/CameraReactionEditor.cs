using UnityEngine;
using UnityEditor;
using AKAeditor;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.DelayedReactions;

[CustomEditor(typeof(CameraReaction))]
public class CameraReactionEditor : ReactionEditor {

    private SerializedProperty delayProperty;           // Reaction delay property
    private SerializedProperty waitForThisReactionProperty;
    private SerializedProperty cameraProperty;           // Represent float field time
    private SerializedProperty OnAnimationEndDelayProperty; // Represent the movementConfig  property
    private SerializedProperty animationsProperty;    // Represents the bool field that active orbit in camera animation

    private const string delayPropertyName = "delay";
    private const string waitForThisReactionPropertyName = "waitForThisReaction";
    private const string cameraPropertyName = "cameraController";
    private const string OnAnimationEndDelayPropertyName = "OnAnimationEndDelay";
    private const string animationsPropertyName = "animations";
    

    private const float buttonWidth = 48f;          // Width in pixels of the button


    protected override void Init() {

        delayProperty = serializedObject.FindProperty(delayPropertyName);
        waitForThisReactionProperty = serializedObject.FindProperty(waitForThisReactionPropertyName);
        cameraProperty = serializedObject.FindProperty(cameraPropertyName);
        OnAnimationEndDelayProperty = serializedObject.FindProperty(OnAnimationEndDelayPropertyName);
        animationsProperty = serializedObject.FindProperty(animationsPropertyName);
    }

    protected override void DrawReaction() {

        // Display default GUI 
        EditorGUILayout.PropertyField(waitForThisReactionProperty);
        EditorGUILayout.PropertyField(delayProperty);
        EditorGUILayout.PropertyField(cameraProperty);
        
        // Slider for OnAnimationEndDelay
        EditorTools.createSliderProperty(ref OnAnimationEndDelayProperty, 0.1f , 30);

        EditorGUILayout.Space();

        //Animations array
        EditorTools.createArrayPropertyButtons(animationsProperty, "Animations", GUILayout.Width( buttonWidth));

        if (animationsProperty.arraySize > 0) {
            for (int i = 0; i < animationsProperty.arraySize; i++) {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.PropertyField(animationsProperty.GetArrayElementAtIndex(i), new GUIContent("Animation " + i), true);
                EditorGUILayout.EndVertical();
            } 
        }
    }


    protected override GUIContent GetFoldoutLabel() {
        return new GUIContent("Camera Reaction", "TODO: tooltip");
    }
}
