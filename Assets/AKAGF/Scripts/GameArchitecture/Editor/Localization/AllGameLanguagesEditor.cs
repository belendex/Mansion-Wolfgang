using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using AKAeditor;
using AKAGF.GameArchitecture.Literals;
using AKAGF.GameArchitecture.ScriptableObjects.Localization;
using AKAGF.GameArchitecture.Utils.FileManagement;

[CustomEditor(typeof(AllGameLanguages))]
[InitializeOnLoad]
public class AllGameLanguagesEditor : Editor {

    private AllGameLanguages allLanguages;
    private static GameLanguage[] fileISOLanguages;

    private const string ISOCODES_FILE_PATH = AKAGF_PATHS.AKAGF_BASE_PATH + AKAGF_PATHS.ISO_LANGUAGES_FILE_NAME;
    private const string BASE_PATH = AKAGF_PATHS.SINGLETONS_FULLPATH;

    // Editor vars
    int ISOListSelectedIndex = 0;
    static bool[] isExpanded = new bool[0];


    // Call this function when the menu item is selected.
    [MenuItem(AKAGF_PATHS.LOCALIZATION_MENU_PATH + AKAGF_PATHS.ALLGAMELANGUAGES_MENU_NAME)]

    public static void CreateAllGameLanguagesAsset() {

        // Create an instance of the All object and make an asset for it.
        AllGameLanguages.Instance = 
            ScriptableObjectUtility.CreateSingletonScriptableObject(
                AllGameLanguages.Instance,
                BASE_PATH);

        // Create a new empty array if there is need of it.
        if (AllGameLanguages.Instance.gameLanguagesList == null)
            AllGameLanguages.Instance.gameLanguagesList = new List<GameLanguageItem>();

        checkPreviousLanguages();
    }

    private void OnDestroy() {
        ScriptableObjectUtility.RemoveScriptableObjectFromPreloadedAssets(AllGameLanguages.Instance);
    }

    private void OnEnable() {

        // target the instance
        allLanguages = target as AllGameLanguages;

        // Check if the game language list is already instantiated
        if (allLanguages.gameLanguagesList == null)
            allLanguages.gameLanguagesList = new List<GameLanguageItem>();


        // Try to load the ISO languages data from file
        // and load into fileISOLanguages array
        tryLoadISOLanguagesFile(ref fileISOLanguages);

        if (isExpanded.Length != allLanguages.gameLanguagesList.Count)
            Array.Resize(ref isExpanded, allLanguages.gameLanguagesList.Count);

        // Reset the ISO list popup
        ISOListSelectedIndex = 0;
    }

    public override void OnInspectorGUI() {

        GUI.enabled = !Application.isPlaying;

        // Update the state of the serializedObject to the current values of the target.
        serializedObject.Update();

        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorTools.createTitleBox("ALL GAME LANGUAGES MASTER CONTROL") ;

            createISOLanguagesBox();

        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.LabelField("Game Languages", EditorStyles.boldLabel);

            createCurrentGameLanguageBox();

        EditorGUILayout.Space();

            createGameLanguagesBoxes();

        EditorGUILayout.EndVertical();


            createGlobalDictionaryBox();

        EditorGUILayout.EndVertical();

        // Push the values from the serializedObject back to the target.
        serializedObject.ApplyModifiedProperties() ;

    }


    private void createISOLanguagesBox() {

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("ISO Languages", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);

        if (fileISOLanguages != null) {

            // Create the options for all iso languages stored in memory
            string[] isoLanguageOptions = new string[fileISOLanguages.Length];
            string prefix = "";

            // If the json items in the file are sorted by code, then they will be already sorted for popup submenus
            // so we can use the first char of the language code to order them.
            for (int i = 0; i < isoLanguageOptions.Length; i++)
            {
                prefix = fileISOLanguages[i].code[0] + "/";
                isoLanguageOptions[i] = prefix + fileISOLanguages[i].code + " - " + fileISOLanguages[i].name + " - " + fileISOLanguages[i].nativeName;
            }

            string label = "File Languages";
            EditorGUILayout.LabelField(label, GUILayout.Width((label.Length * 7)));
            ISOListSelectedIndex = EditorGUILayout.Popup(ISOListSelectedIndex, isoLanguageOptions);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);

            EditorColors.SET_ADD_BUTTON_COLOR();
            if (GUILayout.Button(" Add Selected Language to Game ", GUILayout.ExpandWidth(true))) {
                AddLanguage(fileISOLanguages[ISOListSelectedIndex]);
            }
            EditorColors.SET_DEFAULT_COLOR();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }


    public static void createCurrentGameLanguageBox() {

        GUILayout.BeginHorizontal() ;

        string[] gameLanguageOptions = new string[AllGameLanguages.Instance.gameLanguagesList.Count];

        for (int i = 0; i < gameLanguageOptions.Length; i++) {
            gameLanguageOptions[i] = AllGameLanguages.Instance.gameLanguagesList[i].ToString() ;
        }

        EditorGUI.indentLevel++;
        AllGameLanguages.Instance.setCurrentLanguage(EditorGUILayout.Popup("Current GL", AllGameLanguages.Instance.getCurrentLanguageIndex(), gameLanguageOptions));
        EditorGUI.indentLevel--;
        GUILayout.EndHorizontal() ;

    }


    private void setFoldoutDefaultColor(int index, ref GUIStyle myFoldoutStyle, ref string defaultText)  {

        if (AllGameLanguages.Instance.gameLanguagesList[index].isDefault) {
            GUI.color = Color.yellow;
            myFoldoutStyle.fontStyle = FontStyle.Bold;
            defaultText = " [DEFAULT]";
        }
        else {
            EditorColors.SET_DEFAULT_COLOR();
            myFoldoutStyle.fontStyle = FontStyle.Normal;
            defaultText = "";
        }

    }


    private void createGameLanguagesBoxes() {

        GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        string defaultText = "";

        for (int i = 0; i < allLanguages.gameLanguagesList.Count; i++) {

            setFoldoutDefaultColor(i, ref myFoldoutStyle, ref defaultText);

            EditorGUILayout.BeginVertical(GUI.skin.box) ;
            
            EditorGUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);

            isExpanded[i] = EditorGUILayout.Foldout(isExpanded[i], allLanguages.gameLanguagesList[i].gameLanguage.code + " - " + allLanguages.gameLanguagesList[i].gameLanguage.name + defaultText, true, myFoldoutStyle);

            EditorColors.SET_DEFAULT_COLOR();

            allLanguages.gameLanguagesList[i].isDefault = EditorGUILayout.Toggle(allLanguages.gameLanguagesList[i].isDefault, GUILayout.Width(30));

            if (allLanguages.gameLanguagesList[i].isDefault) {
                allLanguages.setDefaultLanguage(i) ;
            }

            CreateRemoveButton(i) ;

            EditorGUILayout.EndHorizontal();
            
            // If the foldout is open show the expanded GUI.
            if (isExpanded[i]) {
                ExpandedGUI(allLanguages.gameLanguagesList[i]) ;
            }

            EditorGUILayout.EndVertical();

        }

        allLanguages.checkDefaultLanguage();

    }


    private void ExpandedGUI(GameLanguageItem gameLanguageItem) {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        createLabel("Flag Sprite", "", 7);
        
        EditorGUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
        //Sprite
        gameLanguageItem.flag = EditorGUILayout.ObjectField("", gameLanguageItem.flag, typeof(Sprite), false, GUILayout.Width(65)) as Sprite;
        EditorGUILayout.EndHorizontal();

        createLabel("Code: ", gameLanguageItem.gameLanguage.code, 7);
        createLabel("Name: ", gameLanguageItem.gameLanguage.name, 7);
        createLabel("Native Name: ", gameLanguageItem.gameLanguage.nativeName, 6.5f);
        EditorGUILayout.EndVertical();

    }


    private void createLabel(string titleText, string text, float spacer) {
        EditorGUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
        EditorGUILayout.LabelField(titleText, EditorStyles.boldLabel, GUILayout.Width(titleText.Length * spacer));
        EditorGUILayout.LabelField(text);
        EditorGUILayout.EndHorizontal();
    }

    private void createGlobalDictionaryBox() {

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("Global Dictionary", EditorStyles.boldLabel);

        EditorTools.drawMessage("If you want to access the localized Texts from code through a static reference using their groupName + id as key, mark this option.", MessageType.Info);

        allLanguages.useGlobalDictionary = EditorGUILayout.Toggle("Use Global Dictionary", allLanguages.useGlobalDictionary);

        if (allLanguages.useGlobalDictionary) {

            checkValidDictionary();

            EditorTools.drawMessage("There are " + allLanguages.allLocalizedTexts.Count + " localized texts references in the global dictionary.", MessageType.Info);

            if (GUILayout.Button(new GUIContent("Clear Dictionary", "Remove all references from the global dictionary.")))
                allLanguages.allLocalizedTexts.Clear();

            if (GUILayout.Button(new GUIContent("Build Dictionary", "Create the references for all localized Text assest within the project.")))
                fillDictionary();

            if (GUILayout.Button("Test")) {
                for (int i = 0; i < allLanguages.allLocalizedTexts.Count; i++) {
                    Debug.Log(allLanguages.allLocalizedTexts[i].id);
                }
            }
        }

        EditorGUILayout.EndVertical() ;
    }

    private void fillDictionary() {


        LocalizedText[] allLocalizedTexts = ScriptableObjectUtility.GetAllInstances<LocalizedText>();
        allLanguages.allLocalizedTexts.Clear();

        Undo.RecordObject(allLanguages, "allLanguages dictionary");

        string key;

        for (int i = 0; i < allLocalizedTexts.Length; i++) {
           key = allLocalizedTexts[i].textsGroup.name + "-" + allLocalizedTexts[i].textID;
           allLanguages.allLocalizedTexts.Add(new LocalizedTextData { id = key, text = allLocalizedTexts[i]});
        }
            
        EditorUtility.SetDirty(allLanguages);
        AssetDatabase.SaveAssets() ;

        Debug.Log(allLanguages.allLocalizedTexts.Count + " references added to the dictionary.") ;
    }


    private void checkValidDictionary() {

        bool valid = true;
        string key;

        try {
            for (int i = 0; i < allLanguages.allLocalizedTexts.Count; i++) {
                key = allLanguages.allLocalizedTexts[i].text.textsGroup.name + "-" + allLanguages.allLocalizedTexts[i].text.textID;
                if (!allLanguages.allLocalizedTexts.Exists(x => x.id.Equals(key))) {
                    valid = false;
                    break;
                }
            }
        }
        catch {
            valid = false;
        }
        finally {

            if (!valid) {
                Debug.Log("The data in the dictionary is not coherent. Rebuilding dictionary...");
                fillDictionary();
            }
        }
    }


    private void CreateRemoveButton(int index) {

        // Display a button showing 'Remove Collection' which removes the target from the Interactable when clicked.
        if (EditorTools.createListButton(" - ", true) && EditorUtility.DisplayDialog("Remove Game Language?",
            "Are you sure you want to remove all text in Localized Items for this Language? "
            , "Yes, totally sure.", "Cancel"))
        {

            allLanguages.gameLanguagesList.Remove(allLanguages.gameLanguagesList[index]);
            EditorUtility.SetDirty(allLanguages);
            return;
        }

    }


    private void AddLanguage(GameLanguage newGameLanguage) {

        // In first place check if the language is already present as a Game Language
        if (allLanguages.gameLanguagesList.Exists(x => x.gameLanguage.code == newGameLanguage.code)) {
            Debug.LogWarning(newGameLanguage.code + " " + newGameLanguage.name + " language is already present as a Game Language.");
            return;
        }

        // New game language added
        GameLanguageItem newLanguageItem = new GameLanguageItem(newGameLanguage);
        allLanguages.gameLanguagesList.Add(newLanguageItem);

        if(isExpanded.Length != allLanguages.gameLanguagesList.Count)
            Array.Resize(ref isExpanded, allLanguages.gameLanguagesList.Count);

        // Mark the object as changed, so the editor can save the changes
        EditorUtility.SetDirty(allLanguages) ;
    }


    private static void checkPreviousLanguages() {
        // Check if there are already any LocalizableTexts previous to the creation
        // of AllGameLanguages Instance, in positive case, add the languages contained as GameLanguages to it.
        // This is usefull in case that AllGameLanguages may was accidentaly deleted, to avoid
        // losing all the localized Text data when the LocalizedTexts load the config 
        // data of the new AllGameLanguages instance in the OnEnable event of their Editor.
        LocalizedText[] projectLocalizedTextList = ScriptableObjectUtility.GetAllInstances<LocalizedText>();

        if (projectLocalizedTextList != null && projectLocalizedTextList.Length > 0) {

            LocalizedText locTAux = projectLocalizedTextList[0];

            for (int i = 0; i < locTAux.localizedTextsList.Count; i++){
                if (!AllGameLanguages.Instance.gameLanguagesList.Exists(x => x.gameLanguage.code.Equals(locTAux.localizedTextsList[i].languageInfo.code)))
                       AllGameLanguages.Instance.gameLanguagesList.Add(
                           new GameLanguageItem(locTAux.localizedTextsList[i].languageInfo));
            }
        }   
    }


    private void tryLoadISOLanguagesFile(ref GameLanguage[] isoLanguagesArray) {

        // load first from web, so we can ensure the file is in last version
        if (isoLanguagesArray == null) {
            EditorWWW editorWWW = new EditorWWW();
            editorWWW.StartWWW(AKAGF_PATHS.ISO_LANGUAGES_WEB_FILE_PATH, null, responseWWW);
        }

        if (isoLanguagesArray == null) {
            if (FileManager.LoadFromFile<List<GameLanguage>>(ISOCODES_FILE_PATH) != null) {
                isoLanguagesArray = FileManager.LoadFromFile<List<GameLanguage>>(ISOCODES_FILE_PATH).ToArray();
            }
        }  
    }


    private void responseWWW(string result, object[] resultObjs) {
        FileManager.SaveTextToFile(result, ISOCODES_FILE_PATH);
        AssetDatabase.Refresh();
        OnEnable();
    } 
}