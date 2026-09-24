using UnityEngine;

public class SolveBathroom : MonoBehaviour
{
    public void Solve()
    {
        GameStateManager.Instance.MarkSolved(Room.Bathroom);
    }
}
