using AKAGF.GameArchitecture.MonoBehaviours.Characters.Abstracts;
using AKAGF.GameArchitecture.MonoBehaviours.Input;
using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Characters._3D
{
    public class PlayerMovement3D_GPad : PlayerMovement3D {

        public enum MOVEMENT_TYPE { SELF_RELATIVE, CAMERA_RELATIVE }
        public MOVEMENT_TYPE movementType;
        public bool cameraRelativeMovement = true;
        private Rigidbody rigidBody;                    // The rigidbody attached to the player. Use for physics.
        public GamepadInput gamepadInput;


        private void Start() {
            rigidBody = GetComponent<Rigidbody>();
            gamepadInput = GetComponent<GamepadInput>();
        }


        public override void handleMovement(Vector3 direction) {
            switch (movementType) {
                case MOVEMENT_TYPE.SELF_RELATIVE:
                    handleMovementSelfRelative(direction);
                    break;

                case MOVEMENT_TYPE.CAMERA_RELATIVE:
                    handleMovementCameraRelative();
                    break;
            }
        }


        public override void handleRotation(Quaternion rotation) {
            switch (movementType) {
                case MOVEMENT_TYPE.SELF_RELATIVE:
                    handleRotationSelfRelative(rotation);
                    break;

                case MOVEMENT_TYPE.CAMERA_RELATIVE:
                    handleRotationCameraRelative();
                    break;
            }
        }


        #region SELF RELATIVE MOVEMENT METHODS
        public void handleMovementSelfRelative(Vector3 direction) {

            // Normal speed (walkspeed)
            float targetSpeed = walkSpeed;

            // Run button pressed, make the player runs
            if (gamepadInput.controlsScheme.runInput) {
                targetSpeed = runSpeed;
            }

            // Backward movement
            if (Mathf.Sign(gamepadInput.controlsScheme.vInput).Equals(-1)) {
                targetSpeed = (walkSpeed / 2) * -1;
            }

            // Forward Movement
            if (Mathf.Abs(gamepadInput.controlsScheme.vInput) > gamepadInput.directionalInputThreshold) {
                rigidBody.velocity = Mathf.Abs(gamepadInput.controlsScheme.vInput) * direction * targetSpeed;
            }
            else if (Mathf.Abs(gamepadInput.controlsScheme.hInput) > gamepadInput.directionalInputThreshold && Mathf.Abs(gamepadInput.controlsScheme.vInput) < gamepadInput.directionalInputThreshold) {

                rigidBody.velocity = Mathf.Abs(gamepadInput.controlsScheme.hInput) * direction * walkSpeed / 4;
            }
            else {
                rigidBody.velocity = Vector3.zero;
            }

            _currentSpeed = rigidBody.velocity.magnitude * Mathf.Sign(targetSpeed);
        }


        public void handleRotationSelfRelative(Quaternion rotation) {

            if (Mathf.Abs(gamepadInput.controlsScheme.hInput) > gamepadInput.directionalInputThreshold) {

                if (Mathf.Abs(currentSpeed) > turnSpeedThreshold)
                    rotation *= Quaternion.AngleAxis(turnSmoothing * gamepadInput.controlsScheme.hInput * Time.deltaTime, Vector3.up);
                else
                    rotation *= Quaternion.AngleAxis(turnSmoothing / 2 * gamepadInput.controlsScheme.hInput * Time.deltaTime, Vector3.up);


            }

            // Head the player towards the calculated rotation
            transform.rotation = rotation;
        }
        #endregion

        #region CAMERA RELATIVE MOVEMENT METHODS

        public void handleMovementCameraRelative() {

            // Normal speed (walkspeed)
            float targetSpeed = walkSpeed;

            // Run button pressed, make the player runs
            if (gamepadInput.controlsScheme.runInput)
                targetSpeed = runSpeed;

            if (Mathf.Abs(gamepadInput.controlsScheme.hInput) > gamepadInput.directionalInputThreshold ||
                Mathf.Abs(gamepadInput.controlsScheme.vInput) > gamepadInput.directionalInputThreshold) {

                rigidBody.velocity = getJoystickVector(cameraRelativeMovement) * targetSpeed ;
            }
            else {
                rigidBody.velocity = Vector3.zero;
            }

            _currentSpeed = rigidBody.velocity.magnitude * Mathf.Sign(targetSpeed);
        }


        public void handleRotationCameraRelative() {

            if (Mathf.Abs(gamepadInput.controlsScheme.hInput) > gamepadInput.directionalInputThreshold ||
                Mathf.Abs(gamepadInput.controlsScheme.vInput) > gamepadInput.directionalInputThreshold) {

            
                Quaternion newRotation = Quaternion.LookRotation(getJoystickVector(cameraRelativeMovement));
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * turnSmoothing);
            
            }
        }

        private Vector3 getJoystickVector(bool cameraRelative) {

            if (cameraRelative) {
                Vector3 forward = UnityEngine.Camera.main.transform.TransformDirection(Vector3.forward);
                forward.y = 0;
                forward = forward.normalized;
                Vector3 right = new Vector3(forward.z, 0, -forward.x);

                //Player graphic rotation
                return (gamepadInput.controlsScheme.hInput * right + gamepadInput.controlsScheme.vInput * forward).normalized;
            }
            else {
                return new Vector3(gamepadInput.controlsScheme.hInput, 0, gamepadInput.controlsScheme.vInput).normalized;
            }
        }


        #endregion
    }
}

