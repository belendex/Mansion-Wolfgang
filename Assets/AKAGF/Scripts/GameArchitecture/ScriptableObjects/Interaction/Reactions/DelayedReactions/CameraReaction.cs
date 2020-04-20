using AKAGF.GameArchitecture.MonoBehaviours.Camera;
using AKAGF.GameArchitecture.MonoBehaviours.Interaction;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Abstracts;

namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.DelayedReactions
{
    public class CameraReaction : DelayedReaction {

        public CameraMasterController cameraController;
        public float OnAnimationEndDelay = 1f;
        public CameraAnimation[] animations;


        protected override void ImmediateReaction(ref Interactable publisher) {
            cameraController.OnAnimationCall(animations, OnAnimationEndDelay);
        }

    }
}
