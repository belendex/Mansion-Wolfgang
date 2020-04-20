// The SceneReaction is used to change between scenes.
// Though there is a delay while the scene fades out,
// this is done with the SceneController class and so
// this is just a Reaction not a DelayedReaction.

using AKAGF.GameArchitecture.MonoBehaviours.Interaction;
using AKAGF.GameArchitecture.MonoBehaviours.SceneControl;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Abstracts;
using AKAGF.GameArchitecture.ScriptableObjects.SceneControl;
using System.Collections.Generic;

namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.ImmediateReactions
{
    public class SceneReaction : Reaction {

        public string sceneName;                    // The name of the scene to be loaded.
        public string startingPointInLoadedScene;   // The name of the StartingPosition in the newly loaded scene.
        public bool fade = true;
        //public bool asyncLoad;
        //public bool singleSceneMode;

        //public List<ScriptableScene> scenesToUnload;
        //public List<ScriptableScene> scenesToLoad;

        private SceneController sceneController;    // Reference to the SceneController to actually do the loading and unloading of scenes.


        protected override void SpecificInit() {
            sceneController = FindObjectOfType<SceneController>();
        }


        protected override void ImmediateReaction(ref Interactable publisher) {
            // Start the scene loading process.
            sceneController.FadeAndLoadScene(this);
        }
    }
}