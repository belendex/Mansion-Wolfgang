using AKAGF.GameArchitecture.MonoBehaviours.Interaction;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Abstracts;
using AKAGF.GameArchitecture.ScriptableObjects.QuestsSystem;
using UnityEngine;


namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.DelayedReactions
{
    public class QuestReaction : DelayedReaction {

        public enum QUEST_ACTION { ACTIVATE_QUEST, COMPLETE_GOAL, FAIL_GOAL}
        public QUEST_ACTION actionToPerform;
        public QuestsList questsList;
        public Quest quest;
        public int goalIndex;
       

        protected override void ImmediateReaction(ref Interactable publisher) {

            switch (actionToPerform) {
                case QUEST_ACTION.ACTIVATE_QUEST:
                    quest.activeQuest();
                 
                    break;
                case QUEST_ACTION.COMPLETE_GOAL:
                    quest.questGoals[goalIndex].completeGoal(questsList);
                    
                    break;
                case QUEST_ACTION.FAIL_GOAL:
                    quest.questGoals[goalIndex].failGoal(questsList);

                    break;
                default:
                    break;
            }
        }
    }
}