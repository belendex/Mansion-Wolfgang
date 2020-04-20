using AKAGF.GameArchitecture.ScriptableObjects.Variables;
using AKAGF.GameArchitecture.ScriptableObjects.Variables.ReferencedVariables;
using UnityEditor;

[CustomPropertyDrawer(typeof(IntRef))]
public class IntRefDrawer : BasePropertyDrawer<IntVar> { }
