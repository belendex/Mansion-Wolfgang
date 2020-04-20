using System;

namespace AKAGF.GameArchitecture.ScriptableObjects.Variables.ReferencedVariables {

    [Serializable]
    public class FloatRef  {

        public bool UseConstant = true;
        public float ConstantValue;
        public FloatVar Variable;

        public FloatRef() {}

        public FloatRef(float value) {
            UseConstant = true;
            ConstantValue = value;
        }

        public float Value {
            get { return UseConstant ? ConstantValue : Variable.value; }
        }

        public static implicit operator float(FloatRef reference) {
            return reference.Value;
        }
    }

}