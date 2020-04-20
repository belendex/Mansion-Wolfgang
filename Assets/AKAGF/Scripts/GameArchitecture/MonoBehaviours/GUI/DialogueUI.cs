using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VIDE_Data;

//This script will handle everything related to dialogue interface
//It will use a VIDE_Data component to load dialogues and retrieve node data
namespace AKAGF.GameArchitecture.MonoBehaviours.GUI
{
    public class DialogueUI : MonoBehaviour {

        //These are the references to UI components and containers in the scene
        [Header("UI References")]
        bool dialoguePaused = false;                        // Is the dialogue paused?

        //private Color initialCommentTextColor;              // Variable to store the initial value of the color used in character text
        //private Color initialNameTextColor;                 // Variable to store the initial value of the color used in character name text

        //Player/NPC Image component
        [Header("Characters Sprites Options")]                       
        public Image NPCSprite;                             // Reference to the UI Image component where the npc sprite will be displayed
        public Image playerSprite;                          // Reference to the UI Image component where the player sprite will be displayed

        [Header("Text Managers References")]
        public TextManager playerNameLabelTextManager;
        public TextManager NPCnameLabelTextManager;
        public TextManager NPCtextManager;                     // Reference to the textManager where the main text is actually displayed

        
        public TextManager[] playerChoices;               // All the references to the gameobjects where the player options will be displayed
        //public Sprite[] countDownSprites;                   // Sprites to show countdown graphic reference
        


        private void OnEnable() {
            VD.OnNodeChange += UpdateUI;
        }

        private void OnDisable() {
            VD.OnNodeChange -= UpdateUI;
        }


        //When we call VD.Next, nodeData will change. When it changes, OnNodeChange event will fire
        //We subscribed our UpdateUI method to the event in the Begin method
        //Here's where we update our UI
        public void UpdateUI(VD.NodeData data) {

            //Look for dynamic text change in extraData
            DialogManager.PostConditions(data);

            //If this new Node is a Player Node, set the player choices offered by the node
            if (data.isPlayer) {

                Sprite pSprite = null;

                //Set node sprite if there's any, otherwise try to use default sprite
                if (data.sprite != null)
                    pSprite = data.sprite;
                else if (VD.assigned.defaultPlayerSprite != null)
                    pSprite = VD.assigned.defaultPlayerSprite;


                SetChoices(data.comments);

                UnityEvent ue = new UnityEvent();
                ue.AddListener(() => setCharacterSprite(pSprite, playerSprite));


                //If it has a tag, show it, otherwise let's use the alias we set in the VIDE Assign
                if (data.tag.Length > 0)
                    playerNameLabelTextManager.DisplayMessage(new Message { message = data.tag, textColor = Color.white, OnMessageStart = ue });
                else
                    playerNameLabelTextManager.DisplayMessage(new Message { message = VD.assigned.alias, textColor = Color.white, OnMessageStart = ue });

            }
            else  //If it's an NPC Node, let's just update NPC's text and sprite
            {

                // Hack for avoid calling more than once 
                // per node the displayMessage method 
                if (data.commentIndex > 0) {
                    VD.Next();
                    return;
                }
                    

                for (int i = 0; i < data.comments.Length; i++) {

                    AudioClip clip = null;
                    Sprite npcSprite = null;

                    if (data.audios[i] != null) {
                        clip = data.audios[i];
                    }

                    //Set node sprite if there's any, otherwise try to use default sprite
                    if (data.sprites[i] != null) {
                        npcSprite = data.sprites[i];
                    }
                    else if (data.sprite != null) {
                        npcSprite = data.sprite;
                    }
                    //or use the default sprite if there isnt a node sprite at all
                    else if (VD.assigned.defaultNPCSprite != null)
                        npcSprite = VD.assigned.defaultNPCSprite;

                    UnityEvent ue = new UnityEvent();

                    if (NPCSprite.sprite == null)
                        ue.AddListener(() => setCharacterSprite(npcSprite, playerSprite));
                    else
                        ue.AddListener(() => setCharacterSprite(npcSprite, NPCSprite));


                    NPCtextManager.DisplayMessage(new Message { message = data.comments[i], textColor = Color.white, audioClip = clip, OnMessageStart = ue }, i == data.comments.Length - 1 ? true : false);
                }

                //If it has a tag, show it, otherwise let's use the alias we set in the VIDE Assign
                if (data.tag.Length > 0)
                    NPCnameLabelTextManager.DisplayMessage(new Message { message = data.tag, textColor = Color.white });
                else
                    NPCnameLabelTextManager.DisplayMessage(new Message { message = VD.assigned.alias, textColor = Color.white });
            }
        }

        public void setCharacterSprite(Sprite sprite, Image image) {

            if (sprite != null && image != null) {
                image.color = new Color(1f, 1f, 1f, 1f);
                image.sprite = sprite;
                
            }
            else if (sprite == null && image)
                image.color = new Color(0f, 0f, 0f, 0f);
        }


        //This uses the returned string[] from nodeData.comments to create the UIs for each comment
        //It first cleans, then it instantiates new choices
        public void SetChoices(string[] choices){
            for (int i = 0; i < choices.Length; i++) {
                playerChoices[i].DisplayMessage(new Message{message = choices[i] , textColor = Color.white});
            }
        }


        //private IEnumerator countDown()  {

        //    Text secondsText = decisionCountDownContaniner.GetComponentInChildren<Text>();
        //    Image barImage = decisionCountDownContaniner.GetComponentsInChildren<Image>()[1];
        //    decisionCountDownContaniner.SetActive(true);
        //    float startDecisionTime = decissionTime;

        //    float countDownTimer = decissionTime;

        //    while (countDownTimer > 0) {
        //        countDownTimer -= Time.deltaTime;
        //        secondsText.text = global::System.Convert.ToInt16(decissionTime).ToString();


        //        //get the corresponding texture array index
        //        float percentagePerImage = 100 / countDownSprites.Length;
        //        float leftTimePercentage = (decissionTime * 100) / startDecisionTime;
        //        int spriteIndex = (int)(leftTimePercentage / percentagePerImage);

        //        if (spriteIndex >= countDownSprites.Length)
        //            spriteIndex = countDownSprites.Length - 1;

        //        barImage.sprite = countDownSprites[spriteIndex];

        //        yield return null;
        //    }

        //    decisionCountDownContaniner.SetActive(false);

        //    if (decissionTime <= 0 ) {
        //        if (timeOutRandomDecision) {
        //            //DialogManager.SelectChoice(Random.Range(0, playerChoices.Length));
        //        }
        //        else {
        //            //DialogManager.SelectChoice(timeOutDecisionIndex);
        //        }
        //    }
        //}

        // This method clean and hide previous player comments options
        //private void clearOptions() {
        //    //Clean old options
        //    for (int i = 0; i < maxPlayerChoices.Length; i++) {
        //        maxPlayerChoices[i].gameObject.SetActive(false);
        //        maxPlayerChoices[i].gameObject.GetComponentInChildren<Text>().text = "";
        //    }

        //    temporizedDecision = initialTemporizedDecision;
        //    decissionTime = initialDecisionTime;
        //}

    }
}
