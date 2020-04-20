using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.CallbackReactions;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ConversationReaction))]
public class ConversationReactionEditor : ReactionEditor {

    private SerializedProperty dialogueProperty;         // Represents the string field which is the message to be displayed.
    private SerializedProperty nodeOverrideInfoProperty;       // Represents the color field which is the color of the message to be displayed.

    private const float messageGUILines = 3f;                           // How many lines tall the GUI for the message field should be.
    private const float areaWidthOffset = 19f;                          // Offset to account for the message GUI being made of two GUI calls.  It makes the GUI line up.
    private const string ConversationReactionPropdialogueName = "dialogue";       // The name of the field which is the message to be written to the screen.
    private const string ConversationReactionPropnodeOverrideInfoName = "nodeOverrideInfo";   // The name of the field which is the color of the message to be written to the screen.

    private const float buttonWidth = 135f;          // Width in pixels of the button

    protected override void Init() {
        // Cache all the SerializedProperties.
        dialogueProperty = serializedObject.FindProperty(ConversationReactionPropdialogueName);
        nodeOverrideInfoProperty = serializedObject.FindProperty(ConversationReactionPropnodeOverrideInfoName);
    }

    protected override void DrawReaction() {
        EditorGUILayout.BeginHorizontal();
            // Display default GUI 
            EditorGUILayout.PropertyField(dialogueProperty);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        
        // Display a label whose width is offset such that the TextArea lines up with the rest of the GUI.
        EditorGUILayout.LabelField("Override Nodes Info", GUILayout.Width(EditorGUIUtility.labelWidth - areaWidthOffset));
        //Display nodeOverrideInfo list
        if (GUILayout.Button("Add Override Node", GUILayout.Width(buttonWidth))) {
            nodeOverrideInfoProperty.arraySize++;
        }

        if (GUILayout.Button("Delete Override Node", GUILayout.Width(buttonWidth))) {
            nodeOverrideInfoProperty.arraySize--;
        }
        EditorGUILayout.EndHorizontal();

        if (nodeOverrideInfoProperty.arraySize > 0) {
            EditorGUILayout.Space();


            for (int i = 0; i < nodeOverrideInfoProperty.arraySize; i++) {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.PropertyField(nodeOverrideInfoProperty.GetArrayElementAtIndex(i), new GUIContent("Node Override " + i), true);
                EditorGUILayout.EndVertical();
            }
        }
    }


    protected override GUIContent GetFoldoutLabel() {
        return new GUIContent ("Conversation Reaction", "Conversation Reaction Tooltip");
    }
}
