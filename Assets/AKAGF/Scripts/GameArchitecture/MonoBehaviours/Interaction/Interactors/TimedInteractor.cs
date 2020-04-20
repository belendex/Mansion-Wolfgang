using AKAGF.GameArchitecture.MonoBehaviours.Interaction;
using UnityEngine;

public class TimedInteractor : MonoBehaviour {

    public int numberOfInteractions = -1;
    public bool randomTime;
    public int maxRandomSeconds;
    public int minRandomSeconds;
    public int fixedSecondsInteraction;

    public bool interact{ get; set; }

    private float timer;

    private Interactable interactable;

    // Use this for initialization
    void Awake () {
        if (!(interactable = GetComponent<Interactable>())) {
            Debug.LogError("No Interactable Component found on: " + name + " gameObject.");
            enabled = false;
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (!interact || interactable.isInteracting) 
            return;

        if (Time.time > timer && numberOfInteractions != 0) {

            interactable.Interact();

            timer = randomTime ? Random.Range(minRandomSeconds, maxRandomSeconds+1) : fixedSecondsInteraction;
            timer += Time.time;

            if (numberOfInteractions != -1)
                numberOfInteractions--; 
        }
	}
}
