using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AKAGF.GameArchitecture.MonoBehaviours.GUI
{
    [global::System.Serializable]
    public struct UITextElementWrapper {

        public enum TEXT_TYPE { TEXT, TEXT_MESH, TEXT_MESH_PRO, TEXT_MESH_PRO_UGUI }
        public TEXT_TYPE textType;

        public Text uiText;                               // Reference to the Text component that will display the message.
        public TextMesh textMesh;
        public TextMeshProUGUI text_UGUI;
        public TextMeshPro text_PRO;

        public string getText() {

            string text = "";

            switch (textType)  {
                case TEXT_TYPE.TEXT: if (uiText) text = uiText.text;
                    break;
                case TEXT_TYPE.TEXT_MESH: if (textMesh) text = textMesh.text;
                    break;
                case TEXT_TYPE.TEXT_MESH_PRO: if (text_PRO) text = text_UGUI.text;
                    break;
                case TEXT_TYPE.TEXT_MESH_PRO_UGUI: if (text_UGUI) text = text_PRO.text;
                    break;
            }

            if (text.Equals(""))
                Debug.Log("No reference found for " + textType.ToString() + " ui text object.");

            return text;
        }

        public void setText(string text) {
            switch (textType)  {
                case TEXT_TYPE.TEXT: if (uiText) uiText.text = text;
                    break;
                case TEXT_TYPE.TEXT_MESH: if (textMesh) textMesh.text = text;
                    break;
                case TEXT_TYPE.TEXT_MESH_PRO:  if (text_PRO) text_PRO.text = text;
                    break;
                case TEXT_TYPE.TEXT_MESH_PRO_UGUI: if (text_UGUI) text_UGUI.text = text;
                    break;
            }
        }

        public void appendText(string text) {
            switch (textType)  {
                case TEXT_TYPE.TEXT: if (uiText) uiText.text += text;
                    break;
                case TEXT_TYPE.TEXT_MESH: if (textMesh) textMesh.text += text;
                    break;
                case TEXT_TYPE.TEXT_MESH_PRO: if (text_PRO) text_PRO.text += text;
                    break;
                case TEXT_TYPE.TEXT_MESH_PRO_UGUI:  if (text_UGUI) text_UGUI.text += text;
                    break;
            }

        }

        public void setTextColor(Color color) {
            switch (textType)  {
                case TEXT_TYPE.TEXT: if (uiText) uiText.color = color;
                    break;
                case TEXT_TYPE.TEXT_MESH: if (textMesh) textMesh.color = color;
                    break;
                case TEXT_TYPE.TEXT_MESH_PRO: if (text_PRO) text_PRO.color = color;
                    break;
                case TEXT_TYPE.TEXT_MESH_PRO_UGUI: if (text_UGUI) text_UGUI.color = color;
                    break;
            }
        }

        public Color getTextColor() {

            Color color = new Color();

            switch (textType)  {
                case TEXT_TYPE.TEXT: if (uiText) color = uiText.color;
                    break;
                case TEXT_TYPE.TEXT_MESH: if (textMesh) color = textMesh.color;
                    break;
                case TEXT_TYPE.TEXT_MESH_PRO: if (text_PRO) color = text_PRO.color;
                    break;
                case TEXT_TYPE.TEXT_MESH_PRO_UGUI:  if (text_UGUI) color = text_UGUI.color;
                    break;
            }

            return color;
        }

        public void setAlpha(float alpha) {

            switch (textType)  {
                case TEXT_TYPE.TEXT: if (uiText) uiText.canvasRenderer.SetAlpha(alpha);
                    break;
                case TEXT_TYPE.TEXT_MESH:
                    if (textMesh) {
                        Color color = getTextColor();
                        color.a = alpha;
                        setTextColor(color);
                    } 
                    break;
                case TEXT_TYPE.TEXT_MESH_PRO: if (text_PRO) text_PRO.canvasRenderer.SetAlpha(alpha);
                    break;
                case TEXT_TYPE.TEXT_MESH_PRO_UGUI:  if (text_UGUI) text_UGUI.canvasRenderer.SetAlpha(alpha);
                    break;
            }
        }

        public float getAlpha() {

            float alpha = 1f;

            switch (textType)  {
                case TEXT_TYPE.TEXT: if (uiText) alpha = uiText.canvasRenderer.GetAlpha();
                    break;
                case TEXT_TYPE.TEXT_MESH:
                    if (textMesh) {
                        alpha = getTextColor().a;
                    } 
                    break;
                case TEXT_TYPE.TEXT_MESH_PRO: if (text_PRO) alpha = text_PRO.canvasRenderer.GetAlpha();
                    break;
                case TEXT_TYPE.TEXT_MESH_PRO_UGUI:  if (text_UGUI) alpha = text_UGUI.canvasRenderer.GetAlpha();
                    break;
            }

            return alpha;
        }
    }
}
