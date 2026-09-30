using UnityEngine;

public class FinalPuzzleManager : MonoBehaviour
{
    public static FinalPuzzleManager Instance { get; private set; }
    bool[] isSolved = new bool[] { false, false, false, false };
    public GameObject[] doors = new GameObject[4];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Check if an instance already exists in the scene
        if (Instance != null && Instance != this)
        {
            Instance.OpenDoors();
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }

        // Set the active instance and protect it from scene destruction
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OpenDoors()
    {
        for (int i = 0; i < 4; i++)
        {
            if (isSolved[i]) doors[i].SetActive(false);
        }
    }

    public void SolveDoor(int i)
    {
        isSolved[i] = true;
    }
}
