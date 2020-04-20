using System.Collections.Generic;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.DelayedReactions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BehaviourReaction))]
public class BehaviourReactionEditor : ReactionEditor {

    private const string tooltipText = "This Reaction is for turning Behaviours on and off. " +
        "Behaviours are a subset of Components which have the enabled property," +
        " for example all MonoBehaviours are Behaviours as well as Animators, AudioSources and many more.";

    private BehaviourReaction targetReaction;
    

    protected override void Init(){

        targetReaction = target as BehaviourReaction;
    }

    protected override void DrawReaction() {

        serializedObject.Update();

        targetReaction.behavioursContainerObject = EditorGUILayout.ObjectField("Behaviour Container", targetReaction.behavioursContainerObject, typeof(GameObject), true) as GameObject;

        targetReaction.enabledState = EditorGUILayout.Toggle("Enabled State", targetReaction.enabledState);


        // There is a valid gameobject reference where looking for behaviours
        if (targetReaction.behavioursContainerObject) {

            // Get all the behaviours component attached to the GameObject
            Behaviour[] behaviours = targetReaction.behavioursContainerObject.GetComponents<Behaviour>();

            //there is at least one Behaviour in the gameobject
            if (behaviours.Length > 0) {
                        
                // Create an array of strings to store the names of the found behaviours
                string[] behavioursNames = new string[behaviours.Length];
                for (int j = 0; j < behaviours.Length; j++) {
                    behavioursNames[j] = behaviours[j].GetType().ToString();
                }

                // Add all the names to the mask just for graphical representation
                targetReaction.behaviorStatesflags = EditorGUILayout.MaskField("Behaviours Affected", targetReaction.behaviorStatesflags, behavioursNames);

                List<Behaviour> selectedOptions = new List<Behaviour>();

                for (int j = 0; j < behaviours.Length; j++) {
                    if ((targetReaction.behaviorStatesflags & (1 << j)) == (1 << j)) selectedOptions.Add(behaviours[j]);
                }

                // Store the behaviours in target behaviours list
                targetReaction.behaviours = selectedOptions.ToArray();


            } else {
                // If there is no behaviours in current gameObject
                targetReaction.behaviours = new Behaviour[0];
                targetReaction.behaviorStatesflags = 0;
            }

        }
        else {
            targetReaction.behaviours = new Behaviour[0];
            targetReaction.behaviorStatesflags = 0;
        }

        serializedObject.ApplyModifiedProperties();
        

    }

    
    protected override GUIContent GetFoldoutLabel () {
        return new GUIContent("Behaviour Reaction", tooltipText);
    }
}
