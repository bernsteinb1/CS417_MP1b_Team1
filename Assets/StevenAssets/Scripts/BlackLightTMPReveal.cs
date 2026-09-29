using UnityEngine;
using TMPro;

public class BlacklightTMPReveal : MonoBehaviour
{
    public TMP_Text text;
    public BlacklightEmitter blacklight;

    [Range(0f, 1f)]
    public float visibleAlpha = 1f;

    [Range(0f, 1f)]
    public float hiddenAlpha = 0f;

    private void Awake()
    {
        if (text == null)
            text = GetComponent<TMP_Text>();

        SetAlpha(hiddenAlpha);
    }

    private void LateUpdate()
    {
        if (text == null || blacklight == null)
            return;

        Transform source =
            blacklight.beamOrigin != null
            ? blacklight.beamOrigin
            : blacklight.transform;

        Vector3 toText = transform.position - source.position;
        float distance = toText.magnitude;

        if (distance > blacklight.range)
        {
            SetAlpha(hiddenAlpha);
            return;
        }

        Vector3 directionToText = toText.normalized;

        float dot = Vector3.Dot(
            source.forward.normalized,
            directionToText
        );

        bool insideCone = dot >= blacklight.coneDot;

        SetAlpha(insideCone ? visibleAlpha : hiddenAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }
}