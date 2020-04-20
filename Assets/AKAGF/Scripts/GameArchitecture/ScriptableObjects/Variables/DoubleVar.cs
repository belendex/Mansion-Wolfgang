
namespace AKAGF.GameArchitecture.ScriptableObjects.Variables
{
    public class DoubleVar : ScriptableVariable<double> {
        public void ApplyChange(double amount) { value += amount; }
        public void ApplyChange(DoubleVar amount) { value += amount.value; }
    }
}
