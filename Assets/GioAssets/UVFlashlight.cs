using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Put this on the Flashlight (the same object that has XR Grab Interactable).
// While holding the flashlight, press the trigger to switch between normal and UV light.
// Anything using the "Custom/UVReveal" material only shows up where the UV beam hits it.
[RequireComponent(typeof(XRGrabInteractable))]
public class UVFlashlight : MonoBehaviour
{
    [Tooltip("The Spot Light on LightOrigin. Found automatically if left empty.")]
    public Light flashlight;

    [Header("UV mode")]
    public Color uvColor = new Color(0.45f, 0.1f, 1f);   // purple
    [Tooltip("UV light is usually dimmer than the normal beam. 1 = same brightness.")]
    [Range(0.1f, 2f)] public float uvIntensityMultiplier = 0.6f;
    public bool startInUV = false;

    [Header("Sounds")]
    [Tooltip("Plays when switching to UV. Also used for switching back if the next slot is empty.")]
    public AudioClip switchToUVSound;
    [Tooltip("Optional: a different sound for switching back to normal.")]
    public AudioClip switchToNormalSound;
    [Range(0f, 1f)] public float volume = 0.8f;

    public bool IsUV { get; private set; }

    XRGrabInteractable grab;
    AudioSource audioSource;
    Color normalColor;
    float normalIntensity;

    static readonly int UVLightPos = Shader.PropertyToID("_UVLightPos");
    static readonly int UVLightDir = Shader.PropertyToID("_UVLightDir");
    static readonly int UVLightOn = Shader.PropertyToID("_UVLightOn");

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        if (flashlight == null) flashlight = GetComponentInChildren<Light>();
        normalColor = flashlight.color;
        normalIntensity = flashlight.intensity;

        // Sound comes from the flashlight itself, so it's heard in the player's hand.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 0.5f;
    }

    void OnEnable()
    {
        grab.activated.AddListener(OnTriggerPressed);
    }

    void OnDisable()
    {
        grab.activated.RemoveListener(OnTriggerPressed);
        Shader.SetGlobalFloat(UVLightOn, 0f);
    }

    void Start()
    {
        SetUV(startInUV, false);
    }

    void OnTriggerPressed(ActivateEventArgs args)
    {
        SetUV(!IsUV, true);
    }

    public void SetUV(bool uv)
    {
        SetUV(uv, true);
    }

    void SetUV(bool uv, bool playSound)
    {
        IsUV = uv;
        flashlight.color = uv ? uvColor : normalColor;
        flashlight.intensity = uv ? normalIntensity * uvIntensityMultiplier : normalIntensity;

        if (!playSound) return;
        AudioClip clip = uv ? switchToUVSound : (switchToNormalSound != null ? switchToNormalSound : switchToUVSound);
        if (clip != null) audioSource.PlayOneShot(clip, volume);
    }

    void LateUpdate()
    {
        Transform t = flashlight.transform;
        float cosHalfAngle = Mathf.Cos(flashlight.spotAngle * 0.5f * Mathf.Deg2Rad);

        Shader.SetGlobalVector(UVLightPos, new Vector4(t.position.x, t.position.y, t.position.z, flashlight.range));
        Shader.SetGlobalVector(UVLightDir, new Vector4(t.forward.x, t.forward.y, t.forward.z, cosHalfAngle));

        bool shining = IsUV && flashlight.enabled && flashlight.gameObject.activeInHierarchy;
        Shader.SetGlobalFloat(UVLightOn, shining ? 1f : 0f);
    }
}
