using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Conditions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// This simple script represents Items that can be picked
// up in the game.  The inventory system is done using
// this script instead of just sprites to ensure that items
// are extensible.
namespace AKAGF.GameArchitecture.ScriptableObjects.Inventory {
    [System.Serializable]
    public class Item : ScriptableObject {

        public Sprite sprite;                                           // Item icon in Inventory UI
        public string itemName = "Item Name";                           // How Item will be called in the Game
        public string itemShortDescription = "Item Description";        // Short Description of the Item   
        public string itemLongDescription = "";                         // Long Description or content of the Item  
        public bool isUnique;                                           // Is the item unique througt the whole game?
        public bool isStackable;                                        // Can the player carries more than one of this items at a time?
        public float maxQuantity;                                       // if the item is stackable, how many of them can the player carry at a time?
        public float currentQuantity;                                   // Current quantity of this Item carried by the player
        public bool removeIfEmpty;                                      // Remove from inventory when the quantity of this Item is zero?

        public Condition equippedCondition;                                       // Condition to implement drag & drop behaviour with Interactables Objects
        public InventoryItemList inventoryItemList;                     //Reference to the ItemList that contains this Item

#if UNITY_EDITOR
        [System.NonSerialized]
        public bool isExpanded;
#endif
    }

    [System.Serializable]
    public class ItemEvent : UnityEvent<Item> { }
}