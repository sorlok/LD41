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

	// Things your date can be doing when interacting with you vs. fans
	public List<string> DateToYouInteractions = MakeDateToYouInteractions();
	public List<string> DateToFansInteractions = MakeDateToFansInteractions();


	private static List<DateDialogue> MakeDialogueOptions() {
		List<DateDialogue> res = new List<DateDialogue> ();

		res.Add ( new DateDialogue("Your date tells you a story about how they got lost\nin the tropics and defeated some dinosaurs.\n\nWhat do you say?", "*compliment them*", "*laugh manically*", "*feign boredom*") );
		res.Add ( new DateDialogue("Your date waxes poetic about the futility of life.\n\nHow do you respond?", "*sigh meaningfully*", "I agree!", "*show disdain for their opinion*") );
		res.Add ( new DateDialogue("Your date shares and embarassing story from their\nchildhood. They blush all throughout the telling.\n\nHow do you engage?", "*mock their youthful indiscretion*", "*share a similar tale*", "*admire their forthrightness*") );
		res.Add ( new DateDialogue("Your date tells you about their kooky family.\nThe tales are told with a mixture of affection\n and disdain.\n\nWhat do you say?", "*mirror their expressions*", "*share your family history*", "*insult their family*") );
		res.Add ( new DateDialogue("While desperately holding back drool, your\ndate tells you about their favorite cuisine!\n\nDo you share their taste?", "Yes, absolutely!", "That food tastes alright.", "Ew, no.") );
		res.Add ( new DateDialogue("Your date asks you to comment on a flirty\nphoto they sent you earlier today.\n\nHow do you respond?", "It made my heart skip a beat.", "I've used that pose before!", "You're so stupid.") );
		res.Add ( new DateDialogue("Your date stops what they are doing and say, 'I\nreally like you.'\n\nHow do you respond?", "I really like you too.", "I'm...so flattered!", "Well, you kinda suck.") );
		res.Add ( new DateDialogue("Your date asks, 'Do you like jokes? I am a fan of\npun-ishment.'\n\nWhat do you say?", "I like bad puns and I cannot lie.", "You know what you did.", "Ugh!") );
		res.Add ( new DateDialogue("Your date asks, 'Do you like jokes? What's brown\nand sticky?'\n\nWhat do you say?", "Poop!", "A brown stick.", "I don't know.") );
		res.Add ( new DateDialogue("Your date asks, 'Do you like horror flicks? Boo!'\n\nHow do you respond?", "Braaaaains...", "Eep!", "*throw table salt*") );

		return res;
	}

	private static List<DateDialogue> MakeGoodResponses() {
		List<DateDialogue> res = new List<DateDialogue> ();

		res.Add ( new DateDialogue("Your date looks at you dreamily.\n\n\"Wow, what a nice thing to say!\"", null, "I know, right?", null) );
		res.Add ( new DateDialogue("Your date looks happy!\n\n\"No one has ever said that to me before!\"", null, "You deserve it!", null) );
		res.Add ( new DateDialogue("Your date giggles excitedly!\n\n\"What a clever thing to say!\"", null, "They call me Mr[s]. Clever!", null) );



		return res;
	}

	private static List<DateDialogue> MakeNeutralResponses() {
		List<DateDialogue> res = new List<DateDialogue> ();

		res.Add ( new DateDialogue("Your date looks confused.\n\n\"Wow, what a... nice (?) thing to say...\"", null, "You deserve it!", null) );
		res.Add ( new DateDialogue("Your date does a double-take.\n\n\"I can't tell if you're joking.\"", null, "(Wink)", null) );
		res.Add ( new DateDialogue("Your date squints at you.\n\n\"I'm not sure I agree.\"", null, "Fair Enough", null) );

		return res;
	}

	private static List<DateDialogue> MakeBadResponses() {
		List<DateDialogue> res = new List<DateDialogue> ();

		res.Add ( new DateDialogue("Your date looks upset!\n\n\"Are you even TRYING to make a good impression?\"", null, "Eh, not really", null) );
		res.Add ( new DateDialogue("Your date exclaims in shock.\n\n\"Harumph! This is why you're single!\"", null, "Oh there's many reasons", null) );
		res.Add ( new DateDialogue("Your date frowns.\n\n\"That's not a nice thing to say.\"", null, "If you say so", null) );

		return res;
	}

	private static List<string> MakeDateToYouInteractions() {
		List<string> res = new List<string> ();

		res.Add ("Your date explains their view on current events...");
		res.Add ("Your date talks about something they think is interesting...");
		res.Add ("Your date describes their family in excruciating detail...");
		res.Add ("You date lobs ice breakers generated by a popular app...");
		res.Add ("Your date suggests an arm wrestling competition...");
		res.Add ("Your date describes their latest Netflix binges...");
		res.Add ("Your date demands to know your Korean boy band crushes...");
		res.Add ("Your date suggests forming a team to compete in Ludum Dare...");
		res.Add ("Your date yawns incredibly loudly...");

		return res;
	}

	private static List<string> MakeDateToFansInteractions() {
		List<string> res = new List<string> ();

		res.Add ("Your date is tweeting a picture of their food...");
		res.Add ("Your date is searching for the perfect instagram filter...");
		res.Add ("Your date fills out a personality quiz on their phone...");
		res.Add ("Your date completes a survey and finds out they're in Hufflepuff...");
		res.Add ("Your date opens Facebook and instantly regrets it...");
		res.Add ("Your date discreetly checks on mentions from their mutuals...");
		res.Add ("Your date answers a quick phone call from their parents...");
		res.Add ("Your date tries giggles after viewing some funny GIFs...");
		res.Add ("Your date is busy checking their Ludum Dare entry's ratings page...");

		return res;
	}
}
