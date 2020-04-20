using System;
using System.Collections;
using System.Collections.Generic;
using AKAGF.GameArchitecture.MonoBehaviours.Interaction;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Abstracts;
using AKAGF.GameArchitecture.ScriptableObjects.Inventory;

namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.DelayedReactions {
    /// <summary>
    /// This reaction removes an item from an Inventory.
    /// </summary>
    [Serializable]
    public class LostItemReaction : DelayedReaction {
        public Item item;               // Item to be removed from the Inventory.
        public float quantity = 1;      // Quantity of the item to add, only in case it was stackable
        public string inventoryName;    // The name of the inventory in persistent scene
        public ItemEvent OnItemLost;
        private MonoBehaviours.Inventory.Inventory inventory;    // Reference to the Inventory component.


        protected override void SpecificInit() {
            List<MonoBehaviours.Inventory.Inventory> inventories = new List<MonoBehaviours.Inventory.Inventory>(FindObjectsOfType<MonoBehaviours.Inventory.Inventory>());
            inventory = inventories.Find(x => x.inventoryItemList.name == inventoryName);
        }


        protected override void ImmediateReaction(ref Interactable publisher) {
            inventory.RemoveItem (item);
            OnItemLost.Invoke(item);
        }
    }
}
