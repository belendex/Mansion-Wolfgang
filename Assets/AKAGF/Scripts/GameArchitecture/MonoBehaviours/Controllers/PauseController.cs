using System;
using AKAGF.GameArchitecture.ScriptableObjects.Variables.ReferencedVariables;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace AKAGF.GameArchitecture.MonoBehaviours.Controllers {

    public enum PauseManagerState { Pausing, UnPausing, Idle, Paused }

    public sealed class PauseController : MonoBehaviour {
        public float pausedTimeScale = 0;
        public BoolRef pauseInput;
        public event Action Paused;
        public event Action Unpaused;
        public UnityEvent OnPause;
        public UnityEvent OnUnpause;

        private bool hardPause;

        [HideInInspector]
        public PauseManagerState state;
        
        public bool IsPaused {
            get {
                return (state == PauseManagerState.Paused);
            }
        }

        public void Pause() {
            //	The game will be paused at the start of the next update cycle.
            if (state != PauseManagerState.Paused) {
                state = PauseManagerState.Pausing;
            }
        }

        public void UnPause() {
            //	The game will be unpaused at the start of the next update cycle.
            if (state == PauseManagerState.Paused) {
                state = PauseManagerState.UnPausing;
            }
        }

        private void Awake() {
            state = PauseManagerState.Idle;
            hardPause = false;
            SceneManager.sceneLoaded += HandleLevelWasLoaded;
        }
        

        private void Update() {

            switch (state) {
                case PauseManagerState.Pausing:
                    Time.timeScale = pausedTimeScale;
                    state = PauseManagerState.Paused;
                    OnPause.Invoke();
                    //menuUI.SetActive(true);
                    RaisePausedEvent();
                break;

                case PauseManagerState.UnPausing:
                    Time.timeScale = 1.0f;
                    state = PauseManagerState.Idle;
                    OnUnpause.Invoke();
                    //menuUI.SetActive(false);
                    RaiseUnpausedEvent();
                break;
            }

            if (pauseInput)
            {
                pauseInput.ConstantValue = false;
                pause();
            }
        }

        public void pause() {
            if (state == PauseManagerState.Idle) {
                state = PauseManagerState.Pausing;
                hardPause = true;
            }
            else if (state == PauseManagerState.Paused) {
                if (hardPause) {
                    state = PauseManagerState.UnPausing;
                    hardPause = false;
                }
            }
        }

        private void HandleLevelWasLoaded(Scene scene, LoadSceneMode loadSceneMode) {
            if (state != PauseManagerState.Idle && loadSceneMode == LoadSceneMode.Single) {
                Time.timeScale = 1.0f;
                state = PauseManagerState.Idle;
            }
        }

        private void OnDestroy() {
            Paused = null;
            Unpaused = null;
            SceneManager.sceneLoaded -= HandleLevelWasLoaded;
        }

        private void OnApplicationQuit() {
            Paused = null;
            Unpaused = null;
        }

        private void RaisePausedEvent() {
            if (Paused != null)
                Paused();
        }

        private void RaiseUnpausedEvent() {
            if (Unpaused != null)
                Unpaused();
        }
    }
}