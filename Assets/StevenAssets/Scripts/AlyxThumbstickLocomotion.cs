using System;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils;

public class AlyxThumbstickLocomotion : MonoBehaviour
{
    [Header("Thumbstick")]
    public XRNode controllerNode = XRNode.LeftHand;
    [Range(0.4f, 0.95f)] public float aimThreshold = 0.65f;
    [Range(0.1f, 0.8f)] public float releaseThreshold = 0.35f;

    [Header("Teleport Arc")]
    public float launchSpeed = 6.0f;
    public float gravity = 9.81f;
    public float maxArcTime = 2.0f;
    public int arcSegments = 28;
    public float maxSlopeDegrees = 50f;

    private XROrigin xrOrigin;
    private Transform leftController;
    private LineRenderer line;
    private InputDevice device;
    private bool aiming;
    private bool hasValidTarget;
    private Vector3 targetPoint;

    void Start()
    {
        xrOrigin = GetComponent<XROrigin>();
        if (xrOrigin == null) xrOrigin = GetComponentInChildren<XROrigin>(true);
        if (xrOrigin == null) xrOrigin = FindFirstObjectByType<XROrigin>();

        leftController = FindControllerTransform(transform, "Left");
        if (leftController == null && xrOrigin != null)
            leftController = FindControllerTransform(xrOrigin.transform, "Left");

        DisableContinuousMoveProviders();
        BuildLine();
        RefreshDevice();
    }

    void OnEnable() => RefreshDevice();

    void Update()
    {
        if (!device.isValid) RefreshDevice();
        if (!device.isValid || xrOrigin == null || leftController == null) return;

        if (!device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 stick)) return;

        bool forwardIntent = stick.y >= aimThreshold && Mathf.Abs(stick.y) > Mathf.Abs(stick.x) * 1.15f;

        if (forwardIntent)
        {
            aiming = true;
            UpdateArc();
        }
        else if (aiming && stick.y <= releaseThreshold)
        {
            if (hasValidTarget) Teleport();
            aiming = false;
            hasValidTarget = false;
            if (line != null) line.enabled = false;
        }
        else if (!aiming && line != null)
        {
            line.enabled = false;
        }
    }

    private void RefreshDevice()
    {
        device = InputDevices.GetDeviceAtXRNode(controllerNode);
    }

    private void DisableContinuousMoveProviders()
    {
        MonoBehaviour[] behaviours = GetComponentsInChildren<MonoBehaviour>(true);
        foreach (MonoBehaviour b in behaviours)
        {
            if (b == null || b == this) continue;
            string n = b.GetType().Name;
            if (n.Contains("ContinuousMoveProvider", StringComparison.OrdinalIgnoreCase) ||
                n.Contains("DynamicMoveProvider", StringComparison.OrdinalIgnoreCase))
                b.enabled = false;
        }
    }

    private void BuildLine()
    {
        GameObject go = new GameObject("AlyxTeleportArc");
        go.transform.SetParent(transform, false);
        line = go.AddComponent<LineRenderer>();
        line.positionCount = arcSegments;
        line.startWidth = 0.018f;
        line.endWidth = 0.012f;
        line.useWorldSpace = true;
        line.enabled = false;
        Shader shader = Shader.Find("Sprites/Default");
        if (shader != null)
        {
            line.material = new Material(shader);
            line.material.color = new Color(0.2f, 0.8f, 1f, 0.9f);
        }
    }

    private void UpdateArc()
    {
        if (line == null) return;
        line.enabled = true;
        line.positionCount = arcSegments;

        Vector3 start = leftController.position;
        Vector3 velocity = leftController.forward * launchSpeed;
        Vector3 prev = start;
        hasValidTarget = false;

        int used = 1;
        line.SetPosition(0, start);

        for (int i = 1; i < arcSegments; ++i)
        {
            float t = maxArcTime * i / (arcSegments - 1f);
            Vector3 next = start + velocity * t + 0.5f * Vector3.down * gravity * t * t;
            Vector3 delta = next - prev;

            if (Physics.Raycast(prev, delta.normalized, out RaycastHit hit, delta.magnitude, ~0, QueryTriggerInteraction.Ignore))
            {
                line.SetPosition(i, hit.point);
                used = i + 1;
                float slope = Vector3.Angle(hit.normal, Vector3.up);
                if (slope <= maxSlopeDegrees)
                {
                    hasValidTarget = true;
                    targetPoint = hit.point;
                }
                break;
            }

            line.SetPosition(i, next);
            used = i + 1;
            prev = next;
        }

        line.positionCount = used;
        if (line.material != null)
            line.material.color = hasValidTarget ? new Color(0.2f, 0.9f, 1f, 0.95f) : new Color(1f, 0.25f, 0.2f, 0.95f);
    }

    private void Teleport()
    {
        Transform cam = xrOrigin.Camera != null ? xrOrigin.Camera.transform : Camera.main?.transform;
        if (cam == null) return;

        float cameraHeight = Mathf.Max(0.5f, cam.position.y - xrOrigin.transform.position.y);
        Vector3 desiredCamera = targetPoint + Vector3.up * cameraHeight;
        xrOrigin.MoveCameraToWorldLocation(desiredCamera);
    }

    private Transform FindControllerTransform(Transform root, string side)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            string n = t.name;
            if (n.IndexOf(side, StringComparison.OrdinalIgnoreCase) >= 0 &&
                (n.IndexOf("Controller", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 n.IndexOf("Hand", StringComparison.OrdinalIgnoreCase) >= 0))
                return t;
        }
        return null;
    }
}
