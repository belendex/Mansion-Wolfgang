using System.Collections.Generic;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Conditions;
using AKAGF.GameArchitecture.ScriptableObjects.Inventory;
using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Inventory {

    public class Inventory : MonoBehaviour {

        public bool fixedSize = true;                               // is this a dinamic inventory?
        public int numItemSlots = 10;                               // In case that Inventory has fixed size, this represents the max number of items that can be carried.  
        public List<Item> items = new List<Item>(0);                // The Items that are carried by the player.
        public InventoryItemList inventoryItemList;                 // Reference to the scriptable list of items. Each ingame inventory only can have items that are contained in this list.
        public Condition equippedCondition; 
        public ItemEvent OnAddItem;
        public ItemEvent OnRemoveItem;

        public Item equippedItem { get; private set; }


        // This function is called by the PickedUpItemReaction in order to add an item to the inventory.
        public void AddItem(Item itemToAdd, float quantity=1) {

            if (!isValidItem(itemToAdd))
                return;

            // The item is already present in the inventory
            if (items.Contains(itemToAdd)) {

                // is Unique
                if (itemToAdd.isUnique)
                    Debug.Log("The item: " + itemToAdd.name + " is tagged as Unique and is already present in the inventory: " + this.inventoryItemList.name);

                // Stackable Object
                if (itemToAdd.isStackable) {
                    float quantityToAdd = itemToAdd.currentQuantity + quantity;
                    itemToAdd.currentQuantity = (quantityToAdd >= itemToAdd.maxQuantity) ? itemToAdd.maxQuantity : quantityToAdd;

                    //inventoryGUI.resetItemQuantity(itemToAdd);
                }

                return;
            }


            // Inventory is full
            if (fixedSize && items.Count == numItemSlots) {
                return;
            }


            // If we get here, it means that item can be added to the inventory
            items.Add(itemToAdd);
            OnAddItem.Invoke(itemToAdd);
            // inventoryGUI.addItem(itemToAdd);
        }


        // This method checks that the Item which is trying to be added is present in the inventoryItemList attached to this inventory 
        private bool isValidItem(Item item) {

            for (int i = 0; i < inventoryItemList.itemList.Length; i++) {
                if (inventoryItemList.itemList[i] == item)
                    return true;
            }

            Debug.LogWarning("No item found with name: " + item.name + " in inventoryItemList: " + inventoryItemList.name);
            return false;
        }


        public void AddItem(string itemToAddName, float quantity = 1) {

            for (int i = 0; i < inventoryItemList.itemList.Length; i++)
                if (inventoryItemList.itemList[i].name.Equals(itemToAddName)) {
                    AddItem(inventoryItemList.itemList[i], quantity);
                    return;
                }

            Debug.Log("No item found with name: " + itemToAddName + " in inventoryItemList: " + inventoryItemList.name);
        }


        // This function is called by the LostItemReaction in order to remove an item from the inventory.
        public void RemoveItem(Item itemToRemove, float quantity = 1) {

            //If item is not present in inventory, inform and return
            if (!items.Contains(itemToRemove)) {
                Debug.LogWarning("No item found to remove in Inventory: " + name + " with name: " + itemToRemove.name);
                return;
            }

            //Handle Item quantity only if it is stackable
            if (itemToRemove.isStackable) {
                float newQuantity = itemToRemove.currentQuantity - quantity;
                itemToRemove.currentQuantity = (newQuantity <= 0) ? 0 : newQuantity;
                //inventoryGUI.resetItemQuantity(itemToRemove);

                //remove the item from the inventory if the quantity is zero and is marked as removeIfEmpty
                if (itemToRemove.currentQuantity == 0 && itemToRemove.removeIfEmpty) {
                    items.Remove(itemToRemove);
                }

                return;
            }

            // Remove the Item (isUnique or non stackable Item cases)
            items.Remove(itemToRemove);
            OnRemoveItem.Invoke(itemToRemove);
            //inventoryGUI.removeItem(itemToRemove);
        }


        public void RemoveItem(string itemToRemoveName, float quantity = 1){
            for (int i = 0; i < inventoryItemList.itemList.Length; i++)
                if (inventoryItemList.itemList[i].name.Equals(itemToRemoveName)) {
                    RemoveItem(inventoryItemList.itemList[i], quantity);
                    return;
                }

            Debug.Log("No item found with name: " + itemToRemoveName + " in inventoryItemList: " + inventoryItemList.name);
        }


        public void EquipItem(Item item) {

            if (isValidItem(item)) {

                resetEquippedItems();
                if (item != null && item.equippedCondition != null)
                    item.equippedCondition.isSatisfied = true;

                equippedItem = item;

                if (equippedCondition)
                    equippedCondition.isSatisfied = true;
            }
        }


        public void UnequipItem() {

            resetEquippedItems();
            equippedItem = null;

            if (equippedCondition)
                equippedCondition.isSatisfied = false;

        }


        private void resetEquippedItems() {
            for (int i = 0; i < items.Count; i++) {
                if (items[i].equippedCondition != null) {
                    items[i].equippedCondition.isSatisfied = false;
                }
            }
        }


        // This method is used to serialize to json a specific state of the inventory.
        // This is achieve by storing all item's project names in string format, so all
        // items must have differents names through whole project
        public string[] getCurrentItemsNames() {
            string[] itemsNames = new string[items.Count];

            for (int i = 0; i < items.Count; i++) {
                if(items[i] != null)
                    itemsNames[i] = items[i].name;
            }
            
            return itemsNames;
        }


        // This method is used to set the inventory to a certain state. 
        // For example to a save game state
        public void setInventoryState(string[] itemsNames) {

            // Clear the current list states
            items.Clear();
            //items = new Item[numItemSlots];

            //for (int i = 0; i < itemImages.Length; i++) 
            //    itemImages[i].sprite = null;

            // Get all Items scriptableObjects from project resources
            List<Item> foundItems = new List<Item>((Item[])Resources.FindObjectsOfTypeAll(typeof(Item)));

            // Go through all items names and load them from resources
            for (int i = 0; i < itemsNames.Length; i++) {

                // Get resource Item by name
                Item auxItem = foundItems.Find(item => item.name == itemsNames[i]);
        
                if (auxItem)
                    AddItem(auxItem);
                else if(itemsNames[i] != null)
                    Debug.LogError("No Item found with name: " + itemsNames[i]  + " inside project resources.");
            }
        }

    }
}
