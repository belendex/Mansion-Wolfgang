using UnityEngine;
using UnityEngine.EventSystems;
using AKAGF.GameArchitecture.MonoBehaviours.Input.Mouse;
using UnityEngine.Events;


    [System.Serializable]
    public class DragEventHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [System.Serializable] // Mark this as serializable so OnDragMove will show in the inspector.
        public class Vector3Event : UnityEvent<Vector3> { } // Custom UnityEvent with Vector3 parameter.
                                                        // Event to fire when dragging begins - assignable in the inspector
        public GameObjectEvent OnDragStart;
        private Vector3 initialDragPosition = new Vector3();

        // Event to fire when drag is moving - assignable in the inspector
        public Vector3Event OnDragMove;


        public Vector3Event OnEndDragMove;


        public void OnBeginDrag(PointerEventData eventData) {

        if (eventData.button == PointerEventData.InputButton.Right || 
            eventData.button == PointerEventData.InputButton.Middle) 
                return;

            initialDragPosition = eventData.pointerCurrentRaycast.gameObject.transform.localPosition;
            // Otherwise fire the UnityEvent we assigned in the inspector for this event.
            OnDragStart.Invoke(eventData);
        }

        // 2
        public void OnDrag(PointerEventData eventData) {

            if (eventData.button == PointerEventData.InputButton.Right ||
                eventData.button == PointerEventData.InputButton.Middle)
                return;
        
            // Feed the world position of the pointer to OnDragMove
            OnDragMove.Invoke(eventData.position);
        }


        public void OnEndDrag(PointerEventData eventData) {

            if (eventData.button == PointerEventData.InputButton.Right ||
                eventData.button == PointerEventData.InputButton.Middle)
                return;

            OnEndDragMove.Invoke(initialDragPosition);
        }
}
