using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Input
{
    public class GamepadInput : MonoBehaviour {

        [global::System.Serializable]
        public struct GamepadScheme {
            [HideInInspector]
            public float hInput, vInput;    // Horizontal and vertical Input from Gamepad left joystick
            [HideInInspector]
            public bool runInput;           // Is the run button pressed?
            [HideInInspector]
            public bool actionInput;        // Is the action button pressed?

            public string horizontalAxisInputName;
            public string verticalAxisInputName;
            public string runButtomName;
            public string actionButtomName;

            public void resetInput() {
                hInput = vInput = 0;
                runInput = actionInput = false;
            }
        }

        /* Unity InputManager Config*/
        public GamepadScheme controlsScheme = new GamepadScheme();
        public float directionalInputThreshold = .1f;   // How much left joystick must be moved to make the player starts moving
        public bool rawDirGamepadInput = false;         // Directional Input values are discrete?


        public void Update() {
            // Movement
            if (rawDirGamepadInput) {
                controlsScheme.hInput = UnityEngine.Input.GetAxisRaw(controlsScheme.horizontalAxisInputName);
                controlsScheme.vInput = UnityEngine.Input.GetAxisRaw(controlsScheme.verticalAxisInputName);
            }
            else {
                controlsScheme.hInput = UnityEngine.Input.GetAxis(controlsScheme.horizontalAxisInputName);
                controlsScheme.vInput = UnityEngine.Input.GetAxis(controlsScheme.verticalAxisInputName);
            }

            // Run
            controlsScheme.runInput = UnityEngine.Input.GetButton(controlsScheme.runButtomName);

            // Action
            controlsScheme.actionInput = UnityEngine.Input.GetButton(controlsScheme.actionButtomName);
        }  
    }
}
