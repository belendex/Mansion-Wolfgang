using UnityEngine;
using UnityEditor;
using System.IO;
using AKAeditor;
using AKAGF.GameArchitecture.Literals;
using AKAGF.GameArchitecture.ScriptableObjects.Inventory;

[CustomEditor(typeof(InventoryItemList))]
public class InventoryItemListEditor : Editor {

    private string newItemName = "Item Project Name";                           //Placeholder for the new item project name

    private InventoryItemList inventoryItemList;                                //Reference to target ItemList

    private SerializedProperty itemListNameProperty;                            //Property representing the game name of the ItemList
    private SerializedProperty itemListDescriptionProperty;                     //Property representing the description of the list

    private const string itemListNamePropName = "itemListName";                 //Name of the list name property
    private const string itemListDescriptionPropName = "itemListDescription";   //Name of the list description property

    private ItemEditor[] itemsEditors;                                          //Array with all de ItemEditors that belongs to this itemList
 
    private const float messageGUILines = 3f;                                   // How many lines tall the GUI for the TextArea field should be.
    private const float buttonWidth = 48f;                                      // Width fot the buttons in the inspector custom editor

    public const string BASE_PATH = AKAGF_PATHS.INVENTORY_ITEM_LIST_PATH;


    // Call this function when the menu item is selected.
    [MenuItem(AKAGF_PATHS.AKAGF_MENU_FULL_PATH +
                    AKAGF_PATHS.INVENTORY_MENU_PATH +
                        AKAGF_PATHS.INVENTORY_ITEM_LIST_MENU_NAME)]
    public static InventoryItemList CreateInventoryItemList() {
        InventoryItemList asset = ScriptableObject.CreateInstance<InventoryItemList>();

        if (!Directory.Exists(BASE_PATH)) {
            Directory.CreateDirectory(BASE_PATH);
        }

        if (AssetDatabase.LoadAssetAtPath<InventoryItemList>(BASE_PATH + AKAGF_PATHS.NEW_INVENTORY_ITEM_LIST_BASE_NAME) == null) {
            AssetDatabase.CreateAsset(asset, BASE_PATH + AKAGF_PATHS.NEW_INVENTORY_ITEM_LIST_BASE_NAME);
            AssetDatabase.SaveAssets();
            return asset;
        }

        Debug.Log("There is already an InventoryItemList.asset, change the name of that ItemList first, then create another one.");
        return null;
    }

    private void OnEnable() {

        //Target the selected itemList in Unity's project view
        inventoryItemList = (InventoryItemList)target;

        if (inventoryItemList.itemList == null)
            inventoryItemList.itemList = new Item[0];

        if (itemsEditors == null)
            CreateEditors();

        //Cache the itemList properties
        itemListNameProperty = serializedObject.FindProperty(itemListNamePropName);
        itemListDescriptionProperty = serializedObject.FindProperty(itemListDescriptionPropName);
    }


    private void CreateEditors() {
        // Create a new array for the editors which is the same length at the Items array.
        itemsEditors = new ItemEditor[inventoryItemList.itemList.Length];

        // Go through all the empty array...
        for (int i = 0; i < itemsEditors.Length; i++) {
            // ... and create an editor with an editor type to display correctly.
            itemsEditors[i] = CreateEditor(inventoryItemList.itemList[i]) as ItemEditor;
            //itemsEditors[i].isExpanded = false;
        }
    }


    public override void OnInspectorGUI() {
        // Update the state of the serializedObject to the current values of the target.
        serializedObject.Update();

        GUILayout.BeginHorizontal();
        //Renaming
        inventoryItemList.name = EditorGUILayout.TextField("Item List Project Name", inventoryItemList.name as string);
        if (GUILayout.Button("Rename", GUILayout.ExpandWidth(false))) {
            string assetPath = AssetDatabase.GetAssetPath(inventoryItemList.GetInstanceID());
            AssetDatabase.RenameAsset(assetPath, inventoryItemList.name);
            AssetDatabase.SaveAssets();
        }
        GUILayout.EndHorizontal();

        itemListNameProperty.stringValue = EditorGUILayout.TextField("Item List Game Name", itemListNameProperty.stringValue as string);

        GUILayout.Space(10);

        // Description property
        EditorGUILayout.LabelField("Item List Description", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();

        itemListDescriptionProperty.stringValue =
            EditorGUILayout.TextArea(itemListDescriptionProperty.stringValue, EditorStyles.textArea, GUILayout.Height(EditorGUIUtility.singleLineHeight * messageGUILines));
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        newItemName = EditorGUILayout.TextField("New Item Name", newItemName);
        // Change default Unity Editor array elements management
        if (GUILayout.Button("Add Item", GUILayout.ExpandWidth(false))) {
            AddItem(newItemName);
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);


        if (inventoryItemList.itemList.Length > 0) {
            EditorGUILayout.LabelField("Items: " + inventoryItemList.itemList.Length, EditorStyles.boldLabel);
        }
        else {
            GUILayout.Label("This Inventory List is Empty.");
        }


        // If there are different number of editors to Items, create them afresh.
        if (itemsEditors.Length != inventoryItemList.itemList.Length) {
            // Destroy all the old editors.
            for (int i = 0; i < itemsEditors.Length; i++) {
                DestroyImmediate(itemsEditors[i]);
            }

            // Create new editors.
            CreateEditors();
        }

        // Display all the items.
        for (int i = 0; i < itemsEditors.Length; i++) {
            itemsEditors[i].OnInspectorGUI();
        }


        if (GUI.changed) {
            EditorUtility.SetDirty(inventoryItemList);
        }

        // Push data back from the serializedObject to the target.
        serializedObject.ApplyModifiedProperties();
    }

    
    private void AddItem(string name = "") {
        Item newItem = ScriptableObject.CreateInstance<Item>();
        newItem.inventoryItemList = this.inventoryItemList;

        if(name == null || name == "")
            newItem.name = "New Item " + inventoryItemList.itemList.Length+1;
        else
            newItem.name = name;

        ScriptableObjectUtility.AddScriptableObject(inventoryItemList, ref newItem, ref inventoryItemList.itemList, "Created new Item");

    }


    
}
