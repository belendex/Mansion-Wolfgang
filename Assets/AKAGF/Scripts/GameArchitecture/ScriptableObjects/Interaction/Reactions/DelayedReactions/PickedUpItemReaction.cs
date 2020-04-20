using System.Collections.Generic;
using AKAGF.GameArchitecture.MonoBehaviours.Interaction;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Abstracts;
using AKAGF.GameArchitecture.ScriptableObjects.Inventory;
using UnityEngine;

namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.DelayedReactions
{
    /// <summary>
    /// This reaction add an item to an Inventory.
    /// </summary>

    public class PickedUpItemReaction : DelayedReaction{
        public Item item;               // The item asset to be added to the Inventory.
        public float quantity = 1;      // Quantity of the item to add, only in case it was stackable
        public string inventoryName;    // The name of the inventory in persistent scene
        public ItemEvent OnItemPickUp;
        private MonoBehaviours.Inventory.Inventory inventory;    // Reference to the Inventory component.


        protected override void SpecificInit() {
            List<MonoBehaviours.Inventory.Inventory> inventories = new List<MonoBehaviours.Inventory.Inventory>(FindObjectsOfType<MonoBehaviours.Inventory.Inventory>());
            inventory = inventories.Find(x => x.inventoryItemList.name == inventoryName);
        }


        protected override void ImmediateReaction(ref Interactable publisher) {
            inventory.AddItem(item);
            OnItemPickUp.Invoke(item);
        }
    }
}
