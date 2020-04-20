using UnityEngine;
using UnityEditor;
using AKAeditor;
using AKAGF.GameArchitecture.Literals;
using AKAGF.GameArchitecture.ScriptableObjects.DataPersistence;

[CustomEditor(typeof(AllSavesData))]
[InitializeOnLoad]
public class AllSavesDataEditor : Editor {

    private AllSavesData allSavesDatas;
    private SaveDataEditor[] savesDataEditors;
    private static string newSaveDataName = "New Persistent Data";
    private const float buttonWidth = 30f;

    private const string BASE_PATH = AKAGF_PATHS.SINGLETONS_FULLPATH;

    // Call this function when the menu item is selected.
    [MenuItem(AKAGF_PATHS.AKAGF_MENU_FULL_PATH +
                    AKAGF_PATHS.SCENE_MNGMNT_MENU_PATH +
                        AKAGF_PATHS.ALLSCENESAVESDATAS_MENU_NAME)]
    public static void CreateAllSavesDataAsset() {

        // Create an instance of the All object and make an asset for it.
        AllSavesData.Instance =
            ScriptableObjectUtility.CreateSingletonScriptableObject(
                AllSavesData.Instance,
                BASE_PATH);

        // Create a new empty array of Conditions.
        if (AllSavesData.Instance.saveDatas == null)
            AllSavesData.Instance.saveDatas = new SaveData[0];
    }

    private void OnDestroy() {
        ScriptableObjectUtility.RemoveScriptableObjectFromPreloadedAssets(AllSavesData.Instance);
    }

    private void OnEnable() {
        // Cache the reference to the target.
        allSavesDatas = (AllSavesData)target;

        if (allSavesDatas.saveDatas == null)
            allSavesDatas.saveDatas = new SaveData[0];

        // If there aren't any editors, create them.
        if (savesDataEditors == null) {
            CreateEditors();
        }
    }

    private void OnDisable() {

        if (savesDataEditors == null)
            return;

        // Destroy all the editors.
        for (int i = 0; i < savesDataEditors.Length; i++) {
            DestroyImmediate(savesDataEditors[i]);
        }

        // Null out the editor array.
        savesDataEditors = null;
    }

    public override void OnInspectorGUI () {
        // Update the state of the serializedObject to the current values of the target.
        serializedObject.Update();

        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorTools.createTitleBox("ALL GAME PERSISTENT DATA MASTER CONTROL");

        EditorGUILayout.BeginHorizontal ();

        newSaveDataName = EditorGUILayout.TextField(GUIContent.none, newSaveDataName);

        if (EditorTools.createListButton("+", false, GUILayout.Width(buttonWidth))) {
            AddSaveData(newSaveDataName);
            newSaveDataName = "New Persistent Data";
        }

        EditorGUILayout.EndHorizontal ();


        // If there are different number of editors to Conditions, create them afresh.
        if (savesDataEditors.Length != allSavesDatas.saveDatas.Length) {
            // Destroy all the old editors.
            for (int i = 0; i < savesDataEditors.Length; i++) {
                DestroyImmediate(savesDataEditors[i]);
            }

            // Create new editors.
            CreateEditors ();
        }

        // Display all the conditions.
        for (int i = 0; i < savesDataEditors.Length; i++) {
            savesDataEditors[i].OnInspectorGUI ();
        }

        // If there are conditions, add a gap.
        //if (savesDataEditors.Length > 0) {
        //    EditorGUILayout.Space ();
        //    EditorGUILayout.Space ();
        //}

        EditorGUILayout.EndVertical();

        // Push the values from the serializedObject back to the target.
        serializedObject.ApplyModifiedProperties();
    }

    private void CreateEditors ()  {
        // Create a new array for the editors which is the same length at the savesData array.
        savesDataEditors = new SaveDataEditor[allSavesDatas.saveDatas.Length];

        // Go through all the empty array...
        for (int i = 0; i < savesDataEditors.Length; i++){
            // ... and create an editor with an editor type to display correctly.
            savesDataEditors[i] = CreateEditor(ScriptableObjectUtility.TryGetScriptableObjectAt(i, allSavesDatas.saveDatas)) as SaveDataEditor;
            savesDataEditors[i].editorType = SaveDataEditor.EditorType.ALL_ASSETS;
        }
    }

    private void AddSaveData(string newSaveDataName=null) {
        // If there isn't an AllConditions instance yet, put a message in the console and return.
        if (!AllSavesData.Instance) {
            Debug.LogError("AllSavesData has not been created yet.");
            return;
        }

        // Create a condition based on the description.
        SaveData newSaveData = SaveDataEditor.createPersistentDataSave();

        newSaveData.name = (newSaveDataName != null) ? newSaveDataName : "New SaveData";

        ScriptableObjectUtility.AddScriptableObject(AllSavesData.Instance,
                                        ref newSaveData,
                                        ref AllSavesData.Instance.saveDatas,
                                        "Created new SaveData"
            );

    }

    public static void RemoveSaveData(SaveData saveDataToRemove) {
        // If there isn't an AllConditions asset, do nothing.
        if (!AllSavesData.Instance) {
            Debug.LogError("AllSavesData has not been created yet.");
            return;
        }

        ScriptableObjectUtility.RemoveScriptableObject(AllSavesData.Instance,
                                        ref saveDataToRemove,
                                        ref AllSavesData.Instance.saveDatas
            );
    }
}
