
namespace AKAGF.GameArchitecture.ScriptableObjects.Variables
{
    public class FloatVar : ScriptableVariable<float> {
        public void ApplyChange(float amount) { value += amount; }
        public void ApplyChange(FloatVar amount) { value += amount.value; }
    }
}
