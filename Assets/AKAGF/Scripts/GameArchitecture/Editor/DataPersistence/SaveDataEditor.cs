using System;
using AKAGF.GameArchitecture.ScriptableObjects.DataPersistence;
using UnityEngine;
using UnityEditor;
using AKAeditor;

[CustomEditor(typeof(SaveData))]
public class SaveDataEditor : Editor {

    public enum EditorType {
        SINGLE_ASSET, ALL_ASSETS
    }

    public EditorType editorType;

    private SaveData saveData;                          // Reference to the target.
    private Action<bool> boolSpecificGUI;               // Delegate for the GUI that represents bool values.
    private Action<int> intSpecificGUI;                 // Delegate for the GUI that represents int values.
    private Action<string> stringSpecificGUI;           // Delegate for the GUI that represents string values.
    private Action<Vector3> vector3SpecificGUI;         // Delegate for the GUI that represents Vector3 values.
    private Action<Quaternion> quaternionSpecificGUI;   // Delegate for the GUI that represents Quaternion values.

    private const float buttonWidth = 25f;

    private void OnEnable () {
        // Cache the reference to the target.
        saveData = (SaveData)target;

        // Set the values of the delegates to various 'read-only' GUI functions.
        boolSpecificGUI = value => { EditorGUILayout.Toggle(value); };
        intSpecificGUI = value => { EditorGUILayout.LabelField(value.ToString()); };
        stringSpecificGUI = value => { EditorGUILayout.LabelField (value); };
        vector3SpecificGUI = value => { EditorGUILayout.Vector3Field (GUIContent.none, value); };
        quaternionSpecificGUI = value => { EditorGUILayout.Vector3Field (GUIContent.none, value.eulerAngles); };
    }

    public override void OnInspectorGUI() {

        // Call different GUI depending where the Scriptable Scene is.
        switch (editorType) {
            case EditorType.SINGLE_ASSET:
                drawSingleAssetEditor();
                break;
            case EditorType.ALL_ASSETS:
                drawAllAssetsEditor();
                break;
            default:
                throw new UnityException("Unknown SaveDataEditor.EditorType.");
        }
    }

    private void drawSingleAssetEditor() {
        editorGUI(false);
    }


    private void drawAllAssetsEditor() {
        editorGUI(true);
    }


    public void editorGUI(bool removeButton) {

        EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);

        saveData.isExpanded = EditorGUILayout.Foldout(saveData.isExpanded, new GUIContent(saveData.name), true, EditorStyles.foldout);

            serializedObject.Update();

             // Display a button showing a '-' that if clicked removes this asset from the parent asset.
             if (removeButton && EditorTools.createListButton("-", true, GUILayout.Width(buttonWidth))) {
                    AllSavesDataEditor.RemoveSaveData(saveData);
                    return;
             }   
                
            GUILayout.EndHorizontal();


        if (saveData.isExpanded) {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.LabelField(new GUIContent("SaveData Name"));

            GUILayout.BeginHorizontal();
            saveData.name = EditorGUILayout.TextField(saveData.name);
            if (GUILayout.Button("Rename", GUILayout.ExpandWidth(false))) {
                AssetDatabase.SaveAssets();
            }
            EditorGUI.indentLevel -= 1;
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField(new GUIContent("Dictionaries"), GUILayout.ExpandWidth(false));
            GUILayout.Space(5);
            // Display all the values for each data type.
            KeyValuePairListsGUI("Bools", saveData.boolKeyValuePairLists, boolSpecificGUI);
            KeyValuePairListsGUI("Integers", saveData.intKeyValuePairLists, intSpecificGUI);
            KeyValuePairListsGUI("Strings", saveData.stringKeyValuePairLists, stringSpecificGUI);
            KeyValuePairListsGUI("Vector3s", saveData.vector3KeyValuePairLists, vector3SpecificGUI);
            KeyValuePairListsGUI("Quaternions", saveData.quaternionKeyValuePairLists, quaternionSpecificGUI);

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();
    }


    private void KeyValuePairListsGUI<T> (string label, SaveData.KeyValuePairLists<T> keyvaluePairList, Action<T> specificGUI) {
        // Surround each data type in a box.
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        // Display a label for this data type.
        EditorGUILayout.LabelField (label);

        // If there are data elements...
        if (keyvaluePairList.keys.Count > 0)
        {
            // ... go through each of them...
            for (int i = 0; i < keyvaluePairList.keys.Count; i++)
            {
                EditorGUILayout.BeginHorizontal ();

                // ... and display a label for each followed by GUI specific to their type.
                EditorGUILayout.LabelField (keyvaluePairList.keys[i]);
                specificGUI (keyvaluePairList.values[i]);

                EditorGUILayout.EndHorizontal ();
            }
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }


    public static SaveData createPersistentDataSave() {
        // Create the instance of the scriptableObject
        return CreateInstance<SaveData>();
    }
}
