using AKAGF.GameArchitecture.MonoBehaviours.Interaction;
using AKAGF.GameArchitecture.MonoBehaviours.Controllers;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Abstracts;
using UnityEngine;
using UnityEngine.Audio;

// This Reaction is used to play sounds through a given AudioSource.
// Since the AudioSource itself handles delay, this is a Reaction
// rather than an DelayedReaction.
namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.ImmediateReactions {

    public class AudioReaction : DelayedReaction {

        public AUDIO_COMMAND audioCommand;

        public AudioMixerGroup audioSource;
        public AudioClip audioClip;         // The AudioClip to be played.
        [Range(0f, 1f)]
        public float volume = 1f;
        public bool overrideCurrentSound;
        public bool loopSound;
        public bool fadeSound;
        public float fadeTime = 1f;

        private AudioController audioController;

        protected override void SpecificInit() {
            audioController = FindObjectOfType<AudioController>();
        }

        protected override void ImmediateReaction(ref Interactable publisher) {

            switch (audioCommand) {
                case AUDIO_COMMAND.PLAY:
                    audioController.playTrack(audioClip, audioSource, loopSound, overrideCurrentSound, volume, fadeSound, fadeTime);
                    break;

                case AUDIO_COMMAND.STOP:
                    audioController.stopTrack(audioSource, fadeSound, fadeTime);
                    break;

                case AUDIO_COMMAND.PAUSE:
                    audioController.pauseTrack(audioSource, fadeSound, fadeTime);
                    break;

                case AUDIO_COMMAND.UNPAUSE:
                    audioController.unPauseTrack(audioSource, fadeSound, fadeTime);
                    break;
            }  
        }
    }

}