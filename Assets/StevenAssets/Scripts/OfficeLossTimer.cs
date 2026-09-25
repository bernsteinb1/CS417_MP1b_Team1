using TMPro;
using UnityEngine;

public class OfficeLossTimer : MonoBehaviour
{
    [Header("Timer")]
    public float totalSeconds = 480f;

    [Header("Displays")]
    public TMP_Text timerText;      // Wall/HUD timer
    public TMP_Text pcTimerText;    // PC monitor timer

    [Header("Game Logic")]
    public OfficeProgressManager progressManager;
    public InteractiveStatePuzzle puzzle;

    private float remaining;
    private bool expired;

    public float RemainingSeconds => remaining;

    private void Start()
    {
        remaining = totalSeconds;
        UpdateLabel();
    }

    private void Update()
    {
        if (expired)
            return;

        // Player completed the room before time expired.
        if (progressManager != null &&
            progressManager.IsComplete)
        {
            SetBothDisplays(
                "ESCAPED"
            );

            enabled = false;
            return;
        }

        // Count down.
        remaining =
            Mathf.Max(
                0f,
                remaining - Time.deltaTime
            );

        UpdateLabel();

        // Time ran out.
        if (remaining <= 0f)
        {
            ExpireTimer();
        }
    }

    private void UpdateLabel()
    {
        int seconds =
            Mathf.CeilToInt(remaining);

        int minutes =
            seconds / 60;

        int secs =
            seconds % 60;

        string label =
            $"SECURITY RESPONSE\n" +
            $"{minutes:00}:{secs:00}";

        SetBothDisplays(label);
    }

    private void ExpireTimer()
    {
        if (expired)
            return;

        expired = true;

        SetBothDisplays(
            "LOCKDOWN\n00:00"
        );

        if (puzzle != null)
        {
            puzzle.SetExpired();
        }

        Debug.Log(
            "Office loss timer expired."
        );
    }

    private void SetBothDisplays(
        string text)
    {
        if (timerText != null)
        {
            timerText.text = text;
        }

        if (pcTimerText != null)
        {
            pcTimerText.text = text;
        }
    }
}