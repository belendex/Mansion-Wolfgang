using System.Collections;
using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Camera {

    // Enums
    public enum SMOOTH_CAMERA_MOVEMENT { NONE, LERP, SMOOTHDAMP }
    public enum CAMERA_STATES { NONE, ROTATE, FOLLOW, BOTH }    // Camera modes enum

    public class CameraMasterController : MonoBehaviour {

        [Header("Debug")]
        public bool debugMode = true;
        public CAMERA_STATES cameraState = CAMERA_STATES.NONE;      // Current camera state (mode)

        private Vector3 cameraVelocity;                             // Current camera velocity (only used in Smoothdamp interpolated movement) 
        private Transform cameraTargetPosition;                     // Target that camera will follow and look at (target.position + targetOffset)
        private Vector3 nextCameraPosition = Vector3.zero;          
        [HideInInspector]
        public bool isInAnimation = false;
        

        public CameraMovementSettings movementConfig
            = new CameraMovementSettings();                         // Class to handle camera movement
        public CameraOrbitSettings orbitConfig
            = new CameraOrbitSettings();                            // Class to handle camera orbitation
        public CameraZoomSettings zoomConfig
            = new CameraZoomSettings();                             // Class to handle camera zoom
        public CameraInputSettings inputConfig
            = new CameraInputSettings();                            // Class to handle camera input
        public CameraCollisionSettings collisionConfig
            = new CameraCollisionSettings();                        // Class to handle camera collisions
        

        private void Start() {

            cameraTargetPosition = new GameObject().transform;
            cameraTargetPosition.name = "CameraTargetPosition";

            //Set initial camera position
            transform.position = movementConfig.target.position + (Vector3)movementConfig.targetOffset + cameraTargetPosition.forward * -movementConfig.hdistance;
            //Set initial camera rotation
            cameraTargetPosition.eulerAngles = new Vector3(orbitConfig.vRotation, orbitConfig.hRotation, 0);
            // Set initial zoom distance
            zoomConfig.initialZoom = -movementConfig.hdistance;
        }


        private void Update() {
            if (!isInAnimation) {
                // Get input for orbiting and zooming
                inputConfig.getInput();   
            }
        }


        private void LateUpdate() {

            // Adjust the target position with the offset
            cameraTargetPosition.position = movementConfig.target.position + (Vector3)movementConfig.targetOffset;

            switch (cameraState) {
                case CAMERA_STATES.FOLLOW: movementHandling(); // Handle movement
                break;

                case CAMERA_STATES.ROTATE: lookAtTarget();  // handle rotation
                break;

                case CAMERA_STATES.BOTH:
                movementHandling();
                lookAtTarget();
                break;
            }

            if (!isInAnimation) {
                //handle orbiting
                orbitHandling(inputConfig.hOrbitInput, inputConfig.vOrbitInput, inputConfig.hSnapInput, inputConfig.allowManualOrbit);
                //Zoom
                zoomHandling();
            }
        }


        private void orbitHandling(float hInput, float vInput, float snapInput = 0, bool allowOrbit = true) {

            // Do nothing if manual orbit is not allowed
            if (!allowOrbit)
                return;

            // Snap the camera to target's back if snap button is pressed
            if (snapInput > 0)
                orbitConfig.hRotation = Mathf.Lerp(orbitConfig.hRotation, orbitConfig.initialHRotation, orbitConfig.hOrbitSmooth);

            // Horizontal orbit
            orbitConfig.hRotation += hInput * orbitConfig.hOrbitSmooth * Time.deltaTime;

            // Vertical orbit
            orbitConfig.vRotation += vInput * orbitConfig.vOrbitSmooth * Time.deltaTime;
            // Clamp vertical rotation 
            orbitConfig.vRotation = Mathf.Clamp(orbitConfig.vRotation, orbitConfig.minVRotation, orbitConfig.maxVRotation);

            // Unity doesn't expose a method to modify eulerAngles by component, so it's cached in targetRotation
            orbitConfig.targetRotation.x = orbitConfig.vRotation;
            orbitConfig.targetRotation.y = orbitConfig.hRotation;
            cameraTargetPosition.eulerAngles = orbitConfig.targetRotation;
        }


        private void movementHandling() {
            
            // Calculate the next camera position based on the camera Target position and the relative distance between target and camera
            nextCameraPosition = cameraTargetPosition.position + cameraTargetPosition.forward * -movementConfig.hdistance;

            // In case of camera collision recalculate next position based on hit parameters
            if (collisionConfig.handleCollisions && collisionConfig.isCollisioning(cameraTargetPosition.position, nextCameraPosition))
                nextCameraPosition = collisionConfig.cameraHit.point + collisionConfig.cameraHit.normal * collisionConfig.collisionSpace;

            // Make the camera goes to next calculated position  
            switch (movementConfig.smoothMovement) {
                case SMOOTH_CAMERA_MOVEMENT.NONE:
                transform.position = nextCameraPosition;
                break;

                case SMOOTH_CAMERA_MOVEMENT.LERP:
                transform.position = Vector3.Lerp(transform.position, nextCameraPosition, movementConfig.speedSmooth);
                break;

                case SMOOTH_CAMERA_MOVEMENT.SMOOTHDAMP:
                transform.position = Vector3.SmoothDamp(transform.position, nextCameraPosition, ref cameraVelocity, movementConfig.speedSmooth);
                break;
            }
        }


        private void lookAtTarget() {
            // Look at target
            transform.LookAt(cameraTargetPosition);
        }


        private void zoomHandling() {

            if (!inputConfig.allowZoom)
                return;     // zoom not allowed

            // Calculating next zoom position
            zoomConfig.targetZoom += zoomConfig.zoomStep * inputConfig.zoomInput;
            // Moving the camera
            movementConfig.hdistance = Mathf.Lerp(movementConfig.hdistance, zoomConfig.targetZoom, zoomConfig.zoomSmooth * Time.deltaTime);
            // Clamping zoom inside threshold
            movementConfig.hdistance = Mathf.Clamp(movementConfig.hdistance, zoomConfig.minZoom, zoomConfig.maxZoom);
        }


        public void OnAnimationCall(CameraAnimation[] cameraAnimations, float onEndDelay) {

            // This method shouldn't being called when a cameraAnimation is already playing,
            // but if for any reason it does, avoid to start another animation.
            if (isInAnimation)
                return;

            IEnumerator coroutine = startCameraAnimation(cameraAnimations, onEndDelay);
            StartCoroutine(coroutine);
        }


        private IEnumerator startCameraAnimation(CameraAnimation[] cameraAnimations, float onEndDelay) {

            // Begin camera animation
            isInAnimation = true;

            // Time counter
            float currentTime = 0;
            // flag to konw the first frame of cameraAnimation
            bool firstFrame = true; 
            // Store current camera config
            CameraMovementSettings initialMovementConfig = this.movementConfig;
            CameraOrbitSettings initialOrbitSettings = this.orbitConfig;

            // Looping all camera animations based on time
            for (int i = 0; i < cameraAnimations.Length;) {

                // First frame of each animation inside the array
                if (firstFrame) {
                    movementConfig = cameraAnimations[i].movementConfig;
                    orbitConfig = cameraAnimations[i].orbitConfig;
                    firstFrame = false;
                }


                // Orbit handling
                orbitHandling(cameraAnimations[i].orbitHinput, cameraAnimations[i].orbitVinput);
            
                currentTime += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);

                if (currentTime >= cameraAnimations[i].time || (cameraAnimations[i].skippableByPlayer && UnityEngine.Input.GetButton(inputConfig.SKIP_ANIMATION))) {
                    currentTime = 0;
                    firstFrame = true;
                    i++;
                }
            }


            yield return new WaitForSeconds(onEndDelay);

            // Restore before animation camera config
            movementConfig = initialMovementConfig;
            orbitConfig = initialOrbitSettings;

            // Animation is finished
            isInAnimation = false;
        }



        private void OnDrawGizmos() {

            if (!debugMode || !cameraTargetPosition)
                return; // Don't want debug

            // camera Target Gizmo
            Gizmos.DrawIcon(cameraTargetPosition.position, "CameraTargetGizmo.png", true);
            Gizmos.color = Color.green;

            // Change line color to red if the camera is colliding
            if (collisionConfig.handleCollisions && collisionConfig.collisionActive)
                Gizmos.color = Color.red;

            Gizmos.DrawLine(transform.position, cameraTargetPosition.position);
        }
    }

    // Camera Settings Classes

    [global::System.Serializable]
    public class CameraOrbitSettings {
        
        public float vRotation = -20;               // Current camera vertical rotation
        public float hRotation = 180;               // Current camera horizontal rotation
        [Range(0, 89)]
        public float maxVRotation = 25;             // Max vertical rotation
        [Range(-89, 0)]
        public float minVRotation = -85;            // Min vertical rotation
        public float vOrbitSmooth = 150;            // Vertical orbit smooth speed
        public float hOrbitSmooth = 150;            // Horizontal orbit smooth speed

        [HideInInspector]
        public float initialVRotation;              //initial vertical rotation
        [HideInInspector]
        public float initialHRotation;              //Initial horizontal rotation

        [HideInInspector]
        public Vector3 targetRotation;              // New camera target horizontal rotation  orbit

        public CameraOrbitSettings() {
            initialVRotation = vRotation;
            initialHRotation = hRotation;
        }
    }

    [global::System.Serializable]
    public class CameraZoomSettings {
        
        public float zoomSmooth = 100;      // Zoom smooth speed
        public float zoomStep = 2;          // steps at each mousewheel move
        public float maxZoom = -2;          // max zoom
        public float minZoom = -15;         // min zoom

        [HideInInspector]
        public float initialZoom;           // Store initial zoom to use it back when necessary
        [HideInInspector]
        public float targetZoom;            // The new zoom selected by the player
    }

    [global::System.Serializable]
    public class CameraInputSettings {
        public bool allowManualOrbit = false;       // Allow CameraOrbit arround the target
        public string ORBIT_HORIZONTAL_SNAP = "SnapCamera";
        public string ORBIT_HORIZONTAL = "hCameraOrbit";
        public string ORBIT_VERTICAL = "vCameraOrbit";
        public bool allowZoom = false;      // is zoom allowed
        public string ZOOM = "CameraZoom";
        //public string ZOOM_SNAP = "Zoom Snap";

        public string SKIP_ANIMATION = "Action";
        

        [HideInInspector]
        public float zoomInput, hOrbitInput, vOrbitInput, hSnapInput;

        public void getInput() {

            hOrbitInput = UnityEngine.Input.GetAxisRaw(ORBIT_HORIZONTAL);
            vOrbitInput = UnityEngine.Input.GetAxisRaw(ORBIT_VERTICAL);
            hSnapInput = UnityEngine.Input.GetAxisRaw(ORBIT_HORIZONTAL_SNAP);
            zoomInput = UnityEngine.Input.GetAxisRaw(ZOOM);
            //zoomSnapInput = Input.GetAxisRaw(ZOOM_SNAP);
        }

        public void resetInput() {
            zoomInput = hOrbitInput = vOrbitInput = hSnapInput = 0;
        }
    }

    [global::System.Serializable]
    public class CameraCollisionSettings {
        public bool handleCollisions = true;
        public LayerMask collisionMask;
        public RaycastHit cameraHit;
        public float collisionSpace = 0.15f;

        [HideInInspector]
        public bool collisionActive = false;

        public bool isCollisioning(Vector3 origin, Vector3 destination) {
            collisionActive = Physics.Linecast(origin, destination, out cameraHit, collisionMask);
            return collisionActive;
        }
    }

    [global::System.Serializable]
    public class CameraMovementSettings {
        public Transform target;                                    // Target Reference
        public Vector2 targetOffset = new Vector3(0, 2);            // Offset applied to camera target
        public float speedSmooth = 5f;                              // Camera speed Smoothness
        public float hdistance = 4f;                                // Horizontal distance from camera to target                                         
        public SMOOTH_CAMERA_MOVEMENT smoothMovement;
    }
}