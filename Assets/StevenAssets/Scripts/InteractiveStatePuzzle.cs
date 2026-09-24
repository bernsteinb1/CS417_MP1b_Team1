using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractiveStatePuzzle : MonoBehaviour
{
    [Header("Baked Scene References")]
    public Renderer[] currentLights;
    public Renderer[] targetLights;
    public Material onMaterial;
    public Material offMaterial;
    public TMP_Text statusText;
    public TMP_Text moveText;
    public Transform bookshelf;
    public Transform jailBars;

    [Header("Puzzle")]
    public int maxMoves = 6;
    public float revealDuration = 1.35f;

    // 1 0 1 1 0 0
    private readonly bool[] initialState = { true, false, true, true, false, false };
    // 0 0 0 0 0 1 -- minimum distance 5 with the operations below.
    private readonly bool[] targetState = { false, false, false, false, false, true };

    private bool[] state;
    private readonly Stack<bool[]> history = new Stack<bool[]>();
    private int moves;
    private bool powered;
    private bool solved;
    private bool expired;
    private bool revealing;

    public bool IsSolved => solved;
    public bool IsPowered => powered;

    void Start()
    {
        if (state == null) state = (bool[])initialState.Clone();
        UpdateVisuals();
        SetStatus(powered ? "SYSTEM ONLINE - MATCH TARGET" : "INSERT USB TO POWER CONSOLE");
    }

    public void Configure(Renderer[] current, Renderer[] target, Material onMat, Material offMat,
        TMP_Text status, TMP_Text movesLabel, Transform bookshelfTransform, Transform barsTransform)
    {
        currentLights = current;
        targetLights = target;
        onMaterial = onMat;
        offMaterial = offMat;
        statusText = status;
        moveText = movesLabel;
        bookshelf = bookshelfTransform;
        jailBars = barsTransform;

        state = (bool[])initialState.Clone();
        UpdateVisuals();
        SetStatus(powered ? "SYSTEM ONLINE - MATCH TARGET" : "INSERT USB TO POWER CONSOLE");
    }

    public void PowerOn()
    {
        if (expired || solved) return;
        powered = true;
        SetStatus("SYSTEM ONLINE - MATCH TARGET");
        UpdateVisuals();
    }

    public void SetExpired()
    {
        expired = true;
        SetStatus("LOCKDOWN - TIME EXPIRED");
    }

    public void ShiftRight() => Apply(ShiftRightImpl, "SHIFT RIGHT");
    public void SwapHalves() => Apply(SwapHalvesImpl, "SWAP HALVES");
    public void InvertEven() => Apply(InvertEvenImpl, "INVERT 2/4/6");
    public void RotateLeftHalf() => Apply(RotateLeftImpl, "ROTATE 1-3");
    public void RotateRightHalf() => Apply(RotateRightImpl, "ROTATE 4-6");

    public void Undo()
    {
        if (!CanInteract()) return;
        if (history.Count == 0)
        {
            SetStatus("NOTHING TO UNDO");
            return;
        }

        state = history.Pop();
        moves = Mathf.Max(0, moves - 1);
        SetStatus("UNDONE");
        UpdateVisuals();
    }

    public void ResetPuzzle()
    {
        if (!powered || expired || solved || revealing) return;
        history.Clear();
        state = (bool[])initialState.Clone();
        moves = 0;
        SetStatus("RESET - MATCH TARGET");
        UpdateVisuals();
    }

    public void Verify()
    {
        if (!CanInteract()) return;

        if (MatchesTarget())
        {
            solved = true;
            SetStatus("ACCESS GRANTED - SECRET CHAMBER OPENING");
            StartCoroutine(RevealRoutine());
        }
        else
        {
            SetStatus("STATE DOES NOT MATCH TARGET");
        }
    }

    private bool CanInteract()
    {
        if (expired || solved || revealing) return false;
        if (!powered)
        {
            SetStatus("INSERT USB TO POWER CONSOLE");
            return false;
        }
        return true;
    }

    private void Apply(System.Action op, string label)
    {
        if (!CanInteract()) return;
        if (moves >= maxMoves)
        {
            SetStatus("MOVE LIMIT REACHED - UNDO OR RESET");
            return;
        }

        history.Push((bool[])state.Clone());
        op();
        moves++;
        SetStatus(label);
        UpdateVisuals();
    }

    private void ShiftRightImpl()
    {
        bool last = state[5];
        for (int i = 5; i > 0; --i) state[i] = state[i - 1];
        state[0] = last;
    }

    private void SwapHalvesImpl()
    {
        for (int i = 0; i < 3; ++i)
        {
            bool temp = state[i];
            state[i] = state[i + 3];
            state[i + 3] = temp;
        }
    }

    private void InvertEvenImpl()
    {
        state[1] = !state[1];
        state[3] = !state[3];
        state[5] = !state[5];
    }

    private void RotateLeftImpl()
    {
        bool temp = state[2];
        state[2] = state[1];
        state[1] = state[0];
        state[0] = temp;
    }

    private void RotateRightImpl()
    {
        bool temp = state[5];
        state[5] = state[4];
        state[4] = state[3];
        state[3] = temp;
    }

    private bool MatchesTarget()
    {
        for (int i = 0; i < 6; ++i)
            if (state[i] != targetState[i]) return false;
        return true;
    }

    private void UpdateVisuals()
    {
        if (state == null) state = (bool[])initialState.Clone();

        if (currentLights != null)
        {
            for (int i = 0; i < Mathf.Min(6, currentLights.Length); ++i)
                if (currentLights[i] != null)
                    currentLights[i].sharedMaterial = state[i] ? onMaterial : offMaterial;
        }

        if (targetLights != null)
        {
            for (int i = 0; i < Mathf.Min(6, targetLights.Length); ++i)
                if (targetLights[i] != null)
                    targetLights[i].sharedMaterial = targetState[i] ? onMaterial : offMaterial;
        }

        if (moveText != null)
            moveText.text = $"MOVES {moves}/{maxMoves}";
    }

    private void SetStatus(string message)
    {
        if (statusText != null) statusText.text = message;
    }

    private IEnumerator RevealRoutine()
    {
        revealing = true;

        Vector3 shelfStart = bookshelf != null ? bookshelf.localPosition : Vector3.zero;
        Vector3 shelfEnd = shelfStart + new Vector3(1.4f, 0f, 0f);
        Vector3 barsStart = jailBars != null ? jailBars.localPosition : Vector3.zero;
        Vector3 barsEnd = barsStart + new Vector3(0f, 2.8f, 0f);

        float elapsed = 0f;
        while (elapsed < revealDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / revealDuration);
            t = t * t * (3f - 2f * t);
            if (bookshelf != null) bookshelf.localPosition = Vector3.Lerp(shelfStart, shelfEnd, t);
            yield return null;
        }
        if (bookshelf != null) bookshelf.localPosition = shelfEnd;

        yield return new WaitForSeconds(0.2f);
        elapsed = 0f;
        while (elapsed < revealDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / revealDuration);
            t = t * t * (3f - 2f * t);
            if (jailBars != null) jailBars.localPosition = Vector3.Lerp(barsStart, barsEnd, t);
            yield return null;
        }
        if (jailBars != null) jailBars.localPosition = barsEnd;
        revealing = false;
    }
}
