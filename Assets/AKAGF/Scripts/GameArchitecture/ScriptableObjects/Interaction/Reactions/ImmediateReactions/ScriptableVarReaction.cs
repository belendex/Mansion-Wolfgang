using AKAGF.GameArchitecture.MonoBehaviours.Interaction;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Abstracts;
using AKAGF.GameArchitecture.ScriptableObjects.Variables;

namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.ImmediateReactions {

    public enum VarType {
        FLOAT, INT, DOUBLE, BOOL, STRING
    }

    public class ScriptableVarReaction : Reaction {

        public VarType varType;

        public FloatVar floatVar;
        public float floatValue;

        public IntVar intVar;
        public int intValue;

        public DoubleVar doubleVar;
        public double doubleValue;

        public BoolVar boolVar;
        public bool boolValue;

        public StringVar stringVar;
        public string stringValue;


        protected override void ImmediateReaction(ref Interactable publisher) {
            switch (varType) {

                case VarType.FLOAT: floatVar.value = floatValue;
                    break;

                case VarType.INT: intVar.value = intValue;
                    break;

                case VarType.DOUBLE: doubleVar.value = doubleValue;
                    break;

                case VarType.BOOL: boolVar.value = boolValue;
                    break;

                case VarType.STRING: stringVar.value = stringValue;
                    break;

            }
        }
    }
}

