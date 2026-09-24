using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class CompartmentHandleButton : MonoBehaviour
{
    public EasedMover mover;
    public bool hideAfterUse = true;
    private XRSimpleInteractable interactable;
    private bool used;

    void Awake() => interactable = GetComponent<XRSimpleInteractable>();
    void OnEnable()
    {
        if (interactable == null) interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(Use);
    }
    void OnDisable()
    {
        if (interactable != null) interactable.selectEntered.RemoveListener(Use);
    }
    private void Use(SelectEnterEventArgs args)
    {
        if (used) return;
        used = true;
        if (mover != null) mover.MoveToTarget();
        if (hideAfterUse) gameObject.SetActive(false);
    }
}
