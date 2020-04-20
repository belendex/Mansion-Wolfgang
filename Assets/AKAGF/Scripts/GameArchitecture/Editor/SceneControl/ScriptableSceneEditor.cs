using UnityEditor;
using UnityEngine;
using AKAeditor;
using AKAGF.GameArchitecture.ScriptableObjects.SceneControl;


[CustomEditor(typeof(ScriptableScene))]
public class ScriptableSceneEditor : Editor{

    public enum EditorType {
        SINGLE_ASSET, ALL_ASSETS
    }

    public EditorType editorType = EditorType.SINGLE_ASSET;
    private ScriptableScene scriptableScene;                   //Reference to target object

    private SerializedProperty scenePathProperty;
    private SerializedProperty sceneStartingPositionsNamesProperty;

    private const string scenePathPropName = "scenePath";
    private const string sceneStartingPositionsNamesPropName = "sceneStartingPositionsNames";

    private const float removeButtonWidth = 25f;

    private void OnEnable() {
        scriptableScene = target as ScriptableScene;

        scenePathProperty = serializedObject.FindProperty(scenePathPropName);
        sceneStartingPositionsNamesProperty = serializedObject.FindProperty(sceneStartingPositionsNamesPropName);
    }

    public override void OnInspectorGUI() {

        // Call different GUI depending where the Scriptable Scene is.
        switch (editorType) {
            case EditorType.SINGLE_ASSET:
                drawSingleAssetEditor();
                break;
            case EditorType.ALL_ASSETS:
                drawAllAssetsEditor();
                break;
            default:
                throw new UnityException("Unknown ScriptableSceneEditor.EditorType.");
        }
    }

    private void editorGUI(bool removeButton) {

        EditorGUILayout.BeginVertical(GUI.skin.box);

            GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);

            scriptableScene.isExpanded = EditorGUILayout.Foldout(scriptableScene.isExpanded, new GUIContent(scriptableScene.name), true, EditorStyles.foldout);

                SceneAsset oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scriptableScene.scenePath);
       
                serializedObject.Update();

                // Display a button showing a '-' that if clicked removes this Condition from the AllConditions asset.
                if (removeButton && EditorTools.createListButton("-", true, GUILayout.Width(removeButtonWidth))) {
                    AllGameScriptableScenesEditor.RemoveScriptableScene(scriptableScene);
                    return;
                }
            
            GUILayout.EndHorizontal();

        if (scriptableScene.isExpanded) {
            EditorGUI.BeginChangeCheck();

            EditorGUI.indentLevel += 1;
            EditorGUILayout.LabelField("Unity Scene");
            EditorGUI.indentLevel -= 1;
            GUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);

            SceneAsset newScene = EditorGUILayout.ObjectField(oldScene, typeof(SceneAsset), false) as SceneAsset;

            GUILayout.Space(5);

            EditorTools.createArrayPropertyButtons(sceneStartingPositionsNamesProperty, sceneStartingPositionsNamesPropName, GUILayout.Width( 30f), true);

            if (sceneStartingPositionsNamesProperty.arraySize > 0) {
                
                for (int i = 0; i < sceneStartingPositionsNamesProperty.arraySize; i++){
                    GUILayout.Space(5);
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.PropertyField(sceneStartingPositionsNamesProperty.GetArrayElementAtIndex(i), new GUIContent("Position Name " + i), true);
                    EditorGUILayout.EndVertical();
                }
            }

            GUILayout.EndVertical();

            if (newScene) {
                scriptableScene.name = newScene.name;
            }

    
            if (EditorGUI.EndChangeCheck() && newScene != null) {
                string newPath = AssetDatabase.GetAssetPath(newScene.GetInstanceID());            
                scenePathProperty.stringValue = newPath;
                AssetDatabase.SaveAssets();
            
            }
        }
            
        EditorGUILayout.EndVertical();

        // Push the values from the serializedObject back to the target.
        serializedObject.ApplyModifiedProperties();
    }


    private void drawSingleAssetEditor() {
        editorGUI(false);
    }


    private void drawAllAssetsEditor() {
        editorGUI(true);
    }


    public static ScriptableScene CreateScriptableScene() {
        // Create the instance of the scriptableObject
        ScriptableScene newScene = CreateInstance<ScriptableScene>();
        newScene.sceneStartingPositionsNames = new string[0];

        return newScene;
    }

}
