using UnityEngine;
using UnityEngine.SceneManagement;

namespace AKAGF.GameArchitecture.MonoBehaviours.SceneControl.Map
{
    public class MapLocation : MonoBehaviour {

        public string sceneToLoadName;
        public string startingPositionName;
        public GameObject locationImage;
        public GameObject locationBckg;
   

        public void goToScene() {
            if (!SceneManager.GetActiveScene().name.Equals(sceneToLoadName)) {
                FindObjectOfType<SceneController>().FadeAndLoadScene(sceneToLoadName, startingPositionName);
            }
        }

        public void setActive(bool active) {
            locationImage.SetActive(active);

        }
    }
}
