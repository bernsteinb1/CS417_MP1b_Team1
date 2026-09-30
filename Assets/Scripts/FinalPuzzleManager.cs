using UnityEngine;

public class FinalPuzzleManager : MonoBehaviour
{
    public static FinalPuzzleManager Instance { get; private set; }

    private bool[] isSolved = new bool[] { false, false, false, false };

    public GameObject[] doors = new GameObject[4];

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // Reconnect persistent state to the newly loaded scene's doors.
            Instance.doors = doors;
            Instance.OpenDoors();

            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        OpenDoors();
    }

    public void OpenDoors()
    {
        if (doors == null)
            return;

        for (int i = 0; i < isSolved.Length && i < doors.Length; i++)
        {
            if (isSolved[i] && doors[i] != null)
                doors[i].SetActive(false);
        }
    }

    public void SolveDoor(int i)
    {
        if (i < 0 || i >= isSolved.Length)
            return;

        isSolved[i] = true;

        if (doors != null &&
            i < doors.Length &&
            doors[i] != null)
        {
            doors[i].SetActive(false);
        }
    }
}