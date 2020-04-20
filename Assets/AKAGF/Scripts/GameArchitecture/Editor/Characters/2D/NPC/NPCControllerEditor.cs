using UnityEngine;
using UnityEditor;

public class NPCControllerEditor : Editor {

    public SerializedProperty randomMovementActiveProperty;       // The SerializedProperty representing an array of Conditions on a ConditionCollection.
    public SerializedProperty timeBehaviourTypeProperty;
    public SerializedProperty secondsToWaitProperty;
    public SerializedProperty minTimeProperty;
    public SerializedProperty maxTimeProperty;


}
