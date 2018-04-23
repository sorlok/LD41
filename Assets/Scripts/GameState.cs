using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameState : MonoBehaviour {

	// Helper: Make it easy to turn off exceptions when we ship this
	public static void ThrowException(string msg) {
		throw new System.ArgumentException (msg);
	}

	public GameObject GameOverObj;
	public GameObject GameOverTxt;

	public GameObject FinalScreenObj;
	public GameObject FinalScreenTxt;

	public GameObject LeftPanel;
	public GameObject AttributesPanel;
	public GameObject PhasePanel;

	public GameObject RightPanel;
	public GameObject DialoguePanel;
	public GameObject BigButtonPanel;

	// Big button texts
	public Text SkipText;
	public Text NextText;
	public GameObject NextStamp;

	public AudioSource bgmSource;

	public AudioSource sfxSourceBtn;
	public AudioClip buttonSFX;
	public AudioSource sfxSourceTurn;
	public AudioClip turnSFX;

	public AudioClip buttonEvilLaugh;

	// Page 1: Dating container object and text + options.
	public GameObject DialogueStoryTab;
	public Text StoryTxtHeader;
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

	public GameObject GlobalCanvas;
	public GameObject MapHandler;

	// Used to reset stamps
	public GameObject RespStamp1;
	public GameObject RespStamp2;
	public GameObject RespStamp3;

	private ActState SkipPhase = ActState.Nothing; 	// Hack to avoid double-clicking

	public TweetHandler tweetHandler;

	// General random generator
	public static System.Random rng = new System.Random();

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

		// In the middle of some UI interpolation
		DoingInterp,

		// Fading the text out/in for a story text transition
		FadingTextOut,
		FadingTextIn,

		// Waiting for the player to acknowledge the damage all fans have done
		WaitingFanAckFromPlayer,

		// Confirm move
		MoveToNewLocation,

		// Gameover stuff
		GameOverFadein,
		GameOverOnscreen,

		// Final screen stuff
		FinalScreenOnscreen,
	}

	// The state to set after the next fade?
	private ActState AfterFadeState;

	// Whether or not our date liked the last response
	private char LastDateResponse;

	// Current interpolation variables
	private float interpStartTime;
	private float interpTotalLength;
	private float interpSpeed = 1.2f;

	// Interpolation 1
	private Transform interpTarget;
	private Vector3 interpStartPos;
	private Vector3 interpEndPos;
	private Vector3 interpStartSz;
	private Vector3 interpEndSz;

	// Interpolation 2
	private Transform interpTarget2;
	private Vector3 interpStartPos2;
	private Vector3 interpEndPos2;
	private Vector3 interpStartSz2;
	private Vector3 interpEndSz2;

	// Interpolation 3
	private Transform interpTarget3;
	private Vector3 interpStartPos3;
	private Vector3 interpEndPos3;
	private Vector3 interpStartSz3;
	private Vector3 interpEndSz3;
		

	// Current state tree entry. Sub-state(s) should be set to 0 on state change.
	public ActState CurrState = ActState.Nothing;
	public string phaseName = "";
	public PhaseHandler phaseHandler;

	// Countdown for whenever your date is acting
	private static float DateActCountMax = 2; // How many seconds to complete an action
	private float DateActCount = DateActCountMax;  // When < max, counts up

	// Temporary hack for moving NPCs
	private static float NPCMoveCountMax = 0.9f;
	private float NPCMoveCount = NPCMoveCountMax;

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

	private bool updateInterpolation() {
		float distCovered = (Time.time - interpStartTime) * interpSpeed;
		float fracJourney = distCovered / interpTotalLength;

		interpTarget.position = Vector3.Lerp(interpStartPos, interpEndPos, fracJourney);
		interpTarget.localScale = Vector3.Lerp (interpStartSz, interpEndSz, fracJourney);

		interpTarget2.position = Vector3.Lerp(interpStartPos2, interpEndPos2, fracJourney);
		interpTarget2.localScale = Vector3.Lerp (interpStartSz2, interpEndSz2, fracJourney);

		interpTarget3.position = Vector3.Lerp(interpStartPos3, interpEndPos3, fracJourney);
		interpTarget3.localScale = Vector3.Lerp (interpStartSz3, interpEndSz3, fracJourney);

		return interpTarget.position == interpEndPos;
	}

	public void ReactToStamp(int stampId) { // 1, 2, 3 for stamp ID
		// Beginning of game: show the timer, show the dialogue box
		if (CurrState == ActState.Nothing) {
			//TEMP:DEBUG:TESTING
			//StartFansActionState();return;
			//END:TEMP:DEBUG:TESTING


			PlayButtonSound ();

			// Start Invisible
			StoryTxtHeader.text = "Date Action";
			StoryTxt.text = "";
			ShowBoxes (null, null, null);

			// First interpolation
			interpEndPos = PhasePanel.transform.position;
			interpStartPos = PhasePanel.transform.position + new Vector3 (1.3f, 0, 0);
			interpEndSz = new Vector3 (1, 1, 1);
			interpStartSz = new Vector3 (0.1f, 0.1f, 0.1f);
			interpTarget = PhasePanel.transform;
			PhasePanel.SetActive (true);

			// Second interpolation
			interpEndPos2 = DialoguePanel.transform.position;
			interpStartPos2 = DialoguePanel.transform.position + new Vector3 (0, 0, -0.5f);
			interpEndSz2 = new Vector3 (1, 1, 1);
			interpStartSz2 = new Vector3 (0.1f, 0.1f, 0.1f);
			interpTarget2 = DialoguePanel.transform;
			DialoguePanel.SetActive (true);

			// Third interpolation
			interpEndPos3 = AttributesPanel.transform.position;
			interpStartPos3 = AttributesPanel.transform.position + new Vector3 (0, 0, 1f);
			interpEndSz3 = new Vector3 (1, 1, 1);
			interpStartSz3 = new Vector3 (0.1f, 0.1f, 0.1f);
			interpTarget3 = AttributesPanel.transform;
			AttributesPanel.SetActive (true);

			// Set up the interpolation
			interpStartTime = Time.time;
			interpTotalLength = Vector3.Distance (interpStartPos, interpEndPos);

			// Update all
			updateInterpolation ();

			// Temp
			phaseHandler.UpdateColoring();

			CurrState = ActState.DoingInterp;
			return;
		}

		// Phase 1C: Try to move to new location
		if (CurrState == ActState.MoveToNewLocation) {
			// Go back
			if (TooSoonToMove ()) {
				// Fade text to player's turn
				CurrState = ActState.FadingTextOut;
				AfterFadeState = ActState.PlayerActionSelect;
			} else {
				if (phaseHandler.thisHour == 10) {
					// TODO: End date code here.
					ShowFinalScreen();
				} else {
					// Fade back to start
					CurrState = ActState.FadingTextOut;
					AfterFadeState = ActState.PlayerActionSelect;
				}
			}

			PlayButtonSound ();
			return;
		}

		// Phase 1: Select talk, tweet, or move
		if (CurrState == ActState.PlayerActionSelect) {
			PlayButtonSound ();


			if (stampId == 1) {
				// Talk to date
				// Fade out text, fade in new text
				CurrState = ActState.FadingTextOut;
				AfterFadeState = ActState.ChooseInteractTalk;


			} else if (stampId == 2) {
				// Tweet @ fans
				tweetHandler.StartTweeting();
			} else if (stampId == 3) {
				// Move date location
				CurrState = ActState.FadingTextOut;
				AfterFadeState = ActState.MoveToNewLocation;
			}

			return;
		}

		// Phase 1.A - Date reacts to dialogue choice
		if (CurrState == ActState.ChooseInteractTalk) {

			CurrState = ActState.FadingTextOut;
			AfterFadeState = ActState.TalkDateViewResponse;

			ChoiceParticles.GetComponent<ParticleOrientor>().OrientParticles ();
			ChoiceParticles.Clear ();
			ChoiceParticles.gameObject.SetActive (true);
			// Particles
			LastDateResponse = new char[]{'G','B','N'}[rng.Next(3)];
			if (LastDateResponse == 'G') {
				ChoiceParticles.GetComponent<Renderer> ().material = GoodOptionTexture;
				MapHandler.GetComponent<MapHandler> ().LeadPlayerScript.SelfEsteem += 2;
			} else if (LastDateResponse == 'N') {
				ChoiceParticles.GetComponent<Renderer> ().material = NeutralOptionTexture;
				MapHandler.GetComponent<MapHandler> ().LeadPlayerScript.SelfEsteem += 1;
			} else {
				ChoiceParticles.GetComponent<Renderer> ().material = BadOptionTexture;
				MapHandler.GetComponent<MapHandler> ().LeadPlayerScript.SelfEsteem -= 1;
			}

			// TODO:TEMP:DEBUG
			//MapHandler.GetComponent<MapHandler> ().LeadPlayerScript.SelfEsteem -= 99;


			PlayButtonSound ();

			ChoiceParticles.Play();
			return;
		}

		// Phase 1.B - Saw date react, move to next phase
		if (CurrState == ActState.TalkDateViewResponse) {
			CurrState = ActState.FadingTextOut;
			AfterFadeState = ActState.DateAction;
			PlayButtonSound ();
			return;

		}

		// Phase 2 - Ready to switch to fan phase?
		if (CurrState == ActState.DateActShowReward) {
			CurrState = ActState.FadingTextOut;
			AfterFadeState = ActState.FansAction;
			PlayButtonSound ();
			return;

		}

		// Phase 3 - Ready to switch back to player phase?
		if (CurrState == ActState.WaitingFanAckFromPlayer) {
			//Update Phase Clock
			phaseHandler.UpdateMinute();
			phaseHandler.UpdateTime ();

			// Add some new fans
			MapHandler.GetComponent<MapHandler>().SpawnFans(3);

			// Fade text to player's turn
			CurrState = ActState.FadingTextOut;
			AfterFadeState = ActState.PlayerActionSelect;
			PlayButtonSound ();
			return;

		}

	}

	public void PlayButtonSound() {
		sfxSourceBtn.clip = buttonSFX;
		sfxSourceBtn.Play ();
	}

	public void PlayEvilLaugh() {
		sfxSourceBtn.clip = buttonEvilLaugh;
		sfxSourceBtn.Play ();
	}

	// TEST
	/*public void TestFunction(uint val) {
		Debug.Log ("TEST: " + val);
	}*/

	public void SetupChoosePlayerAction() {
		StoryTxtHeader.text = "Date Action";
		StoryTxt.text = "What will you do this turn?";
		string changeStr = phaseHandler.thisHour<9 ? "Change Date Location" : "End Date";
		ShowBoxes (
			"Talk to Date",
			"Tweet @Fans",
			changeStr
		);
		//DialogueStoryTab.SetActive (true);
		CurrState = ActState.PlayerActionSelect;

		// Set Next button text
		SkipText.text = "Main Action";
		NextText.text = "(In Story)";

		// Reset button stamp statuses (but not for the bottom item).
		// TODO

		// Update Phase name
		phaseName = "Your Turn";
		phaseHandler.UpdateActiveUser (phaseName);
	}

	public void SetupInteractWithDate() {
		StoryTxt.text = "How will you interact with your date?";
		ShowBoxes (
			"Talk to Them",
			"Impress Them",
			"Encourage Them"
		);

		DialogueStoryTab.SetActive (true);
		CurrState = ActState.ChooseInteractWithDate;
	}

	private void ShowFinalScreen() {
		Lead player = MapHandler.GetComponent<MapHandler> ().LeadPlayerScript;
		int score = player.Connection * 1000 + player.SelfEsteem * 100 + player.FanCount;

		FinalScreenObj.SetActive (true);
		FinalScreenTxt.GetComponent<Text> ().text = "Final Score: " + score;

		CurrState = ActState.FinalScreenOnscreen;
	}

/*	public void SetupTalkToDate() {
		DateDialogue dd = dateDialogues.DialogueOptions [rng.Next(4)];

		StoryTxt.text = dd.storyText;
		ShowBoxes (
			dd.option1,
			dd.option2,
			dd.option3
		);

		DialogueStoryTab.SetActive (true);
		CurrState = ActState.ChooseInteractTalk;
	}*/

	public void SetupDateTurn() {
		// unused
		//DialogueStoryTab.SetActive (false);
		//DateProgressBarBkg.SetActive (true);

		// todo: migrate over to the right
		DateProressBar.SetActive (true);
		DateActionTab.transform.localPosition = new Vector3 (0, 0, 0);
		DateProgressSkipBtn.GetComponentInChildren <Text> ().text = "Skip";
		ResultTxt.gameObject.SetActive (false);
		//StoryTxt.text = "Your date is deciding what to do...";
		DateProressBar.gameObject.transform.localScale = new Vector3 (0, 1, 1);
		DateActionTab.SetActive (true);
		DateActCount = 0;

		//NextText.text = "(Date Acting)";
		CurrState = ActState.DateAction;
		phaseName = "Date's Turn";
		phaseHandler.UpdateActiveUser (phaseName);
	}

	// What action will our date take?
	public void MakeDateDecision() {
		// TODO : More structured randomness, incorporate Pablo's actors
		if (rng.Next (100) < 50) {
			CurrState = ActState.DateActSocialMedia;
			string txt = dateDialogues.DateToFansInteractions [rng.Next(dateDialogues.DateToFansInteractions.Count)];
			StoryTxt.text += "\n" + txt;
		} else {
			CurrState = ActState.DateActTalkToYou;
			string txt = dateDialogues.DateToYouInteractions [rng.Next(dateDialogues.DateToYouInteractions.Count)];
			StoryTxt.text += "\n" + txt;
		}
		DateActionTab.transform.localPosition = new Vector3 (0, -100, 0);
		DateProressBar.gameObject.transform.localScale = new Vector3 (0, 1, 1);
		DateActionTab.SetActive (true);
		DateActCount = 0;
	}

	public void ChooseOption(int opt) {
		/*
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

			sfxSource.clip = buttonSFX;
			sfxSource.Play ();
			return;
		}

		// Being asked how to interact with date
		if (CurrState == ActState.ChooseInteractWithDate) {
			if (opt == 1) {
				// Talk to your date
				CurrState = ActState.ChooseInteractTalk;
			} else if (opt == 2) {
				// Interact with them
			} else if (opt == 3) {
				// Encourage them
			}

			sfxSource.clip = buttonSFX;
			sfxSource.Play ();
			return;
		}

		// Talking to date, they react to your choice
		if (CurrState == ActState.ChooseInteractTalk) {
			CurrState = ActState.TalkDateSelectReact;
			ChoiceParticles.GetComponent<ParticleOrientor>().OrientParticles ();
			ChoiceParticles.Clear ();
			ChoiceParticles.gameObject.SetActive (true);
			// Particles
			//ChoiceParticles.transform.localPosition = new Vector3 (0, -1 * (opt - 1), 0); // Hack for now
			if (opt == 1) {
				ChoiceParticles.GetComponent<Renderer> ().material = GoodOptionTexture;
			} else if (opt == 2) {
				ChoiceParticles.GetComponent<Renderer> ().material = NeutralOptionTexture;
			} else {
				ChoiceParticles.GetComponent<Renderer> ().material = BadOptionTexture;
			}

			sfxSource.clip = buttonSFX;
			sfxSource.Play ();

			// TODO: Update after setting material?
			ChoiceParticles.Play();
			Invoke("ChoiceParticlesDone", 2);

			return;
		}

		// Noop
		if (CurrState == ActState.TalkDateSelectReact) {
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
		*/
	}

	/*public void ChoiceParticlesDone() {
		//ShowBoxes (
		//	"Done Talking"
		//);

		// Set Response
		//StoryTxt.text = "Wow, what a...\nnice? thing to say...";
		// Need to move the state along.
		CurrState = ActState.TalkDateViewResponse;
	}*/

	void DateCollectsReward() {
		// TODO: actually increase values.
		// TODO: skipping this phase isn't hooked up correctly right now

		//DateActionTab.SetActive (false);

		// TMP: move to next state
		StartFansActionState();
	}

	private void StartFansActionState() 
	{
		CurrState = ActState.FansAction;
		phaseName = "Fans' Turns";
		phaseHandler.UpdateActiveUser (phaseName);
		MapHandler.GetComponent<MapHandler> ().ResetNPCMoves ();
		NPCMoveCount = 0;
	}

	// Use this for initialization
	void Start () {
		dateDialogues = new DateDialogues ();

		GlobalCanvas.SetActive (true);
		DialogueStoryTab.SetActive (false);
		DateActionTab.SetActive (false);

		// Put our lead and a few fans on the board
		MapHandler.GetComponent<MapHandler>().LoadMap1();

		// TEMP: Testing fan actions
		//StartFansActionState();

		// Start with stats, time hidden, just "next" button ready
		LeftPanel.SetActive (true);
		AttributesPanel.SetActive (false);
		PhasePanel.SetActive (false);
		RightPanel.SetActive (true);
		DialoguePanel.SetActive (false);
		BigButtonPanel.SetActive (true);
		GameOverObj.SetActive (false);
		GameOverTxt.SetActive (false);
		FinalScreenObj.SetActive (false);


		// Set up start text on Big Button Panel
		SkipText.text = "Your Date Starts at 7pm";
		NextText.text = "Begin Date";

		// Add a listener for gameovers
		MapHandler.GetComponent<MapHandler>().LeadPlayerScript.SelfEsteemTracker += GameoverTracker;
	}

	public void GameoverTracker(int selfEsteem) {
		if (selfEsteem <= 0) {
			CurrState = ActState.GameOverFadein;

			// Reset practically everything
			DateActCount = DateActCountMax;
			NPCMoveCount = NPCMoveCountMax;
		}
	}


	// Modify the stat and return a string describing it.
	private string RandomlyModifyStatsFromTweet() {
		return RandomlyModifyStatsAny (40);
	}

	private string RandomlyModifyStatsAny(int fanCutoff) {
		// Basically, favor additions and fans; sometimes negations and self esteem.
		string res = "";
		int count = rng.Next(100) > 75 ? 2 : 1;
		for (int i = 0; i < count; i++) {
			bool positive = rng.Next (100) > 75;
			int amt = rng.Next (100) > 75 ? 2 : 1;
			if (rng.Next (100) > fanCutoff) {
				// Fans
				amt *= 10;
				if (positive) {
					MapHandler.GetComponent<MapHandler>().LeadPlayerScript.FanCount += amt;
					res += "\n  +" + amt + " Fans";
				} else {
					MapHandler.GetComponent<MapHandler>().LeadPlayerScript.FanCount -= amt;
					res += "\n  -" + amt + " Fans";
				}
			} else {
				// Self Esteem
				if (positive) {
					MapHandler.GetComponent<MapHandler>().LeadPlayerScript.SelfEsteem += amt;
					res += "\n  +" + amt + " Self Esteem";
				} else {
					MapHandler.GetComponent<MapHandler>().LeadPlayerScript.SelfEsteem -= amt;
					res += "\n  -" + amt + " Self Esteem";
				}
			}
		}
			
		return res;
	}

	// Similar to above, but reversed in some aspects
	private string RandomlyModifyStatsFromPersonal() {
		return RandomlyModifyStatsAny (60);
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
				DateActionTab.SetActive (false);
				//DateProressBar.SetActive (false);
				//DateProgressSkipBtn.transform.localPosition = new Vector3 (165, 120, 0);
				//DateProgressSkipBtn.GetComponentInChildren <Text> ().text = "Ok";
				//ResultTxt.gameObject.SetActive (true);

				if (CurrState == ActState.DateActSocialMedia) {
					StoryTxt.text += "\n" + RandomlyModifyStatsFromTweet ();
				} else {
					StoryTxt.text += "\n" + RandomlyModifyStatsFromPersonal ();
				}

				// Set up next button again
				NextStamp.GetComponent<StampHandler>().HideStamp ();
				NextText.text = "Fan Phase";

				CurrState = ActState.DateActShowReward;
			} else {
				ThrowException ("Bad current state: " + CurrState);
			}
		}
	}

	private void AdvanceNPCMoveCounter(float amt) {
		NPCMoveCount += amt;

		// TODO: Update NPC movement

		// Trigger?
		if (NPCMoveCount >= NPCMoveCountMax) {
			// What were we in the middle of?
			if (CurrState == ActState.FansAction) {
				// Move a fan randomly, if one is next
				if (MapHandler.GetComponent<MapHandler> ().MoveNextNPC ()) {
					NPCMoveCount = 0;
				} else {
					CurrState = ActState.WaitingFanAckFromPlayer;
					NextStamp.GetComponent<StampHandler>().HideStamp ();
					NextText.text = "Time Marches On...";

				}
			} else {
				ThrowException ("Bad NPC current state: " + CurrState);
			}
		}
	}

	// Skip if waiting on an action
	public void SkipWaitCounter() {
		// This is also our "ok" button
		if (CurrState == ActState.DateActShowReward) {
			DateCollectsReward ();

			sfxSourceTurn.clip = turnSFX;
			sfxSourceTurn.Play ();
		}
	}

	private bool TooSoonToMove() {
		//TEMP: DEBUG
		//return false;


		return phaseHandler.thisMinute <= 10;
	}

	public void DoNewGame() {
		SceneManager.LoadScene( SceneManager.GetActiveScene().name );
	}

	// Update is called once per frame
	void Update () {
		// Gameover fading takes priority
		if (CurrState == ActState.GameOverFadein) {
			if (!GameOverObj.activeSelf) {
				// Init
				Color clr = GameOverObj.GetComponent<Image> ().color;
				GameOverObj.GetComponent<Image> ().color = new Color (clr.r, clr.g, clr.b, 0);
				GameOverObj.SetActive (true);

				PlayEvilLaugh ();
			} else {
				// Music fade out
				float newVol = bgmSource.volume - 0.02f;
				if (newVol < 0) {
					newVol = 0;
				}
				bgmSource.volume = newVol;


				// Fade in
				Color clr = GameOverObj.GetComponent<Image> ().color;
				float newAlpha = clr.a += 0.01f;
				bool overflow = false;
				if (newAlpha >= 0.98f) {
					newAlpha = 1;
					overflow = true;
				}
				GameOverObj.GetComponent<Image> ().color = new Color (clr.r, clr.g, clr.b, newAlpha);

				if (overflow) {
					// Show text
					GameOverTxt.SetActive(true);

					// Done
					CurrState = ActState.GameOverOnscreen;
				}
			}

			return;
		}


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

		/*if (Input.GetKey (KeyCode.W)) {
			if (CurrState == GameState.ActState.Nothing) {
				SetupChoosePlayerAction();
				return;
			}
		}*/

		//  Deal with interpolation?
		if (CurrState == ActState.DoingInterp) {
			if (updateInterpolation ()) {
				// What to do next?
				SetupChoosePlayerAction ();
			}
		}

		// Deal with text fading?
		if (CurrState == ActState.FadingTextOut) {
			bool overflow = false;
			float newAlpha = StoryTxt.color.a - 1.2f * Time.deltaTime;
			if (newAlpha <= 0) {
				newAlpha = 0;
				overflow = true;
			}
			Color newAlphaColor = new Color (StoryTxt.color.r, StoryTxt.color.g, StoryTxt.color.b, newAlpha);
			StoryTxt.color = newAlphaColor;
			StoryTxtHeader.color = newAlphaColor;
			Response1.GetComponentInChildren <Text> ().color = newAlphaColor;
			Response2.GetComponentInChildren <Text> ().color = newAlphaColor;
			Response3.GetComponentInChildren <Text> ().color = newAlphaColor;
			if (overflow) {

				// Set Story text or response text
				if (AfterFadeState == ActState.TalkDateViewResponse) {
					List<DateDialogue> resp = dateDialogues.DialogueBadResponses;
					if (LastDateResponse == 'G') {
						resp = dateDialogues.DialogueGoodResponses;
					} else if (LastDateResponse == 'N') {
						resp = dateDialogues.DialogueNeutralResponses;
					}

					DateDialogue dd = resp [rng.Next (resp.Count)];
					StoryTxtHeader.text = "Date Dialogue";
					StoryTxt.text = dd.storyText;

					if (LastDateResponse == 'G') {
						StoryTxt.text += "\n\n  +2 Self Esteem";
					} else if (LastDateResponse == 'N') {
						StoryTxt.text += "\n\n  +1 Self Esteem";
					} else {
						StoryTxt.text += "\n\n  -1 Self Esteem";
					}


					// Set response text
					ShowBoxes (
						dd.option1,
						dd.option2,
						dd.option3
					);		
				} else if (AfterFadeState == ActState.DateAction) {
					StoryTxtHeader.text = "Date Dialogue";
					StoryTxt.text = "Your date is deciding what to do...";
					NextText.text = "(Date Acting)";

					// No responses here
					ShowBoxes (null, null, null);	
				} else if (AfterFadeState == ActState.FansAction) {
					StoryTxtHeader.text = "Fan Reactions";
					StoryTxt.text = "";
					NextText.text = "(Fan Phase)";

					// No responses here
					ShowBoxes (null, null, null);	
				} else if (AfterFadeState == ActState.PlayerActionSelect) {
					StoryTxtHeader.text = "Date Action";
					StoryTxt.text = "";
					SkipText.text = "Main Action";
					NextText.text = "(In Story)";

					// No responses here
					ShowBoxes (null, null, null);	
				} else if (AfterFadeState == ActState.MoveToNewLocation) {
					StoryTxtHeader.text = "Date Action";
					SkipText.text = "Main Action";
					NextText.text = "(Moving Locations)";

					if (TooSoonToMove()) {
						StoryTxt.text = "You can't move to a new date location yet; you basically just arrived here!";
						ShowBoxes (null, "Ok", null);
					} else {
						TokenHandler leadPlayer = MapHandler.GetComponent<MapHandler> ().LeadPlayer.GetComponent<TokenHandler> ();
						TokenHandler leadDate = MapHandler.GetComponent<MapHandler> ().LeadDate.GetComponent<TokenHandler> ();
						if (phaseHandler.thisHour == 7) {
							StoryTxt.text = "You leave the romantic highway overlook behind, and move to your backyard, doding fans along the way.";
							StoryTxt.text += "\n\n  +25 Atmosphere";
							MapHandler.GetComponent<MapHandler> ().LeadPlayerScript.Atmosphere = "B";
							ShowBoxes (null, "Resume Date", null);

							// Actually move
							leadPlayer.MoveToTile(9, 5);
							leadDate.MoveToTile(9, 4);
						} else if (phaseHandler.thisHour == 8) { 
							StoryTxt.text = "Your friend with the fancy house invites you both to hang out by the pool.";
							StoryTxt.text += "\n\n  +50 Atmosphere";
							MapHandler.GetComponent<MapHandler> ().LeadPlayerScript.Atmosphere = "A+";
							ShowBoxes (null, "Resume Date", null);

							// Actually move
							leadPlayer.MoveToTile(4, 9);
							leadDate.MoveToTile(5, 9);
						} else if (phaseHandler.thisHour == 9) { 
							StoryTxt.text = "You both had a great time! You part and go your separate ways.";
							ShowBoxes (null, "Date Complete!", null);

							// Actually move ("home")
							leadPlayer.MoveToTile(3, 4);
							leadDate.MoveToTile(8, 8);
						}

						// Kill all fans
						MapHandler.GetComponent<MapHandler>().DestroyAllFans();
						if (phaseHandler.thisHour == 7) {
							MapHandler.GetComponent<MapHandler> ().SpawnFans (4);
						} else if (phaseHandler.thisHour == 8) {
							MapHandler.GetComponent<MapHandler> ().SpawnFans (5);
						} // No fans when going home


						// Actually do our update
						phaseHandler.UpdateHour();
					}
				} else {
					// Standard date text
					DateDialogue dd = dateDialogues.DialogueOptions [rng.Next(dateDialogues.DialogueOptions.Count)];
					StoryTxtHeader.text = "Date Dialogue";
					StoryTxt.text = dd.storyText;

					// Set response text
					ShowBoxes (
						dd.option1,
						dd.option2,
						dd.option3
					);
				}
					
				// Hide stamps; change to fade-in
				RespStamp1.GetComponent<StampHandler>().HideStamp ();
				RespStamp2.GetComponent<StampHandler>().HideStamp ();
				RespStamp3.GetComponent<StampHandler>().HideStamp ();
				CurrState = ActState.FadingTextIn;
			}
		}

		// Deal with text fading?
		if (CurrState == ActState.FadingTextIn) {
			bool overflow = false;
			float newAlpha = StoryTxt.color.a + 1.2f * Time.deltaTime;
			if (newAlpha >= 0.85) {
				newAlpha = 1;
				overflow = true;
			}
			Color newAlphaColor = new Color (StoryTxt.color.r, StoryTxt.color.g, StoryTxt.color.b, newAlpha);
			StoryTxt.color = newAlphaColor;
			StoryTxtHeader.color = newAlphaColor;
			Response1.GetComponentInChildren <Text> ().color = newAlphaColor;
			Response2.GetComponentInChildren <Text> ().color = newAlphaColor;
			Response3.GetComponentInChildren <Text> ().color = newAlphaColor;
			if (overflow) {
				if (AfterFadeState == ActState.DateAction) {
					SetupDateTurn ();
				} else if (AfterFadeState == ActState.FansAction) {
					StartFansActionState ();
				} else if (AfterFadeState == ActState.PlayerActionSelect) {
					SetupChoosePlayerAction ();	
				} else {
					CurrState = AfterFadeState;
				}
			}
		}
			
		// Test-driven development for heart animation... >3>
		if (Input.GetKey (KeyCode.F)) {
			MapHandler.GetComponent<MapHandler> ().CreateHeart(5,5);
			return;
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

		// Deal with NPCs
		if (NPCMoveCount < NPCMoveCountMax) {
			AdvanceNPCMoveCounter (Time.deltaTime);
		}
	}
}
