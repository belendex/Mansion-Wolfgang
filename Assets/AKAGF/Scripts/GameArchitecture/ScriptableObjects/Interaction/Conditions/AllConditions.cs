using System.Collections.Generic;
using AKAGF.GameArchitecture.ScriptableObjects.Abstracts;
using UnityEngine;

// This script works as a singleton asset.  That means that
// it is globally accessible through a static instance
// reference.  
namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Conditions
{
    [System.Serializable]
    public class AllConditions : ResettableScriptableSingleton<AllConditions> {

        public Condition[] conditions;                       // All the Conditions that exist in the game.

        // This function will be called at Start once per run of the game.
        public override void Reset (){
            // If there are no conditions, do nothing.
            if (conditions == null)
                return;

            // Set all of the conditions to not satisfied.
            for (int i = 0; i < conditions.Length; i++){
                conditions[i].isSatisfied = false;
            }
        }


        // This is called from ConditionCollections when they are being checked by an Interactable that has been clicked on.
        public static bool CheckCondition (Condition requiredCondition){
            // Cache the condition array.
            Condition[] allConditions = Instance.conditions;
            Condition globalCondition = null;
        
            // If there is at least one condition...
            if (allConditions != null && allConditions[0] != null){
                // ... go through all the conditions...
                for (int i = 0; i < allConditions.Length; i++){
                    // ... and if they match the given condition then this is the global version of the requiredConditiond.
                    if (allConditions[i].hash == requiredCondition.hash)
                        globalCondition = allConditions[i];
                }
            }

            // If by this point a globalCondition hasn't been found then return false.
            if (!globalCondition)
                return false;

            // Return true if the satisfied states match, false otherwise.
            return globalCondition.isSatisfied == requiredCondition.isSatisfied;
        }


        public static void setConditionsState(Condition[] newConditions) {

            // If there are no conditions, do nothing.
            if (Instance.conditions == null)
                return;

            // Reset all previous conditions states
            Instance.Reset();

            // Create a list with actual conditions 
            List<Condition> conditionsList = new List<Condition>(Instance.conditions);

            // Iterate through all conditions in newConditions array and look if they
            // are present in AllConditions
            for (int i = 0; i < newConditions.Length; i++) {

                int conditionIndex = conditionsList.IndexOf(conditionsList.Find(condition => condition.hash == newConditions[i].hash));

                if (conditionIndex != -1) {
                    conditionsList[conditionIndex].isSatisfied = newConditions[i].isSatisfied;
                }
                else {
                    Debug.LogError("No condition found in AllConditions project with name: " + newConditions[i].name + " Hash: " + newConditions[i].hash);
                }
            }
        }
    }
}
