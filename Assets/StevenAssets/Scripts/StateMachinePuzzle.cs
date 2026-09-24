using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StateMachinePuzzle : MonoBehaviour
{
    public enum Operation
    {
        ShiftRight,
        SwapHalves,
        InvertEven,
        RotateLeftHalf,
        RotateRightHalf
    }

    [Header("State")]
    public bool[] initialState = new bool[6]
    {
        true, false, true, true, false, false
    };

    public bool[] targetState = new bool[6]
    {
        false, true, true, false, true, false
    };

    [Header("Current State Lights")]
    public Renderer[] currentLights;

    [Header("Target State Lights")]
    public Renderer[] targetLights;

    [Header("Materials")]
    public Material onMaterial;
    public Material offMaterial;

    [Header("UI")]
    public TMP_Text commandQueueText;
    public TMP_Text statusText;

    [Header("Puzzle Settings")]
    public int maxCommands = 6;
    public float stepDelay = 0.6f;

    [Header("Reward")]
    public EasedMover jailBarsMover;

    private bool[] currentState;
    private readonly List<Operation> commandQueue =
        new List<Operation>();

    private bool solved;
    private bool executing;

    private void Start()
    {
        currentState = (bool[])initialState.Clone();

        UpdateCurrentLights();
        UpdateTargetLights();
        UpdateQueueDisplay();

        if (statusText != null)
        {
            statusText.text = "SECURITY CONFIGURATION REQUIRED";
        }
    }

    public void QueueShift()
    {
        AddOperation(Operation.ShiftRight);
    }

    public void QueueSwap()
    {
        AddOperation(Operation.SwapHalves);
    }

    public void QueueInvertEven()
    {
        AddOperation(Operation.InvertEven);
    }

    public void QueueRotateLeft()
    {
        AddOperation(Operation.RotateLeftHalf);
    }

    public void QueueRotateRight()
    {
        AddOperation(Operation.RotateRightHalf);
    }

    private void AddOperation(Operation operation)
    {
        if (solved || executing)
            return;

        if (commandQueue.Count >= maxCommands)
        {
            if (statusText != null)
            {
                statusText.text = "COMMAND BUFFER FULL";
            }

            return;
        }

        commandQueue.Add(operation);

        UpdateQueueDisplay();

        if (statusText != null)
        {
            statusText.text = "COMMAND ADDED";
        }
    }

    public void ExecuteQueue()
    {
        if (solved || executing)
            return;

        if (commandQueue.Count == 0)
        {
            if (statusText != null)
            {
                statusText.text = "NO COMMANDS QUEUED";
            }

            return;
        }

        StartCoroutine(ExecuteRoutine());
    }

    private IEnumerator ExecuteRoutine()
    {
        executing = true;

        currentState = (bool[])initialState.Clone();
        UpdateCurrentLights();

        foreach (Operation operation in commandQueue)
        {
            ApplyOperation(operation);

            UpdateCurrentLights();

            yield return new WaitForSeconds(stepDelay);
        }

        if (MatchesTarget())
        {
            solved = true;

            if (statusText != null)
            {
                statusText.text =
                    "SECURITY CONFIGURATION ACCEPTED";
            }

            if (jailBarsMover != null)
            {
                jailBarsMover.MoveToTarget();
            }
        }
        else
        {
            if (statusText != null)
            {
                statusText.text =
                    "TARGET STATE NOT REACHED";
            }
        }

        executing = false;
    }

    public void ResetPuzzle()
    {
        if (solved || executing)
            return;

        commandQueue.Clear();

        currentState = (bool[])initialState.Clone();

        UpdateCurrentLights();
        UpdateQueueDisplay();

        if (statusText != null)
        {
            statusText.text =
                "SECURITY CONFIGURATION REQUIRED";
        }
    }

    private void ApplyOperation(Operation operation)
    {
        switch (operation)
        {
            case Operation.ShiftRight:
                ShiftRight();
                break;

            case Operation.SwapHalves:
                SwapHalves();
                break;

            case Operation.InvertEven:
                InvertEven();
                break;

            case Operation.RotateLeftHalf:
                RotateLeftHalf();
                break;

            case Operation.RotateRightHalf:
                RotateRightHalf();
                break;
        }
    }

    private void ShiftRight()
    {
        bool last = currentState[5];

        for (int i = 5; i > 0; i--)
        {
            currentState[i] = currentState[i - 1];
        }

        currentState[0] = last;
    }

    private void SwapHalves()
    {
        for (int i = 0; i < 3; i++)
        {
            bool temp = currentState[i];
            currentState[i] = currentState[i + 3];
            currentState[i + 3] = temp;
        }
    }

    private void InvertEven()
    {
        // Human positions 2, 4, and 6.
        currentState[1] = !currentState[1];
        currentState[3] = !currentState[3];
        currentState[5] = !currentState[5];
    }

    private void RotateLeftHalf()
    {
        bool temp = currentState[2];

        currentState[2] = currentState[1];
        currentState[1] = currentState[0];
        currentState[0] = temp;
    }

    private void RotateRightHalf()
    {
        bool temp = currentState[5];

        currentState[5] = currentState[4];
        currentState[4] = currentState[3];
        currentState[3] = temp;
    }

    private bool MatchesTarget()
    {
        if (currentState.Length != targetState.Length)
            return false;

        for (int i = 0; i < currentState.Length; i++)
        {
            if (currentState[i] != targetState[i])
                return false;
        }

        return true;
    }

    private void UpdateCurrentLights()
    {
        if (currentLights == null)
            return;

        for (int i = 0;
             i < currentLights.Length &&
             i < currentState.Length;
             i++)
        {
            if (currentLights[i] == null)
                continue;

            currentLights[i].material =
                currentState[i]
                ? onMaterial
                : offMaterial;
        }
    }

    private void UpdateTargetLights()
    {
        if (targetLights == null)
            return;

        for (int i = 0;
             i < targetLights.Length &&
             i < targetState.Length;
             i++)
        {
            if (targetLights[i] == null)
                continue;

            targetLights[i].material =
                targetState[i]
                ? onMaterial
                : offMaterial;
        }
    }

    private void UpdateQueueDisplay()
    {
        if (commandQueueText == null)
            return;

        string text = "COMMAND QUEUE\n";

        for (int i = 0; i < maxCommands; i++)
        {
            if (i < commandQueue.Count)
            {
                text +=
                    "[" +
                    ShortName(commandQueue[i]) +
                    "]";
            }
            else
            {
                text += "[ ]";
            }

            if (i < maxCommands - 1)
            {
                text += " ";
            }
        }

        commandQueueText.text = text;
    }

    private string ShortName(Operation operation)
    {
        switch (operation)
        {
            case Operation.ShiftRight:
                return "S";

            case Operation.SwapHalves:
                return "X";

            case Operation.InvertEven:
                return "I";

            case Operation.RotateLeftHalf:
                return "L";

            case Operation.RotateRightHalf:
                return "R";

            default:
                return "?";
        }
    }
}