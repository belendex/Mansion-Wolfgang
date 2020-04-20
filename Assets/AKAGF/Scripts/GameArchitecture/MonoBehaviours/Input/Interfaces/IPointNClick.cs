using AKAGF.GameArchitecture.MonoBehaviours.Interaction.Abstracts;
using UnityEngine.EventSystems;

namespace AKAGF.GameArchitecture.MonoBehaviours.Input.Interfaces
{
    public interface IPointNClick {
        
        void OnImmediateInteractableClick(InteractionTrigger interactable, int clickCount);
    }
}
