using AKAeditor;
using AKAGF.GameArchitecture.Literals;
using AKAGF.GameArchitecture.ScriptableObjects.Variables;
using UnityEditor;


[CustomEditor(typeof(FloatVar))]
public class FloatVarEditor : Editor {

    public const string BASE_PATH = AKAGF_PATHS.SCRIPTABLE_VARS_PATH + "Floats/";

    [MenuItem(AKAGF_PATHS.SCRIPTABLE_VARS_MENU_PATH + "Float")]

    public static void createFloatVar() {
        ScriptableObjectUtility.createSingleScriptableObject<FloatVar>(BASE_PATH, "New Float Var");
    }
}
