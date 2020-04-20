using AKAGF.GameArchitecture.ScriptableObjects.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace AKAGF.GameArchitecture.MonoBehaviours.Setters {

    [ExecuteInEditMode]
    public class SliderSetter : MonoBehaviour {
        public Slider Slider;
        public FloatVar Variable;

        private void Update() {
            if (Slider != null && Variable != null)
                Slider.value = Variable.value;
        }
    }
}