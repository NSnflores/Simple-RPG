using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

	private BattleStates currentState;
	private AttackingStates attackState;
	private float attackTimer;

	void Start () {
		currentState = BattleStates.START;
		attackTimer = -10.0f;
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (currentState);
		switch (currentState) {
		case BattleStates.START:
			currentState = BattleStates.PLAYERTURN;
			break;
		case BattleStates.PLAYERTURN:
			currentState = BattleStates.PLAYERANIMATION;
			attackState = AttackingStates.GOING;
			break;
		case BattleStates.PLAYERANIMATION:
			CharacterAttack(player, enemies[0]);
			break;
		case BattleStates.ENEMYTURN:
			break;
		case BattleStates.WIN:
			break;
		case BattleStates.LOSE:
			break;
		}
	}

	private void CharacterAttack(GameObject char1, GameObject char2) {
		Transform char1Model = getModel (char1);
		NavMeshAgent nav = char1Model.GetComponent<NavMeshAgent> ();

		switch (attackState) {
		case AttackingStates.GOING:
			char1Model.GetComponent<Animator> ().SetBool ("Running", true);
			nav.SetDestination (char2.transform.position);

			float distanceToTarget = Vector3.SqrMagnitude (char1Model.transform.position - char2.transform.position);
			if (distanceToTarget < 6) {
				attackState = AttackingStates.ATTACKING;
			}
			break;
		case AttackingStates.ATTACKING:
			if (attackTimer <= -10.0f) {
				char1Model.GetComponent<Animator> ().SetBool ("Attacking", true);
				attackTimer = 1.05f;
			} else if (attackTimer <= 0.0f) {
				attackState = AttackingStates.RETURNING;
				attackTimer = -10.0f;
			}
			attackTimer -= Time.deltaTime;
			break;
		case AttackingStates.RETURNING:
			nav.SetDestination (char1.transform.position);

			float distanceToOrigin = Vector3.SqrMagnitude (char1Model.transform.position - char1.transform.position);
			Debug.Log (distanceToOrigin);
			if (distanceToOrigin < 3) {
				char1Model.GetComponent<Animator> ().SetBool ("Running", false);
			}
			break;
		}
	}

	private Transform getModel(GameObject character) {
		foreach (Transform child in character.GetComponent<Transform> ()) {
			if (child.CompareTag ("Model")) {
				return child;
			}
		}
		return null;
	}
}