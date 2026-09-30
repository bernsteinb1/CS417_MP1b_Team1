using UnityEngine;
using UnityEngine.InputSystem;

public class LaserPersistenceQuickTest : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.f8Key.wasPressedThisFrame)
        {
            if (FinalPuzzleManager.Instance == null)
            {
                Debug.LogError("No FinalPuzzleManager found.");
                return;
            }

            FinalPuzzleManager.Instance.SolveDoor(0);
            FinalPuzzleManager.Instance.SolveDoor(1);
            FinalPuzzleManager.Instance.SolveDoor(2);

            FinalPuzzleManager.Instance.OpenDoors();

            Debug.Log("TEST: first three laser doors marked solved.");
        }
    }
}