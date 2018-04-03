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

	public enum PlayerChoices {
		NONE,
		ATTACK,
		DEFENCE,
		RUN
	}

	public GameObject player;
	public List<GameObject> enemies;
	public int movementSpeed;
	public GameObject playerMenu;

	private BattleStates currentState;
	private AttackingStates attackState;
	private PlayerChoices playerChoice;
	private float attackTimer;
	private bool isActionFinished;
	private GameObject playerTarget;
	private int enemyTurn;

	void Start () {
		currentState = BattleStates.START;
		attackTimer = -10.0f;

		Button attackButton = (Button)playerMenu.transform.Find ("AttackButton").GetComponent<Button> ();
		attackButton.onClick.AddListener (attackClick);
		Button defenceButton = (Button)playerMenu.transform.Find ("DefenceButton").GetComponent<Button> ();
		defenceButton.onClick.AddListener (defenceClick);
		Button runButton = (Button)playerMenu.transform.Find ("RunButton").GetComponent<Button> ();
		runButton.onClick.AddListener (runClick);
	}

	void Update () {
		isActionFinished = false;

		switch (currentState) {
		case BattleStates.START:
			playerChoice = PlayerChoices.NONE;
			playerTarget = null;
			enemyTurn = 1;
			isActionFinished = true;
			player.GetComponent<Animator> ().SetBool ("Defence", false);
			break;
		case BattleStates.PLAYERTURN:
			playerSelecting ();
			break;
		case BattleStates.PLAYERANIMATION:
			playerMenu.SetActive (false);
			playerAnimation ();
			break;
		case BattleStates.ENEMYTURN:
			CharacterAttack (enemies [enemyTurn - 1], player);
			if (isActionFinished && enemies.Count != enemyTurn) {
				isActionFinished = false;
				enemyTurn++;
			}
			break;
		case BattleStates.WIN:
			break;
		case BattleStates.LOSE:
			break;
		}
		if (isActionFinished) {
			nextBattleState ();
		}
	}

	private void playerSelecting () {
		switch (playerChoice) {
		case PlayerChoices.NONE:
			playerMenu.SetActive (true);
			break;
		case PlayerChoices.ATTACK:
			if (Input.GetMouseButtonUp(0)) {
				Vector3 mouse = Input.mousePosition;
				Ray castPoint = Camera.main.ScreenPointToRay(mouse);
				RaycastHit cameraHit;

				if (Physics.Raycast (castPoint, out cameraHit, Mathf.Infinity)) {
					if (cameraHit.transform.gameObject.tag == "Enemy") {
						playerTarget = cameraHit.transform.gameObject;
						isActionFinished = true;
					}
				}
			}
			break;
		case PlayerChoices.DEFENCE:
			isActionFinished = true;
			break;
		case PlayerChoices.RUN:
			isActionFinished = true;
			break;
		}
	}

	private void playerAnimation() {
		switch (playerChoice) {
		case PlayerChoices.ATTACK:
			CharacterAttack (player, playerTarget);
			break;
		case PlayerChoices.DEFENCE:
			player.GetComponent<Animator> ().SetBool ("Defence", true);
			isActionFinished = true;
			break;
		}
	}

	private void CharacterAttack(GameObject character, GameObject target) {
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
				target.GetComponent<Animator> ().SetBool ("Damaged", true);
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
				attackState = AttackingStates.GOING;
				isActionFinished = true;
			}
			break;
		}
	}

	private void attackClick() {
		playerMenu.SetActive (false);
		playerChoice = PlayerChoices.ATTACK;
	}

	private void defenceClick() {
		playerChoice = PlayerChoices.DEFENCE;
	}

	private void runClick() {
		playerChoice = PlayerChoices.RUN;
	}

	private void nextBattleState() {
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
			currentState = BattleStates.START;
			break;
		}
		Debug.Log (currentState);
	}
}