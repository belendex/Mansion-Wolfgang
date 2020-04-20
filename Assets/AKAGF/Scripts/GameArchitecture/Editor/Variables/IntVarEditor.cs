using AKAeditor;
using AKAGF.GameArchitecture.Literals;
using AKAGF.GameArchitecture.ScriptableObjects.Variables;
using UnityEditor;


[CustomEditor(typeof(IntVar))]
public class IntVarEditor : Editor {

    public const string BASE_PATH = AKAGF_PATHS.SCRIPTABLE_VARS_PATH + "Ints/";

    [MenuItem(AKAGF_PATHS.SCRIPTABLE_VARS_MENU_PATH + "Int")]

    public static void createIntVar() {
        ScriptableObjectUtility.createSingleScriptableObject<IntVar>(BASE_PATH, "New Int Var");
    }
}
