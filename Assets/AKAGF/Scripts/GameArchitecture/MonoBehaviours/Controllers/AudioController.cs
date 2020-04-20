using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using AKAGF.GameArchitecture.MonoBehaviours.Setters;

namespace AKAGF.GameArchitecture.MonoBehaviours.Controllers {

    public enum AUDIO_COMMAND { PLAY, STOP, PAUSE, UNPAUSE }

    public class AudioController : MonoBehaviour {

        private static List<AudioChannel> audioChannels;

        // Use this for initialization
        void Awake () {

            // Get all the audioMixer parameters setters attached to the gameobject
            AudioMixerParamSetter[] mixerParams = GetComponents<AudioMixerParamSetter>();
            audioChannels = new List<AudioChannel>(mixerParams.Length);

            // Init all audio channels (one for each audioMixer group)
            for (int i = 0; i < mixerParams.Length; i++) {

                audioChannels.Add(new AudioChannel());

                audioChannels[i].paramSetter = mixerParams[i];
                audioChannels[i].audioSource = gameObject.AddComponent<AudioSource>();
                audioChannels[i].audioSource.outputAudioMixerGroup = mixerParams[i].MixerGroup;
                audioChannels[i].isChannelFading = false;
                audioChannels[i].audioSource.volume = 0f;
                audioChannels[i].currentState = AUDIO_COMMAND.STOP;
            }
        }


        void Update() {
            for (int i = 0; i < audioChannels.Count; i++) {
                audioChannels[i].fadeTrack();
                
            }
            
        }


        public void playTrack(AudioClip clip, AudioMixerGroup mixerGroup, bool loop, bool overrideCurrentClip, float maxVolume = 1f, bool fade = false, float fadeTime = 1f) {

            int channelIndex = audioChannels.FindIndex(x => x.paramSetter.MixerGroup == mixerGroup);
            bool isPlaying = audioChannels[channelIndex].audioSource.isPlaying;


            // Exit the function if override isn't allowed and the audio channel is already playing a clip
            if (!overrideCurrentClip && isPlaying || clip == audioChannels[channelIndex].audioSource.clip) {
                return;
            }

            audioChannels[channelIndex].currentState = AUDIO_COMMAND.PLAY;

            audioChannels[channelIndex].audioSource.loop = loop;
            audioChannels[channelIndex].maxVolume = maxVolume;

            // Fade sound
            if (fade && fadeTime > 0f) {

                // The channel is already playing a clip, so fade out.
                if (isPlaying) {
                    audioChannels[channelIndex].targetVolume = 0f;
                    audioChannels[channelIndex].nextClip = clip;

                }
                // No clip is playing, fade in.
                else {
                    audioChannels[channelIndex].audioSource.clip = clip;
                    audioChannels[channelIndex].targetVolume = maxVolume;
                    audioChannels[channelIndex].audioSource.Play();
                }

                audioChannels[channelIndex].fadeTime = fadeTime;
                audioChannels[channelIndex].isChannelFading = true;
                
            }
            // No sound currently fading or override blocked, 
            // so just directly change the clip, fading is not marked
            else {
                // Mute the source
                audioChannels[channelIndex].audioSource.volume = 0f;

                // Change the clip, loop based on func parameter
                audioChannels[channelIndex].audioSource.clip = clip;

                // Unmute
                audioChannels[channelIndex].audioSource.volume = maxVolume;

                audioChannels[channelIndex].audioSource.Play(); 
            }  
        }


        public void stopTrack(AudioMixerGroup mixerGroup, bool fade = false, float fadeTime = 1f) {

            int channelIndex = audioChannels.FindIndex(x => x.paramSetter.MixerGroup == mixerGroup);
            audioChannels[channelIndex].currentState = AUDIO_COMMAND.STOP;

            if (fade) {
                audioChannels[channelIndex].fadeTime = fadeTime;
                audioChannels[channelIndex].targetVolume = 0f;
                audioChannels[channelIndex].isChannelFading = true;
               
            }
            else {
                audioChannels[channelIndex].audioSource.Stop();
                audioChannels[channelIndex].audioSource.volume = 0f;
            }
        }


        public void pauseTrack(AudioMixerGroup mixerGroup, bool fade = false, float fadeTime = 1f) {
            int channelIndex = audioChannels.FindIndex(x => x.paramSetter.MixerGroup == mixerGroup);
            audioChannels[channelIndex].currentState = AUDIO_COMMAND.PAUSE;

            if (fade) {
                audioChannels[channelIndex].fadeTime = fadeTime;
                audioChannels[channelIndex].targetVolume = 0f;
                audioChannels[channelIndex].isChannelFading = true;

            }
            else {
                audioChannels[channelIndex].audioSource.Pause();
                audioChannels[channelIndex].audioSource.volume = 0f;
            }
        }

        public void unPauseTrack(AudioMixerGroup mixerGroup, bool fade = false, float fadeTime = 1f) {
            int channelIndex = audioChannels.FindIndex(x => x.paramSetter.MixerGroup == mixerGroup);
            audioChannels[channelIndex].currentState = AUDIO_COMMAND.UNPAUSE;

            if (fade) {
                audioChannels[channelIndex].fadeTime = fadeTime;
                audioChannels[channelIndex].isChannelFading = true;

            }
            else {
                audioChannels[channelIndex].audioSource.volume = audioChannels[channelIndex].maxVolume;
                audioChannels[channelIndex].audioSource.UnPause();
                audioChannels[channelIndex].currentState = AUDIO_COMMAND.PLAY;
            }

        }

        [Serializable]
        public class AudioChannel {
            public AudioMixerParamSetter paramSetter;
            public AudioSource audioSource;
            public AudioClip nextClip;
            public bool isChannelFading;
            [Range(0f, 1f)]
            public float maxVolume;
            [Range(0f, 1f)]
            public float targetVolume;
            public float fadeTime;
            public AUDIO_COMMAND currentState;

            public void fadeTrack() {

                if (!isChannelFading)
                    return;

                // Fade out
                if (targetVolume <= audioSource.volume) {
                    audioSource.volume -= Time.deltaTime/fadeTime;

                    if (audioSource.volume <= 0) {

                        audioSource.volume = 0;

                        if (nextClip) {
                            audioSource.clip = nextClip;
                            nextClip = null;
                        }

                        switch (currentState) {
                            case AUDIO_COMMAND.PLAY:
                                targetVolume = maxVolume;
                                audioSource.Play();
                                break;

                            case AUDIO_COMMAND.STOP:
                                audioSource.Stop();
                                isChannelFading = false;
                                break;

                            case AUDIO_COMMAND.PAUSE:
                                audioSource.Pause();
                                isChannelFading = false;
                                break;

                            case AUDIO_COMMAND.UNPAUSE:
                                targetVolume = maxVolume;
                                audioSource.UnPause();
                                currentState = AUDIO_COMMAND.PLAY;
                                isChannelFading = true;
                                break;

                        }
                    }

                }
                // Fade in
                else if (targetVolume > audioSource.volume) {
                    audioSource.volume +=  Time.deltaTime / fadeTime;
                    if (audioSource.volume >= targetVolume) {
                        audioSource.volume = maxVolume;
                        isChannelFading = false;
                    }
                    
                }
            }
        }
    }
}
