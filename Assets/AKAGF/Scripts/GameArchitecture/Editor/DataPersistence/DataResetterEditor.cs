using UnityEngine;
using UnityEditor;
using AKAeditor;
using AKAGF.GameArchitecture.MonoBehaviours.DataPersistence;
using AKAGF.GameArchitecture.ScriptableObjects.Abstracts;

[CustomEditor(typeof(DataResetter))]
public class DataResetterEditor : Editor {
    private DataResetter dataResetter;              // Reference to the target of this Editor.
    private SerializedProperty resettersProperty;   // Represents the only field in the target.


    private const string dataResetterPropResettableScriptableObjectsName = "resettableScriptableObjects";
    // The name of the field to be represented.

   
    private void OnEnable () {

       EditorApplication.playModeStateChanged += ModeChanged;
        // Cache the property and target.
        resettersProperty = serializedObject.FindProperty(dataResetterPropResettableScriptableObjectsName);

        dataResetter = (DataResetter)target;

        // If the array is null, initialise it to prevent null refs.
        if (dataResetter.resettableScriptableObjects == null) {
            dataResetter.resettableScriptableObjects = new ResettableScriptableObject[0];
        }
    }

    
    void ModeChanged(PlayModeStateChange state) {
        if (!EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying) {
            Debug.Log("Exiting playmode.");
            dataResetter.resetScriptableObjects() ;
        }
    }


    public override void OnInspectorGUI () {
        // Update the state of the serializedObject to the current values of the target.
        serializedObject.Update();

        EditorTools.createArrayPropertyButtons(resettersProperty, "Resettable Datas", GUILayout.ExpandWidth(true), true, true);

        // Go through all the resetters and create GUI appropriate for them.
        for (int i = 0; i < dataResetter.resettableScriptableObjects.Length; i++) {
            GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);

            dataResetter.resettableScriptableObjects[i] = 
                EditorGUILayout.ObjectField(dataResetter.resettableScriptableObjects[i], 
                                            typeof(ResettableScriptableObject), false) as ResettableScriptableObject;

            if (EditorTools.createListButton(" - ", true, GUILayout.ExpandWidth(false))){
                // Record all operations so they can be undone.
                Undo.RecordObject(dataResetter, "Remove ressetable scriptable");

                // Remove the specified item from the array.
                ArrayUtility.Remove(ref dataResetter.resettableScriptableObjects, dataResetter.resettableScriptableObjects[i]);

                // Destroy the item, including it's asset and save the assets to recognise the change.
                AssetDatabase.SaveAssets();

                return;
            }

            GUILayout.EndHorizontal();
        }

   
        // Push the values from the serializedObject back to the target.
        serializedObject.ApplyModifiedProperties();
    }
}
