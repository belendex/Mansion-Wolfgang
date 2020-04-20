using AKAGF.GameArchitecture.MonoBehaviours.SceneControl;
using AKAGF.GameArchitecture.MonoBehaviours.Controllers;
using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.GUI
{
    public class MenuController : MonoBehaviour {

        public void NewGame(string sceneName) {
            FindObjectOfType<SceneController>().FadeAndLoadScene(sceneName);
            FindObjectOfType<DiskStorageController>().newGame();
        }


        public void LoadGame(int index) {
            FindObjectOfType<PauseController>().pause();
            FindObjectOfType<DiskStorageController>().loadGame(index);
        }


        public void ResumeGame() {
            FindObjectOfType<PauseController>().pause();
        }


        public void Quit() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
        }


    }
}
