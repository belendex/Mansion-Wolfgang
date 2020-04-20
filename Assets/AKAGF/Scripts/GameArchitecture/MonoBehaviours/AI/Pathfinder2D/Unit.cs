using System.Collections;
using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.AI.Pathfinder2D
{
    public abstract class Unit : MonoBehaviour {

        public Transform destinationPosition;
        public float pathRefreshRate = .25f;
        public float stoppingDistance = .5f;
        protected float currentSpeed;

        Vector2[] path;
        int targetIndex;

        protected virtual void Start() {
            StartCoroutine (RefreshPath ());
        }

        private IEnumerator RefreshPath() {
            Vector2 targetPositionOld = (Vector2)destinationPosition.position + Vector2.up; // ensure != to target.position initially
			
            while (true) {
                if (targetPositionOld != (Vector2)destinationPosition.position) {
                    targetPositionOld = destinationPosition.position;

                    path = Pathfinding.RequestPath (transform.position, destinationPosition.position);
                    StopCoroutine ("FollowPath");
                    StartCoroutine ("FollowPath");
                }

                yield return new WaitForSeconds (pathRefreshRate);
            }
        }
		
        private IEnumerator FollowPath() {
            if (path.Length > 0) {
                targetIndex = 0;
                Vector2 currentWaypoint = path [0];

                while (true) {
                    if ((Vector2)transform.position == currentWaypoint) {
                        targetIndex++;
                        if (targetIndex >= path.Length) {
                            if (Vector2.Distance(transform.position, destinationPosition.position) > stoppingDistance) {
                                while (true) {
                                    MoveToPoint(destinationPosition.position);
                                    yield return null;
                                }
                            }

                            yield break;
                        }
                        currentWaypoint = path [targetIndex];
                    }

                    MoveToPoint(currentWaypoint);
                    yield return null;
                }
            }
        }

        private void MoveToPoint(Vector2 destination) {
            transform.position = Vector2.MoveTowards(transform.position, destination, currentSpeed);
        }

        public void OnDrawGizmos() {
            if (path != null) {
                for (int i = targetIndex; i < path.Length; i ++) {
                    Gizmos.color = Color.red;
                    //Gizmos.DrawCube((Vector3)path[i], Vector3.one *.5f);

                    if (i == targetIndex) {
                        Gizmos.DrawLine(transform.position, path[i]);
                    }
                    else {
                        Gizmos.DrawLine(path[i-1],path[i]);
                    }
                }
            }
        }
    }
}
