using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class GoldenPlunger : MonoBehaviour
{
    public Transform cardAttachPoint;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void AttachGoldenCard()
    {
        if (GoldenKeyQuestState.CardRetrieved) return;
        GoldenKeyQuestState.CardRetrieved = true;

        if (cardAttachPoint == null)
        {
            var attach = new GameObject("GoldenCardAttachPoint");
            attach.transform.SetParent(transform, false);
            attach.transform.localPosition = new Vector3(0f, -0.24f, 0f);
            cardAttachPoint = attach.transform;
        }

        var card = GameObject.CreatePrimitive(PrimitiveType.Cube);
        card.name = "GoldenKeycard";
        card.transform.SetParent(cardAttachPoint, false);
        card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        card.transform.localScale = new Vector3(0.16f, 0.018f, 0.10f);

        var renderer = card.GetComponent<Renderer>();
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader != null)
        {
            var mat = new Material(shader) { name = "M_GoldenKeycard_Runtime" };
            mat.color = new Color(1f, 0.62f, 0.06f, 1f);
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", new Color(2.7f, 1.3f, 0.08f, 1f));
            renderer.material = mat;
        }

        var rb = card.AddComponent<Rigidbody>();
        rb.mass = 0.15f;
        rb.isKinematic = true;
        rb.useGravity = false;

        var grab = card.AddComponent<XRGrabInteractable>();
        grab.throwOnDetach = true;

        var key = card.AddComponent<GoldenKeycard>();
        key.grab = grab;
        key.body = rb;
        key.visualRenderer = renderer;
        key.DetachOnFirstGrab = true;
    }
}
