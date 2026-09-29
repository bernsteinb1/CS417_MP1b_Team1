using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ChessSquareButton : MonoBehaviour
{
    public int x;
    public int y;
    public ChessKnightPuzzle puzzle;
    public XRSimpleInteractable interactable;

    private void Awake()
    {
        if (interactable == null) interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        if (interactable != null) interactable.selectEntered.AddListener(Pressed);
    }

    private void OnDisable()
    {
        if (interactable != null) interactable.selectEntered.RemoveListener(Pressed);
    }

    private void Pressed(SelectEnterEventArgs args)
    {
        if (puzzle != null) puzzle.SelectSquare(x, y);
    }
}
