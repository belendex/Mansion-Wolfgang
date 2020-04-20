using AKAGF.GameArchitecture.ScriptableObjects.DataPersistence;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Conditions;
using AKAGF.GameArchitecture.ScriptableObjects.Localization;
using AKAGF.GameArchitecture.ScriptableObjects.SceneControl;
using System;
using AKAeditor;
using UnityEditor;
using UnityEngine;
using AKAGF.GameArchitecture.ScriptableObjects.Events;
using AKAGF.GameArchitecture.ScriptableObjects.Variables;
using AKAGF.GameArchitecture.ScriptableObjects.QuestsSystem;
using AKAGF.GameArchitecture.ScriptableObjects.Inventory;

public class AKAGF_Window : EditorWindow {

    public enum TOP_TAB_OPTIONS { Conditions, Inventories, Quests, Localization }
    private TOP_TAB_OPTIONS currentTopTabOption;

    public enum BOTTOM_TAB_OPTIONS { GlobalEvents, GlobalVariables, GameScenes, Persistence }
    private BOTTOM_TAB_OPTIONS currentBottomTabOption;

    private static AKAGF_Window window;
    private Vector2 scrollPos;
    private const float buttonWidth = 25f;
    private const string placeHolderName = "New Name";
    private static string newElementName = placeHolderName;
    private static int selectedIndex = 0;

    [MenuItem("Window/AKAGF Window")]
    private static void Init() {
        // Get existing open window or if none, make a new one:
        window = (AKAGF_Window)EditorWindow.GetWindow(typeof(AKAGF_Window));
        window.Show();
    }

    private void OnGUI() {

        if (!window)
            window = (AKAGF_Window)EditorWindow.GetWindow(typeof(AKAGF_Window));

        EditorTools.createTitleBox("AKAGF Options");

        //Tab options
        drawTabs();

        EditorTools.createHorizontalSeparator(0f);

        // ScrollView
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, GUILayout.MaxWidth(window.position.width));

        switch (currentTopTabOption) {
            case TOP_TAB_OPTIONS.Conditions: drawAllconditionsTab();
                break;
            case TOP_TAB_OPTIONS.Inventories: drawInventoriesTab();
                break;
            case TOP_TAB_OPTIONS.Quests: drawQuestsTab();
                break;
            case TOP_TAB_OPTIONS.Localization: drawLocalizationTab();
                break;
        }

        switch (currentBottomTabOption) {
            case BOTTOM_TAB_OPTIONS.GlobalEvents:
                drawVariablesSubWindow<GameEvent>("Game Event", ref newElementName, GameEventEditor.BASE_PATH, ref selectedIndex);
                break;
            case BOTTOM_TAB_OPTIONS.GlobalVariables: drawGlobalVariablesTab();
                break;
            case BOTTOM_TAB_OPTIONS.GameScenes: drawGameScenesTab();
                break;
            case BOTTOM_TAB_OPTIONS.Persistence: drawPersistenceTab();
                break;
        }

        GUILayout.EndScrollView();
    }

    private void drawAllconditionsTab() {
        if (!AllConditions.Instance) {
            EditorTools.drawMessage("AllConditions has not been created yet.", MessageType.Info);
            if (EditorTools.createListButton("Create AllConditions Object", false))
                AllConditionsEditor.CreateAllConditionsAsset();
        }
        else {
            AllConditionsEditor.CreateEditor(AllConditions.Instance).OnInspectorGUI();
        }
    }

    private void drawInventoriesTab() {
        drawVariablesSubWindow<InventoryItemList>("Inventory Lists", ref newElementName, InventoryItemListEditor.BASE_PATH, ref selectedIndex);
    }


    private void drawQuestsTab() {
        drawVariablesSubWindow<QuestsList>("Quests Lists", ref newElementName, QuestsListEditor.BASE_PATH, ref selectedIndex);
    }


    private int currentLocalizationTabOption;


    private void drawLocalizationTab() {

        currentLocalizationTabOption = GUILayout.Toolbar(currentLocalizationTabOption, new string[]{"Master Control", "Localized Text Groups"}, GUILayout.MinWidth(100));

        if (currentLocalizationTabOption == 0) {
            if (!AllGameLanguages.Instance) {
                EditorTools.drawMessage("AllGameLanguages has not been created yet.", MessageType.Info);
                if (EditorTools.createListButton("Create AllGameLanguages Object", false))
                    AllGameLanguagesEditor.CreateAllGameLanguagesAsset();
            }
            else {
                Editor.CreateEditor(AllGameLanguages.Instance).OnInspectorGUI();
            }
        }
        else {

            drawVariablesSubWindow<LocalizedTextsGroup>("Localized Texts Groups", ref newElementName, LocalizedTextsGroupEditor.BASE_PATH, ref selectedIndex);
        }
    }


    public static void drawVariablesSubWindow<T>(string titleLabel, ref string newElementName, string path, ref int selectedIndex ) where T : ScriptableObject {

        T variable = null;

        GUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField(titleLabel, EditorStyles.boldLabel);
        GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);

        // Get and display a string for the name of a new Event.
        newElementName = EditorGUILayout.TextField(GUIContent.none, newElementName);

        if (EditorTools.createListButton("Add", false, GUILayout.ExpandWidth(false))) {
            ScriptableObjectUtility.createSingleScriptableObject<T>(path, newElementName);
            newElementName = placeHolderName;
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);

        EditorTools.createPopUpMenuWithObjectsNames(ref variable, ref selectedIndex, "");
        if (variable != null && EditorTools.createListButton("-", true, GUILayout.Width(buttonWidth))) {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(variable));
        }

        if (variable != null && EditorTools.createListButton("Rename", false, GUILayout.ExpandWidth(false))) {

            string renameError = AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(variable), newElementName);

            if (!renameError.Equals("")) {

                Debug.LogError(renameError);
            }
            else {
                variable.name = newElementName;
                newElementName = placeHolderName;
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        if (variable != null)
            Editor.CreateEditor(variable).OnInspectorGUI();

    }


    public enum VARS_OPTIONS { Bools, Doubles, Floats, Ints, Strings, Vector3}
    private VARS_OPTIONS currentVarsTabOption;

    private void drawGlobalVariablesTab() {

        string[] enumStringsVars = Enum.GetNames(typeof(VARS_OPTIONS));

        currentVarsTabOption = (VARS_OPTIONS)GUILayout.Toolbar((int)currentVarsTabOption, enumStringsVars, GUILayout.MinWidth(100));

        switch (currentVarsTabOption) {
            case VARS_OPTIONS.Bools:
                drawVariablesSubWindow<BoolVar>("Global Bool", ref newElementName, BoolVarEditor.BASE_PATH, ref selectedIndex);
                break;
            case VARS_OPTIONS.Doubles:
                drawVariablesSubWindow<DoubleVar>("Global Double", ref newElementName, DoubleVarEditor.BASE_PATH, ref selectedIndex);
                break;
            case VARS_OPTIONS.Floats:
                drawVariablesSubWindow<FloatVar>("Global Float", ref newElementName, FloatVarEditor.BASE_PATH, ref selectedIndex);
                break;
            case VARS_OPTIONS.Ints:
                drawVariablesSubWindow<IntVar>("Global Int", ref newElementName, IntVarEditor.BASE_PATH, ref selectedIndex);
                break;
            case VARS_OPTIONS.Strings:
                drawVariablesSubWindow<StringVar>("Global String", ref newElementName, StringVarEditor.BASE_PATH, ref selectedIndex);
                break;
            case VARS_OPTIONS.Vector3:
                drawVariablesSubWindow<Vector3Var>("Global Vector3", ref newElementName, Vector3VarEditor.BASE_PATH, ref selectedIndex);
                break;
        }
    }


    private void drawGameScenesTab() {
        if (!AllGameScriptableScenes.Instance) {
            EditorTools.drawMessage("AllGameScriptableScenes has not been created yet.", MessageType.Info);
            if (EditorTools.createListButton("Create AllScriptableScenes Object", false))
                AllGameScriptableScenesEditor.CreateAllGameScriptableScenesAsset();
        }
        else {
            Editor.CreateEditor(AllGameScriptableScenes.Instance).OnInspectorGUI();
        }
    }


    private void drawPersistenceTab() {
        if (!AllSavesData.Instance) {
            EditorTools.drawMessage("AllSavesData. has not been created yet.", MessageType.Info);
            if (EditorTools.createListButton("Create AllSavesData. Object", false))
                AllSavesDataEditor.CreateAllSavesDataAsset();
        }
        else {
            Editor.CreateEditor(AllSavesData.Instance).OnInspectorGUI();
        }

    }


    private void drawTabs() {

        string[] enumStringsTop = Enum.GetNames(typeof(TOP_TAB_OPTIONS));
        string[] enumStringsBottom = Enum.GetNames(typeof(BOTTOM_TAB_OPTIONS));

        currentTopTabOption = (TOP_TAB_OPTIONS)GUILayout.Toolbar((int)currentTopTabOption, enumStringsTop, GUILayout.MinWidth(100));
        

        switch (currentTopTabOption) {
            case TOP_TAB_OPTIONS.Conditions:
                resetBottomTabSelection();
                break;
            case TOP_TAB_OPTIONS.Inventories:
                resetBottomTabSelection();
                break;
            case TOP_TAB_OPTIONS.Quests:
                resetBottomTabSelection();
                break;
            case TOP_TAB_OPTIONS.Localization:
                resetBottomTabSelection();
                break;
        }

        currentBottomTabOption = (BOTTOM_TAB_OPTIONS)GUILayout.Toolbar((int)currentBottomTabOption, enumStringsBottom, GUILayout.MinWidth(100));

        switch (currentBottomTabOption) {
            case BOTTOM_TAB_OPTIONS.GlobalEvents:
                resetTopTabSelection();
                break;
            case BOTTOM_TAB_OPTIONS.GlobalVariables:
                resetTopTabSelection();
                break;
            case BOTTOM_TAB_OPTIONS.GameScenes:
                resetTopTabSelection();
                break;
            case BOTTOM_TAB_OPTIONS.Persistence:
                resetTopTabSelection();
                break;
        }

    }

    private void resetTopTabSelection() {
        currentTopTabOption = (TOP_TAB_OPTIONS)Enum.GetNames(typeof(TOP_TAB_OPTIONS)).Length;
    }

    private void resetBottomTabSelection() {
        currentBottomTabOption = (BOTTOM_TAB_OPTIONS)Enum.GetNames(typeof(BOTTOM_TAB_OPTIONS)).Length;
    }
}
