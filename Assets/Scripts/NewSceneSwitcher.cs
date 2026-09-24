using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class NewSceneSwitcher : MonoBehaviour {
	public XRBaseInteractor rh, lh;
	private static List<GameObject> toMove = new();
	public InputActionReference b;
	public string newRoom;
	public Vector3 targetPos;
	XRBaseInteractable doorInteractable;
	private static int lastSwitchFrame = -1;

	void Awake() {
		if (doorInteractable == null) doorInteractable = GetComponentInParent<XRBaseInteractable>();
		if (doorInteractable == null) Debug.LogError(name + ": no XRBaseInteractable found on this object or its parents");
	}

	void OnEnable() {
		b.action.performed += OnPressed;
		b.action.Enable();
	}

	void OnDisable() {
		b.action.performed -= OnPressed;
	}

	void OnPressed(InputAction.CallbackContext ctx) {
		if (doorInteractable == null || !doorInteractable.isHovered) return;
		SwitchScene(newRoom, targetPos);
	}

	public void SwitchScene(string newSceneName, Vector3 newPos) {
		if (newSceneName != "StevenScene" && newSceneName != "RishiScene" &&
			newSceneName != "Bathrooms" && newSceneName != "GioScene") {
			Debug.Log("INVALID SCENE NAME");
			return;
		}
		if (Time.frameCount == lastSwitchFrame) return;
		lastSwitchFrame = Time.frameCount;

		for (int i = 0; i < toMove.Count; i++) {
			if (toMove[i] == null) continue;
			SceneManager.MoveGameObjectToScene(toMove[i], SceneManager.GetActiveScene());
		}
		toMove.Clear();

		if (rh.hasSelection) {
			IXRSelectInteractable heldInteractable = rh.interactablesSelected[0];

			rh.interactionManager.CancelInteractableSelection(heldInteractable);

			GameObject heldObject = heldInteractable.transform.gameObject;

			toMove.Add(heldObject);
		}
		if (lh.hasSelection) {
			IXRSelectInteractable heldInteractable = lh.interactablesSelected[0];
			lh.interactionManager.CancelInteractableSelection(heldInteractable);
			GameObject heldObject = heldInteractable.transform.gameObject;
			if (!toMove.Contains(heldObject)) toMove.Add(heldObject);
		}
		for (int i = 0; i < toMove.Count; i++) {
			toMove[i].transform.SetParent(null);
			DontDestroyOnLoad(toMove[i]);
		}

		if (newSceneName == "StevenScene") {
			if (GameStateManager.Instance.IsSolved(Room.Office)) {
				SceneManager.LoadScene(newSceneName + "Solved");
			} else {
				SceneManager.LoadScene(newSceneName);
			}
		} else if (newSceneName == "RishiScene") {
			if (GameStateManager.Instance.IsSolved(Room.Classroom)) {
				SceneManager.LoadScene(newSceneName + "Solved");
			} else {
				SceneManager.LoadScene(newSceneName);
			}
		} else if (newSceneName == "Bathrooms") {
			if (GameStateManager.Instance.IsSolved(Room.Bathroom)) {
				SceneManager.LoadScene(newSceneName + "Solved");
			} else {
				SceneManager.LoadScene(newSceneName);
			}

		} else if (newSceneName == "GioScene") {
			if (GameStateManager.Instance.IsSolved(Room.UtilityCloset)) {
				SceneManager.LoadScene(newSceneName + "Solved");
			} else {
				SceneManager.LoadScene(newSceneName);
			}
		}

		for (int i = 0; i < toMove.Count; i++) {
			Vector3 pos = newPos + new Vector3(0.3f * i, 1, 0);
			if (toMove[i].TryGetComponent(out Rigidbody rb)) {
				rb.linearVelocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
			}
			toMove[i].transform.SetPositionAndRotation(pos, Quaternion.identity);
		}
	}
}