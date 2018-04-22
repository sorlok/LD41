using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameState : MonoBehaviour {

	// Helper: Make it easy to turn off exceptions when we ship this
	public static void ThrowException(string msg) {
		throw new System.ArgumentException (msg);
	}

	// Page 1: Dating container object and text + options.
	public GameObject DialogueStoryTab;
	public Text StoryTxt;
	public Button Response1;
	public Button Response2;
	public Button Response3;

	// Our "choice" particle system and response particles
	public ParticleSystem ChoiceParticles;
	public Material GoodOptionTexture;
	public Material NeutralOptionTexture;
	public Material BadOptionTexture;

	// The date's action
	public GameObject DateActionTab;
	public Text DateActTxt;
	public Text ResultTxt;
	public GameObject DateProgressBarBkg;
	public GameObject DateProressBar;
	public GameObject DateProgressSkipBtn;

	// Some Leads/Fans (TODO: actually add)
	//private Lead testLead = new Lead();

	public GameObject MapHandler;

	private ActState SkipPhase = ActState.Nothing; 	// Hack to avoid double-clicking

	// General random generator
	private System.Random rng = new System.Random();

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

		// DateAction sub-actions
		DateActSocialMedia,  // Date is ignoring you and interacting with their fans
		DateActTalkToYou,  // Date is interacting with you
		DateActShowReward, // We're looking at the "reward" the date got from talking to you or using social media.
	}
		

	// Current state tree entry. Sub-state(s) should be set to 0 on state change.
	public ActState CurrState = ActState.Nothing;

	// Countdown for whenever your date is acting
	private static float DateActCountMax = 5; // How many seconds to complete an action
	private float DateActCount = DateActCountMax;  // When < max, counts up

	private DateDialogues dateDialogues;

	// Show or hide 3 option boxes. If null, hide, if not, show and set text
	private void ShowBoxes(string opt1, string opt2=null, string opt3=null) {
		string[] opts = new string[]{ opt1, opt2, opt3 };
		Button[] comps = new Button[] { Response1, Response2, Response3 };

		for (int i = 0; i < 3; i++) {
			if (opts [i] != null) {
				comps [i].GetComponentInChildren <Text> ().text = opts [i];
			} 
			comps [i].gameObject.SetActive (opts [i] != null);
		}
	}

	// TEST
	public void TestFunction(uint val) {
		Debug.Log ("TEST: " + val);
	}

	public void SetupChoosePlayerAction() {
		//testLead.SelfEsteemTracker += TestFunction; // TEMP

		MapHandler.GetComponent<MapHandler> ().CreateLead (2, 2);

		StoryTxt.text = "What will you do this turn?";
		ShowBoxes (
			"Interact with Date",
			"Interact with Fans",
			"End Date"
		);
		DialogueStoryTab.SetActive (true);
		CurrState = ActState.PlayerActionSelect;
	}

	public void SetupInteractWithDate() {
		//testLead.SelfEsteem = 20;
		//testLead.SelfEsteem = 100;

		StoryTxt.text = "How will you interact with your date?";
		ShowBoxes (
			"Talk to Them",
			"Impress Them",
			"Encourage Them"
		);

		DialogueStoryTab.SetActive (true);
		CurrState = ActState.ChooseInteractWithDate;
	}

	public void SetupTalkToDate() {
		DateDialogue dd = dateDialogues.DialogueOptions [0];

		StoryTxt.text = dd.storyText;
		ShowBoxes (
			dd.option1,
			dd.option2,
			dd.option3
		);

		DialogueStoryTab.SetActive (true);
		CurrState = ActState.ChooseInteractTalk;
	}

	public void SetupDateTurn() {
		DialogueStoryTab.SetActive (false);

		DateProgressBarBkg.SetActive (true);
		DateProressBar.SetActive (true);
		DateProgressSkipBtn.transform.localPosition = new Vector3 (165, 70, 0);
		DateProgressSkipBtn.GetComponentInChildren <Text> ().text = "Skip";
		ResultTxt.gameObject.SetActive (false);
		DateActTxt.text = "Your date is deciding what to do...";
		DateProressBar.gameObject.transform.localScale = new Vector3 (0, 1, 1);
		DateActionTab.SetActive (true);
		DateActCount = 0;

		CurrState = ActState.DateAction;
	}

	// What action will our date take?
	public void MakeDateDecision() {
		// TODO : More structured randomness, incorporate Pablo's actors
		if (rng.Next (100) < 50) {
			CurrState = ActState.DateActSocialMedia;
			DateActTxt.text = "Your date is tweeting a picture of their food...";
		} else {
			CurrState = ActState.DateActTalkToYou;
			DateActTxt.text = "Your date explains their view on current events...";
		}
		DateProressBar.gameObject.transform.localScale = new Vector3 (0, 1, 1);
		DateActionTab.SetActive (true);
		DateActCount = 0;
	}
		

	public void ChooseOption(int opt) {
		// Being asked to select the interaction method.
		if (CurrState == ActState.PlayerActionSelect) {
			if (opt == 1) {
				// Interact with your date.
				SetupInteractWithDate ();
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
			ChoiceParticles.GetComponent<ParticleOrientor>().OrientParticles ();
			// Particles
			//ChoiceParticles.transform.localPosition = new Vector3 (0, -1 * (opt - 1), 0); // Hack for now
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

		// Done talking to date, now move to next phase
		if (CurrState == ActState.TalkDateViewResponse) {
			if (opt == 1) {
				SetupDateTurn ();
			}

			return;
		}


		// Comment out on release.
		ThrowException ("Bad option: " + opt);
	}

	public void ChoiceParticlesDone() {
		ShowBoxes (
			"Done Talking"
		);

		// Set Response
		StoryTxt.text = "Wow, what a...\nnice? thing to say...";

		// Need to move the state along.
		CurrState = ActState.TalkDateViewResponse;
	}


	void DateCollectsReward() {
		// TODO: actually increase values.


		DateActionTab.SetActive (false);

		// TMP: move to next state
		CurrState = ActState.FansAction;
	}


	// Use this for initialization
	void Start () {
		DialogueStoryTab.SetActive (false);
		dateDialogues = new DateDialogues ();

		DateActionTab.SetActive (false);

		
	}

	private void AdvanceCounter(float amt) {
		DateActCount += amt;

		// Always update the progress bar
		float perc = DateActCount / DateActCountMax;
		if (perc > 1.0f) { perc = 1.0f; }
		DateProressBar.gameObject.transform.localScale = new Vector3 (perc, 1, 1);

		// Trigger?
		if (DateActCount >= DateActCountMax) {
			// What were we in the middle of?
			if (CurrState == ActState.DateAction) {
				MakeDateDecision ();
			} else if (CurrState == ActState.DateActSocialMedia || CurrState == ActState.DateActTalkToYou) {
				// Show "rewards"
				DateProgressBarBkg.SetActive (false);
				DateProressBar.SetActive (false);
				DateProgressSkipBtn.transform.localPosition = new Vector3 (165, 120, 0);
				DateProgressSkipBtn.GetComponentInChildren <Text> ().text = "Ok";
				ResultTxt.gameObject.SetActive (true);

				ResultTxt.text = "You gained 1 Self-Confidence\nYour date gained 2k fans";

				CurrState = ActState.DateActShowReward;
			} else {
				ThrowException ("Bad current state: " + CurrState);
			}
		}
	}

	// Skip if waiting on an action
	public void SkipWaitCounter() {
		// This is also our "ok" button
		if (CurrState == ActState.DateActShowReward) {
			DateCollectsReward ();
		} 
		/*else {
			if (DateActCount < DateActCountMax) {
				if (SkipPhase == CurrState) {
					AdvanceCounter (9999);
				}
			}
		}*/
	}
	
	// Update is called once per frame
	void Update () {
		// Hack to avoid double-clicking
		if (Input.GetMouseButtonDown (0)) {
			if (EventSystem.current.IsPointerOverGameObject()) {
				SkipPhase = CurrState;
			}
		}
		if (Input.GetMouseButtonUp (0)) {
			if (SkipPhase == CurrState && DateActCount < DateActCountMax) {
				if (EventSystem.current.IsPointerOverGameObject ()) {
					AdvanceCounter (9999);
				}
			}
			SkipPhase = ActState.Nothing;
		}

		if (Input.GetKey (KeyCode.W)) {
			if (CurrState == GameState.ActState.Nothing) {
				SetupChoosePlayerAction();
				return;
			}
		}

		// Deal with counter
		if (DateActCount < DateActCountMax) {
			// Any key will advance the counter 100%
			if (Input.anyKeyDown && SkipPhase == ActState.Nothing) {
				AdvanceCounter (9999);
			} else { 
				AdvanceCounter (Time.deltaTime);
			}
		}
		
	}

}
