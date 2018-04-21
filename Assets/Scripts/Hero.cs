using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {
	public GameObject HeroCube;
	public GameObject HeroShield;

	//Movement Tech
	private float heroMoveAmt = 0.2f;

	//Shield Attack
	private Vector3 jumpSpeed = new Vector3(0f, 0.1f, 0f);
	private float jumpReach = 2;
	private int jumping = 0;

	private float shieldAttackSpeed = .2f; 
	private int shieldAttacking = 0;
	private Vector3 shieldStartPosition = new Vector3 (1, 0, 0);
	private Vector3 shieldEndPosition = new Vector3 (6, 0, 0);
	private float shieldWiggle = .2f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		//Move 
		int moveAmt = 0;
		if ((Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow))) {
			moveAmt -= 1;
		}
		if ((Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow))) {
			moveAmt += 1;
		}
		if (moveAmt != 0) {
			Move (moveAmt);
		}

		// Jump
		if (Input.GetKey(KeyCode.W)) {
			Jump ();
		}

		if (jumping != 0) {
			HandleJump();
		}

		//Shield
		if (Input.GetKey (KeyCode.J) && shieldAttacking == 0) {
			ShieldAttack ();
		}
		if (shieldAttacking != 0) {
			HandleShieldAttack();
		}
	}

	// dir = (-1, 0, 1) for left/none/right
	void Move (int dir) {
		HeroCube.transform.localPosition += new Vector3 (dir * heroMoveAmt, 0, 0);
	}

	void Jump () {
		jumping = 1;
	}

	void ShieldAttack () {
		shieldAttacking = 1;
	}

	void HandleJump() {
		if (jumping == 1) {
			HeroCube.transform.localPosition += jumpSpeed;
			if (HeroCube.transform.localPosition.y >= jumpReach) {
				jumping *= -1;
			}
		}

		else if (jumping == -1) {
			HeroCube.transform.localPosition -= jumpSpeed;
			if (HeroCube.transform.localPosition.y <= 1) {
				jumping = 0;
				HeroCube.transform.localPosition = new Vector3 (0, 1, 0);
			}
		}
	}

	void HandleShieldAttack () {
		//HeroShield
		if (shieldAttacking == 1) {
			print ("Shield Attacking");
			//Move
			//HeroShield.transform.localPosition += new Vector3 (shieldAttackSpeed, 0, 0);
			HeroShield.transform.localPosition = Vector3.Lerp(HeroShield.transform.position, shieldEndPosition,.1f);
			//Check
			if (HeroShield.transform.localPosition.x >= (shieldEndPosition.x - shieldWiggle)) {
				shieldAttacking *= -1;
			}
		} 
		else if (shieldAttacking == -1) {
			print ("Shield Returning");
			//Move
			//HeroShield.transform.localPosition -= new Vector3(shieldAttackSpeed, 0, 0);

			//Check
			if (HeroShield.transform.localPosition.x <= (shieldStartPosition.x + shieldWiggle)) {
				shieldAttacking = 0;
				//Snap
				//HeroShield.transform.localPosition = new Vector3 (1, 0, 0);
				HeroShield.transform.localPosition = Vector3.Lerp(HeroShield.transform.localPosition, shieldStartPosition,.1f);

			}
		}

	}
}
