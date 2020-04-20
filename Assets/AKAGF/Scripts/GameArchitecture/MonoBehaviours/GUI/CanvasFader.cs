using System.Collections;
using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.GUI
{
    public class CanvasFader : MonoBehaviour {

        public CanvasGroup faderCanvasGroup;            // The CanvasGroup that controls the Image used for fading to black.
        public bool isFading { get; private set; }                        // Flag used to determine if the Image is currently fading to or from black.
	    public float localFadeDuration { private get;  set; }

        public void fade(float finalAlpha) {
            StartCoroutine(Fade(finalAlpha, localFadeDuration));
        }

        public void fadeIn(float time) {
            StartCoroutine(Fade(1f, time));
        }

        public void fadeOut(float time) {
            StartCoroutine(Fade(0f, time));
        }

        public IEnumerator Fade (float finalAlpha, float fadeDuration = 1f) {
            // Set the fading flag to true so the FadeAndSwitchScenes coroutine won't be called again.
            isFading = true;

            // Make sure the CanvasGroup blocks raycasts into the scene so no more input can be accepted.
            faderCanvasGroup.blocksRaycasts = true;

            // Calculate how fast the CanvasGroup should fade based on it's current alpha, it's final alpha and how long it has to change between the two.
            float fadeSpeed = Mathf.Abs (faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

            // While the CanvasGroup hasn't reached the final alpha yet...
            while (!Mathf.Approximately (faderCanvasGroup.alpha, finalAlpha)) {
                // ... move the alpha towards it's target alpha.
                faderCanvasGroup.alpha = Mathf.MoveTowards (faderCanvasGroup.alpha, finalAlpha,
                    fadeSpeed * Time.deltaTime);

                // Wait for a frame then continue.
                yield return null;
            }

            // Set the flag to false since the fade has finished.
            isFading = false;

            // Stop the CanvasGroup from blocking raycasts so input is no longer ignored.
            faderCanvasGroup.blocksRaycasts = false;
        }
    }
}
