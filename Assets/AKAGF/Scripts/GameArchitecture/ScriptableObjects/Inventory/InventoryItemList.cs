using UnityEngine;

namespace AKAGF.GameArchitecture.ScriptableObjects.Inventory
{
    public class InventoryItemList : ScriptableObject {

        public string itemListName;             //Name to display for this ItemList within the game
        public string itemListDescription;      //Description of the itemList. It can be used in game or just for developers info
        public Item[] itemList;                 //The itemList itself
    }
}
