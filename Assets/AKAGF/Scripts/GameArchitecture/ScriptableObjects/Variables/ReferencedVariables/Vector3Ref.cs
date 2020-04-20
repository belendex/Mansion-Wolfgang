using System;
using UnityEngine;

namespace AKAGF.GameArchitecture.ScriptableObjects.Variables.ReferencedVariables {

    [Serializable]
    public class Vector3Ref {

        public bool UseConstant = true;
        public Vector3 ConstantValue;
        public Vector3Var Variable;

        public Vector3Ref() {}

        public Vector3Ref(Vector3 value) {
            UseConstant = true;
            ConstantValue = value;
        }

        public Vector3 Value {
            get { return UseConstant ? ConstantValue : Variable.value; }
        }

        public static implicit operator Vector3(Vector3Ref reference) {
            return reference.Value;
        }
    }

}