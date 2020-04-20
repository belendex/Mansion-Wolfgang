using System;
using AKAGF.GameArchitecture.ScriptableObjects.Abstracts;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Conditions;
using UnityEngine.Events;

/*
Grab a normal six-sided die and start The Lazy Quest-o-matic. Make two rolls, and apply the results as below: 

First Roll: 
If Roll is: The action of the quest is to: 
1 Liberate/Recover/Intercept 
2 Destroy/Kill 
3 Guard/Defend 
4 Transport/Escort/Journey To
5 Create/Build/Summon 
6 Gather Information About

Second Roll: 
If Roll is: The object of the quest is: 
1 Item 
2 NPC 
3 Message/Data 
4 Secret or Dangerous Location
5 Magical Equipment/Technology 
6 Monster
*/


namespace AKAGF.GameArchitecture.ScriptableObjects.QuestsSystem {

    public enum QUEST_STATE {INACTIVE, ACTIVE, FAILED, COMPLETE }

    [Serializable]
    public class QuestEvent : UnityEvent<Quest> { }

    [Serializable]
    public class GoalEvent : UnityEvent<Goal> { }


    [Serializable]
    public class Goal {

        public string goalName;
        public string goalDescription;
        public bool isOptional;
        public Condition goalCondition ;

        public QUEST_STATE goalState;

        public GoalEvent onGoalComplete;
        public GoalEvent onGoalFail;

#if UNITY_EDITOR
        [System.NonSerialized]
        public bool isExpanded;
#endif

        public void failGoal(QuestsList list) {

            if (goalState != QUEST_STATE.ACTIVE)
                return;

            goalState = QUEST_STATE.FAILED;
            onGoalFail.Invoke(this);
            list.invokeGoalGlobalEvent(this, goalState);
        }


        public void completeGoal(QuestsList list) {

            if (goalState != QUEST_STATE.ACTIVE)
                return;

            goalState = QUEST_STATE.COMPLETE;
            if(goalCondition)
                goalCondition.isSatisfied = true;
            onGoalComplete.Invoke(this);
            list.invokeGoalGlobalEvent(this, goalState);
        }
    }


    public class Quest : ResettableScriptableObject {
        public string questName;
        public string questShortDescription;
        public string questLongDescription;
        public string[] questHints = new string[0];
        public bool autoEnableQuest;
        public Condition questCondition;

        
        public QuestEvent OnQuestActive;
        public QuestEvent OnQuestFailed;
        public QuestEvent OnQuestComplete;

        public Goal[] questGoals = new Goal[0];

        public Quest[] previousQuests = new Quest[0];

        public QuestsList parentQuestList;

        public QUEST_STATE state;

#if UNITY_EDITOR
        [System.NonSerialized]
        public bool isExpanded;
#endif

        // Reset all variables and goals at game stop
        public override void Reset() {
            if(questCondition)
                questCondition.isSatisfied = false;

            state = QUEST_STATE.INACTIVE;

            for (int i = 0; i < questGoals.Length; i++) {
                if(questGoals[i].goalCondition)
                    questGoals[i].goalCondition.isSatisfied = false;

                questGoals[i].goalState = QUEST_STATE.INACTIVE;
            }
        }


        // This method is called from the questsList which this quest belongs.
        public void checkQuestProgress() {

            // Quest is already failed or completed, so do nothing
            if (state == QUEST_STATE.FAILED || state == QUEST_STATE.COMPLETE)
                return;

            // Checks if the quest needs to be activated 
            // when the previous quests have been completed
            if (state == QUEST_STATE.INACTIVE) {
                if (autoEnableQuest && checkPreviousQuests())
                    activeQuest();

                else return;
            }

            // If execution get here, quest is Active, so it necessary
            // to check all the goals completition
            checkGoalsProgress();
        }


        private void checkGoalsProgress() {

            // Go through all the goals checking their states
            for (int i = 0; i < questGoals.Length; i++) {
                // If goal is failed and is not an optional one, it means that the quest is failed.
                if (questGoals[i].goalState == QUEST_STATE.FAILED && !questGoals[i].isOptional) {
                    failQuest();
                    return;
                }

                // If goal is active and is not optional, it means that the quest is still on going
                if(questGoals[i].goalState == QUEST_STATE.ACTIVE && !questGoals[i].isOptional)
                    return;
            }

            // If the executions gets here,
            // it means all mandatory goals are completed.
            completeQuest();
        }


        // Helper function to check if the quests
        // previous to this one are already completed
        private bool checkPreviousQuests() {

            for (int i = 0; i < previousQuests.Length; i++) {
                if (previousQuests[i].state != QUEST_STATE.COMPLETE) {
                    return false;
                }
            }

            return true;
        }


        // This is the entry point called from QuestReactions in order to activate a quest.
        public void activeQuest() {
            state = QUEST_STATE.ACTIVE;

            for (int i = 0; i < questGoals.Length; i++)
                questGoals[i].goalState = QUEST_STATE.ACTIVE;

            OnQuestActive.Invoke(this);
            parentQuestList.invokeQuestGlobalEvent(this, state);
        }


        private void failQuest() {
            state = QUEST_STATE.FAILED;
            OnQuestFailed.Invoke(this);
            parentQuestList.invokeQuestGlobalEvent(this, state);
        }


        private void completeQuest() {
            state = QUEST_STATE.COMPLETE;
            if(questCondition)
                questCondition.isSatisfied = true;
            OnQuestComplete.Invoke(this);
            parentQuestList.invokeQuestGlobalEvent(this, state);
        }
    }
}