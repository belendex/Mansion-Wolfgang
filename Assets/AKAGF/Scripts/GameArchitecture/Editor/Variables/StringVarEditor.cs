using AKAeditor;
using AKAGF.GameArchitecture.Literals;
using AKAGF.GameArchitecture.ScriptableObjects.Variables;
using UnityEditor;

[CustomEditor(typeof(StringVar))]
public class StringVarEditor : Editor {

    public const string BASE_PATH = AKAGF_PATHS.SCRIPTABLE_VARS_PATH + "Strings/";

    [MenuItem(AKAGF_PATHS.SCRIPTABLE_VARS_MENU_PATH + "String")]

    public static void createStringVar() {
        ScriptableObjectUtility.createSingleScriptableObject<StringVar>(BASE_PATH, "New String Var");
    }
}
