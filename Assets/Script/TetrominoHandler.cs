using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoHandler : MonoBehaviour {
	[SerializeField]
	private float fallSpeed = 1.0f;

	private float fall = 0.0f;

	[SerializeField]
	private bool allowRotation = true;

	[SerializeField]
	private bool limitRotation = false;

	private GameplayManager gameplayManager;

	private void Start(){
		gameplayManager = FindObjectOfType<GameplayManager> ();
	}

	private void Update(){		
		UpdateTetromino ();
		InputKeyboardHandler ();
	}

	private void UpdateTetromino(){
		if (Time.time - fall >= fallSpeed) {
			Handler ("Down");
			fall = Time.time;
		}
	}

	private void InputKeyboardHandler(){
		if (Input.GetKeyDown (KeyCode.RightArrow))
			Handler ("Right");
		if (Input.GetKeyDown (KeyCode.LeftArrow))
			Handler ("Left");
		if (Input.GetKeyDown (KeyCode.DownArrow))
			Handler ("Down");
		if (Input.GetKeyDown (KeyCode.UpArrow))
			Handler ("Action");
	}

	private void Handler(string command){
		switch (command) 
		{
		case "Right":
			MoveHorizontal (Vector3.right);
			break;
		case "Left":
			MoveHorizontal (Vector3.left);
			break;
		case "Down":
			MoveVertical ();
			break;
		case "Action":
			if (allowRotation) {
				ActionLimitRotation (1);

				if (!IsInValidPosition ())
					ActionLimitRotation (-1);
				else
					gameplayManager.UpdateGrid (this);
			}
			break;
		}
	}

	private void ActionLimitRotation(int modifier){
		if (limitRotation) {
			if (transform.rotation.eulerAngles.z >= 90)
				transform.Rotate (Vector3.forward * -90);
			else
				transform.Rotate (Vector3.forward * 90);
		}
		else
			transform.Rotate (Vector3.forward * 90 * modifier);
	}

	private void MoveVertical(){
		transform.position += Vector3.down;

		if (!IsInValidPosition ()) {
			transform.position += Vector3.up;

			gameplayManager.DestroyRow ();

			enabled = false;

			if (gameplayManager.IsReactLimitedGrid (this)) {
				gameplayManager.GameOver (this);
			} else {
				gameplayManager.GenerateTetromino ();
			}
		}
		else
			gameplayManager.UpdateGrid (this);
	}

	private void MoveHorizontal(Vector3 direction){
		transform.position += direction;

		if (!IsInValidPosition ())
			transform.position += direction * -1;
		else
			gameplayManager.UpdateGrid (this);
	}

	private bool IsInValidPosition(){
		foreach (Transform mino in transform) {
			Vector3 pos = gameplayManager.Round(mino.position);

			if (!gameplayManager.IsTetrominoInsideAGrid (pos)) {
				return false;
			}

			if (gameplayManager.GetTransformAtGridPosition(pos) != null &&
				gameplayManager.GetTransformAtGridPosition(pos).parent != transform) {
				return false;
			}
		}

		return true;
	}
}
