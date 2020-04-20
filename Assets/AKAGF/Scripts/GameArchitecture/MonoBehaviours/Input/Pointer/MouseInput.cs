using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AKAGF.GameArchitecture.MonoBehaviours.Input.Mouse {

    [global::System.Serializable]
    public class GameObjectEvent : UnityEvent<PointerEventData> { }

    public class MouseInput : MonoBehaviour  {

        [HideInInspector]
        public bool activeInput = true;

        public GameObjectEvent OnLeftButtonClick;
        public GameObjectEvent OnRightButtonClick;
        public GameObjectEvent OnMiddleButtonClick;


        public void PointerClickHandler(BaseEventData data) {

            if (!activeInput)
                return;

            PointerEventData pointerEventData = data as PointerEventData;            

            //Left click
            if (pointerEventData.button == PointerEventData.InputButton.Left) {
                OnLeftClick(pointerEventData);
            }
            //Right click
            else if (pointerEventData.button == PointerEventData.InputButton.Right) {
                OnRightClick(pointerEventData);
            }
            //Middle click
            else if (pointerEventData.button == PointerEventData.InputButton.Middle) {
                OnMiddleClick(pointerEventData);
            }
        }


        /********************** MousePointerHandler Methods *************************/
        private void OnLeftClick(PointerEventData pressedGameObject) {
            OnLeftButtonClick.Invoke(pressedGameObject);  
        }

        private void OnRightClick(PointerEventData pressedGameObject) {
            OnRightButtonClick.Invoke(pressedGameObject);
        }

        private void OnMiddleClick(PointerEventData pressedGameObject) {
            OnMiddleButtonClick.Invoke(pressedGameObject);
        }
    }
}
