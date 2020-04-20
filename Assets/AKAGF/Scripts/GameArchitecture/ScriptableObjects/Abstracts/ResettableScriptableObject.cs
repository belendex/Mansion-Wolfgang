using UnityEngine;

// This class is used to enforce a Reset function on ScriptableObjects.
// It is used instead of an Interface as Interfaces do not serialize
// and therefore can't be shown in the inspector.
namespace AKAGF.GameArchitecture.ScriptableObjects.Abstracts
{
    public abstract class ResettableScriptableObject : ScriptableObject {
        public abstract void Reset ();
    }
}
