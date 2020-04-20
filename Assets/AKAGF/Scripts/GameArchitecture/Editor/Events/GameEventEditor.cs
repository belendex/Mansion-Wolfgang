using AKAeditor;
using AKAGF.GameArchitecture.Literals;
using AKAGF.GameArchitecture.ScriptableObjects.Events;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor {

    private GameEvent e;

    public const string BASE_PATH = AKAGF_PATHS.SCRIPTABLE_VARS_PATH + "Events/";

    [MenuItem(AKAGF_PATHS.SCRIPTABLE_VARS_MENU_PATH + "Event")]
    public static void createGameEvent() {

        ScriptableObjectUtility.createSingleScriptableObject<GameEvent>(BASE_PATH, "New Scriptable Event");
    }

    private void OnEnable() {
        e = target as GameEvent;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        GUI.enabled = Application.isPlaying;

        if (GUILayout.Button("Raise Event"))
            e.Raise();
    }
}
