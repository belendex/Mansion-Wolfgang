using AKAGF.GameArchitecture.ScriptableObjects.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace AKAGF.GameArchitecture.MonoBehaviours.Inventory {

    public class EquippedItemHolder : MonoBehaviour {
        [Tooltip("Image reference that holds the selectable item in the inventory GUI")]
        public Image holderImage;

        [Tooltip("Event raised when the item is selected (click/drag).")]
        public ItemEvent OnSelect;

        [Tooltip("Event raised when the item is unselected (click).")]
        public ItemEvent OnUnselect;

        /// <summary>
        /// Reference to the currently selected item GUI Slot.
        /// </summary>
        public ItemGUISlot currentSlot { get; private set; }

        // Slot state
        private bool selected  = false;


        /// <summary>
        /// Method to preload all the data from the itemSlot
        /// </summary>
        /// <param name="itemSlot">the item slot to be preloaded</param>
        public void preLoadEquippedItem(ItemGUISlot itemSlot) {

            // Reference to the item for later use in drop event
            currentSlot = itemSlot;

            // Reference to the original Item image from the Inventory slot
            Image originalImage = itemSlot.itemImage;

            // Scale and positioning the holder image over the original image in the slot
            holderImage.rectTransform.localScale = originalImage.rectTransform.localScale;
            holderImage.rectTransform.position = originalImage.rectTransform.position;

            // set the holder image to be the same as the slot image
            holderImage.sprite = originalImage.sprite;

            // Disable the original image and enable the holder one
            originalImage.enabled = false;
            holderImage.enabled = true;
            holderImage.raycastTarget = true;

        }


        /// <summary>
        /// Called from an Event Trigger or button, it usually raises the event 
        /// that will set the equipped item in the inventory or open a specific item
        /// window with extra information. This method acts as a switch between selected
        /// an unselected item in the inventory through the boolean variable selected.
        /// </summary>
        public void onSlotClick() {
            if (!currentSlot)
                return;

            selected = !selected;

            if (selected) {
                OnSelectedSlot();
            }
            else {
                OnUnSelectedSlot();
            }
        }

        /// <summary>
        /// Same behaviour as onSlotClick but for dragging event instead of click.
        /// In this case, always the player drags an item out of the inventory
        /// it will be considered equiped until a Drop or EndDrag event will occur.
        /// </summary>
        public void onSlotDrag(){
            OnSelectedSlot();

            // Once the image is dragging the raycastTarget is
            // deactivated in order to allow the pointer to be
            // detected by the drop event handler.
            holderImage.raycastTarget = false;
        }

        /// <summary>
        /// Method to reset all the parameters and hide the image of the holder.
        /// </summary>
        public void clearEquippedItem() {
            if (currentSlot)
                currentSlot.itemImage.enabled = true;

            holderImage.enabled = false;
            holderImage.raycastTarget = false;

            currentSlot = null;
        }


        private void OnSelectedSlot() {
            OnSelect.Invoke(currentSlot.item);
        }


        private void OnUnSelectedSlot() {
            OnUnselect.Invoke(currentSlot.item);
        } 
    }
}