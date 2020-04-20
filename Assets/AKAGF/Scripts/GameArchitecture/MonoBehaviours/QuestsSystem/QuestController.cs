using AKAGF.GameArchitecture.ScriptableObjects.QuestsSystem;
using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.QuestsSystem {

    public class QuestController : MonoBehaviour {

        public QuestsList[] questsList;

        [Header("Quest List Global Quest Events")]
        public QuestEvent OnQuestActive;
        public QuestEvent OnQuestFailed;
        public QuestEvent OnQuestComplete;

        [Header("Quest List Global Goal Events")]
        public GoalEvent onGoalComplete;
        public GoalEvent onGoalFail;


        private void OnEnable() {
            for (int i = 0; i < questsList.Length; i++) {
                questsList[i].OnQuestActive += onQuestActiveRaised;
                questsList[i].OnQuestFailed += onQuestFailedRaised;
                questsList[i].OnQuestComplete += onQuestCompleteRaised;
                questsList[i].onGoalFail += onGoalFailedRaised;
                questsList[i].onGoalComplete += onGoalCompletedRaised;
            }
        }

        private void OnDisable() {

            for (int i = 0; i < questsList.Length; i++) {
                questsList[i].OnQuestActive -= onQuestActiveRaised;
                questsList[i].OnQuestFailed -= onQuestFailedRaised;
                questsList[i].OnQuestComplete -= onQuestCompleteRaised;
                questsList[i].onGoalFail -= onGoalFailedRaised;
                questsList[i].onGoalComplete -= onGoalCompletedRaised;
            }
            
        }

        private void Update() {

            for (int i = 0; i < questsList.Length; i++)
                questsList[i].checkQuestsProgress();
        }

        private void onQuestActiveRaised(Quest quest) {
            OnQuestActive.Invoke(quest);
            //Debug.Log("onQuestActiveRaised: " + quest.name);
        }

        private void onQuestFailedRaised(Quest quest) {
            OnQuestFailed.Invoke(quest);
            //Debug.Log("onQuestFailedRaised: " + quest.name);
        }

        private void onQuestCompleteRaised(Quest quest) {
            OnQuestComplete.Invoke(quest);
            //Debug.Log("onQuestCompleteRaised: " + quest.name);
        }

        private void onGoalFailedRaised(Goal goal) {
            onGoalFail.Invoke(goal);
            //Debug.Log("onGoalFailedRaised: " + goal.goalName);
        }

        private void onGoalCompletedRaised(Goal goal) {
            onGoalComplete.Invoke(goal);
            //Debug.Log("onGoalCompletedRaised: " + goal.goalName);
        }
    }

}


