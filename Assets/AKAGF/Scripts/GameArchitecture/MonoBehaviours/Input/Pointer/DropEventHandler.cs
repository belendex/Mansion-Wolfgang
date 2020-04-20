using UnityEngine;
using UnityEngine.EventSystems;
using AKAGF.GameArchitecture.MonoBehaviours.Input.Mouse;


    [System.Serializable]
    public class DropEventHandler : MonoBehaviour, IDropHandler {

        public GameObjectEvent OnDropMove;


    public void OnDrop(PointerEventData eventData) {

            OnDropMove.Invoke(eventData);
        }

}
