using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.ImmediateReactions;
using AKAGF.GameArchitecture.ScriptableObjects.Variables;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScriptableVarReaction))]
public class ScriptableVarReactionEditor : ReactionEditor {

    private ScriptableVarReaction scriptableVarReaction;

    private const string tooltipText = "TODO";


    protected override void Init() {
        scriptableVarReaction = target as ScriptableVarReaction;
    }

    protected override void DrawReaction() {

        scriptableVarReaction.varType = (VarType) EditorGUILayout.EnumPopup("Var Type", scriptableVarReaction.varType);

        switch (scriptableVarReaction.varType){
            case VarType.FLOAT:
                scriptableVarReaction.floatVar = EditorGUILayout.ObjectField("Float Var", scriptableVarReaction.floatVar, typeof(FloatVar), true) as FloatVar;
                scriptableVarReaction.floatValue = EditorGUILayout.FloatField("Float Value", scriptableVarReaction.floatValue);
                break;

            case VarType.INT:
                scriptableVarReaction.intVar = EditorGUILayout.ObjectField("Int Var", scriptableVarReaction.intVar, typeof(IntVar), true) as IntVar;
                scriptableVarReaction.intValue = EditorGUILayout.IntField("Int Value", scriptableVarReaction.intValue);
                break;

            case VarType.DOUBLE:
                scriptableVarReaction.doubleVar = EditorGUILayout.ObjectField("Double Var", scriptableVarReaction.doubleVar, typeof(DoubleVar), true) as DoubleVar;
                scriptableVarReaction.doubleValue = EditorGUILayout.DoubleField("Double Value", scriptableVarReaction.doubleValue);
                break;

            case VarType.BOOL:
                scriptableVarReaction.boolVar = EditorGUILayout.ObjectField("Bool Var", scriptableVarReaction.boolVar, typeof(BoolVar), true) as BoolVar;
                scriptableVarReaction.boolValue = EditorGUILayout.Toggle("Bool Value", scriptableVarReaction.boolValue);
                break;

            case VarType.STRING:
                scriptableVarReaction.stringVar = EditorGUILayout.ObjectField("String Var", scriptableVarReaction.stringVar, typeof(StringVar), true) as StringVar;
                scriptableVarReaction.stringValue = EditorGUILayout.TextField("String Value", scriptableVarReaction.stringValue);
                break;
        }
    }


    protected override GUIContent GetFoldoutLabel () {
        return new GUIContent("ScriptableVar Reaction", tooltipText); 
    }
}
