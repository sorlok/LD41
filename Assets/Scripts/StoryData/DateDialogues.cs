using System.Collections.Generic;

// Storage-only class: Date dialogue options

public class DateDialogue {
	public string storyText;
	public string option1;
	public string option2;
	public string option3;

	public DateDialogue(string txt, string o1, string o2, string o3) {
		storyText = txt;
		option1 = o1;
		option2 = o2;
		option3 = o3;
	}
}

public class DateDialogues {
	public List<DateDialogue> DialogueOptions = MakeDialogueOptions();

	private static List<DateDialogue> MakeDialogueOptions() {
		List<DateDialogue> res = new List<DateDialogue> ();

		res.Add (new DateDialogue("Your date tells you a story about how they got lost\nin the tropics and defeated some dinosaurs.\n\nWhat do you say?", "Compliment Them", "Laugh Manically", "Feign Boredom") );


		return res;
	}

}
