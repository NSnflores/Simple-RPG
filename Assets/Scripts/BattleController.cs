using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BattleController : MonoBehaviour {
	public enum BattleStates {
		START,
		PLAYERTURN,
		PLAYERANIMATION,
		ENEMYTURN,
		WIN,
		LOSE
	}

	public enum AttackingStates {
		GOING,
		ATTACKING,
		RETURNING
	}

	public GameObject player;
	public List<GameObject> enemies;
	public Button attackButton;
	public int movementSpeed;

	private BattleStates currentState;
	private AttackingStates attackState;
	private float attackTimer;
	private bool isActionFinished;

	void Start () {
		currentState = BattleStates.START;
		attackTimer = -10.0f;
		attackButton.onClick.AddListener (attackClick);
	}

	void Update () {
		//Debug.Log (currentState);
		isActionFinished = false;
		switch (currentState) {
		case BattleStates.START:
			nextBattleState ();
			break;
		case BattleStates.PLAYERTURN:
			break;
		case BattleStates.PLAYERANIMATION:
			CharacterAttack (player);
			break;
		case BattleStates.ENEMYTURN:
			CharacterAttack(enemies[0]);
			break;
		case BattleStates.WIN:
			break;
		case BattleStates.LOSE:
			break;
		}
		if (isActionFinished) {
			nextBattleState ();
			attackState = AttackingStates.GOING;
		}
	}

	private void CharacterAttack(GameObject character) {
		Vector3 moveDirection = Vector3.zero;

		switch (attackState) {
		case AttackingStates.GOING:
			character.GetComponent<Animator> ().SetBool ("Running", true);
			moveDirection = new Vector3 (0, 0, movementSpeed);
			character.transform.Translate (moveDirection * Time.deltaTime);

			if (Mathf.FloorToInt(character.transform.position.z) == 0) {
				attackState = AttackingStates.ATTACKING;
			}

			break;
		case AttackingStates.ATTACKING:
			if (attackTimer <= -10.0f) {
				character.GetComponent<Animator> ().SetBool ("Attacking", true);
				attackTimer = 1.05f;
			} else if (attackTimer <= 0.0f) {
				character.transform.localRotation *= Quaternion.Euler(0, 180, 0);
				attackState = AttackingStates.RETURNING;
				attackTimer = -10.0f;
			}
			attackTimer -= Time.deltaTime;
			break;
		case AttackingStates.RETURNING:
			character.GetComponent<Animator> ().SetBool ("Running", true);
			moveDirection = new Vector3 (0, 0, movementSpeed);
			character.transform.Translate (moveDirection * Time.deltaTime);

			if (Mathf.Abs(character.transform.position.z) >= 10) {
				character.GetComponent<Animator> ().SetBool ("Running", false);
				character.transform.localRotation *= Quaternion.Euler(0, 180, 0);
				isActionFinished = true;
			}
			break;
		}
	}

	private void attackClick() {
		nextBattleState ();
	}

	private void nextBattleState() {
		Debug.Log ("======");
		Debug.Log (currentState);
		switch (currentState) {
		case BattleStates.START:
			currentState = BattleStates.PLAYERTURN;
			break;
		case BattleStates.PLAYERTURN:
			currentState = BattleStates.PLAYERANIMATION;
			break;
		case BattleStates.PLAYERANIMATION:
			currentState = BattleStates.ENEMYTURN;
			break;
		case BattleStates.ENEMYTURN:
			currentState = BattleStates.PLAYERTURN;
			break;
		}
		Debug.Log (currentState);
	}
}