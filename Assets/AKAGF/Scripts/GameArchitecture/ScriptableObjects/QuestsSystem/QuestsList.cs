using System;
using AKAGF.GameArchitecture.ScriptableObjects.Abstracts;


namespace AKAGF.GameArchitecture.ScriptableObjects.QuestsSystem {

    public class QuestsList : ResettableScriptableObject {

        public event Action<Quest> OnQuestActive;
        public event Action<Quest> OnQuestFailed;
        public event Action<Quest> OnQuestComplete;

        public event Action<Goal> onGoalComplete;
        public event Action<Goal> onGoalFail;

        public string questsListName;
        public string questsListDescription;
        public Quest[] quests = new Quest[0];


        public override void Reset() {
            for (int i = 0; i < quests.Length; i++) {
                quests[i].Reset();
            }
        }

        public void checkQuestsProgress() {
            for (int i = 0; i < quests.Length; i++) {
                quests[i].checkQuestProgress();
            }
        }

        public void invokeQuestGlobalEvent(Quest quest, QUEST_STATE state) {

            switch (state) {
                case QUEST_STATE.ACTIVE:
                    if (OnQuestActive != null) OnQuestActive(quest);
                    break;

                case QUEST_STATE.FAILED:
                    if (OnQuestFailed != null) OnQuestFailed(quest);
                    break;

                case QUEST_STATE.COMPLETE:
                    if (OnQuestComplete != null) OnQuestComplete(quest);
                    break;
            }
        }


        public void invokeGoalGlobalEvent(Goal goal, QUEST_STATE state) {

            switch (state) {

                case QUEST_STATE.FAILED:
                    if (onGoalFail != null) onGoalFail(goal);
                    break;

                case QUEST_STATE.COMPLETE:
                    if (onGoalComplete != null) onGoalComplete(goal);
                    break;
            }
        }
    }

}

