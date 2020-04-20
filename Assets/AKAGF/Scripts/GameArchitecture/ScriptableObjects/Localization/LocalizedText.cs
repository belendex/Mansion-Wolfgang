using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AKAGF.GameArchitecture.ScriptableObjects.Localization
{
    [Serializable]
    public class LocalizedText : ScriptableObject {
        /// <summary>
        /// The ID of this LocalizedText. Must be unique within the LocalizedText group belong it.
        /// </summary>
        public string textID;

        /// <summary>
        /// List that contains one text per each game language present in AllGameLanguages Instance.
        /// </summary>
        public List<LocalizableElement> localizedTextsList;

        /// <summary>
        /// List with all the Scriptables Objects that contain the injectable strings. 
        /// The list accepts any Scriptable Object that overrides its ToString() method to fits 
        /// the needs of the localization system. For example, another LocalizedText object.
        /// </summary>
        public List<InjectableElement> injectableStrings;

        /// <summary>
        /// Localized Text group this LocalizedText belongs to.
        /// </summary>
        public LocalizedTextsGroup textsGroup;

        /// <summary>
        /// Static variable used in a little hack for allowing the use of the parameter in the 
        /// native override ToString() method. As all the LozalizedText objects share this variable,
        /// it makes sense to show all the Localized text in the same language, so all the 
        /// LocalizedText will be shown either in the default or the currentLenguage, 
        /// depending on the first call to getLocalizedText() method of any LocalizedText 
        /// present in the open scene.
        /// </summary>
        private static bool showDefault = false;

        /// <summary>
        /// Reference to a stringBuilder instance for performance porpouses.
        /// </summary>
        private StringBuilder stringBuilder = new StringBuilder();

#if UNITY_EDITOR
        public bool isExpandedGlobal = false;
        public bool[] isExpandedLocalText = new bool[0];
        public bool isExpandedInyectableStrings;
#endif

        /// <summary>
        /// Method to get the localized text of this instance either 
        /// in default or current selected game language.
        /// </summary>
        /// <param name="defaultLanguage"> 
        /// Boolean that indicates if the localizedText should be fetched in the current Language 
        /// selected in AllGameLanguages, or the default language. By default, Localized Text 
        /// will be returned in the current Game Language defined in AllGameLanguages Object.
        /// <returns>
        /// Returns the localized string in default or currently selected game language.
        /// </returns>
        public string getLocalizedText(bool defaultLanguage = false) {

            // cache this variable just in case that ToString method needs it
            showDefault = defaultLanguage;

            // Get the code of the language to display
            string code = showDefault
                ? AllGameLanguages.Instance.getDefaultLanguage().gameLanguage.code 
                : AllGameLanguages.Instance.getCurrentLanguage().gameLanguage.code;

            // Get the text based on language code
            string text = localizedTextsList.Find(x => x.languageInfo.code == code).Text;

            // Create a string builder for parsing inyectable strings
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append(text);

            // Replacing the TAGS with the strings returned by scriptableObjects's overrided ToString Method.
            for (int i = 0; i < injectableStrings.Count; i++) {
                if (injectableStrings[i].injectableReference != null)
                    stringBuilder = stringBuilder.Replace(injectableStrings[i].replacingTAG,
                        injectableStrings[i].injectableReference.ToString());
                else
                    Debug.LogError("No reference found for injectable string " + injectableStrings[i].replacingTAG + " in LocalizableText: " + this.textID);

            }
                

            return stringBuilder.ToString();                
        }

        /// <summary>
        /// Overrided ToString() method that allows using a LocalizedText
        /// as an Injectable String of another Localized Text.
        /// </summary>
        /// <returns>
        /// The localized string in the language currently 
        /// defined by showDefault Static varible.
        /// </returns>
        public override string ToString() {
            return getLocalizedText(showDefault);
        }
    }


    [Serializable]
    public class LocalizableElement {
        public GameLanguage languageInfo;
        [SerializeField]
        private string text;
        public string Text { get { return text;} }

        public LocalizableElement(GameLanguage languageCode, string text) {
            this.languageInfo = languageCode;
            this.text = text;
        }

#if UNITY_EDITOR
        public void setText(string value) {
            text = value;
        }

        public string getText() {
            return text;
        }
#endif
    }


    [Serializable]
    public class InjectableElement {
        public ScriptableObject injectableReference;
        public string replacingTAG;
    }
}