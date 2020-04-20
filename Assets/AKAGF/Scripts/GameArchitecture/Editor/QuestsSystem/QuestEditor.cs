using UnityEngine;
using UnityEditor;
using AKAGF.GameArchitecture.ScriptableObjects.QuestsSystem;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Conditions;
using AKAeditor;
using System;

[CustomEditor(typeof(Quest))]
public class QuestEditor : Editor {

    public enum QUEST_TABS { Goals, Events, PreviousQuests }
    private static QUEST_TABS currentQuestTabOption;

    private Quest targetQuest;

    //private static bool[] isExpandedGoal = new bool[0];
    private const float buttonWidth = 30f;          // Width in pixels of the add and remove buttons



    private void OnEnable() {
        targetQuest = (Quest)target;

        //if (targetQuest.questGoals.Length != isExpandedGoal.Length)
        //    Array.Resize(ref isExpandedGoal, targetQuest.questGoals.Length);
    }

    public override void OnInspectorGUI() {

        // Update the state of the serializedObject to the current values of the target.
        serializedObject.Update();

        EditorGUILayout.BeginVertical(GUI.skin.box);

        GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
        //EditorGUILayout.LabelField(targetItem.name);

        // Handle foldut through isExpanded variable
        targetQuest.isExpanded = EditorGUILayout.Foldout(targetQuest.isExpanded, new GUIContent(targetQuest.name), true, EditorStyles.foldout);

        if (EditorTools.createListButton(" Delete Quest ", true, GUILayout.ExpandWidth(false))) {
            // Record all operations so they can be undone.
            Undo.RecordObject(targetQuest, "Delete Quest");

            // Remove the specified quest from the parent Quests array.
            ArrayUtility.Remove(ref targetQuest.parentQuestList.quests, targetQuest);

            // Destroy the quest, including it's asset and save the assets to recognise the change.
            DestroyImmediate(targetQuest, true);
            AssetDatabase.SaveAssets();

            return;
        }

        GUILayout.EndHorizontal();

        if (targetQuest.isExpanded) {
            GUILayout.Space(5);

            //Project Name
            GUILayout.BeginHorizontal();
            targetQuest.name = EditorGUILayout.TextField("Project Quest Name", targetQuest.name);
            if (GUILayout.Button("Rename Quest", GUILayout.ExpandWidth(false))) {
                AssetDatabase.SaveAssets();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);


            // Quest game name
            targetQuest.questName = EditorGUILayout.TextField("Game Quest Name", targetQuest.questName as string);
            // Quest game short description
            targetQuest.questShortDescription = EditorGUILayout.TextField("Game Quest Short Description", targetQuest.questShortDescription as string);
            // Quest game long description
            targetQuest.questLongDescription = EditorGUILayout.TextField("Game Quest Short Description", targetQuest.questLongDescription as string);
            // Auto enable quest toggle
            targetQuest.autoEnableQuest = EditorGUILayout.Toggle("auto Enable Quest", targetQuest.autoEnableQuest, GUILayout.ExpandWidth(false));
            // Quest condition
            targetQuest.questCondition = EditorGUILayout.ObjectField("Quest Condition", targetQuest.questCondition, typeof(Condition), false) as Condition;

            // Current ques State
            GUI.enabled = false;
            targetQuest.state = (QUEST_STATE)EditorGUILayout.EnumPopup("Quest State", targetQuest.state);
            GUI.enabled = true;

            // Hints array
            SerializedProperty hintsProp = serializedObject.FindProperty("questHints");
            EditorTools.createArrayPropertyButtons(hintsProp, " Add Quest Hint ", GUILayout.ExpandWidth(true), true, true);

            for (int i = 0; i < targetQuest.questHints.Length; i++) {
                GUILayout.BeginHorizontal();
                targetQuest.questHints[i] = EditorGUILayout.TextField("Hint " + (i+1), targetQuest.questHints[i] as string);
                if (EditorTools.createListButton(" - ", true, GUILayout.ExpandWidth(false))) {
                    ArrayUtility.Remove(ref targetQuest.questHints, targetQuest.questHints[i]);
                    AssetDatabase.SaveAssets();
                    return;
                }
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.Separator();
            EditorTools.createHorizontalSeparator();

            string[] enumStringsVars = Enum.GetNames(typeof(QUEST_TABS));
            currentQuestTabOption = (QUEST_TABS)GUILayout.Toolbar((int)currentQuestTabOption, enumStringsVars, GUILayout.MinWidth(100));

            switch (currentQuestTabOption) {
                case QUEST_TABS.Goals:

                    // Quest Goals
                    EditorTools.createTitleBox("Quest Goals", true);

                    EditorGUILayout.Space();
                    SerializedProperty QuestGoalsProp = serializedObject.FindProperty("questGoals");
                    AKAeditor.EditorTools.createArrayPropertyButtons(QuestGoalsProp, " Add Goal ", GUILayout.ExpandWidth(true), true, true);

                    for (int i = 0; i < targetQuest.questGoals.Length; i++) {
                        createGoalElement(i);
                    }

                    break;
                case QUEST_TABS.Events:
                    // Events
                    EditorTools.createTitleBox("Quest Events", true);

                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnQuestActive"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnQuestFailed"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnQuestComplete"), true);
                    break;
                case QUEST_TABS.PreviousQuests:

                    // Quest previous Quests
                    EditorTools.createTitleBox("Previous Quests", true);
                    EditorGUILayout.Space();

                    SerializedProperty previousQuestProp = serializedObject.FindProperty("previousQuests");
                    EditorTools.createArrayPropertyButtons(previousQuestProp, " Add Quest ", GUILayout.ExpandWidth(true), true, true);


                    for (int i = 0; i < targetQuest.previousQuests.Length; i++) {

                        GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
                        targetQuest.previousQuests[i] = EditorGUILayout.ObjectField("Previous Quest " + i, targetQuest.previousQuests[i], typeof(Quest), false) as Quest;
                        if (EditorTools.createListButton(" - ", true, GUILayout.ExpandWidth(false))) {
                            // Record all operations so they can be undone.
                            Undo.RecordObject(targetQuest, "Remove Previous Quest");

                            // Remove the specified item from the array.
                            ArrayUtility.Remove(ref targetQuest.previousQuests, targetQuest.previousQuests[i]);

                            // Destroy the item, including it's asset and save the assets to recognise the change.
                            AssetDatabase.SaveAssets();

                            return;
                        }

                        GUILayout.EndHorizontal();
                    }
                    break;
            }
        }

        EditorGUILayout.EndVertical();

        // Push data back from the serializedObject to the target.
        serializedObject.ApplyModifiedProperties();
    }


    private void createGoalElement(int goalIndex) {

        EditorGUILayout.BeginVertical(GUI.skin.box);

        GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
        //EditorGUILayout.LabelField(targetItem.name);

        Goal currGoal = targetQuest.questGoals[goalIndex];

        //if (targetQuest.questGoals.Length != isExpandedGoal.Length)
        //    Array.Resize(ref isExpandedGoal, targetQuest.questGoals.Length);

        if (currGoal.goalName.Equals(""))
            currGoal.goalName = "Quest Goal " + (goalIndex + 1);

        // Handle foldut through isExpanded variable
        currGoal.isExpanded = EditorGUILayout.Foldout(currGoal.isExpanded, new GUIContent(currGoal.goalName), true, EditorStyles.foldout);

        if (EditorTools.createListButton(" - ", true, GUILayout.ExpandWidth(false))) {
            // Record all operations so they can be undone.
            Undo.RecordObject(targetQuest, "Delete Goal");

            // Remove the specified item from the array.
            ArrayUtility.Remove(ref targetQuest.questGoals, currGoal);

            // Destroy the item, including it's asset and save the assets to recognise the change.
            AssetDatabase.SaveAssets();

            return;
        }

        GUILayout.EndHorizontal();


        if (currGoal.isExpanded) {
            // Goal name
            currGoal.goalName = EditorGUILayout.TextField("Goal Name", currGoal.goalName as string);

            // Goal description
            currGoal.goalDescription = EditorGUILayout.TextField("Goal Description", currGoal.goalDescription as string);

            // optional goal toggle
            currGoal.isOptional = EditorGUILayout.Toggle("Optional Goal?", currGoal.isOptional, GUILayout.ExpandWidth(false));

            // Goal Condition
            currGoal.goalCondition = EditorGUILayout.ObjectField("Goal Condition", currGoal.goalCondition, typeof(Condition), false) as Condition;

            // Current goal State
            GUI.enabled = false;
            currGoal.goalState = (QUEST_STATE) EditorGUILayout.EnumPopup("Goal State", currGoal.goalState);
            GUI.enabled = true;

            // Goal events
            SerializedProperty goalsArrayProp = serializedObject.FindProperty("questGoals");
            EditorGUILayout.PropertyField(goalsArrayProp.GetArrayElementAtIndex(goalIndex).FindPropertyRelative("onGoalComplete"), true);
            EditorGUILayout.PropertyField(goalsArrayProp.GetArrayElementAtIndex(goalIndex).FindPropertyRelative("onGoalFail"), true);

        }

        EditorGUILayout.EndVertical();
    }

}
