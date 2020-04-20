using UnityEditor;
using UnityEngine;

public static class EditorColors {

    private static readonly Color MAINTITLE_COLOR = new Color(.043f, .127f, .225f, .45f);
    private static readonly Color SUBTITLE_COLOR = new Color(.043f, .127f, .225f, .3f);
    private static readonly Color SUBTITLE2_COLOR = new Color(.043f, .127f, .225f, .1f);
    private static readonly Color ADD_BUTTON_COLOR = Color.green;
    private static readonly Color REMOVE_BUTTON_COLOR = new Color(0.75f, 0f, 0f, 0.9f);

    public enum SCOPE { GLOBAL, CONTENT, BCKG }

    public static void SET_DEFAULT_COLOR() {
        GUI.color = EditorGUIUtility.isProSkin
            ? new Color32(56, 56, 56, 255)
            : new Color32(255, 255, 255, 255);

        GUI.backgroundColor = EditorGUIUtility.isProSkin
            ? new Color32(56, 56, 56, 255)
            : new Color32(255, 255, 255, 255);
    }

    private static void setColor(Color color, SCOPE scope = SCOPE.BCKG) {
        switch (scope){
            case SCOPE.GLOBAL: GUI.color = color;
                break;
            case SCOPE.CONTENT: GUI.contentColor = color;
                break;
            case SCOPE.BCKG: GUI.backgroundColor = color;
                break;
            default:
                Debug.Log("No Scope for GUI color found for " + scope.ToString());
                break;
        }
    }

    public static void SET_MAINTITLE_COLOR(SCOPE scope = SCOPE.BCKG) {
        setColor(MAINTITLE_COLOR, scope);
    }

    public static void SET_SUBTITLE_COLOR(SCOPE scope = SCOPE.BCKG) {
        setColor(SUBTITLE_COLOR, scope);
    }

    public static void SET_SUBTITLE2_COLOR(SCOPE scope = SCOPE.BCKG) {
        setColor(SUBTITLE2_COLOR, scope);
    }

    public static void SET_ADD_BUTTON_COLOR(SCOPE scope = SCOPE.BCKG) {
        setColor(ADD_BUTTON_COLOR, scope);
    }

    public static void SET_REMOVE_BUTTON_COLOR(SCOPE scope = SCOPE.BCKG) {
        setColor(REMOVE_BUTTON_COLOR, scope);
    }
}
