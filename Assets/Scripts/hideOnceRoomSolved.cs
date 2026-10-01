using UnityEngine;

public class hideOnceRoomSolved : MonoBehaviour {
	[SerializeField] Room room = Room.UtilityCloset;
	private void Awake() {
		if (GameStateManager.Instance.IsSolved(room)) {
			gameObject.SetActive(false);
		}
	}
}
