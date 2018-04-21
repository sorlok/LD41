using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour {

	// Page 1: Dating container object and text + options.
	public GameObject DialogueStoryChoice;
	public Text StoryTxt;
	public Button Response1;
	public Button Response2;
	public Button Response3;

	// Our "choice" particle system and response particles
	public ParticleSystem ChoiceParticles;
	public Material GoodOptionTexture;
	public Material NeutralOptionTexture;
	public Material BadOptionTexture;

	// State = what you're doing. 
	//         some states may be correllated (i.e., talking to your date implies interacting with them), 
	//         but we'll track that somewhere else.
	public enum ActState {
		Nothing, // Nothing going on
		PlayerActionSelect, // Player is selecting action to take
		DateAction, // Date is deciding on action
		FansAction, // Fans making decisions, moving, etc.

		//PlayerActionSelect sub-states
		ChooseAction, // Player is selecting to interact with date or fans
		ChooseInteractWithDate, // Player has chosen to interact with date
		ChooseDealWithFans, // Player has chosen to interact with fans

		// DateAction sub-states
		// FansAction sub-states

		// ChooseInteractWithDate sub-states
		//ChooseInteractMethod, // Interact with date how?
		ChooseInteractTalk, // Choose to talk to date
		ChooseInteractImpress, // Risky strats
		ChooseInteractEncourage, // Defensive

		// ChooseDealWithFans sub-states
		//ChooseDealMethod, // Deal with fans how?
		ChooseDealDirect, // Deal directly with an adjacent fan
		ChooseDealTweet, // Wide-ranging effect, hard to control
		ChooseDealInstagram, // Powerful, single-use (ish)

		// ChooseInteractTalk sub-states
		TalkDateSelectReact, // Waiting for particles to settle
		TalkDateViewResponse, // See how the date likes your reaction (or not)
	}
		

	// Current state tree entry. Sub-state(s) should be set to 0 on state change.
	public ActState CurrState = ActState.Nothing;



	private DateDialogues dateDialogues;

	public void SetupChoosePlayerAction() {
		StoryTxt.text = "What will you do this turn?";
		Response1.GetComponentInChildren <Text>().text = "Interact with Date";
		Response2.GetComponentInChildren <Text>().text = "Interact with Fans";
		Response3.GetComponentInChildren <Text>().text = "End Date";

		// Show boxes.
		foreach (Button g in new Button[]{Response1, Response2, Response3}) {
			g.gameObject.SetActive (true);
		}

		DialogueStoryChoice.SetActive (true);
	}

	public void SetupInteractWithDate() {
		StoryTxt.text = "How will you interact with your date?";
		Response1.GetComponentInChildren <Text>().text = "Talk to Them";
		Response2.GetComponentInChildren <Text>().text = "Impress Them";
		Response3.GetComponentInChildren <Text>().text = "Encourage Them";

		// Show boxes.
		foreach (Button g in new Button[]{Response1, Response2, Response3}) {
			g.gameObject.SetActive (true);
		}

		DialogueStoryChoice.SetActive (true);
	}

	public void SetupTalkToDate() {
		DateDialogue dd = dateDialogues.DialogueOptions [0];

		StoryTxt.text = dd.storyText;
		Response1.GetComponentInChildren <Text>().text = dd.option1;
		Response2.GetComponentInChildren <Text>().text = dd.option2;
		Response3.GetComponentInChildren <Text>().text = dd.option3;

		// Show boxes.
		foreach (Button g in new Button[]{Response1, Response2, Response3}) {
			g.gameObject.SetActive (true);
		}

		DialogueStoryChoice.SetActive (true);
	}
		

	public void ChooseOption(int opt) {
		// Being asked to select the interaction method.
		if (CurrState == ActState.PlayerActionSelect) {
			if (opt == 1) {
				// Interact with your date.
				SetupInteractWithDate ();
				CurrState = ActState.ChooseInteractWithDate;
			} else if (opt == 2) {
				// Deal with fans
			} else if (opt == 3) {
				// End date (or move)
			}

			return;
		}

		// Being asked how to interact with date
		if (CurrState == ActState.ChooseInteractWithDate) {
			if (opt == 1) {
				// Talk to your date
				SetupTalkToDate ();
				CurrState = ActState.ChooseInteractTalk;
			} else if (opt == 2) {
				// Interact with them
			} else if (opt == 3) {
				// Encourage them
			}

			return;
		}

		// Talking to date, they react to your choice
		if (CurrState == ActState.ChooseInteractTalk) {
			CurrState = ActState.TalkDateSelectReact;
			// Particles
			ChoiceParticles.transform.localPosition = new Vector3 (0, -1 * (opt - 1), 0); // Hack for now
			if (opt == 1) {
				ChoiceParticles.GetComponent<Renderer> ().material = GoodOptionTexture;
			} else if (opt == 2) {
				ChoiceParticles.GetComponent<Renderer> ().material = NeutralOptionTexture;
			} else {
				ChoiceParticles.GetComponent<Renderer> ().material = BadOptionTexture;
			}
			// TODO: Update after setting material?
			ChoiceParticles.Play();
			Invoke("ChoiceParticlesDone", 2);

			return;
		}

		// Comment out on release.
		throw new System.ArgumentException ("Bad option: " + opt);
	}

	public void ChoiceParticlesDone() {
		// Clear boxes.
		foreach (Button g in new Button[]{Response1, Response2, Response3}) {
			g.gameObject.SetActive (false);
		}

		// Set Response
		StoryTxt.text = "Wow, what a...\nnice? thing to say...";
	}



	// Use this for initialization
	void Start () {
		DialogueStoryChoice.SetActive (false);
		dateDialogues = new DateDialogues ();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
