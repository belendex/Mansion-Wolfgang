using AKAGF.GameArchitecture.ScriptableObjects.Variables;
using AKAGF.GameArchitecture.ScriptableObjects.Variables.ReferencedVariables;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FloatRef))]
public class FloatRefDrawer : BasePropertyDrawer<FloatVar> { }