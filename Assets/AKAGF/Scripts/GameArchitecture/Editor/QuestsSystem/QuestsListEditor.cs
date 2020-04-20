using AKAeditor;
using AKAGF.GameArchitecture.Literals;
using AKAGF.GameArchitecture.ScriptableObjects.QuestsSystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuestsList))]
public class QuestsListEditor : Editor {

    private QuestsList targetQuestsList;
    private QuestEditor[] questsEditors;

    private string newQuestName = "The name for the new quest";

    public const string BASE_PATH = AKAGF_PATHS.QUESTS_LIST_PATH;


    // Call this function when the menu item is selected.
    [MenuItem(AKAGF_PATHS.AKAGF_MENU_FULL_PATH +
                    AKAGF_PATHS.QUEST_MENU_PATH +
                        AKAGF_PATHS.QUESTS_LIST_MENU_NAME)]
    public static QuestsList CreateQuestsList() {
        QuestsList asset = ScriptableObject.CreateInstance<QuestsList>();

        if (!Directory.Exists(BASE_PATH)) {
            Directory.CreateDirectory(BASE_PATH);
        }

        if (AssetDatabase.LoadAssetAtPath<QuestsList>(BASE_PATH + AKAGF_PATHS.NEW_QUESTS_LIST_BASE_NAME) == null) {
            AssetDatabase.CreateAsset(asset, BASE_PATH + AKAGF_PATHS.NEW_QUESTS_LIST_BASE_NAME);
            AssetDatabase.SaveAssets();
            return asset;
        }

        Debug.Log("There is already a " + AKAGF_PATHS.NEW_QUESTS_LIST_BASE_NAME + ", change the name first, then create another one.");
        return null;
    }

    private void OnEnable () {

        //Target the selected QuestsList in Unity's project view
        targetQuestsList = (QuestsList)target;

        if (targetQuestsList && targetQuestsList.quests == null)
            targetQuestsList.quests = new Quest[0];

        if (questsEditors == null)
            CreateEditors();

    }


    private void CreateEditors() {
        // Create a new array for the editors which is the same length at the Items array.
        questsEditors = new QuestEditor[targetQuestsList.quests.Length];

        // Go through all the empty array...
        for (int i = 0; i < targetQuestsList.quests.Length; i++) {
            // ... and create an editor with an editor type to display correctly.
            questsEditors[i] = CreateEditor(targetQuestsList.quests[i]) as QuestEditor;
            //questsEditors[i].isExpandedQuest = false;
        }
    }

    public override void OnInspectorGUI() {
        // Update the state of the serializedObject to the current values of the target.
        serializedObject.Update();

        GUILayout.BeginHorizontal();
        //Renaming
        targetQuestsList.name = EditorGUILayout.TextField("Quests List Project Name", targetQuestsList.name as string);
        if (GUILayout.Button(" Rename ", GUILayout.ExpandWidth(false))) {
            string assetPath = AssetDatabase.GetAssetPath(targetQuestsList.GetInstanceID());
            AssetDatabase.RenameAsset(assetPath, targetQuestsList.name);
            AssetDatabase.SaveAssets();
        }
        GUILayout.EndHorizontal();

        targetQuestsList.questsListName = EditorGUILayout.TextField("Quests List Game Name", targetQuestsList.questsListName as string);

        GUILayout.Space(10);

        // Description property
        EditorGUILayout.LabelField("Quests List Description", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();

        targetQuestsList.questsListDescription =
            EditorGUILayout.TextArea(targetQuestsList.questsListDescription, EditorStyles.textArea, GUILayout.Height(EditorGUIUtility.singleLineHeight * 5));
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        newQuestName = EditorGUILayout.TextField("New Quest Name", newQuestName);
        // Change default Unity Editor array elements management
        if (EditorTools.createListButton(" Add Quest ", false, GUILayout.ExpandWidth(false))) {
            AddQuest(newQuestName);
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);


        if (targetQuestsList.quests.Length > 0) {
            EditorGUILayout.LabelField("Total Quests: " + targetQuestsList.quests.Length, EditorStyles.boldLabel);
        }
        else {
            GUILayout.Label("This Quests List is Empty.");
        }


        // If there are different number of editors to Items, create them afresh.
        if (questsEditors.Length != targetQuestsList.quests.Length) {
            // Destroy all the old editors.
            for (int i = 0; i < questsEditors.Length; i++) {
                DestroyImmediate(questsEditors[i]);
            }

            // Create new editors.
            CreateEditors();
        }

        // Display all the items.
        for (int i = 0; i < questsEditors.Length; i++) {
            questsEditors[i].OnInspectorGUI();
        }


        if (GUI.changed) {
            EditorUtility.SetDirty(targetQuestsList);
        }

        // Push data back from the serializedObject to the target.
        serializedObject.ApplyModifiedProperties();
    }


    private void AddQuest(string name = "") {
        Quest newQuest = ScriptableObject.CreateInstance<Quest>();
        newQuest.parentQuestList = this.targetQuestsList;

        if (name == null || name == "")
            newQuest.name = "New Quest " + targetQuestsList.quests.Length + 1;
        else
            newQuest.name = name;

        ScriptableObjectUtility.AddScriptableObject(targetQuestsList, ref newQuest, ref targetQuestsList.quests, "Created new Quest");

        
    }
}
