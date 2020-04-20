using System.Collections.Generic;
using AKAeditor;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.DelayedReactions;
using AKAGF.GameArchitecture.ScriptableObjects.QuestsSystem;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuestReaction))]
public class QuestReactionEditor : ReactionEditor {

    private const string tooltipText = "";

    private QuestReaction targetReaction;
    private static List<Quest> allQuestsList;
    private int questSelectedIndex;
    

    protected override void Init(){

        targetReaction = target as QuestReaction;
        allQuestsList = new List<Quest>(ScriptableObjectUtility.GetAllInstances<Quest>());
    }

    private string[] createOptionsArray() {

        List<string> questOptions = new List<string>(allQuestsList.Count) ;

        for (int i = 0; i < allQuestsList.Count; i++) {
            questOptions.Add(allQuestsList[i].parentQuestList.name + "/" + allQuestsList[i].name);
        }

        return questOptions.ToArray();

    }

    protected override void DrawReaction() {

        serializedObject.Update();

        targetReaction.delay = EditorGUILayout.FloatField("Delay", targetReaction.delay);

        // Set the action to achieve with the reaction
        targetReaction.actionToPerform = (QuestReaction.QUEST_ACTION) EditorGUILayout.EnumPopup("Action", targetReaction.actionToPerform);

        EditorTools.createPopUpMenuWithObjectsNames(ref targetReaction.quest, ref questSelectedIndex, "Quest", createOptionsArray());

        if(!targetReaction.quest)
            return;

        //if (targetReaction.quest)
        //    questSelectedIndex = allQuestsList.IndexOf(targetReaction.quest);

        //questSelectedIndex = EditorGUILayout.Popup("Quest", questSelectedIndex, createOptionsArray());

        //targetReaction.quest = allQuestsList[questSelectedIndex];

        targetReaction.questsList = targetReaction.quest.parentQuestList ;

        //targetReaction.questsList = EditorGUILayout.ObjectField("Quest List", targetReaction.questsList, typeof(QuestsList), false) as QuestsList;
        //targetReaction.quest = EditorGUILayout.ObjectField("Quest ", targetReaction.quest, typeof(Quest), false) as Quest;


        if (targetReaction.actionToPerform == QuestReaction.QUEST_ACTION.COMPLETE_GOAL ||
            targetReaction.actionToPerform == QuestReaction.QUEST_ACTION.FAIL_GOAL) {


            string[] goalsNames = new string[targetReaction.quest.questGoals.Length] ;

            for (int i = 0; i < targetReaction.quest.questGoals.Length; i++) {
                goalsNames[i] = targetReaction.quest.questGoals[i].goalName;
            }

            if (targetReaction.quest.questGoals.Length == 0) {
                string textMessage = "No Goals found in " + targetReaction.questsList.name + "/" + targetReaction.quest.name;
                EditorTools.drawMessage(textMessage, MessageType.Error);
            }
            else {
                targetReaction.goalIndex = EditorGUILayout.Popup("Goal", targetReaction.goalIndex, goalsNames);
            }            
        }


        serializedObject.ApplyModifiedProperties();
       
    }

    
    protected override GUIContent GetFoldoutLabel () {
        return new GUIContent("Quest Reaction", tooltipText);
    }
}
