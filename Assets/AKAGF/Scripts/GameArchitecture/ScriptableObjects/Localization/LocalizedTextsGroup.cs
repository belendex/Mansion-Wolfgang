using System;
using System.Collections.Generic;
using UnityEngine;

namespace AKAGF.GameArchitecture.ScriptableObjects.Localization
{
    [Serializable]
    public class LocalizedTextsGroup : ScriptableObject {

        /// <summary>
        /// The name of the Localized Text Group. The asset name is modified 
        /// at edition time to always match the value of groupName field.
        /// </summary>
        public string groupName;

        /// <summary>
        /// Array that contains all the LocalizedTexts 
        /// belonging to this LocalizetTextGroup
        /// </summary>
        public List<LocalizedText> localizedTextsList;

        /// <summary>
        /// Local list of added game languages. Used to avoid dependency 
        /// with the master controller AllGameLanguages, which actually defines
        /// the languages of the system
        /// </summary>
        public GameLanguageItem[] localGameLanguagesList;

        /// <summary>
        /// Array of urls from an Online source like Google SpreadSheet or AWS s3. 
        /// Only import options are allowed. 
        /// One url per language and group.
        /// </summary>
        public string[] remoteCsvUrls = new string[0];


#if UNITY_EDITOR
        /// <summary>
        /// - Editor Only field - 
        /// Field for developers to write a description of the LocalizedText group.
        /// </summary>
        public string groupDescription;

        /// <summary>
        /// - Editor Only field - 
        /// Integer to keep the value of the selected 
        /// import/export option in the editor.
        /// </summary>
        public int currentSyncOption;

#endif
    }
}
