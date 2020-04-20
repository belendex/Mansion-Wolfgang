

namespace AKAGF.GameArchitecture.ScriptableObjects.Variables
{
    public class IntVar : ScriptableVariable<int> {
        public void ApplyChange(int amount) { value += amount; }
        public void ApplyChange(IntVar amount) { value += amount.value; }
    }
}
