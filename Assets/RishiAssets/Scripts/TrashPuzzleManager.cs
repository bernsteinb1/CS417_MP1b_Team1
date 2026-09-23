using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TrashPuzzleManager : MonoBehaviour {

	[SerializeField] GameObject shelf0;
	[SerializeField] GameObject shelf1;
	[SerializeField] AudioSource shelfAudio;
	[SerializeField] AudioClip shelfClip;
	[SerializeField] float shrinkDuration = 1f;
	[SerializeField] float raiseDuration = 5f;
	[SerializeField] float raiseHeight = 3f;

	bool redTrash = false;
	bool greenTrash = false;
	bool solved = false;

	public void trashPlaced(SelectEnterEventArgs args) {
		string objectName = args.interactableObject.transform.name;
		string socketName = args.interactorObject.transform.name;

		StartCoroutine(Shrink(args.interactableObject));

		if (solved) return;

		string canColor = ExtractColor(socketName);
		string trashColor = ExtractColor(objectName);

		if (canColor == trashColor) {
			if (canColor == "red") {
				redTrash = true;
			} else if (canColor == "green") {
				greenTrash = true;
			}
		}

		if (redTrash && greenTrash) {
			solved = true;
			shelfAudio.PlayOneShot(shelfClip);
			StartCoroutine(Raise(shelf0));
			StartCoroutine(Raise(shelf1));
			Debug.Log("YOU HAVE SOLVED THE TRASH PUZZLE");
		}
	}

	private string ExtractColor(string s) {
		s = s.Replace("(Clone)", "").Trim();
		string[] parts = s.Split('_');
		return parts[0].ToLowerInvariant();
	}

	IEnumerator Shrink(IXRSelectInteractable interactable) {
		GameObject target = interactable.transform.gameObject;

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

	IEnumerator Raise(GameObject target) {
		Vector3 initialPos = target.transform.position;
		Vector3 targetPos = initialPos + Vector3.up * raiseHeight;
		float elapsedTime = 0f;

		while (elapsedTime < raiseDuration) {
			elapsedTime += Time.deltaTime;
			target.transform.position = Vector3.Lerp(initialPos, targetPos, elapsedTime / raiseDuration);
			yield return null;
		}

		target.transform.position = targetPos;
	}
}