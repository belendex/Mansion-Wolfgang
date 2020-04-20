using AKAGF.GameArchitecture.MonoBehaviours.GUI;
using AKAGF.GameArchitecture.ScriptableObjects.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace AKAGF.GameArchitecture.MonoBehaviours.Inventory
{
    public class InventoryGUI : MonoBehaviour {

        public ItemGUISlot[] itemSlots;

        public UITextElementWrapper itemNameText;
        public UITextElementWrapper itemShortDescriptionText;
        public UITextElementWrapper itemLongDescriptionText;

        public EquippedItemHolder equippedItemImage;


        public void Start() {
            for (int i = 0; i < itemSlots.Length; i++) {
                itemSlots[i].setUIFields(itemNameText, itemShortDescriptionText, itemLongDescriptionText, equippedItemImage);
            }
        }

        public void ResetEquippedItem() {
            for (int i = 0; i < itemSlots.Length; i++) {
                itemSlots[i].updateEquippedMark();
            }
        }

        // Adds an item in the first empty slot
        public void addItem(Item item) {
            for (int i = 0; i < itemSlots.Length; i++) {
                if (itemSlots[i].isEmpty) {
                    itemSlots[i].setItem(item);
                    break;
                }
            }

        }

        // Removes the given item from the slot
        public void removeItem(Item item) {
            for (int i = 0; i < itemSlots.Length; i++) {
                if (itemSlots[i].item == item) {
                    itemSlots[i].removeItem();
                    break;
                }
            }
        
            shortSlots();
        }


        public void resetItemQuantity(Item stackableItem) {
            for (int i = 0; i < itemSlots.Length; i++) {
                if (itemSlots[i].item = stackableItem) {
                    itemSlots[i].updateItemQtyText();
                    break;
                }
            }
        }

        // This method shorts itemSlots to avoid gaps
        private void shortSlots() {

            int lastEmptySlot = 0;
            bool shorting = false;

            for (int i = 0; i < itemSlots.Length; i++) {
                if (itemSlots[i].isEmpty) {

                    lastEmptySlot = i;
                    shorting = true;
                }
                else {

                    if (shorting) {
                        itemSlots[lastEmptySlot].item = itemSlots[i].item;
                        itemSlots[i].removeItem();
                        itemSlots[lastEmptySlot].setItem(itemSlots[lastEmptySlot].item);
                        i--;
                        shorting = false;
                    }
                }
            }
        }
    }
}
