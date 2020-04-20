using AKAGF.GameArchitecture.MonoBehaviours.GUI;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UITextElementWrapper))]
public class UITextElementWrapperDrawer : PropertyDrawer {

    /// <summary> Cached style to use to draw the popup button. </summary>
    private GUIStyle popupStyle;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        if (popupStyle == null) {
            popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
            popupStyle.imagePosition = ImagePosition.ImageOnly;
        }

        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);

        EditorGUI.BeginChangeCheck();

        SerializedProperty textType = property.FindPropertyRelative("textType");
        SerializedProperty uiText = property.FindPropertyRelative("uiText");
        SerializedProperty textMesh = property.FindPropertyRelative("textMesh");
        SerializedProperty text_UGUI = property.FindPropertyRelative("text_UGUI");
        SerializedProperty text_PRO = property.FindPropertyRelative("text_PRO");

        // Calculate rect for configuration button
        Rect buttonRect = new Rect(position);
        buttonRect.yMin += popupStyle.margin.top + 2f;
        buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
        position.xMin = buttonRect.xMax;

        // Store old indent level and set it to 0, the PrefixLabel takes care of it
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        textType.intValue = EditorGUI.Popup(buttonRect, textType.intValue, textType.enumDisplayNames, popupStyle);


        switch (textType.intValue) {
            case (int)UITextElementWrapper.TEXT_TYPE.TEXT:
                EditorGUI.PropertyField(position, uiText, GUIContent.none);
                break;

            case (int)UITextElementWrapper.TEXT_TYPE.TEXT_MESH:
                EditorGUI.PropertyField(position, textMesh, GUIContent.none);
                break;

            case (int)UITextElementWrapper.TEXT_TYPE.TEXT_MESH_PRO:
                EditorGUI.PropertyField(position, text_PRO, GUIContent.none);
                break;

            case (int)UITextElementWrapper.TEXT_TYPE.TEXT_MESH_PRO_UGUI:
                EditorGUI.PropertyField(position, text_UGUI, GUIContent.none);
                break;

        }

        if (EditorGUI.EndChangeCheck()) {
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
