using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class StatePuzzleButton : MonoBehaviour
{
    public enum Operation { Shift, Swap, Invert, RotateLeftHalf, RotateRightHalf, Undo, Reset, Verify }

    public InteractiveStatePuzzle puzzle;
    public Operation operation;
    public float pressDistance = 0.025f;
    public float pressTime = 0.08f;
    public float holdTime = 0.05f;

    private XRSimpleInteractable interactable;
    private Vector3 restLocalPosition;
    private bool pressing;

    void Awake()
    {
        restLocalPosition = transform.localPosition;
        interactable = GetComponent<XRSimpleInteractable>();
    }

    void OnEnable()
    {
        if (interactable == null) interactable = GetComponent<XRSimpleInteractable>();
        restLocalPosition = transform.localPosition;
        interactable.selectEntered.AddListener(OnSelected);
    }

    void OnDisable()
    {
        if (interactable != null) interactable.selectEntered.RemoveListener(OnSelected);
        StopAllCoroutines();
        transform.localPosition = restLocalPosition;
        pressing = false;
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        if (pressing) return;
        InvokeOperation();
        StartCoroutine(Pulse());
    }

    private void InvokeOperation()
    {
        if (puzzle == null) return;
        switch (operation)
        {
            case Operation.Shift: puzzle.ShiftRight(); break;
            case Operation.Swap: puzzle.SwapHalves(); break;
            case Operation.Invert: puzzle.InvertEven(); break;
            case Operation.RotateLeftHalf: puzzle.RotateLeftHalf(); break;
            case Operation.RotateRightHalf: puzzle.RotateRightHalf(); break;
            case Operation.Undo: puzzle.Undo(); break;
            case Operation.Reset: puzzle.ResetPuzzle(); break;
            case Operation.Verify: puzzle.Verify(); break;
        }
    }

    private IEnumerator Pulse()
    {
        pressing = true;
        Vector3 pressed = restLocalPosition + Vector3.forward * pressDistance;
        yield return Move(restLocalPosition, pressed, pressTime);
        yield return new WaitForSeconds(holdTime);
        yield return Move(pressed, restLocalPosition, pressTime);
        transform.localPosition = restLocalPosition;
        pressing = false;
    }

    private IEnumerator Move(Vector3 a, Vector3 b, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / Mathf.Max(0.001f, duration));
            t = t * t * (3f - 2f * t);
            transform.localPosition = Vector3.Lerp(a, b, t);
            yield return null;
        }
        transform.localPosition = b;
    }
}
