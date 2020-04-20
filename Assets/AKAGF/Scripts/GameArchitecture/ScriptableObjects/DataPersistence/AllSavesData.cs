
using AKAGF.GameArchitecture.ScriptableObjects.Abstracts;

namespace AKAGF.GameArchitecture.ScriptableObjects.DataPersistence
{
    public class AllSavesData : ResettableScriptableSingleton<AllSavesData> {

        public SaveData[] saveDatas;


        public override void Reset() {
            // If there are no conditions, do nothing.
            if (saveDatas == null)
                return;

            for (int i = 0; i < saveDatas.Length; i++) {
                saveDatas[i].Reset();
            }
        }

    }
}
