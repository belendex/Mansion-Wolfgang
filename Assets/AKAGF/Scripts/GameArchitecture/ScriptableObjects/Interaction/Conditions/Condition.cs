using UnityEngine;

// This class is used to determine whether or not Reactions
// should happen.  Instances of Condition exist in two places:
// as assets which are part of the AllConditions asset and as
// part of ConditionCollections.  The Conditions that are part
// of the AllConditions asset are those that are set by
// Reactions and reflect the state of the game.  Those that
// are on ConditionCollections are compared to the
// AllConditions asset to determine whether other Reactions
// should happen.
namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Conditions
{
    [System.Serializable]
    public class Condition : ScriptableObject {

        public string description;      // A description of the Condition, for example 'BeamsOff'.
        public bool isSatisfied;       // Whether or not the Condition has been satisfied, for example are the beams off?
        public int hash;                // A number which represents the description.  This is used to compare ConditionCollection Conditions to AllConditions Conditions.

#if UNITY_EDITOR
        public string editorDescription; // A description only usefull for game developers at editing time.
        public bool isExpanded { get; set; } // If this variable is declared in the editor script the gui foldout doesn't work in custom window.
#endif

    }
}
