using System.Collections;
using AKAGF.GameArchitecture.MonoBehaviours.Interaction.Abstracts;
using UnityEngine;

/* This class is not tested */


namespace AKAGF.GameArchitecture.MonoBehaviours.Interaction.Sensors {
    public class PlayerInteractionRaycastSensor : PlayerInteractionSensor {

        public UnityEngine.Camera fpsCamera;                   // The player perspective main camera
        public float rayLenght = 2f;                           // How far from the camera the Interactable raycasting is performing
        public float rayCastTimeInterval = .2f;                // The time between rays. Keep this value as high as user experience allows.
        private Vector2 screenCenterPoint;                     // Cached Vector for the center point of the screen
        private RayCaster hitData;                             // Raycast Handling
        private bool rayFlag;                                  // flag to avoid execute corrutine when is no need of it.
        private WaitForSeconds raycastWait;                    // Cached wait for seconds
        private InteractionTrigger cachedTrigger;              // Cached Interaction trigger for avoiding raycast of autoreacting Interactables

        void Start () {
            screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
            raycastWait = new WaitForSeconds(rayCastTimeInterval);
        }
	

        void Update () {

            // Timing raycast
            StartCoroutine(retardedRaycast());

            if(currentInteractable)
                timingInteraction(currentInteractable);
        }


        private IEnumerator retardedRaycast() {

            if (rayFlag) yield break; 

            rayFlag = true;

            hitData.hitEvent(fpsCamera.ScreenPointToRay(screenCenterPoint), rayLenght);
            if (hitData.hitGameObject) {
                cachedTrigger = hitData.hitGameObject.GetComponent<InteractionTrigger>();

                if (!cachedTrigger.autoReact)
                    currentInteractable = cachedTrigger;
            }
            else {
                currentInteractable = cachedTrigger = null;
            }

            yield return raycastWait;

            rayFlag = false;
        }


        [global::System.Serializable]
        public struct RayCaster {

            [HideInInspector]
            public GameObject hitGameObject;
            private RaycastHit hit;

            public void hitEvent(Ray ray, float rayLenght) {
                if (Physics.Raycast(ray, out hit, rayLenght)) {
                    hitGameObject = hit.collider.gameObject;
                }
                else {
                    hitGameObject = null;
                }
            }
        }
    }
}
