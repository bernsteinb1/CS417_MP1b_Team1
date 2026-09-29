using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GoldenKeycard : MonoBehaviour
{
    public XRGrabInteractable grab;
    public Rigidbody body;
    public Renderer visualRenderer;
    public bool DetachOnFirstGrab = true;

    private bool detached;
    private Material runtimeMaterial;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (grab == null) grab = GetComponent<XRGrabInteractable>();
        if (body == null) body = GetComponent<Rigidbody>();
        if (visualRenderer == null) visualRenderer = GetComponent<Renderer>();
        if (visualRenderer != null) runtimeMaterial = visualRenderer.material;
    }

    private void OnEnable()
    {
        if (grab != null) grab.selectEntered.AddListener(OnSelected);
    }

    private void OnDisable()
    {
        if (grab != null) grab.selectEntered.RemoveListener(OnSelected);
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        if (!DetachOnFirstGrab || detached) return;
        detached = true;
        transform.SetParent(null, true);
        DontDestroyOnLoad(gameObject);
        if (body != null)
        {
            body.isKinematic = false;
            body.useGravity = true;
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
    }

    private void Update()
    {
        if (runtimeMaterial == null) return;
        float pulse = 1.25f + 0.75f * (0.5f + 0.5f * Mathf.Sin(Time.time * 5f));
        Color gold = new Color(1f, 0.48f, 0.03f, 1f) * pulse;
        if (runtimeMaterial.HasProperty("_EmissionColor"))
            runtimeMaterial.SetColor("_EmissionColor", gold);
    }
}
