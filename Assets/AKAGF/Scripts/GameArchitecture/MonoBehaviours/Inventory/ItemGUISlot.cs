using AKAGF.GameArchitecture.MonoBehaviours.GUI;
using AKAGF.GameArchitecture.ScriptableObjects.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace AKAGF.GameArchitecture.MonoBehaviours.Inventory {

    [global::System.Serializable]
    public class ItemGUISlot : MonoBehaviour {

        [HideInInspector]
        public bool isEmpty = true;

        //UNITY GUI elements
        public Image itemImage;
        private UITextElementWrapper itemNameText;
        private UITextElementWrapper itemShortDescriptionText;
        private UITextElementWrapper itemLongDescriptionText;

        public Image itemQuantityImage;
        public UITextElementWrapper itemQuantityText;

        public GameObject itemEquippedMark;

        public Item item { get; set; }


        private EquippedItemHolder equippedItemHolder;

        private void Awake() {
            isEmpty = true;
        }

        private void Start() {
            itemEquippedMark.SetActive(false);
        }


        private void LateUpdate() {
            updateEquippedMark();
        }


        public void updateEquippedMark() {
            if (item != null && item.equippedCondition != null)
                itemEquippedMark.SetActive(item.equippedCondition.isSatisfied);
        }


        /// This method assigns the item to the slot filling and enabling the images
        public void setItem(Item i) {
            item = i;
            itemImage.sprite = item.sprite;
            itemImage.enabled = true;

            if (item.isStackable) {
                itemQuantityText.setText( item.currentQuantity.ToString());
                itemQuantityImage.enabled = true;
            }

            isEmpty = false;
        }


        public void removeItem() {
            itemImage.enabled = false;
            itemImage.sprite = null;
            updateEquippedMark();
            item = null;
            itemQuantityImage.enabled = false;

            isEmpty = true;
        }


        public void displayItemInfo() {
            if (isEmpty || !item)
                return;

            itemNameText.setText(item.name);
            itemShortDescriptionText.setText(item.itemShortDescription);
            itemLongDescriptionText.setText(item.itemLongDescription);
        }

        
        public void preloadEquippedItemHolder() {

            if (isEmpty || !equippedItemHolder)
                return;

            equippedItemHolder.preLoadEquippedItem(this);
        }


        public void setUIFields(UITextElementWrapper itemName, UITextElementWrapper itemShorDescription, UITextElementWrapper itemLongDescription, EquippedItemHolder sItem) {
            itemNameText = itemName;
            itemShortDescriptionText = itemShorDescription;
            itemLongDescriptionText = itemLongDescription;
            equippedItemHolder = sItem;
        }


        public void updateItemQtyText() {
            itemQuantityText.setText(item.currentQuantity.ToString());
            itemQuantityImage.enabled = true;
        }
    }
}

