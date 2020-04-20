using UnityEngine;
using UnityEditor;
using AKAeditor;
using AKAGF.GameArchitecture.Literals;
using AKAGF.GameArchitecture.ScriptableObjects.SceneControl;

[CustomEditor(typeof(AllGameScriptableScenes))]
[InitializeOnLoad]
public class AllGameScriptableScenesEditor : Editor {

    private AllGameScriptableScenes allScenes;
    private ScriptableSceneEditor[] scenesEditors;
    private const string BASE_PATH = AKAGF_PATHS.SINGLETONS_FULLPATH;
    private const string NEW_SCENE_BASE_NAME = AKAGF_PATHS.NEW_SCRIPTABLE_SCENE_BASE_NAME;


    // Call this function when the menu item is selected.
    [MenuItem(AKAGF_PATHS.AKAGF_MENU_FULL_PATH + 
                    AKAGF_PATHS.SCENE_MNGMNT_MENU_PATH +
                            AKAGF_PATHS.ALLSCENES_MENU_NAME)]
    public static void CreateAllGameScriptableScenesAsset() {
        // Create an instance of the All object and make an asset for it.
        AllGameScriptableScenes.Instance =
            ScriptableObjectUtility.CreateSingletonScriptableObject(
                AllGameScriptableScenes.Instance,
                BASE_PATH);

        // Create a new empty array if there is need of it.
        if (AllGameScriptableScenes.Instance.scriptableScenes == null)
            AllGameScriptableScenes.Instance.scriptableScenes = new ScriptableScene[0];
    }

     private void OnDestroy() {
        ScriptableObjectUtility.RemoveScriptableObjectFromPreloadedAssets(AllGameScriptableScenes.Instance);
    }

    private void OnEnable() {
        // Cache the reference to the target.
        allScenes = (AllGameScriptableScenes)target;

        if (allScenes.scriptableScenes == null)
            allScenes.scriptableScenes = new ScriptableScene[0];

        // If there aren't any editors, create them.
        if (scenesEditors == null) {
            CreateEditors();
        }
    }

    private void OnDisable() {
        // Destroy all the editors.
        for (int i = 0; i < scenesEditors.Length; i++) {
            DestroyImmediate(scenesEditors[i]);
        }

        // Null out the editor array.
        scenesEditors = null;
    }

    public override void OnInspectorGUI () {
        // Update the state of the serializedObject to the current values of the target.
        serializedObject.Update();

        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorTools.createTitleBox("ALL GAME SCENES MASTER CONTROL");

        EditorGUILayout.BeginHorizontal ();

        if (EditorTools.createListButton(" Add New Scriptable Scene ", false,  GUILayout.ExpandWidth(true))) {

            AddScriptableScene();
        }
        EditorGUILayout.EndHorizontal ();


        // If there are different number of editors to Conditions, create them afresh.
        if (scenesEditors.Length != allScenes.scriptableScenes.Length) {
            // Destroy all the old editors.
            for (int i = 0; i < scenesEditors.Length; i++) {
                DestroyImmediate(scenesEditors[i]);
            }

            // Create new editors.
            CreateEditors ();
        }

        // Display all the conditions.
        for (int i = 0; i < scenesEditors.Length; i++) {
            scenesEditors[i].OnInspectorGUI();
        }

        //// If there are conditions, add a gap.
        //if (scenesEditors.Length > 0) {
        //    EditorGUILayout.Space ();
        //    EditorGUILayout.Space ();
        //}

        EditorGUILayout.EndVertical();

        // Push the values from the serializedObject back to the target.
        serializedObject.ApplyModifiedProperties();
    }

    private void CreateEditors ()  {
        // Create a new array for the editors which is the same length at the conditions array.
        scenesEditors = new ScriptableSceneEditor[allScenes.scriptableScenes.Length];

        // Go through all the empty array...
        for (int i = 0; i < scenesEditors.Length; i++){
            // ... and create an editor with an editor type to display correctly.
            scenesEditors[i] = CreateEditor(ScriptableObjectUtility.TryGetScriptableObjectAt(i, allScenes.scriptableScenes)) as ScriptableSceneEditor;
            scenesEditors[i].editorType = ScriptableSceneEditor.EditorType.ALL_ASSETS;
        }
    }

    private void AddScriptableScene() {

        // If there isn't an AllConditions instance yet, put a message in the console and return.
        if (!AllGameScriptableScenes.Instance) {
            Debug.LogError("AllGameScriptableScenes has not been created yet.");
            return;
        }

        ScriptableScene newScene = ScriptableSceneEditor.CreateScriptableScene();
        newScene.name = NEW_SCENE_BASE_NAME;

        ScriptableObjectUtility.AddScriptableObject(AllGameScriptableScenes.Instance, ref newScene, ref AllGameScriptableScenes.Instance.scriptableScenes, "Created new Scriptable Scene");
    }

    public static void RemoveScriptableScene(ScriptableScene sceneToRemove) {

        // If there isn't an AllConditions instance yet, put a message in the console and return.
        if (!AllGameScriptableScenes.Instance) {
            Debug.LogError("AllGameScriptableScenes has not been created yet.");
            return;
        }

        ScriptableObjectUtility.RemoveScriptableObject(AllGameScriptableScenes.Instance, ref sceneToRemove, ref AllGameScriptableScenes.Instance.scriptableScenes);
    }
}
