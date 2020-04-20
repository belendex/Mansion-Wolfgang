using UnityEngine;
using UnityEditor;
using AKAeditor;
using System.Collections.Generic;
using System;
using System.Text;
using AKAGF.GameArchitecture.Literals;
using AKAGF.GameArchitecture.ScriptableObjects.Localization;
using AKAGF.GameArchitecture.Utils.FileManagement;
using AKAGF.GameArchitecture.Utils.Text;

[CustomEditor(typeof(LocalizedTextsGroup))]
[InitializeOnLoad]
public class LocalizedTextsGroupEditor : Editor {

    public enum SYNC_OPTIONS { REMOTE_CSV_FILE, LOCAL_CSV_FILE }
    private SYNC_OPTIONS syncOption;

    private LocalizedTextsGroup locTextGroup;
    private LocalizedTextEditor[] localizedTextEditors;

    private const float messageGUILines = 3f;     // How many lines tall the GUI for the TextArea field should be.
    private string LocalizedTextsGroupName = "Localized Texts Group Name";

    private bool endReadCSV;
    private Vector2 scrollPos;
    private bool[] toggles = new bool[0];
    private int filesPending = 0;


    // path for saving the groups themselves.
    public const string BASE_PATH = AKAGF_PATHS.LOCALIZATION_GROUPS_PATH;

    //path for saving the csv files of each group
    private const string GROUPS_CSV_FILES_PATH = AKAGF_PATHS.LOCALIZATION_GROUPS_FILES_PATH;

    // Instantiation method
    [MenuItem(AKAGF_PATHS.LOCALIZATION_MENU_PATH + AKAGF_PATHS.LOCALIZED_TEXTS_GROUP_MENU_NAME)]

    public static LocalizedTextsGroup CreateLocalizedTextsGroup() {
        LocalizedTextsGroup asset = ScriptableObject.CreateInstance<LocalizedTextsGroup>();

        FileManager.createDirectory(BASE_PATH);

        if (AssetDatabase.LoadAssetAtPath<LocalizedTextsGroup>(BASE_PATH + AKAGF_PATHS.LOCALIZED_TEXTS_GROUP_NAME) == null) {
            AssetDatabase.CreateAsset(asset, BASE_PATH + AKAGF_PATHS.LOCALIZED_TEXTS_GROUP_NAME);
            AssetDatabase.SaveAssets();
            return asset;
        }

        Debug.Log("There is already a LocalizedTextsGroup.asset, change the name of that LocalizedTextsGroup first, then create another one.");

        return null;
    }


    private void OnEnable() {

        // Get the target reference
        locTextGroup = target as LocalizedTextsGroup;

        // Inizialize both lits it they aren't yet
        if (locTextGroup.localizedTextsList == null)
            locTextGroup.localizedTextsList = new List<LocalizedText>();


        if (locTextGroup.localGameLanguagesList == null)
            locTextGroup.localGameLanguagesList = new GameLanguageItem[0];

        // Create all the editors for the Localized texts belonging to this group
        if (localizedTextEditors == null)
            CreateEditors();

        // Cache the export/import option from the scriptable object
        syncOption = (SYNC_OPTIONS)locTextGroup.currentSyncOption ;

        // In case AllGameLanguages has a valid Instance...
        if (AllGameLanguages.Instance == null) return;

        locTextGroup.localGameLanguagesList = AllGameLanguages.Instance.gameLanguagesList.ToArray();

        if (toggles.Length != AllGameLanguages.Instance.gameLanguagesList.Count)
            Array.Resize(ref toggles, AllGameLanguages.Instance.gameLanguagesList.Count);

        // Mark all language import toggles at start
        for (int i = 0; i < toggles.Length; i++) {
            toggles[i] = true;
        }

        // Resize the URLs array of Sync settings to match the number
        // of gamelanguages in AllGameLanguages
        if (locTextGroup.remoteCsvUrls.Length != AllGameLanguages.Instance.gameLanguagesList.Count) 
            Array.Resize(ref locTextGroup.remoteCsvUrls, AllGameLanguages.Instance.gameLanguagesList.Count);

    }

    private void OnDisable() {

        // Nothing to destroy
        if (localizedTextEditors == null) return;

        // Destroy all the editors.
        for (int i = 0; i < localizedTextEditors.Length; i++) {
            DestroyImmediate(localizedTextEditors[i]);
        }

        // Null out the editor array.
        localizedTextEditors = null;
    }

    private LocalizedText AddLocalizedText(string LocalizedTextName = "") {

        // Create a new scriptable instance of LocalizedText
        LocalizedText newText = ScriptableObject.CreateInstance<LocalizedText>();

        Undo.RecordObject(newText, "new localized text");

        // Add the new item to the group that create its
        newText.textsGroup = this.locTextGroup;

        // Give the new LocalizedText a name
        if (LocalizedTextName == null || LocalizedTextName == "")
            newText.name = LocalizedTextName + locTextGroup.localizedTextsList.Count + 1;
        else
            newText.name = LocalizedTextName;

        // Assets folder name for the object must match the name 
        // of the LocalizedText ID which must be unique within the group
        newText.textID = newText.name;

        // Create a localizedTextsList with the same lenght as local Game LanguageList... 
        newText.localizedTextsList = new List<LocalizableElement>(locTextGroup.localGameLanguagesList.Length);

        // ...and create the localizableElements
        for (int i = 0; i < locTextGroup.localGameLanguagesList.Length; i++)
            newText.localizedTextsList.Add(new LocalizableElement(locTextGroup.localGameLanguagesList[i].gameLanguage, ""));

        // Attach the new localizedText as a child of the group in assests folder
        ScriptableObjectUtility.AddScriptableObject(locTextGroup, ref newText, ref locTextGroup.localizedTextsList, "Created new Localized Text");

        return newText;
    }

    private void CreateEditors() {
        // Create a new array for the editors which is the same length at the Items array.
        localizedTextEditors = new LocalizedTextEditor[locTextGroup.localizedTextsList.Count];

        // Go through all the empty array...
        for (int i = 0; i < localizedTextEditors.Length; i++) {
            // ... and create an editor with an editor type to display correctly.
            localizedTextEditors[i] = CreateEditor(locTextGroup.localizedTextsList[i]) as LocalizedTextEditor;
            localizedTextEditors[i].editorType = LocalizedTextEditor.EditorType.ALL_ASSETS;
        }
    }

    public override void OnInspectorGUI() {
        
        // Update the state of the serializedObject to the current values of the target.
        serializedObject.Update();

        GUILayout.BeginVertical(GUI.skin.box);

        EditorTools.createTitleBox(locTextGroup.name);

            createTitleBox();

        GUILayout.Space(10);

            createImportExportBox();

        GUILayout.Space(10);

            createLocalizedTextsBoxes();

        GUILayout.EndVertical();
        // Push data back from the serializedObject to the target.
        serializedObject.ApplyModifiedProperties();
    }

    private void createTitleBox() {
        GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
        //Renaming
        locTextGroup.name = EditorGUILayout.TextField("Localized Texts Group Name", locTextGroup.name as string);
        if (GUILayout.Button("Rename", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false))) {
            string assetPath = AssetDatabase.GetAssetPath(locTextGroup.GetInstanceID());
            AssetDatabase.RenameAsset(assetPath, locTextGroup.name);
            AssetDatabase.SaveAssets();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        EditorGUI.indentLevel++;
        // Description property
        EditorGUILayout.LabelField("Localized Texts Group Description");
        EditorGUI.indentLevel--;
        GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);

        locTextGroup.groupDescription =
            EditorGUILayout.TextArea(locTextGroup.groupDescription, EditorStyles.textArea, GUILayout.Height(EditorGUIUtility.singleLineHeight * messageGUILines));
        GUILayout.EndHorizontal();
    }

    private void createImportExportBox() {

        // Import/Export options
        GUILayout.BeginVertical(GUI.skin.box);

        EditorTools.createTitleBox("Load / save Options", true);

        GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
        EditorGUILayout.LabelField("Mode");
        
        syncOption = (SYNC_OPTIONS) EditorGUILayout.EnumPopup(syncOption, GUILayout.ExpandWidth(true));
        locTextGroup.currentSyncOption = (int)syncOption;

        GUILayout.EndHorizontal();

        
        if (syncOption == SYNC_OPTIONS.REMOTE_CSV_FILE) {

            GUIContent guiCont;
            string langCode;
            for (int i = 0; i < locTextGroup.localGameLanguagesList.Length; i++) {
                langCode = locTextGroup.localGameLanguagesList[i].gameLanguage.code;
                GUILayout.BeginHorizontal(GUI.skin.box);
                if(toggles[i])
                    guiCont = new GUIContent(" " + langCode  + "_sheet_URL", 
                        EditorGUIUtility.IconContent("RotateTool").image,
                        "Importing enabled for " + langCode + " language.");
                else
                    guiCont = new GUIContent(" " + langCode + "_sheet_URL",
                        EditorGUIUtility.IconContent("RotateTool On").image,
                        "Importing disabled for " + langCode + " language.");

                locTextGroup.remoteCsvUrls[i] = 
                    EditorGUILayout.TextField(guiCont, locTextGroup.remoteCsvUrls[i]);
                GUILayout.EndHorizontal();
            }
            
            GUILayout.Space(5);

            EditorGUI.BeginDisabledGroup(drawToggles() && filesPending == 0);

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent( " Import", EditorTools.getIcon("CollabPull")))) {
                for (int i = 0; i < locTextGroup.remoteCsvUrls.Length; i++) {
                    if (!toggles[i]) continue;

                    importFromRemoteCsvSheet(i);
                }
            }

            GUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();
        }
        else if (syncOption == SYNC_OPTIONS.LOCAL_CSV_FILE) {

            string[] fileNames = new string[locTextGroup.localGameLanguagesList.Length];
            int count = 0;
            string messageFileName = "";

            for (int i = 0; i < locTextGroup.localGameLanguagesList.Length; i++) {

                if (!toggles[i]) continue;
                count++;
                fileNames[i] = locTextGroup.name + " - " + locTextGroup.localGameLanguagesList[i].gameLanguage.code + ".csv";
                messageFileName += fileNames[i];

                if(i != locTextGroup.localGameLanguagesList.Length - 1) messageFileName += "\n";
            }

            EditorGUILayout.HelpBox(count +
                                    " files will be loaded/saved from/to folder:\n" +
                                    AKAGF_PATHS.LOCALIZATION_GROUPS_FILES_PATH + locTextGroup.name + "/\n\n" +
                                    "With the names: \n" +
                                    messageFileName, MessageType.Info);

            GUILayout.Space(5);

            EditorGUI.BeginDisabledGroup(drawToggles() && filesPending == 0);
            
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent(" Load from file", EditorTools.getIcon("CollabPush")))) {

                loadFromCSV(fileNames, GROUPS_CSV_FILES_PATH + locTextGroup.name + "/");

            }

            if (GUILayout.Button(new GUIContent(" Save to file", EditorTools.getIcon("CollabPull")))) {

                saveToCSV(fileNames, GROUPS_CSV_FILES_PATH + locTextGroup.name + "/");
              
            }

            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(5);
        GUILayout.EndVertical();

    }

    private bool drawToggles() {

        bool allEmpty = true;

        EditorGUILayout.BeginVertical(GUI.skin.box);
        int rows = Mathf.CeilToInt(toggles.Length / 4f);
        float toggleWidth = EditorGUIUtility.currentViewWidth/ (toggles.Length/rows);
        EditorGUI.indentLevel++;

        for (int j = 0; j < rows; j++) {
            GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
            for (int i = j*4; i < toggles.Length && i < j*4+4; i++) {
                toggles[i] = EditorGUILayout.ToggleLeft(locTextGroup.localGameLanguagesList[i].gameLanguage.code, toggles[i], GUILayout.Width(toggleWidth-45));
                if(toggles[i]) allEmpty = false;
            }
            GUILayout.EndHorizontal();
        }
        
        EditorGUI.indentLevel--;
        GUILayout.EndVertical();

        return allEmpty;
    }

    private void createLocalizedTextsBoxes() {

        GUILayout.BeginVertical(GUI.skin.box);

        EditorTools.createTitleBox("Localized Texts", true);


        if (locTextGroup.localizedTextsList.Count > 0) {
            EditorGUILayout.LabelField("Total: " + locTextGroup.localizedTextsList.Count, EditorStyles.boldLabel);
        }
        else {
            GUILayout.Label("This Localized Texts Group is Empty.");
        }

        GUILayout.BeginHorizontal();
        LocalizedTextsGroupName = EditorGUILayout.TextField("New Localized Text Name", LocalizedTextsGroupName);

        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        if (EditorTools.createListButton(" Add Text ", false, GUILayout.ExpandWidth(true))) {
            AddLocalizedText(LocalizedTextsGroupName);
        }

        if (EditorTools.createListButton(" Remove All ", true, GUILayout.ExpandWidth(true)) && EditorUtility.DisplayDialog("Remove All Localized Texts in " + locTextGroup.name +"?",
            "Are you sure you want to remove all Localized texts from this group? "
            , "Yes, totally sure.", "Cancel")) {

            for (int i = 0; i < localizedTextEditors.Length; i++) {
                localizedTextEditors[i].removeLocalizedText(false);
            }

            AssetDatabase.SaveAssets();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        // If there are different number of editors to Items, create them afresh.
        if (localizedTextEditors.Length != locTextGroup.localizedTextsList.Count) {
            // Destroy all the old editors.
            for (int i = 0; i < localizedTextEditors.Length; i++) {
                DestroyImmediate(localizedTextEditors[i]);
            }

            // Create new editors.
            CreateEditors();
        }

        
        // Display all the items.
        for (int i = 0; i < localizedTextEditors.Length; i++) {
            localizedTextEditors[i].OnInspectorGUI();

            if (i != localizedTextEditors.Length - 1)
                EditorTools.createHorizontalSeparator();   
        }

        if (GUI.changed) {
            EditorUtility.SetDirty(locTextGroup);
        }

        GUILayout.EndVertical();

    }

    #region CSV sync methods
    private void saveToCSV(string[] fileNames, string path) {

        // using stringbuilder for performance
        StringBuilder csvString = new StringBuilder();
        filesPending++;

        // Go through all the filepaths creting one file for each language
        for (int i = 0; i < fileNames.Length; i++) {
            if (!toggles[i]) continue;
            // Go through all LocalizedTexts inside this group
            for (int j = 0; j < locTextGroup.localizedTextsList.Count; j++) {
                csvString.Append(locTextGroup.localizedTextsList[j].textID +  ",\"" + locTextGroup.localizedTextsList[j].localizedTextsList[i].Text + "\"\n");
            }

            FileManager.createDirectory(path);

            if (FileManager.SaveTextToFile(csvString.ToString(), path + fileNames[i])) {
                AssetDatabase.Refresh();
                Debug.Log(fileNames[i] + " file saved in folder " + path);
            }
                
            csvString.Remove(0, csvString.Length);
        }

        filesPending--;
    }

    private void loadFromCSV(string[] fileNames, string path) {

        for (int i = 0; i < fileNames.Length; i++) {
            if (!toggles[i]) continue;
            // Try load a parsed csv file
            CsvParser.LoadFromFile(path + fileNames[i], new CsvParser.ReadLineDelegate(CsvDelegateReader), i, path + fileNames[i]);
            filesPending++;
        }
    }

    private void CsvDelegateReader(CsvParser.CsvLine line, bool lastLine, params object[] resultObjects) {

        // cast the params
        int languageIndex = (int)resultObjects[0];
        string fileName = (string)resultObjects[1];

        // line.cells[0] --> LocalizedTextId
        // line.cells[1] --> Text    

        LocalizedText ltAux = locTextGroup.localizedTextsList.Find(x => x.textID.Equals(line.cells[0]));

        if (ltAux == null) {
            // LocalizedText Id not found, create a new one
            ltAux = AddLocalizedText(line.cells[0]);
        }

        // Never replace with blank text. To ensure doing it the right way, we create 
        // a clone of the text, replace all the spaces, tabs and eol. 
        // Finally check if it is an empty string
        string checkString = line.cells[1].Replace(" ", "").Replace("\n", "").Replace("\t", "");

        if (!checkString.Equals("")) {
            ltAux.localizedTextsList[languageIndex].setText(line.cells[1]);
        }

        // All lines processed, inform the developer
        if (lastLine) {
            Debug.Log((line.line_index + 1) + " localized texts loaded from " + fileName);
            filesPending--;
        }
    }
   
    private void importFromRemoteCsvSheet(int index) {
         EditorWWW editorWWW = new EditorWWW();
         editorWWW.StartWWW(locTextGroup.remoteCsvUrls[index], null, resultWWW, index, locTextGroup.remoteCsvUrls[index]);
    }

    private void resultWWW(string result, object[] resultObjs) {
        CsvParser.LoadFromString(result, new CsvParser.ReadLineDelegate(CsvDelegateReader), resultObjs);
    }
    #endregion
}
