using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Camera {

    [global::System.Serializable]
    public class CameraAnimation {

        public bool skippableByPlayer;
        public float time;
        public CameraMovementSettings movementConfig;
        [Range(-1, 1)]
        public float orbitHinput = 0;
        [Range(-1, 1)]
        public float orbitVinput = 0;
        public CameraOrbitSettings orbitConfig;
    }
}


