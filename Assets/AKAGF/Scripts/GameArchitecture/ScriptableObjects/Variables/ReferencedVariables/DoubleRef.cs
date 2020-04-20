using System;

namespace AKAGF.GameArchitecture.ScriptableObjects.Variables.ReferencedVariables {

    [Serializable]
    public class DoubleRef  {

        public bool UseConstant = true;
        public double ConstantValue;
        public DoubleVar Variable;

        public DoubleRef() { }

        public DoubleRef(double value) {
            UseConstant = true;
            ConstantValue = value;
        }

        public double Value {
            get { return UseConstant ? ConstantValue : Variable.value; }
        }

        public static implicit operator double(DoubleRef reference) {
            return reference.Value;
        }
    }

}