using UnityEngine;

public class MarkRoomSolved : MonoBehaviour
{
    [SerializeField] private Room room = Room.UtilityCloset;

    public void MarkSolved()
    {
        GameStateManager.Instance.MarkSolved(room);
    }
}