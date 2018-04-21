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



	private DateDialogues dateDialogues;


	public void SetupConversation() {
		DateDialogue dd = dateDialogues.DialogueOptions [0];

		StoryTxt.text = dd.storyText;
		Response1.GetComponentInChildren <Text>().text = dd.option1;
		Response2.GetComponentInChildren <Text>().text = dd.option2;
		Response3.GetComponentInChildren <Text>().text = dd.option3;
	}

	public void ChooseOption(int opt) {
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
	}

	public void ChoiceParticlesDone() {
		// Clear boxes.
		Response1.gameObject.SetActive (false);
		Response2.gameObject.SetActive (false);
		Response3.gameObject.SetActive (false);

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
