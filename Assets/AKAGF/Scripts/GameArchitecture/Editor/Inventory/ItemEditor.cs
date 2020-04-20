using AKAGF.GameArchitecture.ScriptableObjects.Inventory;
using UnityEngine;
using UnityEditor;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Conditions;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor {

    private Item targetItem;
    
    private const float messageGUILines = 3f;                           // How many lines tall the GUI for the message field should be.

    private void OnEnable() {
        targetItem = (Item)target;
    }

    public override void OnInspectorGUI() {

        // Update the state of the serializedObject to the current values of the target.
        serializedObject.Update();

        EditorGUILayout.BeginVertical(GUI.skin.box);

        GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
        //EditorGUILayout.LabelField(targetItem.name);

        // Handle foldut through isExpanded variable
        targetItem.isExpanded = EditorGUILayout.Foldout(targetItem.isExpanded, new GUIContent(targetItem.name), true, EditorStyles.foldout);

        if (GUILayout.Button("Remove Item", GUILayout.ExpandWidth(false))) {
            // Record all operations so they can be undone.
            Undo.RecordObject(targetItem, "Remove Item");

            // Remove the specified item from the array.
            ArrayUtility.Remove(ref targetItem.inventoryItemList.itemList, targetItem);

            // Destroy the item, including it's asset and save the assets to recognise the change.
            DestroyImmediate(targetItem, true);
            AssetDatabase.SaveAssets();

            return;
        }

        GUILayout.EndHorizontal();

        if (targetItem.isExpanded) {
            GUILayout.Space(5);
            /* Item Properties */
            //Project Name
            GUILayout.BeginHorizontal();
            targetItem.name = EditorGUILayout.TextField("Project Item Name", targetItem.name);
            if (GUILayout.Button("Rename Item", GUILayout.ExpandWidth(false))) {
                AssetDatabase.SaveAssets();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            //Sprite
            targetItem.sprite = EditorGUILayout.ObjectField("Item Sprite", targetItem.sprite, typeof(Sprite), false) as Sprite;
            GUILayout.Space(5);
            //Game name
            targetItem.itemName = EditorGUILayout.TextField("Game Item Name", targetItem.itemName as string);
            //Short Description
            targetItem.itemShortDescription = EditorGUILayout.TextField("Item Short Description", targetItem.itemShortDescription as string);
            //Long Description
            EditorGUILayout.LabelField("Item Long Description");
            targetItem.itemLongDescription = EditorGUILayout.TextArea(targetItem.itemLongDescription, EditorStyles.textArea, GUILayout.Height(EditorGUIUtility.singleLineHeight * messageGUILines));
            GUILayout.Space(5);

            targetItem.equippedCondition = EditorGUILayout.ObjectField("Equiped Condition", targetItem.equippedCondition, typeof(Condition), false) as Condition;

            GUILayout.Space(5);
            //is Unique
            targetItem.isUnique = EditorGUILayout.Toggle("Unique", targetItem.isUnique, GUILayout.ExpandWidth(false));

            // If an item is unique, it can't be stackable
            if (!targetItem.isUnique) {

                targetItem.isStackable = EditorGUILayout.Toggle("Stackable ", targetItem.isStackable, GUILayout.ExpandWidth(false));

                if (targetItem.isStackable) {
                    targetItem.removeIfEmpty = EditorGUILayout.Toggle("Remove if Empty ", targetItem.removeIfEmpty, GUILayout.ExpandWidth(false));
                    targetItem.maxQuantity = EditorGUILayout.FloatField("Max Quantity", targetItem.maxQuantity, GUILayout.ExpandWidth(false));
                    targetItem.currentQuantity = EditorGUILayout.FloatField("Current Quantity", targetItem.currentQuantity, GUILayout.ExpandWidth(false));
                    
                }
            }
            else {
                targetItem.isStackable = false;
            }

            GUILayout.Space(5);
        }
        EditorGUILayout.EndVertical();

        // Push data back from the serializedObject to the target.
        serializedObject.ApplyModifiedProperties();
    }
}
