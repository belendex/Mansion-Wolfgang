using AKAGF.GameArchitecture.MonoBehaviours.Characters.Abstracts;
using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Camera
{
    public class CameraController : MonoBehaviour {

        public Transform target;

        [global::System.Serializable]
        public class PositionSettings {
            public Vector3 targetPositionOffset = new Vector3(0, 3.4f, 0);
            public float lookSmooth = 100f;
            public float distanceFromTarget = -8;
            public float zoomSmooth = 100;
            public float zoomStep = 2;
            public float maxZoom = -2;
            public float minZoom = -15;
            public bool smoothFollow = true;
            public float smooth = 0.05f;

            [HideInInspector]
            public float newDistance = -8;
            [HideInInspector]
            public float adjustmentDistance = -8;
        }

        [global::System.Serializable]
        public class OrbitSettings{
            public float xRotation = -20;
            public float yRotation = -180;
            public float maxXRotation = 25;
            public float minXRotation = -85;
            public float vOrbitSmooth = 150;
            public float hOrbitSmooth = 150;
        }

        [global::System.Serializable]
        public class InputSettings {
            //public string MOUSE_ORBIT = "MouseOrbit";
            //public string MOUSE_ORBIT_VERTICAL = "MouseOrbitVertical";
            public string ORBIT_HORIZONTAL_SNAP = "OrbitHorizontalSnap";
            public string ORBIT_HORIZONTAL = "OrbitHorizontal";
            public string ORBIT_VERTICAL = "OrbitVertical";
            public string ZOOM = "Mouse ScrollWheel";
        }

        [global::System.Serializable]
        public class DebugSettings {
            public bool drawDesiredCollisionLines = true;
            public bool drawAdjustedCollisionLines = true;
        }

        public PositionSettings positionConfig = new PositionSettings();
        public OrbitSettings orbitConfig = new OrbitSettings();
        public InputSettings inputConfig = new InputSettings();
        public DebugSettings debugConfig = new DebugSettings();
        public CollisionHandler collisionHandler = new CollisionHandler();

        private Vector3 targetPos = Vector3.zero;
        private Vector3 destination = Vector3.zero;
        Vector3 adjustedDestination = Vector3.zero;
        Vector3 camVel = Vector3.zero;
        private PlayerControllerBase playerController;
        private float vOrbitInput, hOrbitInput, zoomInput, hOrbitSnapInput, mouseOrbitInput, vMouseOrbitInput;
        Vector3 previousMousePos = Vector3.zero;
        Vector3 currentMousePos = Vector3.zero;


        private void Start() {
            setCameraTarget(target);

            vOrbitInput = hOrbitInput = zoomInput = hOrbitSnapInput = mouseOrbitInput = vMouseOrbitInput = 0;

            moveTotarget();

            collisionHandler.initialize(UnityEngine.Camera.main);
            collisionHandler.updateCameraClipPoints(transform.position, transform.rotation, ref collisionHandler.adjustedCameraClipPoints);
            collisionHandler.updateCameraClipPoints(transform.position, transform.rotation, ref collisionHandler.desiredCameraClipPoints);

            previousMousePos = currentMousePos = UnityEngine.Input.mousePosition;
        }


        private void setCameraTarget(Transform t) {
            target = t;

            if (target) {
                if (!(playerController = target.GetComponent<PlayerControllerBase>())) {
                    Debug.LogError("No playerController found");
                }
            }

        }


        private void getInput() {
            vOrbitInput = UnityEngine.Input.GetAxisRaw(inputConfig.ORBIT_VERTICAL);
            hOrbitInput = UnityEngine.Input.GetAxisRaw(inputConfig.ORBIT_HORIZONTAL);
            hOrbitSnapInput = UnityEngine.Input.GetAxisRaw(inputConfig.ORBIT_HORIZONTAL_SNAP);
            zoomInput = UnityEngine.Input.GetAxisRaw(inputConfig.ZOOM);
            //mouseOrbitInput = Input.GetAxisRaw(inputConfig.MOUSE_ORBIT);
            //vMouseOrbitInput = Input.GetAxisRaw(inputConfig.MOUSE_ORBIT_VERTICAL);
        }


        private void Update() {
            getInput();
            ZoomInOnTarget();
        }


        private void FixedUpdate() {
            //Camera movement
            moveTotarget();
            //Camera rotation
            lookAtTarget();
            orbitTarget();
            //mouseOrbitTarget();
        

            collisionHandler.updateCameraClipPoints(transform.position, transform.rotation, ref collisionHandler.adjustedCameraClipPoints);
            collisionHandler.updateCameraClipPoints(transform.position, transform.rotation, ref collisionHandler.desiredCameraClipPoints);

            //Debug lines
            for (int i = 0; i < 5; i++) {
                if (debugConfig.drawDesiredCollisionLines) 
                    Debug.DrawLine(targetPos, collisionHandler.desiredCameraClipPoints[i], Color.white);
            
                if (debugConfig.drawAdjustedCollisionLines) 
                    Debug.DrawLine(targetPos, collisionHandler.adjustedCameraClipPoints[i], Color.green);
            }

            collisionHandler.checkColliding(targetPos); // Raycast
            positionConfig.adjustmentDistance = collisionHandler.getAdjustedDistanceWithRayFrom(targetPos);
        }


        private void moveTotarget() {
            targetPos = target.position + Vector3.up * positionConfig.targetPositionOffset.y + Vector3.forward * positionConfig.targetPositionOffset.z + transform.TransformDirection(Vector3.right * positionConfig.targetPositionOffset.x);
            destination = Quaternion.Euler(orbitConfig.xRotation, orbitConfig.yRotation + target.eulerAngles.y, 0) * -Vector3.forward * positionConfig.distanceFromTarget;
            destination += targetPos;

            if (collisionHandler.colliding) {
                adjustedDestination = Quaternion.Euler(orbitConfig.xRotation, orbitConfig.yRotation + target.eulerAngles.y, 0) * Vector3.forward * positionConfig.adjustmentDistance;
                adjustedDestination += targetPos;

                if (positionConfig.smoothFollow) {
                    transform.position = Vector3.SmoothDamp(transform.position, adjustedDestination, ref camVel, positionConfig.smooth);
                }else {

                    transform.position = adjustedDestination;
                }
            }
            else {
                if (positionConfig.smoothFollow) {
                    transform.position = Vector3.SmoothDamp(transform.position, destination, ref camVel, positionConfig.smooth);
                }
                else {

                    transform.position = destination;
                }
            }
        }

   
        private void lookAtTarget() {
            Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, positionConfig.lookSmooth * Time.deltaTime);
        }


        private void orbitTarget() {
            if (hOrbitSnapInput > 0) {
                orbitConfig.yRotation = -180;
            }

            orbitConfig.xRotation += -vOrbitInput * orbitConfig.vOrbitSmooth * Time.deltaTime;
            orbitConfig.yRotation += -hOrbitInput * orbitConfig.hOrbitSmooth * Time.deltaTime;

            if (orbitConfig.xRotation > orbitConfig.maxXRotation) {
                orbitConfig.xRotation = orbitConfig.maxXRotation;
            }

            if (orbitConfig.xRotation < orbitConfig.minXRotation) {
                orbitConfig.xRotation = orbitConfig.minXRotation;
            }

        }


        private void ZoomInOnTarget() {

            positionConfig.newDistance += positionConfig.zoomStep * zoomInput;
            positionConfig.distanceFromTarget = Mathf.Lerp(positionConfig.distanceFromTarget, positionConfig.newDistance, positionConfig.zoomSmooth * Time.deltaTime);


            if (positionConfig.distanceFromTarget > positionConfig.maxZoom) {
                positionConfig.distanceFromTarget = positionConfig.maxZoom;
                positionConfig.newDistance = positionConfig.maxZoom;
            }

            if (positionConfig.distanceFromTarget < positionConfig.minZoom) {
                positionConfig.distanceFromTarget = positionConfig.minZoom;
                positionConfig.newDistance = positionConfig.minZoom;
            }
        }


        [global::System.Serializable]
        public class CollisionHandler {

            public LayerMask collisionLayer;
            public float collsionSpaceSize = 3.41f;

            [HideInInspector]
            public bool colliding = false;
            [HideInInspector]
            public Vector3[] adjustedCameraClipPoints;
            [HideInInspector]
            public Vector3[] desiredCameraClipPoints;

            private UnityEngine.Camera camera;

            public void initialize(UnityEngine.Camera cam) {
                camera = cam;
                adjustedCameraClipPoints = new Vector3[5];
                desiredCameraClipPoints = new Vector3[5];
            }

            public void updateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray) {
                if (!camera)
                    return;

                //Clear the array
                intoArray = new Vector3[5];

                float z = camera.nearClipPlane;
                float x = Mathf.Tan(camera.fieldOfView / collsionSpaceSize) * z;
                float y = x / camera.aspect;

                //Array Points
                //Top left
                intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition; // added and rotated the point relative to camera
                //Top right
                intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition;
                //bottom left
                intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition;
                //bottom right
                intoArray[3] = (atRotation * new Vector3(x, -y, z)) + cameraPosition;
                //camera's position
                intoArray[4] = cameraPosition - camera.transform.forward;


            }

            private bool collisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition) {
                for (int i = 0; i < clipPoints.Length; i++) {
                    Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
                    float distance = Vector3.Distance(clipPoints[i], fromPosition);

                    if (Physics.Raycast(ray, distance, collisionLayer))
                        return true;
                }

                return false;
            }



            public float getAdjustedDistanceWithRayFrom(Vector3 from) {
                float distance = -1;

                for (int i = 0; i < desiredCameraClipPoints.Length; i++) {
                    Ray ray = new Ray(from, desiredCameraClipPoints[i] - from);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit)) {
                        if (distance == -1) {
                            distance = hit.distance;
                        }
                        else {
                            if (hit.distance < distance)
                                distance = hit.distance;
                        }
                    }
                }

                if (distance == -1)
                    return 0;
                else
                    return distance;
            }

            public void checkColliding(Vector3 targetPosition) {

                if (collisionDetectedAtClipPoints(desiredCameraClipPoints, targetPosition))
                    colliding = true;
                else
                    colliding = false;
            }
        }
    }
}
