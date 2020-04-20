using System.Collections;
using AKAGF.GameArchitecture.MonoBehaviours.GUI;
using AKAGF.GameArchitecture.MonoBehaviours.Interaction;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Abstracts;
using UnityEngine.Events;

namespace AKAGF.GameArchitecture.ScriptableObjects.Interaction.Reactions.CallbackReactions {

    public class ConversationReaction : CallbackReaction{

        public VIDE_Assign dialogue;

        public OverrideNodeInfo[] nodeOverrideInfo;
        private DialogManager dialogController;
        public UnityEvent OnDialogStart;
        public UnityEvent OnDialogEnd;


        protected override void SpecificInit() {
            dialogController = FindObjectOfType<DialogManager>();
        }


        protected override void ImmediateReaction(ref Interactable publisher) {
            dialogController.setCurrentDialog(dialogue, nodeOverrideInfo);
            OnDialogStart.Invoke();

            if(waitForThisReaction)
            publisher.StartCoroutine(OnReactionEnd(publisher));
        }


        protected override IEnumerator OnReactionEnd(Interactable publisher) {
       
            while (dialogController.dialogActive)
                yield return null;

            OnDialogEnd.Invoke();
            publisher.reactionsEnded -= OnInteractionStart;
        }
    }
}
