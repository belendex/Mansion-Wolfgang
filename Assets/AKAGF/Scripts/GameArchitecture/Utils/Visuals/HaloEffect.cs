using UnityEngine;

namespace AKAGF.GameArchitecture.Utils.Visuals
{
    public class HaloEffect : MonoBehaviour {
   
        // Range
        public bool changeRange = true;
        public float changeRangeSpeed = 2f;
        public float maxDistance = 1f;

        // Intensity
        public bool changeIntensity = true;
        public float changeIntensitySpeed = 2f;
        public float maxIntensity = 2f;

        private Light halo;
        private float timer;

        private void Start() {
            if (!(halo = GetComponent<Light>())) {
                Debug.LogError("No light component attached to the gameobject");
            }
        }

        void Update() {
        
            if (changeRange) {
                halo.range = Mathf.PingPong(timer * changeRangeSpeed, maxDistance);
            }

            if (changeIntensity) {
                halo.intensity = Mathf.PingPong(timer * changeIntensitySpeed, maxIntensity);
            }

            timer += (Time.deltaTime);

        }







    }
}
