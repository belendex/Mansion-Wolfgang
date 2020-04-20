using AKAGF.GameArchitecture.ScriptableObjects.Variables.ReferencedVariables;
using UnityEngine;
using UnityEngine.Audio;

namespace AKAGF.GameArchitecture.MonoBehaviours.Setters {

    public class AudioMixerParamSetter : MonoBehaviour {

        public AudioMixerGroup MixerGroup;
        public string ParameterName = "";
        public FloatRef refValue;


        private void Update() {

            float dB = 0;

            if (refValue.UseConstant) {
                dB = refValue.Value > 0.0f ?
                20.0f * Mathf.Log10(refValue.Value) :
                -80.0f;
            }
            else {
                dB = refValue.Variable.value > 0.0f ?
                20.0f * Mathf.Log10(refValue.Variable.value) :
                -80.0f;
            }
            

            MixerGroup.audioMixer.SetFloat(ParameterName, dB);
        }
    }
}
