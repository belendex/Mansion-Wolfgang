using AKAeditor;
using System.Collections.Generic;
using AKAGF.GameArchitecture.MonoBehaviours.SceneControl;
using AKAGF.GameArchitecture.ScriptableObjects.SceneControl;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(StartingPosition))]
public class StartingPositionEditor : Editor {

    private StartingPosition targetStartingPosition;
    private int selectStartingPosName = 0;

    private void OnEnable() {
        targetStartingPosition = target as StartingPosition;
    }


    public override void OnInspectorGUI() {

        // Get the corresponding ScriptableScene that contains the currently open Scene in Unity Editor
        string currentUnityEditorOpenSceneName = EditorSceneManager.GetActiveScene().name;
        ScriptableScene currentScriptableScene = ScriptableObjectUtility.GetScriptableObjectByName<ScriptableScene>(currentUnityEditorOpenSceneName, AllGameScriptableScenes.Instance.scriptableScenes);

        // If there is a Unity Scene that doesn't have a ScriptableScene reference
        // Inform to the developer
        if (!currentScriptableScene) {
            string textMessagee = "No ScriptableScene found for Unity Scene: "
                + currentUnityEditorOpenSceneName + ". Create one for this Unity Scene!";

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.HelpBox(textMessagee, MessageType.Error);
            EditorGUILayout.EndVertical();

            return;
        }


        int posListSize = currentScriptableScene.sceneStartingPositionsNames.Length;

        // No starting Positions Names in the ScriptableScene
        if (posListSize == 0) {

            string textMessage = "No Starting Position Names found in " + currentUnityEditorOpenSceneName + 
                " Scriptable Scene. You may had forgotten to add Starting Positions Names to the ScriptableScene " +
                "or may your game doesn't implement starting positions in scenes.";

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.HelpBox(textMessage, MessageType.Warning);
            EditorGUILayout.EndVertical();

            return;
        }

        // Add all the startnig Position names present in the ScriptableScenes to the option list
        List<string> options = new List<string>(posListSize);
        for (int i = 0; i < posListSize; i++) {
            options.Add(currentScriptableScene.sceneStartingPositionsNames[i]);
        }

        // Check if there is already a StartingPointName
        if (targetStartingPosition.startingPointName != null 
            && !targetStartingPosition.startingPointName.Equals("")) {

            for (int i = 0; i < options.Count; i++) {
                if (options[i].Equals(targetStartingPosition.startingPointName)) {
                    selectStartingPosName = i;
                    break;
                }
            }
        }

        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Space(5);
        selectStartingPosName = EditorGUILayout.Popup("Starting Position Name" ,selectStartingPosName, options.ToArray());
        GUILayout.Space(5);
        EditorGUILayout.EndVertical();

        targetStartingPosition.startingPointName = options[selectStartingPosName];

    }

}
