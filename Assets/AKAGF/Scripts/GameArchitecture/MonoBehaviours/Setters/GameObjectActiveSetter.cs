using AKAGF.GameArchitecture.ScriptableObjects.Variables.ReferencedVariables;
using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Setters {

    [ExecuteInEditMode]
    public class GameObjectActiveSetter : MonoBehaviour {
        [SerializeField]
        private bool updateInEditMode;
        public BoolRef state;
        public GameObject[] gameObjects;
        

        void Update() {
            if (Application.isPlaying || (updateInEditMode && Application.isEditor)) {
                syncValue();
            }
        }

        private void syncValue() {

            if (!state.UseConstant && !state.Variable) {
                Debug.LogError("BoolVar missing reference in: " + name);
                return;
            }


            for (int i = 0; i < gameObjects.Length; i++) {
                gameObjects[i].SetActive(state);
            }

        }


        public void setState(bool newState) {
            if (!state.UseConstant && !state.Variable) {
                Debug.LogError("BoolVar missing reference in: " + name);
                return;
            }

            state.ConstantValue = newState;

            if (state.Variable)
                state.Variable.value = newState;

        }
    }

}