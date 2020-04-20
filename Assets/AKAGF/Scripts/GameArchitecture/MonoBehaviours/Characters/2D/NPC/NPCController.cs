using AKAGF.GameArchitecture.MonoBehaviours.Characters._2D.Player;
using AKAGF.GameArchitecture.MonoBehaviours.Interaction.Abstracts;
using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Characters._2D.NPC
{
    [RequireComponent(typeof(PlayerMovement2D))]
    [RequireComponent(typeof(PlayerAnimation2D))]
    public class NPCController : MonoBehaviour {

        public PolygonCollider2D walkableLayer;     //TODO randomize the interactables Position
        private PlayerMovement2D npcMovement;
        private PlayerAnimation2D npcAnimation;

        private int currentInteractableIndex;                                // Interactable Index the npc is currently headed.
        private InteractionTrigger currentInteractable;                      // Interactable GameObject the npc is currently headed.

        public bool fixedStartingPosition = false;                           // The NPC has to start at a precise position when level is loaded?
        public GameObject levelStartingPosition;                             // If fixedstartingPoint is enabled, set the position for NPC to start  

        public bool randomWaitingTime;                                       // Active/Deactive randomWaitingTime for the NPC                
        public float secondsAtWaypoint;                                      // Time to wait for the npc at each waypoint. (Read-only if randomWaitingTime is enabled)
        public float min, max;                                               // Range of seconds between the randomize time will be.
        private float nextMoveTime;                                          // The precise time when the npc will be allowed to try to move again      

        public bool randomWaypointOrder = false;                             // The secuence of waypoints will be always the same or will it be randomized?

        public InteractionTrigger[] interactableList = new InteractionTrigger[0];        // List of the different interactable objects the npc can interact with

        private void Start() {
            if (!walkableLayer) {
                Debug.LogError("No walkable Layer reference found.");
                enabled = false;
                return;
            }

            // We force the Gameobject to have these components attacched to it, so no need to check the references
            npcMovement = GetComponent<PlayerMovement2D>();
            npcAnimation = GetComponent<PlayerAnimation2D>();

            if (fixedStartingPosition) {
                transform.position = levelStartingPosition.transform.position;
            }
        }

        private void Update() {

            //handleMovement
            npcMovement.handleMovement();
            //handleAnimation
            npcAnimation.handleAnimation(npcMovement.destinationPosition.position, npcMovement.isMoving, npcMovement.velocity);

            checkInteraction();
            calculateWaypoint();
        }

        private void checkInteraction() {
            if (currentInteractable != null && transform.position == currentInteractable.interactionLocation.position) {
                //We are exactly at the reaction position
                Debug.Log("NPC interacting");
                nextMoveTime = Time.time + secondsAtWaypoint;
                //nextMoveTime += interactableList[currentInteractableIndex].React();     //Must be executed after Interaction, otherwhise totalReactionsTime would be 0
                currentInteractable = null;
            }
        }

        private void calculateWaypoint() {
            // If time has passed, try to get another waypoint to make the npc goes to it.
            if (Time.time > nextMoveTime && currentInteractable == null && interactableList.Length > 0) {
                //Try to move
                Debug.Log("inside calculating waypoint");
                int nextIndex;

                if (randomWaypointOrder) {
                    do {
                        nextIndex = Random.Range(0, interactableList.Length);
                    } while (nextIndex == currentInteractableIndex);
                }
                else {
                    //No randomized behaviour, so move to next waypoint
                    nextIndex = (currentInteractableIndex + 1) % interactableList.Length;
                }

                currentInteractableIndex = nextIndex;
                //Set the destination position to start moving the player towards it
                currentInteractable = interactableList[currentInteractableIndex];
                npcMovement.destinationPosition = currentInteractable.interactionLocation;

                // And if time is randomized, get a new waiting time
                if (randomWaitingTime) {
                    secondsAtWaypoint = Random.Range(min, max);
                }
            }
        }
    }
}
