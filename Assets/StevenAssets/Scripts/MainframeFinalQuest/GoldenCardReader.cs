using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GoldenCardReader : MonoBehaviour {
	public TMP_Text statusText;
	public Renderer statusRenderer;
	public UnityEvent OnAccessGranted => onAccessGranted;
	[SerializeField] private UnityEvent onAccessGranted;

	private void Start() => Refresh();

	private void OnTriggerEnter(Collider other) {
		if (GoldenKeyQuestState.CardScanned) return;
		GoldenKeycard card = other.GetComponentInParent<GoldenKeycard>();
		if (card == null) return;
		onAccessGranted?.Invoke();
		GoldenKeyQuestState.CardScanned = true;
		Refresh();
	}

	private void Refresh() {
		if (statusText != null)
			statusText.text = GoldenKeyQuestState.CardScanned ? "GOLD AUTH: ACCEPTED" : "GOLD AUTH: REQUIRED";
		if (statusRenderer != null)
			statusRenderer.material.color = GoldenKeyQuestState.CardScanned
				? new Color(0.15f, 1f, 0.25f)
				: new Color(1f, 0.55f, 0.05f);
	}
}
