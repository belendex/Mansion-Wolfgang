using UnityEngine;
using UnityEngine.EventSystems;
using AKAGF.GameArchitecture.MonoBehaviours.Input.Mouse;


    [System.Serializable]
    public class SwipeEventHandler : MonoBehaviour, IDragHandler {

        // Event to fire when the drag moves upward
        public GameObjectEvent OnDragUpward;

        // Event to fire when the drag moves downward
        public GameObjectEvent OnDragDownward;

        public GameObjectEvent OnDragRight;
        public GameObjectEvent OnDragLeft;


        public void OnDrag(PointerEventData eventData) {

            // If the change in Y position of the pointer is positive
            // then the user is dragging upward. If it is negative
            // then the user is dragging downward.
            float verticalDelta = Mathf.Abs(eventData.delta.y);
            float horizontalDelta = Mathf.Abs(eventData.delta.x );


            if (verticalDelta > horizontalDelta){

                if (eventData.delta.y > 0)
                    OnDragUpward.Invoke(eventData);
                else if (eventData.delta.y < 0)
                    OnDragDownward.Invoke(eventData);

            }
            else {
                if (eventData.delta.x > 0)
                    OnDragRight.Invoke(eventData);
                else if (eventData.delta.x < 0)
                    OnDragLeft.Invoke(eventData);

            }
        }
    }

