using AKAGF.GameArchitecture.ScriptableObjects.Variables.ReferencedVariables;
using UnityEngine;


namespace AKAGF.GameArchitecture.MonoBehaviours.Setters {

    public enum AnimatorParamType { FLOAT, INT, BOOL, TRIGGER }

    [ExecuteInEditMode]
    public class AnimatorParamSetter : MonoBehaviour {

        public bool updateInEditMode;
        public AnimatorParamType paramType;

        [Tooltip("Scriptable Float Reference to read from and send to the Animator as the specified parameter.")]
        public FloatRef floatVar;

        [Tooltip("Scriptable Int Reference to read from and send to the Animator as the specified parameter.")]
        public IntRef intVar;

        [Tooltip("Scriptable Bool Reference to read from and send to the Animator as the specified parameter.")]
        public BoolRef boolVar;

        [Tooltip("Animator to set parameters on.")]
        public Animator Animator;

        [Tooltip("Name of the parameter to set with the value of Variable.")]
        public string ParameterName;

        /// <summary>
        /// Animator Hash of ParameterName, automatically generated.
        /// </summary>
        private int parameterHash;

        private void OnValidate() {
            parameterHash = Animator.StringToHash(ParameterName);
        }

        private void Update() {

            if (Application.isPlaying || (updateInEditMode && Application.isEditor)) {
                set();
            }
        }

        public void set() {

            switch (paramType) {
                case AnimatorParamType.BOOL:
                    Animator.SetBool(parameterHash, boolVar);
                    break;

                case AnimatorParamType.TRIGGER:
                    if (boolVar == true) {
                        Animator.SetTrigger(parameterHash);
                        boolVar.ConstantValue = false;

                        if (boolVar.Variable)
                            boolVar.Variable.value = false;
                    }

                    break;

                case AnimatorParamType.FLOAT:
                    Animator.SetFloat(parameterHash, floatVar);
                    break;

                case AnimatorParamType.INT:
                    Animator.SetInteger(parameterHash, intVar);
                    break;
            }
        }
    }

}