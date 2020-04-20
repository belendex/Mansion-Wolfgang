using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VIDE_Data;

namespace AKAGF.GameArchitecture.MonoBehaviours.GUI
{
    [Serializable]
    public struct OverrideNodeInfo {
        public int numberOfInteractions;
        public int nodeId;
        public string description;
    }


    public class DialogManager : MonoBehaviour {

        public UnityEvent<VD.NodeData> VIDEeventData;

        private bool dialoguePaused = false; //Custom variable to prevent the manager from calling VD.Next
        public bool dialogActive { get; private set; }

        public UnityEvent OnDialogueStart;
        //public UnityEvent OnNodeChange;
        public UnityEvent OnDialogueEnd;

        [Header("Player Dialogue Decission Options")]
        public bool temporizedDecision = false;             // Player decision comments will be temporized?
        [Range(1, 60)]
        public int decissionTime = 15;                     // Total decision time for the player to pick an option
        public int defaultDecissionIndex;

        private int initialDecisionTime;                  // Variable to store the initial value of the decision time to get back to it in case the value is modified by VIDE extravars
        private bool initialTemporizedDecision;             // Variable to store the initial value of temporized decision to get back to it if the value is modified by VIDE extravars
        private bool isDeciding;

        public UnityEvent OnCountDownStart;

        [Serializable]
        public class CountDownEvent : UnityEvent<float>{  } 
        public CountDownEvent OnCountDown;

        public UnityEvent OnCountDownEnd;

        void Awake(){
            VD.LoadDialogues(); //Load all dialogues to memory so that we dont spend time doing so later
            //An alternative to this can be preloading dialogues from the VIDE_Assign component!
        }

        void Start() {
            initialTemporizedDecision = temporizedDecision;
            initialDecisionTime = decissionTime;
        }

        //Call this to begin the dialogue and advance through it
        public void Interact(VIDE_Assign dialogue, OverrideNodeInfo[] nodeOverrideInfo = null) {
            //Sometimes, we might want to check the ExtraVariables and VAs before moving forward
            //We might want to modify the dialogue or perhaps go to another node, or dont start the dialogue at all
            //In such cases, the function will return true
            var doNotInteract = PreConditions(dialogue);
            if (doNotInteract) return;

            if(nodeOverrideInfo != null)
                SpecialStartNodeOverrides(ref dialogue, nodeOverrideInfo);

            if (!VD.isActive) {
                Begin(dialogue);
            }
            else{
                CallNext();
            }
        }

        //Perfect way to change the flow of the conversation
        private static void SpecialStartNodeOverrides(ref VIDE_Assign diagToLoad, OverrideNodeInfo[] dialogInfo) {
            for (int i = 0; i < dialogInfo.Length; i++) {
                if (diagToLoad.interactionCount == dialogInfo[i].numberOfInteractions || dialogInfo[i].numberOfInteractions == -1) {
                    diagToLoad.overrideStartNode = dialogInfo[i].nodeId;
                    return;
                }
            }
        }

        //This begins the conversation. 
        private void Begin(VIDE_Assign dialogue) {

            //Subscribe to events
            VD.OnActionNode += ActionHandler;
            //VD.OnNodeChange += diagUI.UpdateUI;
            VD.OnEnd += EndDialogue;
            VD.BeginDialogue(dialogue); //Begins dialogue, will call the first OnNodeChange

            OnDialogueStart.Invoke();
            dialogActive = true;
        }
    
        //Calls next node in the dialogue
        public void CallNext() {

            if (!dialoguePaused) {
                VD.Next(); //We call the next node and populate nodeData with new data. Will fire OnNodeChange.
               // OnNodeChange.Invoke();

                if (VD.nodeData != null && VD.nodeData.isPlayer && temporizedDecision)
                    StartCoroutine(countDown());
            }
            else
            {
                //Stuff we can do instead if dialogue is paused
            }
        }

        //If not using local input, then the UI buttons are going to call this method when you tap/click them!
        //They will send along the choice index
        public void SelectChoice(int choice) {

            isDeciding = false;

            VD.nodeData.commentIndex = choice;

            Interact(VD.assigned);
            
        }

        // called from ConversationReaction
        public void setCurrentDialog(VIDE_Assign newDialog, OverrideNodeInfo[] nodeOverrideInfo) {
            Interact(newDialog, nodeOverrideInfo);
        }

        static bool PreConditions(VIDE_Assign assigned) {
            var data = VD.nodeData;

            if (VD.isActive) {
                if (!data.isPlayer) {

                }
                else
                {

                }
            }
            else {

            }

            return false;
        }

        //Conditions we check after VD.Next was called but before we update the UI
        public static void PostConditions(VD.NodeData data) {

            //Don't conduct extra variable actions if we are waiting on a paused action
            if (data.pausedAction) return;


            if (!data.isPlayer) {

            }
            else {

            }
        }

        //Called when dialogues are finished loading
        static void OnLoadedAction() {
            //Debug.Log("Finished loading all dialogues");
            VD.OnLoaded -= OnLoadedAction;
        }

        //Another way to handle Action Nodes is to listen to the OnActionNode event, which sends the ID of the action node
        static void ActionHandler(int actionNodeID) {
            //Debug.Log("ACTION TRIGGERED: " + actionNodeID.ToString());
        }

        //Unsuscribe from everything, disable UI, and end dialogue
        //Called automatically because we subscribed to the OnEnd event
        private void EndDialogue(VD.NodeData data) {
            VD.OnActionNode -= ActionHandler;
            //VD.OnNodeChange -= diagUI.UpdateUI;
            VD.OnEnd -= EndDialogue;
            VD.EndDialogue();

            OnDialogueEnd.Invoke();
            dialogActive = false;
        }

        //To prevent errors
        void OnDisable() {
            EndDialogue(null);
        }

        private IEnumerator countDown() {

            if (isDeciding)
                yield break;

            isDeciding = true;

            OnCountDownStart.Invoke();

            float startDecisionTime = decissionTime;

            float countDownTimer = decissionTime;


            while (countDownTimer > 0 && isDeciding) {
                countDownTimer -= Time.deltaTime;
                OnCountDown.Invoke(countDownTimer/decissionTime); // Normalized left time
                yield return null;
            }

            if (isDeciding) {
               SelectChoice(defaultDecissionIndex);

            }

            OnCountDownEnd.Invoke();
            isDeciding = false;
        }

        ///**** Methods called by VIDE action nodes ****/

        //public void sceneAction(string sceneNameAndStartingPositionName)
        //{

        //    StringBuilder sb = new StringBuilder(sceneNameAndStartingPositionName);
        //    string sceneName = "";
        //    string startingPositionName = "";
        //    bool parsingSceneName = true;

        //    if (sb[0] != '[' || sb[sb.Length - 1] != ']')
        //    {
        //        Debug.LogError("Scene Action: Invalid string format. Are you missing brackets in sceneAction Param?");
        //        return;
        //    }

        //    for (int i = 1; i < sb.Length - 1; i++)
        //    {

        //        if (sb[i] == ']')
        //        {
        //            parsingSceneName = false;
        //            if (sb[i + 1] != '[')
        //            {
        //                Debug.LogError("Scene Action: Invalid string format. Are you missing brackets?");
        //                return;
        //            }
        //            else i++;
        //        }

        //        if (parsingSceneName)
        //            sceneName += sb[i];

        //        if (!parsingSceneName)
        //            startingPositionName += sb[i];
        //    }

        //    SceneController sc;

        //    if (!(sc = FindObjectOfType<SceneController>()))
        //    {
        //        Debug.LogError("Class: DialogController, Method: sceneAction \nNo SceneController found in the scene.");
        //        enabled = false;
        //        return;
        //    }

        //    sc.FadeAndLoadScene(sceneName, startingPositionName);
        //}

        //public void pickedUpItemAction(string itemName)
        //{

        //    Inventory.Inventory inventory;

        //    if (!(inventory = FindObjectOfType<Inventory.Inventory>()))
        //    {
        //        Debug.LogError("DialogController: No Inventory Object found on the scene");
        //        return;
        //    }

        //    inventory.AddItem(itemName);
        //}

        //public void lostItemAction(string itemName)
        //{
        //    Inventory.Inventory inventory;

        //    if (!(inventory = FindObjectOfType<Inventory.Inventory>()))
        //    {
        //        Debug.LogError("DialogController: No Inventory Object found on the scene");
        //        return;
        //    }

        //    inventory.RemoveItem(itemName);
        //}
    }
}