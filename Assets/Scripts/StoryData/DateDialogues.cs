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
	// The actual stories.
	public List<DateDialogue> DialogueOptions = MakeDialogueOptions();

	// The responses, based on mood
	public List<DateDialogue> DialogueGoodResponses = MakeGoodResponses();
	public List<DateDialogue> DialogueNeutralResponses = MakeNeutralResponses();
	public List<DateDialogue> DialogueBadResponses = MakeBadResponses();


	private static List<DateDialogue> MakeDialogueOptions() {
		List<DateDialogue> res = new List<DateDialogue> ();

		res.Add ( new DateDialogue("Your date tells you a story about how they got lost\nin the tropics and defeated some dinosaurs.\n\nWhat do you say?", "Compliment Them", "Laugh Manically", "Feign Boredom") );
		res.Add ( new DateDialogue("Your date waxes poetic about the futility of life.\n\nHow do you respond?", "Sigh Meaningfully", "Agree Enthusiastically", "Show disdain for their opinion.") );
		res.Add ( new DateDialogue("Your date shares and embarassing story from their\nchildhood. They blush all throughout the telling.\n\nHow do you engage?", "Mock their youthful indiscretion.", "Share a similar tale.", "Admire their forthrightness.") );
		res.Add ( new DateDialogue("Your date tells you abou their kooky family.\nThe tales are told with a mixture of affection\n and disdain.\n\nWhat do you say?", "Mirror their expressions.", "Share your family history.", "Insult their family.") );
		res.Add ( new DateDialogue("While desperately holding back drool, your\ndate tells you about their favorite cuisine!\n\nDo you share their taste?", "Yes, absolutely!", "That food tastes alright.", "Ew, no.") );
		res.Add ( new DateDialogue("Your date asks you to comment on a flirty\nphoto they sent you earlier today.\n\nHow do you respond?", "It made my heart skip a beat.", "I've used that pose before!", "You're so stupid.") );

		return res;
	}

	private static List<DateDialogue> MakeGoodResponses() {
		List<DateDialogue> res = new List<DateDialogue> ();

		res.Add ( new DateDialogue("Your date looks at you dreamily.\n\n\"Wow, what a nice thing to say!\"", null, "I know, right?", null) );

		return res;
	}

	private static List<DateDialogue> MakeNeutralResponses() {
		List<DateDialogue> res = new List<DateDialogue> ();

		res.Add ( new DateDialogue("Your date looks confused\n\n\"Wow, what a... nice (?) thing to say...\"", null, "You deserve it!", null) );

		return res;
	}

	private static List<DateDialogue> MakeBadResponses() {
		List<DateDialogue> res = new List<DateDialogue> ();

		res.Add ( new DateDialogue("Your date looks upset\n\n\"Are you even TRYING to make a good impression?\"", null, "Eh, not really", null) );

		return res;
	}
}
