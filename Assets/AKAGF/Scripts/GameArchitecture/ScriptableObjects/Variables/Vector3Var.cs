
using UnityEngine;

namespace AKAGF.GameArchitecture.ScriptableObjects.Variables
{
    public class Vector3Var : ScriptableVariable<Vector3> {
        public void ApplyChange(Vector3 amount) { value += amount; }
        public void ApplyChange(Vector3Var amount) { value += amount.value; }
    }
}
