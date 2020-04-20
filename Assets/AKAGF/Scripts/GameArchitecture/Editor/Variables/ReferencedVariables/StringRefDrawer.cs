using AKAGF.GameArchitecture.ScriptableObjects.Variables;
using AKAGF.GameArchitecture.ScriptableObjects.Variables.ReferencedVariables;
using UnityEditor;

[CustomPropertyDrawer(typeof(StringRef))]
public class StringRefDrawer : BasePropertyDrawer<StringVar> { }
