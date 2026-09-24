using TMPro;
using UnityEngine;

public class MainframeAccessController : MonoBehaviour
{
    public bool officeSolved;
    public bool classroomSolved;
    public bool bathroomSolved;
    public bool utilityClosetSolved;
    public OpenHinge doorHinge;
    public TMP_Text statusText;

    private bool opened;

    void Start() => Refresh();

    void Update()
    {
        // Team-wide state is already centralized in GameStateManager. Mirror it automatically
        // so no teammate has to wire special Steven-scene events into this door.
        if (GameStateManager.Instance != null)
        {
            officeSolved = GameStateManager.Instance.IsSolved(Room.Office);
            classroomSolved = GameStateManager.Instance.IsSolved(Room.Classroom);
            bathroomSolved = GameStateManager.Instance.IsSolved(Room.Bathroom);
            utilityClosetSolved = GameStateManager.Instance.IsSolved(Room.UtilityCloset);
            Refresh();
        }
    }

    public void MarkOfficeSolved() { officeSolved = true; Refresh(); }
    public void MarkClassroomSolved() { classroomSolved = true; Refresh(); }
    public void MarkBathroomSolved() { bathroomSolved = true; Refresh(); }
    public void MarkUtilityClosetSolved() { utilityClosetSolved = true; Refresh(); }

    public void SetOfficeSolved(bool value) { officeSolved = value; Refresh(); }
    public void SetClassroomSolved(bool value) { classroomSolved = value; Refresh(); }
    public void SetBathroomSolved(bool value) { bathroomSolved = value; Refresh(); }
    public void SetUtilityClosetSolved(bool value) { utilityClosetSolved = value; Refresh(); }

    [ContextMenu("DEBUG - Mark All Rooms Solved")]
    public void DebugMarkAllSolved()
    {
        officeSolved = classroomSolved = bathroomSolved = utilityClosetSolved = true;
        Refresh();
    }

    private void Refresh()
    {
        int solved = (officeSolved ? 1 : 0) + (classroomSolved ? 1 : 0) + (bathroomSolved ? 1 : 0) + (utilityClosetSolved ? 1 : 0);
        if (statusText != null)
            statusText.text = solved >= 4 ? "MULTI-ROOM AUTH 4/4\nDOOR RELEASED" : $"MULTI-ROOM AUTH {solved}/4\nACCESS DENIED";

        if (!opened && solved >= 4)
        {
            opened = true;
            if (doorHinge != null) doorHinge.enabled = true;
        }
    }
}
