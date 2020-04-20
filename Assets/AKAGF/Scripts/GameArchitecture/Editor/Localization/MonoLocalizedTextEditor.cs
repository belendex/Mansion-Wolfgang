using AKAGF.GameArchitecture.MonoBehaviours.Localization;
using UnityEditor;

[CustomEditor(typeof(MonoLocalizedText))]
public class MonoLocalizedTextEditor : Editor {

    private MonoLocalizedText monoLT;
    LocalizedTextEditor localizedTextEditor;

    private void OnEnable() {
        monoLT = target as MonoLocalizedText;

    }

    public override void OnInspectorGUI() {

        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUI.indentLevel--;
        AllGameLanguagesEditor.createCurrentGameLanguageBox();
        EditorGUI.indentLevel++;

        if (monoLT.localizedText != null) {

            if (localizedTextEditor == null) {
                localizedTextEditor = CreateEditor(monoLT.localizedText) as LocalizedTextEditor;
                localizedTextEditor.editorType = LocalizedTextEditor.EditorType.INSPECTOR;
            }
            
            localizedTextEditor.OnInspectorGUI();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
