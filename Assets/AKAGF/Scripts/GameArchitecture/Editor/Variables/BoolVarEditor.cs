using AKAeditor;
using AKAGF.GameArchitecture.Literals;
using AKAGF.GameArchitecture.ScriptableObjects.Variables;
using UnityEditor;


[CustomEditor(typeof(BoolVar))]
public class BoolVarEditor : Editor {

    public const string BASE_PATH = AKAGF_PATHS.SCRIPTABLE_VARS_PATH + "Bools/";

    [MenuItem(AKAGF_PATHS.SCRIPTABLE_VARS_MENU_PATH  + "Bool")]

    public static void createBoolVar() {

        ScriptableObjectUtility.createSingleScriptableObject<BoolVar>(BASE_PATH, "New Bool Var");
    }

    public void ala() {
        createBoolVar();
    }
}

