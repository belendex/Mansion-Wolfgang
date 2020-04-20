using UnityEngine;
using UnityEngine.EventSystems;
using AKAGF.GameArchitecture.MonoBehaviours.Input.Mouse;
using UnityEngine.Events;


    [System.Serializable]
    public class OnPointerUpEventHandler : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {

        // The event to fire when the pointer has been released.
        public GameObjectEvent OnPointerReleased;

        public void OnPointerUp(PointerEventData eventData) {

            // Otherwise fire the OnPointerReleased event.
            OnPointerReleased.Invoke(eventData);
        }

        // IPointerDownHandler implementation is required for IPointerUpHandler to work.
        // It does not have to actually do anything.
        public void OnPointerDown(PointerEventData eventData) { }
    }

