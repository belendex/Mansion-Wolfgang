using AKAGF.GameArchitecture.MonoBehaviours.Characters._2D.Player;
using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Characters._2D.Utils
{
    public class CharacterScaler2D : SpriteScaler {

        public PlayerMovement2D character;      //Reference of the movement script
    
        protected override void Awake() {

            base.Awake();

            if (!character) {
                Debug.LogError("No character GameObject Attached.");
                enabled = false;
                return;
            }
        }

        protected override void Update() {

            base.Update();
            //Multiply the character's speed by the same scale factor as scale.
            character.modifyCurrentSpeed(sizeFactor);
        }
    }
}



