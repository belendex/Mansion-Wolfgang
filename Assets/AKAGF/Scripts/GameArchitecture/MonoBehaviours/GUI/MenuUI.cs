using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AKAGF.GameArchitecture.MonoBehaviours.GUI
{
    public class MenuUI : MonoBehaviour {

        public float transitionDelay = 0.5f;
        public bool overlapAnimations = false;
        private GameObject currentPanel;                  // Currently opened menu panel
        private GameObject previouslySelectedOption;      // The option selected in the previous panel to keep track of 
        public GameObject initialPanel;                   // Initialy opened menu panel
        public GameObject[] menuWindows;                  // All windows that menu is composed
        

        // When MenuUI is enabled, it opens the assigned Initial Window
        public void OnEnable() {
            if (initialPanel == null)
                return;

            OpenPanel(initialPanel);
        }


        // When MenuUI is disabled, it closes all windows that belongs to UI, including the current one
        private void OnDisable() {
            for (int i = 0; i < menuWindows.Length; i++) {

                checkForAnimableGUIElement(menuWindows[i], false);
            }

            CloseCurrent();
        }


        // Method that opens a given window or panel passed by parameter 
        // and close the rest of them. It also handles the selected option in 
        // each window or pannel 
        public void OpenPanel(GameObject panel) {

            // Panel already open
            if (currentPanel == panel) 
                return;
            
            // cache the previous panel
            previouslySelectedOption = EventSystem.current.currentSelectedGameObject;

            if (overlapAnimations) {
                CloseCurrent();
                currentPanel = panel;

                StartCoroutine(delayedCheckForAnimableGUIElement(currentPanel, true));

                GameObject go = FindFirstEnabledSelectable(panel);

                SetSelected(go);
            }
            else {

                StartCoroutine(OpenPanelAsync(panel));

            }
        }


        public IEnumerator OpenPanelAsync(GameObject panel) {

            yield return StartCoroutine(CloseCurrentAsync());
            currentPanel = panel;

            yield return StartCoroutine(delayedCheckForAnimableGUIElement(currentPanel, true));

            GameObject go = FindFirstEnabledSelectable(panel);

            SetSelected(go);
        }


        // Method that close the current opened window or panel and
        // set the previous selected option
        public void CloseCurrent() {

            if (currentPanel == null)
                return;

            if (overlapAnimations) {
                SetSelected(previouslySelectedOption);

                StartCoroutine(delayedCheckForAnimableGUIElement(currentPanel, false));
                currentPanel = previouslySelectedOption;
            }
            else {

                StartCoroutine(CloseCurrentAsync());
            }
        }


        public IEnumerator CloseCurrentAsync() {

            if (currentPanel == null) yield break;

            SetSelected(previouslySelectedOption);

            yield return StartCoroutine(delayedCheckForAnimableGUIElement(currentPanel, false));
            currentPanel = previouslySelectedOption;
        }


        private IEnumerator delayedCheckForAnimableGUIElement(GameObject panel, bool state) {

            AnimableGUIElement aux = panel.GetComponent<AnimableGUIElement>();

            if (aux) {

                aux.setState(state);

                // Wait until the animation is finished
                while (!aux.endAnimationsFlag)
                    yield return null;
            } 
            else panel.SetActive(state);

            // Extra time added between gui animated elements
            yield return new WaitForSeconds(transitionDelay);
        }


        private void checkForAnimableGUIElement(GameObject panel, bool state) {

            AnimableGUIElement aux = panel.GetComponent<AnimableGUIElement>();

            if (aux) aux.setState(state);
            else panel.SetActive(state);
        }


        // Static method that find the first selectable GUI element of a given Window or panel
        static GameObject FindFirstEnabledSelectable(GameObject panel) {
            GameObject go = null;
            var selectables = panel.GetComponentsInChildren<Selectable>(true);
            foreach (var selectable in selectables) {
                if (selectable.IsActive() && selectable.IsInteractable()){
                    go = selectable.gameObject;
                    break;
                }
            }
            return go;
        }

        // Method indicates to Unity Event System what is the current selected GUI element
        private void SetSelected(GameObject panel) {
            EventSystem.current.SetSelectedGameObject(panel);
        }
    }
}
