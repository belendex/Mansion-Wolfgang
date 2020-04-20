using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Camera {

    public class CameraTrigger : MonoBehaviour {

        public CameraMasterController cameraController;         // Reference to the CameraMasterController Script
        public Transform newCameraPosition;
        public CAMERA_STATES newState = CAMERA_STATES.NONE;
        public CameraMovementSettings cameraMovementSettings = new CameraMovementSettings();
        public CameraOrbitSettings cameraOrbitSettings = new CameraOrbitSettings();
        public CameraInputSettings cameraInputSettings = new CameraInputSettings();

        public void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Player")) {
                cameraController.cameraState = newState;
                
                cameraController.orbitConfig = cameraOrbitSettings;
                cameraController.movementConfig = cameraMovementSettings;
                cameraController.inputConfig = cameraInputSettings;

                if(newCameraPosition != null)
                    cameraController.transform.position = Vector3.Lerp(transform.position, newCameraPosition.position, cameraMovementSettings.speedSmooth);
            }
        }


    }
}
