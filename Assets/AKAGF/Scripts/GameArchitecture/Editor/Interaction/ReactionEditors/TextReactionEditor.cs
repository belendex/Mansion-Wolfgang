using UnityEditor;
using AKAeditor;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.CallbackReactions;

[CustomEditor(typeof(TextReaction))]
public class TextReactionEditor : ReactionEditor {
   
    private SerializedProperty textManagerProperty;
    private SerializedProperty messagesProperty;
    private SerializedProperty waitForThisReactionProperty;
    private SerializedProperty delayProperty;

    private const string textManagerPropertyName = "textManager";
    private const string messagesPropertyName = "messages";
    private const string waitForThisReactionPropertyName = "waitForThisReaction";
    private const string delayPropertyName = "delay";

    private const float messageGUILines = 3f;                           // How many lines tall the GUI for the message field should be.
    private const float areaWidthOffset = 19f;                          // Offset to account for the message GUI being made of two GUI calls.  It makes the GUI line up.
    private const float buttonWidth = 48f;
    private const int maxCharactersInTitle = 30;

    private bool[] expandedEvents = new bool[0];

    private const string tooltipText = "This Reaction has a delay but is not a DelayedReaction." +
        " This is because the TextManager component handles the" +
        " delay instead of the Reaction.";

    protected override void Init() {
        // Cache all the SerializedProperties.
        textManagerProperty = serializedObject.FindProperty(textManagerPropertyName); ;
        messagesProperty = serializedObject.FindProperty(messagesPropertyName);
        waitForThisReactionProperty = serializedObject.FindProperty(waitForThisReactionPropertyName);
        delayProperty = serializedObject.FindProperty(delayPropertyName);
}


    protected override void DrawReaction() {
       
        // Display default GUI vars
        EditorGUILayout.PropertyField(waitForThisReactionProperty);
        EditorGUILayout.PropertyField(delayProperty);
        EditorGUILayout.PropertyField(textManagerProperty);

        EditorGUILayout.Space();

        // Change default Unity Editor array elements management
        EditorTools.createArrayPropertyButtons(messagesProperty, "Messages", GUILayout.Width(buttonWidth));

        if (messagesProperty.arraySize > 0) {

            if (messagesProperty.arraySize != expandedEvents.Length)
                Array.Resize(ref expandedEvents, messagesProperty.arraySize);

            for (int i = 0; i < messagesProperty.arraySize; i++) {

                EditorGUILayout.BeginVertical(GUI.skin.box);

                // Targeting property from the array
                SerializedProperty auxProperty = messagesProperty.GetArrayElementAtIndex(i);
                string messageTitle = i + "- " + auxProperty.FindPropertyRelative("message").stringValue;

                // Display as title part of the message itself
                if (messageTitle.Length > maxCharactersInTitle)
                    messageTitle = messageTitle.Remove(maxCharactersInTitle) + "...";
                  
                // Handle foldut through isExpanded variable
                messagesProperty.GetArrayElementAtIndex(i).isExpanded = EditorGUILayout.Foldout(messagesProperty.GetArrayElementAtIndex(i).isExpanded, new GUIContent(messageTitle), true, EditorStyles.foldout);

                // If the menu is expanded for this message, show custom inspector
                if (messagesProperty.GetArrayElementAtIndex(i).isExpanded) {
                    // Textarea instead of Textfield
                    auxProperty.FindPropertyRelative("message").stringValue = EditorGUILayout.TextArea(auxProperty.FindPropertyRelative("message").stringValue, EditorStyles.textArea, GUILayout.Height(EditorGUIUtility.singleLineHeight * messageGUILines));

                    EditorGUILayout.Space();

                    // Color property
                    EditorGUILayout.PropertyField(messagesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("textColor"));

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    expandedEvents[i] = EditorGUILayout.Foldout(expandedEvents[i], "Message Events", true);

                    if (expandedEvents[i]) {
                                            
                        EditorGUILayout.PropertyField(messagesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("OnMessageStart"));
                        EditorGUILayout.PropertyField(messagesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("OnTextAnimationEnd"));
                        EditorGUILayout.PropertyField(messagesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("OnMessageEnd"));
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();

                    
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
        }
    }


    protected override GUIContent GetFoldoutLabel (){
        return new GUIContent("Text Reaction", tooltipText);
    }
}
