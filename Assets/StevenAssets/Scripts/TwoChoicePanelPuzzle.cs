using TMPro;
using UnityEngine;

public class TwoChoicePanelPuzzle : MonoBehaviour
{
    [Header("Answer")]
    public int correctChoice = 0;

    [Header("Optional prerequisite")]
    public bool requiresPrerequisite;
    public bool prerequisiteSatisfied;

    [Header("Result")]
    public EasedMover successMover;
    public TMP_Text statusText;

    [Header("Messages")]
    [TextArea] public string readyMessage = "CHOOSE AN ANSWER";
    [TextArea] public string prerequisiteMessage = "SWIPE BLUE KEYCARD FIRST";
    [TextArea] public string correctMessage = "CORRECT";
    [TextArea] public string wrongMessage = "TRY AGAIN";

    private bool solved;

    public bool IsSolved => solved;

    private void Start()
    {
        RefreshStatus();
    }

    public void UnlockPrerequisite()
    {
        prerequisiteSatisfied = true;
        RefreshStatus();
    }

    public void Choose(int choice)
    {
        if (solved)
            return;

        if (requiresPrerequisite && !prerequisiteSatisfied)
        {
            SetStatus(prerequisiteMessage);
            return;
        }

        if (choice != correctChoice)
        {
            SetStatus(wrongMessage);
            return;
        }

        solved = true;
        SetStatus(correctMessage);
        if (successMover != null)
            successMover.MoveToTarget();
    }

    private void RefreshStatus()
    {
        if (solved)
            SetStatus(correctMessage);
        else if (requiresPrerequisite && !prerequisiteSatisfied)
            SetStatus(prerequisiteMessage);
        else
            SetStatus(readyMessage);
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }
}
