using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DeskPaperPuzzleManager : MonoBehaviour {

	[SerializeField] TextMeshProUGUI lockdownText;
	[SerializeField] List<Light> lockdownLights;
	[SerializeField] AudioSource speakerSource;
	[SerializeField] AudioClip lockdownClip;
	[SerializeField] AudioClip unlockdownClip;
	[SerializeField] AudioClip successClip;
	[SerializeField] float shrinkDuration = 1f;

	[SerializeField] GameObject bathroomsExit;
	[SerializeField] GameObject officeExit;

	List<GameObject> papers = new List<GameObject>();
	string[] solution = { "yellow", "red", "white", "blue", "green" };
	bool[] progress = { false, false, false, false, false };
	bool solved = false;

	public void Start() {
		speakerSource.PlayOneShot(lockdownClip);
	}

	//Win debug key
	//private void Update() {
	//	if (Keyboard.current.spaceKey.wasPressedThisFrame) {
	//		win();
	//	}
	//}

	public void paperPlaced(SelectEnterEventArgs args) {
		if (solved) return;

		GameObject paper = args.interactableObject.transform.gameObject;
		if (!papers.Contains(paper)) {
			papers.Add(paper);
		}

		string objectName = args.interactableObject.transform.name;
		string socketName = args.interactorObject.transform.name;

		int deskIndex = ExtractDeskIndex(socketName);
		string paperColor = ExtractPaperColor(objectName);

		if (solution[deskIndex] == paperColor) {
			progress[deskIndex] = true;
		}

		for (int i = 0; i < progress.Length; i++) {
			if (!progress[i]) { return; }
		}

		solved = true;

		win();

	}

	private void win() {
		foreach (GameObject p in new List<GameObject>(papers)) {
			if (p == null) continue;
			IXRSelectInteractable interactable = p.GetComponent<IXRSelectInteractable>();
			StartCoroutine(Shrink(p, interactable));
		}
		lockdownText.text = "LOCKDOWN\nPROTOCOL\nDISENGAGED";
		lockdownText.color = Color.limeGreen;

		for (int i = 0; i < lockdownLights.Count; i++) {
			lockdownLights[i].color = Color.limeGreen;
		}

		speakerSource.PlayOneShot(unlockdownClip);
		speakerSource.PlayOneShot(successClip);

		bathroomsExit.SetActive(true);
		officeExit.SetActive(true);

		GameStateManager.Instance.MarkSolved(Room.Classroom);
		Debug.Log("YOU HAVE SOLVED THE PAPER PUZZLE");
	}

	public void paperRemoved(SelectExitEventArgs args) {
		if (solved) return;

		papers.Remove(args.interactableObject.transform.gameObject);
		string socketName = args.interactorObject.transform.name;
		int deskIndex = ExtractDeskIndex(socketName);
		progress[deskIndex] = false;
	}

	private int ExtractDeskIndex(string s) {
		string[] parts = s.Split('_');
		return int.Parse(parts[1]);
	}

	private string ExtractPaperColor(string s) {
		s = s.Replace("(Clone)", "").Trim();
		string[] parts = s.Split('_');
		return parts[1].ToLowerInvariant();
	}

	IEnumerator Shrink(GameObject target, IXRSelectInteractable interactable) {
		yield return null;
		if (target == null) yield break;

		Vector3 heldPos = target.transform.position;
		Quaternion heldRot = target.transform.rotation;

		MonoBehaviour interactableComponent = interactable as MonoBehaviour;
		if (interactableComponent != null) {
			interactableComponent.enabled = false;
		}

		Rigidbody rb = target.GetComponent<Rigidbody>();
		FreezeBody(rb);

		foreach (Collider c in target.GetComponentsInChildren<Collider>()) {
			c.enabled = false;
		}

		target.transform.SetPositionAndRotation(heldPos, heldRot);

		Vector3 initialScale = target.transform.localScale;
		float elapsedTime = 0f;

		while (elapsedTime < shrinkDuration) {
			if (target == null) yield break;

			FreezeBody(rb);
			target.transform.SetPositionAndRotation(heldPos, heldRot);

			elapsedTime += Time.deltaTime;
			target.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, elapsedTime / shrinkDuration);
			yield return null;
		}

		if (target != null) {
			papers.Remove(target);
			Destroy(target);
		}
	}

	private void FreezeBody(Rigidbody rb) {
		if (rb == null) return;
		if (!rb.isKinematic) {
			rb.linearVelocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}
		rb.useGravity = false;
		rb.isKinematic = true;
	}
}