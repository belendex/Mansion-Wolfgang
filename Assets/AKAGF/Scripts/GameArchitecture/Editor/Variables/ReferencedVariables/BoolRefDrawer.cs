using AKAeditor;
using AKAGF.GameArchitecture.ScriptableObjects.Variables;
using AKAGF.GameArchitecture.ScriptableObjects.Variables.ReferencedVariables;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BoolRef))]
public class BoolRefDrawer : BasePropertyDrawer<BoolVar>{ }

