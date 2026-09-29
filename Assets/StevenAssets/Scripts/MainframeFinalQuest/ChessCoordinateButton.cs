using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ChessCoordinateButton : MonoBehaviour
{
    public enum ButtonAction
    {
        NextFile,
        NextRank,
        Submit,
        Reset
    }

    public ButtonAction action;
    public ChessCoordinatePuzzle puzzle;
    public XRSimpleInteractable interactable;

    private void Awake()
    {
        if (interactable == null)
            interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        if (interactable != null)
            interactable.selectEntered.AddListener(Pressed);
    }

    private void OnDisable()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(Pressed);
    }

    private void Pressed(SelectEnterEventArgs args)
    {
        if (puzzle == null) return;

        switch (action)
        {
            case ButtonAction.NextFile: puzzle.NextFile(); break;
            case ButtonAction.NextRank: puzzle.NextRank(); break;
            case ButtonAction.Submit: puzzle.Submit(); break;
            case ButtonAction.Reset: puzzle.ResetSelection(); break;
        }
    }
}
