using System;
using System.Collections.Generic;
using AKAGF.GameArchitecture.ScriptableObjects.Abstracts;
using UnityEngine;

namespace AKAGF.GameArchitecture.ScriptableObjects.Localization
{
    [Serializable]
    public struct LocalizedTextData {
        public string id;
        public LocalizedText text;
    }

    public class AllGameLanguages : ScriptableSingleton<AllGameLanguages> {

       
        public List<LocalizedTextData> allLocalizedTexts = new List<LocalizedTextData>();
        public List<GameLanguageItem> gameLanguagesList = new List<GameLanguageItem>();
        private int lastDefaultIndex;
        private int selectedGameLanguageIndex;

        public bool useGlobalDictionary;
        


        public GameLanguageItem getDefaultLanguage() {

            for (int i = 0; i < Instance.gameLanguagesList.Count; i++) {
                if (Instance.gameLanguagesList[i].isDefault) {
                    return Instance.gameLanguagesList[i];
                }
            }

            return null;
        }

        public void setDefaultLanguage(int index) {

            if (Instance.gameLanguagesList.Count == 0)
                return;
            
            for (int i = 0; i < Instance.gameLanguagesList.Count; i++) {
                Instance.gameLanguagesList[i].isDefault = false;
            }

            index = Mathf.Clamp(index, 0, Instance.gameLanguagesList.Count - 1);

            Instance.gameLanguagesList[index].isDefault = true;
            lastDefaultIndex = index;
        }

        public void checkDefaultLanguage() {
            if (getDefaultLanguage() == null) {

                lastDefaultIndex = Mathf.Clamp(lastDefaultIndex, 0, Instance.gameLanguagesList.Count - 1);
                setDefaultLanguage(lastDefaultIndex);
            }
        }

        public string[] getLanguagesCodes() {

            string[] languageCodes = new string[Instance.gameLanguagesList.Count];

            for (int i = 0; i < Instance.gameLanguagesList.Count; i++) {
                languageCodes[i] = Instance.gameLanguagesList[i].gameLanguage.code;
            }

            return languageCodes;
        }

        public GameLanguageItem getCurrentLanguage() {

            if (Instance.gameLanguagesList.Count == 0)
                return null;

            return gameLanguagesList[selectedGameLanguageIndex];
        }

        public void setCurrentLanguage(int index) {

            if (Instance.gameLanguagesList.Count == 0)
                return;

            index = Mathf.Clamp(index, 0, Instance.gameLanguagesList.Count - 1);

            selectedGameLanguageIndex = index;
        }

        public void setCurrentLanguage(string code) {

            if (Instance.gameLanguagesList.Count == 0)
                return;

            int langIndex = gameLanguagesList.FindIndex(x => x.gameLanguage.code.Equals(code));

            if (langIndex != -1)
                selectedGameLanguageIndex = langIndex;
            else
                Debug.Log("No language found in All Game Languages with code: " + code);

        }

        public int getCurrentLanguageIndex() {

            return selectedGameLanguageIndex;
        }
    }

    [Serializable]
    public struct GameLanguage {
        public string code;
        public string name;
        public string nativeName;
    }


    [Serializable]
    public class GameLanguageItem {
        public GameLanguage gameLanguage;
        public Sprite flag;
        public bool isDefault;

        public GameLanguageItem(GameLanguage newGameLanguage) {
            gameLanguage = newGameLanguage;
        }

        public override string ToString() {
            return gameLanguage.code + " - " +
                   gameLanguage.name + " - " +
                   gameLanguage.nativeName;
        }
    }
}