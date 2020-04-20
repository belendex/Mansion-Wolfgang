using AKAeditor;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Conditions;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.ImmediateReactions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ConditionReaction))]
public class ConditionReactionEditor : ReactionEditor
{
    private SerializedProperty conditionProperty;       // Represents the Condition that will be changed.
    private SerializedProperty satisfiedProperty;       // Represents the value that the Condition's satifised flag will be set to.

    private const string conditionReactionPropConditionName = "condition";
                                                        // Name of the field which is the Condition that will be changed.
    private const string conditionReactionPropSatisfiedName = "satisfied";
                                                        // Name of the bool field which is the value the Condition will get.


    private const string tooltipText = "This is the Reaction to change the satisfied state of a Condition. " +
        "The Condition here is a reference to one on the AllConditions asset.  " +
        "That means by changing the Condition here, the global game Condition will change." +
        "Since Reaction decisions are made based on Conditions, the change must be" +
        " immediate and therefore this is a Reaction rather than a DelayedReaction.";

    protected override void Init () {
        // Cache the SerializedProperties.
        conditionProperty = serializedObject.FindProperty (conditionReactionPropConditionName);
        satisfiedProperty = serializedObject.FindProperty (conditionReactionPropSatisfiedName);
    }

	
    protected override void DrawReaction ()
    {
        // If there's isn't a Condition yet, set it to the first Condition from the AllConditions array.
        if (conditionProperty.objectReferenceValue == null)
            conditionProperty.objectReferenceValue = ScriptableObjectUtility.TryGetScriptableObjectAt(0, AllConditions.Instance.conditions);

        // Get the index of the Condition in the AllConditions array.
        int index = AllConditionsEditor.TryGetConditionIndex ((Condition)conditionProperty.objectReferenceValue);

        // Use and set that index based on a popup of all the descriptions of the Conditions.
        index = EditorGUILayout.Popup (index, AllConditionsEditor.AllConditionDescriptions);

        // Set the Condition based on the new index from the AllConditions array.
        conditionProperty.objectReferenceValue = ScriptableObjectUtility.TryGetScriptableObjectAt(index, AllConditions.Instance.conditions);

        // Use default toggle GUI for the satisfied field.
        EditorGUILayout.PropertyField (satisfiedProperty);
    }


    protected override GUIContent GetFoldoutLabel () {
        return new GUIContent("Condition Reaction", tooltipText);
    }
}
