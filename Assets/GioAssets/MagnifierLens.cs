using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Put this on the LensView disc (a very flat Cylinder sitting where the glass is).
// A small camera looks from the lens in the direction you're looking through it,
// zooms in, and shows that picture on the lens, so tiny text becomes readable.
[RequireComponent(typeof(Renderer))]
public class MagnifierLens : MonoBehaviour
{
    [Tooltip("The LensCamera child. It must NOT be tagged MainCamera.")]
    public Camera lensCamera;

    [Tooltip("How much bigger things look through the glass.")]
    [Range(1.5f, 8f)] public float zoom = 3f;

    [Tooltip("Sharpness of the zoomed image. 512 is good for Quest, 1024 if text is still blurry.")]
    public int resolution = 512;

    [Tooltip("Only magnify while someone is holding it (saves performance).")]
    public bool onlyWhenHeld = true;

    [Tooltip("Optional: the model's original glass, hidden while the zoom view is showing.")]
    public Renderer originalGlass;

    [Header("If the image looks mirrored or upside down")]
    public bool flipHorizontal;
    public bool flipVertical;

    Renderer lensRenderer;
    Material lensMaterial;
    RenderTexture renderTexture;
    Transform head;
    XRGrabInteractable grab;

    void Awake()
    {
        lensRenderer = GetComponent<Renderer>();
        lensMaterial = lensRenderer.material;

        renderTexture = new RenderTexture(resolution, resolution, 24);
        renderTexture.name = "MagnifierView";

        lensCamera.targetTexture = renderTexture;
        lensCamera.stereoTargetEye = StereoTargetEyeMask.None; // don't draw to the headset
        lensCamera.nearClipPlane = 0.01f;

        AudioListener listener = lensCamera.GetComponent<AudioListener>();
        if (listener != null) Destroy(listener);

        lensMaterial.SetTexture("_BaseMap", renderTexture);
        ApplyFlip();

        grab = GetComponentInParent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        if (grab == null) return;
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        if (grab == null) return;
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }

    void Start()
    {
        if (Camera.main != null) head = Camera.main.transform;
        bool held = grab != null && grab.isSelected;
        SetViewActive(!onlyWhenHeld || held);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        SetViewActive(true);
    }

    void OnRelease(SelectExitEventArgs args)
    {
        if (onlyWhenHeld && (grab == null || !grab.isSelected)) SetViewActive(false);
    }

    void SetViewActive(bool on)
    {
        lensCamera.enabled = on;
        lensRenderer.enabled = on;
        if (originalGlass != null) originalGlass.enabled = !on;
    }

    void LateUpdate()
    {
        if (!lensCamera.enabled) return;
        if (head == null)
        {
            if (Camera.main == null) return;
            head = Camera.main.transform;
        }

        Vector3 lensPos = transform.position;
        Vector3 toLens = lensPos - head.position;
        float distance = toLens.magnitude;
        if (distance < 0.01f) return;
        Vector3 lookDir = toLens / distance;

        // Keep the picture lined up with the lens so it turns with the glass.
        Vector3 up = Vector3.ProjectOnPlane(transform.forward, lookDir);
        if (up.sqrMagnitude < 0.0001f) up = Vector3.ProjectOnPlane(transform.right, lookDir);

        lensCamera.transform.SetPositionAndRotation(lensPos, Quaternion.LookRotation(lookDir, up));

        // Field of view = how big the lens looks from your eye, divided by the zoom.
        float radius = 0.5f * Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);
        float lensAngle = 2f * Mathf.Atan(radius / distance) * Mathf.Rad2Deg;
        lensCamera.fieldOfView = Mathf.Clamp(lensAngle / zoom, 1f, 60f);
    }

    void ApplyFlip()
    {
        if (lensMaterial == null) return;
        lensMaterial.SetTextureScale("_BaseMap", new Vector2(flipHorizontal ? -1f : 1f, flipVertical ? -1f : 1f));
        lensMaterial.SetTextureOffset("_BaseMap", new Vector2(flipHorizontal ? 1f : 0f, flipVertical ? 1f : 0f));
    }

    void OnValidate()
    {
        ApplyFlip();
    }

    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
        if (lensMaterial != null) Destroy(lensMaterial);
    }
}
