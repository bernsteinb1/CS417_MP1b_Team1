using UnityEngine;

public class DoorSolver : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SolveDoor(int i)
    {
        FinalPuzzleManager.Instance.SolveDoor(i);
    }
}
