using AKAGF.GameArchitecture.MonoBehaviours.Interaction.Abstracts;
using UnityEngine.EventSystems;

namespace AKAGF.GameArchitecture.MonoBehaviours.Input.Interfaces
{
    public interface IWalkablePointNClick : IPointNClick {
        void OnWalkableLayerClick(PointerEventData data, int clickCount);
        void OnUnWalkableLayerClick(PointerEventData data, int clickCount);
        void OnReachableInteractableClick(InteractionTrigger interactable, int clickCount);
    }
}
