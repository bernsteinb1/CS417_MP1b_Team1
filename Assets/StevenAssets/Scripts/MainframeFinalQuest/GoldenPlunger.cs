using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class GoldenPlunger : MonoBehaviour
{
    [Header("Golden card")]
    [Tooltip("Assign Assets/StevenAssets/Prefabs/Gold Card.prefab here.")]
    public GameObject goldenCardPrefab;

    public Transform cardAttachPoint;

    public void AttachGoldenCard()
    {
        if (GoldenKeyQuestState.CardRetrieved)
            return;

        if (goldenCardPrefab == null)
        {
            Debug.LogError(
                "GoldenPlunger: Golden Card prefab is not assigned. " +
                "Drag Assets/StevenAssets/Prefabs/Gold Card.prefab into the " +
                "Golden Card Prefab field on GoldenKey_ToiletPlunger.",
                this);
            return;
        }

        if (cardAttachPoint == null)
        {
            GameObject attach = new GameObject("GoldenCardAttachPoint");
            attach.transform.SetParent(transform, false);
            attach.transform.localPosition = new Vector3(0f, -0.24f, 0f);
            cardAttachPoint = attach.transform;
        }

        GameObject card = Instantiate(goldenCardPrefab, cardAttachPoint, false);
        card.name = "GoldenKeycard";
        card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

        // Do not inherit a strange scale from the plunger hierarchy.
        // The prefab keeps its authored local scale.

        Rigidbody rb = card.GetComponent<Rigidbody>();
        if (rb == null)
            rb = card.AddComponent<Rigidbody>();

        rb.mass = 0.15f;
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        XRGrabInteractable grab = card.GetComponent<XRGrabInteractable>();
        if (grab == null)
            grab = card.AddComponent<XRGrabInteractable>();
        grab.throwOnDetach = true;

        if (card.GetComponentInChildren<Collider>(true) == null)
            card.AddComponent<BoxCollider>();

        InventoryStowable stowable = card.GetComponent<InventoryStowable>();
        if (stowable == null)
            card.AddComponent<InventoryStowable>();

        GoldenKeycard key = card.GetComponent<GoldenKeycard>();
        if (key == null)
            key = card.AddComponent<GoldenKeycard>();

        key.grab = grab;
        key.body = rb;
        key.DetachOnFirstGrab = true;
        key.ApplyGoldPulseNow();

        GoldenKeyQuestState.CardRetrieved = true;
        Debug.Log("GoldenPlunger: instantiated Gold Card prefab and attached it to the plunger.");
    }
}
