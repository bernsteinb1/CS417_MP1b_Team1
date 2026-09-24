using System.Collections.Generic;
using UnityEngine;

public enum Room {
	Office,
	Classroom,
	Bathroom,
	UtilityCloset
}

public class GameStateManager : MonoBehaviour {
	public static GameStateManager Instance { get; private set; }

	private readonly HashSet<Room> solvedRooms = new HashSet<Room>();

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Bootstrap() {
		if (Instance != null) return;
		new GameObject("GameStateManager").AddComponent<GameStateManager>();
	}

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public bool IsSolved(Room room) { return solvedRooms.Contains(room); }

	public void MarkSolved(Room room) {
		solvedRooms.Add(room);
		Debug.Log(room + " marked as solved");
	}

	public void MarkUnsolved(Room room) {
		solvedRooms.Remove(room);
		Debug.Log(room + " marked as unsolved");
	}
}