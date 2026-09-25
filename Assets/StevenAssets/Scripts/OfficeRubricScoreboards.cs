using TMPro;
using UnityEngine;

public class OfficeRubricScoreboards : MonoBehaviour
{
    [Header("Progress sources")]
    public OfficeProgressManager progressManager;
    public TwoChoicePanelPuzzle keycardPuzzle;
    public TwoChoicePanelPuzzle usbPuzzle;
    public InteractiveStatePuzzle statePuzzle;

    [Header("Displays")]
    public TMP_Text progressText;
    public TMP_Text puzzleText;

    private int lastKeysDiscovered = -1;
    private int lastLocksSolved = -1;

    private void Start()
    {
        Refresh(true);
    }

    private void Update()
    {
        Refresh(false);
    }

    private void Refresh(bool force)
    {
        int keysDiscovered = 0;
        if (keycardPuzzle != null && keycardPuzzle.IsSolved) keysDiscovered++;
        if (usbPuzzle != null && usbPuzzle.IsSolved) keysDiscovered++;
        if (statePuzzle != null && statePuzzle.IsSolved) keysDiscovered++;

        int locksSolved = 0;
        if (progressManager != null)
        {
            if (progressManager.cardReaderSolved) locksSolved++;
            if (progressManager.usbPortSolved) locksSolved++;
            if (progressManager.doorPanelSolved) locksSolved++;
        }

        if (force || keysDiscovered != lastKeysDiscovered || locksSolved != lastLocksSolved)
        {
            if (progressText != null)
            {
                progressText.text =
                    $"KEYS DISCOVERED  {keysDiscovered}/3     KEYS REMAINING  {3 - keysDiscovered}/3\n" +
                    $"LOCKS SOLVED     {locksSolved}/3     LOCKS UNSOLVED   {3 - locksSolved}/3";
            }

            lastKeysDiscovered = keysDiscovered;
            lastLocksSolved = locksSolved;
        }

        if (puzzleText != null)
        {
            puzzleText.text =
                "CLUES TO FIND:  BLUE MATH 1   |   ORANGE CS 1   |   STATE MACHINE 1   |   TOTAL 3";
        }
    }
}
