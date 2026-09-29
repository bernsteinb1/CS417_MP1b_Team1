using System.Collections;
using UnityEngine;

public class RelevantFeedbackCue : MonoBehaviour
{
    [Header("Sound")]
    public float frequency = 720f;
    public float duration = 0.12f;
    [Range(0f, 1f)]
    public float volume = 0.18f;

    [Header("Optional Visual")]
    public Renderer flashRenderer;
    public float flashDuration = 0.20f;
    public Color flashColor = Color.green;

    private AudioSource source;
    private Material materialInstance;
    private Color originalEmission;
    private AudioClip clip;

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 1f;
        source.playOnAwake = false;
        source.volume = volume;

        MakeBeep();

        if (flashRenderer != null)
        {
            materialInstance = flashRenderer.material;

            if (materialInstance.HasProperty("_EmissionColor"))
            {
                originalEmission =
                    materialInstance.GetColor("_EmissionColor");

                materialInstance.EnableKeyword("_EMISSION");
            }
        }
    }

    private void MakeBeep()
    {
        int sampleRate = 44100;
        int sampleCount =
            Mathf.CeilToInt(sampleRate * duration);

        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = i / (float)sampleRate;

            // Small fade-out to avoid clicking.
            float envelope = 1f - i / (float)sampleCount;

            samples[i] =
                Mathf.Sin(2f * Mathf.PI * frequency * t)
                * envelope
                * 0.35f;
        }

        clip = AudioClip.Create(
            "SuccessBeep",
            sampleCount,
            1,
            sampleRate,
            false
        );

        clip.SetData(samples, 0);
        source.clip = clip;
    }

    public void PlaySuccess()
    {
        source.Stop();
        source.Play();

        if (materialInstance != null &&
            materialInstance.HasProperty("_EmissionColor"))
        {
            StopAllCoroutines();
            StartCoroutine(FlashRoutine());
        }
    }

    private IEnumerator FlashRoutine()
    {
        materialInstance.SetColor(
            "_EmissionColor",
            flashColor * 2.5f
        );

        yield return new WaitForSeconds(flashDuration);

        materialInstance.SetColor(
            "_EmissionColor",
            originalEmission
        );
    }
}