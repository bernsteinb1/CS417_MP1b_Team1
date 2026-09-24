using UnityEngine;

public class SequencePuzzle : MonoBehaviour
{
    [Header("Correct Sequence")]
    public int[] correctSequence;

    [Header("Success")]
    public EasedMover revealMover;

    private int currentStep = 0;
    private bool solved = false;

    public void Press(int value)
    {
        if (solved)
            return;

        if (correctSequence == null ||
            correctSequence.Length == 0)
            return;

        if (value == correctSequence[currentStep])
        {
            currentStep++;

            Debug.Log(
                "Correct puzzle input. Step "
                + currentStep
                + " / "
                + correctSequence.Length
            );

            if (currentStep >= correctSequence.Length)
            {
                Solve();
            }
        }
        else
        {
            Debug.Log("Incorrect puzzle input. Resetting.");

            currentStep = 0;
        }
    }

    void Solve()
    {
        solved = true;

        Debug.Log("Puzzle solved.");

        if (revealMover != null)
        {
            revealMover.MoveToTarget();
        }
    }
}