using AKAGF.GameArchitecture.MonoBehaviours.AI.Pathfinder2D;
using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Characters._2D.Player
{
    public class PlayerMovement2D : Unit {

        public float maxSpeed;
        // private float currentSpeed;
        private float hSpeedSmoothing;                  //Smoothing factor of the character speed
        [HideInInspector]
        public Vector2 velocity;
        private Vector3 previousPos;
        public float verticalSpeedDivider = 2f;
        public float verticalSpeedLimit = 0.1f;

        [HideInInspector]
        public bool isMoving;


        protected override void Start() {

            velocity = Vector2.zero;
            previousPos = transform.position;
            // Set the initial destination as the player's current position.
            destinationPosition.position = transform.position;

            base.Start();
        }

        public void handleMovement() {
            if (Vector2.Distance(transform.position, destinationPosition.position) < stoppingDistance) {
                Stopping();
            }
            else {
                Moving();
            }

            velocity = (transform.position - previousPos) / Time.deltaTime;
            previousPos = transform.position;

            //float step = currentSpeed * Time.deltaTime;
            //transform.position = Vector3.MoveTowards(transform.position, destinationPosition.position, step);

        }

        private void Stopping() {
            calculateSpeed(0, .1f);
            transform.position = destinationPosition.position;
            isMoving = false;
        }

        private void Moving() {
            float fc = 1;
            //Clamping velocity to three decimals. This avoid vertical/horizontal sprite flickering in animations
            decimal vy = (decimal.Round((decimal)Mathf.Abs(velocity.y), 3));
            decimal vx = (decimal.Round((decimal)Mathf.Abs(velocity.x), 3));

            if ((vy - vx > (decimal)verticalSpeedLimit))
                fc = verticalSpeedDivider;

            calculateSpeed(maxSpeed / fc, .1f);
            isMoving = true;
        }

        private void calculateSpeed(float targetSpeed, float acceleration) {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref hSpeedSmoothing, acceleration ) ;
        }

        public void modifyCurrentSpeed(float factor) {

            if (isMoving) {
                currentSpeed *= Mathf.Abs(factor);
            }
        }
    }
}
