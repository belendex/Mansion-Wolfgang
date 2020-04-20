using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Characters._3D
{
    class PlayerAnimation3D : MonoBehaviour{

        public Animator animator;                 // Reference to the animator component.

        //Animator Params
        private readonly int hashSpeedPara = Animator.StringToHash("Speed");
        // An hash representing the Speed animator parameter, this is used at runtime in place of a string.
        private readonly int hashLocomotionTag = Animator.StringToHash("Locomotion");
        // An hash representing the Locomotion tag, this is used at runtime in place of a string.


        public void handleAnimation(float speed, float speedDampTime) {
            // Set the animator's Speed parameter based on the (possibly modified) speed that the nav mesh agent wants to move at.
            animator.SetFloat(hashSpeedPara, speed, speedDampTime, Time.deltaTime);
        }


        public bool isInLocomotion() {
            return animator.GetCurrentAnimatorStateInfo(0).tagHash == hashLocomotionTag;
        }
    }
}

