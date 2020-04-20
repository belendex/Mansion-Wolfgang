using System.Collections;
using System.Collections.Generic;
using AKAGF.GameArchitecture.MonoBehaviours.Controllers;
using AKAGF.GameArchitecture.Utils.Text;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

// This class is used to manage the text that is
// displayed on screen.  In situations where many
// messages are triggered one after another it
// makes sure they are played in the correct order.
namespace AKAGF.GameArchitecture.MonoBehaviours.GUI
{

    [global::System.Serializable]
    public class Message {
        public string message;                      // The text to be displayed to the screen.
        public Color textColor = Color.white;       // The color of the text when it's displayed (different colors for different characters talking).
        public AudioClip audioClip;
        public UnityEvent OnMessageStart;
        public UnityEvent OnTextAnimationEnd;
        public UnityEvent OnMessageEnd;
    }


    public class TextManager : MonoBehaviour {

        public enum TEXT_DISPLAY_MODE {BY_CHARACTER, BY_WORD, FULL_TEXT}

        public UITextElementWrapper textWrapper;
        [Tooltip("The text display mode.")]
        public TEXT_DISPLAY_MODE displayMode; 
        [Tooltip("The amount of time that each character in a message should be displayed for.")]
        public float displayTimePerCharacter = 0.1f;
        [Tooltip("The additional time that is added at the end of each message displaying.")]
        public float additionalDisplayTime = 0.5f;
        [Tooltip("The initial time delay that is added before the start of the first message displaying.")]
        public float firstMessageDisplayDelay = 1f;
        [Tooltip("The final time delay that is added after the end of the last message displaying.")]
        public float lastMessageDisplayDelay = 1f;
        [Tooltip("Will be the text fade In at the beginning of the message? (only valid for displaying full_text)")]
        public bool fadeIn;                             
        [Tooltip("Will be the text fade Out at the end of the message? (apply to all display modes)")]
        public bool fadeOut;
        public float fadeDuration = 1f;
        public float finalAlpha = 1f;
        [Tooltip("Will be player input feedback required to load the next message?")]
        public bool loadNextMessageByInput;
        [Tooltip("Will be the player allowed to skip the text animation?")]
        public bool allowSkipAnimationByInput;
        public string inputAxis = "Action";

        public AudioMixerGroup audioSource;

        [Tooltip("Event raised before the first message will be displayed.")]
        public UnityEvent OnFirstMessage;
        [Tooltip("Event raised after the last message will be displayed.")]
        public UnityEvent OnLastMessage;
        [Tooltip("Event raised just before the required input to the player in order to load the next message.")]
        public UnityEvent OnMessageInputRequiredStart;
        [Tooltip("Event raised just after input given by the player to load the next message.")]
        public UnityEvent OnMessageInputRequiredEnd;

        private float initialDisplayTimePerCharacter;
        private bool isFading;
        private bool isDisplayingText;
        private List<Message> instructions = new List<Message> ();
        private AudioController audioController;

        private bool firstMessage;
        private bool skipAnimation;
        private bool loadNextMessage;
        public bool blocked { get; private set; }       // Allow new messages to add in the list
        public bool fixText { get; set; }

        private void Awake() {
            audioController = FindObjectOfType<AudioController>();
            initialDisplayTimePerCharacter = displayTimePerCharacter;
        }

        private void Update () {

            // If there are instructions and the time is beyond the start time of the first instruction...
            if (instructions.Count > 0 ) {

                handleInput();

                if (isFading)
                    return;

                StartCoroutine(displayText());
            }
            else if(!isFading){
                textWrapper.setText(string.Empty);
                textWrapper.setAlpha(1f);
                blocked = false;
            }
        }


        private void handleInput() {

            if (allowSkipAnimationByInput && !skipAnimation) {
                if (UnityEngine.Input.GetButtonDown(inputAxis)) {
                    skipAnimation = true;
                    loadNextMessage = false;
                }
            }

            else if (loadNextMessageByInput && !loadNextMessage) {
                if (UnityEngine.Input.GetButtonDown(inputAxis)) {
                    loadNextMessage = true;
                    skipAnimation = false;
                }
            }

            else if(!skipAnimation && loadNextMessage) {
                loadNextMessage = false;
                skipAnimation = false;
            }

        }


        // This function is called from TextReactions in order to display a message to the screen.
        public void DisplayMessage (Message message, bool block=true) {

            // If TextManager are blocked don't allow new messages
            if (blocked) {
                Debug.LogWarning("You are trying to add new messages to a blocked TextManager: " + name);
                return;
            }
                
            // Block the text manager to avoid add new messages
            // and for ensuring the raise of the OnFirstMessageEvent
            blocked = block;

      
            // First message
            if (instructions.Count == 0) {
                OnFirstMessage.Invoke();
                firstMessage = true;
            }

            // Add the new instruction to the collection.
            instructions.Add (message);
        }

        private IEnumerator displayText(){

            if (isDisplayingText) yield break;

            isDisplayingText = true;

            if (instructions[0].OnMessageStart != null)
                instructions[0].OnMessageStart.Invoke();

            textWrapper.setTextColor(instructions[0].textColor);

            textWrapper.setAlpha(1f);

            if (firstMessage) {
                yield return new WaitForSeconds(firstMessageDisplayDelay);
                firstMessage = false;
            }

            playSound();

            switch (displayMode) {
                case TEXT_DISPLAY_MODE.BY_CHARACTER:
                   yield return StartCoroutine(displayCharByChar());
                    break;
                case TEXT_DISPLAY_MODE.BY_WORD:
                    yield return StartCoroutine(displayWordByWord());
                    break;
                case TEXT_DISPLAY_MODE.FULL_TEXT:
                    yield return StartCoroutine(displayFullText());
                    break;
            }


            if (instructions[0].OnTextAnimationEnd != null)
                instructions[0].OnTextAnimationEnd.Invoke();

            if (loadNextMessageByInput) {

                OnMessageInputRequiredStart.Invoke();

                while (!loadNextMessage)
                    yield return null;

                OnMessageInputRequiredEnd.Invoke();
            }

            // All message text displayed
            while (fixText)
                yield return null;

            skipAnimation = false;
            loadNextMessage = false;

            if (fadeOut) {
               yield return Fade(0, fadeDuration);
            }


            if (instructions[0].OnMessageEnd != null)
                instructions[0].OnMessageEnd.Invoke();

            // Then remove the instruction.
            instructions.RemoveAt(0);

            if (instructions.Count == 0) {
                yield return new WaitForSeconds(lastMessageDisplayDelay);
                OnLastMessage.Invoke();
            }

            isDisplayingText = false;
        }


        private IEnumerator displayFullText() {

            // Full text
            textWrapper.setText(instructions[0].message);

            if (fadeIn) {
                textWrapper.setAlpha(0f);
                yield return Fade(finalAlpha, fadeDuration);
            }

            // Calculate how long the message should be displayed based on the number of characters.
            float displayDuration = StringExt.RichTextLength(instructions[0].message) * displayTimePerCharacter + additionalDisplayTime;

            while (displayDuration > 0 && !skipAnimation) {
                displayDuration -= Time.deltaTime;
                yield return null;
            }

            // End Full text
        }


        private IEnumerator displayCharByChar() {

            // Display char by char 
            var maker = new RichTextSubStringMaker(instructions[0].message);
            
            while (maker.IsConsumable() && !skipAnimation) {
                maker.Consume();
                textWrapper.setText(maker.GetRichText());
                yield return new WaitForSeconds(displayTimePerCharacter);
            }


            textWrapper.setText(instructions[0].message);

            float timeToWait = additionalDisplayTime;

            while (timeToWait > 0f && !skipAnimation) {
                timeToWait -= Time.deltaTime;
                yield return null;
            }

            // End char by char
        }


        private IEnumerator displayWordByWord() {

            // Word by word
            string[] array = instructions[0].message.Split(' ');
            textWrapper.setText(array[0]);

            yield return new WaitForSeconds(StringExt.RichTextLength(array[0]) * displayTimePerCharacter);

            for (int i = 1; i < array.Length && !skipAnimation; i++) {
                textWrapper.appendText(" " + array[i]);
                yield return new WaitForSeconds(StringExt.RichTextLength(array[i]) * displayTimePerCharacter);
            }

            float timeToWait = additionalDisplayTime;

            while (timeToWait > 0f && !skipAnimation) {
                timeToWait -= Time.deltaTime;
                yield return null;
            }

            textWrapper.setText(instructions[0].message);

            // End word by word

        }


        private void playSound() {
            if (instructions[0].audioClip != null) {
                if (!audioController) {
                    Debug.LogWarning("No audiocontroller found in scene.");
                }
                else if (!audioSource) {
                    Debug.LogWarning("No audiomixer reference found in: " + name);
                }
                else {

                    displayTimePerCharacter = instructions[0].audioClip.length /
                                              StringExt.RichTextLength(instructions[0].message);

                    audioController.playTrack(instructions[0].audioClip, audioSource, false, true);
                }
            }
            else {
                displayTimePerCharacter = initialDisplayTimePerCharacter;
            }
        }


        private IEnumerator Fade (float finalAlpha, float fadeDuration = 1f) {

            if (isFading) yield break;
            // Set the fading flag to true so the FadeAndSwitchScenes coroutine won't be called again.
            isFading = true;

            // Calculate how fast the CanvasGroup should fade based on it's current alpha, it's final alpha and how long it has to change between the two.
            float fadeSpeed = Mathf.Abs (textWrapper.getAlpha() - finalAlpha) / fadeDuration;

            // While the CanvasGroup hasn't reached the final alpha yet...
            while (!Mathf.Approximately (textWrapper.getAlpha(), finalAlpha) && !skipAnimation) {

                // ... move the alpha towards it's target alpha.
                textWrapper.setAlpha(Mathf.MoveTowards(textWrapper.getAlpha(), finalAlpha,
                    fadeSpeed * Time.deltaTime));
            
                // Wait for a frame then continue.
                yield return null;
            }

            textWrapper.setAlpha(finalAlpha);
           
            // Set the flag to false since the fade has finished.
            isFading = false;

        }
    }
}

