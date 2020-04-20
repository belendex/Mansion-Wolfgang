using AKAGF.GameArchitecture.MonoBehaviours.Interaction;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Conditions;
using UnityEngine;
using UnityEditor;
using AKAeditor;

// This is the Editor for the Interactable MonoBehaviour.
// However, since the Interactable contains many sub-objects, 
// it requires many sub-editors to display them.
// For more details see the EditorWithSubEditors class.
[CustomEditor(typeof(Interactable))]
public class InteractableEditor : EditorWithSubEditors<ConditionCollectionEditor, ConditionCollection> {

    private Interactable interactable;                              // Reference to the target.
    private SerializedProperty collectionsProperty;                 // Represents the ConditionCollection array on the Interactable.
    private SerializedProperty defaultReactionCollectionProperty;   // Represents the ReactionCollection which is used if none of the ConditionCollections are.

    private SerializedProperty randomDefaultReactionProperty;       // Represents the boolean field that determines if the defaultReactions will be random. 
    private SerializedProperty defaultReactionIndexProperty;        // Represents the integer that defines the defaultReaction index in case that there were more than one DefaultReaction and randomReaction is disabled


    private bool[] isExpanded = new bool[2];

    private const float collectionButtonWidth = 125f;                                                   // Width in pixels of the button for adding to the ConditionCollection array.
    private const string interactablePropConditionCollectionsName = "conditionCollections";             // Name of the ConditionCollection array.
    private const string interactablePropDefaultReactionCollectionName = "defaultReactionCollection";   // Name of the ReactionCollection field which is used if none of the ConditionCollections are.

    private const string randomDefaultReactionPropertyName = "randomDefaultReaction";
    private const string defaultReactionIndexPropertyName = "defaultReactionIndex";


    private void OnEnable (){
        // Cache the target reference.
        interactable = (Interactable)target;

        // Cache the SerializedProperties.
        collectionsProperty = serializedObject.FindProperty(interactablePropConditionCollectionsName);
        defaultReactionCollectionProperty = serializedObject.FindProperty(interactablePropDefaultReactionCollectionName);

        randomDefaultReactionProperty = serializedObject.FindProperty(randomDefaultReactionPropertyName);
        defaultReactionIndexProperty = serializedObject.FindProperty(defaultReactionIndexPropertyName);

        // Create the necessary Editors for the ConditionCollections.
        CheckAndCreateSubEditors(interactable.conditionCollections);
    }


    private void OnDisable (){
        // When the InteractableEditor is disabled, destroy all the ConditionCollection editors.
        CleanupEditors ();
    }


    // This is called when the ConditionCollection editors are created.
    protected override void SubEditorSetup(ConditionCollectionEditor editor){
        // Give the ConditionCollection editor a reference to the array to which it belongs.
        editor.collectionsProperty = collectionsProperty;
    }


    public override void OnInspectorGUI (){
        // Pull information from the target into the serializedObject.
        serializedObject.Update ();

        EditorTools.createHorizontalSeparator();
        EditorTools.createTitleBox("Interactable Reactions", true);

        EditorGUILayout.Space();

        // If necessary, create editors for the ConditionCollections.
        CheckAndCreateSubEditors(interactable.conditionCollections);

        EditorGUILayout.BeginVertical(GUI.skin.box);
       
        GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);

        isExpanded[0] = EditorGUILayout.Foldout(isExpanded[0], new GUIContent("Conditioned Reactions"), true, EditorStyles.foldout);

        EditorGUILayout.EndHorizontal();

        if (isExpanded[0]) {

            GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
            // Create a right-aligned button which when clicked, creates a new ConditionCollection in the ConditionCollections array.
            if (GUILayout.Button("Add Condition Collection", GUILayout.ExpandWidth(false))) {
                ConditionCollection newCollection = ConditionCollectionEditor.CreateConditionCollection();
                collectionsProperty.AddToObjectArray(newCollection);
            }

            EditorGUILayout.EndHorizontal();

            // Display all of the ConditionCollections.
            for (int i = 0; i < subEditors.Length; i++){
                
                subEditors[i].OnInspectorGUI ();
                
                EditorGUILayout.Separator();
            }

        }

        EditorGUILayout.EndVertical();


        EditorGUILayout.Separator();


        EditorGUILayout.BeginVertical(GUI.skin.box);

        GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);

        isExpanded[1] = EditorGUILayout.Foldout(isExpanded[1], new GUIContent("Default Reactions"), true, EditorStyles.foldout);

        EditorGUILayout.EndHorizontal();

        //EditorGUILayout.PropertyField(defaultReactionCollectionProperty.FindPropertyRelative("Array.size"));
        if (isExpanded[1]) {
            //Show options for defaultReaction only when there was more than one DefaultReactionCollection
            if (defaultReactionCollectionProperty.arraySize > 1) {

                EditorGUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
                EditorGUILayout.PropertyField(randomDefaultReactionProperty);

                //If the random DefaultReaction is disabled, show the texfield for the defaultReactionCollection index
                if (!randomDefaultReactionProperty.boolValue) {
                    EditorGUILayout.PropertyField(defaultReactionIndexProperty);
                }

                EditorGUILayout.EndHorizontal();
            }

            //Buttons to add or remove default ReactionCollection
            EditorGUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
            if (GUILayout.Button("+", GUILayout.Width(collectionButtonWidth))) {
                defaultReactionCollectionProperty.arraySize++;
            }

            if (GUILayout.Button("-", GUILayout.Width(collectionButtonWidth)) && defaultReactionCollectionProperty.arraySize > 0) {
                defaultReactionCollectionProperty.arraySize--;
            }
            EditorGUILayout.EndHorizontal();

            //Fields for the array of ReactionCollections
            
            for (int i = 0; i < defaultReactionCollectionProperty.arraySize; i++) {
                EditorGUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.PropertyField(defaultReactionCollectionProperty.GetArrayElementAtIndex(i), new GUIContent("Default Reaction " + i));
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Separator();

        EditorTools.createHorizontalSeparator();
        EditorTools.createTitleBox("Interactable Events", true);

        EditorGUILayout.Space();


        EditorGUILayout.PropertyField(serializedObject.FindProperty("onInteractionStart"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onInteractionEnd"), true);

        EditorGUILayout.Separator();

        EditorTools.createHorizontalSeparator();

        GUI.enabled = Application.isPlaying;

        if (GUILayout.Button("Interact")) {
            interactable.Interact();
        }
        
        // Push information back to the target from the serializedObject.
        serializedObject.ApplyModifiedProperties ();
    }
}
