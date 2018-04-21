using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour {
	public Text StoryTxt;
	public Text Response1;
	public Text Response2;
	public Text Response3;

	private DateDialogues dateDialogues;


	public void SetupConversation() {
		DateDialogue dd = dateDialogues.DialogueOptions [0];

		StoryTxt.text = dd.storyText;
		Response1.text = dd.option1;
		Response2.text = dd.option2;
		Response3.text = dd.option3;



	}



	// Use this for initialization
	void Start () {
		dateDialogues = new DateDialogues ();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
