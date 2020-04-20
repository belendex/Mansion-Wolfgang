using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Characters._2D.Utils
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteOverlappingController : MonoBehaviour {

        SpriteRenderer sprite;      //Reference to SpriteRenderer component

        void Start () {
            sprite = GetComponent<SpriteRenderer>();
        }
	
        void Update () {
            sprite.sortingOrder = (int)-transform.position.y;
        }
    }
}
