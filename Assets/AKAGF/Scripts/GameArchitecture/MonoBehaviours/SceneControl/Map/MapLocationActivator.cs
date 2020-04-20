using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.SceneControl.Map
{
    public class MapLocationActivator : MonoBehaviour {

        public string locationName;
        public bool enable;

        // Use this for initialization
        void Start () {
            FindObjectOfType<MapController>().activateLocation(locationName, enable);
            gameObject.SetActive(false);
        }
    }
}
