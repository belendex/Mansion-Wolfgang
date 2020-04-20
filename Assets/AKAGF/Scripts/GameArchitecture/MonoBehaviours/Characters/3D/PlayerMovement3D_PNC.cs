using AKAGF.GameArchitecture.MonoBehaviours.Characters.Abstracts;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_5_5_OR_NEWER

#endif

namespace AKAGF.GameArchitecture.MonoBehaviours.Characters._3D
{
    public class PlayerMovement3D_PNC : PlayerMovement3D {

        public NavMeshAgent agent;                              // Reference to the nav mesh agent component.
        public Transform destinationPosition;                   // The position that is currently being headed towards, this is the interactionLocation of the currentInteractable if it is not null.

        public float stoppingDistance = .15f;                   // This is a hack for Unity version 5.6. Misteriosly agent.stoppingDistance property is modified from inside the mesh agent implemented by unity, so the scritp behaves in strange ways
        public float slowingSpeed = 0.175f;                     // The speed the player moves as it reaches close to it's destination.
        private const float stopDistanceProportion = 0.1f;      // The proportion of the nav mesh agent's stopping distance within which the player stops completely.
        private const float navMeshSampleDistance = 4f;         // The maximum distance from the nav mesh a click can be to be accepted.

        private void Start() {
            // The player will be rotated by this script so the nav mesh agent should not rotate it.
            agent.updateRotation = false;

            agent.speed = runSpeed;

            // Set the initial destination as the player's current position.
            destinationPosition.position = transform.position;
        }

        public override void handleRotation(Quaternion targetRotation) {
            // If the nav mesh agent is currently waiting for a path, do nothing.
            if (agent.pathPending)
                return;

            // Cache the speed that nav mesh agent wants to move at.
            _currentSpeed = agent.desiredVelocity.magnitude;

            // If the nav mesh agent is very close to it's destination, call the Stopping function.
            if (agent.remainingDistance <= stoppingDistance * stopDistanceProportion)
                Stopping(out _currentSpeed);
            // Otherwise, if the nav mesh agent is close to it's destination, call the Slowing function.
            else if (agent.remainingDistance <= stoppingDistance)
                Slowing(out _currentSpeed, agent.remainingDistance, targetRotation);
            // Otherwise, if the nav mesh agent wants to move fast enough, call the Moving function.
            else if (_currentSpeed > turnSpeedThreshold)
                Moving();
        }

        public override void handleMovement(Vector3 velocity) {

            agent.velocity = velocity;
        }

        // This is called when the nav mesh agent is very close to it's destination.
        private void Stopping(out float speed) {
        

            // Set the player's position to the destination.
            transform.position = destinationPosition.position;

            // Stop the nav mesh agent from moving the player.
            stopAgent(true);

            // Set the speed (which is what the animator will use) to zero.
            speed = 0f;
        }


        // This is called when the nav mesh agent is close to its destination but not so close it's position should snap to it's destination.
        private void Slowing(out float speed, float distanceToDestination, Quaternion targetRotation) {
            // Although the player will continue to move, it will be controlled manually so stop the nav mesh agent.
            stopAgent(true);

            // Find the distance to the destination as a percentage of the stopping distance.
            float proportionalDistance = 1f - distanceToDestination / stoppingDistance;

            // Interpolate the player's rotation between itself and the target rotation based on how close to the destination the player is.
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, proportionalDistance);

            // Move the player towards the destination by an amount based on the slowing speed.
            transform.position = Vector3.MoveTowards(transform.position, destinationPosition.position, slowingSpeed * Time.deltaTime);

            // Set the speed (for use by the animator) to a value between slowing speed and zero based on the proportional distance.
            speed = Mathf.Lerp(slowingSpeed, 0f, proportionalDistance);
        }


        // This is called when the player is moving normally.  In such cases the player is moved by the nav mesh agent, but rotated by this function.
        private void Moving() {
            // Create a rotation looking down the nav mesh agent's desired velocity.
            Quaternion targetRotation = Quaternion.LookRotation(agent.desiredVelocity);

            // Interpolate the player's rotation towards the target rotation.
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSmoothing * Time.deltaTime);
        }

        public Vector3 getPointFromMesh(Vector3 pointToCheck) {
            NavMeshHit hit;
            Vector3 checkedPoint;
            if (NavMesh.SamplePosition(pointToCheck, out hit, navMeshSampleDistance, NavMesh.AllAreas))
                checkedPoint = hit.position;
            else
                // In the event that the nearest position cannot be found, set the position as the world position of the click.
                checkedPoint = pointToCheck;

            return checkedPoint;
        }

        public void setDestination(Vector3 destination) {
        
            destinationPosition.position = new Vector3(destination.x, agent.baseOffset + destination.y , destination.z);

            // Set the destination of the nav mesh agent to the found destination position and start the nav mesh agent go to it.
            agent.SetDestination(destinationPosition.position);

            // start the nav mesh agent going
            stopAgent(false);
        }

        private void stopAgent(bool stop) {
#if UNITY_5_5_OR_NEWER
            agent.isStopped = stop;
#else
            if(stop)
                agent.Stop();  
             else
                agent.Resume();  
#endif
        }
    }
}

