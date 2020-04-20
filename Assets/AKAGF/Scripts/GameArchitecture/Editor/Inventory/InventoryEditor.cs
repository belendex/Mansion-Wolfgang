using AKAGF.GameArchitecture.MonoBehaviours.Inventory;
using UnityEngine;
using UnityEditor;
using AKAeditor;
using AKAGF.GameArchitecture.ScriptableObjects.Inventory;

[CustomEditor(typeof(Inventory))]
public class InventoryEditor : Editor {

    private Inventory targetInventory;

    //private bool[] showItemSlots;                                       // Whether the GUI for each Item slot is expanded.
    //private SerializedProperty itemImagesProperty;                    // Represents the array of Image components to display the Items.
    private SerializedProperty itemsProperty;                           // Represents the array of Items.
    private SerializedProperty numItemSlotsProperty;
    private SerializedProperty fixedSizeProperty;
    private SerializedProperty itemListProperty;

    //private const string inventoryPropItemImagesName = "itemImages";  // The name of the field that is an array of Image components.
    private const string inventoryPropItemsName = "items";              // The name of the field that is an array of Items.
    private const string numItemSlotsPropName = "numItemSlots";
    private const string fixedSizePropName = "fixedSize";
    private const string itemListPropName = "inventoryItemList";


    private void OnEnable() {

        targetInventory = (Inventory)target;

        //Cache the SerializedProperties.
        //itemImagesProperty = serializedObject.FindProperty(inventoryPropItemImagesName);
        itemsProperty = serializedObject.FindProperty(inventoryPropItemsName);
        numItemSlotsProperty = serializedObject.FindProperty(numItemSlotsPropName);
        fixedSizeProperty = serializedObject.FindProperty(fixedSizePropName);
        itemListProperty = serializedObject.FindProperty(itemListPropName);
        // resetShowItemSlots();
    }

    //private void resetShowItemSlots() {
    //    if (fixedSizeProperty.boolValue) {
    //        showItemSlots = new bool[numItemSlotsProperty.intValue];
    //    }
    //    else {
    //        showItemSlots = new bool[targetInventory.items.Count];
    //    }
    //}

    private int inventoryItemListSelectedIndex;

    public override void OnInspectorGUI() {

        //Pull all the information from the target into the serializedObject.
        serializedObject.Update();

        //ScriptableObject List that contain the items which could be carried in this inventory
        EditorTools.createPopUpMenuWithObjectsNames<InventoryItemList>(ref targetInventory.inventoryItemList, ref inventoryItemListSelectedIndex, "Inventory Item List");
        //EditorGUILayout.PropertyField(itemListProperty);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("equippedCondition"), true);

        //Boolean value to know if the inventory is dinamic or static (fixed size)
        EditorGUILayout.PropertyField(fixedSizeProperty);

        int currentItemsNumber = 0;

        //Fixed size inventory
        if (fixedSizeProperty.boolValue) {
            currentItemsNumber = numItemSlotsProperty.intValue;
            targetInventory.items.Capacity = currentItemsNumber;
            EditorGUILayout.PropertyField(numItemSlotsProperty);
        }
        //Dinamic size inventory
        else {
            currentItemsNumber = targetInventory.items.Count;
        }

        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnAddItem"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnRemoveItem"), true);

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.box);

        //resetShowItemSlots();

        //Display GUI for each Item slot.
        for (int i = 0; i < currentItemsNumber; i++) {
            ItemSlotGUI(i);
        }

        if (currentItemsNumber == 0) {
            EditorGUILayout.LabelField("Inventory is empty");
        }

        

        EditorGUILayout.EndVertical();
        //Push all the information from the serializedObject back into the target.
        serializedObject.ApplyModifiedProperties();
    }



    private void ItemSlotGUI(int index) {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        EditorGUILayout.LabelField("Item slot " + index, EditorStyles.boldLabel);

        if (targetInventory.items.Count > index) {
            EditorGUILayout.PropertyField(itemsProperty.GetArrayElementAtIndex(index));
        }
        else {
            EditorGUILayout.LabelField("Empty slot");
        }

        //// Display a foldout to determine whether the GUI should be shown or not.
        //showItemSlots[index] = EditorGUILayout.Foldout(showItemSlots[index], "Item slot " + index, true);

        //// If the foldout is open then display default GUI for the specific elements in each array index.
        //if (showItemSlots[index]) {
        //    //EditorGUILayout.PropertyField(itemImagesProperty.GetArrayElementAtIndex(index));
        //    //EditorGUILayout.PropertyField(itemsProperty.GetArrayElementAtIndex(index));


            

        //    // TODO display item properties
        //}

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }
}
