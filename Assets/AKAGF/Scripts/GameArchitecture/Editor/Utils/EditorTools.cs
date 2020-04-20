using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AKAeditor {

    public static class EditorTools {

        public static void createArrayPropertyButtons(SerializedProperty array, string label, GUILayoutOption option = null, bool allowEmpty = false, bool onlyAddOption = false, string addLabel = "+", string removeLabel = "-") {

            GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
            EditorGUILayout.LabelField(label + " ", array.arraySize.ToString());

            int minSize = allowEmpty ? 0 : 1;

            if (array.arraySize < minSize)
                array.arraySize = minSize;

            
            //Buttons to add or remove array entries
            if (createListButton(addLabel, false, option)) {
                array.arraySize++;
                //array.GetArrayElementAtIndex(array.arraySize - 1);

            }
                

            if(!onlyAddOption)
                if (createListButton(removeLabel, true, option) && array.arraySize > minSize)
                    array.arraySize--;

            GUILayout.EndHorizontal();
        }


        public static void createSliderProperty(ref SerializedProperty property, float minValue, float maxValue, string label = null) {

            string name = property.name;

            if (label != null)
                name = label;

            property.floatValue = EditorGUILayout.Slider(name, property.floatValue, minValue, maxValue);
        }


        public static void drawMessage(string message, MessageType messageType) {
            GUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.HelpBox(message, messageType);
            GUILayout.EndVertical();
        }


        public static void createTitleBox(string title, bool subTitle = false) {

            if (subTitle)
                EditorColors.SET_SUBTITLE_COLOR();
            else
            EditorColors.SET_MAINTITLE_COLOR();

            GUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            GUILayout.EndVertical();
            EditorColors.SET_DEFAULT_COLOR();
        }

        public static void createHorizontalSeparator(float space = 2.5f) {
            GUILayout.Space(space);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Space(space);
        }

        public static bool createListButton(string title, bool removeButton = false, GUILayoutOption option = null) {

            if (removeButton)
                EditorColors.SET_REMOVE_BUTTON_COLOR();
            else
                EditorColors.SET_ADD_BUTTON_COLOR();

            if (option == null)
                option = GUILayout.ExpandWidth(false);

            bool pressed = GUILayout.Button(title, option);
            EditorColors.SET_DEFAULT_COLOR();

            return pressed;
        }

        public static Texture getIcon(string iconName) {
            return EditorGUIUtility.IconContent(iconName).image;
        }


        public static void CalcGUIContentSize(GUIContent content, GUIStyle style, out float width, out float height) {
            float minWidth;
            float maxWidth;

            //GetPadding();

            style.CalcMinMaxWidth(content, out minWidth, out maxWidth);

            float threshold = 250;

            if (maxWidth < threshold)
            {
                style.wordWrap = false;
                Vector2 size = style.CalcSize(content);
                style.wordWrap = true;
                maxWidth = size.x;
            }

            width = Mathf.Clamp(maxWidth, 0, threshold);
            height = Mathf.Clamp(style.CalcHeight(content, width), 21, 150);
            //Debug.LogWarning(string.Format("min: {0}, max: {1} => w: {2}, isHeightDependentonwidht: {3}", minWidth, maxWidth, width, style));

            //SetPadding(l, t, r, b);
        }


        public static void createPopUpMenuWithObjectsNames<T>(ref T unityObject, ref int selectedIndex, string popupLabel, string[] overrideNames = null) where T : ScriptableObject {

            List<T> instances = new List<T>(ScriptableObjectUtility.GetAllInstances<T>());

            if (unityObject)
                selectedIndex = instances.IndexOf(unityObject);

            string[] names = overrideNames == null ? ScriptableObjectUtility.getAllInstancesNames<T>() : overrideNames;

            if (names.Length != instances.Count) {
                Debug.Log("The amount of names don't match the object instances count " + unityObject);
                return;
            }

            if (instances.Count == 0) {
                EditorGUILayout.HelpBox("No Scriptable Objects of this type found on project!", MessageType.Warning);
                return;
            }
            else {
                selectedIndex = EditorGUILayout.Popup(popupLabel, selectedIndex, names);

                selectedIndex = (selectedIndex >= 0 && selectedIndex <= instances.Count - 1) ? selectedIndex : 0;
                unityObject = instances[selectedIndex];
            }
 
        }

        public static void createPopUpMenuWithObjectsNames<T>(ref T unityObject, ref int selectedIndex, string popupLabel, Rect position, string[] overrideNames = null) where T : ScriptableObject {

            List<T> instances = new List<T>(ScriptableObjectUtility.GetAllInstances<T>());

            if (unityObject)
                selectedIndex = instances.IndexOf(unityObject);

            string[] names = overrideNames == null ? ScriptableObjectUtility.getAllInstancesNames<T>() : overrideNames;

            if (names.Length != instances.Count) {
                Debug.Log("The amount of names don't match the object instances count " + unityObject);
                return;
            }

            if (instances.Count == 0) {
                EditorGUI.HelpBox(position, "No Scriptable Objects of this type found on project!", MessageType.Warning);
            }
            else {
                selectedIndex = EditorGUI.Popup(position, popupLabel, selectedIndex, names);

                selectedIndex = (selectedIndex >= 0 && selectedIndex <= instances.Count - 1) ? selectedIndex : 0;
                unityObject = instances[selectedIndex];
            }

        }
    }
}