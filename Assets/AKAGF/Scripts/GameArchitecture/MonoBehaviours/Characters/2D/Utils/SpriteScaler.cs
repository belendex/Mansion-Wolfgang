using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Characters._2D.Utils
{
    public class SpriteScaler : MonoBehaviour {

        public Transform vanishingPoint;        //Reference of the vanishinPoint gameObject transform
        [Range(0.1f, 10)]
        public float scaleProportion = 1;       //Initial scale proportion
    
        [HideInInspector]
        protected float sizeFactor;             //Factor by which the character scale is multiplied
        public float scalingAcceleration = 1;   //Scale Acceleration factor
        protected Vector2 originalSize;         //Original size of the character before the scale calculation

        protected virtual void Awake() {
            if (!vanishingPoint) {
                Debug.LogError("No vanishingPoint GameObject Attached.");
                enabled = false;
                return;
            }

            originalSize = transform.localScale;
        }

        protected virtual void Update() {

            //Scale sprite 
            sizeFactor = scaleProportion * (1 - Mathf.Pow(transform.position.y / vanishingPoint.position.y, scalingAcceleration));
            Vector3 newScale = originalSize * sizeFactor;
            transform.localScale = new Vector3(newScale.x * Mathf.Sign(transform.localScale.x), newScale.y, newScale.z);
        }

    }
}
