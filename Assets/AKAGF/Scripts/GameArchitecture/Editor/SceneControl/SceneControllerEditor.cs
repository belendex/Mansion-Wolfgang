using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AKAeditor;
using AKAGF.GameArchitecture.Literals;
using AKAGF.GameArchitecture.MonoBehaviours.SceneControl;
using AKAGF.GameArchitecture.ScriptableObjects.DataPersistence;
using AKAGF.GameArchitecture.ScriptableObjects.SceneControl;
using AKAGF.GameArchitecture.MonoBehaviours.GUI;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using System.Linq;
using UnityEngine.SceneManagement ;


[CustomEditor(typeof(SceneController))]
public class SceneControllerEditor : Editor {

    private SceneController targetSceneController;

    private void OnEnable() {

        targetSceneController = target as SceneController;
    }


    public override void OnInspectorGUI() {

        serializedObject.Update();

        targetSceneController.playerSaveData = EditorGUILayout.ObjectField("Player Save Data", targetSceneController.playerSaveData, typeof(SaveData), false) as SaveData;

        targetSceneController.sceneScreenFader = EditorGUILayout.ObjectField("Canvas Fader", targetSceneController.sceneScreenFader, typeof(CanvasFader), true) as CanvasFader;

        drawSceneEditor(ref targetSceneController.startingSceneName, ref targetSceneController.initialStartingPositionName);

        serializedObject.ApplyModifiedProperties();

    }


    public static void drawSceneEditor(ref string sceneName, ref string startingPointInLoadedScene) {

        // No instance found of AllGameScriptableScenes
        if (!AllGameScriptableScenes.Instance) {
            string textMessage = "No AllGameScriptableScenes Instance found. " +
                 " Create it in Assets > Create > AKAGF > AllGameScriptableScenes.";

            EditorTools.drawMessage(textMessage, MessageType.Error);
            return;
        }

        // No ScriptableScenes present in the project
        if (AllGameScriptableScenes.Instance.scriptableScenes.Length == 0) {
            string textMessage = "No ScriptableScenes found in AllGameScriptableScenes Instance. " +
                "You must create scriptable Scenes in order to change between them through a Scene Reaction.";

            EditorTools.drawMessage(textMessage, MessageType.Warning);
            return;
        }


        /* Scenes Popup Menu */
        // Add all the scenes Names present in AllGameScenes as popup options
        int listSize = AllGameScriptableScenes.Instance.scriptableScenes.Length;
        List<string> sceneOptions = new List<string>(AllGameScriptableScenes.Instance.scriptableScenes.Length);
        for (int i = 0; i < listSize; i++) {
            // Ensure the ScriptableScene has a valid Scene Attached to it 
            // by not adding to the list the scriptables Scenes present 
            // in AllGameScriptableScenes with the default creation name
            // defined in AKAGF_PATHS struct
            if (!AllGameScriptableScenes.Instance.scriptableScenes[i].name.Equals(AKAGF_PATHS.NEW_SCRIPTABLE_SCENE_BASE_NAME))
                sceneOptions.Add(AllGameScriptableScenes.Instance.scriptableScenes[i].name);
        }

        // All scriptableScenes found don't have a Unity Scene attached to them
        if (sceneOptions.Count == 0) {
            string textMessage = "All scriptableScenes found don't have a Unity Scene attached to them" +
                 " Check it in Project > Resources > AKAGF > SINGLETONS > AllGameScriptableScenes.";

            EditorTools.drawMessage(textMessage, MessageType.Error);
            return;
        }

        int selectedSceneIndex = 0;

        // Check if there is already a SceneName for this Scene Reaction
        if (sceneName != null && !sceneName.Equals("")) {
            for (int i = 0; i < sceneOptions.Count; i++) {
                if (sceneOptions[i].Equals(sceneName)) {
                    selectedSceneIndex = i;
                    break;
                }
            }
        }


        selectedSceneIndex = EditorGUILayout.Popup("Scene To Switch", selectedSceneIndex, sceneOptions.ToArray());

        sceneName = sceneOptions[selectedSceneIndex];

        /* Starting Position Popup Menu */
        ScriptableScene scriptableScene = AllGameScriptableScenes.Instance.scriptableScenes[selectedSceneIndex];
        int posListSize = scriptableScene.sceneStartingPositionsNames.Length;

        // No starting Positions Names in the ScriptableScene
        if (posListSize == 0) {

            string textMessage = "No Starting Position Names found in " + scriptableScene.name +
                " Scriptable Scene. You may had forgotten to add Starting Positions Names to the ScriptableScene " +
                "or may your game doesn't implement starting positions in scenes.";

            EditorTools.drawMessage(textMessage, MessageType.Info);

            return;
        }

        // Add all the startnig Position names present in the ScriptableScenes to the option list
        List<string> startingPointsOptions = new List<string>(posListSize);
        for (int i = 0; i < posListSize; i++) {
            if (!scriptableScene.sceneStartingPositionsNames[i].Equals(""))
                startingPointsOptions.Add(scriptableScene.sceneStartingPositionsNames[i]);
        }

        if (startingPointsOptions.Count != posListSize) {
            string textMessage = "There is at least one StartingPositionName with empty value in " +
                 scriptableScene.name + " Scriptable Scene. You probably forgotten to add Starting Positions Names to the ScriptableScene " +
                "in AllGameScriptableScenes Editor. Check it before you continue.";

            EditorTools.drawMessage(textMessage, MessageType.Error);

            return;
        }

        int selectedStartingPositionIndex = 0;

        // Check if there is already a StartingPointName
        if (startingPointInLoadedScene != null
            && !startingPointInLoadedScene.Equals("")) {

            for (int i = 0; i < startingPointsOptions.Count; i++) {
                if (startingPointsOptions[i].Equals(startingPointInLoadedScene)) {
                    selectedStartingPositionIndex = i;
                    break;
                }
            }
        }


        selectedStartingPositionIndex = EditorGUILayout.Popup("Starting Point", selectedStartingPositionIndex, startingPointsOptions.ToArray());
        GUILayout.Space(5);

        startingPointInLoadedScene = startingPointsOptions[selectedStartingPositionIndex];
    }
}


#region TODO new SceneControllerEditor
//[CustomEditor(typeof(SceneController))]
//public class SceneControllerEditor : Editor {

//    private SceneController targetSceneController;
//    private static int selectedSceneIndex;
//    private ReorderableList persistentScenesList;
//    private int selectedPersistentIndex;
//    private ReorderableList loadedScenesList;
//    private int selectedloadedSceneIndex;


//    private void OnEnable() {

//        targetSceneController = target as SceneController;

//        persistentScenesList = new ReorderableList(targetSceneController.persistentScenes, typeof(ScriptableScene), true, true, true, true);

//        persistentScenesList.drawHeaderCallback += DrawPersistentListHeader;
//        persistentScenesList.drawElementCallback += DrawPersistentScenesListElement;
//        persistentScenesList.onAddCallback += AddPersistentScene;
//        persistentScenesList.onRemoveCallback += RemovePersistentScene;

//        loadedScenesList = new ReorderableList(targetSceneController.gameScenes, typeof(ScriptableScene), true, true, true, true);

//        loadedScenesList.drawHeaderCallback += DrawLoadedScenesListHeader;
//        loadedScenesList.drawElementCallback += DrawLoadedScenesListElement;
//        loadedScenesList.onAddCallback += AddGameScene;
//        loadedScenesList.onRemoveCallback += RemoveGameScene;
//    }

//    private void OnDisable() {
//        // Make sure we don't get memory leaks etc.
//        persistentScenesList.drawHeaderCallback -= DrawPersistentListHeader;
//        persistentScenesList.drawElementCallback -= DrawPersistentScenesListElement;
//        persistentScenesList.onAddCallback -= AddPersistentScene;
//        persistentScenesList.onRemoveCallback -= RemovePersistentScene;

//        loadedScenesList.drawHeaderCallback -= DrawLoadedScenesListHeader;
//        loadedScenesList.drawElementCallback -= DrawLoadedScenesListElement;
//        loadedScenesList.onAddCallback -= AddGameScene;
//        loadedScenesList.onRemoveCallback -= RemoveGameScene;
//    }

//    private void DrawPersistentListHeader(Rect rect) {

//        GUI.Label(rect, "Persistent Scenes");
//    }

//    private void DrawPersistentScenesListElement(Rect rect, int i, bool active, bool focused) {


//        EditorGUI.BeginChangeCheck();

//        selectedPersistentIndex = i;
//        ScriptableScene var = targetSceneController.persistentScenes[i];
//        EditorTools.createPopUpMenuWithObjectsNames(ref var, ref selectedPersistentIndex, "", new Rect( rect.x, rect.y + rect.height/8, rect.width * 3/5, rect.height));
//        targetSceneController.persistentScenes[i] = var;

//        Scene sceneAux = EditorSceneManager.GetSceneByName(targetSceneController.persistentScenes[i].name);

//        // Change the color of the button depending on the scene loading state
//        var oldColor = GUI.backgroundColor;

//        // Loaded --> green
//        if (sceneAux.isLoaded) {
//            GUI.backgroundColor = Color.green;
//        }
//        // Not loaded --> red
//        else {
//            GUI.backgroundColor = Color.red;
//        }


//        if (GUI.Button(new Rect(rect.width * 3 / 5 + 40, rect.y + rect.height / 12, rect.width * 2/5 - 5, rect.height - 5), "Load / Unload")){
//            if (sceneAux.isLoaded) {

//                if (sceneAux.isDirty)
//                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

//                EditorSceneManager.CloseScene(sceneAux, true);
//            }
//            else
//                EditorSceneManager.OpenScene(targetSceneController.persistentScenes[i].scenePath, OpenSceneMode.Additive);

//        }

//        GUI.backgroundColor = oldColor;

//        if (EditorGUI.EndChangeCheck()) {
//            EditorUtility.SetDirty(target);
//        }

//    }

//    private void AddPersistentScene(ReorderableList list) {
//        targetSceneController.persistentScenes.Add(CreateInstance<ScriptableScene>());

//        EditorUtility.SetDirty(target);
//    }

//    private void RemovePersistentScene(ReorderableList list) {
//        targetSceneController.persistentScenes.RemoveAt(list.index);

//        EditorUtility.SetDirty(target);
//    }

//    private void DrawLoadedScenesListHeader(Rect rect) {

//        GUI.Label(rect, "Loaded Game Scenes");
//    }

//    private void DrawLoadedScenesListElement(Rect rect, int i, bool active, bool focused) {


//        EditorGUI.BeginChangeCheck();

//        selectedloadedSceneIndex = i;
//        ScriptableScene var = targetSceneController.gameScenes[i];
//        EditorTools.createPopUpMenuWithObjectsNames(ref var, ref selectedloadedSceneIndex, "", new Rect(rect.x, rect.y + rect.height / 8, rect.width * 3 / 5, rect.height));
//        targetSceneController.gameScenes[i] = var;

//        Scene sceneAux = EditorSceneManager.GetSceneByName(targetSceneController.gameScenes[i].name);

//        // Change the color of the button depending on the scene loading state
//        var oldColor = GUI.backgroundColor;

//        // Loaded --> green
//        if (sceneAux.isLoaded) {
//            GUI.backgroundColor = Color.green;
//        }
//        // Not loaded --> red
//        else {
//            GUI.backgroundColor = Color.red;
//        }


//        if (GUI.Button(new Rect(rect.width * 3 / 5 + 40, rect.y + rect.height / 12, rect.width * 2 / 5 - 5, rect.height - 5), "Load / Unload")) {
//            if (sceneAux.isLoaded) {

//                if (sceneAux.isDirty)
//                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

//                EditorSceneManager.CloseScene(sceneAux, true);
//            }
//            else
//                EditorSceneManager.OpenScene(targetSceneController.gameScenes[i].scenePath, OpenSceneMode.Additive);

//        }

//        GUI.backgroundColor = oldColor;

//        if (EditorGUI.EndChangeCheck()) {
//            EditorUtility.SetDirty(target);
//        }

//    }

//    private void AddGameScene(ReorderableList list) {
//        targetSceneController.gameScenes.Add(CreateInstance<ScriptableScene>());

//        EditorUtility.SetDirty(target);
//    }

//    private void RemoveGameScene(ReorderableList list) {
//        targetSceneController.gameScenes.RemoveAt(list.index);

//        EditorUtility.SetDirty(target);
//    }

//    public override void OnInspectorGUI() {

//        serializedObject.Update();

//        targetSceneController.playerSaveData = EditorGUILayout.ObjectField("Player Save Data", targetSceneController.playerSaveData, typeof(SaveData), false) as SaveData;

//        targetSceneController.sceneScreenFader = EditorGUILayout.ObjectField("Canvas Fader", targetSceneController.sceneScreenFader, typeof(CanvasFader), true) as CanvasFader;

//        drawSceneEditor(ref targetSceneController.startingScene, ref targetSceneController.initialStartingPositionName);

//        EditorTools.createHorizontalSeparator();

//        // Actually draw the list in the inspector
//        persistentScenesList.DoLayoutList();

//        loadedScenesList.DoLayoutList();

//        /* Scene Editor Loader Helper */
//        GUI.enabled = !Application.isPlaying;

//        //if (GUILayout.Button("Load Selected Scene")) {
//        //    if (EditorSceneManager.loadedSceneCount > 1) {
//        //        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
//        //        EditorSceneManager.CloseScene(EditorSceneManager.GetSceneAt(1), true);
//        //    }

//        //    // Get the path of the scene through the scriptable scene
//        //    ScriptableScene scriptableScene = AllGameScriptableScenes.Instance.scriptableScenes[selectedSceneIndex];
//        //    EditorSceneManager.OpenScene(scriptableScene.scenePath, OpenSceneMode.Additive);
//        //}

//        EditorGUILayout.Separator();

//        serializedObject.ApplyModifiedProperties();

//    }


//    public static void drawSceneEditor(ref ScriptableScene scene, ref string startingPointInLoadedScene) {

//        // No instance found of AllGameScriptableScenes
//        if (!AllGameScriptableScenes.Instance) {
//            string textMessage = "No AllGameScriptableScenes Instance found. " +
//                 " Create it in Assets > Create > AKAGF > AllGameScriptableScenes.";

//            EditorTools.drawMessage(textMessage, MessageType.Error);
//            return;
//        }

//        // No ScriptableScenes present in the project
//        if (AllGameScriptableScenes.Instance.scriptableScenes.Length == 0) {
//            string textMessage = "No ScriptableScenes found in AllGameScriptableScenes Instance. " +
//                "You must create scriptable Scenes in order to change between them through a Scene Reaction.";

//            EditorTools.drawMessage(textMessage, MessageType.Warning);
//            return;
//        }


//        /* Scenes Popup Menu */
//        // Add all the scenes Names present in AllGameScenes as popup options
//        int listSize = AllGameScriptableScenes.Instance.scriptableScenes.Length;
//        List<string> sceneOptions = new List<string>(AllGameScriptableScenes.Instance.scriptableScenes.Length);
//        for (int i = 0; i < listSize; i++) {
//            // Ensure the ScriptableScene has a valid Scene Attached to it 
//            // by not adding to the list the scriptables Scenes present 
//            // in AllGameScriptableScenes with the default creation name
//            // defined in AKAGF_PATHS struct
//            if(!AllGameScriptableScenes.Instance.scriptableScenes[i].name.Equals(AKAGF_PATHS.NEW_SCRIPTABLE_SCENE_BASE_NAME))
//                sceneOptions.Add(AllGameScriptableScenes.Instance.scriptableScenes[i].name);
//        }

//        // All scriptableScenes found don't have a Unity Scene attached to them
//        if (sceneOptions.Count == 0) {
//            string textMessage = "All scriptableScenes found don't have a Unity Scene attached to them" +
//                 " Check it in Project > Resources > AKAGF > SINGLETONS > AllGameScriptableScenes.";

//            EditorTools.drawMessage(textMessage, MessageType.Error);
//            return;
//        }

//        selectedSceneIndex = 0;

//        // Check if there is already a SceneName for this Scene Reaction
//        if (scene != null  && !scene.Equals("")) {
//            for (int i = 0; i < sceneOptions.Count; i++) {
//                if (sceneOptions[i].Equals(scene)) {
//                    selectedSceneIndex = i;
//                    i = sceneOptions.Count;
//                }
//            }
//        }

//        GUIContent sName = new GUIContent("Starting Scene", "The name of the non persistent scene that will be loaded in first place.");
//        selectedSceneIndex = EditorGUILayout.Popup(sName, selectedSceneIndex, sceneOptions.ToArray());

//        //scene = sceneOptions[selectedSceneIndex];

//        /* Starting Position Popup Menu */
//        scene = AllGameScriptableScenes.Instance.scriptableScenes[selectedSceneIndex];
//        int posListSize = scene.sceneStartingPositionsNames.Length;

//        // No starting Positions Names in the ScriptableScene
//        if (posListSize == 0) {

//            string textMessage = "No Starting Position Names found in " + scene.name + 
//                " Scriptable Scene. You may had forgotten to add Starting Positions Names to the ScriptableScene " +
//                "or may your game doesn't implement starting positions in scenes.";

//            EditorTools.drawMessage(textMessage, MessageType.Info);

//            return;
//        }

//        // Add all the starting Position names present in the ScriptableScenes to the option list
//        List<string> startingPointsOptions = new List<string>(posListSize);
//        for (int i = 0; i < posListSize; i++) {
//            if(!scene.sceneStartingPositionsNames[i].Equals(""))
//                startingPointsOptions.Add(scene.sceneStartingPositionsNames[i]);
//        }

//        if (startingPointsOptions.Count != posListSize) {
//            string textMessage = "There is at least one StartingPositionName with empty value in " +
//                 scene.name + " Scriptable Scene. You probably forgotten to add Starting Positions Names to the ScriptableScene " +
//                "in AllGameScriptableScenes Editor. Check it before you continue.";

//            EditorTools.drawMessage(textMessage, MessageType.Error);

//            return;
//        }

//        int selectedStartingPositionIndex = 0;

//        // Check if there is already a StartingPointName
//        if (startingPointInLoadedScene != null 
//            && !startingPointInLoadedScene.Equals("")) {

//            for (int i = 0; i < startingPointsOptions.Count; i++) {
//                if (startingPointsOptions[i].Equals(startingPointInLoadedScene)) {
//                    selectedStartingPositionIndex = i;
//                    break;
//                }
//            }
//        }

//        GUIContent pName = new GUIContent("Starting Point", "The starting position name on the scene to load where the player will be spawned");
//        selectedStartingPositionIndex = EditorGUILayout.Popup(pName , selectedStartingPositionIndex, startingPointsOptions.ToArray());
//        GUILayout.Space(5);

//        startingPointInLoadedScene = startingPointsOptions[selectedStartingPositionIndex];
//    }
//}
#endregion