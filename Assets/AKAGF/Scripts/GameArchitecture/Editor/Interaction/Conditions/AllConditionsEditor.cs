using UnityEngine;
using UnityEditor;
using AKAeditor;
using AKAGF.GameArchitecture.Literals;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Conditions;

[CustomEditor(typeof(AllConditions))]
[InitializeOnLoad]
public class AllConditionsEditor : Editor {
    // Property for accessing the descriptions for all the Conditions.
    // This is used for the Popups on the ConditionEditor.
    public static string[] AllConditionDescriptions {
        get {
            // If the description array doesn't exist yet, set it.
            if (allConditionDescriptions == null) {
                SetAllConditionDescriptions ();
            }

            return allConditionDescriptions;
        }

        private set { allConditionDescriptions = value; }
    }


    private static string[] allConditionDescriptions;           // Field to store the descriptions of all the Conditions.

    private ConditionEditor[] conditionEditors;                 // All of the subEditors to display the Conditions.
    private AllConditions allConditions;                        // Reference to the target.
    private static string newConditionDescription = "New Condition";   // String to start off the naming of new Conditions.


    private const string BASE_PATH = AKAGF_PATHS.SINGLETONS_FULLPATH; // The path that the AllConditions asset is created at.

    private const float buttonWidth = 30f;                      // Width in pixels of the button to create Conditions.


    // Call this function when the menu item is selected.
    [MenuItem(AKAGF_PATHS.AKAGF_MENU_FULL_PATH + 
                    AKAGF_PATHS.GAME_FLOW_MENU_PATH + 
                          AKAGF_PATHS.ALLCONDITIONS_MENU_NAME)]

    public static void CreateAllConditionsAsset() {

        // Create an instance of the AllConditions object and make an asset for it.
        AllConditions.Instance =
            ScriptableObjectUtility.CreateSingletonScriptableObject(
                AllConditions.Instance,
                BASE_PATH);

        // Create a new empty array if there is need of it.
        if (AllConditions.Instance.conditions == null)
            AllConditions.Instance.conditions = new Condition[0];

    }

    private void OnDestroy() {
        ScriptableObjectUtility.RemoveScriptableObjectFromPreloadedAssets(AllConditions.Instance);
    }

    private void OnEnable() {
        // Cache the reference to the target.
        allConditions = (AllConditions)target;

        // If there aren't any Conditions on the target, create an empty array of Conditions.
        if (allConditions.conditions == null)
            allConditions.conditions = new Condition[0];

        // If there aren't any editors, create them.
        if (conditionEditors == null) {
            CreateEditors();
        }
    }

    
    private void OnDisable() {
        // Destroy all the editors.
        for (int i = 0; i < conditionEditors.Length; i++) {
            DestroyImmediate(conditionEditors[i]);
        }

        // Null out the editor array.
        conditionEditors = null;
    }


    public static void SetAllConditionDescriptions () {
        // Create a new array that has the same number of elements as there are Conditions.
        AllConditionDescriptions = new string[ScriptableObjectUtility.TryGetScriptablesArrayLength(AllConditions.Instance.conditions)];

        Condition auxCon;

        // Go through the array and assign the description of the condition at the same index.
        for (int i = 0; i < AllConditionDescriptions.Length; i++) {
            auxCon = ScriptableObjectUtility.TryGetScriptableObjectAt(i, AllConditions.Instance.conditions) as Condition;
            AllConditionDescriptions[i] = auxCon.description;
        }
    }


    public override void OnInspectorGUI () {
        // Update the state of the serializedObject to the current values of the target.
        serializedObject.Update();

        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorTools.createTitleBox("ALL GAME CONDITIONS MASTER CONTROL");

        EditorGUILayout.BeginHorizontal ();
        
        // Get and display a string for the name of a new Condition.
        newConditionDescription = EditorGUILayout.TextField (GUIContent.none, newConditionDescription);

        // Display a button that when clicked adds a new Condition to the AllConditions asset and resets the new description string.
        if (EditorTools.createListButton("+", false, GUILayout.Width(buttonWidth))) {
            AddCondition (newConditionDescription);
            newConditionDescription = "New condition name" ;
        }
        EditorGUILayout.EndHorizontal ();


        // If there are different number of editors to Conditions, create them afresh.
        if (conditionEditors.Length != ScriptableObjectUtility.TryGetScriptablesArrayLength(AllConditions.Instance.conditions)) {
            // Destroy all the old editors.
            for (int i = 0; i < conditionEditors.Length; i++) {
                DestroyImmediate(conditionEditors[i]);
            }

            // Create new editors.
            CreateEditors ();
        }

        // Display all the conditions.
        for (int i = 0; i < conditionEditors.Length; i++) {
            conditionEditors[i].OnInspectorGUI ();
        }

        // If there are conditions, add a gap.
        //if (ScriptableObjectUtility.TryGetScriptablesArrayLength(AllConditions.Instance.conditions) > 0) {
        //    EditorGUILayout.Space ();
        //    EditorGUILayout.Space ();
        //}

        EditorGUILayout.EndVertical();

        // Push the values from the serializedObject back to the target.
        serializedObject.ApplyModifiedProperties();
    }


    private void CreateEditors ()  {
        // Create a new array for the editors which is the same length at the conditions array.
        conditionEditors = new ConditionEditor[allConditions.conditions.Length];

        // Go through all the empty array...
        for (int i = 0; i < conditionEditors.Length; i++) {
            // ... and create an editor with an editor type to display correctly.
            conditionEditors[i] = CreateEditor(ScriptableObjectUtility.TryGetScriptableObjectAt(i, AllConditions.Instance.conditions)) as ConditionEditor;
            conditionEditors[i].editorType = ConditionEditor.EditorType.AllConditionAsset;
        }
    }


    private void AddCondition(string description) {
        // If there isn't an AllConditions instance yet, put a message in the console and return.
        if (!AllConditions.Instance) {
            Debug.LogError("AllConditions has not been created yet.");
            return;
        }

        // Create a condition based on the description.
        Condition newCondition = ConditionEditor.CreateCondition (description);

        // The name is what is displayed by the asset so set that too.
        newCondition.name = description;

        ScriptableObjectUtility.AddScriptableObject(AllConditions.Instance, 
                                        ref newCondition,
                                        ref AllConditions.Instance.conditions,
                                        "Created new Condition"
            );

        // Recreate the condition description array with the new added Condition.
        SetAllConditionDescriptions ();
    }

    
    public static void RemoveCondition(Condition condition) {
        // If there isn't an AllConditions asset, do nothing.
        if (!AllConditions.Instance) {
            Debug.LogError("AllConditions has not been created yet.");
            return;
        }

        ScriptableObjectUtility.RemoveScriptableObject(AllConditions.Instance,
                                        ref condition,
                                        ref AllConditions.Instance.conditions
            );

        // Recreate the condition description array without the removed condition.
        SetAllConditionDescriptions ();
    }


    public static int TryGetConditionIndex (Condition condition) {
        // Go through all the Conditions...
        int length = ScriptableObjectUtility.TryGetScriptablesArrayLength(AllConditions.Instance.conditions);
        for (int i = 0; i < length ; i++) {

            Condition auxCon = ScriptableObjectUtility.TryGetScriptableObjectAt(i, AllConditions.Instance.conditions) as Condition;
            // ... and if one matches the given Condition, return its index.
            if (auxCon.hash == condition.hash)
                return i;
        }

        // If the Condition wasn't found, return -1.
        return -1;
    }


}
