using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Characters.Abstracts
{
    public abstract class PlayerMovement3D : MonoBehaviour {

        protected float _currentSpeed;
        public float currentSpeed {
            get { return _currentSpeed; }
        }
        public float runSpeed = 6;                              // Running speed (max speed for player)
        public float walkSpeed = 3;                             // Walking speed
        public float turnSmoothing = 15f;                       // The amount of smoothing applied to the player's turning using spherical interpolation.
        public float speedDampTime = 0.1f;                      // The approximate amount of time it takes for the speed parameter to reach its value upon being set.
    
        public float turnSpeedThreshold = 0.5f;                 // The speed beyond which the player can move and turn normally.


        /*All classes inherit from this must implement these methods*/
        public abstract void handleMovement(Vector3 velocity);
        public abstract void handleRotation(Quaternion rotation);
    }
}
