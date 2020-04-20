using UnityEngine;

namespace AKAGF.GameArchitecture.ScriptableObjects.Variables {

    public abstract class ScriptableVariable<T> : ScriptableObject {

        [Multiline]
        public string DeveloperDescription = "";

        [SerializeField]
        private T Value;

        public T value {
            get { return Value; }
            set { this.Value = value; }
        }

        public override string ToString() {
            return Value.ToString();
        }

    }
}

