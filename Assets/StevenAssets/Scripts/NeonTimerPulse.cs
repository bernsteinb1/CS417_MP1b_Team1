using TMPro;
using UnityEngine;

public class NeonTimerPulse : MonoBehaviour
{
    public TMP_Text text;
    public float lowTimePulseSpeed = 4f;
    public float lowTimeThreshold = 60f;
    public OfficeLossTimer timer;
    public Color baseColor = new Color(1f, 0.04f, 0.05f, 1f);
    public Color alarmColor = new Color(1f, 0.45f, 0.45f, 1f);
    public float subtleFlicker = 0.06f;

    void Update()
    {
        if (text == null) return;

        float pulse = 1f;
        float alpha = 0.96f - Mathf.PerlinNoise(Time.time * 8f, 0.13f) * subtleFlicker;

        if (timer != null && timer.RemainingSeconds <= lowTimeThreshold)
        {
            pulse = (Mathf.Sin(Time.time * lowTimePulseSpeed) + 1f) * 0.5f;
            alpha = Mathf.Lerp(0.55f, 1f, pulse);
        }

        text.color = Color.Lerp(baseColor, alarmColor, pulse * 0.55f);
        var c = text.color; c.a = alpha; text.color = c;
        text.transform.localScale = Vector3.one * Mathf.Lerp(1f, 1.06f, pulse * 0.4f);
    }
}
