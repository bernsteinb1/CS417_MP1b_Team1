using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// Put this on the LeverPivot object (the empty that holds all the lever parts).
// The power starts OFF. Grab the handle and push it to the ON position to turn the power on.
[RequireComponent(typeof(XRSimpleInteractable))]
public class PowerLever : MonoBehaviour
{
    public enum Motion { Rotate, Slide }
    public enum Axis { X, Y, Z }

    [Header("Lever movement")]
    [Tooltip("Rotate = lever swings on a hinge. Slide = lever moves straight along a track.")]
    public Motion motion = Motion.Rotate;

    [Tooltip("Rotate: the axis the lever swings around. Slide: the axis it moves along. (Red = X, Green = Y, Blue = Z in Local mode.)")]
    public Axis axis = Axis.X;

    [Tooltip("How far the lever travels from OFF (where you placed it) to ON. Degrees for Rotate, units for Slide. Use a negative number if it moves the wrong way.")]
    public float onAmount = 60f;

    [Tooltip("How close to fully ON counts as switched on (0.9 = 90% of the way).")]
    [Range(0.5f, 1f)] public float onThreshold = 0.9f;

    [Tooltip("If the player lets go before reaching ON, the lever drops back to OFF.")]
    public bool returnIfReleasedEarly = true;

    [Header("Panel lamps (each keeps its own material; only the glow turns on/off)")]
    [Tooltip("Glows while the power is OFF.")]
    public Renderer redLamp;
    [Tooltip("Glows once the power is ON.")]
    public Renderer greenLamp;
    [Tooltip("Optional small point light next to the lamps; turns red/green with the power.")]
    public Light statusLight;

    [Header("Room lights")]
    [Tooltip("Turned OFF at start, ON when power is restored (e.g. your RoomLights group).")]
    public GameObject[] turnOnWithPower;
    [Tooltip("Turned ON at start, OFF when power is restored (e.g. a red emergency light).")]
    public GameObject[] turnOffWithPower;

    [Header("Anything else that should happen (e.g. open a door)")]
    public UnityEvent onPowerOn;

    public bool IsOn { get; private set; }

    static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    XRSimpleInteractable interactable;
    IXRSelectInteractor hand;
    Vector3 startLocalPos;
    Quaternion startLocalRot;
    Vector3 grabStartPoint;
    float grabStartAmount;
    float amount; // 0 = OFF, onAmount = ON

    Vector3 AxisVector => axis == Axis.X ? Vector3.right : axis == Axis.Y ? Vector3.up : Vector3.forward;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        startLocalPos = transform.localPosition;
        startLocalRot = transform.localRotation;
    }

    void OnEnable()
    {
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnGrab);
        interactable.selectExited.RemoveListener(OnRelease);
    }

    void Start()
    {
        ApplyAmount(0f);
        SetPower(false);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        if (IsOn) return;
        hand = args.interactorObject;
        grabStartPoint = HandPointLocal();
        grabStartAmount = amount;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        hand = null;
        if (!IsOn && returnIfReleasedEarly) ApplyAmount(0f);
    }

    void Update()
    {
        if (hand == null || IsOn) return;

        Vector3 now = HandPointLocal();
        Vector3 axisInParent = startLocalRot * AxisVector;
        float delta;

        if (motion == Motion.Slide)
        {
            delta = Vector3.Dot(now - grabStartPoint, axisInParent);
        }
        else
        {
            Vector3 from = Vector3.ProjectOnPlane(grabStartPoint - startLocalPos, axisInParent);
            Vector3 to = Vector3.ProjectOnPlane(now - startLocalPos, axisInParent);
            delta = Vector3.SignedAngle(from, to, axisInParent);
        }

        float min = Mathf.Min(0f, onAmount);
        float max = Mathf.Max(0f, onAmount);
        ApplyAmount(Mathf.Clamp(grabStartAmount + delta, min, max));

        if (!Mathf.Approximately(onAmount, 0f) && amount / onAmount >= onThreshold)
        {
            ApplyAmount(onAmount);
            SetPower(true);
            hand = null;
        }
    }

    Vector3 HandPointLocal()
    {
        Vector3 world = hand.GetAttachTransform(interactable).position;
        return transform.parent != null ? transform.parent.InverseTransformPoint(world) : world;
    }

    void ApplyAmount(float value)
    {
        amount = value;
        if (motion == Motion.Rotate)
            transform.localRotation = startLocalRot * Quaternion.AngleAxis(amount, AxisVector);
        else
            transform.localPosition = startLocalPos + startLocalRot * AxisVector * amount;
    }

    void SetPower(bool on)
    {
        IsOn = on;

        SetLampGlow(redLamp, !on);
        SetLampGlow(greenLamp, on);
        if (statusLight != null) statusLight.color = on ? Color.green : Color.red;

        foreach (GameObject g in turnOnWithPower) if (g != null) g.SetActive(on);
        foreach (GameObject g in turnOffWithPower) if (g != null) g.SetActive(!on);

        if (on) onPowerOn.Invoke();
    }

    // Turns a lamp's glow on or off without changing its material.
    // Uses the emission color already set on the lamp's material.
    static void SetLampGlow(Renderer lamp, bool glow)
    {
        if (lamp == null || lamp.sharedMaterial == null) return;

        Color glowColor = lamp.sharedMaterial.HasProperty(EmissionColor)
            ? lamp.sharedMaterial.GetColor(EmissionColor)
            : Color.white;

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        lamp.GetPropertyBlock(block);
        block.SetColor(EmissionColor, glow ? glowColor : Color.black);
        lamp.SetPropertyBlock(block);
    }
}