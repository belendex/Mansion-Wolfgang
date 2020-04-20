using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Characters._2D.Player
{
    public class PlayerAnimation2D : MonoBehaviour {

        public Animator animator;                   // Reference to the animator component.
        public float verticalAnimationSpeedLimit = .1f;

        //Animator Params
        private readonly int speedParamHash = Animator.StringToHash("Speed");           // An hash representing the Speed animator parameter, this is used at runtime in place of a string.
        private readonly int WalkInParamHash = Animator.StringToHash("Walkin");
        private readonly int WalkOutParamHash = Animator.StringToHash("Walkout");
        //Animator Tags
        private readonly int hashLocomotionTag = Animator.StringToHash("Locomotion");   // An hash representing the Locomotion tag, this is used at runtime in place of a string.

        private void Start () {
            animator = GetComponent<Animator>();
            if (!animator) {
                Debug.LogError("No animator component attached.");
                enabled = false;
                return;
            }
        }

        public void handleAnimation(Vector3 destinationPosition, bool isMoving, Vector2 velocity) {

            //Horizontal sprite flipping management
            if ((destinationPosition.x < transform.position.x && Mathf.Sign(transform.localScale.x) != -1)
                || (destinationPosition.x > transform.position.x && Mathf.Sign(transform.localScale.x) == -1)) {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }

            if (isMoving) {
                //Clamping velocity to three decimals. This avoid sidewalk sprite flickering
                decimal vy = (decimal.Round((decimal)Mathf.Abs(velocity.y), 3));
                decimal vx = (decimal.Round((decimal)Mathf.Abs(velocity.x), 3));

                //4 direction animations management
                if ( vy - vx > (decimal)verticalAnimationSpeedLimit) {
                    if (destinationPosition.y < transform.position.y) {
                        animator.SetBool(WalkOutParamHash, true);
                        animator.SetBool(WalkInParamHash, false);
                    }
                    else {
                        animator.SetBool(WalkOutParamHash, false);
                        animator.SetBool(WalkInParamHash, true);
                    }
                }
                else {
                    animator.SetBool(WalkOutParamHash, false);
                    animator.SetBool(WalkInParamHash, false);
                }
            }
            else {
                animator.SetBool(WalkOutParamHash, false);
                animator.SetBool(WalkInParamHash, false);
            }

            animator.SetFloat(speedParamHash, velocity.magnitude);
        }

        public bool isInLocomotion() {
            return animator.GetCurrentAnimatorStateInfo(0).tagHash == hashLocomotionTag;
        }
    }
}
