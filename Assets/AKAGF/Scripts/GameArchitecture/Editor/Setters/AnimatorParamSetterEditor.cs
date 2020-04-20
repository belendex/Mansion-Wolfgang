using AKAGF.GameArchitecture.MonoBehaviours.Setters;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimatorParamSetter))]
public class AnimatorParamSetterEditor : Editor {

    private AnimatorParamSetter setter;

    private SerializedProperty floatRefProperty;
    private SerializedProperty intRefProperty;
    private SerializedProperty boolRefProperty;

    private const string floatRefPropertyName = "floatVar";
    private const string intRefPropertyName = "intVar";
    private const string boolRefPropertyName = "boolVar";


    public void OnEnable() {
        setter = target as AnimatorParamSetter;

        floatRefProperty = serializedObject.FindProperty(floatRefPropertyName);
        intRefProperty = serializedObject.FindProperty(intRefPropertyName);
        boolRefProperty = serializedObject.FindProperty(boolRefPropertyName);

    }

    public override void OnInspectorGUI() {
        // Update the state of the serializedObject to the current values of the target.
        serializedObject.Update();

        setter.updateInEditMode = EditorGUILayout.Toggle("Update in Edit Mode", setter.updateInEditMode);

        setter.paramType = (AnimatorParamType)EditorGUILayout.EnumPopup("Animator Param Type", setter.paramType);

        switch (setter.paramType) {
            case AnimatorParamType.FLOAT:
                EditorGUILayout.PropertyField(floatRefProperty);
                break;

            case AnimatorParamType.INT:
                EditorGUILayout.PropertyField(intRefProperty);
                break;

            case AnimatorParamType.TRIGGER:
            case AnimatorParamType.BOOL:
                EditorGUILayout.PropertyField(boolRefProperty);
                break;
        }

        setter.Animator = EditorGUILayout.ObjectField("Animator", setter.Animator, typeof(Animator), true) as Animator;
        setter.ParameterName = EditorGUILayout.TextField("Parameter Name", setter.ParameterName);

        // Push data back from the serializedObject to the target.
        serializedObject.ApplyModifiedProperties();
    }
}
