using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GoldenKeycard : MonoBehaviour
{
    public XRGrabInteractable grab;
    public Rigidbody body;
    public bool DetachOnFirstGrab = true;

    [Header("Gold pulse")]
    public Color baseGold = new Color(1f, 0.55f, 0.05f, 1f);
    public Color emissionGold = new Color(1f, 0.36f, 0.015f, 1f);
    public float pulseSpeed = 5f;
    public float minEmission = 1.25f;
    public float maxEmission = 2.0f;

    private bool detached;
    private readonly List<Material> runtimeMaterials = new List<Material>();

    private void Awake()
    {
        if (grab == null)
            grab = GetComponent<XRGrabInteractable>();

        if (body == null)
            body = GetComponent<Rigidbody>();

        ApplyGoldPulseNow();
    }

    private void OnEnable()
    {
        if (grab == null)
            grab = GetComponent<XRGrabInteractable>();

        if (grab != null)
        {
            grab.selectEntered.AddListener(OnSelected);
            grab.selectExited.AddListener(OnReleased);
        }
    }

    private void OnDisable()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnSelected);
            grab.selectExited.RemoveListener(OnReleased);
        }
    }

    public void ApplyGoldPulseNow()
    {
        runtimeMaterials.Clear();

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
        {
            Material[] mats = renderer.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                Material mat = mats[i];
                if (mat == null)
                    continue;

                // renderer.materials already gives this instance its own
                // material copies, so changing these does not recolor the
                // prefab asset or every other card in the project.
                if (mat.HasProperty("_BaseColor"))
                    mat.SetColor("_BaseColor", baseGold);
                else if (mat.HasProperty("_Color"))
                    mat.SetColor("_Color", baseGold);

                if (mat.HasProperty("_EmissionColor"))
                {
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", emissionGold * minEmission);
                }

                runtimeMaterials.Add(mat);
            }
        }
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        if (DetachOnFirstGrab && !detached)
        {
            detached = true;
            transform.SetParent(null, true);

            DontDestroyOnLoad(gameObject);
        }

        MakeDynamic();
    }
    
    private void OnReleased(SelectExitEventArgs args)
    {
        // XRI may temporarily alter rigidbody state while an item is selected.
        // Force normal loose-object physics back on at release so the card
        // falls to the floor instead of floating with the plunger.
        transform.SetParent(null, true);
        MakeDynamic();
    }

    private void MakeDynamic()
    {
        if (body == null)
            body = GetComponent<Rigidbody>();

        if (body == null)
            return;

        body.isKinematic = false;
        body.useGravity = true;
    }

    private void Update()
    {
        if (runtimeMaterials.Count == 0)
            return;

        float t = 0.5f + 0.5f * Mathf.Sin(Time.time * pulseSpeed);
        float strength = Mathf.Lerp(minEmission, maxEmission, t);
        Color pulseColor = emissionGold * strength;

        foreach (Material mat in runtimeMaterials)
        {
            if (mat != null && mat.HasProperty("_EmissionColor"))
                mat.SetColor("_EmissionColor", pulseColor);
        }
    }
}
