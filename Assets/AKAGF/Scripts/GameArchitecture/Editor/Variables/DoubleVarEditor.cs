using AKAeditor;
using AKAGF.GameArchitecture.Literals;
using AKAGF.GameArchitecture.ScriptableObjects.Variables;
using UnityEditor;


[CustomEditor(typeof(DoubleVar))]
public class DoubleVarEditor : Editor {

    public const string BASE_PATH = AKAGF_PATHS.SCRIPTABLE_VARS_PATH + "Doubles/";

    [MenuItem(AKAGF_PATHS.SCRIPTABLE_VARS_MENU_PATH + "Double")]
    public static void createDoubleVar() {
        ScriptableObjectUtility.createSingleScriptableObject<DoubleVar>(BASE_PATH, "New Double Var");
    }
}
