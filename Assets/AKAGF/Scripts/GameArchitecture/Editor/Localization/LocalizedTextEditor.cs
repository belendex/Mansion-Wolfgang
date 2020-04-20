using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using AKAeditor;
using AKAGF.GameArchitecture.ScriptableObjects.Localization;

[CustomEditor(typeof(LocalizedText))]
[InitializeOnLoad]
public class LocalizedTextEditor : Editor {

    public enum EditorType {
        SINGLE_ASSET, ALL_ASSETS, INSPECTOR
    }

    public EditorType editorType = EditorType.SINGLE_ASSET ;

    private LocalizedText localizedText ;
    
    
    string tmpId;

    private const string INJECTABLE_STRING_FORETAG = "[<*";
    private const string INJECTABLE_STRING_BACKTAG = "*>]";

    public void OnEnable() {
        // Cache the target variable
        localizedText = target as LocalizedText;

        // Inizialize both, the list of localized texts and the array of 
        // injectables Strigns in case they were null
        if (localizedText.localizedTextsList == null)
            localizedText.localizedTextsList = new List<LocalizableElement>();

        if (localizedText.injectableStrings == null)
            localizedText.injectableStrings = new List<InjectableElement>();

        // Keep the array of texts elements and expandedFoldouts fixed to the same size
        if (localizedText.isExpandedLocalText.Length != localizedText.localizedTextsList.Count)
            Array.Resize(ref localizedText.isExpandedLocalText, localizedText.localizedTextsList.Count);

        // In case AllGameLanguages has a valid Instance...
        if (AllGameLanguages.Instance == null) return;
            
        // ...check that All GameLanguages and texts List have the same length, if not,
        // a language was either added to or removed from global game Languages.
        if (AllGameLanguages.Instance.gameLanguagesList.Count != localizedText.localizedTextsList.Count) {

            // Add all the languages present in AllGameLanguages
            for (int i = 0; i < AllGameLanguages.Instance.gameLanguagesList.Count; i++) {
                if (!localizedText.localizedTextsList.Exists(x => x.languageInfo.code == AllGameLanguages.Instance.gameLanguagesList[i].gameLanguage.code)) {
                    localizedText.localizedTextsList.Add(new LocalizableElement(AllGameLanguages.Instance.gameLanguagesList[i].gameLanguage, ""));
                }
            }

            // Remove the languages not present in AllGameLanguages
            for (int i = 0; i < localizedText.localizedTextsList.Count; i++) {
                if (!AllGameLanguages.Instance.gameLanguagesList.Exists( x => x.gameLanguage.code == localizedText.localizedTextsList[i].languageInfo.code)) {
                    localizedText.localizedTextsList.RemoveAt(i);
                }
            }
        }

        // Keep the array of texts elements and expandedFoldouts fixed to the same size
        if (localizedText.isExpandedLocalText.Length != localizedText.localizedTextsList.Count)
            Array.Resize(ref localizedText.isExpandedLocalText, localizedText.localizedTextsList.Count);

        tmpId = localizedText.textID;
    }

    public override void OnInspectorGUI() {

        if (editorType == EditorType.SINGLE_ASSET)
            localizedText.isExpandedGlobal = true;

        // Update the state of the serializedObject to the current values of the target.
        serializedObject.Update();

        // Global GUI Box
        GUILayout.BeginVertical(GUI.skin.box);

        EditorColors.SET_SUBTITLE2_COLOR();
        GUILayout.BeginHorizontal(GUI.skin.box);
        GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);

            GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
            myFoldoutStyle.fontStyle = FontStyle.Bold;
            myFoldoutStyle.fontSize = 11;
            myFoldoutStyle.fixedHeight = 16f;
            myFoldoutStyle.stretchWidth = true;

            // Global Foldout
            localizedText.isExpandedGlobal = EditorGUILayout.Foldout(localizedText.isExpandedGlobal, new GUIContent(localizedText.name), true, myFoldoutStyle);
            
            // Show remove button only in case that this LocalizedText belongs to one LocalizedTextsGroup
            if (editorType == EditorType.ALL_ASSETS) {
                // Display a button showing a '-' that if clicked removes this item from the group.
                if (EditorTools.createListButton(" - ", true)) {

                    removeLocalizedText();
                    return;
                }
            }

        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();
        EditorColors.SET_DEFAULT_COLOR();

        if (localizedText.isExpandedGlobal)
            expandedGUI();

        GUILayout.EndVertical();
        
        // Push data back from the serializedObject to the target.
        serializedObject.ApplyModifiedProperties();
    }

    public void removeLocalizedText(bool saveAssets = true) {
        // Record all operations so they can be undone.
        // Undo.RecordObject(localizedText, "Remove Localized Text");

        // Remove the specified LocalizedText from the LocalizedTextsGroup.
        localizedText.textsGroup.localizedTextsList.Remove(localizedText);

        // Destroy the instance and the asset of the removed Localized Text
        DestroyImmediate(localizedText, true);
        if(saveAssets)
            AssetDatabase.SaveAssets();
    }
    

    private void expandedGUI() {
        // LocalizedText ID/name Textfield
            GUILayout.BeginHorizontal();
                tmpId = EditorGUILayout.TextField(new GUIContent("Localized Text ID"), tmpId);
            GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
            // LocalizedText ID/name Rename button
            if (GUILayout.Button("Rename", GUILayout.ExpandWidth(true))) {
                if(!localizedText.textsGroup.localizedTextsList.Exists(x => x.textID.Equals(tmpId))) {
                    
                    Debug.Log("LocalizedText: " + localizedText.textID + " renamed to: " + tmpId + ".");

                    localizedText.textID = tmpId;
                    localizedText.name = localizedText.textID;
                    AssetDatabase.SaveAssets();

                } else {
                    Debug.LogWarning("There is already a LocalizedText with ID: " + tmpId + ". Try another ID for the LocalizedText.");
                }
            }
            GUILayout.Space(5);
        GUILayout.EndHorizontal();
            // Injectable Strings
            createInjectableStringListGUIBOX();

            GUILayout.Space(5);

            // LocalizedText text area for every 
            // language present in AllGameLanguages
            createLocalizedTextsListGUIBOXES();

            GUILayout.Space(5);

            // Test/Debug options
            if(editorType != EditorType.INSPECTOR)
                createTestButtons();
    }

    private void createInjectableStringListGUIBOX() {

        // Style for the foldout title
        GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        myFoldoutStyle.fontStyle = FontStyle.Bold;

        // Injectable Strings List GUI Box
        GUILayout.BeginVertical(GUI.skin.box);

            // Injectable Strings List foldout
            GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
            localizedText.isExpandedInyectableStrings = EditorGUILayout.Foldout(localizedText.isExpandedInyectableStrings, new GUIContent("Injectable Strings"), true, myFoldoutStyle);
            GUILayout.EndHorizontal();
            

            if (localizedText.isExpandedInyectableStrings) {
                // Array add/remove options
                EditorTools.createArrayPropertyButtons(serializedObject.FindProperty("injectableStrings"), "Total Objects:", GUILayout.Width( 40f), true, true);

                checkAndRepairTags();

                for (int i = 0; i < localizedText.injectableStrings.Count; i++) {

                    // Create a GUI box for each element
                    GUILayout.BeginVertical(GUI.skin.box);
                        EditorGUI.indentLevel--;
                        GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
                
                        // Injectable string replacing TAG
                        EditorGUILayout.SelectableLabel(localizedText.injectableStrings[i].replacingTAG, GUILayout.Height(16), GUILayout.Width(60));
                        localizedText.injectableStrings[i].injectableReference = EditorGUILayout.ObjectField(localizedText.injectableStrings[i].injectableReference, typeof( ScriptableObject), false) as ScriptableObject;

                        addInjectableTagToLocalizedTexts(localizedText.injectableStrings[i].replacingTAG);
                
                        // Display a button for removing each element indepently
                        if (EditorTools.createListButton(" - ", true)) {
                            removeInjectableTagFromLocalizedTexts(localizedText.injectableStrings[i].replacingTAG);
                            localizedText.injectableStrings.RemoveAt(i);
                            EditorUtility.SetDirty(localizedText);
                            return;
                        }
                
                        GUILayout.EndHorizontal();
                        EditorGUI.indentLevel++;
                    GUILayout.EndVertical();
                }

            }

        GUILayout.EndVertical();    
    }

    private void checkAndRepairTags() {

        for (int i = localizedText.injectableStrings.Count - 1; i >= 0  ; i--) {
            if (localizedText.injectableStrings[i].replacingTAG == "" || 
                localizedText.injectableStrings.FindAll(x => x.replacingTAG.Equals(localizedText.injectableStrings[i].replacingTAG)).Count > 1) {

                string auxTag;

                for (int j = 1;  ; j++) {
                    auxTag = INJECTABLE_STRING_FORETAG + j + INJECTABLE_STRING_BACKTAG;
                    if (localizedText.injectableStrings.Find(x => x.replacingTAG.Equals(auxTag)) == null) {
                        localizedText.injectableStrings[i].replacingTAG = auxTag;
                        break;
                    }
                }
            }
        }
    }

    private void addInjectableTagToLocalizedTexts(string tag) {

        for (int i = 0; i < localizedText.localizedTextsList.Count; i++) {
            if (!localizedText.localizedTextsList[i].getText().Contains(tag)) {
                localizedText.localizedTextsList[i].setText(localizedText.localizedTextsList[i].getText() + tag);
            }
        }
    }

    private void removeInjectableTagFromLocalizedTexts(string tag) {
        for (int i = 0; i < localizedText.localizedTextsList.Count; i++) {
            if (localizedText.localizedTextsList[i].getText().Contains(tag)) {
                localizedText.localizedTextsList[i].setText(localizedText.localizedTextsList[i].getText().Replace(tag, ""));
            }
        }
    }

    private void createLocalizedTextsListGUIBOXES() {

        EditorTools.createTitleBox("Localized Texts", true);

            GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.miniButton);
            string defaultText = "";
            myFoldoutStyle.fixedWidth = 35f;

            for (int i = 0; i < localizedText.localizedTextsList.Count; i++) {
                
                //AllGameLanguagesEditor.setFoldoutDefaultColor(i, ref myFoldoutStyle, ref defaultText);

                GUILayout.BeginVertical(GUI.skin.box);

                GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);

                string LanguageKey = localizedText.localizedTextsList[i].languageInfo.code;

            // Handle foldut through isExpanded variable
            localizedText.isExpandedLocalText[i] = EditorGUILayout.Foldout(localizedText.isExpandedLocalText[i], new GUIContent(LanguageKey + defaultText), true, myFoldoutStyle);

                if (localizedText.localizedTextsList[i].getText() == "") {
                    EditorGUILayout.LabelField(new GUIContent("Empty Text", EditorGUIUtility.IconContent("console.warnicon.sml").image), EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                }
                else {
                    EditorGUILayout.LabelField(new GUIContent("OK", EditorGUIUtility.IconContent("Collab").image), EditorStyles.boldLabel, GUILayout.ExpandWidth(false));

                }

                GUILayout.EndHorizontal();

                EditorColors.SET_DEFAULT_COLOR();


                if (localizedText.isExpandedLocalText[i]) {

                    string lText = localizedText.localizedTextsList[i].getText();

                    float minWidth, maxWidth;

                    GUI.skin.label.CalcMinMaxWidth(new GUIContent(lText), out minWidth, out maxWidth);
                    float currentViewWidth = EditorGUIUtility.currentViewWidth;

                    int numLines = (int)Math.Round(maxWidth / currentViewWidth + 1) + lText.Split('\n').Length -1;
                    lText = EditorGUILayout.TextArea(lText, EditorStyles.textArea, GUILayout.Height(EditorGUIUtility.singleLineHeight * numLines));

                    localizedText.localizedTextsList[i].setText(lText);

                    GUILayout.Space(2.5f);
 
                }

                GUILayout.EndVertical();

                if (i != localizedText.localizedTextsList.Count - 1 &&
                                editorType == EditorType.SINGLE_ASSET)
                    EditorTools.createHorizontalSeparator();
            }
        
    }

    private void createTestButtons() {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Test Current Language", GUILayout.ExpandWidth(true)))
            Debug.Log(localizedText.getLocalizedText());
        
        if (GUILayout.Button("Test Default Language", GUILayout.ExpandWidth(true)))
            Debug.Log(localizedText.getLocalizedText(true));

        GUILayout.EndHorizontal();

        GUILayout.Space(2.5f);
    }


    //private const string BASE_PATH = AKAGF_PATHS.AKAGF_BASE_PATH +
    //                                 AKAGF_PATHS.DATA_PATH +
    //                                 AKAGF_PATHS.LOCALIZATION_PATH +
    //                                 AKAGF_PATHS.RESOURCES_LOCALIZATION_SINGLE_PATH;

    //[MenuItem("Assets/Create/AKAF Assets/ Single Localized Text")]
    //public static LocalizedText CreateSingleLocalizedText() {

    //    // Check if there is a valid AllGameLanguages Instance currently running
    //    if (!AllGameLanguages.Instance){
    //        Debug.LogError("AllGameLanguages has not been created yet.");
    //        return null;
    //    }

    //    // Create the directory if doesn't exist yet.
    //    FileManager.createDirectory(BASE_PATH) ;

    //    if (AssetDatabase.LoadAssetAtPath<LocalizedText>(BASE_PATH + AKAGF_PATHS.LOCALIZED_SINGLE_TEXT_NAME) == null) {

    //        LocalizedText asset = ScriptableObject.CreateInstance<LocalizedText>() ;

    //        Undo.RecordObject(asset as ScriptableObject, "Single Localized Text created");

    //        asset.textID = AKAGF_PATHS.LOCALIZED_SINGLE_TEXT_NAME;
    //        asset.name = asset.textID;
    //        asset.localizedTextsList  = new List<LocalizableElement>();

    //        // Add all the languages present in AllGameLanguages
    //        for (int i = 0; i < AllGameLanguages.Instance.gameLanguagesList.Count; i++) {
    //            asset.localizedTextsList.Add(new LocalizableElement(AllGameLanguages.Instance.gameLanguagesList[i].gameLanguage, ""));
    //        }

    //        // Create and save the asset in resources folder
    //        AssetDatabase.CreateAsset(asset, BASE_PATH + AKAGF_PATHS.LOCALIZED_SINGLE_TEXT_NAME) ;
    //        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset as ScriptableObject));
    //        AssetDatabase.SaveAssets();
    //        EditorUtility.SetDirty(asset);

    //        return asset;
    //    }

    //    Debug.Log("There is already a SingleLocalizedTexts.asset, change the name first, then create another one.");
    //    return null;
    //}

}
