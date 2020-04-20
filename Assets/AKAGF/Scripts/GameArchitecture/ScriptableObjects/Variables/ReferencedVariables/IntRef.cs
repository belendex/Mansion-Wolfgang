using System;

namespace AKAGF.GameArchitecture.ScriptableObjects.Variables.ReferencedVariables {

    [Serializable]
    public class IntRef  {

        public bool UseConstant = true;
        public int ConstantValue;
        public IntRef Variable;

        public IntRef() { }

        public IntRef(int value) {
            UseConstant = true;
            ConstantValue = value;
        }

        public int Value {
            get { return UseConstant ? ConstantValue : Variable.Value; }
        }

        public static implicit operator int(IntRef reference) {
            return reference.Value;
        }
    }

}