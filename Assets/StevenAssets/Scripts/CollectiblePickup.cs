using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class CollectiblePickup : MonoBehaviour
{
    public OfficeCollectibleTracker tracker;
    private XRSimpleInteractable interactable;
    private bool collected;

    void Awake() => interactable = GetComponent<XRSimpleInteractable>();
    void OnEnable()
    {
        if (interactable == null) interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnSelected);
    }
    void OnDisable()
    {
        if (interactable != null) interactable.selectEntered.RemoveListener(OnSelected);
    }
    void OnSelected(SelectEnterEventArgs args)
    {
        if (collected) return;
        collected = true;
        if (tracker != null) tracker.Collect(gameObject);
    }
}
