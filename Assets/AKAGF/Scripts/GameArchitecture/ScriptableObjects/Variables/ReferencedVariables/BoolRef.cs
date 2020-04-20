using System;

namespace AKAGF.GameArchitecture.ScriptableObjects.Variables.ReferencedVariables {

    [Serializable]
    public class BoolRef {

        public bool UseConstant = true;
        public bool ConstantValue;
        public BoolVar Variable;

        public BoolRef() {}

        public BoolRef(bool value) {
            UseConstant = true;
            ConstantValue = value;
        }

        public bool Value {
            get { return UseConstant ? ConstantValue : Variable.value; }
        }

        public static implicit operator bool(BoolRef reference) {
            return reference.Value;
        }
    }

}