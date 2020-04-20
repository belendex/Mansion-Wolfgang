using System;

namespace AKAGF.GameArchitecture.ScriptableObjects.Variables.ReferencedVariables {

    [Serializable]
    public class StringRef  {

        public bool UseConstant = true;
        public string ConstantValue;
        public StringVar Variable;

        public StringRef(){ }

        public StringRef(string value) {
            UseConstant = true;
            ConstantValue = value;
        }

        public string Value {
            get { return UseConstant ? ConstantValue : Variable.value; }
        }

        public static implicit operator string(StringRef reference) {
            return reference.Value;
        }
    }

}