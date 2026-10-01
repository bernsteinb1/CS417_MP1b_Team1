using UnityEngine;

public class FinalPuzzleManager : MonoBehaviour {
	public static FinalPuzzleManager Instance { get; private set; }

	private static bool[] isSolved = new bool[] { false, false, false, false };

	public GameObject[] doors = new GameObject[4];

	void Awake() {
		Instance = this;
		OpenDoors();
	}

	public void OpenDoors() {
		if (doors == null)
			return;

		for (int i = 0; i < isSolved.Length && i < doors.Length; i++) {
			if (isSolved[i] && doors[i] != null)
				doors[i].SetActive(false);
		}
	}

	public void SolveDoor(int i) {
		if (i < 0 || i >= isSolved.Length)
			return;

		isSolved[i] = true;

		if (doors != null &&
			i < doors.Length &&
			doors[i] != null) {
			doors[i].SetActive(false);
		}
	}
}