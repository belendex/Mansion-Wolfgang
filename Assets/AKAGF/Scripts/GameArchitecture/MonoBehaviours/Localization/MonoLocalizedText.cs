using AKAGF.GameArchitecture.ScriptableObjects.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AKAGF.GameArchitecture.MonoBehaviours.Localization
{
    [ExecuteInEditMode]
    public class MonoLocalizedText : MonoBehaviour {

        public enum TEXT_TYPE { UNITY_TEXT, TEXTMESHPRO_TEXT, TEXTMESHPRO_TEXT_UGUI }
        public TEXT_TYPE targetTextType;
        public bool AlwaysUpdate;
        public LocalizedText localizedText;

        private Text text_u;
        private TextMeshProUGUI text_UGUI;
        private TextMeshPro text_PRO;

        private void OnValidate() {
            getReferences();
        }

        private void OnEnable() {
            setText();
        }

        private void Update() {
            if (AlwaysUpdate) {
                setText();
            }
        }

        private void getReferences() {

            switch (targetTextType) {
                case TEXT_TYPE.UNITY_TEXT:
                    if(!(text_u = GetComponent<Text>()))
                        Debug.LogError("No Unity Text component attached to this gameobject.");
                    break;

                case TEXT_TYPE.TEXTMESHPRO_TEXT:
                    if (!(text_PRO = GetComponent<TextMeshPro>()))
                        Debug.LogError("No Text Mesh Pro component attached to this gameobject.");
                    break;

                case TEXT_TYPE.TEXTMESHPRO_TEXT_UGUI:
                    if (!(text_UGUI = GetComponent<TextMeshProUGUI>()))
                        Debug.LogError("No Text Mesh Pro UGUI component attached to this gameobject.");
                    break;

            }
        }

        private void setText() {

            if (!localizedText)
                return;

            switch (targetTextType) {
                case TEXT_TYPE.UNITY_TEXT:
                    if(text_u != null) text_u.text = localizedText.getLocalizedText();
                    break;

                case TEXT_TYPE.TEXTMESHPRO_TEXT:
                    if (text_PRO != null) text_PRO.text = localizedText.getLocalizedText();
                    break;

                case TEXT_TYPE.TEXTMESHPRO_TEXT_UGUI:
                    if (text_UGUI != null) text_UGUI.text = localizedText.getLocalizedText();
                    break;
            }

        }
    }
}
