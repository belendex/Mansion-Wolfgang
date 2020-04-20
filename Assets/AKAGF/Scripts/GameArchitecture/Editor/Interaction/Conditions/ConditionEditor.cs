using UnityEngine;
using UnityEditor;
using AKAeditor;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Conditions;

// This class controls all the GUI for Conditions
// in all the places they are found.

[CustomEditor(typeof(Condition))]
public class ConditionEditor : Editor {
    // This enum is used to represent where the Condition is being seen in the inspector.
    // ConditionAsset is for when a single Condition asset is selected as a child of the AllConditions asset.
    // AllConditionAsset is when the AllConditions asset is selected and this is a nested Editor.
    // ConditionCollection is when an Interactable is selected and this is a nested Editor within a ConditionCollection.
    public enum EditorType {
        ConditionAsset, AllConditionAsset, ConditionCollection
    }

    public EditorType editorType;                               // The type of this Editor.
    public SerializedProperty requiredConditionsProperty;       // The SerializedProperty representing an array of required Conditions on a ConditionCollection.


    private SerializedProperty descriptionProperty;     // Represents a string description of this Editor's target.
    private SerializedProperty satisfiedProperty;       // Represents a bool of whether this Editor's target is satisfied.
    private SerializedProperty hashProperty;            // Represents the number that identified this Editor's target.
    private Condition condition;                        // Reference to the target.
    
    //public bool isExpanded { get; set; }

    private const float conditionButtonWidth = 25f;                     // Width in pixels of the button to remove this Condition from it's array.
    private const float toggleOffset = 22f;                             // Offset to line up the satisfied toggle with its label.
    private const string conditionPropDescriptionName = "description";  // Name of the field that represents the description.
    private const string conditionPropSatisfiedName = "isSatisfied";      // Name of the field that represents whether or not the Condition is satisfied.
    private const string conditionPropHashName = "hash";                // Name of the field that represents the Condition's identifier.
    private const string conditionPropEditorDescriptionName = "editorDescription";
    private const string blankDescription = "No conditions set.";       // Description to use in case no Conditions have been created yet.


    private void OnEnable() {
        // Cache the target.
        condition = (Condition)target;

        // If this Editor has persisted through the destruction of it's target then destroy it.
        if (target == null) {
            DestroyImmediate(this);
            return;
        }

        // Cache the SerializedProperties.
        descriptionProperty = serializedObject.FindProperty(conditionPropDescriptionName);
        satisfiedProperty = serializedObject.FindProperty(conditionPropSatisfiedName);
        hashProperty = serializedObject.FindProperty(conditionPropHashName);
    }


    public override void OnInspectorGUI() {

        // Update the state of the serializedObject to the current values of the target.
        serializedObject.Update();

        // Call different GUI depending where the Condition is.
        switch (editorType) {
            case EditorType.AllConditionAsset:
                AllConditionsAssetGUI();
                break;
            case EditorType.ConditionAsset:
                ConditionAssetGUI();
                break;
            case EditorType.ConditionCollection:
                InteractableGUI();
                break;
            default:
                throw new UnityException("Unknown ConditionEditor.EditorType.");
        }

    }


    // This is displayed for each Condition when the AllConditions asset is selected.
    private void AllConditionsAssetGUI() {
        editorGUI(true);
    }


    // This is displayed when a single Condition asset is selected as a child of the AllConditions asset.
    private void ConditionAssetGUI() {
        editorGUI(false);
    }

    private void editorGUI(bool removeButton) {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
        // Display the description of the Condition.

        string title = "";
        for (int i = 0; i < condition.description.Length && i < 20; i++) {
            title += condition.description[i];
        }

        if (condition.description.Length > title.Length)
            title += "...";

        condition.isExpanded = EditorGUILayout.Foldout(condition.isExpanded, new GUIContent(title), true, EditorStyles.foldout);

        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.indentLevel+=7;
        EditorGUILayout.ToggleLeft("is satisfied?", condition.isSatisfied, GUILayout.MinWidth(0));
        EditorGUI.indentLevel -=7;
        EditorGUI.EndDisabledGroup();
     
        EditorGUI.BeginDisabledGroup(Application.isPlaying);
        
        // Display a button showing a '-' that if clicked removes this Condition from the AllConditions asset.
        if (removeButton && EditorTools.createListButton("-", true, GUILayout.Width(conditionButtonWidth)))
            AllConditionsEditor.RemoveCondition(condition);

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndHorizontal();

        if (condition.isExpanded) {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.LabelField("Condition Name");
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
            condition.description = EditorGUILayout.TextField(condition.description);
            
            if (GUILayout.Button(" Rename ", GUILayout.ExpandWidth(false))) {
               condition = ModifyConditionName(condition);
                // Mark the AllConditions asset as dirty so the editor knows to save changes to it when a project save happens.
                EditorUtility.SetDirty(AllConditions.Instance);
                // Recreate the condition description array with the new added Condition.
                AllConditionsEditor.SetAllConditionDescriptions();
                AssetDatabase.SaveAssets();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
            EditorGUILayout.LabelField("Editor Description");
            condition.editorDescription = EditorGUILayout.TextArea(condition.editorDescription, EditorStyles.textArea, GUILayout.Height(EditorGUIUtility.singleLineHeight * 5));
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
        }

        EditorGUILayout.EndVertical();
    }


    private void InteractableGUI() {
        // Pull the information from the target into the serializedObject.
        serializedObject.Update();

        // The width for the Popup, Toggle and remove Button.
        float popupWidth = EditorGUIUtility.currentViewWidth / 2f;
        float width = popupWidth / 2;

        EditorGUILayout.BeginHorizontal();

        // Find the index for the target based on the AllConditions array.
        int conditionIndex = AllConditionsEditor.TryGetConditionIndex(condition);

        
        // If the target can't be found in the AllConditions array use the first condition.
        if (conditionIndex == -1)
            conditionIndex = 0;

        // Info icon with the editor description of the condition as tooltip
        Texture icon = EditorGUIUtility.IconContent("console.infoicon.sml").image;
        EditorGUILayout.LabelField(new GUIContent(icon, condition.editorDescription), GUILayout.Width(20f));
        
        // Set the index based on the user selection of the condition by the user.
        conditionIndex = EditorGUILayout.Popup(conditionIndex, AllConditionsEditor.AllConditionDescriptions,
            GUILayout.Width(popupWidth - toggleOffset * 2.5f));

        // Find the equivalent condition in the AllConditions array.
        Condition globalCondition = ScriptableObjectUtility.TryGetScriptableObjectAt(conditionIndex, AllConditions.Instance.conditions);

        EditorGUILayout.LabelField("", GUILayout.Width(toggleOffset));

        // Set the description based on the globalCondition's description.
        descriptionProperty.stringValue = globalCondition != null ? globalCondition.description : blankDescription;

        // Set the hash based on the description.
        hashProperty.intValue = Animator.StringToHash(descriptionProperty.stringValue);
        

        // Display the toggle for the satisfied bool.
        EditorGUILayout.PropertyField(satisfiedProperty, GUIContent.none, GUILayout.MaxWidth(width - toggleOffset));

        
        // Display a button with a '-' that when clicked removes the target from the ConditionCollection's conditions array.
        if (EditorTools.createListButton("-", true, GUILayout.Width(conditionButtonWidth))) {
            DestroyImmediate(this);
            requiredConditionsProperty.RemoveFromObjectArray(target);
        }

        EditorGUILayout.EndHorizontal();

        // Push all changes made on the serializedObject back to the target.
        serializedObject.ApplyModifiedProperties();
    }


    // This function is static such that it can be called without an editor being instanced.
    public static Condition CreateCondition() {
        // Create a new instance of Condition.
        Condition newCondition = CreateInstance<Condition>();

        string blankDescription = "No conditions set.";
        
        // Try and set the new condition's description to the first condition in the AllConditions array.
        Condition globalCondition = ScriptableObjectUtility.TryGetScriptableObjectAt(0, AllConditions.Instance.conditions);
        newCondition.description = globalCondition != null ? globalCondition.description : blankDescription;

        // Set the hash based on this description.
        SetHash(newCondition);

        EditorUtility.SetDirty(newCondition);

        return newCondition;
    }


    public static Condition CreateCondition(string description) {
        // Create a new instance of the Condition.
        Condition newCondition = CreateInstance<Condition>();

        // Set the description and the hash based on it.
        newCondition.description = description;
        SetHash(newCondition);

        EditorUtility.SetDirty(newCondition);

        return newCondition;
    }

    private static Condition ModifyConditionName(Condition conditionToModify) {

        conditionToModify.name = conditionToModify.description;
        SetHash(conditionToModify);

        return conditionToModify;
        
    }

    private static void SetHash(Condition condition) {
        condition.hash = Animator.StringToHash(condition.description);
    }
}
