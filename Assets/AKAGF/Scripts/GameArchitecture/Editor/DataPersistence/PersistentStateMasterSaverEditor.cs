using System;
using System.Collections.Generic;
using AKAeditor;
using AKAGF.GameArchitecture.MonoBehaviours.DataPersistence;
using AKAGF.GameArchitecture.ScriptableObjects.DataPersistence;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PersistentStateMasterSaver))]
public class PersistentStateMasterSaverEditor : Editor {

    private PersistentStateMasterSaver targetElement;   // The target element of precise type
    private bool[] isExpanded = new bool[1];            // array of bools to keep track of folded state of each element
    private int saveDataSelectedIndex;

    private void OnEnable() {
        targetElement = (PersistentStateMasterSaver)target;

        if (targetElement.saversData.Length != isExpanded.Length) {
            Array.Resize(ref isExpanded, targetElement.saversData.Length);
        }

    }


    public override void OnInspectorGUI() {

        // Update the state of the serializedObject to the current values of the target.
        serializedObject.Update() ;

        Undo.RecordObject(targetElement, "save data");


        EditorTools.createPopUpMenuWithObjectsNames(ref targetElement.saveData, ref saveDataSelectedIndex, "Data Save");

        //if (targetElement.saveData)
        //    saveDataSelectedIndex = savesDatasAux.IndexOf(targetElement.saveData);

        //string[] names = ScriptableObjectUtility.getAllInstancesNames<SaveData>();

        //saveDataSelectedIndex = EditorGUILayout.Popup("Save Data", saveDataSelectedIndex, names);

        //if (savesDatasAux.Count == 0) {
        //    EditorGUILayout.HelpBox("No ScriptableObject Save Data Reference Found!", MessageType.Warning);
        //}
        //else
        //    targetElement.saveData = savesDatasAux[saveDataSelectedIndex];


        GUILayout.Space(5);

        // Button for adding another element just by resizing the arrays
        if (GUILayout.Button("Add Saver")) {
            Array.Resize(ref targetElement.saversData, targetElement.saversData.Length + 1);
            Array.Resize(ref isExpanded, targetElement.saversData.Length);
            
           // EditorUtility.SetDirty(targetElement);
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        GUILayout.Space(5);

        // Iterate throught all the saverData elements
        for (int i = 0; i < targetElement.saversData.Length; i++) {

            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
            //EditorGUILayout.LabelField(targetItem.name);

            // Handle foldout through isExpanded variable
            isExpanded[i] = EditorGUILayout.Foldout(isExpanded[i], new GUIContent(targetElement.saversData[i].uniquePrefixID), true, EditorStyles.foldout) ;

            // Create a remove button in each element of the list
            if (GUILayout.Button(" - ", GUILayout.ExpandWidth(false))) {
                var list = new List<StateSaverElement>(targetElement.saversData);
                list.RemoveAt(i);
                targetElement.saversData = list.ToArray();
       
                //Array.Resize(ref targetElement.saversData, targetElement.saversData.Length - 1);
                Array.Resize(ref isExpanded, targetElement.saversData.Length);
                //serializedObject.ApplyModifiedProperties();
                //serializedObject.Update();
                continue;
            }

            GUILayout.EndHorizontal();

            // Designer is inspecting the content of this element
            if (isExpanded[i]) {
                GUILayout.Space(5);

                /* Element Properties */
                targetElement.saversData[i].uniquePrefixID = EditorGUILayout.TextField("Unique Prefix ID", targetElement.saversData[i].uniquePrefixID);
                targetElement.saversData[i].persistentGameObject = EditorGUILayout.ObjectField("Persistent GameObject", targetElement.saversData[i].persistentGameObject, typeof(GameObject), true) as GameObject;
                if (!targetElement.saversData[i].persistentGameObject) {
                    EditorGUILayout.HelpBox("No Persistent Game Object Reference Found!", MessageType.Warning);
                }


                targetElement.saversData[i].saveGameObjectState = EditorGUILayout.Toggle("Save GameObject State", targetElement.saversData[i].saveGameObjectState, GUILayout.ExpandWidth(false));
                targetElement.saversData[i].savePositionState = EditorGUILayout.Toggle("Save Position State", targetElement.saversData[i].savePositionState, GUILayout.ExpandWidth(false));
                targetElement.saversData[i].saveRotationState = EditorGUILayout.Toggle("Save Rotation State", targetElement.saversData[i].saveRotationState, GUILayout.ExpandWidth(false));

                targetElement.saversData[i].saveBehaviourEnableState = EditorGUILayout.Toggle("Save Behaviour Enable State", targetElement.saversData[i].saveBehaviourEnableState, GUILayout.ExpandWidth(false));

                // Creates the saversData only in case that having a valid reference to a GameObject
                if (targetElement.saversData[i].persistentGameObject) {
                    initSavers(ref targetElement.saversData[i]);

                    if (targetElement.saversData[i].saveBehaviourEnableState) {
                        // Get all the behaviours component attached to persisten GameObject
                        Behaviour[] behaviours = targetElement.saversData[i].persistentGameObject.GetComponents<Behaviour>();

                        // Create an array of strings to store the names of the found behaviours
                        string[] behavioursNames = new string[behaviours.Length];

                        for (int j = 0; j < behaviours.Length; j++) {
                            behavioursNames[j] = behaviours[j].GetType().ToString();
                        }

                        // Fetch all behaviours that must to be saved from a maskField of persistent GameObject contained behaviours
                        targetElement.saversData[i].behaviorStatesflags = EditorGUILayout.MaskField("Behaviours to Save", targetElement.saversData[i].behaviorStatesflags, behavioursNames);

                        List<Behaviour> selectedOptions = new List<Behaviour>();

                        for (int j = 0; j < behaviours.Length; j++) {
                            if ((targetElement.saversData[i].behaviorStatesflags & (1 << j)) == (1 << j)) selectedOptions.Add(behaviours[j]);
                        }

                        // Store the behaviours in target behaviours list
                        targetElement.saversData[i].persistentGameObjectSelectedBehaviours = selectedOptions;

                    }
                    else {
                        // If save behaviours checkbox is unmarked, set the array to null
                        targetElement.saversData[i].persistentGameObjectSelectedBehaviours.Clear();
                        targetElement.saversData[i].behaviorStatesflags = 0;
                    }

                }
       
            }

            EditorGUILayout.EndVertical();

           

            // Push data back from the serializedObject to the target.
            serializedObject.ApplyModifiedProperties();
        }

        GUILayout.Space(5);
    }

    // Initializes the four types of savers (GameObject, Behaviour, Position and Rotation) 
    // only if its corresponding checkbox is marked by the designer.
    public void initSavers(ref StateSaverElement stateSaver) {
        if (stateSaver.saveBehaviourEnableState) {

            stateSaver.behaviourEnableStateSavers = new BehaviourEnableStateSaver[stateSaver.persistentGameObjectSelectedBehaviours.Count];

            for (int i = 0; i < stateSaver.behaviourEnableStateSavers.Length; i++){
                stateSaver.behaviourEnableStateSavers[i] = new BehaviourEnableStateSaver();
                stateSaver.behaviourEnableStateSavers[i].behaviourToSave = stateSaver.persistentGameObjectSelectedBehaviours[i];
                stateSaver.behaviourEnableStateSavers[i].SetKey(stateSaver.uniquePrefixID);
            }
        }

        if (stateSaver.saveGameObjectState) {
            stateSaver.gameObjectActivitySaver = new GameObjectActivitySaver();
            stateSaver.gameObjectActivitySaver.gameObjectToSave = stateSaver.persistentGameObject;
            stateSaver.gameObjectActivitySaver.SetKey(stateSaver.uniquePrefixID);
        }

        if (stateSaver.savePositionState) {
            stateSaver.positionSaver = new PositionSaver();
            stateSaver.positionSaver.transformToSave = stateSaver.persistentGameObject.transform;
            stateSaver.positionSaver.SetKey(stateSaver.uniquePrefixID);
        }

        if (stateSaver.savePositionState) {
            stateSaver.rotationSaver = new RotationSaver();
            stateSaver.rotationSaver.transformToSave = stateSaver.persistentGameObject.transform;
            stateSaver.rotationSaver.SetKey(stateSaver.uniquePrefixID);
        }
    }
}
