using System;
using System.Collections;
using System.Collections.Generic;
using AKAGF.GameArchitecture.MonoBehaviours.GUI;
using AKAGF.GameArchitecture.ScriptableObjects.DataPersistence;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.ImmediateReactions;
using AKAGF.GameArchitecture.ScriptableObjects.SceneControl;
using UnityEngine;
using UnityEngine.SceneManagement;

// This script exists in the Persistent scene and manages the content
// based scene's loading.  It works on a principle that the
// Persistent scene will be loaded first, then it loads the scenes that
// contain the player and other visual elements when they are needed.
// At the same time it will unload the scenes that are not needed when
// the player leaves them.

namespace AKAGF.GameArchitecture.MonoBehaviours.SceneControl {
    [Serializable]
    public class SceneController : MonoBehaviour {

        public event Action BeforeSceneUnload;                  // Event delegate that is called just before a scene is unloaded.
        public event Action AfterSceneLoad;                     // Event delegate that is called just after a scene is loaded.

        public string startingSceneName;                 // The name of the scene that should be loaded first.
        public string initialStartingPositionName;       // The name of the StartingPosition in the first scene to be loaded.

        public SaveData playerSaveData;                  // Reference to the ScriptableObject which stores the name of the StartingPosition in the next scene.
        public CanvasFader sceneScreenFader;


        private IEnumerator Start() {

            // Set the initial alpha to start off with a black screen.
            StartCoroutine(sceneScreenFader.Fade(1f, 0f));

            if (!playerSaveData) {
                Debug.LogWarning("No PlayerSaveData Asset attached to SceneController.");
            }
            else if (!initialStartingPositionName.Equals("")) {
                // Write the initial starting position to the playerSaveData so it can be loaded by the player when the first scene is loaded.
                playerSaveData.Save(StartingPosition.startingPositionKey, initialStartingPositionName);
            }

            // Start the first scene loading and wait for it to finish.
            yield return StartCoroutine(FadeAndSwitchScenes(startingSceneName));

            // Once the scene is finished loading, start fading in.
            StartCoroutine(sceneScreenFader.Fade(0f));
        }


        // This is the main external point of contact and influence from the rest of the project.
        // This will be called by a SceneReaction when the player wants to switch scenes.
        public void FadeAndLoadScene(SceneReaction sceneReaction) {
            // If a fade isn't happening then start fading and switching scenes.
            if (!sceneScreenFader.isFading) {
                if (playerSaveData && !initialStartingPositionName.Equals("")) {
                    // Save the StartingPosition's name to the data asset.
                    playerSaveData.Save(StartingPosition.startingPositionKey, sceneReaction.startingPointInLoadedScene);
                }


                StartCoroutine(FadeAndSwitchScenes(sceneReaction.sceneName)) ;
                startingSceneName = sceneReaction.name;
                initialStartingPositionName = sceneReaction.startingPointInLoadedScene;
            }
        }

        public void FadeAndLoadScene(string sceneName) {
            // If a fade isn't happening then start fading and switching scenes.
            if (!sceneScreenFader.isFading) {
                StartCoroutine(FadeAndSwitchScenes(sceneName));
                startingSceneName = sceneName;
                initialStartingPositionName = null;
            }
        }

        public void FadeAndLoadScene(string sceneName, string startingPositionName) {
            // If a fade isn't happening then start fading and switching scenes.
            if (!sceneScreenFader.isFading) {
                playerSaveData.Save(StartingPosition.startingPositionKey, startingPositionName);

                StartCoroutine(FadeAndSwitchScenes(sceneName));
                startingSceneName = sceneName;
                initialStartingPositionName = startingPositionName;
            }
        }


        // This is the coroutine where the 'building blocks' of the script are put together.
        private IEnumerator FadeAndSwitchScenes(string sceneName) {
            // Start fading to black and wait for it to finish before continuing.
            yield return StartCoroutine(sceneScreenFader.Fade(1f));

            // If this event has any subscribers, call it.
            if (BeforeSceneUnload != null)
                BeforeSceneUnload();

            // Unload the current active scene only if it isn't the persistent scene
            if (SceneManager.GetActiveScene().buildIndex != 0) {
#if UNITY_5_5_OR_NEWER
                yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
#else
                SceneManager.UnloadScene (SceneManager.GetActiveScene ().buildIndex);
#endif
            }

            // Start loading the given scene and wait for it to finish.
            yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

            // If this event has any subscribers, call it.
            if (AfterSceneLoad != null) {
                AfterSceneLoad();
            }


            // Start fading back in and wait for it to finish before exiting the function.
            yield return StartCoroutine(sceneScreenFader.Fade(0f));
        }


        private IEnumerator LoadSceneAndSetActive(string sceneName) {
            // Allow the given scene to load over several frames and add it to the already loaded scenes (just the Persistent scene at this point).
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            // Find the scene that was most recently loaded (the one at the last index of the loaded scenes).
            Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            // Set the newly loaded scene as the active scene (this marks it as the one to be unloaded next).
            SceneManager.SetActiveScene(newlyLoadedScene);
        }

    }
}

#region TODO new Scene Controller
//namespace AKAGF.GameArchitecture.MonoBehaviours.SceneControl
//{
//    [Serializable]
//    public class SceneController : MonoBehaviour {

//        public event Action BeforeSceneUnload;                  // Event delegate that is called just before a scene is unloaded.
//        public event Action AfterSceneLoad;                     // Event delegate that is called just after a scene is loaded.

//        //public ScriptableScene startingScene;           // The name of the scene that should be loaded first.
//        public string initialStartingPositionName;       // The name of the StartingPosition in the first scene to be loaded.

//        public SaveData playerSaveData;                  // Reference to the ScriptableObject which stores the name of the StartingPosition in the next scene.
//        public CanvasFader sceneScreenFader;

//        public List<ScriptableScene> persistentScenes;
//        public List<ScriptableScene> gameScenes;


//        private IEnumerator Start () {

//            // Set the initial alpha to start off with a black screen.
//            StartCoroutine(sceneScreenFader.Fade(1f, 0f));

//            if (!playerSaveData) {
//                Debug.LogWarning("No PlayerSaveData Asset attached to SceneController.");
//            }
//            else if(!initialStartingPositionName.Equals("")) {
//                // Write the initial starting position to the playerSaveData so it can be loaded by the player when the first scene is loaded.
//                playerSaveData.Save(StartingPosition.startingPositionKey, initialStartingPositionName);
//            }

//            // Load all persistent scenes included in the list
//            for (int i = 0; i < persistentScenes.Count; i++) {
//                if(SceneManager.GetSceneByPath(persistentScenes[i].scenePath).isLoaded == false)
//                    yield return SceneManager.LoadSceneAsync(persistentScenes[i].scenePath, LoadSceneMode.Additive);
//            }

//            // Load all game starting scenes
//            for (int i = 0; i < gameScenes.Count; i++) {
//                    yield return loadGameSceneAdditive(gameScenes[i], false);
//            }


//            // Start the first scene loading and wait for it to finish.
//           // yield return StartCoroutine (FadeAndSwitchScenes(startingScene));

//            // Once the scene is finished loading, start fading in.
//            StartCoroutine (sceneScreenFader.Fade(0f));
//        }


//        private IEnumerator loadGameSceneAdditive(ScriptableScene scene, bool asyncLoad) {

//            if (asyncLoad) {
//                if (SceneManager.GetSceneByPath(scene.scenePath).isLoaded == false) {
//                    SceneManager.LoadSceneAsync(scene.scenePath, LoadSceneMode.Additive);

//                    yield return null;

//                    if (!gameScenes.Contains(scene))
//                        gameScenes.Add(scene);

//                    SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scene.scenePath));

//                    // If this event has any subscribers, call it.
//                    if (AfterSceneLoad != null) {
//                        AfterSceneLoad();
//                    }
//                }

//            }
//            else {
//                if (SceneManager.GetSceneByPath(scene.scenePath).isLoaded == false) {
//                    yield return SceneManager.LoadSceneAsync(scene.scenePath, LoadSceneMode.Additive);

//                    if (!gameScenes.Contains(scene))
//                        gameScenes.Add(scene);

//                    // Set the newly loaded scene as the active scene
//                    SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scene.scenePath));
//                    // If this event has any subscribers, call it.
//                    if (AfterSceneLoad != null) {
//                        AfterSceneLoad();
//                    }
//                }
//            }
//        }


//        private IEnumerator unloadGameScene(ScriptableScene scene, bool asyncLoad) {

//            if (asyncLoad) {
//                if (SceneManager.GetSceneByPath(scene.scenePath).isLoaded == true) {

//                    // If this event has any subscribers, call it.
//                    if (BeforeSceneUnload != null)
//                        BeforeSceneUnload();

//                    SceneManager.UnloadSceneAsync(SceneManager.GetSceneByPath(scene.scenePath));

//                    yield return null;

//                    if (gameScenes.Contains(scene))
//                        gameScenes.Remove(scene);
//                }

//            }
//            else {
//                if (SceneManager.GetSceneByPath(scene.scenePath).isLoaded == true) {

//                    // If this event has any subscribers, call it.
//                    if (BeforeSceneUnload != null)
//                        BeforeSceneUnload();

//                    yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneByPath(scene.scenePath));

//                    if (gameScenes.Contains(scene))
//                        gameScenes.Remove(scene);
//                }
//            }
//        }

//        private IEnumerator updateGameScenes(SceneReaction sceneReaction) {

//            // Start fading to black and wait for it to finish before continuing.
//            if (sceneReaction.fade) yield return StartCoroutine(sceneScreenFader.Fade(1f));





//            // Start fading back in and wait for it to finish before exiting the function.
//            if (sceneReaction.fade) yield return StartCoroutine(sceneScreenFader.Fade(0f));
//        }



//        public void updateLoadedScenes(SceneReaction sceneReaction) {
//            // If a fade isn't happening then start fading and switching scenes.
//            if (!sceneScreenFader.isFading) {
//                if (playerSaveData && !initialStartingPositionName.Equals("")) {
//                    // Save the StartingPosition's name to the data asset.
//                    playerSaveData.Save(StartingPosition.startingPositionKey, sceneReaction.startingPointInLoadedScene);
//                }

//                StartCoroutine(updateGameScenes(sceneReaction));
//                initialStartingPositionName = sceneReaction.startingPointInLoadedScene;

//            }
//        }



//        // This is the coroutine where the 'building blocks' of the script are put together.
//        private IEnumerator FadeAndSwitchScenes(string sceneName, bool fade = true) {
//            // Start fading to black and wait for it to finish before continuing.
//            if (fade) yield return StartCoroutine(sceneScreenFader.Fade(1f));

//            // If this event has any subscribers, call it.
//            if (BeforeSceneUnload != null)
//                BeforeSceneUnload();

//            // Unload the current active scene only if it isn't the persistent scene
//            //if (SceneManager.GetActiveScene().buildIndex != 0) {
//            //#if UNITY_5_5_OR_NEWER
//            //        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

//            //#else
//            //    SceneManager.UnloadScene (SceneManager.GetActiveScene ().buildIndex);
//            //#endif
//            //}



//            // Start loading the given scene and wait for it to finish.
//            yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

//            // If this event has any subscribers, call it.
//            if (AfterSceneLoad != null) {
//                AfterSceneLoad();
//            }


//            // Start fading back in and wait for it to finish before exiting the function.
//            if (fade) yield return StartCoroutine(sceneScreenFader.Fade(0f));
//        }

//        private IEnumerator LoadSceneAndSetActive(string sceneName) {
//            // Allow the given scene to load over several frames and add it to the already loaded scenes (just the Persistent scene at this point).
//            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

//            // Find the scene that was most recently loaded (the one at the last index of the loaded scenes).
//            Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

//            // Set the newly loaded scene as the active scene (this marks it as the one to be unloaded next).
//            SceneManager.SetActiveScene(newlyLoadedScene);
//        }

//        // This is the main external point of contact and influence from the rest of the project.
//        // This will be called by a SceneReaction when the player wants to switch scenes.
//        public void FadeAndLoadScene(SceneReaction sceneReaction) {
//            // If a fade isn't happening then start fading and switching scenes.
//            if (!sceneScreenFader.isFading) {
//                if (playerSaveData && !initialStartingPositionName.Equals("")) {
//                    // Save the StartingPosition's name to the data asset.
//                    playerSaveData.Save(StartingPosition.startingPositionKey, sceneReaction.startingPointInLoadedScene);
//                }


//                //StartCoroutine (FadeAndSwitchScenes (sceneReaction.sceneName, sceneReaction.fade));
//                //startingScene = sceneReaction.name;
//                initialStartingPositionName = sceneReaction.startingPointInLoadedScene;
//            }
//        }

//        public void FadeAndLoadScene(string sceneName) {
//            // If a fade isn't happening then start fading and switching scenes.
//            if (!sceneScreenFader.isFading) {
//                StartCoroutine(FadeAndSwitchScenes(sceneName));
//               // startingScene = sceneName;
//                initialStartingPositionName = null;
//            }
//        }

//        public void NoFadeLoadScene(string sceneName) {
//            // If a fade isn't happening then start fading and switching scenes.
//            if (!sceneScreenFader.isFading) {
//                StartCoroutine(FadeAndSwitchScenes(sceneName, false));
//               // startingScene = sceneName;
//                initialStartingPositionName = null;
//            }
//        }

//        public void FadeAndLoadScene(string sceneName, string startingPositionName, bool fade = true) {
//            // If a fade isn't happening then start fading and switching scenes.
//            if (!sceneScreenFader.isFading) {
//                playerSaveData.Save(StartingPosition.startingPositionKey, startingPositionName);

//                StartCoroutine(FadeAndSwitchScenes(sceneName, fade));
//                //startingScene = sceneName;
//                initialStartingPositionName = startingPositionName;
//            }
//        }

//    }
//}
#endregion