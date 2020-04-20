using AKAeditor;
using AKAGF.GameArchitecture.Literals;
using AKAGF.GameArchitecture.ScriptableObjects.Variables;
using UnityEditor;


[CustomEditor(typeof(Vector3Var))]
public class Vector3VarEditor : Editor {

    public const string BASE_PATH = AKAGF_PATHS.SCRIPTABLE_VARS_PATH + "Vector3/";

    [MenuItem(AKAGF_PATHS.SCRIPTABLE_VARS_MENU_PATH + "Vector3")]

    public static void createFloatVar() {
        ScriptableObjectUtility.createSingleScriptableObject<Vector3Var>(BASE_PATH, "New Vector3 Var");
    }
}
