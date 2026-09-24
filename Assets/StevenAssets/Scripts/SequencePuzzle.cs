using UnityEngine;
using TMPro;

public class SequencePuzzle : MonoBehaviour
{
    [Header("Correct Input Sequence")]
    [Tooltip("Example: 0 = A, 1 = B, 2 = C, 3 = D, 4 = E")]
    public int[] correctSequence;

    [Header("Reveal")]
    public EasedMover revealMover;

    [Header("Optional UI")]
    public TMP_Text feedbackText;

    [Header("Settings")]
    public bool resetOnWrongInput = true;

    private int currentStep = 0;
    private bool solved = false;

    public void Press(int value)
    {
        if (solved)
            return;

        if (correctSequence == null || correctSequence.Length == 0)
        {
            Debug.LogWarning("SequencePuzzle has no correct sequence.");
            return;
        }

        if (value == correctSequence[currentStep])
        {
            currentStep++;

            Debug.Log(
                "Correct puzzle input. Progress: " +
                currentStep +
                "/" +
                correctSequence.Length
            );

            if (feedbackText != null)
            {
                feedbackText.text =
                    "DIAGNOSTIC: " +
                    currentStep +
                    "/" +
                    correctSequence.Length;
            }

            if (currentStep >= correctSequence.Length)
            {
                Solve();
            }
        }
        else
        {
            Debug.Log(
                "Wrong puzzle input. Received " +
                value +
                "."
            );

            if (feedbackText != null)
            {
                feedbackText.text = "DIAGNOSTIC RESET";
            }

            if (resetOnWrongInput)
            {
                currentStep = 0;
            }
        }
    }

    private void Solve()
    {
        solved = true;

        Debug.Log("Server diagnostic puzzle solved.");

        if (feedbackText != null)
        {
            feedbackText.text = "FAULT ISOLATED\nACCESS GRANTED";
        }

        if (revealMover != null)
        {
            revealMover.MoveToTarget();
        }
    }

    public void ResetPuzzle()
    {
        solved = false;
        currentStep = 0;

        if (feedbackText != null)
        {
            feedbackText.text = "DIAGNOSTIC READY";
        }
    }
}